-- =============================================
-- Script: Inserción de Datos Iniciales - Módulo Médico
-- Autor: Claude Code
-- Fecha: 2025-10-31
-- Descripción: Inserta datos iniciales en las tablas catálogo del módulo veterinario
-- =============================================

USE PETSHOMEDB
GO

-- =============================================
-- Datos: tbTiposConsulta
-- =============================================
PRINT 'Insertando datos en [Medico].[tbTiposConsulta]...'

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposConsulta] WHERE tipoCon_Descripcion = 'General')
    INSERT INTO [Medico].[tbTiposConsulta] (tipoCon_Descripcion, tipoCon_UsuarioCrea)
    VALUES ('General', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposConsulta] WHERE tipoCon_Descripcion = 'Emergencia')
    INSERT INTO [Medico].[tbTiposConsulta] (tipoCon_Descripcion, tipoCon_UsuarioCrea)
    VALUES ('Emergencia', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposConsulta] WHERE tipoCon_Descripcion = 'Seguimiento')
    INSERT INTO [Medico].[tbTiposConsulta] (tipoCon_Descripcion, tipoCon_UsuarioCrea)
    VALUES ('Seguimiento', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposConsulta] WHERE tipoCon_Descripcion = 'Pre-quirúrgico')
    INSERT INTO [Medico].[tbTiposConsulta] (tipoCon_Descripcion, tipoCon_UsuarioCrea)
    VALUES ('Pre-quirúrgico', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposConsulta] WHERE tipoCon_Descripcion = 'Post-quirúrgico')
    INSERT INTO [Medico].[tbTiposConsulta] (tipoCon_Descripcion, tipoCon_UsuarioCrea)
    VALUES ('Post-quirúrgico', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposConsulta] WHERE tipoCon_Descripcion = 'Vacunación')
    INSERT INTO [Medico].[tbTiposConsulta] (tipoCon_Descripcion, tipoCon_UsuarioCrea)
    VALUES ('Vacunación', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposConsulta] WHERE tipoCon_Descripcion = 'Chequeo de rutina')
    INSERT INTO [Medico].[tbTiposConsulta] (tipoCon_Descripcion, tipoCon_UsuarioCrea)
    VALUES ('Chequeo de rutina', 1)

PRINT 'Datos insertados en [Medico].[tbTiposConsulta]: ' + CAST(@@ROWCOUNT AS VARCHAR(10))
GO

-- =============================================
-- Datos: tbGravedades
-- =============================================
PRINT 'Insertando datos en [Medico].[tbGravedades]...'

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbGravedades] WHERE grav_Descripcion = 'Leve')
    INSERT INTO [Medico].[tbGravedades] (grav_Descripcion, grav_UsuarioCrea)
    VALUES ('Leve', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbGravedades] WHERE grav_Descripcion = 'Moderada')
    INSERT INTO [Medico].[tbGravedades] (grav_Descripcion, grav_UsuarioCrea)
    VALUES ('Moderada', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbGravedades] WHERE grav_Descripcion = 'Grave')
    INSERT INTO [Medico].[tbGravedades] (grav_Descripcion, grav_UsuarioCrea)
    VALUES ('Grave', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbGravedades] WHERE grav_Descripcion = 'Crítica')
    INSERT INTO [Medico].[tbGravedades] (grav_Descripcion, grav_UsuarioCrea)
    VALUES ('Crítica', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbGravedades] WHERE grav_Descripcion = 'Terminal')
    INSERT INTO [Medico].[tbGravedades] (grav_Descripcion, grav_UsuarioCrea)
    VALUES ('Terminal', 1)

PRINT 'Datos insertados en [Medico].[tbGravedades]: ' + CAST(@@ROWCOUNT AS VARCHAR(10))
GO

-- =============================================
-- Datos: tbTiposMedicamento
-- =============================================
PRINT 'Insertando datos en [Medico].[tbTiposMedicamento]...'

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposMedicamento] WHERE tipoMed_Descripcion = 'Antibiótico')
    INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea)
    VALUES ('Antibiótico', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposMedicamento] WHERE tipoMed_Descripcion = 'Antiparasitario')
    INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea)
    VALUES ('Antiparasitario', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposMedicamento] WHERE tipoMed_Descripcion = 'Antiinflamatorio')
    INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea)
    VALUES ('Antiinflamatorio', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposMedicamento] WHERE tipoMed_Descripcion = 'Analgésico')
    INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea)
    VALUES ('Analgésico', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposMedicamento] WHERE tipoMed_Descripcion = 'Vitamina')
    INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea)
    VALUES ('Vitamina', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposMedicamento] WHERE tipoMed_Descripcion = 'Suplemento')
    INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea)
    VALUES ('Suplemento', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposMedicamento] WHERE tipoMed_Descripcion = 'Anestésico')
    INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea)
    VALUES ('Anestésico', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposMedicamento] WHERE tipoMed_Descripcion = 'Antipulgas')
    INSERT INTO [Medico].[tbTiposMedicamento] (tipoMed_Descripcion, tipoMed_UsuarioCrea)
    VALUES ('Antipulgas', 1)

PRINT 'Datos insertados en [Medico].[tbTiposMedicamento]: ' + CAST(@@ROWCOUNT AS VARCHAR(10))
GO

-- =============================================
-- Datos: tbViasAdministracion
-- =============================================
PRINT 'Insertando datos en [Medico].[tbViasAdministracion]...'

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbViasAdministracion] WHERE viaAdmin_Descripcion = 'Oral')
    INSERT INTO [Medico].[tbViasAdministracion] (viaAdmin_Descripcion, viaAdmin_UsuarioCrea)
    VALUES ('Oral', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbViasAdministracion] WHERE viaAdmin_Descripcion = 'Inyectable intramuscular')
    INSERT INTO [Medico].[tbViasAdministracion] (viaAdmin_Descripcion, viaAdmin_UsuarioCrea)
    VALUES ('Inyectable intramuscular', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbViasAdministracion] WHERE viaAdmin_Descripcion = 'Inyectable subcutánea')
    INSERT INTO [Medico].[tbViasAdministracion] (viaAdmin_Descripcion, viaAdmin_UsuarioCrea)
    VALUES ('Inyectable subcutánea', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbViasAdministracion] WHERE viaAdmin_Descripcion = 'Intravenosa')
    INSERT INTO [Medico].[tbViasAdministracion] (viaAdmin_Descripcion, viaAdmin_UsuarioCrea)
    VALUES ('Intravenosa', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbViasAdministracion] WHERE viaAdmin_Descripcion = 'Tópica')
    INSERT INTO [Medico].[tbViasAdministracion] (viaAdmin_Descripcion, viaAdmin_UsuarioCrea)
    VALUES ('Tópica', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbViasAdministracion] WHERE viaAdmin_Descripcion = 'Ocular')
    INSERT INTO [Medico].[tbViasAdministracion] (viaAdmin_Descripcion, viaAdmin_UsuarioCrea)
    VALUES ('Ocular', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbViasAdministracion] WHERE viaAdmin_Descripcion = 'Ótica')
    INSERT INTO [Medico].[tbViasAdministracion] (viaAdmin_Descripcion, viaAdmin_UsuarioCrea)
    VALUES ('Ótica', 1)

PRINT 'Datos insertados en [Medico].[tbViasAdministracion]: ' + CAST(@@ROWCOUNT AS VARCHAR(10))
GO

-- =============================================
-- Datos: tbTiposParasito
-- =============================================
PRINT 'Insertando datos en [Medico].[tbTiposParasito]...'

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposParasito] WHERE tipoPar_Descripcion = 'Pulgas')
    INSERT INTO [Medico].[tbTiposParasito] (tipoPar_Descripcion, tipoPar_Categoria, tipoPar_UsuarioCrea)
    VALUES ('Pulgas', 'Externo', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposParasito] WHERE tipoPar_Descripcion = 'Garrapatas')
    INSERT INTO [Medico].[tbTiposParasito] (tipoPar_Descripcion, tipoPar_Categoria, tipoPar_UsuarioCrea)
    VALUES ('Garrapatas', 'Externo', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposParasito] WHERE tipoPar_Descripcion = 'Ácaros')
    INSERT INTO [Medico].[tbTiposParasito] (tipoPar_Descripcion, tipoPar_Categoria, tipoPar_UsuarioCrea)
    VALUES ('Ácaros', 'Externo', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposParasito] WHERE tipoPar_Descripcion = 'Lombrices intestinales')
    INSERT INTO [Medico].[tbTiposParasito] (tipoPar_Descripcion, tipoPar_Categoria, tipoPar_UsuarioCrea)
    VALUES ('Lombrices intestinales', 'Interno', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposParasito] WHERE tipoPar_Descripcion = 'Giardia')
    INSERT INTO [Medico].[tbTiposParasito] (tipoPar_Descripcion, tipoPar_Categoria, tipoPar_UsuarioCrea)
    VALUES ('Giardia', 'Interno', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposParasito] WHERE tipoPar_Descripcion = 'Tenias')
    INSERT INTO [Medico].[tbTiposParasito] (tipoPar_Descripcion, tipoPar_Categoria, tipoPar_UsuarioCrea)
    VALUES ('Tenias', 'Interno', 1)

PRINT 'Datos insertados en [Medico].[tbTiposParasito]: ' + CAST(@@ROWCOUNT AS VARCHAR(10))
GO

-- =============================================
-- Datos: tbTiposEsterilizacion
-- =============================================
PRINT 'Insertando datos en [Medico].[tbTiposEsterilizacion]...'

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposEsterilizacion] WHERE tipoEst_Descripcion = 'Castración')
    INSERT INTO [Medico].[tbTiposEsterilizacion] (tipoEst_Descripcion, tipoEst_Sexo, tipoEst_UsuarioCrea)
    VALUES ('Castración', 'Macho', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposEsterilizacion] WHERE tipoEst_Descripcion = 'Ovariohisterectomía')
    INSERT INTO [Medico].[tbTiposEsterilizacion] (tipoEst_Descripcion, tipoEst_Sexo, tipoEst_UsuarioCrea)
    VALUES ('Ovariohisterectomía', 'Hembra', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposEsterilizacion] WHERE tipoEst_Descripcion = 'Vasectomía')
    INSERT INTO [Medico].[tbTiposEsterilizacion] (tipoEst_Descripcion, tipoEst_Sexo, tipoEst_UsuarioCrea)
    VALUES ('Vasectomía', 'Macho', 1)

IF NOT EXISTS (SELECT 1 FROM [Medico].[tbTiposEsterilizacion] WHERE tipoEst_Descripcion = 'Ligadura de trompas')
    INSERT INTO [Medico].[tbTiposEsterilizacion] (tipoEst_Descripcion, tipoEst_Sexo, tipoEst_UsuarioCrea)
    VALUES ('Ligadura de trompas', 'Hembra', 1)

PRINT 'Datos insertados en [Medico].[tbTiposEsterilizacion]: ' + CAST(@@ROWCOUNT AS VARCHAR(10))
GO

PRINT '============================================='
PRINT 'Script de datos iniciales completado.'
PRINT '============================================='
GO
