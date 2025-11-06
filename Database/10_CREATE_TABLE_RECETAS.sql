/*
=============================================
SCRIPT: Creación de tabla tbRecetas
AUTOR: Sistema PetsHome
FECHA: 2025-11-04
DESCRIPCIÓN:
  - Tabla para gestionar recetas médicas
  - Almacena medicamentos prescritos por veterinarios
  - Integra con catálogos de tipos de medicamento y vías de administración
=============================================
*/

USE PETSHOMEDB
GO

-- =============================================
-- PASO 1: Crear tabla tbRecetas
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbRecetas' AND schema_id = SCHEMA_ID('Medico'))
BEGIN
    CREATE TABLE [Medico].[tbRecetas]
    (
        -- IDENTIFICADORES
        receta_Id INT IDENTITY(1,1) NOT NULL,
        cita_Id INT NOT NULL,                           -- FK a tbCitaMedica
        masc_Id INT NOT NULL,                           -- FK a tbMascotas

        -- MEDICAMENTO
        receta_Medicamento NVARCHAR(200) NOT NULL,
        tipoMed_Id INT NULL,                            -- FK a tbTiposMedicamento
        viaAdmin_Id INT NULL,                           -- FK a tbViasAdministracion

        -- DOSIFICACIÓN
        receta_Dosis NVARCHAR(100) NULL,
        receta_Frecuencia NVARCHAR(100) NULL,           -- Ej: "Cada 12 horas"
        receta_Duracion NVARCHAR(50) NULL,              -- Ej: "7 días"
        receta_Instrucciones NVARCHAR(500) NULL,

        -- PERÍODO
        receta_FechaInicio DATE NULL,
        receta_FechaFin DATE NULL,
        receta_Estado NVARCHAR(20) NOT NULL DEFAULT 'Activo',  -- Activo/Completado/Suspendido

        -- AUDITORÍA
        receta_EsEliminado BIT NOT NULL DEFAULT 0,
        receta_UsuarioCrea INT NOT NULL,
        receta_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        receta_UsuarioModifica INT NULL,
        receta_FechaModifica DATETIME NULL,

        -- PRIMARY KEY
        CONSTRAINT PK_Medico_tbRecetas PRIMARY KEY CLUSTERED (receta_Id ASC)
    )

    PRINT '✅ Tabla [Medico].[tbRecetas] creada exitosamente'
END
ELSE
BEGIN
    PRINT '⚠️ La tabla [Medico].[tbRecetas] ya existe'
END
GO

-- =============================================
-- PASO 2: Agregar Foreign Keys
-- =============================================

-- FK a CitaMedica
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Recetas_CitaMedica')
BEGIN
    ALTER TABLE [Medico].[tbRecetas]
    ADD CONSTRAINT FK_Medico_Recetas_CitaMedica
        FOREIGN KEY (cita_Id) REFERENCES [Medico].[tbCitaMedica](cita_Id)
    PRINT '✅ FK a tbCitaMedica creada'
END

-- FK a Mascotas
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Recetas_Mascota')
BEGIN
    ALTER TABLE [Medico].[tbRecetas]
    ADD CONSTRAINT FK_Medico_Recetas_Mascota
        FOREIGN KEY (masc_Id) REFERENCES [Refugio].[tbMascotas](masc_Id)
    PRINT '✅ FK a tbMascotas creada'
END

-- FK a TiposMedicamento
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Recetas_TipoMedicamento')
BEGIN
    ALTER TABLE [Medico].[tbRecetas]
    ADD CONSTRAINT FK_Medico_Recetas_TipoMedicamento
        FOREIGN KEY (tipoMed_Id) REFERENCES [Medico].[tbTiposMedicamento](tipoMed_Id)
    PRINT '✅ FK a tbTiposMedicamento creada'
END

-- FK a ViasAdministracion
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Recetas_ViaAdministracion')
BEGIN
    ALTER TABLE [Medico].[tbRecetas]
    ADD CONSTRAINT FK_Medico_Recetas_ViaAdministracion
        FOREIGN KEY (viaAdmin_Id) REFERENCES [Medico].[tbViasAdministracion](viaAdmin_Id)
    PRINT '✅ FK a tbViasAdministracion creada'
END

-- FK a Usuarios (Creación)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Recetas_UsuarioCrea')
BEGIN
    ALTER TABLE [Medico].[tbRecetas]
    ADD CONSTRAINT FK_Medico_Recetas_UsuarioCrea
        FOREIGN KEY (receta_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    PRINT '✅ FK a tbUsuarios (Creación) creada'
END

-- FK a Usuarios (Modificación)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Recetas_UsuarioModifica')
BEGIN
    ALTER TABLE [Medico].[tbRecetas]
    ADD CONSTRAINT FK_Medico_Recetas_UsuarioModifica
        FOREIGN KEY (receta_UsuarioModifica) REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    PRINT '✅ FK a tbUsuarios (Modificación) creada'
END
GO

-- =============================================
-- PASO 3: Crear índices
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Recetas_CitaMedica')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Recetas_CitaMedica
    ON [Medico].[tbRecetas](cita_Id)
    PRINT '✅ Índice IX_Recetas_CitaMedica creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Recetas_Mascota')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Recetas_Mascota
    ON [Medico].[tbRecetas](masc_Id)
    PRINT '✅ Índice IX_Recetas_Mascota creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Recetas_Estado')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Recetas_Estado
    ON [Medico].[tbRecetas](receta_Estado)
    WHERE receta_EsEliminado = 0
    PRINT '✅ Índice IX_Recetas_Estado creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Recetas_TipoMedicamento')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Recetas_TipoMedicamento
    ON [Medico].[tbRecetas](tipoMed_Id)
    PRINT '✅ Índice IX_Recetas_TipoMedicamento creado'
END
GO

PRINT ''
PRINT '=========================================='
PRINT '✅ TABLA RECETAS CREADA EXITOSAMENTE'
PRINT '=========================================='
PRINT 'Tabla: [Medico].[tbRecetas]'
PRINT 'Foreign Keys: 6 creadas'
PRINT 'Índices: 4 creados'
PRINT '=========================================='
GO
