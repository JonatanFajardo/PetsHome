/*
=============================================================================
 99_FACTURA_DESCUENTOS_IMPUESTOS.sql
 Descuentos / ajustes en facturacion + impuesto por item (exentos) +
 nota de credito / reembolso simple. Replica la facturacion de OpenVPM.

 QUE replicamos de OpenVPM (apps/web, packages/db/schema/billing.ts):
   - services.taxable / products.taxable  -> aqui serv_Exento / itm_Exento
     (exento = lo contrario de taxable). Se GUARDA una FOTO del valor en la
     linea de factura (fdet_Exento), inmutable: cambiar el catalogo nunca
     reescribe facturas viejas (igual que invoice_items.taxable de OpenVPM).
   - El impuesto se calcula UNA sola vez a nivel factura, SOLO sobre las
     lineas NO exentas (calculateInvoiceTaxTotals en invoice-tax.ts).
   - Credito / cancelacion de saldo / reembolso: OpenVPM lo modela como
     "invoice_adjustments" (type credit|write_off) con motivo, clave de
     idempotencia y saldo resultante. Aqui: Veterinaria.tbFacturaAjustes
     (tipo NotaCredito|Reembolso) + columna rollup fact_Ajuste; el saldo pasa
     a ser Total - Pagado - Ajuste en todos los listados.

 DECISION de orden de calculo (documentada, defendible y compatible):
     fdet_Subtotal = Cantidad*PrecioUnit - fdet_Descuento        (nunca < 0)
     Subtotal      = SUMA(fdet_Subtotal)
     BaseGravable  = SUMA(fdet_Subtotal de lineas NO exentas)
     Impuesto      = ROUND(BaseGravable * tasa/100, 2)
     Descuento fac = fact_Descuento (acotado a [0, Subtotal])
     Total         = Subtotal - fact_Descuento + Impuesto         (nunca < 0)
   El descuento por factura es un ajuste global (cortesia) que resta del
   total despues del impuesto, como el credit de OpenVPM. Todo en 0 por
   defecto => el comportamiento es identico al actual (backward compatible).

 Idempotente (COL_LENGTH / IF NOT EXISTS / CREATE OR ALTER). Textos sin tildes.
=============================================================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

-- =============================================================================
-- PASO 1: Flag de EXENTO en catalogos (servicio / item)
--         exento = NO causa impuesto (equivale a taxable=false de OpenVPM)
-- =============================================================================
IF COL_LENGTH('Veterinaria.tbServicios', 'serv_Exento') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbServicios]
        ADD serv_Exento BIT NOT NULL CONSTRAINT DF_serv_Exento DEFAULT 0;
    PRINT 'serv_Exento agregado a tbServicios';
END
ELSE PRINT 'serv_Exento ya existia';
GO

IF COL_LENGTH('Inventario.tbItems', 'itm_Exento') IS NULL
BEGIN
    ALTER TABLE [Inventario].[tbItems]
        ADD itm_Exento BIT NOT NULL CONSTRAINT DF_itm_Exento DEFAULT 0;
    PRINT 'itm_Exento agregado a tbItems';
END
ELSE PRINT 'itm_Exento ya existia';
GO

-- =============================================================================
-- PASO 2: Descuento y FOTO de exento POR LINEA de factura
-- =============================================================================
IF COL_LENGTH('Veterinaria.tbFacturaDetalle', 'fdet_Descuento') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbFacturaDetalle]
        ADD fdet_Descuento DECIMAL(18,2) NOT NULL CONSTRAINT DF_fdet_Descuento DEFAULT 0;
    PRINT 'fdet_Descuento agregado a tbFacturaDetalle';
END
ELSE PRINT 'fdet_Descuento ya existia';
GO

IF COL_LENGTH('Veterinaria.tbFacturaDetalle', 'fdet_Exento') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbFacturaDetalle]
        ADD fdet_Exento BIT NOT NULL CONSTRAINT DF_fdet_Exento DEFAULT 0;
    PRINT 'fdet_Exento agregado a tbFacturaDetalle';
END
ELSE PRINT 'fdet_Exento ya existia';
GO

-- =============================================================================
-- PASO 3: Descuento de factura + rollup de ajustes (creditos/reembolsos)
-- =============================================================================
IF COL_LENGTH('Veterinaria.tbFacturas', 'fact_Descuento') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbFacturas]
        ADD fact_Descuento DECIMAL(18,2) NOT NULL CONSTRAINT DF_fact_Descuento DEFAULT 0;
    PRINT 'fact_Descuento agregado a tbFacturas';
END
ELSE PRINT 'fact_Descuento ya existia';
GO

IF COL_LENGTH('Veterinaria.tbFacturas', 'fact_Ajuste') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbFacturas]
        ADD fact_Ajuste DECIMAL(18,2) NOT NULL CONSTRAINT DF_fact_Ajuste DEFAULT 0;
    PRINT 'fact_Ajuste (rollup de creditos/reembolsos) agregado a tbFacturas';
END
ELSE PRINT 'fact_Ajuste ya existia';
GO

-- =============================================================================
-- PASO 4: Libro de ajustes (nota de credito / reembolso) estilo OpenVPM
--         type credit|write_off -> tipo NotaCredito|Reembolso
--         operationKey -> faj_ClaveOperacion (idempotencia)
--         balanceAfter -> faj_SaldoDespues
-- =============================================================================
IF OBJECT_ID('Veterinaria.tbFacturaAjustes', 'U') IS NULL
BEGIN
    CREATE TABLE [Veterinaria].[tbFacturaAjustes] (
        faj_Id            INT IDENTITY(1,1) PRIMARY KEY,
        fact_Id           INT NOT NULL,
        faj_Tipo          NVARCHAR(20) NOT NULL,   -- 'NotaCredito' (condona saldo) | 'Reembolso' (devuelve pagado)
        faj_Monto         DECIMAL(18,2) NOT NULL,
        faj_Motivo        NVARCHAR(500) NULL,
        faj_SaldoDespues  DECIMAL(18,2) NULL,
        faj_ClaveOperacion NVARCHAR(160) NULL,
        faj_EsEliminado   BIT NOT NULL CONSTRAINT DF_faj_EsEliminado DEFAULT 0,
        faj_UsuarioCrea   INT NOT NULL,
        faj_FechaCrea     DATETIME NOT NULL CONSTRAINT DF_faj_FechaCrea DEFAULT GETDATE(),
        faj_UsuarioModifica INT NULL,
        faj_FechaModifica   DATETIME NULL,
        CONSTRAINT FK_tbFacturaAjustes_Factura FOREIGN KEY (fact_Id)
            REFERENCES [Veterinaria].[tbFacturas](fact_Id),
        CONSTRAINT CK_tbFacturaAjustes_Tipo CHECK (faj_Tipo IN ('NotaCredito','Reembolso')),
        CONSTRAINT CK_tbFacturaAjustes_Monto CHECK (faj_Monto > 0)
    );
    PRINT 'tbFacturaAjustes creada';
END
ELSE PRINT 'tbFacturaAjustes ya existia';
GO

-- Clave de operacion unica (idempotencia de reintentos), solo no nulas/no borradas
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_tbFacturaAjustes_Clave'
               AND object_id = OBJECT_ID('Veterinaria.tbFacturaAjustes'))
BEGIN
    CREATE UNIQUE INDEX UQ_tbFacturaAjustes_Clave
        ON [Veterinaria].[tbFacturaAjustes](faj_ClaveOperacion)
        WHERE faj_ClaveOperacion IS NOT NULL AND faj_EsEliminado = 0;
    PRINT 'Indice unico de clave de operacion creado';
END
GO

-- =============================================================================
-- PASO 5: Dropdowns del POS llevan el flag de exento
-- =============================================================================

-- 5a. Productos vendibles (ya era SP de facturacion) + itm_Exento
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_ProductosVendibles_Dropdown]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        i.itm_Id,
        i.itm_Descripcion AS Descripcion,
        i.itm_Precio,
        i.itm_Exento
    FROM [Inventario].[tbItems] i
    WHERE i.itm_EsEliminado = 0
      AND i.itm_EsVendible = 1
    ORDER BY i.itm_Descripcion
END
GO

-- 5b. Servicios para el POS (SP propio de facturacion, no toca el de Consulta)
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Servicio_DropdownFactura]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        s.serv_Id,
        s.serv_Nombre,
        s.serv_Precio,
        s.serv_Categoria,
        s.serv_Exento
    FROM [Veterinaria].[tbServicios] s
    WHERE s.serv_EsEliminado = 0
      AND s.serv_EsActivo = 1
    ORDER BY s.serv_Categoria, s.serv_Nombre
END
GO

-- =============================================================================
-- PASO 6: Crear factura (POS) con descuento por linea, descuento de factura
--         y exento por item. Recalcula todo en el servidor (no confia en el
--         cliente): el exento se FOTOGRAFIA del catalogo por serv_Id/itm_Id.
-- =============================================================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Factura_Crear]
    @cli_Id INT = NULL,                 -- NULL -> Consumidor Final
    @masc_Id INT = NULL,
    @refg_Id INT = NULL,
    @fact_Numero VARCHAR(40) = NULL,
    @impuestoPorc DECIMAL(5,2) = 15.00, -- ISV
    @usuarioCrea INT,
    @fact_Descuento DECIMAL(18,2) = 0,  -- descuento/ajuste a nivel factura
    @detalleJson NVARCHAR(MAX)          -- [{"tipo","serv_Id","itm_Id","descripcion","cantidad","precioUnit","descuento","exento"}]
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRANSACTION

        IF @cli_Id IS NULL
            SET @cli_Id = (SELECT TOP 1 cli_Id FROM [Veterinaria].[tbClientes]
                           WHERE cli_EsConsumidorFinal = 1 AND cli_EsEliminado = 0)

        -- Parsear lineas (descuento/exento son opcionales en el JSON)
        DECLARE @det TABLE (
            tipo VARCHAR(20),
            serv_Id INT NULL,
            itm_Id INT NULL,
            descripcion VARCHAR(250),
            cantidad DECIMAL(18,2),
            precioUnit DECIMAL(18,2),
            descuento DECIMAL(18,2),
            exentoJson BIT NULL
        )
        INSERT INTO @det (tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit, descuento, exentoJson)
        SELECT tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit,
               ISNULL(descuento, 0), exento
        FROM OPENJSON(@detalleJson)
        WITH (
            tipo VARCHAR(20)         '$.tipo',
            serv_Id INT              '$.serv_Id',
            itm_Id INT               '$.itm_Id',
            descripcion VARCHAR(250) '$.descripcion',
            cantidad DECIMAL(18,2)   '$.cantidad',
            precioUnit DECIMAL(18,2) '$.precioUnit',
            descuento DECIMAL(18,2)  '$.descuento',
            exento BIT               '$.exento'
        )

        IF NOT EXISTS (SELECT 1 FROM @det)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1 AS CodeStatus, 'La factura no tiene lineas.' AS MessageStatus
            RETURN
        END

        -- Normalizar: descuento de linea no negativo ni mayor al bruto; foto de exento.
        -- El exento se toma del catalogo (autoridad); si no hay serv/itm, usa el del JSON o 0.
        DECLARE @linea TABLE (
            tipo VARCHAR(20), serv_Id INT NULL, itm_Id INT NULL, descripcion VARCHAR(250),
            cantidad DECIMAL(18,2), precioUnit DECIMAL(18,2), descuento DECIMAL(18,2),
            exento BIT, subtotal DECIMAL(18,2)
        )
        INSERT INTO @linea (tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit, descuento, exento, subtotal)
        SELECT
            d.tipo, d.serv_Id, d.itm_Id, d.descripcion, d.cantidad, d.precioUnit,
            -- descuento acotado a [0, bruto]
            CASE WHEN ISNULL(d.descuento,0) < 0 THEN 0
                 WHEN ISNULL(d.descuento,0) > (d.cantidad * d.precioUnit) THEN (d.cantidad * d.precioUnit)
                 ELSE d.descuento END AS descuento,
            -- exento: catalogo manda; si no hay, JSON; si no, 0
            CAST(COALESCE(sv.serv_Exento, it.itm_Exento, d.exentoJson, 0) AS BIT) AS exento,
            -- subtotal de linea = bruto - descuento acotado
            (d.cantidad * d.precioUnit) -
              CASE WHEN ISNULL(d.descuento,0) < 0 THEN 0
                   WHEN ISNULL(d.descuento,0) > (d.cantidad * d.precioUnit) THEN (d.cantidad * d.precioUnit)
                   ELSE d.descuento END AS subtotal
        FROM @det d
        LEFT JOIN [Veterinaria].[tbServicios] sv ON d.serv_Id = sv.serv_Id
        LEFT JOIN [Inventario].[tbItems]      it ON d.itm_Id  = it.itm_Id

        -- Totales
        DECLARE @subtotal DECIMAL(18,2) = (SELECT SUM(subtotal) FROM @linea)
        DECLARE @base     DECIMAL(18,2) = (SELECT ISNULL(SUM(subtotal),0) FROM @linea WHERE exento = 0)
        DECLARE @impuesto DECIMAL(18,2) = ROUND(@base * @impuestoPorc / 100.0, 2)
        -- descuento de factura acotado a [0, subtotal]
        DECLARE @factDesc DECIMAL(18,2) =
            CASE WHEN ISNULL(@fact_Descuento,0) < 0 THEN 0
                 WHEN ISNULL(@fact_Descuento,0) > @subtotal THEN @subtotal
                 ELSE @fact_Descuento END
        DECLARE @total DECIMAL(18,2) = @subtotal - @factDesc + @impuesto
        IF @total < 0 SET @total = 0

        -- Encabezado
        INSERT INTO [Veterinaria].[tbFacturas]
            (fact_Numero, cli_Id, masc_Id, refg_Id, fact_Fecha, fact_Subtotal, fact_Descuento,
             fact_Impuesto, fact_Total, fact_Estado, fact_UsuarioCrea, fact_FechaCrea)
        VALUES
            (@fact_Numero, @cli_Id, @masc_Id, @refg_Id, GETDATE(), @subtotal, @factDesc,
             @impuesto, @total, 'Emitida', @usuarioCrea, GETDATE())

        DECLARE @fact_Id INT = SCOPE_IDENTITY()

        -- Detalle (incluye descuento y foto de exento por linea)
        INSERT INTO [Veterinaria].[tbFacturaDetalle]
            (fact_Id, fdet_Tipo, serv_Id, itm_Id, fdet_Descripcion, fdet_Cantidad,
             fdet_PrecioUnit, fdet_Descuento, fdet_Exento, fdet_Subtotal)
        SELECT @fact_Id, tipo, serv_Id, itm_Id, descripcion, cantidad,
               precioUnit, descuento, exento, subtotal
        FROM @linea

        -- Salida de inventario por cada linea de Producto
        INSERT INTO [Veterinaria].[tbSalidasInventario]
            (itm_Id, fact_Id, sal_Cantidad, sal_Motivo, sal_Fecha, sal_UsuarioCrea)
        SELECT itm_Id, @fact_Id, cantidad, 'Venta', GETDATE(), @usuarioCrea
        FROM @linea
        WHERE tipo = 'Producto' AND itm_Id IS NOT NULL

        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Factura creada correctamente.' AS MessageStatus, @fact_Id AS fact_Id
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =============================================================================
-- PASO 7: Facturar/resolver una visita (cita) tomando en cuenta exentos.
--         (Este flujo no usa descuentos; se mantiene en 0 y 100% compatible.)
--         Foto de exento por linea desde el catalogo del origen del cargo.
-- =============================================================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_FacturarVisita]
    @cita_Id      INT,
    @disposicion  NVARCHAR(20),              -- 'Pagado' | 'PorCobrar' | 'SinCargo'
    @vencimiento  DATE = NULL,
    @motivo       NVARCHAR(1000) = NULL,
    @usuario      INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
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

        IF NOT EXISTS (SELECT 1 FROM [Veterinaria].[tbCargoDispensacion]
                       WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0)
        BEGIN
            SELECT CAST(NULL AS INT) AS fact_Id, -1 AS CodeStatus, 'La visita no tiene cargos pendientes.' AS MessageStatus
            RETURN
        END

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

        DECLARE @cli_Id INT, @masc_Id INT
        SELECT TOP 1 @cli_Id = cli_Id, @masc_Id = masc_Id
        FROM [Veterinaria].[tbCargoDispensacion]
        WHERE cita_Id = @cita_Id AND cargo_Estado = 'Pendiente' AND cargo_EsEliminado = 0

        -- Base gravable = cargos NO exentos. El exento se toma del catalogo del origen:
        --   Servicio  -> tbServicios.serv_Exento (cargo_OrigenId = serv_Id)
        --   Medicamento/Producto -> tbItems.itm_Exento (itm_Id)
        DECLARE @subtotal DECIMAL(18,2) =
            (SELECT SUM(c.cargo_Cantidad * c.cargo_PrecioUnitSnap)
             FROM [Veterinaria].[tbCargoDispensacion] c
             WHERE c.cita_Id = @cita_Id AND c.cargo_Estado = 'Pendiente' AND c.cargo_EsEliminado = 0);

        DECLARE @base DECIMAL(18,2) =
            (SELECT ISNULL(SUM(c.cargo_Cantidad * c.cargo_PrecioUnitSnap), 0)
             FROM [Veterinaria].[tbCargoDispensacion] c
             LEFT JOIN [Veterinaria].[tbServicios] sv
                    ON c.cargo_TipoOrigen = 'Servicio' AND c.cargo_OrigenId = sv.serv_Id
             LEFT JOIN [Inventario].[tbItems] it ON c.itm_Id = it.itm_Id
             WHERE c.cita_Id = @cita_Id AND c.cargo_Estado = 'Pendiente' AND c.cargo_EsEliminado = 0
               AND CAST(COALESCE(sv.serv_Exento, it.itm_Exento, 0) AS BIT) = 0);

        DECLARE @impuesto DECIMAL(18,2) = ROUND(@base * 15.00 / 100.0, 2);
        DECLARE @total    DECIMAL(18,2) = @subtotal + @impuesto;
        DECLARE @estado   VARCHAR(20)   = CASE WHEN @disposicion = 'Pagado' THEN 'Pagada' ELSE 'PorCobrar' END;
        DECLARE @venc     DATE          = CASE WHEN @disposicion = 'PorCobrar' THEN @vencimiento ELSE NULL END;

        BEGIN TRAN

        INSERT INTO [Veterinaria].[tbFacturas]
            (fact_Numero, cli_Id, masc_Id, refg_Id, fact_Fecha,
             fact_Subtotal, fact_Descuento, fact_Impuesto, fact_Total, fact_Estado, fact_Vencimiento,
             fact_EsEliminado, fact_UsuarioCrea, fact_FechaCrea)
        VALUES
            (NULL, @cli_Id, @masc_Id, NULL, GETDATE(),
             @subtotal, 0, @impuesto, @total, @estado, @venc,
             0, @usuario, GETDATE())

        DECLARE @fact_Id INT = CAST(SCOPE_IDENTITY() AS INT)

        DECLARE @cargo_Id INT, @itm_Id INT, @cant DECIMAL(18,2), @desc NVARCHAR(500),
                @precio DECIMAL(18,2), @fdet_Id INT, @tipoOrigen NVARCHAR(20), @origenId INT, @exento BIT

        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
            SELECT c.cargo_Id, c.itm_Id, c.cargo_Cantidad, c.cargo_DescripcionSnap, c.cargo_PrecioUnitSnap,
                   c.cargo_TipoOrigen, c.cargo_OrigenId,
                   CAST(COALESCE(sv.serv_Exento, it.itm_Exento, 0) AS BIT)
            FROM [Veterinaria].[tbCargoDispensacion] c
            LEFT JOIN [Veterinaria].[tbServicios] sv
                   ON c.cargo_TipoOrigen = 'Servicio' AND c.cargo_OrigenId = sv.serv_Id
            LEFT JOIN [Inventario].[tbItems] it ON c.itm_Id = it.itm_Id
            WHERE c.cita_Id = @cita_Id AND c.cargo_Estado = 'Pendiente' AND c.cargo_EsEliminado = 0

        OPEN cur
        FETCH NEXT FROM cur INTO @cargo_Id, @itm_Id, @cant, @desc, @precio, @tipoOrigen, @origenId, @exento
        WHILE @@FETCH_STATUS = 0
        BEGIN
            IF @tipoOrigen = 'Servicio'
                INSERT INTO [Veterinaria].[tbFacturaDetalle]
                    (fact_Id, fdet_Tipo, serv_Id, itm_Id, fdet_Descripcion,
                     fdet_Cantidad, fdet_PrecioUnit, fdet_Descuento, fdet_Exento, fdet_Subtotal)
                VALUES
                    (@fact_Id, 'Servicio', @origenId, NULL, @desc,
                     @cant, @precio, 0, @exento, @cant * @precio)
            ELSE
                INSERT INTO [Veterinaria].[tbFacturaDetalle]
                    (fact_Id, fdet_Tipo, serv_Id, itm_Id, fdet_Descripcion,
                     fdet_Cantidad, fdet_PrecioUnit, fdet_Descuento, fdet_Exento, fdet_Subtotal)
                VALUES
                    (@fact_Id, 'Producto', NULL, @itm_Id, @desc,
                     @cant, @precio, 0, @exento, @cant * @precio)

            SET @fdet_Id = CAST(SCOPE_IDENTITY() AS INT)

            UPDATE [Veterinaria].[tbCargoDispensacion]
            SET cargo_Estado = 'Facturado',
                fact_Id = @fact_Id, fdet_Id = @fdet_Id,
                cargo_ResueltoPor = @usuario, cargo_FechaResuelto = GETDATE(),
                cargo_MotivoResolucion = NULL,
                cargo_UsuarioModifica = @usuario, cargo_FechaModifica = GETDATE()
            WHERE cargo_Id = @cargo_Id

            FETCH NEXT FROM cur INTO @cargo_Id, @itm_Id, @cant, @desc, @precio, @tipoOrigen, @origenId, @exento
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

-- =============================================================================
-- PASO 8: Listados/detalle devuelven los campos nuevos; Saldo = Total-Pagado-Ajuste
-- =============================================================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Facturas_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER(ORDER BY f.fact_Id DESC) AS Fila,
        f.fact_Id,
        f.fact_Numero,
        f.cli_Id,
        LTRIM(RTRIM(ISNULL(c.cli_Nombres,'') + ' ' + ISNULL(c.cli_Apellidos,''))) AS Cliente,
        f.masc_Id,
        m.masc_Nombre AS Mascota,
        f.fact_Fecha,
        f.fact_Subtotal,
        f.fact_Descuento,
        f.fact_Impuesto,
        f.fact_Total,
        f.fact_Estado,
        f.fact_MontoPagado,
        f.fact_Ajuste,
        (f.fact_Total - f.fact_MontoPagado - f.fact_Ajuste) AS Saldo,
        f.fact_Vencimiento
    FROM [Veterinaria].[tbFacturas] f
    LEFT JOIN [Veterinaria].[tbClientes] c ON f.cli_Id = c.cli_Id
    LEFT JOIN [Refugio].[tbMascotas] m ON f.masc_Id = m.masc_Id
    WHERE f.fact_EsEliminado = 0
    ORDER BY f.fact_Id DESC
END
GO

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
        f.fact_Descuento,
        f.fact_Impuesto,
        f.fact_Total,
        f.fact_Estado,
        f.fact_Vencimiento,
        f.fact_MontoPagado,
        f.fact_Ajuste,
        (f.fact_Total - f.fact_MontoPagado - f.fact_Ajuste) AS Saldo
    FROM [Veterinaria].[tbFacturas] f
    LEFT JOIN [Veterinaria].[tbClientes] c ON f.cli_Id = c.cli_Id
    LEFT JOIN [Refugio].[tbMascotas] m ON f.masc_Id = m.masc_Id
    WHERE f.fact_Id = @fact_Id AND f.fact_EsEliminado = 0
END
GO

CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_FacturaDetalle_List]
    @fact_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        d.fdet_Id,
        d.fact_Id,
        d.fdet_Tipo,
        d.serv_Id,
        d.itm_Id,
        d.fdet_Descripcion,
        d.fdet_Cantidad,
        d.fdet_PrecioUnit,
        d.fdet_Descuento,
        d.fdet_Exento,
        d.fdet_Subtotal
    FROM [Veterinaria].[tbFacturaDetalle] d
    WHERE d.fact_Id = @fact_Id
    ORDER BY d.fdet_Id
END
GO

-- Resumen A/R: el saldo tambien resta los ajustes (creditos/reembolsos)
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Facturas_ResumenAR]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Hoy date = CAST(GETDATE() AS date);
    DECLARE @InicioMes date = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);

    SELECT
        PorCobrar = ISNULL((
            SELECT SUM(f.fact_Total - f.fact_MontoPagado - f.fact_Ajuste)
            FROM [Veterinaria].[tbFacturas] f
            WHERE f.fact_EsEliminado = 0 AND f.fact_Estado <> 'Anulada'
              AND (f.fact_Total - f.fact_MontoPagado - f.fact_Ajuste) > 0
        ), 0),
        Vencido = ISNULL((
            SELECT SUM(f.fact_Total - f.fact_MontoPagado - f.fact_Ajuste)
            FROM [Veterinaria].[tbFacturas] f
            WHERE f.fact_EsEliminado = 0 AND f.fact_Estado <> 'Anulada'
              AND (f.fact_Total - f.fact_MontoPagado - f.fact_Ajuste) > 0
              AND f.fact_Vencimiento IS NOT NULL AND f.fact_Vencimiento < @Hoy
        ), 0),
        CobradoMes = ISNULL((
            SELECT SUM(p.pago_Monto)
            FROM [Veterinaria].[tbPagos] p
            WHERE p.pago_EsEliminado = 0 AND p.pago_Fecha >= @InicioMes
        ), 0);
END
GO

-- =============================================================================
-- PASO 9: Pago respeta el saldo con ajustes (Total - Pagado - Ajuste)
-- =============================================================================
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
        DECLARE @total DECIMAL(18,2), @pagado DECIMAL(18,2), @ajuste DECIMAL(18,2), @estado VARCHAR(20)
        SELECT @total = fact_Total, @pagado = fact_MontoPagado, @ajuste = fact_Ajuste, @estado = fact_Estado
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
            SELECT CAST(NULL AS INT) AS pago_Id, (@total - @pagado - @ajuste) AS nuevo_saldo, @estado AS fact_Estado,
                   -2 AS CodeStatus, 'La factura no admite pagos en su estado actual.' AS MessageStatus
            RETURN
        END
        IF @pago_Monto IS NULL OR @pago_Monto <= 0
        BEGIN
            SELECT CAST(NULL AS INT) AS pago_Id, (@total - @pagado - @ajuste) AS nuevo_saldo, @estado AS fact_Estado,
                   -3 AS CodeStatus, 'El monto del pago debe ser mayor a cero.' AS MessageStatus
            RETURN
        END

        IF @pago_Metodo IS NULL OR @pago_Metodo NOT IN ('Efectivo','TarjetaCredito','TarjetaDebito','Cheque','Transferencia','Otro')
            SET @pago_Metodo = 'Efectivo'

        DECLARE @saldo DECIMAL(18,2) = @total - @pagado - @ajuste
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
        DECLARE @nuevoEstado VARCHAR(20) = CASE WHEN (@nuevoPagado + @ajuste) >= @total THEN 'Pagada' ELSE @estado END

        UPDATE [Veterinaria].[tbFacturas]
        SET fact_MontoPagado = @nuevoPagado,
            fact_Estado = @nuevoEstado,
            fact_UsuarioModifica = @usuario, fact_FechaModifica = GETDATE()
        WHERE fact_Id = @fact_Id

        COMMIT

        SELECT @pago_Id AS pago_Id, (@total - @nuevoPagado - @ajuste) AS nuevo_saldo, @nuevoEstado AS fact_Estado,
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

-- =============================================================================
-- PASO 10: Ajustar factura = nota de credito (condona saldo) o reembolso
--          (devuelve pagado). Estilo invoice_adjustments de OpenVPM:
--          idempotente por clave, valida que no exceda el saldo, deja traza y
--          el saldo resultante. NotaCredito sube fact_Ajuste; Reembolso baja
--          fact_MontoPagado. Si cierra el saldo, marca 'Pagada'.
-- =============================================================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Factura_Ajustar]
    @fact_Id        INT,
    @tipo           NVARCHAR(20),            -- 'NotaCredito' | 'Reembolso'
    @monto          DECIMAL(18,2),
    @motivo         NVARCHAR(500) = NULL,
    @claveOperacion NVARCHAR(160) = NULL,    -- idempotencia opcional
    @usuario        INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @tipo NOT IN ('NotaCredito','Reembolso')
        BEGIN
            SELECT CAST(NULL AS INT) AS faj_Id, CAST(0 AS DECIMAL(18,2)) AS nuevo_saldo,
                   CAST(NULL AS VARCHAR(20)) AS fact_Estado,
                   -2 AS CodeStatus, 'Tipo de ajuste invalido (NotaCredito/Reembolso).' AS MessageStatus
            RETURN
        END

        -- Reintento idempotente: si la clave ya se uso, devolver el ajuste previo.
        IF @claveOperacion IS NOT NULL
        BEGIN
            DECLARE @prevId INT = (SELECT TOP 1 faj_Id FROM [Veterinaria].[tbFacturaAjustes]
                                   WHERE faj_ClaveOperacion = @claveOperacion AND faj_EsEliminado = 0)
            IF @prevId IS NOT NULL
            BEGIN
                SELECT @prevId AS faj_Id,
                       (SELECT faj_SaldoDespues FROM [Veterinaria].[tbFacturaAjustes] WHERE faj_Id = @prevId) AS nuevo_saldo,
                       (SELECT fact_Estado FROM [Veterinaria].[tbFacturas] WHERE fact_Id = @fact_Id) AS fact_Estado,
                       1 AS CodeStatus, 'Ajuste ya aplicado (idempotente).' AS MessageStatus
                RETURN
            END
        END

        DECLARE @total DECIMAL(18,2), @pagado DECIMAL(18,2), @ajuste DECIMAL(18,2), @estado VARCHAR(20)
        SELECT @total = fact_Total, @pagado = fact_MontoPagado, @ajuste = fact_Ajuste, @estado = fact_Estado
        FROM [Veterinaria].[tbFacturas]
        WHERE fact_Id = @fact_Id AND fact_EsEliminado = 0

        IF @total IS NULL
        BEGIN
            SELECT CAST(NULL AS INT) AS faj_Id, CAST(0 AS DECIMAL(18,2)) AS nuevo_saldo,
                   CAST(NULL AS VARCHAR(20)) AS fact_Estado,
                   -1 AS CodeStatus, 'La factura no existe o fue eliminada.' AS MessageStatus
            RETURN
        END
        IF @estado = 'Anulada'
        BEGIN
            SELECT CAST(NULL AS INT) AS faj_Id, CAST(0 AS DECIMAL(18,2)) AS nuevo_saldo, @estado AS fact_Estado,
                   -2 AS CodeStatus, 'La factura esta anulada.' AS MessageStatus
            RETURN
        END
        IF @monto IS NULL OR @monto <= 0
        BEGIN
            SELECT CAST(NULL AS INT) AS faj_Id, (@total - @pagado - @ajuste) AS nuevo_saldo, @estado AS fact_Estado,
                   -3 AS CodeStatus, 'El monto debe ser mayor a cero.' AS MessageStatus
            RETURN
        END

        DECLARE @saldo DECIMAL(18,2) = @total - @pagado - @ajuste

        IF @tipo = 'NotaCredito'
        BEGIN
            -- Condona parte (o todo) del saldo pendiente: no puede exceder el saldo.
            IF @monto > @saldo
            BEGIN
                SELECT CAST(NULL AS INT) AS faj_Id, @saldo AS nuevo_saldo, @estado AS fact_Estado,
                       -4 AS CodeStatus, 'La nota de credito excede el saldo pendiente.' AS MessageStatus
                RETURN
            END
        END
        ELSE
        BEGIN
            -- Reembolso: devuelve dinero ya pagado; no puede exceder lo pagado.
            IF @monto > @pagado
            BEGIN
                SELECT CAST(NULL AS INT) AS faj_Id, @saldo AS nuevo_saldo, @estado AS fact_Estado,
                       -4 AS CodeStatus, 'El reembolso excede el monto pagado.' AS MessageStatus
                RETURN
            END
        END

        BEGIN TRAN

        DECLARE @nuevoPagado DECIMAL(18,2) = @pagado
        DECLARE @nuevoAjuste DECIMAL(18,2) = @ajuste

        IF @tipo = 'NotaCredito'
            SET @nuevoAjuste = @ajuste + @monto
        ELSE
            SET @nuevoPagado = @pagado - @monto   -- reembolso reabre saldo

        DECLARE @nuevoSaldo DECIMAL(18,2) = @total - @nuevoPagado - @nuevoAjuste
        DECLARE @nuevoEstado VARCHAR(20) =
            CASE WHEN @nuevoSaldo <= 0 THEN 'Pagada'
                 WHEN @estado = 'Pagada' AND @nuevoSaldo > 0 THEN 'PorCobrar'
                 ELSE @estado END

        INSERT INTO [Veterinaria].[tbFacturaAjustes]
            (fact_Id, faj_Tipo, faj_Monto, faj_Motivo, faj_SaldoDespues, faj_ClaveOperacion,
             faj_EsEliminado, faj_UsuarioCrea, faj_FechaCrea)
        VALUES
            (@fact_Id, @tipo, @monto, @motivo, @nuevoSaldo, @claveOperacion,
             0, @usuario, GETDATE())

        DECLARE @faj_Id INT = CAST(SCOPE_IDENTITY() AS INT)

        UPDATE [Veterinaria].[tbFacturas]
        SET fact_MontoPagado = @nuevoPagado,
            fact_Ajuste = @nuevoAjuste,
            fact_Estado = @nuevoEstado,
            fact_UsuarioModifica = @usuario, fact_FechaModifica = GETDATE()
        WHERE fact_Id = @fact_Id

        COMMIT

        SELECT @faj_Id AS faj_Id, @nuevoSaldo AS nuevo_saldo, @nuevoEstado AS fact_Estado,
               1 AS CodeStatus,
               CASE WHEN @tipo = 'NotaCredito' THEN 'Nota de credito aplicada.' ELSE 'Reembolso registrado.' END AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK
        SELECT CAST(NULL AS INT) AS faj_Id, CAST(0 AS DECIMAL(18,2)) AS nuevo_saldo,
               CAST(NULL AS VARCHAR(20)) AS fact_Estado,
               -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- Historial de ajustes de una factura (para el comprobante / auditoria)
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_FacturaAjustes_List]
    @fact_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        a.faj_Id,
        a.fact_Id,
        a.faj_Tipo,
        a.faj_Monto,
        a.faj_Motivo,
        a.faj_SaldoDespues,
        a.faj_FechaCrea,
        u.Usu_Nombre AS RegistradoPor
    FROM [Veterinaria].[tbFacturaAjustes] a
    LEFT JOIN [Seguridad].[tbUsuarios] u ON a.faj_UsuarioCrea = u.usu_Id
    WHERE a.fact_Id = @fact_Id AND a.faj_EsEliminado = 0
    ORDER BY a.faj_Id DESC
END
GO

PRINT '== Script 99: descuentos, exentos y ajustes (nota de credito/reembolso) aplicados ==';
GO
