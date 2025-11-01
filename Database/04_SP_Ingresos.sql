/****************************************************************************************
    STORED PROCEDURES: Rescate.tbIngresos
    BASE DE DATOS: [PETSHOMEDB]
****************************************************************************************/
USE [PETSHOMEDB];
GO

----------------------------------------------------------------------------------------
-- 1. PR_Rescate_Ingresos_List
-- Lista todos los ingresos
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_Ingresos_List]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_Ingresos_List]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_Ingresos_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY ingr.ingr_FechaIngreso DESC) AS Fila,
            ingr.ingr_Id,
            ingr.repa_Id,
            ingr.refg_Id,
            refg.refg_Nombre AS NombreRefugio,
            ingr.ingr_FechaIngreso,
            ingr.ingr_LugarRescate,
            ingr.ingr_CondicionInicial,
            ingr.ingr_PersonaRescatista,
            ingr.ingr_MedioTransporte,
            ingr.ingr_Observaciones,
            ingr.ingr_EsEmergencia,
            -- Datos del reporte (si existe)
            repa.repa_UbicacionIncidente AS LugarReporte,
            repa.repa_DescripcionAnimal,
            -- Verificar si ya tiene mascota asociada
            (SELECT COUNT(*) FROM [Refugio].[tbMascotas] WHERE masc_IngresoId = ingr.ingr_Id AND masc_EsEliminado != 1) AS TieneMascota
    FROM [Rescate].[tbIngresos] AS ingr
    INNER JOIN [Refugio].[tbRefugios] AS refg
        ON ingr.refg_Id = refg.refg_Id
    LEFT JOIN [Rescate].[tbReportesAbandono] AS repa
        ON ingr.repa_Id = repa.repa_Id
    WHERE ingr.ingr_EsEliminado != 1
    ORDER BY ingr.ingr_FechaIngreso DESC
END
GO
PRINT '✓ PR_Rescate_Ingresos_List creado';
GO

----------------------------------------------------------------------------------------
-- 2. PR_Rescate_Ingresos_Detail
-- Detalle con información de auditoría
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_Ingresos_Detail]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_Ingresos_Detail]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_Ingresos_Detail]
AS
BEGIN
    SELECT  ingr.ingr_Id,
            ingr.repa_Id,
            ingr.refg_Id,
            refg.refg_Nombre AS NombreRefugio,
            ingr.ingr_FechaIngreso,
            ingr.ingr_LugarRescate,
            ingr.ingr_CondicionInicial,
            ingr.ingr_PersonaRescatista,
            ingr.ingr_MedioTransporte,
            ingr.ingr_Observaciones,
            ingr.ingr_EsEmergencia,
            -- Datos del reporte (si existe)
            repa.repa_UbicacionIncidente AS LugarReporte,
            repa.repa_DescripcionAnimal,
            repa.repa_EstadoAtencion,
            repa.repa_NombreReportante,
            repa.repa_TelefonoContacto AS TelefonoReportante,
            -- Auditoría
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            ingr.ingr_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            ingr.ingr_FechaModifica,
            -- Mascota asociada (si existe)
            (SELECT COUNT(*) FROM [Refugio].[tbMascotas] WHERE masc_IngresoId = ingr.ingr_Id AND masc_EsEliminado != 1) AS TieneMascota,
            (SELECT TOP 1 masc_Id FROM [Refugio].[tbMascotas] WHERE masc_IngresoId = ingr.ingr_Id AND masc_EsEliminado != 1) AS MascotaId,
            (SELECT TOP 1 masc_Nombre FROM [Refugio].[tbMascotas] WHERE masc_IngresoId = ingr.ingr_Id AND masc_EsEliminado != 1) AS MascotaNombre
    FROM [Rescate].[tbIngresos] AS ingr
    INNER JOIN [Refugio].[tbRefugios] AS refg
        ON ingr.refg_Id = refg.refg_Id
    LEFT JOIN [Rescate].[tbReportesAbandono] AS repa
        ON ingr.repa_Id = repa.repa_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON ingr.ingr_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON ingr.ingr_UsuarioModifica = usuarioModifica.usu_Id
    WHERE ingr.ingr_EsEliminado != 1
    ORDER BY ingr.ingr_FechaIngreso DESC
END
GO
PRINT '✓ PR_Rescate_Ingresos_Detail creado';
GO

----------------------------------------------------------------------------------------
-- 3. PR_Rescate_Ingresos_Find
-- Buscar por ID
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_Ingresos_Find]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_Ingresos_Find]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_Ingresos_Find]
    @ingr_Id INT
AS
BEGIN
    SELECT  ingr.ingr_Id,
            ingr.repa_Id,
            ingr.refg_Id,
            refg.refg_Nombre AS NombreRefugio,
            ingr.ingr_FechaIngreso,
            ingr.ingr_LugarRescate,
            ingr.ingr_CondicionInicial,
            ingr.ingr_PersonaRescatista,
            ingr.ingr_MedioTransporte,
            ingr.ingr_Observaciones,
            ingr.ingr_EsEmergencia,
            -- Datos del reporte (si existe)
            repa.repa_UbicacionIncidente AS LugarReporte,
            repa.repa_DescripcionAnimal,
            repa.repa_EstadoAtencion,
            repa.repa_NombreReportante,
            repa.repa_TelefonoContacto AS TelefonoReportante,
            -- Auditoría
            ingr.ingr_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            ingr.ingr_FechaCrea,
            ingr.ingr_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            ingr.ingr_FechaModifica,
            -- Mascota asociada (si existe)
            (SELECT COUNT(*) FROM [Refugio].[tbMascotas] WHERE masc_IngresoId = ingr.ingr_Id AND masc_EsEliminado != 1) AS TieneMascota,
            (SELECT TOP 1 masc_Id FROM [Refugio].[tbMascotas] WHERE masc_IngresoId = ingr.ingr_Id AND masc_EsEliminado != 1) AS MascotaId,
            (SELECT TOP 1 masc_Nombre FROM [Refugio].[tbMascotas] WHERE masc_IngresoId = ingr.ingr_Id AND masc_EsEliminado != 1) AS MascotaNombre
    FROM [Rescate].[tbIngresos] AS ingr
    INNER JOIN [Refugio].[tbRefugios] AS refg
        ON ingr.refg_Id = refg.refg_Id
    LEFT JOIN [Rescate].[tbReportesAbandono] AS repa
        ON ingr.repa_Id = repa.repa_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON ingr.ingr_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON ingr.ingr_UsuarioModifica = usuarioModifica.usu_Id
    WHERE ingr.ingr_EsEliminado != 1
    AND ingr.ingr_Id = @ingr_Id
END
GO
PRINT '✓ PR_Rescate_Ingresos_Find creado';
GO

----------------------------------------------------------------------------------------
-- 4. PR_Rescate_Ingresos_Insert
-- Insertar nuevo ingreso
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_Ingresos_Insert]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_Ingresos_Insert]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_Ingresos_Insert]
    @repa_Id INT,
    @refg_Id INT,
    @ingr_FechaIngreso DATETIME,
    @ingr_LugarRescate VARCHAR(200),
    @ingr_CondicionInicial VARCHAR(200),
    @ingr_PersonaRescatista VARCHAR(150),
    @ingr_MedioTransporte VARCHAR(100),
    @ingr_Observaciones VARCHAR(300),
    @ingr_EsEmergencia BIT,
    @ingr_UsuarioCrea INT
AS
BEGIN
    INSERT INTO [Rescate].[tbIngresos]
    (
        repa_Id,
        refg_Id,
        ingr_FechaIngreso,
        ingr_LugarRescate,
        ingr_CondicionInicial,
        ingr_PersonaRescatista,
        ingr_MedioTransporte,
        ingr_Observaciones,
        ingr_EsEmergencia,
        ingr_UsuarioCrea,
        ingr_FechaCrea
    )
    VALUES
    (
        @repa_Id,
        @refg_Id,
        @ingr_FechaIngreso,
        @ingr_LugarRescate,
        @ingr_CondicionInicial,
        @ingr_PersonaRescatista,
        @ingr_MedioTransporte,
        @ingr_Observaciones,
        @ingr_EsEmergencia,
        @ingr_UsuarioCrea,
        GETDATE()
    )

    -- Si viene de un reporte, actualizar estado a "En Proceso"
    IF @repa_Id IS NOT NULL
    BEGIN
        UPDATE [Rescate].[tbReportesAbandono]
        SET repa_EstadoAtencion = 'En Proceso'
        WHERE repa_Id = @repa_Id
    END
END
GO
PRINT '✓ PR_Rescate_Ingresos_Insert creado';
GO

----------------------------------------------------------------------------------------
-- 5. PR_Rescate_Ingresos_Update
-- Actualizar ingreso
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_Ingresos_Update]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_Ingresos_Update]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_Ingresos_Update]
    @ingr_Id INT,
    @repa_Id INT,
    @refg_Id INT,
    @ingr_FechaIngreso DATETIME,
    @ingr_LugarRescate VARCHAR(200),
    @ingr_CondicionInicial VARCHAR(200),
    @ingr_PersonaRescatista VARCHAR(150),
    @ingr_MedioTransporte VARCHAR(100),
    @ingr_Observaciones VARCHAR(300),
    @ingr_EsEmergencia BIT,
    @ingr_UsuarioModifica INT
AS
BEGIN
    UPDATE [Rescate].[tbIngresos]
    SET repa_Id = @repa_Id,
        refg_Id = @refg_Id,
        ingr_FechaIngreso = @ingr_FechaIngreso,
        ingr_LugarRescate = @ingr_LugarRescate,
        ingr_CondicionInicial = @ingr_CondicionInicial,
        ingr_PersonaRescatista = @ingr_PersonaRescatista,
        ingr_MedioTransporte = @ingr_MedioTransporte,
        ingr_Observaciones = @ingr_Observaciones,
        ingr_EsEmergencia = @ingr_EsEmergencia,
        ingr_UsuarioModifica = @ingr_UsuarioModifica,
        ingr_FechaModifica = GETDATE()
    WHERE ingr_Id = @ingr_Id
END
GO
PRINT '✓ PR_Rescate_Ingresos_Update creado';
GO

----------------------------------------------------------------------------------------
-- 6. PR_Rescate_Ingresos_Delete
-- Eliminación lógica
----------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[PR_Rescate_Ingresos_Delete]') AND type in (N'P'))
    DROP PROCEDURE [Rescate].[PR_Rescate_Ingresos_Delete]
GO

CREATE PROCEDURE [Rescate].[PR_Rescate_Ingresos_Delete]
    @ingr_Id INT
AS
BEGIN
    UPDATE [Rescate].[tbIngresos]
    SET ingr_EsEliminado = 1
    WHERE ingr_Id = @ingr_Id
END
GO
PRINT '✓ PR_Rescate_Ingresos_Delete creado';
GO

PRINT '';
PRINT '══════════════════════════════════════════════════════════════════════';
PRINT '  ✓ SPs de Ingresos creados exitosamente';
PRINT '══════════════════════════════════════════════════════════════════════';
GO
