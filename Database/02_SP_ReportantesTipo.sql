/****************************************************************************************
    STORED PROCEDURES: Rescate.tbReportantesTipo
    BASE DE DATOS: [PETSHOMEDB]
****************************************************************************************/
USE [PETSHOMEDB];
GO

----------------------------------------------------------------------------------------
-- 1. PR_Rescate_ReportantesTipo_List
-- Lista todos los tipos de reportantes activos
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportantesTipo_List]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_List]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY reptip_Id ASC) AS Fila,
            reptip_Id,
            reptip_Descripcion,
            reptip_EsActivo
    FROM [Rescate].[tbReportantesTipo]
    WHERE reptip_EsEliminado != 1
    ORDER BY reptip_Descripcion
END
GO
PRINT '✓ PR_Rescate_ReportantesTipo_List creado';
GO

----------------------------------------------------------------------------------------
-- 2. PR_Rescate_ReportantesTipo_Detail
-- Detalle con información de auditoría
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportantesTipo_Detail]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Detail]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Detail]
AS
BEGIN
    SELECT  reptip_Id,
            reptip_Descripcion,
            reptip_EsActivo,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            reptip_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            reptip_FechaModifica
    FROM [Rescate].[tbReportantesTipo] AS reptip
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON reptip.reptip_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON reptip.reptip_UsuarioModifica = usuarioModifica.usu_Id
    WHERE reptip_EsEliminado != 1
    ORDER BY reptip_Descripcion
END
GO
PRINT '✓ PR_Rescate_ReportantesTipo_Detail creado';
GO

----------------------------------------------------------------------------------------
-- 3. PR_Rescate_ReportantesTipo_Find
-- Buscar por ID
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportantesTipo_Find]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Find]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Find]
    @reptip_Id INT
AS
BEGIN
    SELECT  reptip_Id,
            reptip_Descripcion,
            reptip_EsActivo,
            reptip_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            reptip_FechaCrea,
            reptip_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            reptip_FechaModifica
    FROM [Rescate].[tbReportantesTipo] AS reptip
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON reptip.reptip_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON reptip.reptip_UsuarioModifica = usuarioModifica.usu_Id
    WHERE reptip_EsEliminado != 1
    AND reptip_Id = @reptip_Id
END
GO
PRINT '✓ PR_Rescate_ReportantesTipo_Find creado';
GO

----------------------------------------------------------------------------------------
-- 4. PR_Rescate_ReportantesTipo_Insert
-- Insertar nuevo tipo de reportante
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportantesTipo_Insert]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Insert]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Insert]
    @reptip_Descripcion VARCHAR(100),
    @reptip_EsActivo BIT,
    @reptip_UsuarioCrea INT
AS
BEGIN
    INSERT INTO [Rescate].[tbReportantesTipo]
    (
        reptip_Descripcion,
        reptip_EsActivo,
        reptip_UsuarioCrea,
        reptip_FechaCrea
    )
    VALUES
    (
        @reptip_Descripcion,
        @reptip_EsActivo,
        @reptip_UsuarioCrea,
        GETDATE()
    )
END
GO
PRINT '✓ PR_Rescate_ReportantesTipo_Insert creado';
GO

----------------------------------------------------------------------------------------
-- 5. PR_Rescate_ReportantesTipo_Update
-- Actualizar tipo de reportante
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportantesTipo_Update]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Update]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Update]
    @reptip_Id INT,
    @reptip_Descripcion VARCHAR(100),
    @reptip_EsActivo BIT,
    @reptip_UsuarioModifica INT
AS
BEGIN
    UPDATE [Rescate].[tbReportantesTipo]
    SET reptip_Descripcion = @reptip_Descripcion,
        reptip_EsActivo = @reptip_EsActivo,
        reptip_UsuarioModifica = @reptip_UsuarioModifica,
        reptip_FechaModifica = GETDATE()
    WHERE reptip_Id = @reptip_Id
END
GO
PRINT '✓ PR_Rescate_ReportantesTipo_Update creado';
GO

----------------------------------------------------------------------------------------
-- 6. PR_Rescate_ReportantesTipo_Delete
-- Eliminación lógica
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportantesTipo_Delete]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Delete]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportantesTipo_Delete]
    @reptip_Id INT
AS
BEGIN
    UPDATE [Rescate].[tbReportantesTipo]
    SET reptip_EsEliminado = 1
    WHERE reptip_Id = @reptip_Id
END
GO
PRINT '✓ PR_Rescate_ReportantesTipo_Delete creado';
GO

PRINT '';
PRINT '══════════════════════════════════════════════════════════════════════';
PRINT '  ✓ SPs de ReportantesTipo creados exitosamente';
PRINT '══════════════════════════════════════════════════════════════════════';
GO
