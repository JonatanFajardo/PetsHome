-- =============================================================================
-- 84_PAGOS.sql
-- Registro de PAGOS sobre la factura del CLIENTE (cobro por la atencion de su
-- mascota). Espejo del modelo de OpenVPM (payments + invoices.paidAmount):
--   - Veterinaria.tbPagos: una fila por pago/abono recibido.
--   - Veterinaria.tbFacturas.fact_MontoPagado: monto pagado acumulado.
--   - SALDO = fact_Total - fact_MontoPagado (derivado, NO columna persistida).
-- Al registrar un pago: valida amount <= saldo (rechaza sobrepago), suma a
-- fact_MontoPagado y, si cubre el total, marca la factura 'Pagada'. Permite
-- ABONOS PARCIALES (la factura sigue 'PorCobrar' hasta saldar).
--
-- Idempotente. Requiere SET QUOTED_IDENTIFIER/ANSI_NULLS ON porque el bloque
-- reescribe PR_Veterinaria_CargoDispensacion_FacturarVisita, que toca
-- tbCargoDispensacion (indice filtrado que exige QUOTED_IDENTIFIER ON).
-- =============================================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

USE PETSHOMEDB;
GO

-- -----------------------------------------------------------------------------
-- 1) Tabla Veterinaria.tbPagos
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'tbPagos' AND schema_id = SCHEMA_ID('Veterinaria'))
BEGIN
    CREATE TABLE [Veterinaria].[tbPagos]
    (
        pago_Id              INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_Veterinaria_tbPagos PRIMARY KEY,
        fact_Id              INT NOT NULL,
        pago_Monto           DECIMAL(18,2) NOT NULL,
        pago_Metodo          NVARCHAR(20) NOT NULL,
        pago_RecibidoPor     INT NOT NULL,
        pago_Fecha           DATETIME NOT NULL
            CONSTRAINT DF_Veterinaria_tbPagos_Fecha DEFAULT (GETDATE()),
        pago_Referencia      NVARCHAR(160) NULL,
        pago_Notas           NVARCHAR(500) NULL,
        -- Auditoria
        pago_EsEliminado     BIT NOT NULL
            CONSTRAINT DF_Veterinaria_tbPagos_EsEliminado DEFAULT (0),
        pago_UsuarioCrea     INT NOT NULL,
        pago_FechaCrea       DATETIME NOT NULL
            CONSTRAINT DF_Veterinaria_tbPagos_FechaCrea DEFAULT (GETDATE()),
        pago_UsuarioModifica INT NULL,
        pago_FechaModifica   DATETIME NULL,
        CONSTRAINT FK_Veterinaria_tbPagos_Factura
            FOREIGN KEY (fact_Id) REFERENCES [Veterinaria].[tbFacturas](fact_Id),
        CONSTRAINT CK_Veterinaria_tbPagos_Metodo
            CHECK (pago_Metodo IN ('Efectivo','TarjetaCredito','TarjetaDebito','Cheque','Transferencia','Otro')),
        CONSTRAINT CK_Veterinaria_tbPagos_Monto
            CHECK (pago_Monto > 0)
    );

    CREATE INDEX IX_Veterinaria_tbPagos_Factura
        ON [Veterinaria].[tbPagos](fact_Id) WHERE pago_EsEliminado = 0;
END
GO

-- -----------------------------------------------------------------------------
-- 2) tbFacturas.fact_MontoPagado (monto pagado acumulado)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID('Veterinaria.tbFacturas') AND name = 'fact_MontoPagado')
BEGIN
    ALTER TABLE [Veterinaria].[tbFacturas]
        ADD fact_MontoPagado DECIMAL(18,2) NOT NULL
            CONSTRAINT DF_Veterinaria_tbFacturas_MontoPagado DEFAULT (0);
END
GO

-- -----------------------------------------------------------------------------
-- 3) PR_Veterinaria_Pago_Registrar: registra un pago/abono sobre una factura
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Pago_Registrar]
    @fact_Id         INT,
    @pago_Monto      DECIMAL(18,2),
    @pago_Metodo     NVARCHAR(20),
    @pago_Referencia NVARCHAR(160) = NULL,
    @pago_Notas      NVARCHAR(500) = NULL,
    @usuario         INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        -- Validaciones de entrada ------------------------------------------------
        DECLARE @total DECIMAL(18,2), @pagado DECIMAL(18,2), @estado VARCHAR(20)
        SELECT @total = fact_Total, @pagado = fact_MontoPagado, @estado = fact_Estado
        FROM [Veterinaria].[tbFacturas]
        WHERE fact_Id = @fact_Id AND fact_EsEliminado = 0

        IF @total IS NULL
        BEGIN
            SELECT CAST(NULL AS INT) AS pago_Id, CAST(0 AS DECIMAL(18,2)) AS nuevo_saldo,
                   CAST(NULL AS VARCHAR(20)) AS fact_Estado,
                   -1 AS CodeStatus, 'La factura no existe o fue eliminada.' AS MessageStatus
            RETURN
        END

        IF @estado IN ('Anulada','SinCargo')
        BEGIN
            SELECT CAST(NULL AS INT) AS pago_Id, (@total - @pagado) AS nuevo_saldo, @estado AS fact_Estado,
                   -2 AS CodeStatus, 'La factura no admite pagos en su estado actual.' AS MessageStatus
            RETURN
        END

        IF @pago_Monto IS NULL OR @pago_Monto <= 0
        BEGIN
            SELECT CAST(NULL AS INT) AS pago_Id, (@total - @pagado) AS nuevo_saldo, @estado AS fact_Estado,
                   -3 AS CodeStatus, 'El monto del pago debe ser mayor a cero.' AS MessageStatus
            RETURN
        END

        -- Normalizar metodo (default Efectivo si viene invalido/null)
        IF @pago_Metodo IS NULL OR @pago_Metodo NOT IN ('Efectivo','TarjetaCredito','TarjetaDebito','Cheque','Transferencia','Otro')
            SET @pago_Metodo = 'Efectivo'

        DECLARE @saldo DECIMAL(18,2) = @total - @pagado
        IF @pago_Monto > @saldo
        BEGIN
            SELECT CAST(NULL AS INT) AS pago_Id, @saldo AS nuevo_saldo, @estado AS fact_Estado,
                   -4 AS CodeStatus, 'El pago excede el saldo pendiente.' AS MessageStatus
            RETURN
        END

        BEGIN TRAN

        INSERT INTO [Veterinaria].[tbPagos]
            (fact_Id, pago_Monto, pago_Metodo, pago_RecibidoPor, pago_Fecha,
             pago_Referencia, pago_Notas, pago_EsEliminado, pago_UsuarioCrea, pago_FechaCrea)
        VALUES
            (@fact_Id, @pago_Monto, @pago_Metodo, @usuario, GETDATE(),
             @pago_Referencia, @pago_Notas, 0, @usuario, GETDATE())

        DECLARE @pago_Id INT = CAST(SCOPE_IDENTITY() AS INT)
        DECLARE @nuevoPagado DECIMAL(18,2) = @pagado + @pago_Monto
        DECLARE @nuevoEstado VARCHAR(20) = CASE WHEN @nuevoPagado >= @total THEN 'Pagada' ELSE @estado END

        UPDATE [Veterinaria].[tbFacturas]
        SET fact_MontoPagado = @nuevoPagado,
            fact_Estado = @nuevoEstado,
            fact_UsuarioModifica = @usuario, fact_FechaModifica = GETDATE()
        WHERE fact_Id = @fact_Id

        COMMIT

        SELECT @pago_Id AS pago_Id, (@total - @nuevoPagado) AS nuevo_saldo, @nuevoEstado AS fact_Estado,
               1 AS CodeStatus, 'Pago registrado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK
        SELECT CAST(NULL AS INT) AS pago_Id, CAST(0 AS DECIMAL(18,2)) AS nuevo_saldo,
               CAST(NULL AS VARCHAR(20)) AS fact_Estado,
               -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- -----------------------------------------------------------------------------
-- 4) PR_Veterinaria_Pago_ListPorFactura: pagos no eliminados de una factura
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Pago_ListPorFactura]
    @fact_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        p.pago_Id,
        p.pago_Monto,
        p.pago_Metodo,
        p.pago_Fecha,
        p.pago_RecibidoPor,
        u.Usu_Nombre AS RecibidoPorNombre,
        p.pago_Referencia,
        p.pago_Notas
    FROM [Veterinaria].[tbPagos] p
    LEFT JOIN [Seguridad].[tbUsuarios] u ON p.pago_RecibidoPor = u.usu_Id
    WHERE p.fact_Id = @fact_Id AND p.pago_EsEliminado = 0
    ORDER BY p.pago_Fecha ASC, p.pago_Id ASC
END
GO

-- -----------------------------------------------------------------------------
-- 5) Facturas_Find expone fact_MontoPagado y Saldo (para el comprobante)
-- -----------------------------------------------------------------------------
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
        f.fact_Vencimiento,
        f.fact_MontoPagado,
        (f.fact_Total - f.fact_MontoPagado) AS Saldo
    FROM [Veterinaria].[tbFacturas] f
    LEFT JOIN [Veterinaria].[tbClientes] c ON f.cli_Id = c.cli_Id
    LEFT JOIN [Refugio].[tbMascotas] m ON f.masc_Id = m.masc_Id
    WHERE f.fact_Id = @fact_Id AND f.fact_EsEliminado = 0
END
GO

-- -----------------------------------------------------------------------------
-- 6) Reescribe FacturarVisita: registra el pago en el MISMO checkout.
--    @pago_Metodo + @abono_Inicial nuevos. 'Pagado' -> pago por el total;
--    'PorCobrar' -> abono inicial opcional; 'SinCargo' -> sin pago (condona).
--    SET QUOTED_IDENTIFIER/ANSI_NULLS ON ya aplicados al inicio del script.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_FacturarVisita]
    @cita_Id       INT,
    @disposicion   NVARCHAR(20),              -- 'Pagado' | 'PorCobrar' | 'SinCargo' (sin tildes)
    @vencimiento   DATE = NULL,               -- requerido si 'PorCobrar'
    @motivo        NVARCHAR(1000) = NULL,     -- requerido (>=5) si 'SinCargo'
    @usuario       INT,
    @pago_Metodo   NVARCHAR(20) = NULL,       -- metodo del pago/abono (default Efectivo)
    @abono_Inicial DECIMAL(18,2) = NULL       -- abono inicial opcional (solo 'PorCobrar')
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        -- Validaciones de entrada ------------------------------------------------
        IF @cita_Id IS NULL OR @cita_Id <= 0
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -2 AS CodeStatus, 'Visita (cita) invalida.' AS MessageStatus
            RETURN
        END
        IF @disposicion NOT IN ('Pagado','PorCobrar','SinCargo')
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -2 AS CodeStatus, 'Disposicion invalida (Pagado/PorCobrar/SinCargo).' AS MessageStatus
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

        -- Normalizar metodo de pago (default Efectivo si viene invalido/null)
        IF @pago_Metodo IS NULL OR @pago_Metodo NOT IN ('Efectivo','TarjetaCredito','TarjetaDebito','Cheque','Transferencia','Otro')
            SET @pago_Metodo = 'Efectivo'

        -- -- SinCargo: condonar los cargos de la visita (sin factura) --------------
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

        -- -- Pagado / PorCobrar: crear UNA factura con los cargos de la visita -----
        DECLARE @cli_Id INT, @masc_Id INT
        SELECT TOP 1 @cli_Id = cli_Id, @masc_Id = masc_Id
        FROM [Veterinaria].[tbCargoDispensacion]
        WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

        DECLARE @subtotal DECIMAL(18,2) =
            (SELECT SUM(cargo_Cantidad * cargo_PrecioUnitSnap)
             FROM [Veterinaria].[tbCargoDispensacion]
             WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0);

        -- ISV 15% (Honduras). Misma convencion que scripts 43 y 66.
        DECLARE @impuesto DECIMAL(18,2) = ROUND(@subtotal * 15.00 / 100.0, 2);
        DECLARE @total    DECIMAL(18,2) = @subtotal + @impuesto;
        DECLARE @estado   VARCHAR(20)   = CASE WHEN @disposicion = 'Pagado' THEN 'Pagada' ELSE 'PorCobrar' END;
        DECLARE @venc     DATE          = CASE WHEN @disposicion = 'PorCobrar' THEN @vencimiento ELSE NULL END;

        -- Monto pagado en este checkout
        DECLARE @montoPagado DECIMAL(18,2) = 0;
        IF @disposicion = 'Pagado'
            SET @montoPagado = @total;
        ELSE IF @disposicion = 'PorCobrar' AND @abono_Inicial IS NOT NULL AND @abono_Inicial > 0
        BEGIN
            -- El abono no puede exceder el total de la visita
            IF @abono_Inicial > @total
            BEGIN
                SELECT CAST(NULL AS INT) AS fact_Id, -6 AS CodeStatus, 'El abono inicial excede el total de la visita.' AS MessageStatus
                RETURN
            END
            SET @montoPagado = @abono_Inicial;
        END

        -- Si el abono cubre el total, la factura queda saldada
        IF @montoPagado >= @total AND @total > 0
            SET @estado = 'Pagada';

        BEGIN TRAN

        -- 1. Encabezado (factura de la visita, con su mascota de origen)
        INSERT INTO [Veterinaria].[tbFacturas]
            (fact_Numero, cli_Id, masc_Id, refg_Id, fact_Fecha,
             fact_Subtotal, fact_Impuesto, fact_Total, fact_Estado, fact_Vencimiento,
             fact_MontoPagado, fact_EsEliminado, fact_UsuarioCrea, fact_FechaCrea)
        VALUES
            (NULL, @cli_Id, @masc_Id, NULL, GETDATE(),
             @subtotal, @impuesto, @total, @estado, @venc,
             @montoPagado, 0, @usuario, GETDATE())

        DECLARE @fact_Id INT = CAST(SCOPE_IDENTITY() AS INT)

        -- 2. Una linea por cargo pendiente de la visita + marcar Facturado
        DECLARE @cargo_Id INT, @itm_Id INT, @cant INT, @desc NVARCHAR(500), @precio DECIMAL(18,2), @fdet_Id INT
        DECLARE @tipoOrigen NVARCHAR(20), @origenId INT

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT cargo_Id, itm_Id, cargo_Cantidad, cargo_DescripcionSnap, cargo_PrecioUnitSnap,
                   cargo_TipoOrigen, cargo_OrigenId
            FROM [Veterinaria].[tbCargoDispensacion]
            WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

        OPEN cur
        FETCH NEXT FROM cur INTO @cargo_Id, @itm_Id, @cant, @desc, @precio, @tipoOrigen, @origenId
        WHILE @@FETCH_STATUS = 0
        BEGIN
            -- Mapea el cargo a su linea de factura: Servicio (vacuna/lab/procedimiento)
            -- usa serv_Id; Producto (medicamento) usa itm_Id. La regla CK exige exactamente uno.
            IF @tipoOrigen = 'Servicio'
                INSERT INTO [Veterinaria].[tbFacturaDetalle]
                    (fact_Id, fdet_Tipo, serv_Id, itm_Id, fdet_Descripcion,
                     fdet_Cantidad, fdet_PrecioUnit, fdet_Subtotal)
                VALUES
                    (@fact_Id, 'Servicio', @origenId, NULL, @desc,
                     @cant, @precio, @cant * @precio)
            ELSE
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

            FETCH NEXT FROM cur INTO @cargo_Id, @itm_Id, @cant, @desc, @precio, @tipoOrigen, @origenId
        END
        CLOSE cur
        DEALLOCATE cur

        -- 3. Registrar el pago del checkout (si hubo monto pagado)
        IF @montoPagado > 0
        BEGIN
            INSERT INTO [Veterinaria].[tbPagos]
                (fact_Id, pago_Monto, pago_Metodo, pago_RecibidoPor, pago_Fecha,
                 pago_Referencia, pago_Notas, pago_EsEliminado, pago_UsuarioCrea, pago_FechaCrea)
            VALUES
                (@fact_Id, @montoPagado, @pago_Metodo, @usuario, GETDATE(),
                 NULL, 'Pago en checkout de la visita.', 0, @usuario, GETDATE())
        END

        COMMIT

        SELECT @fact_Id AS fact_Id, 1 AS CodeStatus, 'Factura de la visita creada.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK
        SELECT CAST(NULL AS INT) AS fact_Id, -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO
