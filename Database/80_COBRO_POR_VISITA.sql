/*
=============================================
SCRIPT 80: Cobro POR VISITA (feature/cobro-por-visita)
AUTOR: Sistema PetsHome
FECHA: 2026-10-28
DESCRIPCIÓN:
  Rediseña el cobro de medicamentos dispensados para que sea POR VISITA (cita),
  nunca deuda flotante cross-visita por cliente. Regla OpenVPM (replicamos regla,
  no código):
    - Cada cargo ya se ata a la CITA (cita_Id). ConsultaService ya lo encola con el
      cita_Id real; el seed legacy lo dejó NULL (ver script 81 para corregir demo).
    - Se factura/resuelve SOLO los cargos de ESA visita con una DISPOSICIÓN:
        'Pagado'    -> factura de la visita, estado 'Pagada'.
        'PorCobrar' -> factura con fecha de vencimiento, estado 'PorCobrar' (A/R).
        'SinCargo'  -> cargos condonados CON MOTIVO (sin factura).
    - Una factura por visita; nunca se agrupan cargos de varias visitas.

  CAMBIOS:
    1. tbFacturas += fact_Vencimiento DATE NULL (A/R formal).
       Estados usados (varchar, SIN CHECK en la tabla -> no hay constraint que tocar):
       'Emitida' | 'Pagada' | 'PorCobrar' | 'Anulada' (sin tildes).
    2. PR_Veterinaria_Facturas_Find += fact_Vencimiento en el SELECT.
    3. PR_Veterinaria_CargoDispensacion_FacturarVisita  (nuevo).
    4. PR_Veterinaria_CargoDispensacion_ListPendientesPorVisita  (nuevo).

  Patrón de escritura: CodeStatus/MessageStatus (Success = CodeStatus>0);
  los que crean factura devuelven también fact_Id. ISV 15% (igual que scripts 43/66).
  Requiere scripts 43, 65, 66.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

-- =============================================
-- PASO 1: tbFacturas += fact_Vencimiento
-- =============================================
IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID('Veterinaria.tbFacturas') AND name = 'fact_Vencimiento')
BEGIN
    ALTER TABLE [Veterinaria].[tbFacturas] ADD fact_Vencimiento DATE NULL
    PRINT '✅ Columna fact_Vencimiento agregada a tbFacturas'
END
ELSE
    PRINT '⚠️ La columna fact_Vencimiento ya existe'
GO

-- =============================================
-- PASO 2: Facturas_Find expone fact_Vencimiento (comprobante A/R)
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Facturas_Find]
    @fact_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        f.fact_Id,
        f.fact_Numero,
        f.cli_Id,
        LTRIM(RTRIM(ISNULL(c.cli_Nombres,'') + ' ' + ISNULL(c.cli_Apellidos,''))) AS Cliente,
        c.cli_Identidad,
        f.masc_Id,
        m.masc_Nombre AS Mascota,
        f.fact_Fecha,
        f.fact_Subtotal,
        f.fact_Impuesto,
        f.fact_Total,
        f.fact_Estado,
        f.fact_Vencimiento
    FROM [Veterinaria].[tbFacturas] f
    LEFT JOIN [Veterinaria].[tbClientes] c ON f.cli_Id = c.cli_Id
    LEFT JOIN [Refugio].[tbMascotas] m ON f.masc_Id = m.masc_Id
    WHERE f.fact_Id = @fact_Id AND f.fact_EsEliminado = 0
END
GO

-- =============================================
-- PASO 3: Facturar/resolver UNA visita (cita) según disposición
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_FacturarVisita]
    @cita_Id      INT,
    @disposicion  NVARCHAR(20),              -- 'Pagado' | 'PorCobrar' | 'SinCargo' (sin tildes)
    @vencimiento  DATE = NULL,               -- requerido si 'PorCobrar'
    @motivo       NVARCHAR(1000) = NULL,     -- requerido (>=5) si 'SinCargo'
    @usuario      INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        -- Validaciones de entrada ------------------------------------------------
        IF @cita_Id IS NULL OR @cita_Id <= 0
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -2 AS CodeStatus, 'Visita (cita) inválida.' AS MessageStatus
            RETURN
        END
        IF @disposicion NOT IN ('Pagado','PorCobrar','SinCargo')
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -2 AS CodeStatus, 'Disposición inválida (Pagado/PorCobrar/SinCargo).' AS MessageStatus
            RETURN
        END
        IF @disposicion = 'PorCobrar' AND @vencimiento IS NULL
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -3 AS CodeStatus, 'Por cobrar requiere fecha de vencimiento.' AS MessageStatus
            RETURN
        END
        IF @disposicion = 'SinCargo' AND (@motivo IS NULL OR LEN(@motivo) < 5)
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -4 AS CodeStatus, 'Sin cargo requiere un motivo de al menos 5 caracteres.' AS MessageStatus
            RETURN
        END

        -- Debe haber cargos pendientes en ESA visita -----------------------------
        IF NOT EXISTS (SELECT 1 FROM [Veterinaria].[tbCargoDispensacion]
                       WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0)
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -1 AS CodeStatus, 'La visita no tiene cargos pendientes.' AS MessageStatus
            RETURN
        END

        -- ── SinCargo: condonar los cargos de la visita (sin factura) ────────────
        IF @disposicion = 'SinCargo'
        BEGIN
            UPDATE [Veterinaria].[tbCargoDispensacion]
            SET cargo_Estado = 'Condonado',
                cargo_ResueltoPor = @usuario, cargo_FechaResuelto = GETDATE(),
                cargo_MotivoResolucion = @motivo,
                cargo_UsuarioModifica = @usuario, cargo_FechaModifica = GETDATE()
            WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

            SELECT CAST(NULL AS INT) AS fact_Id, 1 AS CodeStatus, 'Cargos de la visita condonados (sin cargo).' AS MessageStatus
            RETURN
        END

        -- ── Pagado / PorCobrar: crear UNA factura con los cargos de la visita ───
        DECLARE @cli_Id INT, @masc_Id INT
        SELECT TOP 1 @cli_Id = cli_Id, @masc_Id = masc_Id
        FROM [Veterinaria].[tbCargoDispensacion]
        WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

        DECLARE @subtotal DECIMAL(18,2) =
            (SELECT SUM(cargo_Cantidad * cargo_PrecioUnitSnap)
             FROM [Veterinaria].[tbCargoDispensacion]
             WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0);

        -- ISV 15% (Honduras). Misma convención que scripts 43 y 66.
        DECLARE @impuesto DECIMAL(18,2) = ROUND(@subtotal * 15.00 / 100.0, 2);
        DECLARE @total    DECIMAL(18,2) = @subtotal + @impuesto;
        DECLARE @estado   VARCHAR(20)   = CASE WHEN @disposicion = 'Pagado' THEN 'Pagada' ELSE 'PorCobrar' END;
        DECLARE @venc     DATE          = CASE WHEN @disposicion = 'PorCobrar' THEN @vencimiento ELSE NULL END;

        BEGIN TRAN

        -- 1. Encabezado (factura de la visita, con su mascota de origen)
        INSERT INTO [Veterinaria].[tbFacturas]
            (fact_Numero, cli_Id, masc_Id, refg_Id, fact_Fecha,
             fact_Subtotal, fact_Impuesto, fact_Total, fact_Estado, fact_Vencimiento,
             fact_EsEliminado, fact_UsuarioCrea, fact_FechaCrea)
        VALUES
            (NULL, @cli_Id, @masc_Id, NULL, GETDATE(),
             @subtotal, @impuesto, @total, @estado, @venc,
             0, @usuario, GETDATE())

        DECLARE @fact_Id INT = CAST(SCOPE_IDENTITY() AS INT)

        -- 2. Una línea por cargo pendiente de la visita + marcar Facturado
        DECLARE @cargo_Id INT, @itm_Id INT, @cant INT, @desc NVARCHAR(500), @precio DECIMAL(18,2), @fdet_Id INT

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT cargo_Id, itm_Id, cargo_Cantidad, cargo_DescripcionSnap, cargo_PrecioUnitSnap
            FROM [Veterinaria].[tbCargoDispensacion]
            WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

        OPEN cur
        FETCH NEXT FROM cur INTO @cargo_Id, @itm_Id, @cant, @desc, @precio
        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO [Veterinaria].[tbFacturaDetalle]
                (fact_Id, fdet_Tipo, serv_Id, itm_Id, fdet_Descripcion,
                 fdet_Cantidad, fdet_PrecioUnit, fdet_Subtotal)
            VALUES
                (@fact_Id, 'Producto', NULL, @itm_Id, @desc,
                 @cant, @precio, @cant * @precio)

            SET @fdet_Id = CAST(SCOPE_IDENTITY() AS INT)

            UPDATE [Veterinaria].[tbCargoDispensacion]
            SET cargo_Estado = 'Facturado',
                fact_Id = @fact_Id, fdet_Id = @fdet_Id,
                cargo_ResueltoPor = @usuario, cargo_FechaResuelto = GETDATE(),
                cargo_MotivoResolucion = NULL,
                cargo_UsuarioModifica = @usuario, cargo_FechaModifica = GETDATE()
            WHERE cargo_Id = @cargo_Id

            FETCH NEXT FROM cur INTO @cargo_Id, @itm_Id, @cant, @desc, @precio
        END
        CLOSE cur
        DEALLOCATE cur

        COMMIT

        SELECT @fact_Id AS fact_Id, 1 AS CodeStatus, 'Factura de la visita creada.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK
        SELECT CAST(NULL AS INT) AS fact_Id, -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =============================================
-- PASO 4: Listar pendientes agrupables POR VISITA (cita)
--         Una fila por cargo + columnas de agrupación (cita/mascota/cliente/fecha).
--         cita_Id NULL = legacy "Sin visita asignada" (se agrupa aparte en la UI).
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_ListPendientesPorVisita]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER (ORDER BY c.cita_Id, c.cargo_FechaCrea) AS Fila,
        c.cargo_Id, c.receta_Id, c.masc_Id, c.cli_Id, c.cita_Id, c.itm_Id,
        c.cargo_Cantidad, c.cargo_DescripcionSnap, c.cargo_PrecioUnitSnap,
        (c.cargo_Cantidad * c.cargo_PrecioUnitSnap) AS cargo_Total,
        c.cargo_Estado, c.cargo_FechaCrea,
        m.masc_Nombre AS MascotaNombre,
        LTRIM(RTRIM(ISNULL(cl.cli_Nombres,'') + ' ' + ISNULL(cl.cli_Apellidos,''))) AS ClienteNombre,
        ci.cita_FechaConsulta AS CitaFecha,
        ci.cita_MotivoConsulta AS CitaMotivo
    FROM [Veterinaria].[tbCargoDispensacion] c
    LEFT JOIN [Refugio].[tbMascotas]  m  ON c.masc_Id = m.masc_Id
    LEFT JOIN [Veterinaria].[tbClientes] cl ON c.cli_Id = cl.cli_Id
    LEFT JOIN [Medico].[tbCitaMedica] ci ON c.cita_Id = ci.cita_Id
    WHERE c.cargo_EsEliminado = 0
      AND c.cargo_Estado = 'Pendiente'
    ORDER BY Fila
END
GO

PRINT '✅ Script 80: cobro por visita (fact_Vencimiento + FacturarVisita + ListPendientesPorVisita)'
GO
