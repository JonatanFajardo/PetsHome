/****************************************************************************************
    MÓDULO: Rescate / Ingreso
    ESQUEMA: [Rescate]
    BASE DE DATOS: [PETSHOMEDB]
    DESCRIPCIÓN: Tablas para gestionar reportes de abandono e ingresos de mascotas
****************************************************************************************/
USE [PETSHOMEDB];
GO

----------------------------------------------------------------------------------------
-- 0. Crear esquema Rescate (si no existe)
----------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Rescate')
BEGIN
    EXEC('CREATE SCHEMA [Rescate]')
END
GO

----------------------------------------------------------------------------------------
-- 1. Tabla: Rescate.tbReportantesTipo
-- Catálogo de tipos de reportantes (ciudadano, policía, bomberos, etc.)
----------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[tbReportantesTipo]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Rescate].[tbReportantesTipo] (
        [reptip_Id]              INT IDENTITY(1,1) PRIMARY KEY,
        [reptip_Descripcion]     VARCHAR(100) NOT NULL,
        [reptip_EsActivo]        BIT NOT NULL DEFAULT (1),
        [reptip_EsEliminado]     BIT NOT NULL DEFAULT (0),
        [reptip_UsuarioCrea]     INT NOT NULL,
        [reptip_FechaCrea]       DATETIME NOT NULL DEFAULT (GETDATE()),
        [reptip_UsuarioModifica] INT NULL,
        [reptip_FechaModifica]   DATETIME NULL,
        CONSTRAINT FK_ReportantesTipo_UsuarioCrea
            FOREIGN KEY ([reptip_UsuarioCrea]) REFERENCES [Seguridad].[tbUsuarios]([usu_Id]),
        CONSTRAINT FK_ReportantesTipo_UsuarioModifica
            FOREIGN KEY ([reptip_UsuarioModifica]) REFERENCES [Seguridad].[tbUsuarios]([usu_Id])
    );
    PRINT '✓ Tabla [Rescate].[tbReportantesTipo] creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠ Tabla [Rescate].[tbReportantesTipo] ya existe';
END
GO

----------------------------------------------------------------------------------------
-- 2. Tabla: Rescate.tbReportesAbandono
-- Registros de reportes de animales abandonados
----------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[tbReportesAbandono]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Rescate].[tbReportesAbandono] (
        [repa_Id]               INT IDENTITY(1,1) PRIMARY KEY,
        [reptip_Id]             INT NOT NULL,
        [repa_NombreReportante] VARCHAR(150) NULL,
        [repa_TelefonoContacto]         VARCHAR(20) NULL,
        [repa_Email]            VARCHAR(100) NULL,
        [repa_FechaReporte]     DATETIME NOT NULL DEFAULT (GETDATE()),
        [repa_UbicacionIncidente]            VARCHAR(200) NULL,
        [repa_DescripcionAnimal] VARCHAR(300) NULL,
        [repa_EstadoAtencion]   VARCHAR(50) NOT NULL DEFAULT ('Pendiente'),
        [repa_Observaciones]    VARCHAR(300) NULL,
        [repa_EsAnonimo]        BIT NOT NULL DEFAULT (0),
        [refg_Id]               INT NOT NULL,
        [repa_EsEliminado]      BIT NOT NULL DEFAULT (0),
        [repa_UsuarioCrea]      INT NOT NULL,
        [repa_FechaCrea]        DATETIME NOT NULL DEFAULT (GETDATE()),
        [repa_UsuarioModifica]  INT NULL,
        [repa_FechaModifica]    DATETIME NULL,
        CONSTRAINT FK_ReportesAbandono_ReportantesTipo
            FOREIGN KEY ([reptip_Id]) REFERENCES [Rescate].[tbReportantesTipo]([reptip_Id]),
        CONSTRAINT FK_ReportesAbandono_Refugios
            FOREIGN KEY ([refg_Id]) REFERENCES [Refugio].[tbRefugios]([refg_Id]),
        CONSTRAINT FK_ReportesAbandono_UsuarioCrea
            FOREIGN KEY ([repa_UsuarioCrea]) REFERENCES [Seguridad].[tbUsuarios]([usu_Id]),
        CONSTRAINT FK_ReportesAbandono_UsuarioModifica
            FOREIGN KEY ([repa_UsuarioModifica]) REFERENCES [Seguridad].[tbUsuarios]([usu_Id])
    );
    PRINT '✓ Tabla [Rescate].[tbReportesAbandono] creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠ Tabla [Rescate].[tbReportesAbandono] ya existe';
END
GO

----------------------------------------------------------------------------------------
-- 3. Tabla: Rescate.tbIngresos
-- Registros de ingresos de animales al refugio
----------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Rescate].[tbIngresos]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Rescate].[tbIngresos] (
        [ingr_Id]              INT IDENTITY(1,1) PRIMARY KEY,
        [repa_Id]              INT NULL,
        [refg_Id]              INT NOT NULL,
        [ingr_FechaIngreso]    DATETIME NOT NULL DEFAULT (GETDATE()),
        [ingr_LugarRescate]    VARCHAR(200) NULL,
        [ingr_CondicionInicial] VARCHAR(200) NULL,
        [ingr_PersonaRescatista] VARCHAR(150) NULL,
        [ingr_MedioTransporte] VARCHAR(100) NULL,
        [ingr_Observaciones]   VARCHAR(300) NULL,
        [ingr_EsEmergencia]    BIT NOT NULL DEFAULT (0),
        [ingr_EsEliminado]     BIT NOT NULL DEFAULT (0),
        [ingr_UsuarioCrea]     INT NOT NULL,
        [ingr_FechaCrea]       DATETIME NOT NULL DEFAULT (GETDATE()),
        [ingr_UsuarioModifica] INT NULL,
        [ingr_FechaModifica]   DATETIME NULL,
        CONSTRAINT FK_Ingresos_ReportesAbandono
            FOREIGN KEY ([repa_Id]) REFERENCES [Rescate].[tbReportesAbandono]([repa_Id]),
        CONSTRAINT FK_Ingresos_Refugios
            FOREIGN KEY ([refg_Id]) REFERENCES [Refugio].[tbRefugios]([refg_Id]),
        CONSTRAINT FK_Ingresos_UsuarioCrea
            FOREIGN KEY ([ingr_UsuarioCrea]) REFERENCES [Seguridad].[tbUsuarios]([usu_Id]),
        CONSTRAINT FK_Ingresos_UsuarioModifica
            FOREIGN KEY ([ingr_UsuarioModifica]) REFERENCES [Seguridad].[tbUsuarios]([usu_Id])
    );
    PRINT '✓ Tabla [Rescate].[tbIngresos] creada exitosamente';
END
ELSE
BEGIN
    PRINT '⚠ Tabla [Rescate].[tbIngresos] ya existe';
END
GO

----------------------------------------------------------------------------------------
-- 4. Agregar campo masc_IngresoId a tbMascotas (vínculo opcional)
----------------------------------------------------------------------------------------
IF NOT EXISTS (
    SELECT * FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[Refugio].[tbMascotas]')
    AND name = 'masc_IngresoId'
)
BEGIN
    ALTER TABLE [Refugio].[tbMascotas]
    ADD [masc_IngresoId] INT NULL;

    ALTER TABLE [Refugio].[tbMascotas]
    ADD CONSTRAINT FK_Mascotas_Ingresos
        FOREIGN KEY ([masc_IngresoId])
        REFERENCES [Rescate].[tbIngresos]([ingr_Id]);

    PRINT '✓ Campo masc_IngresoId agregado a [Refugio].[tbMascotas]';
END
ELSE
BEGIN
    PRINT '⚠ Campo masc_IngresoId ya existe en [Refugio].[tbMascotas]';
END
GO

----------------------------------------------------------------------------------------
-- 5. Datos iniciales: Tipos de reportantes
----------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT * FROM [Rescate].[tbReportantesTipo])
BEGIN
    SET IDENTITY_INSERT [Rescate].[tbReportantesTipo] ON;

    INSERT INTO [Rescate].[tbReportantesTipo] (
        [reptip_Id],
        [reptip_Descripcion],
        [reptip_UsuarioCrea]
    )
    VALUES
     (1, 'Ciudadano Particular', 1),
     (2, 'Policía', 1),
     (3, 'Bomberos', 1),
     (4, 'Otro Refugio', 1),
     (5, 'Hospital Veterinario', 1),
     (6, 'Personal Municipal', 1),
     (7, 'Anónimo', 1);

    SET IDENTITY_INSERT [Rescate].[tbReportantesTipo] OFF;

    PRINT '✓ Datos iniciales insertados en [Rescate].[tbReportantesTipo]';
END
ELSE
BEGIN
    PRINT '⚠ Ya existen datos en [Rescate].[tbReportantesTipo]';
END
GO

PRINT '';
PRINT '══════════════════════════════════════════════════════════════════════';
PRINT '  ✓ SCRIPT COMPLETADO: Módulo Rescate/Ingreso creado exitosamente';
PRINT '══════════════════════════════════════════════════════════════════════';
GO
