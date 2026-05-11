-- Script para verificar el login y generar hash de prueba

USE PETSHOMEDB;
GO

-- 1. Verificar datos del usuario admin
PRINT '===== DATOS DEL USUARIO ADMIN =====';
SELECT
    usu_Id,
    Emp_Id,
    Usu_Nombre,
    Usu_PasswordHash,
    Usu_PasswordSalt,
    Rol_Id,
    Usu_EsActivo,
    Usu_Suspendido,
    Usu_EsEliminado
FROM [Seguridad].[tbUsuarios]
WHERE Usu_Nombre = 'admin';
GO

-- 2. Verificar empleado asociado
PRINT '';
PRINT '===== EMPLEADO ASOCIADO =====';
SELECT
    e.emp_Id,
    e.per_Id,
    e.emp_EsActivo,
    CONCAT(p.per_PrimerNombre, ' ', p.per_ApellidoPaterno) AS NombreCompleto
FROM [Seguridad].[tbUsuarios] u
INNER JOIN [Refugio].[tbEmpleados] e ON u.Emp_Id = e.emp_Id
INNER JOIN [General].[tbPersonas] p ON e.per_Id = p.per_Id
WHERE u.Usu_Nombre = 'admin';
GO

-- 3. Verificar rol asociado
PRINT '';
PRINT '===== ROL ASIGNADO =====';
SELECT
    r.Rol_Id,
    r.Rol_Descripcion,
    r.Rol_EsActivo
FROM [Seguridad].[tbUsuarios] u
INNER JOIN [Seguridad].[tbRoles] r ON u.Rol_Id = r.Rol_Id
WHERE u.Usu_Nombre = 'admin';
GO

-- 4. Probar el procedimiento almacenado directamente
PRINT '';
PRINT '===== PRUEBA DE PROCEDIMIENTO ALMACENADO =====';
PRINT 'Probando con hash de "admin123"...';
EXEC [Seguridad].[UDP_Acce_tbUsuarios_Login]
    @usu_NombreUsuario = 'admin',
    @contrasena = '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9';
GO

-- 5. Generar hashes de prueba comunes
PRINT '';
PRINT '===== HASHES DE CONTRASEÑAS COMUNES (SHA256 - MAYÚSCULAS) =====';
PRINT 'admin123 = 240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9';
PRINT 'admin    = 8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918';
PRINT 'password = 5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8';
PRINT '12345678 = EF797C8118F02DFB649607DD5D3F8C7623048C9C063D532CC95C5ED7A898A64F';
PRINT '';
PRINT '===== HASHES DE CONTRASEÑAS COMUNES (SHA256 - MINÚSCULAS) =====';
PRINT 'admin123 = 240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9';
PRINT 'admin    = 8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918';
PRINT 'password = 5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8';
PRINT '12345678 = ef797c8118f02dfb649607dd5d3f8c7623048c9c063d532cc95c5ed7a898a64f';
GO
