-- =============================================
-- Script: Stored Procedures - tbTiposConsulta
-- Autor: Claude Code
-- Fecha: 2025-10-31
-- =============================================

USE PETSHOMEDB
GO

-- =============================================
-- SP: PR_Medico_TiposConsulta_List
-- Descripción: Lista todos los tipos de consulta activos
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposConsulta_List]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposConsulta_List]
GO

CREATE PROCEDURE [Medico].[PR_Medico_TiposConsulta_List]
AS
BEGIN
    SET NOCOUNT ON

    SELECT  tipoCon_Id,
            tipoCon_Descripcion
    FROM [Medico].[tbTiposConsulta]
    WHERE tipoCon_EsEliminado = 0
    ORDER BY tipoCon_Descripcion ASC
END
GO

-- =============================================
-- SP: PR_Medico_TiposConsulta_Detail
-- Descripción: Obtiene el detalle de un tipo de consulta con información de auditoría
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposConsulta_Detail]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposConsulta_Detail]
GO

CREATE PROCEDURE [Medico].[PR_Medico_TiposConsulta_Detail]
    @tipoCon_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT  tc.tipoCon_Id,
            tc.tipoCon_Descripcion,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            tc.tipoCon_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            tc.tipoCon_FechaModifica
    FROM [Medico].[tbTiposConsulta] AS tc
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON tc.tipoCon_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON tc.tipoCon_UsuarioModifica = usuarioModifica.usu_Id
    WHERE tc.tipoCon_EsEliminado = 0
      AND tc.tipoCon_Id = @tipoCon_Id
END
GO

-- =============================================
-- SP: PR_Medico_TiposConsulta_Find
-- Descripción: Busca un tipo de consulta por ID para edición
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposConsulta_Find]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposConsulta_Find]
GO

CREATE PROCEDURE [Medico].[PR_Medico_TiposConsulta_Find]
    @tipoCon_Id INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT  tc.tipoCon_Id,
            tc.tipoCon_Descripcion,
            tc.tipoCon_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            tc.tipoCon_FechaCrea,
            tc.tipoCon_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            tc.tipoCon_FechaModifica
    FROM [Medico].[tbTiposConsulta] AS tc
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON tc.tipoCon_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON tc.tipoCon_UsuarioModifica = usuarioModifica.usu_Id
    WHERE tc.tipoCon_EsEliminado = 0
      AND tc.tipoCon_Id = @tipoCon_Id
END
GO

-- =============================================
-- SP: PR_Medico_TiposConsulta_Insert
-- Descripción: Inserta un nuevo tipo de consulta
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposConsulta_Insert]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposConsulta_Insert]
GO

CREATE PROCEDURE [Medico].[PR_Medico_TiposConsulta_Insert]
    @tipoCon_Descripcion NVARCHAR(100),
    @tipoCon_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON

    BEGIN TRY
        INSERT INTO [Medico].[tbTiposConsulta]
        (
            tipoCon_Descripcion,
            tipoCon_UsuarioCrea,
            tipoCon_FechaCrea
        )
        VALUES
        (
            @tipoCon_Descripcion,
            @tipoCon_UsuarioCrea,
            GETDATE()
        )

        RETURN 1 -- Éxito
    END TRY
    BEGIN CATCH
        RETURN 0 -- Error
    END CATCH
END
GO

-- =============================================
-- SP: PR_Medico_TiposConsulta_Update
-- Descripción: Actualiza un tipo de consulta existente
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposConsulta_Update]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposConsulta_Update]
GO

CREATE PROCEDURE [Medico].[PR_Medico_TiposConsulta_Update]
    @tipoCon_Id INT,
    @tipoCon_Descripcion NVARCHAR(100),
    @tipoCon_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON

    BEGIN TRY
        UPDATE [Medico].[tbTiposConsulta]
        SET tipoCon_Descripcion = @tipoCon_Descripcion,
            tipoCon_UsuarioModifica = @tipoCon_UsuarioModifica,
            tipoCon_FechaModifica = GETDATE()
        WHERE tipoCon_Id = @tipoCon_Id

        RETURN 1 -- Éxito
    END TRY
    BEGIN CATCH
        RETURN 0 -- Error
    END CATCH
END
GO

-- =============================================
-- SP: PR_Medico_TiposConsulta_Delete
-- Descripción: Elimina lógicamente un tipo de consulta
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposConsulta_Delete]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposConsulta_Delete]
GO

CREATE PROCEDURE [Medico].[PR_Medico_TiposConsulta_Delete]
    @tipoCon_Id INT
AS
BEGIN
    SET NOCOUNT ON

    BEGIN TRY
        UPDATE [Medico].[tbTiposConsulta]
        SET tipoCon_EsEliminado = 1
        WHERE tipoCon_Id = @tipoCon_Id

        RETURN 1 -- Éxito
    END TRY
    BEGIN CATCH
        RETURN 0 -- Error
    END CATCH
END
GO

-- =============================================
-- SP: PR_Medico_TiposConsulta_Dropdown
-- Descripción: Lista para dropdown/select
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[PR_Medico_TiposConsulta_Dropdown]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [Medico].[PR_Medico_TiposConsulta_Dropdown]
GO

CREATE PROCEDURE [Medico].[PR_Medico_TiposConsulta_Dropdown]
AS
BEGIN
    SET NOCOUNT ON

    SELECT  tipoCon_Id,
            tipoCon_Descripcion
    FROM [Medico].[tbTiposConsulta]
    WHERE tipoCon_EsEliminado = 0
    ORDER BY tipoCon_Descripcion ASC
END
GO

PRINT 'Stored Procedures para tbTiposConsulta creados exitosamente.'
GO
