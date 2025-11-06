-- =============================================
-- Script: Stored Procedures - tbGravedades
-- Autor: Claude Code
-- Fecha: 2025-10-31
-- =============================================

USE PETSHOMEDB
GO

-- List
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_Gravedades_List]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_Gravedades_List]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Gravedades_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT grav_Id, grav_Descripcion
    FROM [Medico].[tbGravedades]
    WHERE grav_EsEliminado = 0
    ORDER BY grav_Id ASC
END
GO

-- Detail
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_Gravedades_Detail]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_Gravedades_Detail]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Gravedades_Detail]
    @grav_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  g.grav_Id, g.grav_Descripcion,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            g.grav_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            g.grav_FechaModifica
    FROM [Medico].[tbGravedades] AS g
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON g.grav_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON g.grav_UsuarioModifica = usuarioModifica.usu_Id
    WHERE g.grav_EsEliminado = 0 AND g.grav_Id = @grav_Id
END
GO

-- Find
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_Gravedades_Find]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_Gravedades_Find]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Gravedades_Find]
    @grav_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  g.grav_Id, g.grav_Descripcion,
            g.grav_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            g.grav_FechaCrea,
            g.grav_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            g.grav_FechaModifica
    FROM [Medico].[tbGravedades] AS g
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON g.grav_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON g.grav_UsuarioModifica = usuarioModifica.usu_Id
    WHERE g.grav_EsEliminado = 0 AND g.grav_Id = @grav_Id
END
GO

-- Insert
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_Gravedades_Insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_Gravedades_Insert]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Gravedades_Insert]
    @grav_Descripcion NVARCHAR(50),
    @grav_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        INSERT INTO [Medico].[tbGravedades] (grav_Descripcion, grav_UsuarioCrea, grav_FechaCrea)
        VALUES (@grav_Descripcion, @grav_UsuarioCrea, GETDATE())
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_Gravedades_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_Gravedades_Update]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Gravedades_Update]
    @grav_Id INT,
    @grav_Descripcion NVARCHAR(50),
    @grav_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbGravedades]
        SET grav_Descripcion = @grav_Descripcion,
            grav_UsuarioModifica = @grav_UsuarioModifica,
            grav_FechaModifica = GETDATE()
        WHERE grav_Id = @grav_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_Gravedades_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_Gravedades_Delete]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Gravedades_Delete]
    @grav_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbGravedades]
        SET grav_EsEliminado = 1
        WHERE grav_Id = @grav_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Dropdown
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_Gravedades_Dropdown]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_Gravedades_Dropdown]
GO

CREATE PROCEDURE [Medico].[PR_Medico_Gravedades_Dropdown]
AS
BEGIN
    SET NOCOUNT ON
    SELECT grav_Id, grav_Descripcion
    FROM [Medico].[tbGravedades]
    WHERE grav_EsEliminado = 0
    ORDER BY grav_Id ASC
END
GO

PRINT 'Stored Procedures para tbGravedades creados exitosamente.'
GO
