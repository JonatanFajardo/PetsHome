-- Obtener definici\u00f3n de procedimientos almacenados clave para login

USE PETSHOMEDB;
GO

-- 1. Procedimiento de Login principal
PRINT '===== UDP_Acce_tbUsuarios_Login ====='
EXEC sp_helptext 'Seguridad.UDP_Acce_tbUsuarios_Login';
GO

PRINT ''
PRINT '===== PR_Seguridad_Usuarios_Login ====='
EXEC sp_helptext 'Seguridad.PR_Seguridad_Usuarios_Login';
GO

PRINT ''
PRINT '===== UDP_Acce_tbUsuarios_LoginIn ====='
EXEC sp_helptext 'Seguridad.UDP_Acce_tbUsuarios_LoginIn';
GO

PRINT ''
PRINT '===== UDP_Acce_tbUsuarios_Logout ====='
EXEC sp_helptext 'Seguridad.UDP_Acce_tbUsuarios_Logout';
GO

PRINT ''
PRINT '===== ESTRUCTURA DE tbRoles ====='
SELECT
    c.COLUMN_NAME AS 'Campo',
    c.DATA_TYPE AS 'Tipo de Dato',
    c.IS_NULLABLE AS 'Permite NULL'
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'tbRoles'
ORDER BY c.ORDINAL_POSITION;
GO

PRINT ''
PRINT '===== DATOS DE tbRoles ====='
SELECT * FROM [Seguridad].[tbRoles];
GO
