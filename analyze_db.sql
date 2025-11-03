-- Script para analizar base de datos PETSHOMEDB para sistema de login

-- 1. Listar todas las tablas
SELECT
    TABLE_SCHEMA,
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- 2. Verificar estructura de tabla Empleado (candidata para login)
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Empleado'
ORDER BY ORDINAL_POSITION;

-- 3. Buscar tablas relacionadas con usuarios/autenticación
SELECT
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
    AND (TABLE_NAME LIKE '%User%'
         OR TABLE_NAME LIKE '%Usuario%'
         OR TABLE_NAME LIKE '%Login%'
         OR TABLE_NAME LIKE '%Auth%'
         OR TABLE_NAME LIKE '%Empleado%')
ORDER BY TABLE_NAME;

-- 4. Listar todos los procedimientos almacenados
SELECT
    ROUTINE_SCHEMA,
    ROUTINE_NAME,
    ROUTINE_TYPE,
    CREATED,
    LAST_ALTERED
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
ORDER BY ROUTINE_NAME;

-- 5. Buscar procedimientos relacionados con login/autenticación
SELECT
    ROUTINE_NAME
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
    AND (ROUTINE_NAME LIKE '%Login%'
         OR ROUTINE_NAME LIKE '%Auth%'
         OR ROUTINE_NAME LIKE '%Usuario%'
         OR ROUTINE_NAME LIKE '%User%'
         OR ROUTINE_NAME LIKE '%Empleado%')
ORDER BY ROUTINE_NAME;

-- 6. Ver campos de todas las tablas para identificar campos de autenticación
SELECT
    t.TABLE_NAME,
    c.COLUMN_NAME,
    c.DATA_TYPE
FROM INFORMATION_SCHEMA.TABLES t
JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_TYPE = 'BASE TABLE'
    AND (c.COLUMN_NAME LIKE '%Password%'
         OR c.COLUMN_NAME LIKE '%Contrasena%'
         OR c.COLUMN_NAME LIKE '%Clave%'
         OR c.COLUMN_NAME LIKE '%Email%'
         OR c.COLUMN_NAME LIKE '%Username%'
         OR c.COLUMN_NAME LIKE '%Usuario%')
ORDER BY t.TABLE_NAME, c.COLUMN_NAME;
