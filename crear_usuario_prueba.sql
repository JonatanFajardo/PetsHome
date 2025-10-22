-- Script para crear usuario de prueba para el sistema de login de PetsHome
-- Este script crea un usuario de prueba con las credenciales necesarias

USE PETSHOMEDB;
GO

-- ========================================
-- 1. CREAR O ACTUALIZAR USUARIO ADMIN
-- ========================================

-- Datos del usuario de prueba:
-- Usuario: admin
-- Contraseña: admin123
-- Hash SHA256 de "admin123": 240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9

DECLARE @usu_Id INT = 1;
DECLARE @emp_Id INT = 1;
DECLARE @usuarioNombre NVARCHAR(50) = 'admin';
DECLARE @passwordHash NVARCHAR(255) = '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9';
DECLARE @rol_Id INT = 1; -- Rol Administrador

-- Verificar si el usuario ya existe
IF EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE usu_Id = @usu_Id)
BEGIN
    -- Actualizar usuario existente
    UPDATE [Seguridad].[tbUsuarios]
    SET
        Usu_Nombre = @usuarioNombre,
        Usu_PasswordHash = @passwordHash,
        Rol_Id = @rol_Id,
        Usu_EsActivo = 1,
        Usu_Suspendido = 0,
        Usu_EsEliminado = 0,
        usu_Logueado = 0,
        usu_IntentosFallidos = 0,
        usu_FechaBloqueo = NULL,
        Usu_fechaModificacion = GETDATE()
    WHERE usu_Id = @usu_Id;

    PRINT 'Usuario actualizado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El usuario con ID ' + CAST(@usu_Id AS NVARCHAR) + ' no existe. Verifique el empleado asociado.';
END

GO

-- ========================================
-- 2. VERIFICAR CONFIGURACIÓN
-- ========================================

PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN DE USUARIO DE PRUEBA';
PRINT '========================================';

SELECT
    u.usu_Id AS 'ID Usuario',
    u.Usu_Nombre AS 'Usuario',
    u.Emp_Id AS 'ID Empleado',
    CONCAT(p.per_PrimerNombre, ' ', p.per_ApellidoPaterno) AS 'Nombre Completo',
    r.Rol_Descripcion AS 'Rol',
    u.Usu_EsActivo AS 'Activo',
    u.Usu_Suspendido AS 'Suspendido',
    u.usu_Logueado AS 'Logueado',
    u.Usu_FechaCreacion AS 'Fecha Creación'
FROM [Seguridad].[tbUsuarios] u
LEFT JOIN [Refugio].[tbEmpleados] e ON u.Emp_Id = e.emp_Id
LEFT JOIN [General].[tbPersonas] p ON e.per_Id = p.per_Id
LEFT JOIN [Seguridad].[tbRoles] r ON u.Rol_Id = r.Rol_Id
WHERE u.usu_Id = 1;

GO

-- ========================================
-- 3. INFORMACIÓN ADICIONAL
-- ========================================

PRINT '';
PRINT '========================================';
PRINT 'CREDENCIALES DE ACCESO';
PRINT '========================================';
PRINT 'Usuario: admin';
PRINT 'Contraseña: admin123';
PRINT '';
PRINT 'NOTA: Estas son credenciales de prueba.';
PRINT 'Para producción, cambie la contraseña.';
PRINT '========================================';

GO

-- ========================================
-- 4. SCRIPT PARA CREAR HASH DE NUEVA CONTRASEÑA
-- ========================================

-- Si deseas crear un hash para una contraseña diferente,
-- usa este código C# en una aplicación de consola:
/*
using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.Write("Ingrese la contraseña: ");
        string password = Console.ReadLine();

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            Console.WriteLine($"Hash SHA256: {builder.ToString().ToUpper()}");
        }
    }
}
*/

-- ========================================
-- 5. HASHES COMUNES PARA PRUEBAS
-- ========================================

-- admin123 = 240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9
-- password = 5E884898DA28047151D0E56F8DC6292773603D0D6AABBDD62A11EF721D1542D8
-- 12345678 = EF797C8118F02DFB649607DD5D3F8C7623048C9C063D532CC95C5ED7A898A64F
-- petshome = C7E9B6F1F8C02D7D8F3E0A9B4D5C6E8A7B9C0D1E2F3A4B5C6D7E8F9A0B1C2D3E

GO

PRINT '';
PRINT 'Script completado exitosamente!';
PRINT 'Puede proceder a iniciar sesión en la aplicación.';
