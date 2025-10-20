-- Script para agregar nuevos campos a la tabla Refugio.tbRazas
-- Fecha: 2025-10-19
-- Descripción: Agregar campos de Tamaño, Tipo de Animal, Tipo de Pelaje y URL de Imagen

USE PETSHOMEDB
GO

-- Agregar campo Tamaño (pequeño, mediano, grande, gigante)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Refugio].[tbRazas]') AND name = 'raza_Tamano')
BEGIN
    ALTER TABLE [Refugio].[tbRazas]
    ADD raza_Tamano VARCHAR(20) NULL
END
GO

-- Agregar campo Tipo de Animal (perro, gato, etc.)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Refugio].[tbRazas]') AND name = 'raza_TipoAnimal')
BEGIN
    ALTER TABLE [Refugio].[tbRazas]
    ADD raza_TipoAnimal VARCHAR(50) NULL
END
GO

-- Agregar campo Tipo de Pelaje (corto, largo, rizado, sin pelo)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Refugio].[tbRazas]') AND name = 'raza_TipoPelaje')
BEGIN
    ALTER TABLE [Refugio].[tbRazas]
    ADD raza_TipoPelaje VARCHAR(30) NULL
END
GO

-- Agregar campo URL de Imagen
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Refugio].[tbRazas]') AND name = 'raza_ImagenUrl')
BEGIN
    ALTER TABLE [Refugio].[tbRazas]
    ADD raza_ImagenUrl VARCHAR(500) NULL
END
GO

-- Verificar que los campos se agregaron correctamente
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'Refugio'
    AND TABLE_NAME = 'tbRazas'
    AND COLUMN_NAME IN ('raza_Tamano', 'raza_TipoAnimal', 'raza_TipoPelaje', 'raza_ImagenUrl')
ORDER BY ORDINAL_POSITION
GO

PRINT 'Los campos se agregaron correctamente a la tabla Refugio.tbRazas'
GO
