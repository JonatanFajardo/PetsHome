-- =============================================
-- SCRIPT: Limpiar datos de prueba y poblar con datos realistas de portafolio
-- Base de datos: PETSHOMEDB
-- Fecha: 2026-02-22
-- Descripcion: Elimina todos los datos de prueba (GUIDs, "Test", "CRUD", etc.),
--              resetea los IDENTITY seeds, e inserta datos realistas y coherentes.
-- =============================================

USE PETSHOMEDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

BEGIN TRANSACTION;

PRINT '========================================';
PRINT 'PASO 1: Deshabilitar FK constraints';
PRINT '========================================';

EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
PRINT 'FK constraints deshabilitadas.';

PRINT '';
PRINT '========================================';
PRINT 'PASO 2: Limpiar todas las tablas';
PRINT '========================================';

-- Junction tables
DELETE FROM Refugio.tbEventos_tbVoluntarios;
DELETE FROM Refugio.tbCitaMedica_tbVacunas;
DELETE FROM Medico.tbCitaMedica_tbVacunas;

-- Nivel 3 (hijas de hijas)
DELETE FROM Medico.tbTratamientos;
DELETE FROM Medico.tbRecetas;
DELETE FROM Inventario.tbRecepcionesDetalles;
DELETE FROM Inventario.tbEntradasDetalles;
DELETE FROM Inventario.tbSalidasDetalles;

-- Nivel 2
DELETE FROM Refugio.tbAdopciones;
DELETE FROM Medico.tbCitaMedica;
DELETE FROM Refugio.tbCitaMedica;
DELETE FROM Inventario.tbRecepcionesMercancia;
DELETE FROM Inventario.tbEntradas;
DELETE FROM Inventario.tbSalidas;
DELETE FROM Inventario.tbExistencias;
DELETE FROM Inventario.tbMovimientos;

-- Nivel 1
DELETE FROM Refugio.tbMascotas;
DELETE FROM Refugio.tbSolicitudes;
DELETE FROM Refugio.tbEmpleados;
DELETE FROM Refugio.tbVoluntarios;
DELETE FROM Refugio.tbEventos;
DELETE FROM Refugio.tbDonaciones;
DELETE FROM Inventario.tbItems;
DELETE FROM Rescate.tbIngresos;
DELETE FROM Rescate.tbReportesAbandono;

-- Catalogos
DELETE FROM Refugio.tbRazas;
DELETE FROM Refugio.tbProcedencias;
DELETE FROM Refugio.tbEmpleadosCargos;
DELETE FROM Refugio.tbVacunas;
DELETE FROM Refugio.tbComportamientos;
DELETE FROM Inventario.tbCategorias;

-- Catalogos medicos
DELETE FROM Medico.tbGravedades;
DELETE FROM Medico.tbTiposConsulta;
DELETE FROM Medico.tbTiposMedicamento;
DELETE FROM Medico.tbViasAdministracion;
DELETE FROM Medico.tbTiposParasito;
DELETE FROM Medico.tbTiposEsterilizacion;

-- Tablas base
DELETE FROM General.tbPersonas;
DELETE FROM Refugio.tbRefugios;

-- Logs
DELETE FROM Seguridad.tbRegistroEventos;

PRINT 'Todas las tablas limpiadas.';

PRINT '';
PRINT '========================================';
PRINT 'PASO 3: Resetear IDENTITY seeds';
PRINT '========================================';

-- Junction
DBCC CHECKIDENT ('Refugio.tbEventos_tbVoluntarios', RESEED, 0);
-- tbCitaMedica_tbVacunas no tiene IDENTITY

-- Medico
DBCC CHECKIDENT ('Medico.tbTratamientos', RESEED, 0);
DBCC CHECKIDENT ('Medico.tbRecetas', RESEED, 0);
DBCC CHECKIDENT ('Medico.tbCitaMedica', RESEED, 0);
DBCC CHECKIDENT ('Medico.tbGravedades', RESEED, 0);
DBCC CHECKIDENT ('Medico.tbTiposConsulta', RESEED, 0);
DBCC CHECKIDENT ('Medico.tbTiposMedicamento', RESEED, 0);
DBCC CHECKIDENT ('Medico.tbViasAdministracion', RESEED, 0);
DBCC CHECKIDENT ('Medico.tbTiposParasito', RESEED, 0);
DBCC CHECKIDENT ('Medico.tbTiposEsterilizacion', RESEED, 0);

-- Inventario
DBCC CHECKIDENT ('Inventario.tbRecepcionesDetalles', RESEED, 0);
DBCC CHECKIDENT ('Inventario.tbRecepcionesMercancia', RESEED, 0);
DBCC CHECKIDENT ('Inventario.tbEntradasDetalles', RESEED, 0);
DBCC CHECKIDENT ('Inventario.tbEntradas', RESEED, 0);
DBCC CHECKIDENT ('Inventario.tbSalidasDetalles', RESEED, 0);
DBCC CHECKIDENT ('Inventario.tbSalidas', RESEED, 0);
DBCC CHECKIDENT ('Inventario.tbItems', RESEED, 0);
DBCC CHECKIDENT ('Inventario.tbCategorias', RESEED, 0);

-- Refugio
DBCC CHECKIDENT ('Refugio.tbAdopciones', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbCitaMedica', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbMascotas', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbSolicitudes', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbEmpleados', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbVoluntarios', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbEventos', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbDonaciones', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbRazas', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbProcedencias', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbEmpleadosCargos', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbVacunas', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbComportamientos', RESEED, 0);
DBCC CHECKIDENT ('Refugio.tbRefugios', RESEED, 0);

-- General
DBCC CHECKIDENT ('General.tbPersonas', RESEED, 0);

-- Seguridad
DBCC CHECKIDENT ('Seguridad.tbRegistroEventos', RESEED, 0);

-- Rescate
DBCC CHECKIDENT ('Rescate.tbIngresos', RESEED, 0);
DBCC CHECKIDENT ('Rescate.tbReportesAbandono', RESEED, 0);

PRINT 'IDENTITY seeds reseteados.';

PRINT '';
PRINT '========================================';
PRINT 'PASO 4: Rehabilitar FK constraints';
PRINT '========================================';

EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
PRINT 'FK constraints rehabilitadas.';

PRINT '';
PRINT '========================================';
PRINT 'PASO 5: Insertar datos realistas';
PRINT '========================================';

-- =============================================
-- 5.1 CATALOGOS REFUGIO
-- =============================================

PRINT '  Insertando Procedencias...';
SET IDENTITY_INSERT Refugio.tbProcedencias ON;
INSERT INTO Refugio.tbProcedencias (proc_Id, proc_Descripcion, proc_EsActivo, proc_EsEliminado, proc_UsuarioCrea, proc_FechaCrea) VALUES
(1, N'Rescate callejero',       1, 0, 1, '2025-01-15'),
(2, N'Entrega voluntaria',      1, 0, 1, '2025-01-15'),
(3, N'Decomiso por maltrato',   1, 0, 1, '2025-01-15'),
(4, N'Transferencia entre refugios', 1, 0, 1, '2025-01-15'),
(5, N'Nacimiento en refugio',   1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Refugio.tbProcedencias OFF;

PRINT '  Insertando Comportamientos...';
SET IDENTITY_INSERT Refugio.tbComportamientos ON;
INSERT INTO Refugio.tbComportamientos (com_Id, com_Descripcion, com_EsEliminado, com_UsuarioCrea, com_FechaCrea) VALUES
(1,  N'Sociable',        0, 1, '2025-01-15'),
(2,  N'Timido',           0, 1, '2025-01-15'),
(3,  N'Activo',           0, 1, '2025-01-15'),
(4,  N'Carinoso',         0, 1, '2025-01-15'),
(5,  N'Independiente',    0, 1, '2025-01-15'),
(6,  N'Jugueton',         0, 1, '2025-01-15'),
(7,  N'Tranquilo',        0, 1, '2025-01-15'),
(8,  N'Protector',        0, 1, '2025-01-15'),
(9,  N'Docil',            0, 1, '2025-01-15'),
(10, N'Inteligente',      0, 1, '2025-01-15');
SET IDENTITY_INSERT Refugio.tbComportamientos OFF;

PRINT '  Insertando Empleados Cargos...';
SET IDENTITY_INSERT Refugio.tbEmpleadosCargos ON;
INSERT INTO Refugio.tbEmpleadosCargos (cag_Id, cag_Descripcion, cag_Salario, cag_EsActivo, cag_EsEliminado, cag_UsuarioCrea, cag_FechaCrea) VALUES
(1, N'Administrador General',     15000.00, 1, 0, 1, '2025-01-15'),
(2, N'Veterinario',               12000.00, 1, 0, 1, '2025-01-15'),
(3, N'Recepcionista',              7500.00, 1, 0, 1, '2025-01-15'),
(4, N'Cuidador de Animales',       7000.00, 1, 0, 1, '2025-01-15'),
(5, N'Encargado de Inventario',    8000.00, 1, 0, 1, '2025-01-15'),
(6, N'Conserje',                   6500.00, 1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Refugio.tbEmpleadosCargos OFF;

PRINT '  Insertando Razas...';
SET IDENTITY_INSERT Refugio.tbRazas ON;
INSERT INTO Refugio.tbRazas (raza_Id, raza_Descripcion, raza_Tamano, raza_TipoAnimal, raza_TipoPelaje, raza_ImagenUrl, raza_EsActivo, raza_EsEliminado, raza_UsuarioCrea, raza_FechaCrea) VALUES
(1,  N'Labrador Retriever',  'Grande',   'Perro', 'Corto y denso',   NULL, 1, 0, 1, '2025-01-15'),
(2,  N'Pastor Aleman',       'Grande',   'Perro', 'Medio y denso',   NULL, 1, 0, 1, '2025-01-15'),
(3,  N'Golden Retriever',    'Grande',   'Perro', 'Largo y ondulado', NULL, 1, 0, 1, '2025-01-15'),
(4,  N'Bulldog Frances',     'Pequeno',  'Perro', 'Corto y liso',    NULL, 1, 0, 1, '2025-01-15'),
(5,  N'Beagle',              'Mediano',  'Perro', 'Corto y denso',   NULL, 1, 0, 1, '2025-01-15'),
(6,  N'Chihuahua',           'Pequeno',  'Perro', 'Corto y liso',    NULL, 1, 0, 1, '2025-01-15'),
(7,  N'Rottweiler',          'Grande',   'Perro', 'Corto y grueso',  NULL, 1, 0, 1, '2025-01-15'),
(8,  N'Husky Siberiano',     'Grande',   'Perro', 'Largo y doble capa', NULL, 1, 0, 1, '2025-01-15'),
(9,  N'Mestizo Criollo',     'Mediano',  'Perro', 'Variado',         NULL, 1, 0, 1, '2025-01-15'),
(10, N'Schnauzer',           'Mediano',  'Perro', 'Duro y alambrado', NULL, 1, 0, 1, '2025-01-15'),
(11, N'Siames',              'Mediano',  'Gato',  'Corto y fino',    NULL, 1, 0, 1, '2025-01-15'),
(12, N'Persa',               'Mediano',  'Gato',  'Largo y sedoso',  NULL, 1, 0, 1, '2025-01-15'),
(13, N'Maine Coon',          'Grande',   'Gato',  'Largo y espeso',  NULL, 1, 0, 1, '2025-01-15'),
(14, N'Bengali',             'Mediano',  'Gato',  'Corto y brillante', NULL, 1, 0, 1, '2025-01-15'),
(15, N'Gato Mestizo',        'Pequeno',  'Gato',  'Variado',         NULL, 1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Refugio.tbRazas OFF;

PRINT '  Insertando Vacunas...';
SET IDENTITY_INSERT Refugio.tbVacunas ON;
INSERT INTO Refugio.tbVacunas (vac_Id, vac_Descripcion, vacu_Especie, vacu_DosisRecomendada, vacu_PeriodoRefuerzo, vac_EsActivo, vac_EsEliminado, vac_UsuarioCrea, vac_FechaCrea) VALUES
(1, N'Parvovirus',          'Perro',      '1 ml subcutanea',     'Anual',       1, 0, 1, '2025-01-15'),
(2, N'Moquillo',            'Perro',      '1 ml subcutanea',     'Anual',       1, 0, 1, '2025-01-15'),
(3, N'Rabia',               'Perro/Gato', '1 ml intramuscular',  'Anual',       1, 0, 1, '2025-01-15'),
(4, N'Polivalente Canina',  'Perro',      '1 ml subcutanea',     'Cada 3 anos', 1, 0, 1, '2025-01-15'),
(5, N'Triple Felina',       'Gato',       '1 ml subcutanea',     'Anual',       1, 0, 1, '2025-01-15'),
(6, N'Leucemia Felina',     'Gato',       '1 ml subcutanea',     'Anual',       1, 0, 1, '2025-01-15'),
(7, N'Bordetella',          'Perro',      '0.5 ml intranasal',   'Cada 6 meses', 1, 0, 1, '2025-01-15'),
(8, N'Leptospirosis',       'Perro',      '1 ml subcutanea',     'Anual',       1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Refugio.tbVacunas OFF;

-- =============================================
-- 5.2 CATALOGOS MEDICOS
-- =============================================

PRINT '  Insertando Gravedades...';
SET IDENTITY_INSERT Medico.tbGravedades ON;
INSERT INTO Medico.tbGravedades (grav_Id, grav_Descripcion, grav_EsActivo, grav_EsEliminado, grav_UsuarioCrea, grav_FechaCrea) VALUES
(1, N'Leve',      1, 0, 1, '2025-01-15'),
(2, N'Moderada',  1, 0, 1, '2025-01-15'),
(3, N'Grave',     1, 0, 1, '2025-01-15'),
(4, N'Critica',   1, 0, 1, '2025-01-15'),
(5, N'Terminal',  0, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Medico.tbGravedades OFF;

PRINT '  Insertando Tipos Consulta...';
SET IDENTITY_INSERT Medico.tbTiposConsulta ON;
INSERT INTO Medico.tbTiposConsulta (tipoCon_Id, tipoCon_Descripcion, tipoCon_EsActivo, tipoCon_EsEliminado, tipoCon_UsuarioCrea, tipoCon_FechaCrea) VALUES
(1, N'General',           1, 0, 1, '2025-01-15'),
(2, N'Emergencia',        1, 0, 1, '2025-01-15'),
(3, N'Seguimiento',       1, 0, 1, '2025-01-15'),
(4, N'Pre-quirurgico',    1, 0, 1, '2025-01-15'),
(5, N'Post-quirurgico',   1, 0, 1, '2025-01-15'),
(6, N'Vacunacion',        1, 0, 1, '2025-01-15'),
(7, N'Chequeo de rutina', 1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Medico.tbTiposConsulta OFF;

PRINT '  Insertando Tipos Medicamento...';
SET IDENTITY_INSERT Medico.tbTiposMedicamento ON;
INSERT INTO Medico.tbTiposMedicamento (tipoMed_Id, tipoMed_Descripcion, tipoMed_EsActivo, tipoMed_EsEliminado, tipoMed_UsuarioCrea, tipoMed_FechaCrea) VALUES
(1, N'Antibiotico',       1, 0, 1, '2025-01-15'),
(2, N'Antiparasitario',   1, 0, 1, '2025-01-15'),
(3, N'Antiinflamatorio',  1, 0, 1, '2025-01-15'),
(4, N'Analgesico',        1, 0, 1, '2025-01-15'),
(5, N'Vitamina',          1, 0, 1, '2025-01-15'),
(6, N'Suplemento',        1, 0, 1, '2025-01-15'),
(7, N'Anestesico',        1, 0, 1, '2025-01-15'),
(8, N'Antipulgas',        1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Medico.tbTiposMedicamento OFF;

PRINT '  Insertando Vias Administracion...';
SET IDENTITY_INSERT Medico.tbViasAdministracion ON;
INSERT INTO Medico.tbViasAdministracion (viaAdmin_Id, viaAdmin_Descripcion, viaAdmin_EsActivo, viaAdmin_EsEliminado, viaAdmin_UsuarioCrea, viaAdmin_FechaCrea) VALUES
(1, N'Oral',                       1, 0, 1, '2025-01-15'),
(2, N'Inyectable intramuscular',   1, 0, 1, '2025-01-15'),
(3, N'Inyectable subcutanea',      1, 0, 1, '2025-01-15'),
(4, N'Intravenosa',                1, 0, 1, '2025-01-15'),
(5, N'Topica',                     1, 0, 1, '2025-01-15'),
(6, N'Ocular',                     1, 0, 1, '2025-01-15'),
(7, N'Otica',                      1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Medico.tbViasAdministracion OFF;

PRINT '  Insertando Tipos Parasito...';
SET IDENTITY_INSERT Medico.tbTiposParasito ON;
INSERT INTO Medico.tbTiposParasito (tipoPar_Id, tipoPar_Descripcion, tipoPar_Categoria, tipoPar_EsActivo, tipoPar_EsEliminado, tipoPar_UsuarioCrea, tipoPar_FechaCrea) VALUES
(1, N'Pulgas',                   'Externo',  1, 0, 1, '2025-01-15'),
(2, N'Garrapatas',               'Externo',  1, 0, 1, '2025-01-15'),
(3, N'Acaros',                   'Externo',  1, 0, 1, '2025-01-15'),
(4, N'Lombrices intestinales',   'Interno',  1, 0, 1, '2025-01-15'),
(5, N'Giardia',                  'Interno',  1, 0, 1, '2025-01-15'),
(6, N'Tenias',                   'Interno',  1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Medico.tbTiposParasito OFF;

PRINT '  Insertando Tipos Esterilizacion...';
SET IDENTITY_INSERT Medico.tbTiposEsterilizacion ON;
INSERT INTO Medico.tbTiposEsterilizacion (tipoEst_Id, tipoEst_Descripcion, tipoEst_Sexo, tipoEst_EsActivo, tipoEst_EsEliminado, tipoEst_UsuarioCrea, tipoEst_FechaCrea) VALUES
(1, N'Castracion',            'Macho',  1, 0, 1, '2025-01-15'),
(2, N'Ovariohisterectomia',   'Hembra', 1, 0, 1, '2025-01-15'),
(3, N'Vasectomia',            'Macho',  1, 0, 1, '2025-01-15'),
(4, N'Ligadura de trompas',   'Hembra', 1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Medico.tbTiposEsterilizacion OFF;

-- =============================================
-- 5.3 REFUGIOS
-- =============================================

PRINT '  Insertando Refugios...';
SET IDENTITY_INSERT Refugio.tbRefugios ON;
INSERT INTO Refugio.tbRefugios (refg_Id, refg_Nombre, refg_Ubicacion, refg_RTN, refg_Telefono, refg_Correo, depto_Id, mpio_Id, refg_InformacionAdicional, refg_EsActivo, refg_EsEliminado, refg_UsuarioCrea, refg_FechaCrea) VALUES
(1, N'Refugio Las Lomas',     N'Barrio Guamilito, 3ra Avenida, 2da Calle NE',  '08019012345678', '25574312', 'laslomas@petshome.hn',     5, 63,  N'Refugio principal con capacidad para 80 animales. Cuenta con area de cuarentena y consultorio veterinario.', 1, 0, 1, '2025-01-15'),
(2, N'Refugio La Esperanza',  N'Colonia Kennedy, Boulevard Morazan, Bloque M',  '08019087654321', '22354567', 'esperanza@petshome.hn',    8, 110, N'Refugio urbano con enfoque en gatos y perros pequenos. Capacidad para 50 animales.',                        1, 0, 1, '2025-02-01'),
(3, N'Refugio Vida Animal',   N'Barrio La Isla, Calle Principal frente al parque', '08019011223344', '24431289', 'vidaanimal@petshome.hn', 1, 1,   N'Refugio costero especializado en rescate de animales en situacion de calle. Capacidad para 40 animales.',    1, 0, 1, '2025-03-01');
SET IDENTITY_INSERT Refugio.tbRefugios OFF;

-- =============================================
-- 5.4 CATEGORIAS INVENTARIO
-- =============================================

PRINT '  Insertando Categorias...';
SET IDENTITY_INSERT Inventario.tbCategorias ON;
INSERT INTO Inventario.tbCategorias (cat_Id, cat_Descripcion, cat_EsActivo, cat_EsEliminado, cat_UsuarioCrea, cat_FechaCrea) VALUES
(1, N'Alimento para Perros',  1, 0, 1, '2025-01-15'),
(2, N'Alimento para Gatos',   1, 0, 1, '2025-01-15'),
(3, N'Medicamentos',          1, 0, 1, '2025-01-15'),
(4, N'Juguetes',              1, 0, 1, '2025-01-15'),
(5, N'Accesorios',            1, 0, 1, '2025-01-15'),
(6, N'Limpieza e Higiene',    1, 0, 1, '2025-01-15'),
(7, N'Mobiliario',            1, 0, 1, '2025-01-15'),
(8, N'Insumos Medicos',       1, 0, 1, '2025-01-15');
SET IDENTITY_INSERT Inventario.tbCategorias OFF;

-- =============================================
-- 5.5 PERSONAS (22 total: 10 empleados + 12 voluntarios)
-- =============================================

PRINT '  Insertando Personas...';
SET IDENTITY_INSERT General.tbPersonas ON;
INSERT INTO General.tbPersonas (per_Id, per_Identidad, per_PrimerNombre, per_SegundoNombre, per_ApellidoPaterno, per_ApellidoMaterno, per_FechaNacimiento, per_Domicilio, per_Telefono, per_Correo, per_EsEliminado, per_UsuarioCrea, per_FechaCrea) VALUES
-- Empleados (1-10)
(1,  '0501199001234', N'Carlos',    N'Eduardo',  N'Martinez',   N'Lopez',     '1990-03-15', N'Col. Las Palmas, SPS',           '95551234', 'carlos.martinez@email.com',  0, 1, '2025-01-15'),
(2,  '0501198505678', N'Maria',     N'Fernanda', N'Rodriguez',  N'Garcia',    '1985-07-22', N'Barrio El Centro, SPS',          '95552345', 'maria.rodriguez@email.com',  0, 1, '2025-01-15'),
(3,  '0501199209012', N'Jose',      N'Luis',     N'Hernandez',  N'Mejia',     '1992-11-08', N'Col. Stibys, SPS',               '95553456', 'jose.hernandez@email.com',   0, 1, '2025-01-15'),
(4,  '0801198803456', N'Ana',       N'Patricia', N'Flores',     N'Castillo',  '1988-05-30', N'Col. Kennedy, Tegucigalpa',      '97771234', 'ana.flores@email.com',       0, 1, '2025-01-15'),
(5,  '0801199107890', N'Roberto',   N'Antonio',  N'Zuniga',     N'Reyes',     '1991-09-14', N'Col. Miraflores, Tegucigalpa',   '97772345', 'roberto.zuniga@email.com',   0, 1, '2025-01-15'),
(6,  '0801198601234', N'Sandra',    N'Elizabeth', N'Avila',     N'Pineda',    '1986-01-25', N'Barrio Morazan, Tegucigalpa',    '97773456', 'sandra.avila@email.com',     0, 1, '2025-01-15'),
(7,  '0101199305678', N'Miguel',    N'Angel',    N'Caballero',  N'Orellana',  '1993-04-17', N'Barrio La Isla, La Ceiba',       '94441234', 'miguel.caballero@email.com', 0, 1, '2025-01-15'),
(8,  '0101198709012', N'Carmen',    N'Lucia',    N'Padilla',    N'Velasquez', '1987-12-03', N'Col. El Naranjal, La Ceiba',     '94442345', 'carmen.padilla@email.com',   0, 1, '2025-01-15'),
(9,  '0501199403456', N'Fernando',  N'David',    N'Ramos',      N'Cruz',      '1994-06-20', N'Col. Universidad, SPS',          '95554567', 'fernando.ramos@email.com',   0, 1, '2025-01-15'),
(10, '0101199607890', N'Laura',     N'Daniela',  N'Bonilla',    N'Sierra',    '1996-02-11', N'Barrio Potreritos, La Ceiba',    '94443456', 'laura.bonilla@email.com',    0, 1, '2025-01-15'),
-- Voluntarios (11-22)
(11, '0501200201234', N'Kevin',     N'Alexander', N'Fuentes',   N'Molina',    '2002-08-05', N'Col. Satelite, SPS',             '95555678', 'kevin.fuentes@email.com',    0, 1, '2025-02-01'),
(12, '0501200305678', N'Gabriela',  N'Nicole',   N'Ponce',      N'Aguilar',   '2003-01-19', N'Barrio Rio Piedras, SPS',        '95556789', 'gabriela.ponce@email.com',   0, 1, '2025-02-01'),
(13, '0801200109012', N'Daniel',    N'Enrique',  N'Espinoza',   N'Bautista',  '2001-10-28', N'Col. Tocontin, Tegucigalpa',     '97774567', 'daniel.espinoza@email.com',  0, 1, '2025-02-01'),
(14, '0801200403456', N'Valeria',   NULL,         N'Montoya',   N'Salgado',   '2004-03-07', N'Col. Lomas del Guijarro, Teg.',  '97775678', 'valeria.montoya@email.com',  0, 1, '2025-02-01'),
(15, '0101200207890', N'Josue',     N'Emmanuel', N'Rivera',     N'Nunez',     '2002-07-14', N'Barrio Ingles, La Ceiba',        '94444567', 'josue.rivera@email.com',     0, 1, '2025-02-01'),
(16, '0101200501234', N'Andrea',    N'Michell',  N'Castro',     N'Zelaya',    '2005-11-22', N'Col. El Sauce, La Ceiba',        '94445678', 'andrea.castro@email.com',    0, 1, '2025-02-01'),
(17, '0501200005678', N'Oscar',     N'Mauricio', N'Mendez',     N'Torres',    '2000-04-09', N'Barrio Lempira, SPS',            '95557890', 'oscar.mendez@email.com',     0, 1, '2025-03-01'),
(18, '0801200309012', N'Isabella',  N'Sofia',    N'Ramirez',    N'Caceres',   '2003-09-16', N'Col. Palmira, Tegucigalpa',      '97776789', 'isabella.ramirez@email.com', 0, 1, '2025-03-01'),
(19, '0501200103456', N'Bryan',     N'Josue',    N'Perez',      N'Rivas',     '2001-12-01', N'Col. Trejo, SPS',                '95558901', 'bryan.perez@email.com',      0, 1, '2025-03-01'),
(20, '0801200207890', N'Stephanie', NULL,         N'Valle',     N'Amaya',     '2002-05-25', N'Col. Florencia, Tegucigalpa',    '97777890', 'stephanie.valle@email.com',  0, 1, '2025-03-01'),
(21, '0101200401234', N'Erick',     N'Joel',     N'Santos',     N'Ordonez',   '2004-08-18', N'Barrio El Centro, La Ceiba',     '94446789', 'erick.santos@email.com',     0, 1, '2025-03-01'),
(22, '0501200505678', N'Kimberly',  N'Paola',    N'Hernandez',  N'Lagos',     '2005-02-14', N'Col. Las Acacias, SPS',          '95559012', 'kimberly.hernandez@email.com', 0, 1, '2025-03-01');
SET IDENTITY_INSERT General.tbPersonas OFF;

-- =============================================
-- 5.6 EMPLEADOS (10 distribuidos en 3 refugios)
-- =============================================

PRINT '  Insertando Empleados...';
SET IDENTITY_INSERT Refugio.tbEmpleados ON;
INSERT INTO Refugio.tbEmpleados (emp_Id, emp_Codigo, per_Id, refg_Id, cag_Id, emp_EsActivo) VALUES
-- Refugio 1 - Las Lomas (SPS): 4 empleados
(1,  'EMP0001', 1,  1, 1, 1),  -- Carlos Martinez - Administrador
(2,  'EMP0002', 2,  1, 2, 1),  -- Maria Rodriguez - Veterinaria
(3,  'EMP0003', 3,  1, 4, 1),  -- Jose Hernandez - Cuidador
(4,  'EMP0004', 9,  1, 3, 1),  -- Fernando Ramos - Recepcionista
-- Refugio 2 - La Esperanza (Tegucigalpa): 3 empleados
(5,  'EMP0005', 4,  2, 1, 1),  -- Ana Flores - Administradora
(6,  'EMP0006', 5,  2, 2, 1),  -- Roberto Zuniga - Veterinario
(7,  'EMP0007', 6,  2, 5, 1),  -- Sandra Avila - Encargada Inventario
-- Refugio 3 - Vida Animal (La Ceiba): 3 empleados
(8,  'EMP0008', 7,  3, 1, 1),  -- Miguel Caballero - Administrador
(9,  'EMP0009', 8,  3, 2, 1),  -- Carmen Padilla - Veterinaria
(10, 'EMP0010', 10, 3, 4, 1);  -- Laura Bonilla - Cuidadora
SET IDENTITY_INSERT Refugio.tbEmpleados OFF;

-- =============================================
-- 5.7 VOLUNTARIOS (12)
-- =============================================

PRINT '  Insertando Voluntarios...';
SET IDENTITY_INSERT Refugio.tbVoluntarios ON;
INSERT INTO Refugio.tbVoluntarios (vol_Id, vol_HorasTrabajadas, per_Id, vol_Recurrente) VALUES
(1,  48, 11, 1),   -- Kevin Fuentes
(2,  36, 12, 1),   -- Gabriela Ponce
(3,  24, 13, 1),   -- Daniel Espinoza
(4,  52, 14, 1),   -- Valeria Montoya
(5,  16, 15, 0),   -- Josue Rivera
(6,  40, 16, 1),   -- Andrea Castro
(7,  28, 17, 1),   -- Oscar Mendez
(8,  20, 18, 0),   -- Isabella Ramirez
(9,  44, 19, 1),   -- Bryan Perez
(10, 32, 20, 1),   -- Stephanie Valle
(11, 12, 21, 0),   -- Erick Santos
(12, 56, 22, 1);   -- Kimberly Hernandez
SET IDENTITY_INSERT Refugio.tbVoluntarios OFF;

-- =============================================
-- 5.8 ITEMS INVENTARIO (20)
-- =============================================

PRINT '  Insertando Items...';
SET IDENTITY_INSERT Inventario.tbItems ON;
INSERT INTO Inventario.tbItems (itm_Id, itm_Codigo, itm_Descripcion, cat_Id, itm_Precio, itm_EsEliminado, itm_UsuarioCrea, itm_FechaCrea) VALUES
-- Alimento Perros (cat_Id=1)
(1,  'ALI001', 'Concentrado Premium Adulto 15kg',      1, 850.00,  0, 1, '2025-01-20'),
(2,  'ALI002', 'Concentrado Cachorro 8kg',              1, 620.00,  0, 1, '2025-01-20'),
(3,  'ALI003', 'Alimento Humedo Latas x12',             1, 480.00,  0, 1, '2025-01-20'),
-- Alimento Gatos (cat_Id=2)
(4,  'ALI004', 'Concentrado Gato Adulto 8kg',           2, 650.00,  0, 1, '2025-01-20'),
(5,  'ALI005', 'Concentrado Gato Castrado 4kg',         2, 420.00,  0, 1, '2025-01-20'),
-- Medicamentos (cat_Id=3)
(6,  'MED001', 'Antiparasitario Interno 50ml',          3, 120.00,  0, 1, '2025-01-20'),
(7,  'MED002', 'Antibiotico Amoxicilina 100ml',         3, 185.00,  0, 1, '2025-01-20'),
(8,  'MED003', 'Antipulgas Topico x3 pipetas',          3, 350.00,  0, 1, '2025-01-20'),
-- Juguetes (cat_Id=4)
(9,  'JUG001', 'Pelota de Goma Mediana',                4, 45.00,   0, 1, '2025-01-20'),
(10, 'JUG002', 'Hueso Masticable Grande',                4, 65.00,   0, 1, '2025-01-20'),
(11, 'JUG003', 'Raton de Peluche para Gato',             4, 35.00,   0, 1, '2025-01-20'),
-- Accesorios (cat_Id=5)
(12, 'ACC001', 'Collar Ajustable Mediano',               5, 95.00,   0, 1, '2025-01-20'),
(13, 'ACC002', 'Correa Retractil 5m',                    5, 180.00,  0, 1, '2025-01-20'),
(14, 'ACC003', 'Placa de Identificacion',                5, 60.00,   0, 1, '2025-01-20'),
-- Limpieza (cat_Id=6)
(15, 'LIM001', 'Champu Antipulgas 500ml',                6, 145.00,  0, 1, '2025-01-20'),
(16, 'LIM002', 'Desinfectante de Jaulas 1L',             6, 85.00,   0, 1, '2025-01-20'),
(17, 'LIM003', 'Bolsas Sanitarias x100',                 6, 55.00,   0, 1, '2025-01-20'),
-- Mobiliario (cat_Id=7)
(18, 'MOB001', 'Cama para Perro Grande',                  7, 450.00,  0, 1, '2025-01-20'),
(19, 'MOB002', 'Jaula de Transporte Mediana',             7, 1200.00, 0, 1, '2025-01-20'),
-- Insumos Medicos (cat_Id=8)
(20, 'INS001', 'Guantes Quirurgicos Caja x100',           8, 280.00,  0, 1, '2025-01-20');
SET IDENTITY_INSERT Inventario.tbItems OFF;

-- =============================================
-- 5.9 MASCOTAS (25)
-- =============================================

PRINT '  Insertando Mascotas...';
SET IDENTITY_INSERT Refugio.tbMascotas ON;
INSERT INTO Refugio.tbMascotas (masc_Id, masc_Nombre, raza_Id, masc_Edad, masc_Sexo, masc_Peso, masc_Color, masc_Historia, refg_Id, proc_Id, masc_EsAdoptado, masc_EsReservado, masc_EsEliminado, masc_UsuarioCrea, masc_FechaCrea) VALUES
-- Refugio 1 - Las Lomas (10 mascotas)
(1,  N'Thor',     1,  3, 'M', 28.5, N'Dorado',                     N'Rescatado de la calle en condiciones de desnutricion. Se ha recuperado completamente y es muy sociable.',                        1, 1, 1, 0, 0, 1, '2025-02-10'),
(2,  N'Luna',     9,  2, 'H', 12.0, N'Negro con blanco',           N'Entregada por su dueno anterior por cambio de domicilio. Es calmada y buena con ninos.',                                       1, 2, 0, 0, 0, 1, '2025-02-15'),
(3,  N'Max',      2,  4, 'M', 32.0, N'Negro con cafe',             N'Decomisado por maltrato animal. Despues de rehabilitacion muestra buen comportamiento.',                                        1, 3, 0, 0, 0, 1, '2025-03-01'),
(4,  N'Mia',      11, 1, 'H',  3.8, N'Crema con cafe oscuro',      N'Nacida en el refugio. Juguetona y curiosa.',                                                                                    1, 5, 0, 0, 0, 1, '2025-03-10'),
(5,  N'Rocky',    7,  5, 'M', 38.0, N'Negro con marcas cafe',      N'Encontrado abandonado en un parque. Fuerte y protector, necesita dueno con experiencia.',                                        1, 1, 0, 1, 0, 1, '2025-03-20'),
(6,  N'Coco',     6,  2, 'M',  2.5, N'Cafe claro',                 N'Rescatado de una caja en la calle siendo cachorro. Muy carinoso y apegado.',                                                     1, 1, 1, 0, 0, 1, '2025-04-05'),
(7,  N'Nala',     15, 3, 'H',  4.2, N'Atigrada gris',              N'Entregada por vecinos que la encontraron en un terreno baldio. Independiente pero afectuosa.',                                   1, 2, 0, 0, 0, 1, '2025-04-15'),
(8,  N'Bruno',    5,  3, 'M', 15.0, N'Tricolor',                   N'Transferido desde otro refugio por falta de espacio. Activo y obediente.',                                                       1, 4, 0, 0, 0, 1, '2025-05-01'),
(9,  N'Kira',     3,  1, 'H', 22.0, N'Dorado claro',               N'Cachorra entregada voluntariamente. Muy inteligente y facil de entrenar.',                                                       1, 2, 0, 0, 0, 1, '2025-05-20'),
(10, N'Simba',    13, 2, 'M',  6.5, N'Naranja con blanco',         N'Gato rescatado de un arbol. Grande y majestuoso, se lleva bien con otros gatos.',                                                1, 1, 0, 0, 0, 1, '2025-06-01'),

-- Refugio 2 - La Esperanza (8 mascotas)
(11, N'Bella',    9,  4, 'H', 18.0, N'Cafe con blanco',            N'Encontrada en las calles de Tegucigalpa. Muy docil y sociable con personas y otros perros.',                                     2, 1, 1, 0, 0, 1, '2025-02-20'),
(12, N'Oliver',   12, 5, 'M',  5.0, N'Blanco puro',                N'Gato persa entregado por su dueno por alergias familiares. Tranquilo y amigable.',                                               2, 2, 0, 0, 0, 1, '2025-03-05'),
(13, N'Duke',     1,  2, 'M', 25.0, N'Chocolate',                  N'Labrador joven rescatado de una construccion abandonada. Energetico y jugueton.',                                                2, 1, 0, 0, 0, 1, '2025-03-15'),
(14, N'Lily',     14, 1, 'H',  3.5, N'Manchas marron y negro',     N'Gata bengali encontrada como cachorra. Muy activa y curiosa.',                                                                  2, 1, 0, 0, 0, 1, '2025-04-01'),
(15, N'Zeus',     8,  3, 'M', 28.0, N'Gris y blanco',              N'Husky decomisado por tenerlo en condiciones inadecuadas. Ya rehabilitado.',                                                      2, 3, 0, 1, 0, 1, '2025-04-20'),
(16, N'Canela',   10, 4, 'H',  8.5, N'Gris sal y pimienta',        N'Schnauzer entregada voluntariamente por familia que emigra. Bien entrenada.',                                                    2, 2, 1, 0, 0, 1, '2025-05-10'),
(17, N'Toby',     4,  2, 'M',  9.0, N'Blanco con manchas negras',  N'Bulldog frances encontrado deambulando en un mercado. Amigable y tranquilo.',                                                    2, 1, 0, 0, 0, 1, '2025-05-25'),
(18, N'Michi',    15, 6, 'H',  3.0, N'Negra',                      N'Gata rescatada de la calle ya adulta. Timida al principio pero carinosa cuando toma confianza.',                                 2, 1, 0, 0, 0, 1, '2025-06-10'),

-- Refugio 3 - Vida Animal (7 mascotas)
(19, N'Jack',     9,  3, 'M', 20.0, N'Negro',                      N'Perro mestizo rescatado de la playa. Sociable y le encanta el agua.',                                                             3, 1, 0, 0, 0, 1, '2025-03-01'),
(20, N'Lola',     5,  5, 'H', 12.0, N'Tricolor',                   N'Beagle mayor encontrada en condiciones de abandono. Tranquila y carinosa.',                                                      3, 1, 1, 0, 0, 1, '2025-03-15'),
(21, N'Cleo',     11, 2, 'H',  3.5, N'Azul point',                 N'Gata siamesa entregada por un vecino. Muy vocal y sociable.',                                                                    3, 2, 0, 0, 0, 1, '2025-04-01'),
(22, N'Rex',      2,  6, 'M', 35.0, N'Negro con marcas cafe',      N'Pastor aleman senior transferido de otro refugio. Leal y protector.',                                                            3, 4, 0, 0, 0, 1, '2025-04-20'),
(23, N'Pelusa',   15, 1, 'H',  2.8, N'Gris atigrada',              N'Gatita nacida en el refugio. Juguetona e hiperactiva.',                                                                          3, 5, 0, 0, 0, 1, '2025-05-05'),
(24, N'Firulais', 9,  4, 'M', 22.0, N'Cafe con blanco',            N'Perro criollo rescatado de inundacion. Agradecido y fiel.',                                                                      3, 1, 0, 0, 0, 1, '2025-05-20'),
(25, N'Daisy',    3,  1, 'H', 20.0, N'Dorado',                     N'Golden retriever cachorra entregada voluntariamente. Excelente temperamento.',                                                    3, 2, 0, 1, 0, 1, '2025-06-15');
SET IDENTITY_INSERT Refugio.tbMascotas OFF;

-- =============================================
-- 5.10 SOLICITUDES DE ADOPCION (10)
-- =============================================

PRINT '  Insertando Solicitudes...';
SET IDENTITY_INSERT Refugio.tbSolicitudes ON;
INSERT INTO Refugio.tbSolicitudes (sol_Id, sol_Identidad, sol_Nombres, sol_Apellidos, sol_Telefono, sol_Correo, sol_Fecha, masc_Id, sol_EsEliminado, sol_UsuarioCrea, sol_FechaCrea) VALUES
(1,  '0501198512345', N'Patricia Elena',    N'Mendez Rivera',     '95561234', 'patricia.mendez@email.com',  '2025-04-10', 1,  0, 1, '2025-04-10'),
(2,  '0801199023456', N'Jorge Alberto',     N'Sandoval Pineda',   '97781234', 'jorge.sandoval@email.com',   '2025-04-15', 6,  0, 1, '2025-04-15'),
(3,  '0501199534567', N'Karla Marcela',     N'Orellana Bautista', '95562345', 'karla.orellana@email.com',   '2025-05-01', 11, 0, 1, '2025-05-01'),
(4,  '0801200045678', N'Luis Fernando',     N'Turcios Mejia',     '97782345', 'luis.turcios@email.com',     '2025-05-10', 16, 0, 1, '2025-05-10'),
(5,  '0101199256789', N'Rosa Maria',        N'Gutierrez Lagos',   '94451234', 'rosa.gutierrez@email.com',   '2025-05-20', 20, 0, 1, '2025-05-20'),
(6,  '0501198867890', N'Carlos Humberto',   N'Portillo Cruz',     '95563456', 'carlos.portillo@email.com',  '2025-06-01', 2,  0, 1, '2025-06-01'),
(7,  '0801199178901', N'Diana Carolina',    N'Velasquez Sosa',    '97783456', 'diana.velasquez@email.com',  '2025-06-10', 5,  0, 1, '2025-06-10'),
(8,  '0101200089012', N'Marco Antonio',     N'Duron Aguilar',     '94452345', 'marco.duron@email.com',      '2025-06-15', 15, 0, 1, '2025-06-15'),
(9,  '0501199390123', N'Jennifer Paola',    N'Euceda Flores',     '95564567', 'jennifer.euceda@email.com',  '2025-07-01', 3,  0, 1, '2025-07-01'),
(10, '0801198601234', N'Hector Danilo',     N'Contreras Valle',   '97784567', 'hector.contreras@email.com', '2025-07-10', 13, 0, 1, '2025-07-10');
SET IDENTITY_INSERT Refugio.tbSolicitudes OFF;

-- =============================================
-- 5.11 ADOPCIONES (8)
-- =============================================

PRINT '  Insertando Adopciones...';
SET IDENTITY_INSERT Refugio.tbAdopciones ON;
INSERT INTO Refugio.tbAdopciones (adop_Id, sol_Id, adop_EsAprobado, adop_Estado, adop_EsEliminado, adop_UsuarioCrea, adop_FechaCrea) VALUES
(1, 1,  1, 'Aprobado',   0, 1, '2025-04-15'),   -- Thor adoptado
(2, 2,  1, 'Aprobado',   0, 1, '2025-04-20'),   -- Coco adoptado
(3, 3,  1, 'Aprobado',   0, 1, '2025-05-05'),   -- Bella adoptada
(4, 4,  1, 'Aprobado',   0, 1, '2025-05-15'),   -- Canela adoptada
(5, 5,  1, 'Aprobado',   0, 1, '2025-05-25'),   -- Lola adoptada
(6, 6,  0, 'Pendiente',  0, 1, '2025-06-05'),   -- Luna pendiente
(7, 7,  0, 'Rechazado',  0, 1, '2025-06-15'),   -- Rocky rechazado (necesita experiencia)
(8, 8,  0, 'Pendiente',  0, 1, '2025-06-20');   -- Zeus pendiente
SET IDENTITY_INSERT Refugio.tbAdopciones OFF;

-- =============================================
-- 5.12 EVENTOS (6)
-- =============================================

PRINT '  Insertando Eventos...';
SET IDENTITY_INSERT Refugio.tbEventos ON;
INSERT INTO Refugio.tbEventos (eve_Id, eve_Descripcion, refg_Id, eve_HoraInicio, eve_HoraFinal, eve_Fecha, eve_EsEliminado, eve_UsuarioCrea, eve_FechaCrea) VALUES
(1, N'Jornada de Adopcion Masiva - Las Lomas',               1, '09:00', '16:00', '2025-06-15', 0, 1, '2025-05-15'),
(2, N'Campana de Vacunacion Antirabica',                      1, '08:00', '14:00', '2025-07-20', 0, 1, '2025-06-20'),
(3, N'Feria de Mascotas Tegucigalpa 2025',                    2, '10:00', '18:00', '2025-08-10', 0, 1, '2025-07-01'),
(4, N'Jornada de Esterilizacion Gratuita',                    2, '07:00', '15:00', '2025-09-05', 0, 1, '2025-08-01'),
(5, N'Festival Costeno de Adopcion',                           3, '09:00', '17:00', '2025-07-04', 0, 1, '2025-06-01'),
(6, N'Charla Educativa: Tenencia Responsable de Mascotas',    3, '14:00', '17:00', '2025-08-22', 0, 1, '2025-07-15');
SET IDENTITY_INSERT Refugio.tbEventos OFF;

-- =============================================
-- 5.12b EVENTOS-VOLUNTARIOS (junction)
-- =============================================

PRINT '  Insertando Eventos-Voluntarios...';
SET IDENTITY_INSERT Refugio.tbEventos_tbVoluntarios ON;
INSERT INTO Refugio.tbEventos_tbVoluntarios (evevol_Id, eve_Id, vol_Id) VALUES
(1, 1, 1),   -- Kevin en Jornada Adopcion SPS
(2, 1, 2),   -- Gabriela en Jornada Adopcion SPS
(3, 2, 7),   -- Oscar en Campana Vacunacion
(4, 3, 3),   -- Daniel en Feria Tegucigalpa
(5, 3, 4),   -- Valeria en Feria Tegucigalpa
(6, 4, 8),   -- Isabella en Jornada Esterilizacion
(7, 5, 5),   -- Josue en Festival Costeno
(8, 5, 6),   -- Andrea en Festival Costeno
(9, 6, 11),  -- Erick en Charla Educativa
(10, 6, 6);  -- Andrea Castro en Charla Educativa (segundo evento)
SET IDENTITY_INSERT Refugio.tbEventos_tbVoluntarios OFF;

-- =============================================
-- 5.13 RECEPCIONES DE MERCANCIA (6) + DETALLES (18)
-- =============================================

PRINT '  Insertando Recepciones de Mercancia...';
SET IDENTITY_INSERT Inventario.tbRecepcionesMercancia ON;
INSERT INTO Inventario.tbRecepcionesMercancia (recep_Id, recep_Descripcion, recep_Fecha, refg_Id, recep_EsEliminado, recep_UsuarioCrea, recep_FechaCrea, recep_TipoRecepcion, recep_OrigenId, recep_NumeroDocumento) VALUES
(1, N'Compra inicial de alimentos e insumos - Las Lomas',             '2025-02-01', 1, 0, 1, '2025-02-01', 'C', NULL, 'FAC-2025-001'),
(2, N'Donacion de medicamentos y accesorios - Las Lomas',             '2025-04-15', 1, 0, 1, '2025-04-15', 'D', NULL, 'DON-2025-001'),
(3, N'Compra mensual de alimentos - La Esperanza',                     '2025-03-01', 2, 0, 1, '2025-03-01', 'C', NULL, 'FAC-2025-002'),
(4, N'Compra de insumos medicos y limpieza - La Esperanza',            '2025-05-10', 2, 0, 1, '2025-05-10', 'C', NULL, 'FAC-2025-003'),
(5, N'Compra inicial de suministros - Vida Animal',                     '2025-03-15', 3, 0, 1, '2025-03-15', 'C', NULL, 'FAC-2025-004'),
(6, N'Donacion de juguetes y accesorios - Vida Animal',                 '2025-06-01', 3, 0, 1, '2025-06-01', 'D', NULL, 'DON-2025-002');
SET IDENTITY_INSERT Inventario.tbRecepcionesMercancia OFF;

PRINT '  Insertando Recepciones Detalles...';
SET IDENTITY_INSERT Inventario.tbRecepcionesDetalles ON;
INSERT INTO Inventario.tbRecepcionesDetalles (recdet_Id, recep_Id, itm_Id, recdet_Cantidad, recdet_EsEliminado, recdet_UsuarioCrea, recdet_FechaCrea, recdet_PrecioUnitario, recdet_FechaVencimiento, recdet_NumeroLote) VALUES
-- Recepcion 1 (Las Lomas - alimentos)
(1,  1, 1,  10, 0, 1, '2025-02-01', 850.00,  '2026-02-01', 'LOT-ALI-001'),
(2,  1, 2,   5, 0, 1, '2025-02-01', 620.00,  '2026-02-01', 'LOT-ALI-002'),
(3,  1, 4,   8, 0, 1, '2025-02-01', 650.00,  '2026-02-01', 'LOT-ALI-003'),
-- Recepcion 2 (Las Lomas - donacion medicamentos)
(4,  2, 6,  15, 0, 1, '2025-04-15', 120.00,  '2026-04-15', 'LOT-MED-001'),
(5,  2, 8,  10, 0, 1, '2025-04-15', 350.00,  '2026-04-15', 'LOT-MED-002'),
(6,  2, 12,  20, 0, 1, '2025-04-15', 95.00,  NULL,          NULL),
-- Recepcion 3 (La Esperanza - alimentos)
(7,  3, 1,   8, 0, 1, '2025-03-01', 850.00,  '2026-03-01', 'LOT-ALI-004'),
(8,  3, 4,   6, 0, 1, '2025-03-01', 650.00,  '2026-03-01', 'LOT-ALI-005'),
(9,  3, 5,   5, 0, 1, '2025-03-01', 420.00,  '2026-03-01', 'LOT-ALI-006'),
-- Recepcion 4 (La Esperanza - insumos)
(10, 4, 7,   12, 0, 1, '2025-05-10', 185.00, '2026-05-10', 'LOT-MED-003'),
(11, 4, 15,   8, 0, 1, '2025-05-10', 145.00, '2026-11-10', 'LOT-LIM-001'),
(12, 4, 20,   5, 0, 1, '2025-05-10', 280.00, '2026-11-10', 'LOT-INS-001'),
-- Recepcion 5 (Vida Animal - suministros)
(13, 5, 1,   6, 0, 1, '2025-03-15', 850.00,  '2026-03-15', 'LOT-ALI-007'),
(14, 5, 3,   4, 0, 1, '2025-03-15', 480.00,  '2026-03-15', 'LOT-ALI-008'),
(15, 5, 16,  10, 0, 1, '2025-03-15', 85.00,   '2026-09-15', 'LOT-LIM-002'),
-- Recepcion 6 (Vida Animal - donacion juguetes)
(16, 6, 9,   15, 0, 1, '2025-06-01', 45.00,  NULL,          NULL),
(17, 6, 10,  10, 0, 1, '2025-06-01', 65.00,  NULL,          NULL),
(18, 6, 13,   8, 0, 1, '2025-06-01', 180.00, NULL,          NULL);
SET IDENTITY_INSERT Inventario.tbRecepcionesDetalles OFF;

-- =============================================
-- 5.14 CITAS MEDICAS (15) - Schema Medico
-- =============================================

PRINT '  Insertando Citas Medicas...';
SET IDENTITY_INSERT Medico.tbCitaMedica ON;
INSERT INTO Medico.tbCitaMedica (cita_Id, masc_Id, cita_FechaConsulta, tipoCon_Id, cita_MotivoConsulta, cita_Diagnostico, grav_Id, cita_Peso, cita_Temperatura, cita_FrecuenciaCardiaca, cita_FrecuenciaRespiratoria, com_Id, vac_Id, cita_ProcedimientosRealizados, cita_ResultadosExamenes, cita_ProximaCita, cita_MotivoProximaCita, cita_EsEliminado, cita_UsuarioCrea, cita_FechaCrea) VALUES
-- Consultas de ingreso
(1,  1,  '2025-02-10', 1, N'Evaluacion inicial al ingreso del refugio',          N'Desnutricion leve, parasitos internos',                 2, 22.0, 38.5, 90, 22, 2, NULL, N'Examen fisico completo, desparasitacion',              N'Hemograma: anemia leve',          '2025-03-10', N'Control de peso y parasitos',       0, 1, '2025-02-10'),
(2,  2,  '2025-02-15', 7, N'Chequeo de rutina al ingreso',                        N'Buena salud general',                                    1, 11.5, 38.2, 85, 20, 1, 3,   N'Examen fisico, vacunacion antirabica',                  N'Sin hallazgos anormales',         '2025-08-15', N'Refuerzo de vacunas',               0, 1, '2025-02-15'),
(3,  3,  '2025-03-01', 1, N'Evaluacion post-decomiso',                             N'Lesiones cutaneas por maltrato, desnutricion moderada', 2, 28.0, 39.1, 100, 25, 2, NULL, N'Curacion de heridas, desparasitacion',                  N'Heridas superficiales en tronco', '2025-03-15', N'Seguimiento de heridas',            0, 1, '2025-03-01'),
(4,  5,  '2025-03-20', 1, N'Evaluacion al ingreso',                                N'Buen estado general, sobrepeso leve',                    1, 40.0, 38.4, 95, 21, 8, 1,   N'Examen fisico, vacuna parvovirus',                      N'Sin anomalias',                   '2025-09-20', N'Refuerzo de vacuna',                0, 1, '2025-03-20'),
(5,  11, '2025-02-20', 7, N'Chequeo de rutina al ingreso',                        N'Salud general buena, pulgas',                             1, 17.5, 38.3, 88, 20, 1, 3,   N'Examen fisico, tratamiento antipulgas, vacuna rabia',   N'Normal',                          '2025-08-20', N'Refuerzo vacunas',                  0, 1, '2025-02-20'),
-- Consultas de seguimiento
(6,  1,  '2025-03-10', 3, N'Control de peso y parasitos',                          N'Mejora notable, peso recuperado',                         1, 26.0, 38.3, 88, 20, 1, NULL, N'Pesaje, examen de heces',                               N'Heces: negativo para parasitos',  NULL,          NULL,                                 0, 1, '2025-03-10'),
(7,  3,  '2025-03-15', 3, N'Seguimiento de heridas',                               N'Heridas en proceso de cicatrizacion',                     1, 29.5, 38.5, 92, 22, 7, NULL, N'Curacion y limpieza de heridas',                        N'Cicatrizacion favorable',         '2025-04-01', N'Revision final de heridas',         0, 1, '2025-03-15'),
-- Vacunaciones
(8,  13, '2025-03-15', 6, N'Vacunacion programada',                                N'Apto para vacunacion',                                    1, 24.0, 38.2, 86, 20, 3, 1,   N'Aplicacion de vacuna parvovirus',                       N'Sin reacciones adversas',         '2025-04-15', N'Segunda dosis',                     0, 1, '2025-03-15'),
(9,  14, '2025-04-01', 6, N'Vacunacion programada',                                N'Apta para vacunacion',                                    1, 3.2,  38.6, 140, 28, 6, 5,   N'Aplicacion de triple felina',                           N'Sin reacciones adversas',         '2025-05-01', N'Refuerzo triple felina',            0, 1, '2025-04-01'),
-- Emergencias
(10, 19, '2025-04-10', 2, N'Vomitos y diarrea persistente',                        N'Gastroenteritis aguda por ingesta de basura',             3, 18.5, 39.8, 110, 30, 2, NULL, N'Hidratacion IV, medicacion antivomitiva',               N'Deshidratacion moderada',         '2025-04-12', N'Control post-tratamiento',          0, 1, '2025-04-10'),
(11, 22, '2025-05-05', 2, N'Cojera en pata trasera derecha',                       N'Esguince leve en articulacion',                           2, 34.0, 38.7, 95, 22, 7, NULL, N'Radiografia, vendaje, antiinflamatorio',                N'Radiografia: sin fractura',       '2025-05-15', N'Control de movilidad',              0, 1, '2025-05-05'),
-- Pre-quirurgicos
(12, 8,  '2025-05-15', 4, N'Evaluacion pre-quirurgica para esterilizacion',        N'Apto para cirugia',                                       1, 14.5, 38.3, 86, 20, 7, NULL, N'Examen fisico, analisis de sangre pre-quirurgico',     N'Hemograma y quimica normal',      '2025-05-20', N'Cirugia de esterilizacion',         0, 1, '2025-05-15'),
-- Post-quirurgicos
(13, 8,  '2025-05-25', 5, N'Control post-esterilizacion',                          N'Recuperacion satisfactoria',                               1, 14.2, 38.4, 88, 21, 7, NULL, N'Revision de sutura, retiro de puntos',                  N'Herida quirurgica sana',          NULL,          NULL,                                 0, 1, '2025-05-25'),
-- Mas consultas generales
(14, 4,  '2025-04-10', 1, N'Estornudos frecuentes',                                N'Rinitis leve, posible alergia ambiental',                 1, 3.9,  38.5, 145, 30, 6, NULL, N'Examen fisico, limpieza nasal',                         N'Sin infeccion',                   '2025-05-10', N'Control si persisten sintomas',     0, 1, '2025-04-10'),
(15, 24, '2025-06-01', 7, N'Chequeo de rutina',                                    N'Buen estado general',                                     1, 21.5, 38.2, 88, 20, 1, 4,   N'Examen fisico completo, vacunacion polivalente canina', N'Todo normal',                     '2025-12-01', N'Refuerzo anual',                    0, 1, '2025-06-01');
SET IDENTITY_INSERT Medico.tbCitaMedica OFF;

-- =============================================
-- 5.15 RECETAS (8)
-- =============================================

PRINT '  Insertando Recetas...';
SET IDENTITY_INSERT Medico.tbRecetas ON;
INSERT INTO Medico.tbRecetas (receta_Id, cita_Id, masc_Id, receta_Medicamento, tipoMed_Id, viaAdmin_Id, receta_Dosis, receta_Frecuencia, receta_Duracion, receta_Instrucciones, receta_FechaInicio, receta_FechaFin, receta_Estado, receta_EsEliminado, receta_UsuarioCrea, receta_FechaCrea) VALUES
(1, 1,  1,  N'Fenbendazol',         2, 1, N'5 ml',    N'Cada 24 horas', N'3 dias',   N'Administrar con alimento. Repetir en 15 dias.',                  '2025-02-10', '2025-02-13', N'Completada',  0, 1, '2025-02-10'),
(2, 1,  1,  N'Complejo B inyectable', 5, 3, N'1 ml',  N'Cada 48 horas', N'5 dosis',  N'Inyeccion subcutanea. Monitorear apetito.',                      '2025-02-10', '2025-02-20', N'Completada',  0, 1, '2025-02-10'),
(3, 3,  3,  N'Amoxicilina',         1, 1, N'250 mg',  N'Cada 12 horas', N'7 dias',   N'Administrar despues de las comidas.',                             '2025-03-01', '2025-03-08', N'Completada',  0, 1, '2025-03-01'),
(4, 3,  3,  N'Meloxicam',           3, 1, N'0.1 mg/kg', N'Cada 24 horas', N'5 dias', N'Administrar con alimento para proteger el estomago.',             '2025-03-01', '2025-03-06', N'Completada',  0, 1, '2025-03-01'),
(5, 10, 19, N'Metoclopramida',      4, 3, N'0.5 mg/kg', N'Cada 8 horas', N'3 dias',  N'Inyeccion subcutanea antes de las comidas.',                      '2025-04-10', '2025-04-13', N'Completada',  0, 1, '2025-04-10'),
(6, 10, 19, N'Suero oral rehidratante', 6, 1, N'50 ml', N'Cada 4 horas', N'2 dias',  N'Administrar lentamente por via oral. Suspender si vomita.',       '2025-04-10', '2025-04-12', N'Completada',  0, 1, '2025-04-10'),
(7, 11, 22, N'Carprofeno',          3, 1, N'2.2 mg/kg', N'Cada 12 horas', N'7 dias', N'Administrar con alimento. No exceder la dosis indicada.',         '2025-05-05', '2025-05-12', N'Completada',  0, 1, '2025-05-05'),
(8, 14, 4,  N'Clorfenamina',        3, 1, N'0.5 ml',  N'Cada 12 horas', N'5 dias',   N'Antihistaminico oral. Observar somnolencia.',                     '2025-04-10', '2025-04-15', N'Activa',      0, 1, '2025-04-10');
SET IDENTITY_INSERT Medico.tbRecetas OFF;

-- =============================================
-- 5.16 TRATAMIENTOS (8)
-- =============================================

PRINT '  Insertando Tratamientos...';
SET IDENTITY_INSERT Medico.tbTratamientos ON;
INSERT INTO Medico.tbTratamientos (trat_Id, masc_Id, cita_Id, receta_Id, tipoPar_Id, trat_ParasitoDetectado, trat_Medicamento, tipoMed_Id, viaAdmin_Id, trat_FechaAplicacion, trat_AplicadoPor, trat_ProximaDosis, trat_Estado, trat_Observaciones, trat_EsEliminado, trat_UsuarioCrea, trat_FechaCrea) VALUES
(1, 1,  1,  1,  4,   N'Ascaris',          N'Fenbendazol',           2, 1, '2025-02-10', N'Dra. Maria Rodriguez', '2025-02-25', N'Completado',  N'Primera desparasitacion. Repetir en 15 dias.',                         0, 1, '2025-02-10'),
(2, 1,  1,  NULL, 4, N'Ascaris',          N'Fenbendazol',           2, 1, '2025-02-25', N'Dra. Maria Rodriguez', NULL,          N'Completado',  N'Segunda desparasitacion. Examen de heces negativo.',                   0, 1, '2025-02-25'),
(3, 5,  4,  NULL, 1, N'Pulgas',           N'Fipronil topico',       8, 5, '2025-03-20', N'Dra. Maria Rodriguez', '2025-04-20', N'Completado',  N'Tratamiento antipulgas topico. Bano previo realizado.',                0, 1, '2025-03-20'),
(4, 11, 5,  NULL, 1, N'Pulgas',           N'Fipronil topico',       8, 5, '2025-02-20', N'Dr. Roberto Zuniga',  '2025-03-20', N'Completado',  N'Tratamiento antipulgas. Animal cooperador.',                           0, 1, '2025-02-20'),
(5, 3,  3,  3,  NULL, NULL,                N'Amoxicilina',           1, 1, '2025-03-01', N'Dra. Maria Rodriguez', NULL,          N'Completado',  N'Tratamiento antibiotico por heridas infectadas. Buena evolucion.',     0, 1, '2025-03-01'),
(6, 19, 10, 5,  NULL, NULL,                N'Metoclopramida',        4, 3, '2025-04-10', N'Dr. Roberto Zuniga',  NULL,          N'Completado',  N'Tratamiento de emergencia por gastroenteritis. Respuesta favorable.', 0, 1, '2025-04-10'),
(7, 22, 11, 7,  NULL, NULL,                N'Carprofeno',            3, 1, '2025-05-05', N'Dra. Carmen Padilla',  '2025-05-15', N'En progreso', N'Tratamiento antiinflamatorio por esguince. Reposo recomendado.',       0, 1, '2025-05-05'),
(8, 8,  12, NULL, NULL, NULL,              N'Meloxicam post-quirurgico', 3, 1, '2025-05-20', N'Dr. Roberto Zuniga', NULL,       N'Completado',  N'Manejo del dolor post-esterilizacion. Sin complicaciones.',            0, 1, '2025-05-20');
SET IDENTITY_INSERT Medico.tbTratamientos OFF;

-- =============================================
-- FIN DE INSERCIONES
-- =============================================

COMMIT TRANSACTION;

PRINT '';
PRINT '========================================';
PRINT 'SCRIPT COMPLETADO EXITOSAMENTE';
PRINT '========================================';
PRINT '';

-- Verificacion de conteos
SELECT 'Refugio.tbProcedencias' AS Tabla, COUNT(*) AS Registros FROM Refugio.tbProcedencias UNION ALL
SELECT 'Refugio.tbComportamientos', COUNT(*) FROM Refugio.tbComportamientos UNION ALL
SELECT 'Refugio.tbEmpleadosCargos', COUNT(*) FROM Refugio.tbEmpleadosCargos UNION ALL
SELECT 'Refugio.tbRazas', COUNT(*) FROM Refugio.tbRazas UNION ALL
SELECT 'Refugio.tbVacunas', COUNT(*) FROM Refugio.tbVacunas UNION ALL
SELECT 'Refugio.tbRefugios', COUNT(*) FROM Refugio.tbRefugios UNION ALL
SELECT 'General.tbPersonas', COUNT(*) FROM General.tbPersonas UNION ALL
SELECT 'Refugio.tbEmpleados', COUNT(*) FROM Refugio.tbEmpleados UNION ALL
SELECT 'Refugio.tbVoluntarios', COUNT(*) FROM Refugio.tbVoluntarios UNION ALL
SELECT 'Inventario.tbCategorias', COUNT(*) FROM Inventario.tbCategorias UNION ALL
SELECT 'Inventario.tbItems', COUNT(*) FROM Inventario.tbItems UNION ALL
SELECT 'Refugio.tbMascotas', COUNT(*) FROM Refugio.tbMascotas UNION ALL
SELECT 'Refugio.tbSolicitudes', COUNT(*) FROM Refugio.tbSolicitudes UNION ALL
SELECT 'Refugio.tbAdopciones', COUNT(*) FROM Refugio.tbAdopciones UNION ALL
SELECT 'Refugio.tbEventos', COUNT(*) FROM Refugio.tbEventos UNION ALL
SELECT 'Refugio.tbEventos_tbVoluntarios', COUNT(*) FROM Refugio.tbEventos_tbVoluntarios UNION ALL
SELECT 'Inventario.tbRecepcionesMercancia', COUNT(*) FROM Inventario.tbRecepcionesMercancia UNION ALL
SELECT 'Inventario.tbRecepcionesDetalles', COUNT(*) FROM Inventario.tbRecepcionesDetalles UNION ALL
SELECT 'Medico.tbGravedades', COUNT(*) FROM Medico.tbGravedades UNION ALL
SELECT 'Medico.tbTiposConsulta', COUNT(*) FROM Medico.tbTiposConsulta UNION ALL
SELECT 'Medico.tbTiposMedicamento', COUNT(*) FROM Medico.tbTiposMedicamento UNION ALL
SELECT 'Medico.tbViasAdministracion', COUNT(*) FROM Medico.tbViasAdministracion UNION ALL
SELECT 'Medico.tbTiposParasito', COUNT(*) FROM Medico.tbTiposParasito UNION ALL
SELECT 'Medico.tbTiposEsterilizacion', COUNT(*) FROM Medico.tbTiposEsterilizacion UNION ALL
SELECT 'Medico.tbCitaMedica', COUNT(*) FROM Medico.tbCitaMedica UNION ALL
SELECT 'Medico.tbRecetas', COUNT(*) FROM Medico.tbRecetas UNION ALL
SELECT 'Medico.tbTratamientos', COUNT(*) FROM Medico.tbTratamientos
ORDER BY Tabla;

GO
