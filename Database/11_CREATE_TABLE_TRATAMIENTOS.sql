/*
=============================================
SCRIPT: Creación de tabla tbTratamientos
AUTOR: Sistema PetsHome
FECHA: 2025-11-04
DESCRIPCIÓN:
  - Tabla para gestionar tratamientos y desparasitación
  - Registra aplicación real de medicamentos
  - Integra con catálogos de parásitos, medicamentos y vías
=============================================
*/

USE PETSHOMEDB
GO

-- =============================================
-- PASO 1: Crear tabla tbTratamientos
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbTratamientos' AND schema_id = SCHEMA_ID('Medico'))
BEGIN
    CREATE TABLE [Medico].[tbTratamientos]
    (
        -- IDENTIFICADORES
        trat_Id INT IDENTITY(1,1) NOT NULL,
        masc_Id INT NOT NULL,                           -- FK a tbMascotas
        cita_Id INT NULL,                               -- FK opcional a tbCitaMedica
        receta_Id INT NULL,                             -- FK opcional a tbRecetas

        -- PARÁSITO (si aplica)
        tipoPar_Id INT NULL,                            -- FK a tbTiposParasito
        trat_ParasitoDetectado NVARCHAR(200) NULL,

        -- MEDICAMENTO APLICADO
        trat_Medicamento NVARCHAR(200) NULL,
        tipoMed_Id INT NULL,                            -- FK a tbTiposMedicamento
        viaAdmin_Id INT NULL,                           -- FK a tbViasAdministracion

        -- APLICACIÓN
        trat_FechaAplicacion DATETIME NOT NULL,
        trat_AplicadoPor NVARCHAR(100) NULL,
        trat_ProximaDosis DATE NULL,
        trat_Estado NVARCHAR(20) NOT NULL DEFAULT 'Iniciado',  -- Iniciado/EnCurso/Finalizado
        trat_Observaciones NVARCHAR(500) NULL,

        -- AUDITORÍA
        trat_EsEliminado BIT NOT NULL DEFAULT 0,
        trat_UsuarioCrea INT NOT NULL,
        trat_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        trat_UsuarioModifica INT NULL,
        trat_FechaModifica DATETIME NULL,

        -- PRIMARY KEY
        CONSTRAINT PK_Medico_tbTratamientos PRIMARY KEY CLUSTERED (trat_Id ASC)
    )

    PRINT '✅ Tabla [Medico].[tbTratamientos] creada exitosamente'
END
ELSE
BEGIN
    PRINT '⚠️ La tabla [Medico].[tbTratamientos] ya existe'
END
GO

-- =============================================
-- PASO 2: Agregar Foreign Keys
-- =============================================

-- FK a Mascotas
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Tratamientos_Mascota')
BEGIN
    ALTER TABLE [Medico].[tbTratamientos]
    ADD CONSTRAINT FK_Medico_Tratamientos_Mascota
        FOREIGN KEY (masc_Id) REFERENCES [Refugio].[tbMascotas](masc_Id)
    PRINT '✅ FK a tbMascotas creada'
END

-- FK a CitaMedica (opcional)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Tratamientos_CitaMedica')
BEGIN
    ALTER TABLE [Medico].[tbTratamientos]
    ADD CONSTRAINT FK_Medico_Tratamientos_CitaMedica
        FOREIGN KEY (cita_Id) REFERENCES [Medico].[tbCitaMedica](cita_Id)
    PRINT '✅ FK a tbCitaMedica creada'
END

-- FK a Recetas (opcional)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Tratamientos_Receta')
BEGIN
    ALTER TABLE [Medico].[tbTratamientos]
    ADD CONSTRAINT FK_Medico_Tratamientos_Receta
        FOREIGN KEY (receta_Id) REFERENCES [Medico].[tbRecetas](receta_Id)
    PRINT '✅ FK a tbRecetas creada'
END

-- FK a TiposParasito
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Tratamientos_TipoParasito')
BEGIN
    ALTER TABLE [Medico].[tbTratamientos]
    ADD CONSTRAINT FK_Medico_Tratamientos_TipoParasito
        FOREIGN KEY (tipoPar_Id) REFERENCES [Medico].[tbTiposParasito](tipoPar_Id)
    PRINT '✅ FK a tbTiposParasito creada'
END

-- FK a TiposMedicamento
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Tratamientos_TipoMedicamento')
BEGIN
    ALTER TABLE [Medico].[tbTratamientos]
    ADD CONSTRAINT FK_Medico_Tratamientos_TipoMedicamento
        FOREIGN KEY (tipoMed_Id) REFERENCES [Medico].[tbTiposMedicamento](tipoMed_Id)
    PRINT '✅ FK a tbTiposMedicamento creada'
END

-- FK a ViasAdministracion
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Tratamientos_ViaAdministracion')
BEGIN
    ALTER TABLE [Medico].[tbTratamientos]
    ADD CONSTRAINT FK_Medico_Tratamientos_ViaAdministracion
        FOREIGN KEY (viaAdmin_Id) REFERENCES [Medico].[tbViasAdministracion](viaAdmin_Id)
    PRINT '✅ FK a tbViasAdministracion creada'
END

-- FK a Usuarios (Creación)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Tratamientos_UsuarioCrea')
BEGIN
    ALTER TABLE [Medico].[tbTratamientos]
    ADD CONSTRAINT FK_Medico_Tratamientos_UsuarioCrea
        FOREIGN KEY (trat_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    PRINT '✅ FK a tbUsuarios (Creación) creada'
END

-- FK a Usuarios (Modificación)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_Tratamientos_UsuarioModifica')
BEGIN
    ALTER TABLE [Medico].[tbTratamientos]
    ADD CONSTRAINT FK_Medico_Tratamientos_UsuarioModifica
        FOREIGN KEY (trat_UsuarioModifica) REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    PRINT '✅ FK a tbUsuarios (Modificación) creada'
END
GO

-- =============================================
-- PASO 3: Crear índices
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tratamientos_Mascota')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tratamientos_Mascota
    ON [Medico].[tbTratamientos](masc_Id)
    PRINT '✅ Índice IX_Tratamientos_Mascota creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tratamientos_CitaMedica')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tratamientos_CitaMedica
    ON [Medico].[tbTratamientos](cita_Id)
    PRINT '✅ Índice IX_Tratamientos_CitaMedica creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tratamientos_FechaAplicacion')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tratamientos_FechaAplicacion
    ON [Medico].[tbTratamientos](trat_FechaAplicacion DESC)
    PRINT '✅ Índice IX_Tratamientos_FechaAplicacion creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tratamientos_TipoParasito')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tratamientos_TipoParasito
    ON [Medico].[tbTratamientos](tipoPar_Id)
    PRINT '✅ Índice IX_Tratamientos_TipoParasito creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tratamientos_Estado')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Tratamientos_Estado
    ON [Medico].[tbTratamientos](trat_Estado)
    WHERE trat_EsEliminado = 0
    PRINT '✅ Índice IX_Tratamientos_Estado creado'
END
GO

PRINT ''
PRINT '=========================================='
PRINT '✅ TABLA TRATAMIENTOS CREADA EXITOSAMENTE'
PRINT '=========================================='
PRINT 'Tabla: [Medico].[tbTratamientos]'
PRINT 'Foreign Keys: 8 creadas'
PRINT 'Índices: 5 creados'
PRINT '=========================================='
GO
