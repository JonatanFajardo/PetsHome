-- =============================================================================
-- 82_SERVICIOS_FACTURABLES.sql
-- Hace facturables VACUNA, LABORATORIO y PROCEDIMIENTO (hoy solo medicamentos).
-- Modelo OpenVPM: todo lo no-inventario es un "servicio" con precio.
--
-- ADITIVO y backward-compatible. Idempotente (IF NOT EXISTS / COL_LENGTH IS NULL).
-- Textos/enum SIN acentos (evita mojibake).
-- =============================================================================
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- =============================================================================
-- PASO 1: Generalizar Veterinaria.tbCargoDispensacion (ADITIVO)
-- =============================================================================

-- 1a. receta_Id e itm_Id pasan a NULL (un servicio no tiene receta ni item).
--     Las FK permiten NULL sin dropearlas; solo cambia la nulabilidad.
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('Veterinaria.tbCargoDispensacion')
             AND name = 'receta_Id' AND is_nullable = 0)
BEGIN
    ALTER TABLE [Veterinaria].[tbCargoDispensacion] ALTER COLUMN receta_Id INT NULL;
    PRINT 'receta_Id -> NULL';
END
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('Veterinaria.tbCargoDispensacion')
             AND name = 'itm_Id' AND is_nullable = 0)
BEGIN
    ALTER TABLE [Veterinaria].[tbCargoDispensacion] ALTER COLUMN itm_Id INT NULL;
    PRINT 'itm_Id -> NULL';
END
GO

-- 1b. cargo_TipoOrigen: 'Medicamento' | 'Servicio'
IF COL_LENGTH('Veterinaria.tbCargoDispensacion', 'cargo_TipoOrigen') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbCargoDispensacion]
        ADD cargo_TipoOrigen NVARCHAR(20) NOT NULL
            CONSTRAINT DF_cargo_TipoOrigen DEFAULT 'Medicamento';
    PRINT 'cargo_TipoOrigen agregado';
END
GO

-- 1c. cargo_OrigenId: receta_Id (medicamento) | serv_Id (servicio)
IF COL_LENGTH('Veterinaria.tbCargoDispensacion', 'cargo_OrigenId') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbCargoDispensacion] ADD cargo_OrigenId INT NULL;
    PRINT 'cargo_OrigenId agregado';
END
GO

-- 1d. Backfill filas existentes: todas son medicamentos ligados a su receta.
UPDATE [Veterinaria].[tbCargoDispensacion]
    SET cargo_TipoOrigen = 'Medicamento',
        cargo_OrigenId   = receta_Id
    WHERE cargo_OrigenId IS NULL;
GO

-- =============================================================================
-- PASO 2: Catalogo Veterinaria.tbServicios (ADITIVO)
-- =============================================================================

-- 2a. serv_Categoria: 'Vacuna' | 'Laboratorio' | 'Procedimiento' | 'Consulta'
IF COL_LENGTH('Veterinaria.tbServicios', 'serv_Categoria') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbServicios]
        ADD serv_Categoria NVARCHAR(20) NOT NULL
            CONSTRAINT DF_serv_Categoria DEFAULT 'Procedimiento';
    PRINT 'serv_Categoria agregado';
END
GO

-- 2b. serv_EsActivo
IF COL_LENGTH('Veterinaria.tbServicios', 'serv_EsActivo') IS NULL
BEGIN
    ALTER TABLE [Veterinaria].[tbServicios]
        ADD serv_EsActivo BIT NOT NULL
            CONSTRAINT DF_serv_EsActivo DEFAULT 1;
    PRINT 'serv_EsActivo agregado';
END
GO

-- 2c. SEED demo (idempotente por serv_Codigo). Precios realistas en Lempiras.
MERGE [Veterinaria].[tbServicios] AS t
USING (VALUES
    -- Vacunas
    ('VAC-RAB', 'Vacuna Rabia',          'Vacuna antirrabica anual',                 350.00, 'Vacuna'),
    ('VAC-QNT', 'Vacuna Quintuple',       'Vacuna multiple (quintuple canina)',       450.00, 'Vacuna'),
    ('VAC-TRP', 'Vacuna Triple Felina',   'Vacuna triple felina',                     420.00, 'Vacuna'),
    -- Laboratorio
    ('LAB-HEM', 'Hemograma completo',     'Conteo sanguineo completo',                600.00, 'Laboratorio'),
    ('LAB-QUI', 'Quimica sanguinea',      'Perfil bioquimico sanguineo',              850.00, 'Laboratorio'),
    ('LAB-COP', 'Coprologico',            'Analisis coprologico de heces',            250.00, 'Laboratorio'),
    -- Procedimiento
    ('PRO-CON', 'Consulta general',       'Consulta clinica general',                 300.00, 'Procedimiento'),
    ('PRO-DES', 'Desparasitacion',        'Desparasitacion interna/externa',          200.00, 'Procedimiento'),
    ('PRO-CUR', 'Curacion',               'Curacion y limpieza de herida',            180.00, 'Procedimiento')
) AS s (serv_Codigo, serv_Nombre, serv_Descripcion, serv_Precio, serv_Categoria)
ON (t.serv_Codigo = s.serv_Codigo)
WHEN NOT MATCHED BY TARGET THEN
    INSERT (serv_Codigo, serv_Nombre, serv_Descripcion, serv_Precio, serv_Categoria,
            serv_EsActivo, serv_EsEliminado, serv_UsuarioCrea, serv_FechaCrea)
    VALUES (s.serv_Codigo, s.serv_Nombre, s.serv_Descripcion, s.serv_Precio, s.serv_Categoria,
            1, 0, 1, GETDATE());
GO

-- =============================================================================
-- PASO 3: SP EncolarServicio (idempotente por (cita_Id, serv_Id))
-- =============================================================================
IF OBJECT_ID('Veterinaria.PR_Veterinaria_CargoDispensacion_EncolarServicio', 'P') IS NOT NULL
    DROP PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_EncolarServicio];
GO
CREATE PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_EncolarServicio]
    @serv_Id               INT,
    @masc_Id               INT,
    @cli_Id                INT,
    @cita_Id               INT = NULL,
    @cargo_Cantidad        INT = 1,
    @cargo_UsuarioCrea     INT,
    @cargo_PrecioUnitSnap  DECIMAL(18,2) = NULL,  -- si NULL -> serv_Precio
    @cargo_DescripcionSnap NVARCHAR(500) = NULL   -- si NULL -> serv_Nombre
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        -- Idempotencia por visita: no duplicar el mismo servicio en la misma cita.
        IF @cita_Id IS NOT NULL
        BEGIN
            DECLARE @existente INT =
                (SELECT TOP 1 cargo_Id FROM [Veterinaria].[tbCargoDispensacion]
                 WHERE cita_Id = @cita_Id
                   AND cargo_TipoOrigen = 'Servicio'
                   AND cargo_OrigenId = @serv_Id
                   AND cargo_EsEliminado = 0);

            IF @existente IS NOT NULL
            BEGIN
                SELECT @existente AS cargo_Id, 1 AS CodeStatus,
                       'Cargo de servicio ya existia (idempotente).' AS MessageStatus
                RETURN
            END
        END

        -- Snapshot del precio/descripcion vigentes del servicio.
        DECLARE @precioSnap DECIMAL(18,2) =
            COALESCE(@cargo_PrecioUnitSnap, (SELECT serv_Precio FROM [Veterinaria].[tbServicios] WHERE serv_Id = @serv_Id));
        DECLARE @descSnap NVARCHAR(500) =
            COALESCE(@cargo_DescripcionSnap, (SELECT serv_Nombre FROM [Veterinaria].[tbServicios] WHERE serv_Id = @serv_Id));

        INSERT INTO [Veterinaria].[tbCargoDispensacion]
            (receta_Id, masc_Id, cli_Id, cita_Id, itm_Id, cargo_Cantidad,
             cargo_DescripcionSnap, cargo_PrecioUnitSnap, cargo_Estado,
             cargo_TipoOrigen, cargo_OrigenId,
             cargo_UsuarioCrea, cargo_FechaCrea)
        VALUES
            (NULL, @masc_Id, @cli_Id, @cita_Id, NULL, @cargo_Cantidad,
             @descSnap, @precioSnap, 'Pendiente',
             'Servicio', @serv_Id,
             @cargo_UsuarioCrea, GETDATE())

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS cargo_Id, 1 AS CodeStatus, 'Cargo de servicio encolado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT CAST(NULL AS INT) AS cargo_Id, -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =============================================================================
-- PASO 4: Actualizar Encolar de medicamentos para setear TipoOrigen/OrigenId
--         (misma firma, misma idempotencia por receta_Id)
-- =============================================================================
IF OBJECT_ID('Veterinaria.PR_Veterinaria_CargoDispensacion_Encolar', 'P') IS NOT NULL
    DROP PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_Encolar];
GO
CREATE PROCEDURE [Veterinaria].[PR_Veterinaria_CargoDispensacion_Encolar]
    @receta_Id            INT,
    @masc_Id              INT,
    @cli_Id               INT,
    @cita_Id              INT = NULL,
    @itm_Id               INT,
    @cargo_Cantidad       INT,
    @cargo_UsuarioCrea    INT,
    @cargo_PrecioUnitSnap DECIMAL(18,2) = NULL,  -- si NULL, se toma de tbItems
    @cargo_DescripcionSnap NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        DECLARE @existente INT =
            (SELECT TOP 1 cargo_Id FROM [Veterinaria].[tbCargoDispensacion]
             WHERE receta_Id = @receta_Id AND cargo_EsEliminado = 0);

        IF @existente IS NOT NULL
        BEGIN
            SELECT @existente AS cargo_Id, 1 AS CodeStatus, 'Cargo ya existia (idempotente).' AS MessageStatus
            RETURN
        END

        -- Snapshot: si no se envia, se congela el valor VIGENTE del item.
        DECLARE @precioSnap DECIMAL(18,2) =
            COALESCE(@cargo_PrecioUnitSnap, (SELECT itm_Precio FROM [Inventario].[tbItems] WHERE itm_Id = @itm_Id));
        DECLARE @descSnap NVARCHAR(500) =
            COALESCE(@cargo_DescripcionSnap, (SELECT itm_Descripcion FROM [Inventario].[tbItems] WHERE itm_Id = @itm_Id));

        INSERT INTO [Veterinaria].[tbCargoDispensacion]
            (receta_Id, masc_Id, cli_Id, cita_Id, itm_Id, cargo_Cantidad,
             cargo_DescripcionSnap, cargo_PrecioUnitSnap, cargo_Estado,
             cargo_TipoOrigen, cargo_OrigenId,
             cargo_UsuarioCrea, cargo_FechaCrea)
        VALUES
            (@receta_Id, @masc_Id, @cli_Id, @cita_Id, @itm_Id, @cargo_Cantidad,
             @descSnap, @precioSnap, 'Pendiente',
             'Medicamento', @receta_Id,
             @cargo_UsuarioCrea, GETDATE())

        SELECT CAST(SCOPE_IDENTITY() AS INT) AS cargo_Id, 1 AS CodeStatus, 'Cargo encolado.' AS MessageStatus
    END TRY
    BEGIN CATCH
        SELECT CAST(NULL AS INT) AS cargo_Id, -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- =============================================================================
-- PASO 5: SP Servicio_Dropdown (picker de servicios por categoria)
-- =============================================================================
IF OBJECT_ID('Veterinaria.PR_Veterinaria_Servicio_Dropdown', 'P') IS NOT NULL
    DROP PROCEDURE [Veterinaria].[PR_Veterinaria_Servicio_Dropdown];
GO
CREATE PROCEDURE [Veterinaria].[PR_Veterinaria_Servicio_Dropdown]
    @categoria NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON
    SELECT serv_Id, serv_Nombre, serv_Precio, serv_Categoria
    FROM [Veterinaria].[tbServicios]
    WHERE serv_EsEliminado = 0
      AND serv_EsActivo = 1
      AND (@categoria IS NULL OR serv_Categoria = @categoria)
    ORDER BY serv_Categoria, serv_Nombre
END
GO
