-- =============================================
-- Script de Datos de Prueba para Módulo de Seguridad
-- Sistema: PetsHome
-- Descripción: Crear datos básicos para probar el sistema de menús
-- =============================================

USE [petshome-7-24-2025]
GO

-- Verificar si existe un usuario de prueba
DECLARE @usu_Id INT;
SELECT @usu_Id = usu_Id FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = 'admin' OR Usu_Nombre LIKE '%admin%';

-- Si no existe, crear un usuario admin básico
IF @usu_Id IS NULL
BEGIN
    -- Buscar un empleado para usar como base
    DECLARE @emp_Id INT;
    SELECT TOP 1 @emp_Id = Emp_Id FROM [General].[tbEmpleados];
    
    IF @emp_Id IS NOT NULL
    BEGIN
        -- Obtener rol de administrador
        DECLARE @rol_Id INT;
        SELECT @rol_Id = Rol_Id FROM [Seguridad].[tbRoles] WHERE Rol_Descripcion = 'Administrador';
        
        -- Crear usuario admin si no existe
        INSERT INTO [Seguridad].[tbUsuarios] (
            Emp_Id, 
            Rol_Id, 
            Usu_Nombre, 
            Usu_Contrasena, 
            Usu_FechaCreacion, 
            Usu_UsuCreacion, 
            Usu_Estado
        )
        VALUES (
            @emp_Id,
            @rol_Id,
            'admin',
            '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', -- hash de 'admin123'
            GETDATE(),
            1,
            1
        );
        
        SET @usu_Id = SCOPE_IDENTITY();
        PRINT 'Usuario admin creado con ID: ' + CAST(@usu_Id AS VARCHAR(10));
    END
END
ELSE
BEGIN
    PRINT 'Usuario encontrado con ID: ' + CAST(@usu_Id AS VARCHAR(10));
END

-- Verificar que el usuario tenga roles asignados
IF @usu_Id IS NOT NULL
BEGIN
    -- Obtener rol de administrador
    DECLARE @admin_rol_Id INT;
    SELECT @admin_rol_Id = Rol_Id FROM [Seguridad].[tbRoles] WHERE Rol_Descripcion = 'Administrador';
    
    -- Asignar rol si no existe
    IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesUsuarios] WHERE usu_Id = @usu_Id AND rol_Id = @admin_rol_Id)
    BEGIN
        INSERT INTO [Seguridad].[tbRolesUsuarios] (usu_Id, rol_Id, usrol_FechaAsignacion, usrol_UsuCreacion)
        VALUES (@usu_Id, @admin_rol_Id, GETDATE(), 1);
        
        PRINT 'Rol de administrador asignado al usuario ID: ' + CAST(@usu_Id AS VARCHAR(10));
    END
    ELSE
    BEGIN
        PRINT 'Usuario ya tiene rol de administrador asignado';
    END
END

-- Verificar pantallas disponibles
SELECT 
    mp.modpt_Id,
    mp.modpt_Descripcion,
    m.Mod_Nombre,
    CASE WHEN rmp.modpt_Id IS NOT NULL THEN 'SÍ' ELSE 'NO' END as AsignadoAdminRol
FROM [Seguridad].[tbModulosPantallas] mp
INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
LEFT JOIN [Seguridad].[tbRolModulosPantallas] rmp ON mp.modpt_Id = rmp.modpt_Id 
    AND rmp.rol_Id = (SELECT Rol_Id FROM [Seguridad].[tbRoles] WHERE Rol_Descripcion = 'Administrador')
ORDER BY m.Mod_Nombre, mp.modpt_Orden;

-- Mostrar información del usuario de prueba
SELECT 
    u.usu_Id,
    u.Usu_Nombre,
    r.Rol_Descripcion,
    e.Emp_NombreCompleto
FROM [Seguridad].[tbUsuarios] u
INNER JOIN [Seguridad].[tbRoles] r ON u.Rol_Id = r.Rol_Id
INNER JOIN [General].[tbEmpleados] e ON u.Emp_Id = e.Emp_Id
WHERE u.usu_Id = @usu_Id;

PRINT 'Script de datos de prueba completado';