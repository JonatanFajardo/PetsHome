-- =============================================
-- Script: Stored Procedures - tbTiposMedicamento
-- Autor: Claude Code
-- Fecha: 2025-10-31
-- =============================================

USE PETSHOMEDB
GO

-- List
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposMedicamento_List]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposMedicamento_List]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposMedicamento_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT tipoMed_Id, tipoMed_Descripcion
    FROM [Medico].[tbTiposMedicamento]
    WHERE tipoMed_EsEliminado = 0
    ORDER BY tipoMed_Descripcion ASC
END
GO

-- Detail
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposMedicamento_Detail]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Detail]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Detail]
    @tipoMed_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  tm.tipoMed_Id, tm.tipoMed_Descripcion,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            tm.tipoMed_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            tm.tipoMed_FechaModifica
    FROM [Medico].[tbTiposMedicamento] AS tm
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON tm.tipoMed_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON tm.tipoMed_UsuarioModifica = usuarioModifica.usu_Id
    WHERE tm.tipoMed_EsEliminado = 0 AND tm.tipoMed_Id = @tipoMed_Id
END
GO

-- Find
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposMedicamento_Find]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Find]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Find]
    @tipoMed_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  tm.tipoMed_Id, tm.tipoMed_Descripcion,
            tm.tipoMed_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            tm.tipoMed_FechaCrea,
            tm.tipoMed_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            tm.tipoMed_FechaModifica
    FROM [Medico].[tbTiposMedicamento] AS tm
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON tm.tipoMed_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON tm.tipoMed_UsuarioModifica = usuarioModifica.usu_Id
    WHERE tm.tipoMed_EsEliminado = 0 AND tm.tipoMed_Id = @tipoMed_Id
END
GO

-- Insert
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposMedicamento_Insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Insert]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Insert]
    @tipoMed_Descripcion NVARCHAR(100),
    @tipoMed_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea, tipoMed_FechaCrea)
        VALUES (@tipoMed_Descripcion, @tipoMed_UsuarioCrea, GETDATE())
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposMedicamento_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Update]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Update]
    @tipoMed_Id INT,
    @tipoMed_Descripcion NVARCHAR(100),
    @tipoMed_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbTiposMedicamento]
        SET tipoMed_Descripcion = @tipoMed_Descripcion,
            tipoMed_UsuarioModifica = @tipoMed_UsuarioModifica,
            tipoMed_FechaModifica = GETDATE()
        WHERE tipoMed_Id = @tipoMed_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposMedicamento_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Delete]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Delete]
    @tipoMed_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbTiposMedicamento]
        SET tipoMed_EsEliminado = 1
        WHERE tipoMed_Id = @tipoMed_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Dropdown
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposMedicamento_Dropdown]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Dropdown]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposMedicamento_Dropdown]
AS
BEGIN
    SET NOCOUNT ON
    SELECT tipoMed_Id, tipoMed_Descripcion
    FROM [Medico].[tbTiposMedicamento]
    WHERE tipoMed_EsEliminado = 0
    ORDER BY tipoMed_Descripcion ASC
END
GO

PRINT 'Stored Procedures para tbTiposMedicamento creados exitosamente.'
GO
