-- =============================================
-- Script Maestro para Actualización de Tabla Razas
-- Fecha: 2025-10-19
-- Descripción: Ejecuta todos los cambios necesarios en orden
-- =============================================

USE PETSHOMEDB
GO

PRINT '=========================================='
PRINT 'INICIANDO ACTUALIZACIÓN DE TABLA RAZAS'
PRINT '=========================================='
PRINT ''

-- =============================================
-- PASO 1: Agregar nuevas columnas a la tabla
-- =============================================
PRINT 'PASO 1: Agregando nuevas columnas a Refugio.tbRazas...'
PRINT ''

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Refugio].[tbRazas]') AND name = 'raza_Tamano')
BEGIN
    ALTER TABLE [Refugio].[tbRazas]
    ADD raza_Tamano VARCHAR(20) NULL
    PRINT '  ✓ Campo raza_Tamano agregado correctamente'
END
ELSE
    PRINT '  - Campo raza_Tamano ya existe'
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Refugio].[tbRazas]') AND name = 'raza_TipoAnimal')
BEGIN
    ALTER TABLE [Refugio].[tbRazas]
    ADD raza_TipoAnimal VARCHAR(50) NULL
    PRINT '  ✓ Campo raza_TipoAnimal agregado correctamente'
END
ELSE
    PRINT '  - Campo raza_TipoAnimal ya existe'
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Refugio].[tbRazas]') AND name = 'raza_TipoPelaje')
BEGIN
    ALTER TABLE [Refugio].[tbRazas]
    ADD raza_TipoPelaje VARCHAR(30) NULL
    PRINT '  ✓ Campo raza_TipoPelaje agregado correctamente'
END
ELSE
    PRINT '  - Campo raza_TipoPelaje ya existe'
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Refugio].[tbRazas]') AND name = 'raza_ImagenUrl')
BEGIN
    ALTER TABLE [Refugio].[tbRazas]
    ADD raza_ImagenUrl VARCHAR(500) NULL
    PRINT '  ✓ Campo raza_ImagenUrl agregado correctamente'
END
ELSE
    PRINT '  - Campo raza_ImagenUrl ya existe'
GO

PRINT ''
PRINT 'PASO 1 COMPLETADO'
PRINT ''

-- =============================================
-- PASO 2: Actualizar Stored Procedures
-- =============================================
PRINT 'PASO 2: Actualizando Stored Procedures...'
PRINT ''

-- PR_Refugio_Razas_Insert
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_Insert')
BEGIN
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_Insert]
    PRINT '  - Stored Procedure PR_Refugio_Razas_Insert eliminado para recreación'
END
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Razas_Insert]
    @raza_Descripcion VARCHAR(50),
    @raza_Tamano VARCHAR(20) = NULL,
    @raza_TipoAnimal VARCHAR(50) = NULL,
    @raza_TipoPelaje VARCHAR(30) = NULL,
    @raza_ImagenUrl VARCHAR(500) = NULL,
    @raza_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO [Refugio].[tbRazas]
        (
            raza_Descripcion,
            raza_Tamano,
            raza_TipoAnimal,
            raza_TipoPelaje,
            raza_ImagenUrl,
            raza_EsEliminado,
            raza_UsuarioCrea,
            raza_FechaCrea
        )
        VALUES
        (
            @raza_Descripcion,
            @raza_Tamano,
            @raza_TipoAnimal,
            @raza_TipoPelaje,
            @raza_ImagenUrl,
            0,
            @raza_UsuarioCrea,
            GETDATE()
        )

        SELECT 1 AS Success
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success
    END CATCH
END
GO
PRINT '  ✓ PR_Refugio_Razas_Insert creado correctamente'

-- PR_Refugio_Razas_Update
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_Update')
BEGIN
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_Update]
    PRINT '  - Stored Procedure PR_Refugio_Razas_Update eliminado para recreación'
END
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Razas_Update]
    @raza_Id INT,
    @raza_Descripcion VARCHAR(50),
    @raza_Tamano VARCHAR(20) = NULL,
    @raza_TipoAnimal VARCHAR(50) = NULL,
    @raza_TipoPelaje VARCHAR(30) = NULL,
    @raza_ImagenUrl VARCHAR(500) = NULL,
    @raza_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE [Refugio].[tbRazas]
        SET
            raza_Descripcion = @raza_Descripcion,
            raza_Tamano = @raza_Tamano,
            raza_TipoAnimal = @raza_TipoAnimal,
            raza_TipoPelaje = @raza_TipoPelaje,
            raza_ImagenUrl = @raza_ImagenUrl,
            raza_UsuarioModifica = @raza_UsuarioModifica,
            raza_FechaModifica = GETDATE()
        WHERE
            raza_Id = @raza_Id

        SELECT 1 AS Success
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success
    END CATCH
END
GO
PRINT '  ✓ PR_Refugio_Razas_Update creado correctamente'

-- PR_Refugio_Razas_List
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_List')
BEGIN
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_List]
    PRINT '  - Stored Procedure PR_Refugio_Razas_List eliminado para recreación'
END
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Razas_List]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        raza_Id,
        raza_Descripcion,
        raza_Tamano,
        raza_TipoAnimal,
        raza_TipoPelaje,
        raza_ImagenUrl
    FROM [Refugio].[tbRazas]
    WHERE raza_EsEliminado = 0
    ORDER BY raza_Descripcion
END
GO
PRINT '  ✓ PR_Refugio_Razas_List creado correctamente'

-- PR_Refugio_Razas_Find
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_Find')
BEGIN
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_Find]
    PRINT '  - Stored Procedure PR_Refugio_Razas_Find eliminado para recreación'
END
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Razas_Find]
    @raza_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        raza_Id,
        raza_Descripcion,
        raza_Tamano,
        raza_TipoAnimal,
        raza_TipoPelaje,
        raza_ImagenUrl
    FROM [Refugio].[tbRazas]
    WHERE raza_Id = @raza_Id
        AND raza_EsEliminado = 0
END
GO
PRINT '  ✓ PR_Refugio_Razas_Find creado correctamente'

-- PR_Refugio_Razas_Detail
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_Detail')
BEGIN
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_Detail]
    PRINT '  - Stored Procedure PR_Refugio_Razas_Detail eliminado para recreación'
END
GO

CREATE PROCEDURE [Refugio].[PR_Refugio_Razas_Detail]
    @raza_Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.raza_Id,
        r.raza_Descripcion,
        r.raza_Tamano,
        r.raza_TipoAnimal,
        r.raza_TipoPelaje,
        r.raza_ImagenUrl,
        r.raza_UsuarioCrea,
        uc.usua_Usuario as raza_NombreUsuarioCrea,
        r.raza_FechaCrea,
        r.raza_UsuarioModifica,
        um.usua_Usuario as raza_NombreUsuarioModifica,
        r.raza_FechaModifica
    FROM [Refugio].[tbRazas] r
    LEFT JOIN [Acce].[tbUsuarios] uc ON r.raza_UsuarioCrea = uc.usua_Id
    LEFT JOIN [Acce].[tbUsuarios] um ON r.raza_UsuarioModifica = um.usua_Id
    WHERE r.raza_Id = @raza_Id
        AND r.raza_EsEliminado = 0
END
GO
PRINT '  ✓ PR_Refugio_Razas_Detail creado correctamente'

PRINT ''
PRINT 'PASO 2 COMPLETADO'
PRINT ''

-- =============================================
-- PASO 3: Verificación
-- =============================================
PRINT 'PASO 3: Verificando cambios...'
PRINT ''

PRINT 'Columnas de la tabla Refugio.tbRazas:'
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'Refugio'
    AND TABLE_NAME = 'tbRazas'
ORDER BY ORDINAL_POSITION

PRINT ''
PRINT 'Stored Procedures actualizados:'
SELECT
    ROUTINE_NAME,
    CREATED,
    LAST_ALTERED
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'Refugio'
    AND ROUTINE_NAME LIKE '%Razas%'
ORDER BY ROUTINE_NAME

PRINT ''
PRINT '=========================================='
PRINT 'ACTUALIZACIÓN COMPLETADA EXITOSAMENTE'
PRINT '=========================================='
PRINT ''
PRINT 'Próximos pasos:'
PRINT '1. Compilar la solución: dotnet build PetsHome.sln'
PRINT '2. Ejecutar la aplicación: dotnet run --project PetsHome.UI/PetsHome.UI.csproj'
PRINT ''
GO
