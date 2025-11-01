/****************************************************************************************
    STORED PROCEDURES: Rescate.tbReportesAbandono
    BASE DE DATOS: [PETSHOMEDB]
****************************************************************************************/
USE [PETSHOMEDB];
GO

----------------------------------------------------------------------------------------
-- 1. PR_Rescate_ReportesAbandono_List
-- Lista todos los reportes de abandono
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportesAbandono_List]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_List]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY repa.repa_FechaReporte DESC) AS Fila,
            repa.repa_Id,
            repa.reptip_Id,
            reptip.reptip_Descripcion AS TipoReportante,
            repa.repa_NombreReportante,
            repa.repa_TelefonoContacto,
            repa.repa_Email,
            repa.repa_FechaReporte,
            repa.repa_UbicacionIncidente,
            repa.repa_DescripcionAnimal,
            repa.repa_EstadoAtencion,
            repa.repa_Observaciones,
            repa.repa_EsAnonimo,
            repa.refg_Id,
            refg.refg_Nombre AS NombreRefugio
    FROM [Rescate].[tbReportesAbandono] AS repa
    INNER JOIN [Rescate].[tbReportantesTipo] AS reptip
        ON repa.reptip_Id = reptip.reptip_Id
    INNER JOIN [Refugio].[tbRefugios] AS refg
        ON repa.refg_Id = refg.refg_Id
    WHERE repa.repa_EsEliminado != 1
    ORDER BY repa.repa_FechaReporte DESC
END
GO
PRINT '✓ PR_Rescate_ReportesAbandono_List creado';
GO

----------------------------------------------------------------------------------------
-- 2. PR_Rescate_ReportesAbandono_Detail
-- Detalle con información de auditoría
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportesAbandono_Detail]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Detail]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Detail]
AS
BEGIN
    SELECT  repa.repa_Id,
            repa.reptip_Id,
            reptip.reptip_Descripcion AS TipoReportante,
            repa.repa_NombreReportante,
            repa.repa_TelefonoContacto,
            repa.repa_Email,
            repa.repa_FechaReporte,
            repa.repa_UbicacionIncidente,
            repa.repa_DescripcionAnimal,
            repa.repa_EstadoAtencion,
            repa.repa_Observaciones,
            repa.repa_EsAnonimo,
            repa.refg_Id,
            refg.refg_Nombre AS NombreRefugio,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            repa.repa_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            repa.repa_FechaModifica
    FROM [Rescate].[tbReportesAbandono] AS repa
    INNER JOIN [Rescate].[tbReportantesTipo] AS reptip
        ON repa.reptip_Id = reptip.reptip_Id
    INNER JOIN [Refugio].[tbRefugios] AS refg
        ON repa.refg_Id = refg.refg_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON repa.repa_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON repa.repa_UsuarioModifica = usuarioModifica.usu_Id
    WHERE repa.repa_EsEliminado != 1
    ORDER BY repa.repa_FechaReporte DESC
END
GO
PRINT '✓ PR_Rescate_ReportesAbandono_Detail creado';
GO

----------------------------------------------------------------------------------------
-- 3. PR_Rescate_ReportesAbandono_Find
-- Buscar por ID
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportesAbandono_Find]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Find]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Find]
    @repa_Id INT
AS
BEGIN
    SELECT  repa.repa_Id,
            repa.reptip_Id,
            reptip.reptip_Descripcion AS TipoReportante,
            repa.repa_NombreReportante,
            repa.repa_TelefonoContacto,
            repa.repa_Email,
            repa.repa_FechaReporte,
            repa.repa_UbicacionIncidente,
            repa.repa_DescripcionAnimal,
            repa.repa_EstadoAtencion,
            repa.repa_Observaciones,
            repa.repa_EsAnonimo,
            repa.refg_Id,
            refg.refg_Nombre AS NombreRefugio,
            repa.repa_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            repa.repa_FechaCrea,
            repa.repa_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            repa.repa_FechaModifica
    FROM [Rescate].[tbReportesAbandono] AS repa
    INNER JOIN [Rescate].[tbReportantesTipo] AS reptip
        ON repa.reptip_Id = reptip.reptip_Id
    INNER JOIN [Refugio].[tbRefugios] AS refg
        ON repa.refg_Id = refg.refg_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON repa.repa_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON repa.repa_UsuarioModifica = usuarioModifica.usu_Id
    WHERE repa.repa_EsEliminado != 1
    AND repa.repa_Id = @repa_Id
END
GO
PRINT '✓ PR_Rescate_ReportesAbandono_Find creado';
GO

----------------------------------------------------------------------------------------
-- 4. PR_Rescate_ReportesAbandono_Insert
-- Insertar nuevo reporte de abandono
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportesAbandono_Insert]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Insert]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Insert]
    @reptip_Id INT,
    @repa_NombreReportante VARCHAR(150),
    @repa_TelefonoContacto VARCHAR(20),
    @repa_Email VARCHAR(100),
    @repa_FechaReporte DATETIME,
    @repa_UbicacionIncidente VARCHAR(200),
    @repa_DescripcionAnimal VARCHAR(300),
    @repa_EstadoAtencion VARCHAR(50),
    @repa_Observaciones VARCHAR(300),
    @repa_EsAnonimo BIT,
    @refg_Id INT,
    @repa_UsuarioCrea INT
AS
BEGIN
    INSERT INTO [Rescate].[tbReportesAbandono]
    (
        reptip_Id,
        repa_NombreReportante,
        repa_TelefonoContacto,
        repa_Email,
        repa_FechaReporte,
        repa_UbicacionIncidente,
        repa_DescripcionAnimal,
        repa_EstadoAtencion,
        repa_Observaciones,
        repa_EsAnonimo,
        refg_Id,
        repa_UsuarioCrea,
        repa_FechaCrea
    )
    VALUES
    (
        @reptip_Id,
        @repa_NombreReportante,
        @repa_TelefonoContacto,
        @repa_Email,
        @repa_FechaReporte,
        @repa_UbicacionIncidente,
        @repa_DescripcionAnimal,
        @repa_EstadoAtencion,
        @repa_Observaciones,
        @repa_EsAnonimo,
        @refg_Id,
        @repa_UsuarioCrea,
        GETDATE()
    )
END
GO
PRINT '✓ PR_Rescate_ReportesAbandono_Insert creado';
GO

----------------------------------------------------------------------------------------
-- 5. PR_Rescate_ReportesAbandono_Update
-- Actualizar reporte de abandono
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportesAbandono_Update]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Update]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Update]
    @repa_Id INT,
    @reptip_Id INT,
    @repa_NombreReportante VARCHAR(150),
    @repa_TelefonoContacto VARCHAR(20),
    @repa_Email VARCHAR(100),
    @repa_FechaReporte DATETIME,
    @repa_UbicacionIncidente VARCHAR(200),
    @repa_DescripcionAnimal VARCHAR(300),
    @repa_EstadoAtencion VARCHAR(50),
    @repa_Observaciones VARCHAR(300),
    @repa_EsAnonimo BIT,
    @refg_Id INT,
    @repa_UsuarioModifica INT
AS
BEGIN
    UPDATE [Rescate].[tbReportesAbandono]
    SET reptip_Id = @reptip_Id,
        repa_NombreReportante = @repa_NombreReportante,
        repa_TelefonoContacto = @repa_TelefonoContacto,
        repa_Email = @repa_Email,
        repa_FechaReporte = @repa_FechaReporte,
        repa_UbicacionIncidente = @repa_UbicacionIncidente,
        repa_DescripcionAnimal = @repa_DescripcionAnimal,
        repa_EstadoAtencion = @repa_EstadoAtencion,
        repa_Observaciones = @repa_Observaciones,
        repa_EsAnonimo = @repa_EsAnonimo,
        refg_Id = @refg_Id,
        repa_UsuarioModifica = @repa_UsuarioModifica,
        repa_FechaModifica = GETDATE()
    WHERE repa_Id = @repa_Id
END
GO
PRINT '✓ PR_Rescate_ReportesAbandono_Update creado';
GO

----------------------------------------------------------------------------------------
-- 6. PR_Rescate_ReportesAbandono_Delete
-- Eliminación lógica
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_ReportesAbandono_Delete]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Delete]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_ReportesAbandono_Delete]
    @repa_Id INT
AS
BEGIN
    UPDATE [Rescate].[tbReportesAbandono]
    SET repa_EsEliminado = 1
    WHERE repa_Id = @repa_Id
END
GO
PRINT '✓ PR_Rescate_ReportesAbandono_Delete creado';
GO

PRINT '';
PRINT '══════════════════════════════════════════════════════════════════════';
PRINT '  ✓ SPs de ReportesAbandono creados exitosamente';
PRINT '══════════════════════════════════════════════════════════════════════';
GO
