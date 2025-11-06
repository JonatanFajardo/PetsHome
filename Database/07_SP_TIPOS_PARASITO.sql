-- =============================================
-- Script: Stored Procedures - tbTiposParasito
-- Autor: Claude Code
-- Fecha: 2025-10-31
-- =============================================

USE PETSHOMEDB
GO

-- List
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposParasito_List]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposParasito_List]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposParasito_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT tipoPar_Id, tipoPar_Descripcion, tipoPar_Categoria
    FROM [Medico].[tbTiposParasito]
    WHERE tipoPar_EsEliminado = 0
    ORDER BY tipoPar_Categoria ASC, tipoPar_Descripcion ASC
END
GO

-- Detail
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposParasito_Detail]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposParasito_Detail]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposParasito_Detail]
    @tipoPar_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  tp.tipoPar_Id, tp.tipoPar_Descripcion, tp.tipoPar_Categoria,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            tp.tipoPar_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            tp.tipoPar_FechaModifica
    FROM [Medico].[tbTiposParasito] AS tp
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON tp.tipoPar_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON tp.tipoPar_UsuarioModifica = usuarioModifica.usu_Id
    WHERE tp.tipoPar_EsEliminado = 0 AND tp.tipoPar_Id = @tipoPar_Id
END
GO

-- Find
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposParasito_Find]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposParasito_Find]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposParasito_Find]
    @tipoPar_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  tp.tipoPar_Id, tp.tipoPar_Descripcion, tp.tipoPar_Categoria,
            tp.tipoPar_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            tp.tipoPar_FechaCrea,
            tp.tipoPar_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            tp.tipoPar_FechaModifica
    FROM [Medico].[tbTiposParasito] AS tp
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea ON tp.tipoPar_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON tp.tipoPar_UsuarioModifica = usuarioModifica.usu_Id
    WHERE tp.tipoPar_EsEliminado = 0 AND tp.tipoPar_Id = @tipoPar_Id
END
GO

-- Insert
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposParasito_Insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposParasito_Insert]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposParasito_Insert]
    @tipoPar_Descripcion NVARCHAR(100),
    @tipoPar_Categoria NVARCHAR(50),
    @tipoPar_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        INSERT INTO [Medico].[tbTiposParasito] (tipoPar_Descripcion, tipoPar_Categoria, tipoPar_UsuarioCrea, tipoPar_FechaCrea)
        VALUES (@tipoPar_Descripcion, @tipoPar_Categoria, @tipoPar_UsuarioCrea, GETDATE())
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Update
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposParasito_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposParasito_Update]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposParasito_Update]
    @tipoPar_Id INT,
    @tipoPar_Descripcion NVARCHAR(100),
    @tipoPar_Categoria NVARCHAR(50),
    @tipoPar_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbTiposParasito]
        SET tipoPar_Descripcion = @tipoPar_Descripcion,
            tipoPar_Categoria = @tipoPar_Categoria,
            tipoPar_UsuarioModifica = @tipoPar_UsuarioModifica,
            tipoPar_FechaModifica = GETDATE()
        WHERE tipoPar_Id = @tipoPar_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Delete
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposParasito_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposParasito_Delete]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposParasito_Delete]
    @tipoPar_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        UPDATE [Medico].[tbTiposParasito]
        SET tipoPar_EsEliminado = 1
        WHERE tipoPar_Id = @tipoPar_Id
        RETURN 1
    END TRY
    BEGIN CATCH
        RETURN 0
    END CATCH
END
GO

-- Dropdown
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposParasito_Dropdown]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposParasito_Dropdown]
GO
CREATE PROCEDURE [Medico].[PR_Medico_TiposParasito_Dropdown]
AS
BEGIN
    SET NOCOUNT ON
    SELECT tipoPar_Id, tipoPar_Descripcion, tipoPar_Categoria
    FROM [Medico].[tbTiposParasito]
    WHERE tipoPar_EsEliminado = 0
    ORDER BY tipoPar_Categoria ASC, tipoPar_Descripcion ASC
END
GO

PRINT 'Stored Procedures para tbTiposParasito creados exitosamente.'
GO
