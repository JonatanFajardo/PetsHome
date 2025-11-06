-- =============================================
-- Script: Stored Procedures - tbTiposEsterilizacion
-- Autor: Claude Code
-- Fecha: 2025-10-31
-- =============================================

USE PETSHOMEDB
GO

-- List
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposEsterilizacion_List]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_List]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT tipoEst_Id, tipoEst_Descripcion, tipoEst_Sexo
    FROM [Medico].[tbTiposEsterilizacion]
    WHERE tipoEst_EsEliminado = 0
    ORDER BY tipoEst_Sexo ASC, tipoEst_Descripcion ASC
END
GO

-- Detail
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposEsterilizacion_Detail]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Detail]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Detail]
    @tipoEst_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  te.tipoEst_Id, te.tipoEst_Descripcion, te.tipoEst_Sexo,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            te.tipoEst_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            te.tipoEst_FechaModifica
    FROM [Medico].[tbTiposEsterilizacion] AS te
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON te.tipoEst_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON te.tipoEst_UsuarioModifica = usuarioModifica.usu_Id
    WHERE te.tipoEst_EsEliminado = 0 AND te.tipoEst_Id = @tipoEst_Id
END
GO

-- Find
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposEsterilizacion_Find]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Find]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Find]
    @tipoEst_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  te.tipoEst_Id, te.tipoEst_Descripcion, te.tipoEst_Sexo,
            te.tipoEst_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            te.tipoEst_FechaCrea,
            te.tipoEst_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            te.tipoEst_FechaModifica
    FROM [Medico].[tbTiposEsterilizacion] AS te
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON te.tipoEst_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON te.tipoEst_UsuarioModifica = usuarioModifica.usu_Id
    WHERE te.tipoEst_EsEliminado = 0 AND te.tipoEst_Id = @tipoEst_Id
END
GO

-- Insert
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposEsterilizacion_Insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Insert]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Insert]
    @tipoEst_Descripcion NVARCHAR(100),
    @tipoEst_Sexo NVARCHAR(10),
    @tipoEst_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        INSERT INTO [Medico].[tbTiposEsterilizacion] (tipoEst_Descripcion, tipoEst_Sexo, tipoEst_UsuarioCrea, tipoEst_FechaCrea)
        VALUES (@tipoEst_Descripcion, @tipoEst_Sexo, @tipoEst_UsuarioCrea, GETDATE())
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposEsterilizacion_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Update]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Update]
    @tipoEst_Id INT,
    @tipoEst_Descripcion NVARCHAR(100),
    @tipoEst_Sexo NVARCHAR(10),
    @tipoEst_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbTiposEsterilizacion]
        SET tipoEst_Descripcion = @tipoEst_Descripcion,
            tipoEst_Sexo = @tipoEst_Sexo,
            tipoEst_UsuarioModifica = @tipoEst_UsuarioModifica,
            tipoEst_FechaModifica = GETDATE()
        WHERE tipoEst_Id = @tipoEst_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposEsterilizacion_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Delete]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Delete]
    @tipoEst_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbTiposEsterilizacion]
        SET tipoEst_EsEliminado = 1
        WHERE tipoEst_Id = @tipoEst_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Dropdown
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposEsterilizacion_Dropdown]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Dropdown]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposEsterilizacion_Dropdown]
AS
BEGIN
    SET NOCOUNT ON
    SELECT tipoEst_Id, tipoEst_Descripcion, tipoEst_Sexo
    FROM [Medico].[tbTiposEsterilizacion]
    WHERE tipoEst_EsEliminado = 0
    ORDER BY tipoEst_Sexo ASC, tipoEst_Descripcion ASC
END
GO

PRINT 'Stored Procedures para tbTiposEsterilizacion creados exitosamente.'
GO
