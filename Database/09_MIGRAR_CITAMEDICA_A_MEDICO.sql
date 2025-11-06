/*
=============================================
SCRIPT: Migración de tbCitaMedica al esquema Medico
AUTOR: Sistema PetsHome
FECHA: 2025-11-04
DESCRIPCIÓN:
  - Migra tbCitaMedica de [Refugio] a [Medico]
  - Agrega campos para integración con catálogos médicos
  - Mantiene datos existentes
=============================================
*/

USE PETSHOMEDB
GO

-- =============================================
-- PASO 1: Crear nueva tabla en esquema Medico
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbCitaMedica' AND schema_id = SCHEMA_ID('Medico'))
BEGIN
    CREATE TABLE [Medico].[tbCitaMedica]
    (
        -- IDENTIFICADORES
        cita_Id INT IDENTITY(1,1) NOT NULL,
        masc_Id INT NOT NULL,

        -- FECHA Y TIPO DE CONSULTA
        cita_FechaConsulta DATETIME NOT NULL,
        tipoCon_Id INT NULL,                    -- ✅ NUEVO: FK a tbTiposConsulta
        cita_MotivoConsulta NVARCHAR(500) NULL,

        -- DIAGNÓSTICO Y GRAVEDAD
        cita_Diagnostico NVARCHAR(500) NULL,
        grav_Id INT NULL,                       -- ✅ NUEVO: FK a tbGravedades

        -- SIGNOS VITALES
        cita_Peso DECIMAL(5,2) NULL,
        cita_Temperatura DECIMAL(4,2) NULL,
        cita_FrecuenciaCardiaca INT NULL,
        cita_FrecuenciaRespiratoria INT NULL,

        -- COMPORTAMIENTO
        com_Id INT NULL,

        -- VACUNAS (mantener por compatibilidad)
        vac_Id INT NULL,

        -- PROCEDIMIENTOS Y RESULTADOS
        cita_ProcedimientosRealizados NVARCHAR(500) NULL,
        cita_ResultadosExamenes NVARCHAR(500) NULL,

        -- NOTA: Campos de medicamentos eliminados - ahora van en tbRecetas
        -- cita_MedicamentosRecetados → tbRecetas
        -- cita_Dosificacion → tbRecetas

        -- PRÓXIMA CITA
        cita_ProximaCita DATETIME NULL,
        cita_MotivoProximaCita NVARCHAR(200) NULL,

        -- AUDITORÍA
        cita_EsEliminado BIT NOT NULL DEFAULT 0,
        cita_UsuarioCrea INT NOT NULL,
        cita_FechaCrea DATETIME NOT NULL DEFAULT GETDATE(),
        cita_UsuarioModifica INT NULL,
        cita_FechaModifica DATETIME NULL,

        -- PRIMARY KEY
        CONSTRAINT PK_Medico_tbCitaMedica PRIMARY KEY CLUSTERED (cita_Id ASC)
    )

    PRINT '✅ Tabla [Medico].[tbCitaMedica] creada exitosamente'
END
ELSE
BEGIN
    PRINT '⚠️ La tabla [Medico].[tbCitaMedica] ya existe'
END
GO

-- =============================================
-- PASO 2: Agregar Foreign Keys
-- =============================================

-- FK a Mascotas
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_CitaMedica_Mascota')
BEGIN
    ALTER TABLE [Medico].[tbCitaMedica]
    ADD CONSTRAINT FK_Medico_CitaMedica_Mascota
        FOREIGN KEY (masc_Id) REFERENCES [Refugio].[tbMascotas](masc_Id)
    PRINT '✅ FK a tbMascotas creada'
END

-- FK a TiposConsulta
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_CitaMedica_TipoConsulta')
BEGIN
    ALTER TABLE [Medico].[tbCitaMedica]
    ADD CONSTRAINT FK_Medico_CitaMedica_TipoConsulta
        FOREIGN KEY (tipoCon_Id) REFERENCES [Medico].[tbTiposConsulta](tipoCon_Id)
    PRINT '✅ FK a tbTiposConsulta creada'
END

-- FK a Gravedades
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_CitaMedica_Gravedad')
BEGIN
    ALTER TABLE [Medico].[tbCitaMedica]
    ADD CONSTRAINT FK_Medico_CitaMedica_Gravedad
        FOREIGN KEY (grav_Id) REFERENCES [Medico].[tbGravedades](grav_Id)
    PRINT '✅ FK a tbGravedades creada'
END

-- FK a Comportamientos
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_CitaMedica_Comportamiento')
BEGIN
    ALTER TABLE [Medico].[tbCitaMedica]
    ADD CONSTRAINT FK_Medico_CitaMedica_Comportamiento
        FOREIGN KEY (com_Id) REFERENCES [Refugio].[tbComportamientos](com_Id)
    PRINT '✅ FK a tbComportamientos creada'
END

-- FK a Usuarios (Creación)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_CitaMedica_UsuarioCrea')
BEGIN
    ALTER TABLE [Medico].[tbCitaMedica]
    ADD CONSTRAINT FK_Medico_CitaMedica_UsuarioCrea
        FOREIGN KEY (cita_UsuarioCrea) REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    PRINT '✅ FK a tbUsuarios (Creación) creada'
END

-- FK a Usuarios (Modificación)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Medico_CitaMedica_UsuarioModifica')
BEGIN
    ALTER TABLE [Medico].[tbCitaMedica]
    ADD CONSTRAINT FK_Medico_CitaMedica_UsuarioModifica
        FOREIGN KEY (cita_UsuarioModifica) REFERENCES [Seguridad].[tbUsuarios](usu_Id)
    PRINT '✅ FK a tbUsuarios (Modificación) creada'
END
GO

-- =============================================
-- PASO 3: Migrar datos existentes (SI EXISTEN)
-- =============================================

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'tbCitaMedica' AND schema_id = SCHEMA_ID('Refugio'))
BEGIN
    PRINT '📦 Migrando datos de [Refugio].[tbCitaMedica]...'

    SET IDENTITY_INSERT [Medico].[tbCitaMedica] ON

    INSERT INTO [Medico].[tbCitaMedica]
    (
        cita_Id,
        masc_Id,
        cita_FechaConsulta,
        cita_MotivoConsulta,
        cita_Diagnostico,
        cita_Peso,
        cita_Temperatura,
        cita_FrecuenciaCardiaca,
        cita_FrecuenciaRespiratoria,
        com_Id,
        vac_Id,
        cita_ProcedimientosRealizados,
        cita_ResultadosExamenes,
        cita_ProximaCita,
        cita_MotivoProximaCita,
        cita_EsEliminado,
        cita_UsuarioCrea,
        cita_FechaCrea,
        cita_UsuarioModifica,
        cita_FechaModifica
    )
    SELECT
        cita_Id,
        masc_Id,
        cita_FechaConsulta,
        cita_MotivoConsulta,
        cita_Diagnostico,
        cita_Peso,
        cita_Temperatura,
        cita_FrecuenciaCardiaca,
        cita_FrecuenciaRespiratoria,
        com_Id,
        vac_Id,
        cita_ProcedimientosRealizados,
        cita_ResultadosExamenes,
        cita_ProximaCita,
        cita_MotivoProximaCita,
        cita_EsEliminado,
        cita_UsuarioCrea,
        cita_FechaCrea,
        cita_UsuarioModifica,
        cita_FechaModifica
    FROM [Refugio].[tbCitaMedica]
    WHERE NOT EXISTS (
        SELECT 1 FROM [Medico].[tbCitaMedica]
        WHERE cita_Id = [Refugio].[tbCitaMedica].cita_Id
    )

    SET IDENTITY_INSERT [Medico].[tbCitaMedica] OFF

    DECLARE @registrosMigrados INT = @@ROWCOUNT
    PRINT '✅ ' + CAST(@registrosMigrados AS VARCHAR) + ' registros migrados'

    -- Migrar tabla relación con vacunas
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'tbCitaMedica_tbVacunas' AND schema_id = SCHEMA_ID('Refugio'))
    BEGIN
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbCitaMedica_tbVacunas' AND schema_id = SCHEMA_ID('Medico'))
        BEGIN
            CREATE TABLE [Medico].[tbCitaMedica_tbVacunas]
            (
                cita_Id INT NOT NULL,
                vac_Id INT NOT NULL,
                CONSTRAINT PK_Medico_CitaMedica_Vacunas PRIMARY KEY (cita_Id, vac_Id),
                CONSTRAINT FK_Medico_CitaMedica_Vacunas_Cita FOREIGN KEY (cita_Id) REFERENCES [Medico].[tbCitaMedica](cita_Id),
                CONSTRAINT FK_Medico_CitaMedica_Vacunas_Vacuna FOREIGN KEY (vac_Id) REFERENCES [Refugio].[tbVacunas](vac_Id)
            )
        END

        INSERT INTO [Medico].[tbCitaMedica_tbVacunas] (cita_Id, vac_Id)
        SELECT cita_Id, vac_Id
        FROM [Refugio].[tbCitaMedica_tbVacunas]
        WHERE NOT EXISTS (
            SELECT 1 FROM [Medico].[tbCitaMedica_tbVacunas]
            WHERE cita_Id = [Refugio].[tbCitaMedica_tbVacunas].cita_Id
            AND vac_Id = [Refugio].[tbCitaMedica_tbVacunas].vac_Id
        )

        PRINT '✅ Relación Citas-Vacunas migrada'
    END
END
ELSE
BEGIN
    PRINT '⚠️ No existe [Refugio].[tbCitaMedica] - no hay datos para migrar'
END
GO

-- =============================================
-- PASO 4: Crear índices para optimización
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CitaMedica_Mascota')
BEGIN
    CREATE NONCLUSTERED INDEX IX_CitaMedica_Mascota
    ON [Medico].[tbCitaMedica](masc_Id)
    PRINT '✅ Índice IX_CitaMedica_Mascota creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CitaMedica_Fecha')
BEGIN
    CREATE NONCLUSTERED INDEX IX_CitaMedica_Fecha
    ON [Medico].[tbCitaMedica](cita_FechaConsulta DESC)
    PRINT '✅ Índice IX_CitaMedica_Fecha creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CitaMedica_TipoConsulta')
BEGIN
    CREATE NONCLUSTERED INDEX IX_CitaMedica_TipoConsulta
    ON [Medico].[tbCitaMedica](tipoCon_Id)
    PRINT '✅ Índice IX_CitaMedica_TipoConsulta creado'
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CitaMedica_Gravedad')
BEGIN
    CREATE NONCLUSTERED INDEX IX_CitaMedica_Gravedad
    ON [Medico].[tbCitaMedica](grav_Id)
    PRINT '✅ Índice IX_CitaMedica_Gravedad creado'
END
GO

PRINT ''
PRINT '=========================================='
PRINT '✅ MIGRACIÓN COMPLETADA EXITOSAMENTE'
PRINT '=========================================='
PRINT 'Tabla: [Medico].[tbCitaMedica]'
PRINT 'Foreign Keys: 6 creadas'
PRINT 'Índices: 4 creados'
PRINT ''
PRINT 'NOTA: La tabla [Refugio].[tbCitaMedica] NO se elimina.'
PRINT '      Puedes eliminarla manualmente después de validar.'
PRINT '=========================================='
GO
