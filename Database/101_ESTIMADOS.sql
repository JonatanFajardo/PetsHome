/*
=============================================
SCRIPT 101: ESTIMADOS / PRESUPUESTOS (feature/estimados)
AUTOR: Sistema PetsHome
FECHA: 2026-10-31
DESCRIPCION:
  Modulo de ESTIMADOS/PRESUPUESTOS: una cotizacion (la cuenta estimada que
  aun NO se cobra) con lineas de servicios/productos, un total, y la accion de
  CONVERTIR un presupuesto aceptado en factura real.

  Replica el concepto de "estimates" del billing de OpenVPM (un presupuesto es
  una cuenta en borrador que, solo cuando el cliente la acepta, se convierte en
  factura; no se puede cobrar un presupuesto, primero se convierte). En OpenVPM
  el estimado es una factura con bandera "isEstimate"; aqui lo modelamos como
  tablas propias (tbEstimados/tbEstimadoDetalle) y convertimos insertando en las
  tablas de factura existentes (tbFacturas/tbFacturaDetalle).

  MODELO:
    - Veterinaria.tbEstimados       (encabezado del presupuesto)
    - Veterinaria.tbEstimadoDetalle (lineas: servicio o producto)

  ESTADOS (est_Estado, sin tildes):
    'Borrador' -> recien creado, editable.
    'Enviado'  -> enviado al cliente.
    'Aceptado' -> el cliente lo aprobo; listo para convertir a factura.
    'Rechazado'-> el cliente no lo aprobo.
    'Convertido'-> ya se convirtio en factura (est_FacturaId apunta a la factura).

  CONVERSION (PR_Veterinaria_Estimado_ConvertirAFactura):
    Solo si est_Estado='Aceptado' y aun no convertido. Inserta encabezado +
    detalle en tbFacturas/tbFacturaDetalle (ISV 15%, igual que scripts 43/80),
    marca el presupuesto como 'Convertido' y guarda el fact_Id resultante.
    Idempotente: si ya estaba convertido, devuelve el fact_Id existente sin
    duplicar. NO toca ni llama los SP de factura del otro agente; inserta directo.

  Patron de escritura: CodeStatus/MessageStatus (Success = CodeStatus>0);
  los que crean algo devuelven ademas el id (est_Id / fact_Id).

  PANTALLA: "Presupuestos" (modu_Id = 10, Facturacion y caja; es de DINERO ->
  degradable con el modulo de facturacion). Asignada a Administrador (1),
  Veterinario (4) y Recepcionista (7). REQUIERE RE-LOGIN para refrescar claims.

  Idempotente: usa IF NOT EXISTS / CREATE OR ALTER. GO entre lotes.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

-- =============================================
-- PASO 1: Tabla encabezado  Veterinaria.tbEstimados
-- =============================================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('Veterinaria.tbEstimados') AND type = 'U')
BEGIN
    CREATE TABLE [Veterinaria].[tbEstimados] (
        est_Id              INT IDENTITY(1,1) NOT NULL,
        cli_Id              INT NULL,
        masc_Id             INT NULL,
        est_Fecha           DATE NOT NULL CONSTRAINT DF_tbEstimados_Fecha DEFAULT (CAST(GETDATE() AS DATE)),
        est_Vigencia        DATE NULL,
        est_Estado          NVARCHAR(20) NOT NULL CONSTRAINT DF_tbEstimados_Estado DEFAULT ('Borrador'),
        est_Total           DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbEstimados_Total DEFAULT (0),
        est_Notas           NVARCHAR(500) NULL,
        est_FacturaId       INT NULL,
        -- Auditoria
        est_EsEliminado     BIT NOT NULL CONSTRAINT DF_tbEstimados_EsEliminado DEFAULT (0),
        est_UsuarioCrea     INT NOT NULL,
        est_FechaCrea       DATETIME NOT NULL CONSTRAINT DF_tbEstimados_FechaCrea DEFAULT (GETDATE()),
        est_UsuarioModifica INT NULL,
        est_FechaModifica   DATETIME NULL,
        CONSTRAINT PK_tbEstimados PRIMARY KEY CLUSTERED (est_Id),
        CONSTRAINT CK_tbEstimados_Estado CHECK (est_Estado IN ('Borrador','Enviado','Aceptado','Rechazado','Convertido')),
        CONSTRAINT FK_tbEstimados_Clientes FOREIGN KEY (cli_Id) REFERENCES [Veterinaria].[tbClientes](cli_Id),
        CONSTRAINT FK_tbEstimados_Mascotas FOREIGN KEY (masc_Id) REFERENCES [Refugio].[tbMascotas](masc_Id),
        CONSTRAINT FK_tbEstimados_Factura FOREIGN KEY (est_FacturaId) REFERENCES [Veterinaria].[tbFacturas](fact_Id)
    )
    PRINT 'Tabla Veterinaria.tbEstimados creada'
END
ELSE
    PRINT 'Tabla Veterinaria.tbEstimados ya existe'
GO

-- =============================================
-- PASO 2: Tabla detalle  Veterinaria.tbEstimadoDetalle
-- =============================================
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID('Veterinaria.tbEstimadoDetalle') AND type = 'U')
BEGIN
    CREATE TABLE [Veterinaria].[tbEstimadoDetalle] (
        estd_Id             INT IDENTITY(1,1) NOT NULL,
        est_Id              INT NOT NULL,
        estd_Tipo           NVARCHAR(20) NOT NULL,          -- 'Servicio' | 'Producto'
        serv_Id             INT NULL,
        itm_Id              INT NULL,
        estd_Descripcion    NVARCHAR(250) NOT NULL,
        estd_Cantidad       DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbEstimadoDetalle_Cant DEFAULT (1),
        estd_PrecioUnit     DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbEstimadoDetalle_Precio DEFAULT (0),
        estd_Subtotal       DECIMAL(18,2) NOT NULL CONSTRAINT DF_tbEstimadoDetalle_Subtotal DEFAULT (0),
        -- Auditoria
        estd_EsEliminado    BIT NOT NULL CONSTRAINT DF_tbEstimadoDetalle_EsEliminado DEFAULT (0),
        estd_UsuarioCrea    INT NOT NULL,
        estd_FechaCrea      DATETIME NOT NULL CONSTRAINT DF_tbEstimadoDetalle_FechaCrea DEFAULT (GETDATE()),
        estd_UsuarioModifica INT NULL,
        estd_FechaModifica  DATETIME NULL,
        CONSTRAINT PK_tbEstimadoDetalle PRIMARY KEY CLUSTERED (estd_Id),
        CONSTRAINT CK_tbEstimadoDetalle_Tipo CHECK (estd_Tipo IN ('Servicio','Producto')),
        CONSTRAINT CK_tbEstimadoDetalle_TipoFK CHECK (
            (estd_Tipo = 'Servicio' AND serv_Id IS NOT NULL AND itm_Id IS NULL) OR
            (estd_Tipo = 'Producto' AND itm_Id IS NOT NULL AND serv_Id IS NULL) OR
            (serv_Id IS NULL AND itm_Id IS NULL)   -- linea libre (sin catalogo)
        ),
        CONSTRAINT FK_tbEstimadoDetalle_Estimado FOREIGN KEY (est_Id) REFERENCES [Veterinaria].[tbEstimados](est_Id),
        CONSTRAINT FK_tbEstimadoDetalle_Servicio FOREIGN KEY (serv_Id) REFERENCES [Veterinaria].[tbServicios](serv_Id),
        CONSTRAINT FK_tbEstimadoDetalle_Item FOREIGN KEY (itm_Id) REFERENCES [Inventario].[tbItems](itm_Id)
    )
    PRINT 'Tabla Veterinaria.tbEstimadoDetalle creada'
END
ELSE
    PRINT 'Tabla Veterinaria.tbEstimadoDetalle ya existe'
GO

-- =============================================
-- PASO 3: Crear presupuesto (encabezado + detalle) TRANSACCIONAL
--         Lineas via JSON (OPENJSON). est_Total = suma de subtotales.
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Estimado_Crear]
    @cli_Id       INT = NULL,
    @masc_Id      INT = NULL,
    @est_Vigencia DATE = NULL,
    @est_Notas    NVARCHAR(500) = NULL,
    @usuarioCrea  INT,
    @detalleJson  NVARCHAR(MAX)   -- [{"tipo":"Servicio","serv_Id":1,"itm_Id":null,"descripcion":"Consulta","cantidad":1,"precioUnit":400.00}, ...]
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        BEGIN TRANSACTION

        DECLARE @det TABLE (
            tipo        VARCHAR(20),
            serv_Id     INT NULL,
            itm_Id      INT NULL,
            descripcion VARCHAR(250),
            cantidad    DECIMAL(18,2),
            precioUnit  DECIMAL(18,2)
        )
        INSERT INTO @det (tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit)
        SELECT tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit
        FROM OPENJSON(@detalleJson)
        WITH (
            tipo VARCHAR(20)         '$.tipo',
            serv_Id INT              '$.serv_Id',
            itm_Id INT               '$.itm_Id',
            descripcion VARCHAR(250) '$.descripcion',
            cantidad DECIMAL(18,2)   '$.cantidad',
            precioUnit DECIMAL(18,2) '$.precioUnit'
        )

        IF NOT EXISTS (SELECT 1 FROM @det)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1 AS CodeStatus, 'El presupuesto no tiene lineas.' AS MessageStatus, CAST(NULL AS INT) AS est_Id
            RETURN
        END

        DECLARE @total DECIMAL(18,2) = (SELECT SUM(cantidad * precioUnit) FROM @det)

        INSERT INTO [Veterinaria].[tbEstimados]
            (cli_Id, masc_Id, est_Fecha, est_Vigencia, est_Estado, est_Total, est_Notas,
             est_EsEliminado, est_UsuarioCrea, est_FechaCrea)
        VALUES
            (@cli_Id, @masc_Id, CAST(GETDATE() AS DATE), @est_Vigencia, 'Borrador', @total, @est_Notas,
             0, @usuarioCrea, GETDATE())

        DECLARE @est_Id INT = CAST(SCOPE_IDENTITY() AS INT)

        INSERT INTO [Veterinaria].[tbEstimadoDetalle]
            (est_Id, estd_Tipo, serv_Id, itm_Id, estd_Descripcion, estd_Cantidad, estd_PrecioUnit, estd_Subtotal,
             estd_EsEliminado, estd_UsuarioCrea, estd_FechaCrea)
        SELECT @est_Id, tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit, cantidad * precioUnit,
               0, @usuarioCrea, GETDATE()
        FROM @det

        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Presupuesto creado correctamente.' AS MessageStatus, @est_Id AS est_Id
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus, CAST(NULL AS INT) AS est_Id
    END CATCH
END
GO

-- =============================================
-- PASO 4: Actualizar presupuesto (solo si esta en Borrador)
--         Reemplaza las lineas y recalcula el total.
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Estimado_Actualizar]
    @est_Id       INT,
    @cli_Id       INT = NULL,
    @masc_Id      INT = NULL,
    @est_Vigencia DATE = NULL,
    @est_Notas    NVARCHAR(500) = NULL,
    @usuario      INT,
    @detalleJson  NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [Veterinaria].[tbEstimados]
                       WHERE est_Id = @est_Id AND est_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'El presupuesto no existe.' AS MessageStatus
            RETURN
        END
        IF EXISTS (SELECT 1 FROM [Veterinaria].[tbEstimados]
                   WHERE est_Id = @est_Id AND est_Estado <> 'Borrador')
        BEGIN
            SELECT -2 AS CodeStatus, 'Solo se puede editar un presupuesto en Borrador.' AS MessageStatus
            RETURN
        END

        BEGIN TRANSACTION

        DECLARE @det TABLE (
            tipo VARCHAR(20), serv_Id INT NULL, itm_Id INT NULL,
            descripcion VARCHAR(250), cantidad DECIMAL(18,2), precioUnit DECIMAL(18,2)
        )
        INSERT INTO @det (tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit)
        SELECT tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit
        FROM OPENJSON(@detalleJson)
        WITH (
            tipo VARCHAR(20)         '$.tipo',
            serv_Id INT              '$.serv_Id',
            itm_Id INT               '$.itm_Id',
            descripcion VARCHAR(250) '$.descripcion',
            cantidad DECIMAL(18,2)   '$.cantidad',
            precioUnit DECIMAL(18,2) '$.precioUnit'
        )

        IF NOT EXISTS (SELECT 1 FROM @det)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -3 AS CodeStatus, 'El presupuesto no tiene lineas.' AS MessageStatus
            RETURN
        END

        -- Reemplazar lineas (borrado fisico del detalle: es borrador, no hay historico que cuidar)
        DELETE FROM [Veterinaria].[tbEstimadoDetalle] WHERE est_Id = @est_Id

        INSERT INTO [Veterinaria].[tbEstimadoDetalle]
            (est_Id, estd_Tipo, serv_Id, itm_Id, estd_Descripcion, estd_Cantidad, estd_PrecioUnit, estd_Subtotal,
             estd_EsEliminado, estd_UsuarioCrea, estd_FechaCrea)
        SELECT @est_Id, tipo, serv_Id, itm_Id, descripcion, cantidad, precioUnit, cantidad * precioUnit,
               0, @usuario, GETDATE()
        FROM @det

        DECLARE @total DECIMAL(18,2) = (SELECT SUM(cantidad * precioUnit) FROM @det)

        UPDATE [Veterinaria].[tbEstimados]
        SET cli_Id = @cli_Id, masc_Id = @masc_Id, est_Vigencia = @est_Vigencia,
            est_Notas = @est_Notas, est_Total = @total,
            est_UsuarioModifica = @usuario, est_FechaModifica = GETDATE()
        WHERE est_Id = @est_Id

        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Presupuesto actualizado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =============================================
-- PASO 5: Listar presupuestos (filtro opcional por estado)
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Estimados_List]
    @est_Estado NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        ROW_NUMBER() OVER (ORDER BY e.est_Id DESC) AS Fila,
        e.est_Id,
        e.cli_Id,
        LTRIM(RTRIM(ISNULL(c.cli_Nombres,'') + ' ' + ISNULL(c.cli_Apellidos,''))) AS Cliente,
        e.masc_Id,
        m.masc_Nombre AS Mascota,
        e.est_Fecha,
        e.est_Vigencia,
        e.est_Estado,
        e.est_Total,
        e.est_FacturaId,
        (SELECT COUNT(*) FROM [Veterinaria].[tbEstimadoDetalle] d WHERE d.est_Id = e.est_Id) AS Lineas
    FROM [Veterinaria].[tbEstimados] e
    LEFT JOIN [Veterinaria].[tbClientes] c ON e.cli_Id = c.cli_Id
    LEFT JOIN [Refugio].[tbMascotas] m ON e.masc_Id = m.masc_Id
    WHERE e.est_EsEliminado = 0
      AND (@est_Estado IS NULL OR e.est_Estado = @est_Estado)
    ORDER BY e.est_Id DESC
END
GO

-- =============================================
-- PASO 6: Encabezado de un presupuesto
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Estimados_Find]
    @est_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        e.est_Id,
        e.cli_Id,
        LTRIM(RTRIM(ISNULL(c.cli_Nombres,'') + ' ' + ISNULL(c.cli_Apellidos,''))) AS Cliente,
        c.cli_Identidad,
        e.masc_Id,
        m.masc_Nombre AS Mascota,
        e.est_Fecha,
        e.est_Vigencia,
        e.est_Estado,
        e.est_Total,
        e.est_Notas,
        e.est_FacturaId
    FROM [Veterinaria].[tbEstimados] e
    LEFT JOIN [Veterinaria].[tbClientes] c ON e.cli_Id = c.cli_Id
    LEFT JOIN [Refugio].[tbMascotas] m ON e.masc_Id = m.masc_Id
    WHERE e.est_Id = @est_Id AND e.est_EsEliminado = 0
END
GO

-- =============================================
-- PASO 7: Lineas de un presupuesto
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_EstimadoDetalle_List]
    @est_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT
        d.estd_Id,
        d.est_Id,
        d.estd_Tipo,
        d.serv_Id,
        d.itm_Id,
        d.estd_Descripcion,
        d.estd_Cantidad,
        d.estd_PrecioUnit,
        d.estd_Subtotal
    FROM [Veterinaria].[tbEstimadoDetalle] d
    WHERE d.est_Id = @est_Id
    ORDER BY d.estd_Id
END
GO

-- =============================================
-- PASO 8: Cambiar estado (Enviado / Aceptado / Rechazado)
--         No permite tocar un presupuesto ya Convertido.
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Estimado_CambiarEstado]
    @est_Id     INT,
    @est_Estado NVARCHAR(20),
    @usuario    INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF @est_Estado NOT IN ('Enviado','Aceptado','Rechazado')
        BEGIN
            SELECT -2 AS CodeStatus, 'Estado invalido (Enviado/Aceptado/Rechazado).' AS MessageStatus
            RETURN
        END

        DECLARE @actual NVARCHAR(20)
        SELECT @actual = est_Estado FROM [Veterinaria].[tbEstimados]
        WHERE est_Id = @est_Id AND est_EsEliminado = 0

        IF @actual IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, 'El presupuesto no existe.' AS MessageStatus
            RETURN
        END
        IF @actual = 'Convertido'
        BEGIN
            SELECT -3 AS CodeStatus, 'El presupuesto ya fue convertido en factura; no se puede cambiar.' AS MessageStatus
            RETURN
        END

        UPDATE [Veterinaria].[tbEstimados]
        SET est_Estado = @est_Estado,
            est_UsuarioModifica = @usuario, est_FechaModifica = GETDATE()
        WHERE est_Id = @est_Id

        SELECT 1 AS CodeStatus, 'Estado actualizado a ' + @est_Estado + '.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =============================================
-- PASO 9: CONVERTIR presupuesto Aceptado -> factura (idempotente)
--         Inserta directo en tbFacturas + tbFacturaDetalle (ISV 15%).
--         No llama ni modifica los SP de factura; inserta las lineas del
--         presupuesto. Marca 'Convertido' y guarda est_FacturaId.
-- =============================================
CREATE OR ALTER PROCEDURE [Veterinaria].[PR_Veterinaria_Estimado_ConvertirAFactura]
    @est_Id  INT,
    @usuario INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @estado NVARCHAR(20), @facturaId INT, @cli_Id INT, @masc_Id INT
        SELECT @estado = est_Estado, @facturaId = est_FacturaId, @cli_Id = cli_Id, @masc_Id = masc_Id
        FROM [Veterinaria].[tbEstimados]
        WHERE est_Id = @est_Id AND est_EsEliminado = 0

        IF @estado IS NULL
        BEGIN
            SELECT -1 AS CodeStatus, 'El presupuesto no existe.' AS MessageStatus, CAST(NULL AS INT) AS fact_Id
            RETURN
        END

        -- Idempotente: si ya se convirtio, devolver la factura existente sin duplicar
        IF @estado = 'Convertido' AND @facturaId IS NOT NULL
        BEGIN
            SELECT 1 AS CodeStatus, 'El presupuesto ya estaba convertido.' AS MessageStatus, @facturaId AS fact_Id
            RETURN
        END

        IF @estado <> 'Aceptado'
        BEGIN
            SELECT -2 AS CodeStatus, 'Solo se puede convertir un presupuesto Aceptado.' AS MessageStatus, CAST(NULL AS INT) AS fact_Id
            RETURN
        END

        IF NOT EXISTS (SELECT 1 FROM [Veterinaria].[tbEstimadoDetalle] WHERE est_Id = @est_Id)
        BEGIN
            SELECT -3 AS CodeStatus, 'El presupuesto no tiene lineas.' AS MessageStatus, CAST(NULL AS INT) AS fact_Id
            RETURN
        END

        -- Si no hay cliente, usar Consumidor Final (igual que PR_Veterinaria_Factura_Crear)
        IF @cli_Id IS NULL
            SET @cli_Id = (SELECT TOP 1 cli_Id FROM [Veterinaria].[tbClientes]
                           WHERE cli_EsConsumidorFinal = 1 AND cli_EsEliminado = 0)

        DECLARE @subtotal DECIMAL(18,2) = (SELECT SUM(estd_Subtotal) FROM [Veterinaria].[tbEstimadoDetalle] WHERE est_Id = @est_Id)
        DECLARE @impuesto DECIMAL(18,2) = ROUND(@subtotal * 15.00 / 100.0, 2)   -- ISV 15% (Honduras)
        DECLARE @total    DECIMAL(18,2) = @subtotal + @impuesto

        BEGIN TRANSACTION

        -- 1. Encabezado de factura (campos nuevos del otro agente quedan en su DEFAULT)
        INSERT INTO [Veterinaria].[tbFacturas]
            (fact_Numero, cli_Id, masc_Id, refg_Id, fact_Fecha,
             fact_Subtotal, fact_Impuesto, fact_Total, fact_Estado,
             fact_EsEliminado, fact_UsuarioCrea, fact_FechaCrea)
        VALUES
            (NULL, @cli_Id, @masc_Id, NULL, GETDATE(),
             @subtotal, @impuesto, @total, 'Emitida',
             0, @usuario, GETDATE())

        DECLARE @fact_Id INT = CAST(SCOPE_IDENTITY() AS INT)

        -- 2. Detalle de factura desde las lineas del presupuesto
        INSERT INTO [Veterinaria].[tbFacturaDetalle]
            (fact_Id, fdet_Tipo, serv_Id, itm_Id, fdet_Descripcion, fdet_Cantidad, fdet_PrecioUnit, fdet_Subtotal)
        SELECT @fact_Id, estd_Tipo, serv_Id, itm_Id, estd_Descripcion, estd_Cantidad, estd_PrecioUnit, estd_Subtotal
        FROM [Veterinaria].[tbEstimadoDetalle]
        WHERE est_Id = @est_Id

        -- 3. Marcar el presupuesto como Convertido
        UPDATE [Veterinaria].[tbEstimados]
        SET est_Estado = 'Convertido', est_FacturaId = @fact_Id,
            est_UsuarioModifica = @usuario, est_FechaModifica = GETDATE()
        WHERE est_Id = @est_Id

        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Presupuesto convertido en factura.' AS MessageStatus, @fact_Id AS fact_Id
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus, CAST(NULL AS INT) AS fact_Id
    END CATCH
END
GO

-- =============================================
-- PASO 10: Registrar la pantalla "Presupuestos" y asignarla a roles
--          modu_Id = 10 (Facturacion y caja): es de DINERO -> degradable con
--          el modulo de facturacion. Roles: Administrador(1), Veterinario(4),
--          Recepcionista(7). REQUIERE RE-LOGIN para refrescar claims.
-- =============================================
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Presupuestos')
BEGIN
    INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo, pan_EsActivo, modu_Id)
    VALUES ('Presupuestos', 'Veterinaria', 1, 10);
    PRINT 'Pantalla "Presupuestos" registrada';
END
ELSE
BEGIN
    UPDATE [Seguridad].[tbPantallas]
    SET pan_EsActivo = 1, pan_Grupo = 'Veterinaria', modu_Id = 10
    WHERE pan_Descripcion = 'Presupuestos';
    PRINT 'Pantalla "Presupuestos" ya existia (actualizada)';
END
GO

DECLARE @pan_Id INT = (SELECT pan_Id FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Presupuestos');

-- Administrador(1)/Veterinario(4): CRUD; Recepcionista(7): consultar + insertar (arma presupuestos)
;WITH roles AS (
    SELECT 1 AS rol_Id, 1 AS ins, 1 AS edi, 1 AS eli
    UNION ALL SELECT 4, 1, 1, 0
    UNION ALL SELECT 7, 1, 0, 0
)
MERGE [Seguridad].[tbRolesPantallas] AS t
USING roles AS s
    ON t.rol_Id = s.rol_Id AND t.pan_Id = @pan_Id
WHEN MATCHED THEN
    UPDATE SET ropan_EsActivo = 1, ropan_Consultar = 1,
               ropan_Insertar = s.ins, ropan_Editar = s.edi, ropan_Eliminar = s.eli
WHEN NOT MATCHED THEN
    INSERT (rol_Id, pan_Id, ropan_EsActivo, ropan_Consultar, ropan_Insertar, ropan_Editar, ropan_Eliminar)
    VALUES (s.rol_Id, @pan_Id, 1, 1, s.ins, s.edi, s.eli);

PRINT 'Pantalla "Presupuestos" asignada a Administrador, Veterinario y Recepcionista';
GO

-- =============================================
-- Verificacion
-- =============================================
SELECT p.pan_Id, p.pan_Descripcion, p.pan_Grupo, p.pan_EsActivo, p.modu_Id
FROM [Seguridad].[tbPantallas] p WHERE p.pan_Descripcion = 'Presupuestos';
GO

PRINT '============================================='
PRINT 'Script 101: Estimados/Presupuestos creado. RE-LOGIN para ver la pantalla.'
PRINT '============================================='
GO
