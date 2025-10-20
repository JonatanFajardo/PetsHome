-- Script para actualizar los stored procedures de Raza con los nuevos campos
-- Fecha: 2025-10-19

USE PETSHOMEDB
GO

-- =============================================
-- Stored Procedure: PR_Refugio_Razas_Insert
-- Descripción: Insertar una nueva raza con los campos adicionales
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_Insert')
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_Insert]
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

-- =============================================
-- Stored Procedure: PR_Refugio_Razas_Update
-- Descripción: Actualizar una raza existente con los campos adicionales
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_Update')
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_Update]
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

-- =============================================
-- Stored Procedure: PR_Refugio_Razas_List
-- Descripción: Listar todas las razas incluyendo los nuevos campos
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_List')
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_List]
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

-- =============================================
-- Stored Procedure: PR_Refugio_Razas_Find
-- Descripción: Buscar una raza por ID incluyendo los nuevos campos
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_Find')
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_Find]
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

-- =============================================
-- Stored Procedure: PR_Refugio_Razas_Detail
-- Descripción: Obtener detalle completo de una raza incluyendo auditoría
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'PR_Refugio_Razas_Detail')
    DROP PROCEDURE [Refugio].[PR_Refugio_Razas_Detail]
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

PRINT 'Los stored procedures de Raza se actualizaron correctamente'
GO
