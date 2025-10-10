-- =============================================
-- Diseño Correcto: Sistema RBAC (Role-Based Access Control)
-- =============================================

-- Tabla de Roles (simplificada)
CREATE TABLE seguridad.tbRoles (
    Rol_Id INT IDENTITY(1,1) PRIMARY KEY,
    Rol_Descripcion NVARCHAR(100) NOT NULL,
    Rol_EsActivo BIT NOT NULL DEFAULT 1,
    Rol_EsEliminado BIT NOT NULL DEFAULT 0,
    Rol_FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    Rol_FechaModificacion DATETIME NULL
);

-- Tabla de Módulos/Pantallas del Sistema
CREATE TABLE seguridad.tbModulos (
    Mod_Id INT IDENTITY(1,1) PRIMARY KEY,
    Mod_Nombre NVARCHAR(50) NOT NULL,
    Mod_Descripcion NVARCHAR(200) NULL,
    Mod_Icono NVARCHAR(50) NULL,
    Mod_Url NVARCHAR(200) NULL,
    Mod_Orden INT NULL,
    Mod_EsActivo BIT NOT NULL DEFAULT 1,
    Mod_FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);

-- Tabla de Permisos (CRUD + otros)
CREATE TABLE seguridad.tbPermisos (
    Per_Id INT IDENTITY(1,1) PRIMARY KEY,
    Per_Nombre NVARCHAR(50) NOT NULL,
    Per_Descripcion NVARCHAR(200) NULL,
    Per_EsActivo BIT NOT NULL DEFAULT 1
);

-- Tabla Intermedia: Qué módulos puede ver cada rol
CREATE TABLE seguridad.tbRolModulos (
    RolMod_Id INT IDENTITY(1,1) PRIMARY KEY,
    Rol_Id INT NOT NULL,
    Mod_Id INT NOT NULL,
    RolMod_FechaAsignacion DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (Rol_Id) REFERENCES seguridad.tbRoles(Rol_Id),
    FOREIGN KEY (Mod_Id) REFERENCES seguridad.tbModulos(Mod_Id),
    UNIQUE(Rol_Id, Mod_Id)
);

-- Tabla Intermedia: Qué permisos tiene cada rol en cada módulo
CREATE TABLE seguridad.tbRolModuloPermisos (
    RolModPer_Id INT IDENTITY(1,1) PRIMARY KEY,
    Rol_Id INT NOT NULL,
    Mod_Id INT NOT NULL,
    Per_Id INT NOT NULL,
    RolModPer_FechaAsignacion DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (Rol_Id) REFERENCES seguridad.tbRoles(Rol_Id),
    FOREIGN KEY (Mod_Id) REFERENCES seguridad.tbModulos(Mod_Id),
    FOREIGN KEY (Per_Id) REFERENCES seguridad.tbPermisos(Per_Id),
    UNIQUE(Rol_Id, Mod_Id, Per_Id)
);

-- =============================================
-- Insertar datos maestros
-- =============================================

-- Insertar Roles
INSERT INTO seguridad.tbRoles (Rol_Descripcion) VALUES 
('Administrador'),
('Director'),
('Supervisor'),
('Veterinario'),
('Cuidador'),
('Usuario Básico');

-- Insertar Módulos (actualizados según controladores existentes)
INSERT INTO seguridad.tbModulos (Mod_Nombre, Mod_Descripcion, Mod_Icono, Mod_Url, Mod_Orden) VALUES 
('EMPLEADOS', 'Gestión de Empleados', 'fa-user-tie', '/empleado', 1),
('MASCOTAS', 'Gestión de Mascotas', 'fa-paw', '/mascota', 2),
('REFUGIOS', 'Gestión de Refugios', 'fa-home', '/refugio', 3),
('ADOPCIONES', 'Gestión de Adopciones', 'fa-heart', '/adopcion', 4),
('CITAS_MEDICAS', 'Citas Médicas', 'fa-stethoscope', '/citamedica', 5),
('VOLUNTARIOS', 'Gestión de Voluntarios', 'fa-hands-helping', '/voluntario', 6),
('SOLICITUDES', 'Gestión de Solicitudes', 'fa-file-alt', '/solicitud', 7),
('EVENTOS', 'Gestión de Eventos', 'fa-calendar', '/evento', 8),
('DONACIONES', 'Gestión de Donaciones', 'fa-donate', '/donacion', 9),
('REPORTES', 'Reportes del Sistema', 'fa-chart-bar', '/reportes', 10),
('INVENTARIO', 'Gestión de Inventario', 'fa-boxes', '/item', 11),
('VACUNAS', 'Gestión de Vacunas', 'fa-syringe', '/vacuna', 12),
('USUARIOS', 'Gestión de Usuarios del Sistema', 'fa-users', '/account', 13);

-- Insertar Permisos
INSERT INTO seguridad.tbPermisos (Per_Nombre, Per_Descripcion) VALUES 
('CREATE', 'Crear nuevos registros'),
('READ', 'Ver/Consultar registros'),
('UPDATE', 'Modificar registros existentes'),
('DELETE', 'Eliminar registros'),
('EXPORT', 'Exportar información'),
('APPROVE', 'Aprobar procesos');

-- =============================================
-- Asignar módulos a roles
-- =============================================

-- Administrador: Todos los módulos
INSERT INTO seguridad.tbRolModulos (Rol_Id, Mod_Id)
SELECT 1, Mod_Id FROM seguridad.tbModulos;

-- Director: Empleados, Refugios, Mascotas, Adopciones, Voluntarios, Reportes
INSERT INTO seguridad.tbRolModulos (Rol_Id, Mod_Id)
SELECT 2, Mod_Id FROM seguridad.tbModulos WHERE Mod_Nombre IN ('EMPLEADOS', 'REFUGIOS', 'MASCOTAS', 'ADOPCIONES', 'VOLUNTARIOS', 'REPORTES');

-- Supervisor: Empleados, Mascotas, Voluntarios, Reportes
INSERT INTO seguridad.tbRolModulos (Rol_Id, Mod_Id)
SELECT 3, Mod_Id FROM seguridad.tbModulos WHERE Mod_Nombre IN ('EMPLEADOS', 'MASCOTAS', 'VOLUNTARIOS', 'REPORTES');

-- Veterinario: Mascotas, Citas Médicas, Vacunas, Reportes
INSERT INTO seguridad.tbRolModulos (Rol_Id, Mod_Id)
SELECT 4, Mod_Id FROM seguridad.tbModulos WHERE Mod_Nombre IN ('MASCOTAS', 'CITAS_MEDICAS', 'VACUNAS', 'REPORTES');

-- Cuidador: Mascotas, Citas Médicas
INSERT INTO seguridad.tbRolModulos (Rol_Id, Mod_Id)
SELECT 5, Mod_Id FROM seguridad.tbModulos WHERE Mod_Nombre IN ('MASCOTAS', 'CITAS_MEDICAS');

-- Usuario Básico: Solo Reportes
INSERT INTO seguridad.tbRolModulos (Rol_Id, Mod_Id)
SELECT 6, Mod_Id FROM seguridad.tbModulos WHERE Mod_Nombre = 'REPORTES';

-- =============================================
-- Asignar permisos específicos
-- =============================================

-- Administrador: Todos los permisos en todos sus módulos
INSERT INTO seguridad.tbRolModuloPermisos (Rol_Id, Mod_Id, Per_Id)
SELECT rm.Rol_Id, rm.Mod_Id, p.Per_Id
FROM seguridad.tbRolModulos rm
CROSS JOIN seguridad.tbPermisos p
WHERE rm.Rol_Id = 1; -- Administrador

-- Director: CRUD + EXPORT en sus módulos
INSERT INTO seguridad.tbRolModuloPermisos (Rol_Id, Mod_Id, Per_Id)
SELECT rm.Rol_Id, rm.Mod_Id, p.Per_Id
FROM seguridad.tbRolModulos rm
CROSS JOIN seguridad.tbPermisos p
WHERE rm.Rol_Id = 2 -- Director
AND p.Per_Nombre IN ('CREATE', 'READ', 'UPDATE', 'DELETE', 'EXPORT');

-- Supervisor: READ, UPDATE en sus módulos
INSERT INTO seguridad.tbRolModuloPermisos (Rol_Id, Mod_Id, Per_Id)
SELECT rm.Rol_Id, rm.Mod_Id, p.Per_Id
FROM seguridad.tbRolModulos rm
CROSS JOIN seguridad.tbPermisos p
WHERE rm.Rol_Id = 3 -- Supervisor
AND p.Per_Nombre IN ('READ', 'UPDATE');

-- Veterinario: CRUD en Mascotas, Citas Médicas, Vacunas; READ en Reportes
INSERT INTO seguridad.tbRolModuloPermisos (Rol_Id, Mod_Id, Per_Id)
SELECT rm.Rol_Id, rm.Mod_Id, p.Per_Id
FROM seguridad.tbRolModulos rm
CROSS JOIN seguridad.tbPermisos p
WHERE rm.Rol_Id = 4 -- Veterinario
AND ((rm.Mod_Id IN (SELECT Mod_Id FROM seguridad.tbModulos WHERE Mod_Nombre IN ('MASCOTAS', 'CITAS_MEDICAS', 'VACUNAS')) 
      AND p.Per_Nombre IN ('CREATE', 'READ', 'UPDATE', 'DELETE'))
     OR (rm.Mod_Id = (SELECT Mod_Id FROM seguridad.tbModulos WHERE Mod_Nombre = 'REPORTES') 
         AND p.Per_Nombre = 'READ'));

-- Cuidador: READ, UPDATE en Mascotas y Citas Médicas
INSERT INTO seguridad.tbRolModuloPermisos (Rol_Id, Mod_Id, Per_Id)
SELECT rm.Rol_Id, rm.Mod_Id, p.Per_Id
FROM seguridad.tbRolModulos rm
CROSS JOIN seguridad.tbPermisos p
WHERE rm.Rol_Id = 5 -- Cuidador
AND p.Per_Nombre IN ('READ', 'UPDATE');

-- Usuario Básico: Solo READ en Reportes
INSERT INTO seguridad.tbRolModuloPermisos (Rol_Id, Mod_Id, Per_Id)
SELECT rm.Rol_Id, rm.Mod_Id, p.Per_Id
FROM seguridad.tbRolModulos rm
CROSS JOIN seguridad.tbPermisos p
WHERE rm.Rol_Id = 6 -- Usuario Básico
AND p.Per_Nombre = 'READ';

-- =============================================
-- Procedimientos para consultar permisos
-- =============================================

-- Obtener todos los módulos y permisos de un rol
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Roles_GetPermissions]
    @Rol_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.Rol_Descripcion,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        m.Mod_Icono,
        m.Mod_Url,
        m.Mod_Orden,
        STRING_AGG(p.Per_Nombre, ',') as Permisos
    FROM seguridad.tbRoles r
    INNER JOIN seguridad.tbRolModulos rm ON r.Rol_Id = rm.Rol_Id
    INNER JOIN seguridad.tbModulos m ON rm.Mod_Id = m.Mod_Id
    LEFT JOIN seguridad.tbRolModuloPermisos rmp ON r.Rol_Id = rmp.Rol_Id AND m.Mod_Id = rmp.Mod_Id
    LEFT JOIN seguridad.tbPermisos p ON rmp.Per_Id = p.Per_Id
    WHERE r.Rol_Id = @Rol_Id
        AND r.Rol_EsActivo = 1
        AND m.Mod_EsActivo = 1
    GROUP BY r.Rol_Descripcion, m.Mod_Nombre, m.Mod_Descripcion, m.Mod_Icono, m.Mod_Url, m.Mod_Orden
    ORDER BY m.Mod_Orden;
END
GO

-- Verificar si un usuario tiene un permiso específico
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_CheckPermission]
    @usu_Id INT,
    @Mod_Nombre NVARCHAR(50),
    @Per_Nombre NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) as TienePermiso
    FROM seguridad.tbUsuarios u
    INNER JOIN seguridad.tbRolModuloPermisos rmp ON u.Rol_Id = rmp.Rol_Id
    INNER JOIN seguridad.tbModulos m ON rmp.Mod_Id = m.Mod_Id
    INNER JOIN seguridad.tbPermisos p ON rmp.Per_Id = p.Per_Id
    WHERE u.usu_Id = @usu_Id
        AND m.Mod_Nombre = @Mod_Nombre
        AND p.Per_Nombre = @Per_Nombre
        AND u.Usu_EsActivo = 1
        AND ISNULL(u.Usu_Suspendido, 0) = 0
        AND ISNULL(u.Usu_EsEliminado, 0) = 0;
END
GO

-- =============================================
-- Consultas de ejemplo
-- =============================================

-- Ver permisos de un rol específico
SELECT 
    r.Rol_Descripcion,
    m.Mod_Nombre,
    p.Per_Nombre
FROM seguridad.tbRoles r
INNER JOIN seguridad.tbRolModuloPermisos rmp ON r.Rol_Id = rmp.Rol_Id
INNER JOIN seguridad.tbModulos m ON rmp.Mod_Id = m.Mod_Id
INNER JOIN seguridad.tbPermisos p ON rmp.Per_Id = p.Per_Id
WHERE r.Rol_Id = 4 -- Veterinario
ORDER BY m.Mod_Nombre, p.Per_Nombre;

-- Ver todos los módulos disponibles para un rol
SELECT 
    r.Rol_Descripcion,
    m.Mod_Nombre,
    m.Mod_Descripcion,
    m.Mod_Url
FROM seguridad.tbRoles r
INNER JOIN seguridad.tbRolModulos rm ON r.Rol_Id = rm.Rol_Id
INNER JOIN seguridad.tbModulos m ON rm.Mod_Id = m.Mod_Id
WHERE r.Rol_Id = 3 -- Supervisor
ORDER BY m.Mod_Orden;