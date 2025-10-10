-- =============================================
-- Scripts SQL para Compatibilidad con AHM_INSTA_HELP_ADM
-- Sistema: PetsHome - Hacer AccountController idéntico a AHM
-- Fecha: 2025-07-26
-- =============================================

USE [petshome-7-24-2025]
GO

-- =============================================
-- PROCEDIMIENTOS COMPATIBLES CON AHM
-- =============================================

-- 1. Login simplificado como AHM (UDP_Acce_tbUsuarios_Login)
CREATE OR ALTER PROCEDURE [Seguridad].[UDP_Acce_tbUsuarios_Login]
    @usu_NombreUsuario NVARCHAR(150),
    @contrasena NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.usu_Id,
        u.Emp_Id,
        u.Usu_Nombre AS usu_NombreUsuario,
        CONCAT(p.per_PrimerNombre, ' ', ISNULL(p.per_SegundoNombre, ''), ' ', 
               p.per_ApellidoPaterno, ' ', ISNULL(p.per_ApellidoMaterno, '')) AS usu_NombreCompleto,
        p.per_PrimerNombre,
        p.per_ApellidoPaterno,
        -- Obtener el rol principal (el primero si tiene múltiples)
        (SELECT TOP 1 ru.rol_Id 
         FROM [Seguridad].[tbRolesUsuarios] ru 
         WHERE ru.usu_Id = u.usu_Id 
         ORDER BY ru.rol_usu_FechaAsignacion ASC) AS rol_Id,
        (SELECT TOP 1 r.Rol_Descripcion 
         FROM [Seguridad].[tbRolesUsuarios] ru 
         INNER JOIN [Seguridad].[tbRoles] r ON ru.rol_Id = r.Rol_Id
         WHERE ru.usu_Id = u.usu_Id 
         ORDER BY ru.rol_usu_FechaAsignacion ASC) AS rol_Descripcion,
        u.Usu_EsActivo AS usu_Estado,
        u.usu_ImagenPerfil,
        u.usu_Logueado
    FROM [Seguridad].[tbUsuarios] u
    INNER JOIN [Refugio].[tbEmpleados] e ON u.Emp_Id = e.emp_Id
    INNER JOIN [General].[tbPersonas] p ON e.per_Id = p.per_Id
    WHERE u.Usu_Nombre = @usu_NombreUsuario 
        AND u.Usu_PasswordHash = @contrasena
        AND u.Usu_EsActivo = 1 
        AND ISNULL(u.Usu_Suspendido, 0) = 0
        AND ISNULL(u.Usu_EsEliminado, 0) = 0
        AND e.emp_EsActivo = 1
        -- Verificar que el usuario tenga al menos un rol activo
        AND EXISTS (
            SELECT 1 FROM [Seguridad].[tbRolesUsuarios] ru 
            INNER JOIN [Seguridad].[tbRoles] r ON ru.rol_Id = r.Rol_Id
            WHERE ru.usu_Id = u.usu_Id AND r.Rol_EsActivo = 1
        );
END
GO

-- 2. Login/Logout State Management como AHM
CREATE OR ALTER PROCEDURE [Seguridad].[UDP_Acce_tbUsuarios_LoginIn]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [Seguridad].[tbUsuarios]
    SET usu_Logueado = 1,
        usu_UltimoAcceso = GETDATE(),
        usu_IntentosFallidos = 0,
        usu_FechaBloqueo = NULL
    WHERE usu_Id = @usu_Id;
    
    -- Registrar evento de login
    INSERT INTO [Seguridad].[tbRegistroEventos]
    (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_FechaCreacion)
    VALUES (1, @usu_Id, 'Usuario inició sesión', GETDATE());
    
    SELECT 0 AS Resultado; -- 0 = éxito en AHM
END
GO

CREATE OR ALTER PROCEDURE [Seguridad].[UDP_Acce_tbUsuarios_Logout]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [Seguridad].[tbUsuarios]
    SET usu_Logueado = 0
    WHERE usu_Id = @usu_Id;
    
    -- Registrar evento de logout
    INSERT INTO [Seguridad].[tbRegistroEventos]
    (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_FechaCreacion)
    VALUES (2, @usu_Id, 'Usuario cerró sesión', GETDATE());
    
    SELECT 0 AS Resultado; -- 0 = éxito en AHM
END
GO

-- 3. Obtener pantallas por rol simplificado como AHM (UDP_Acce_PantallasXRol)
CREATE OR ALTER PROCEDURE [Seguridad].[UDP_Acce_PantallasXRol]
    @rol_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT
        mp.modpt_Id,
        mp.modpt_Descripcion,
        mp.modpt_Url,
        mp.modpt_Icono,
        mp.modpt_Orden,
        mp.mod_Id,
        m.Mod_Nombre,
        m.Mod_Descripcion AS Mod_Descripcion,
        m.Mod_Icono AS Mod_Icono,
        m.Mod_Orden AS Mod_Orden
    FROM [Seguridad].[tbRolModulosPantallas] rmp
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    WHERE rmp.rol_Id = @rol_Id 
        AND mp.modpt_EsActivo = 1 
        AND m.Mod_EsActivo = 1
    ORDER BY m.Mod_Orden, mp.modpt_Orden, mp.modpt_Descripcion;
END
GO

-- 4. Helpers para obtener pantallas por usuario (compatible con múltiples roles)
CREATE OR ALTER PROCEDURE [Seguridad].[UDP_Acce_PantallasXUsuario]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT
        mp.modpt_Id,
        mp.modpt_Descripcion,
        mp.modpt_Url,
        mp.modpt_Icono,
        mp.modpt_Orden,
        mp.mod_Id,
        m.Mod_Nombre,
        m.Mod_Descripcion AS Mod_Descripcion,
        m.Mod_Icono AS Mod_Icono,
        m.Mod_Orden AS Mod_Orden
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    WHERE ru.usu_Id = @usu_Id 
        AND mp.modpt_EsActivo = 1 
        AND m.Mod_EsActivo = 1
    ORDER BY m.Mod_Orden, mp.modpt_Orden, mp.modpt_Descripcion;
END
GO

-- 5. Procedimiento para validar usuario (compatible con AHM)
CREATE OR ALTER PROCEDURE [Seguridad].[UDP_Acce_tbUsuarios_NameValidation]
    @usu_UsuarioNombre NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        usu_Id,
        usu_Nombre AS usu_NombreUsuario,
        Emp_Id,
        Usu_EsActivo AS usu_Estado
    FROM [Seguridad].[tbUsuarios]
    WHERE Usu_Nombre = @usu_UsuarioNombre 
        AND ISNULL(Usu_EsEliminado, 0) = 0;
END
GO

-- 6. Procedimiento para obtener detalle de usuario
CREATE OR ALTER PROCEDURE [Seguridad].[UDP_Acce_tbUsuarios_FindDetalle]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.usu_Id,
        u.Emp_Id,
        u.Usu_Nombre AS usu_NombreUsuario,
        CONCAT(p.per_PrimerNombre, ' ', ISNULL(p.per_SegundoNombre, ''), ' ', 
               p.per_ApellidoPaterno, ' ', ISNULL(p.per_ApellidoMaterno, '')) AS usu_NombreCompleto,
        p.per_PrimerNombre,
        p.per_ApellidoPaterno,
        -- Obtener el rol principal
        (SELECT TOP 1 ru.rol_Id 
         FROM [Seguridad].[tbRolesUsuarios] ru 
         WHERE ru.usu_Id = u.usu_Id 
         ORDER BY ru.rol_usu_FechaAsignacion ASC) AS rol_Id,
        (SELECT TOP 1 r.Rol_Descripcion 
         FROM [Seguridad].[tbRolesUsuarios] ru 
         INNER JOIN [Seguridad].[tbRoles] r ON ru.rol_Id = r.Rol_Id
         WHERE ru.usu_Id = u.usu_Id 
         ORDER BY ru.rol_usu_FechaAsignacion ASC) AS rol_Descripcion,
        u.Usu_EsActivo AS usu_Estado,
        u.usu_ImagenPerfil,
        u.usu_Logueado,
        u.usu_UltimoAcceso,
        u.Usu_FechaCrea AS usu_FechaCreacion
    FROM [Seguridad].[tbUsuarios] u
    INNER JOIN [Refugio].[tbEmpleados] e ON u.Emp_Id = e.emp_Id
    INNER JOIN [General].[tbPersonas] p ON e.per_Id = p.per_Id
    WHERE u.usu_Id = @usu_Id 
        AND ISNULL(u.Usu_EsEliminado, 0) = 0;
END
GO

-- 7. Función auxiliar para obtener lista simple de pantallas (como string)
CREATE OR ALTER FUNCTION [Seguridad].[FN_GetPantallasStringPorUsuario]
(
    @usu_Id INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @pantallas NVARCHAR(MAX) = '';
    
    SELECT @pantallas = STRING_AGG(mp.modpt_Descripcion, ',')
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    WHERE ru.usu_Id = @usu_Id 
        AND mp.modpt_EsActivo = 1 
        AND m.Mod_EsActivo = 1;
    
    RETURN ISNULL(@pantallas, '');
END
GO

-- 8. Función auxiliar para obtener lista simple de pantallas por rol (como string)
CREATE OR ALTER FUNCTION [Seguridad].[FN_GetPantallasStringPorRol]
(
    @rol_Id INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @pantallas NVARCHAR(MAX) = '';
    
    SELECT @pantallas = STRING_AGG(mp.modpt_Descripcion, ',')
    FROM [Seguridad].[tbRolModulosPantallas] rmp
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    WHERE rmp.rol_Id = @rol_Id 
        AND mp.modpt_EsActivo = 1 
        AND m.Mod_EsActivo = 1;
    
    RETURN ISNULL(@pantallas, '');
END
GO

-- =============================================
-- ENTIDADES DE COMPATIBILIDAD
-- =============================================

-- Asegurarse de que existe la tabla de componentes
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Seguridad].[tbComponentes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Seguridad].[tbComponentes](
        [comp_Id] [int] IDENTITY(1,1) NOT NULL,
        [comp_Descripcion] [nvarchar](50) NOT NULL,
        CONSTRAINT [PK_Seguridad_tbComponentes_comp_Id] PRIMARY KEY CLUSTERED ([comp_Id] ASC)
    )
    
    -- Insertar componentes básicos
    INSERT INTO [Seguridad].[tbComponentes] ([comp_Descripcion])
    VALUES ('Portal Administrativo'), ('Portal Cliente'), ('Portal Empleado')
END
GO

-- Asegurarse de que existe la tabla de módulos pantallas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Seguridad].[tbModulosPantallas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Seguridad].[tbModulosPantallas](
        [modpt_Id] [int] IDENTITY(1,1) NOT NULL,
        [mod_Id] [int] NOT NULL,
        [modpt_Descripcion] [nvarchar](100) NOT NULL,
        [modpt_Url] [nvarchar](200) NULL,
        [modpt_Icono] [nvarchar](50) NULL,
        [modpt_Orden] [int] NULL,
        [modpt_EsActivo] [bit] NOT NULL DEFAULT(1),
        CONSTRAINT [PK_Seguridad_tbModulosPantallas_modpt_Id] PRIMARY KEY CLUSTERED ([modpt_Id] ASC)
    )
END
GO

-- Asegurarse de que existe la tabla de rol módulos pantallas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Seguridad].[tbRolModulosPantallas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Seguridad].[tbRolModulosPantallas](
        [rolpt_Id] [int] IDENTITY(1,1) NOT NULL,
        [modpt_Id] [int] NOT NULL,
        [rol_Id] [int] NOT NULL,
        [rolpt_FechaAsignacion] [datetime] NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT [PK_Seguridad_tbRolModulosPantallas_rolpt_Id] PRIMARY KEY CLUSTERED ([rolpt_Id] ASC),
        CONSTRAINT [UQ_Seguridad_RolModulosPantallas] UNIQUE ([modpt_Id], [rol_Id])
    )
END
GO

-- =============================================
-- DATOS BÁSICOS PARA COMPATIBILIDAD
-- =============================================

-- Insertar pantallas básicas para el sistema PetsHome si no existen
DECLARE @mod_Admin INT = (SELECT TOP 1 Mod_Id FROM [Seguridad].[tbModulos] WHERE Mod_Nombre LIKE '%EMPLEADOS%' OR Mod_Nombre LIKE '%ADMIN%' OR Mod_Nombre LIKE '%USUARIOS%');
DECLARE @comp_Admin INT = (SELECT TOP 1 comp_Id FROM [Seguridad].[tbComponentes] WHERE comp_Descripcion = 'Portal Administrativo');

-- Crear módulo administrativo si no existe
IF @mod_Admin IS NULL
BEGIN
    INSERT INTO [Seguridad].[tbModulos] (Mod_Nombre, Mod_Descripcion, Mod_Icono, Mod_Orden, comp_Id, Mod_EsActivo)
    VALUES ('ADMINISTRACION', 'Administración del Sistema', 'fa-cogs', 1, @comp_Admin, 1);
    SET @mod_Admin = SCOPE_IDENTITY();
END

-- Insertar pantallas básicas si no existen
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbModulosPantallas] WHERE mod_Id = @mod_Admin)
BEGIN
    INSERT INTO [Seguridad].[tbModulosPantallas] (mod_Id, modpt_Descripcion, modpt_Url, modpt_Icono, modpt_Orden)
    VALUES 
        (@mod_Admin, 'Dashboard', '/Home/Index', 'fa-tachometer-alt', 1),
        (@mod_Admin, 'Gestión de Usuarios', '/Account/Register', 'fa-users', 2),
        (@mod_Admin, 'Mascotas', '/mascota', 'fa-paw', 3),
        (@mod_Admin, 'Adopciones', '/adopcion', 'fa-heart', 4),
        (@mod_Admin, 'Citas Médicas', '/citamedica', 'fa-stethoscope', 5),
        (@mod_Admin, 'Voluntarios', '/voluntario', 'fa-hands-helping', 6),
        (@mod_Admin, 'Refugios', '/refugio', 'fa-home', 7),
        (@mod_Admin, 'Empleados', '/empleado', 'fa-user-tie', 8),
        (@mod_Admin, 'Reportes', '/reportes', 'fa-chart-bar', 9),
        (@mod_Admin, 'Inventario', '/item', 'fa-boxes', 10);
END

-- Asignar todas las pantallas al rol de administrador
DECLARE @rol_Admin INT = (SELECT TOP 1 Rol_Id FROM [Seguridad].[tbRoles] WHERE Rol_Descripcion = 'Administrador');

IF @rol_Admin IS NOT NULL
BEGIN
    INSERT INTO [Seguridad].[tbRolModulosPantallas] (modpt_Id, rol_Id)
    SELECT mp.modpt_Id, @rol_Admin
    FROM [Seguridad].[tbModulosPantallas] mp
    WHERE NOT EXISTS (
        SELECT 1 FROM [Seguridad].[tbRolModulosPantallas] rmp 
        WHERE rmp.modpt_Id = mp.modpt_Id AND rmp.rol_Id = @rol_Admin
    );
END

PRINT '================================================';
PRINT 'Scripts de compatibilidad con AHM ejecutados exitosamente';
PRINT '================================================';
PRINT 'Procedimientos creados:';
PRINT '- UDP_Acce_tbUsuarios_Login';
PRINT '- UDP_Acce_tbUsuarios_LoginIn';
PRINT '- UDP_Acce_tbUsuarios_Logout';
PRINT '- UDP_Acce_PantallasXRol';
PRINT '- UDP_Acce_PantallasXUsuario';
PRINT '- UDP_Acce_tbUsuarios_NameValidation';
PRINT '- UDP_Acce_tbUsuarios_FindDetalle';
PRINT '';
PRINT 'Funciones auxiliares:';
PRINT '- FN_GetPantallasStringPorUsuario';
PRINT '- FN_GetPantallasStringPorRol';
PRINT '';
PRINT 'Listo para actualizar AccountController';
PRINT '================================================';
GO