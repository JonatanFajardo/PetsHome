-- An\u00e1lisis detallado de base de datos PETSHOMEDB para sistema de login

USE PETSHOMEDB;
GO

-- 1. Verificar estructura completa de tbUsuarios
PRINT '===== TABLA tbUsuarios ====='
SELECT
    c.COLUMN_NAME AS 'Campo',
    c.DATA_TYPE AS 'Tipo de Dato',
    c.CHARACTER_MAXIMUM_LENGTH AS 'Longitud M\u00e1xima',
    c.IS_NULLABLE AS 'Permite NULL',
    c.COLUMN_DEFAULT AS 'Valor por Defecto'
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'tbUsuarios'
ORDER BY c.ORDINAL_POSITION;
GO

-- 2. Verificar si existe tabla de contrase\u00f1as o similar
PRINT '===== TABLAS RELACIONADAS CON SEGURIDAD ====='
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'Seguridad'
ORDER BY TABLE_NAME;
GO

-- 3. Buscar procedimientos almacenados de Usuarios
PRINT '===== PROCEDIMIENTOS ALMACENADOS EN SCHEMA SEGURIDAD ====='
SELECT
    ROUTINE_NAME AS 'Procedimiento',
    CREATED AS 'Fecha Creaci\u00f3n',
    LAST_ALTERED AS '\u00daltima Modificaci\u00f3n'
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
    AND ROUTINE_SCHEMA = 'Seguridad'
ORDER BY ROUTINE_NAME;
GO

-- 4. Buscar procedimientos que contengan 'Usuario' o 'Login' en el nombre
PRINT '===== PROCEDIMIENTOS RELACIONADOS CON USUARIOS/LOGIN ====='
SELECT
    ROUTINE_SCHEMA AS 'Esquema',
    ROUTINE_NAME AS 'Procedimiento'
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
    AND (ROUTINE_NAME LIKE '%Usuario%'
         OR ROUTINE_NAME LIKE '%Login%'
         OR ROUTINE_NAME LIKE '%Auth%'
         OR ROUTINE_NAME LIKE '%Contrasena%')
ORDER BY ROUTINE_SCHEMA, ROUTINE_NAME;
GO

-- 5. Verificar estructura de tbEmpleados
PRINT '===== TABLA tbEmpleados ====='
SELECT
    c.COLUMN_NAME AS 'Campo',
    c.DATA_TYPE AS 'Tipo de Dato',
    c.IS_NULLABLE AS 'Permite NULL'
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'tbEmpleados'
ORDER BY c.ORDINAL_POSITION;
GO

-- 6. Verificar relaciones entre tbUsuarios y tbEmpleados
PRINT '===== RELACIONES ENTRE TABLAS ====='
SELECT
    fk.name AS 'Nombre FK',
    OBJECT_NAME(fk.parent_object_id) AS 'Tabla Origen',
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS 'Campo Origen',
    OBJECT_NAME(fk.referenced_object_id) AS 'Tabla Referenciada',
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS 'Campo Referenciado'
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fc
    ON fk.object_id = fc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'tbUsuarios'
   OR OBJECT_NAME(fk.referenced_object_id) = 'tbUsuarios';
GO

-- 7. Verificar si existe tabla de Roles
PRINT '===== TABLA DE ROLES ====='
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME LIKE '%Rol%'
ORDER BY TABLE_NAME;
GO

-- 8. Contar registros en tbUsuarios
PRINT '===== CANTIDAD DE USUARIOS REGISTRADOS ====='
SELECT COUNT(*) AS 'Total Usuarios' FROM [Seguridad].[tbUsuarios];
GO

-- 9. Muestra de datos de tbUsuarios (sin contrase\u00f1as)
PRINT '===== MUESTRA DE DATOS tbUsuarios ====='
SELECT TOP 5
    usu_Id,
    Emp_Id,
    Usu_Nombre,
    Rol_Id,
    Usu_EsActivo,
    Usu_Suspendido,
    Usu_FechaCreacion
FROM [Seguridad].[tbUsuarios];
GO

-- 10. Verificar todos los schemas existentes
PRINT '===== SCHEMAS DISPONIBLES EN LA BASE DE DATOS ====='
SELECT
    name AS 'Nombre Schema'
FROM sys.schemas
WHERE name NOT IN ('db_owner', 'db_accessadmin', 'db_securityadmin',
                    'db_ddladmin', 'db_backupoperator', 'db_datareader',
                    'db_datawriter', 'db_denydatareader', 'db_denydatawriter',
                    'sys', 'INFORMATION_SCHEMA', 'guest')
ORDER BY name;
GO
