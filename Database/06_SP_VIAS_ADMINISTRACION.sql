-- =============================================
-- Script: Stored Procedures - tbViasAdministracion
-- Autor: Claude Code
-- Fecha: 2025-10-31
-- =============================================

USE PETSHOMEDB
GO

-- List
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_ViasAdministracion_List]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_ViasAdministracion_List]
GO
CREATE PROCEDURE [Medico].[PR_Medico_ViasAdministracion_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT viaAdmin_Id, viaAdmin_Descripcion
    FROM [Medico].[tbViasAdministracion]
    WHERE viaAdmin_EsEliminado = 0
    ORDER BY viaAdmin_Descripcion ASC
END
GO

-- Detail
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_ViasAdministracion_Detail]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Detail]
GO
CREATE PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Detail]
    @viaAdmin_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  va.viaAdmin_Id, va.viaAdmin_Descripcion,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            va.viaAdmin_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            va.viaAdmin_FechaModifica
    FROM [Medico].[tbViasAdministracion] AS va
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON va.viaAdmin_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON va.viaAdmin_UsuarioModifica = usuarioModifica.usu_Id
    WHERE va.viaAdmin_EsEliminado = 0 AND va.viaAdmin_Id = @viaAdmin_Id
END
GO

-- Find
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_ViasAdministracion_Find]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Find]
GO
CREATE PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Find]
    @viaAdmin_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  va.viaAdmin_Id, va.viaAdmin_Descripcion,
            va.viaAdmin_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            va.viaAdmin_FechaCrea,
            va.viaAdmin_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            va.viaAdmin_FechaModifica
    FROM [Medico].[tbViasAdministracion] AS va
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON va.viaAdmin_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON va.viaAdmin_UsuarioModifica = usuarioModifica.usu_Id
    WHERE va.viaAdmin_EsEliminado = 0 AND va.viaAdmin_Id = @viaAdmin_Id
END
GO

-- Insert
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_ViasAdministracion_Insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Insert]
GO
CREATE PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Insert]
    @viaAdmin_Descripcion NVARCHAR(100),
    @viaAdmin_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        INSERT INTO [Medico].[tbViasAdministracion] (viaAdmin_Descripcion, viaAdmin_UsuarioCrea, viaAdmin_FechaCrea)
        VALUES (@viaAdmin_Descripcion, @viaAdmin_UsuarioCrea, GETDATE())
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_ViasAdministracion_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Update]
GO
CREATE PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Update]
    @viaAdmin_Id INT,
    @viaAdmin_Descripcion NVARCHAR(100),
    @viaAdmin_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbViasAdministracion]
        SET viaAdmin_Descripcion = @viaAdmin_Descripcion,
            viaAdmin_UsuarioModifica = @viaAdmin_UsuarioModifica,
            viaAdmin_FechaModifica = GETDATE()
        WHERE viaAdmin_Id = @viaAdmin_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_ViasAdministracion_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Delete]
GO
CREATE PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Delete]
    @viaAdmin_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbViasAdministracion]
        SET viaAdmin_EsEliminado = 1
        WHERE viaAdmin_Id = @viaAdmin_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Dropdown
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_ViasAdministracion_Dropdown]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Dropdown]
GO
CREATE PROCEDURE [Medico].[PR_Medico_ViasAdministracion_Dropdown]
AS
BEGIN
    SET NOCOUNT ON
    SELECT viaAdmin_Id, viaAdmin_Descripcion
    FROM [Medico].[tbViasAdministracion]
    WHERE viaAdmin_EsEliminado = 0
    ORDER BY viaAdmin_Descripcion ASC
END
GO

PRINT 'Stored Procedures para tbViasAdministracion creados exitosamente.'
GO
