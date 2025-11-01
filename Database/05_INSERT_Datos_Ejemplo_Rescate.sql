-- =============================================
-- Script: Inserción de Datos de Ejemplo para Módulo Rescate
-- Descripción: Inserta 5 registros en cada tabla del módulo de Rescate/Ingreso
-- Fecha: 2025-10-29
-- =============================================

USE PETSHOMEDB
GO

-- =============================================
-- PASO 1: Insertar Reportes de Abandono
-- =============================================
PRINT 'Insertando reportes de abandono...'

-- Reporte 1: Cachorro abandonado en parque
INSERT INTO [Rescate].[tbReportesAbandono]
(
    reptip_Id,
    repa_NombreReportante,
    repa_Telefono,
    repa_Email,
    repa_FechaReporte,
    repa_Lugar,
    repa_DescripcionAnimal,
    repa_EstadoAtencion,
    repa_Observaciones,
    repa_EsAnonimo,
    refg_Id,
    repa_UsuarioCrea,
    repa_FechaCrea,
    repa_EsEliminado
)
VALUES
(
    1, -- Vecino del sector
    'María González',
    '9876-5432',
    'maria.gonzalez@email.com',
    '2025-01-15 09:30:00',
    'Parque Central, frente a la cancha de fútbol',
    'Cachorro mestizo, aproximadamente 2 meses, color café con manchas blancas. Desnutrido, con señales de sarna, muy asustado',
    'Atendido',
    'El animal fue rescatado el mismo día. Se encuentra en tratamiento veterinario.',
    0,
    1,
    1,
    GETDATE(),
    0
)

-- Reporte 2: Gato herido en calle
INSERT INTO [Rescate].[tbReportesAbandono]
(
    reptip_Id,
    repa_NombreReportante,
    repa_Telefono,
    repa_Email,
    repa_FechaReporte,
    repa_Lugar,
    repa_DescripcionAnimal,
    repa_EstadoAtencion,
    repa_Observaciones,
    repa_EsAnonimo,
    refg_Id,
    repa_UsuarioCrea,
    repa_FechaCrea,
    repa_EsEliminado
)
VALUES
(
    2, -- Transeúnte
    'Carlos Ramírez',
    '3345-6789',
    'carlos.ramirez@email.com',
    '2025-01-20 14:15:00',
    'Avenida Principal, entre 3ra y 4ta calle',
    'Gato adulto, color naranja, con collar rojo. Herida en pata trasera derecha, posible atropello',
    'En Proceso',
    'Urgente: necesita atención veterinaria inmediata. Posible fractura.',
    0,
    2,
    1,
    GETDATE(),
    0
)

-- Reporte 3: Perros abandonados en terreno baldío
INSERT INTO [Rescate].[tbReportesAbandono]
(
    reptip_Id,
    repa_NombreReportante,
    repa_Telefono,
    repa_Email,
    repa_FechaReporte,
    repa_Lugar,
    repa_DescripcionAnimal,
    repa_EstadoAtencion,
    repa_Observaciones,
    repa_EsAnonimo,
    refg_Id,
    repa_UsuarioCrea,
    repa_FechaCrea,
    repa_EsEliminado
)
VALUES
(
    3, -- Personal de seguridad
    'Jorge Martínez',
    '2234-5678',
    'jorge.martinez@security.com',
    '2025-02-05 08:00:00',
    'Terreno baldío en Colonia Las Palmas, al lado del supermercado',
    'Tres perros mestizos adultos, uno negro, uno blanco y uno café. En buen estado general, pero sin alimento ni agua',
    'Pendiente',
    'Los perros parecen ser de la misma familia. Están en un terreno cercado.',
    0,
    1,
    1,
    GETDATE(),
    0
)

-- Reporte 4: Cachorro en caja de cartón
INSERT INTO [Rescate].[tbReportesAbandono]
(
    reptip_Id,
    repa_NombreReportante,
    repa_TelefonoContacto,
    repa_FechaReporte,
    repa_UbicacionIncidente,
    repa_DescripcionAnimal,
    repa_EstadoAnimal,
    repa_EstadoAtencion,
    repa_Observaciones,
    refg_Id,
    repa_UsuarioCrea,
    repa_FechaCrea,
    repa_EsEliminado
)
VALUES
(
    4, -- Estudiante
    'Ana Rodríguez',
    '9988-7766',
    '2025-02-10 16:45:00',
    'Puerta del Instituto Técnico, Barrio El Centro',
    'Cachorro muy pequeño, aproximadamente 3-4 semanas, color negro',
    'Hipotérmico, muy débil, apenas se mueve',
    'Atendido',
    'Rescatado de emergencia. Requirió alimentación con biberón y calor.',
    2,
    1,
    GETDATE(),
    0
)

-- Reporte 5: Perro atado sin comida ni agua
INSERT INTO [Rescate].[tbReportesAbandono]
(
    reptip_Id,
    repa_NombreReportante,
    repa_TelefonoContacto,
    repa_FechaReporte,
    repa_UbicacionIncidente,
    repa_DescripcionAnimal,
    repa_EstadoAnimal,
    repa_EstadoAtencion,
    repa_Observaciones,
    refg_Id,
    repa_UsuarioCrea,
    repa_FechaCrea,
    repa_EsEliminado
)
VALUES
(
    1, -- Vecino del sector
    'Pedro López',
    '8877-6655',
    '2025-02-18 10:30:00',
    'Casa abandonada en Residencial Los Pinos, casa #45',
    'Perro mediano, raza indefinida, color blanco con marrón',
    'Desnutrido severo, deshidratado, con cadena muy corta al cuello',
    'Rechazado',
    'Las autoridades locales se hicieron cargo del caso por maltrato animal.',
    NULL,
    1,
    GETDATE(),
    0
)

PRINT 'Reportes de abandono insertados correctamente.'
GO

-- =============================================
-- PASO 2: Insertar Ingresos
-- =============================================
PRINT 'Insertando ingresos...'

-- Ingreso 1: Vinculado al reporte 1 (cachorro del parque)
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
    ingr_FechaCrea,
    ingr_EsEliminado
)
VALUES
(
    1, -- Vinculado al reporte del cachorro en el parque
    1,
    '2025-01-15 11:00:00',
    'Parque Central, frente a la cancha de fútbol',
    'Cachorro desnutrido con sarna, temperatura corporal baja. Peso: 1.5 kg. Se observan costillas prominentes.',
    'Dr. Roberto Sánchez',
    'Vehículo particular del voluntario',
    'Ingresó con protocolo de emergencia. Se inició tratamiento antiparasitario y baño medicado.',
    1,
    1,
    GETDATE(),
    0
)

-- Ingreso 2: Vinculado al reporte 2 (gato herido)
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
    ingr_FechaCrea,
    ingr_EsEliminado
)
VALUES
(
    2, -- Vinculado al reporte del gato herido
    2,
    '2025-01-20 15:30:00',
    'Avenida Principal, entre 3ra y 4ta calle',
    'Fractura expuesta en pata trasera derecha. En shock. Respiración acelerada. Requiere cirugía urgente.',
    'Dra. Patricia Morales',
    'Ambulancia veterinaria del refugio',
    'Cirugía realizada el mismo día. Pronóstico reservado. En observación 24h.',
    1,
    1,
    GETDATE(),
    0
)

-- Ingreso 3: Vinculado al reporte 4 (cachorro en caja)
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
    ingr_FechaCrea,
    ingr_EsEliminado
)
VALUES
(
    4, -- Vinculado al cachorro en caja de cartón
    2,
    '2025-02-10 17:15:00',
    'Puerta del Instituto Técnico, Barrio El Centro',
    'Cachorro neonato, hipotérmico (35°C), muy débil. Peso: 400g. Cordón umbilical aún presente.',
    'Voluntaria Elena Castro',
    'Vehículo particular',
    'Requirió cuidados intensivos: incubadora, alimentación cada 2 horas con fórmula especial.',
    1,
    1,
    GETDATE(),
    0
)

-- Ingreso 4: Rescate sin reporte previo (perro callejero)
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
    ingr_FechaCrea,
    ingr_EsEliminado
)
VALUES
(
    NULL, -- Sin reporte previo
    1,
    '2025-02-15 08:45:00',
    'Mercado Municipal, zona de carga y descarga',
    'Perro adulto, aproximadamente 3 años. Buen estado general. Sin collar. Sociable y dócil.',
    'Equipo de rescate del refugio',
    'Camioneta del refugio',
    'Capturado con red de forma segura. Se comportó tranquilo durante el traslado.',
    0,
    1,
    GETDATE(),
    0
)

-- Ingreso 5: Rescate de emergencia en carretera
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
    ingr_FechaCrea,
    ingr_EsEliminado
)
VALUES
(
    NULL, -- Sin reporte previo
    1,
    '2025-02-22 19:30:00',
    'Carretera CA-5, kilómetro 28, cerca del desvío a La Lima',
    'Perra preñada, en trabajo de parto, atropellada. Heridas superficiales. Estrés extremo.',
    'Dr. Fernando Ulloa',
    'Ambulancia veterinaria',
    'Emergencia nocturna. Cesárea de emergencia. Nacieron 4 cachorros, 3 sobrevivieron.',
    1,
    1,
    GETDATE(),
    0
)

PRINT 'Ingresos insertados correctamente.'
GO

-- =============================================
-- PASO 3: Vincular algunos Ingresos con Mascotas existentes
-- =============================================
PRINT 'Vinculando ingresos con mascotas existentes...'

-- Vincular Ingreso 1 con Mascota 49
UPDATE [Refugio].[tbMascotas]
SET masc_IngresoId = 1
WHERE masc_Id = 49

-- Vincular Ingreso 2 con Mascota 50
UPDATE [Refugio].[tbMascotas]
SET masc_IngresoId = 2
WHERE masc_Id = 50

-- Vincular Ingreso 3 con Mascota 51
UPDATE [Refugio].[tbMascotas]
SET masc_IngresoId = 3
WHERE masc_Id = 51

-- Vincular Ingreso 4 con Mascota 52
UPDATE [Refugio].[tbMascotas]
SET masc_IngresoId = 4
WHERE masc_Id = 52

-- Vincular Ingreso 5 con Mascota 53
UPDATE [Refugio].[tbMascotas]
SET masc_IngresoId = 5
WHERE masc_Id = 53

-- La Mascota 54 queda sin ingreso asociado (flujo directo de creación)

PRINT 'Mascotas vinculadas con ingresos correctamente.'
GO

-- =============================================
-- VERIFICACIÓN DE DATOS
-- =============================================
PRINT '=========================================='
PRINT 'VERIFICACIÓN DE DATOS INSERTADOS'
PRINT '=========================================='

-- Contar reportes
PRINT 'Total de Reportes de Abandono: ' + CAST((SELECT COUNT(*) FROM [Rescate].[tbReportesAbandono]) AS VARCHAR(10))

-- Contar ingresos
PRINT 'Total de Ingresos: ' + CAST((SELECT COUNT(*) FROM [Rescate].[tbIngresos]) AS VARCHAR(10))

-- Contar mascotas vinculadas
PRINT 'Total de Mascotas con Ingreso: ' + CAST((SELECT COUNT(*) FROM [Refugio].[tbMascotas] WHERE masc_IngresoId IS NOT NULL) AS VARCHAR(10))

PRINT '=========================================='
PRINT 'RESUMEN POR ESTADO'
PRINT '=========================================='

-- Reportes por estado
SELECT
    repa_EstadoAtencion AS [Estado],
    COUNT(*) AS [Cantidad]
FROM [Rescate].[tbReportesAbandono]
WHERE repa_EsEliminado = 0
GROUP BY repa_EstadoAtencion

-- Ingresos por emergencia
SELECT
    CASE WHEN ingr_EsEmergencia = 1 THEN 'Emergencia' ELSE 'Normal' END AS [Tipo],
    COUNT(*) AS [Cantidad]
FROM [Rescate].[tbIngresos]
WHERE ingr_EsEliminado = 0
GROUP BY ingr_EsEmergencia

-- Ingresos con y sin reporte
SELECT
    CASE WHEN repa_Id IS NULL THEN 'Sin Reporte' ELSE 'Con Reporte' END AS [Origen],
    COUNT(*) AS [Cantidad]
FROM [Rescate].[tbIngresos]
WHERE ingr_EsEliminado = 0
GROUP BY CASE WHEN repa_Id IS NULL THEN 'Sin Reporte' ELSE 'Con Reporte' END

PRINT '=========================================='
PRINT 'Script completado exitosamente.'
PRINT '=========================================='
GO
