-- =============================================
-- Script: Creación de Tablas Catálogo - Módulo Médico
-- Autor: Claude Code
-- Fecha: 2025-10-31
-- Descripción: Crea las 6 tablas catálogo para el módulo veterinario
-- =============================================

USE PETSHOMEDB
GO

-- Verificar si el esquema [Medico] existe, si no, crearlo
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Medico')
BEGIN
    EXEC('CREATE SCHEMA [Medico]')
    PRINT 'Esquema [Medico] creado exitosamente.'
END
ELSE
BEGIN
    PRINT 'El esquema [Medico] ya existe.'
END
GO

-- =============================================
-- Tabla: tbTiposConsulta
-- Descripción: Catálogo de tipos de consulta médica
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[tbTiposConsulta]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Medico].[tbTiposConsulta]
    (
        tipoCon_Id INT IDENTITY(1,1) NOT NULL,
        tipoCon_Descripcion NVARCHAR(100) NOT NULL,
        tipoCon_EsEliminado BIT NOT NULL DEFAULT 0,
        tipoCon_UsuarioCrea INT NOT NULL,
        tipoCon_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        tipoCon_UsuarioModifica INT NULL,
        tipoCon_FechaModifica DATETIME NULL,
        CONSTRAINT PK_Medico_tbTiposConsulta PRIMARY KEY CLUSTERED (tipoCon_Id ASC),
        CONSTRAINT FK_Medico_tbTiposConsulta_UsuarioCrea FOREIGN KEY (tipoCon_UsuarioCrea)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT FK_Medico_tbTiposConsulta_UsuarioModifica FOREIGN KEY (tipoCon_UsuarioModifica)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    )
    PRINT 'Tabla [Medico].[tbTiposConsulta] creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla [Medico].[tbTiposConsulta] ya existe.'
END
GO

-- =============================================
-- Tabla: tbGravedades
-- Descripción: Catálogo de niveles de gravedad
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[tbGravedades]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Medico].[tbGravedades]
    (
        grav_Id INT IDENTITY(1,1) NOT NULL,
        grav_Descripcion NVARCHAR(50) NOT NULL,
        grav_EsEliminado BIT NOT NULL DEFAULT 0,
        grav_UsuarioCrea INT NOT NULL,
        grav_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        grav_UsuarioModifica INT NULL,
        grav_FechaModifica DATETIME NULL,
        CONSTRAINT PK_Medico_tbGravedades PRIMARY KEY CLUSTERED (grav_Id ASC),
        CONSTRAINT FK_Medico_tbGravedades_UsuarioCrea FOREIGN KEY (grav_UsuarioCrea)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT FK_Medico_tbGravedades_UsuarioModifica FOREIGN KEY (grav_UsuarioModifica)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    )
    PRINT 'Tabla [Medico].[tbGravedades] creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla [Medico].[tbGravedades] ya existe.'
END
GO

-- =============================================
-- Tabla: tbTiposMedicamento
-- Descripción: Catálogo de tipos de medicamentos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[tbTiposMedicamento]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Medico].[tbTiposMedicamento]
    (
        tipoMed_Id INT IDENTITY(1,1) NOT NULL,
        tipoMed_Descripcion NVARCHAR(100) NOT NULL,
        tipoMed_EsEliminado BIT NOT NULL DEFAULT 0,
        tipoMed_UsuarioCrea INT NOT NULL,
        tipoMed_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        tipoMed_UsuarioModifica INT NULL,
        tipoMed_FechaModifica DATETIME NULL,
        CONSTRAINT PK_Medico_tbTiposMedicamento PRIMARY KEY CLUSTERED (tipoMed_Id ASC),
        CONSTRAINT FK_Medico_tbTiposMedicamento_UsuarioCrea FOREIGN KEY (tipoMed_UsuarioCrea)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT FK_Medico_tbTiposMedicamento_UsuarioModifica FOREIGN KEY (tipoMed_UsuarioModifica)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    )
    PRINT 'Tabla [Medico].[tbTiposMedicamento] creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla [Medico].[tbTiposMedicamento] ya existe.'
END
GO

-- =============================================
-- Tabla: tbViasAdministracion
-- Descripción: Catálogo de vías de administración de medicamentos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[tbViasAdministracion]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Medico].[tbViasAdministracion]
    (
        viaAdmin_Id INT IDENTITY(1,1) NOT NULL,
        viaAdmin_Descripcion NVARCHAR(100) NOT NULL,
        viaAdmin_EsEliminado BIT NOT NULL DEFAULT 0,
        viaAdmin_UsuarioCrea INT NOT NULL,
        viaAdmin_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        viaAdmin_UsuarioModifica INT NULL,
        viaAdmin_FechaModifica DATETIME NULL,
        CONSTRAINT PK_Medico_tbViasAdministracion PRIMARY KEY CLUSTERED (viaAdmin_Id ASC),
        CONSTRAINT FK_Medico_tbViasAdministracion_UsuarioCrea FOREIGN KEY (viaAdmin_UsuarioCrea)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT FK_Medico_tbViasAdministracion_UsuarioModifica FOREIGN KEY (viaAdmin_UsuarioModifica)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    )
    PRINT 'Tabla [Medico].[tbViasAdministracion] creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla [Medico].[tbViasAdministracion] ya existe.'
END
GO

-- =============================================
-- Tabla: tbTiposParasito
-- Descripción: Catálogo de tipos de parásitos
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[tbTiposParasito]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Medico].[tbTiposParasito]
    (
        tipoPar_Id INT IDENTITY(1,1) NOT NULL,
        tipoPar_Descripcion NVARCHAR(100) NOT NULL,
        tipoPar_Categoria NVARCHAR(50) NULL, -- 'Externo' o 'Interno'
        tipoPar_EsEliminado BIT NOT NULL DEFAULT 0,
        tipoPar_UsuarioCrea INT NOT NULL,
        tipoPar_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        tipoPar_UsuarioModifica INT NULL,
        tipoPar_FechaModifica DATETIME NULL,
        CONSTRAINT PK_Medico_tbTiposParasito PRIMARY KEY CLUSTERED (tipoPar_Id ASC),
        CONSTRAINT FK_Medico_tbTiposParasito_UsuarioCrea FOREIGN KEY (tipoPar_UsuarioCrea)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT FK_Medico_tbTiposParasito_UsuarioModifica FOREIGN KEY (tipoPar_UsuarioModifica)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    )
    PRINT 'Tabla [Medico].[tbTiposParasito] creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla [Medico].[tbTiposParasito] ya existe.'
END
GO

-- =============================================
-- Tabla: tbTiposEsterilizacion
-- Descripción: Catálogo de tipos de esterilización
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Medico].[tbTiposEsterilizacion]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Medico].[tbTiposEsterilizacion]
    (
        tipoEst_Id INT IDENTITY(1,1) NOT NULL,
        tipoEst_Descripcion NVARCHAR(100) NOT NULL,
        tipoEst_Sexo NVARCHAR(10) NULL, -- 'Macho', 'Hembra', 'Ambos'
        tipoEst_EsEliminado BIT NOT NULL DEFAULT 0,
        tipoEst_UsuarioCrea INT NOT NULL,
        tipoEst_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        tipoEst_UsuarioModifica INT NULL,
        tipoEst_FechaModifica DATETIME NULL,
        CONSTRAINT PK_Medico_tbTiposEsterilizacion PRIMARY KEY CLUSTERED (tipoEst_Id ASC),
        CONSTRAINT FK_Medico_tbTiposEsterilizacion_UsuarioCrea FOREIGN KEY (tipoEst_UsuarioCrea)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id),
        CONSTRAINT FK_Medico_tbTiposEsterilizacion_UsuarioModifica FOREIGN KEY (tipoEst_UsuarioModifica)
            REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    )
    PRINT 'Tabla [Medico].[tbTiposEsterilizacion] creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla [Medico].[tbTiposEsterilizacion] ya existe.'
END
GO

PRINT '============================================='
PRINT 'Script completado exitosamente.'
PRINT 'Total de tablas creadas: 6'
PRINT '============================================='
GO
