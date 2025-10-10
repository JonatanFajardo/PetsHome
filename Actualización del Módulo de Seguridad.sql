-- =============================================
-- Script de Actualización del Módulo de Seguridad
-- Sistema: PetsHome
-- Fecha: 2025-07-24
-- Descripción: Implementación de modelo de seguridad basado en accesAHM
-- =============================================

USE [petshome-7-24-2025]
GO

-- =============================================
-- PASO 1: CREAR NUEVAS TABLAS PARA EL MODELO COMPLETO
-- =============================================

-- Tabla de Componentes (para diferenciar portales/áreas del sistema)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Seguridad].[tbComponentes]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Seguridad].[tbComponentes](
        [comp_Id] [int] IDENTITY(1,1) NOT NULL,
        [comp_Descripcion] [nvarchar](50) NOT NULL,
        CONSTRAINT [PK_Seguridad_tbComponentes_comp_Id] PRIMARY KEY CLUSTERED ([comp_Id] ASC)
    ) ON [PRIMARY]
END
GO

-- Modificar tabla de Módulos para incluir referencia a Componente
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Seguridad].[tbModulos]') AND name = 'comp_Id')
BEGIN
    ALTER TABLE [Seguridad].[tbModulos] 
    ADD [comp_Id] [int] NOT NULL DEFAULT(1)
END
GO

-- Tabla de Pantallas por Módulo
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
    ) ON [PRIMARY]
END
GO

-- Tabla de Roles por Pantallas (reemplaza RolModulos)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Seguridad].[tbRolModulosPantallas]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Seguridad].[tbRolModulosPantallas](
        [rolpt_Id] [int] IDENTITY(1,1) NOT NULL,
        [modpt_Id] [int] NOT NULL,
        [rol_Id] [int] NOT NULL,
        [rolpt_FechaAsignacion] [datetime] NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT [PK_Seguridad_tbRolModulosPantallas_rolpt_Id] PRIMARY KEY CLUSTERED ([rolpt_Id] ASC),
        CONSTRAINT [UQ_Seguridad_RolModulosPantallas] UNIQUE ([modpt_Id], [rol_Id])
    ) ON [PRIMARY]
END
GO

-- Tabla de Roles por Usuario (relación muchos a muchos)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Seguridad].[tbRolesUsuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Seguridad].[tbRolesUsuarios](
        [rol_usu_Id] [int] IDENTITY(1,1) NOT NULL,
        [rol_Id] [int] NOT NULL,
        [usu_Id] [int] NOT NULL,
        [rol_usu_FechaAsignacion] [datetime] NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT [PK_Seguridad_tbRolesUsuarios_rol_usu_Id] PRIMARY KEY CLUSTERED ([rol_usu_Id] ASC),
        CONSTRAINT [UQ_Seguridad_RolesUsuarios] UNIQUE ([rol_Id], [usu_Id])
    ) ON [PRIMARY]
END
GO

-- Agregar campos adicionales a la tabla de Usuarios
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Seguridad].[tbUsuarios]') AND name = 'usu_ImagenPerfil')
BEGIN
    ALTER TABLE [Seguridad].[tbUsuarios] 
    ADD [usu_ImagenPerfil] [nvarchar](max) NULL,
        [usu_Logueado] [bit] NULL DEFAULT(0),
        [usu_UltimoAcceso] [datetime] NULL,
        [usu_IntentosFallidos] [int] NULL DEFAULT(0),
        [usu_FechaBloqueo] [datetime] NULL
END
GO

-- Modificar tabla de RegistroEventos para agregar más campos
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Seguridad].[tbRegistroEventos]') AND name = 'Evt_Pantalla')
BEGIN
    ALTER TABLE [Seguridad].[tbRegistroEventos] 
    ADD [Evt_Pantalla] [nvarchar](100) NULL,
        [Evt_Modulo] [nvarchar](100) NULL,
        [Evt_Componente] [nvarchar](50) NULL
END
GO

-- =============================================
-- PASO 2: CREAR RELACIONES (FOREIGN KEYS)
-- =============================================

-- FK: Módulos -> Componentes
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Seguridad].[FK_tbModulos_tbComponentes_comp_Id]'))
BEGIN
    ALTER TABLE [Seguridad].[tbModulos] WITH CHECK 
    ADD CONSTRAINT [FK_tbModulos_tbComponentes_comp_Id] FOREIGN KEY([comp_Id])
    REFERENCES [Seguridad].[tbComponentes] ([comp_Id])
END
GO

-- FK: ModulosPantallas -> Módulos
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Seguridad].[FK_tbModulosPantallas_tbModulos_mod_Id]'))
BEGIN
    ALTER TABLE [Seguridad].[tbModulosPantallas] WITH CHECK 
    ADD CONSTRAINT [FK_tbModulosPantallas_tbModulos_mod_Id] FOREIGN KEY([mod_Id])
    REFERENCES [Seguridad].[tbModulos] ([Mod_Id])
END
GO

-- FK: RolModulosPantallas -> ModulosPantallas
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Seguridad].[FK_tbRolModulosPantallas_tbModulosPantallas_modpt_Id]'))
BEGIN
    ALTER TABLE [Seguridad].[tbRolModulosPantallas] WITH CHECK 
    ADD CONSTRAINT [FK_tbRolModulosPantallas_tbModulosPantallas_modpt_Id] FOREIGN KEY([modpt_Id])
    REFERENCES [Seguridad].[tbModulosPantallas] ([modpt_Id])
END
GO

-- FK: RolModulosPantallas -> Roles
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Seguridad].[FK_tbRolModulosPantallas_tbRoles_rol_Id]'))
BEGIN
    ALTER TABLE [Seguridad].[tbRolModulosPantallas] WITH CHECK 
    ADD CONSTRAINT [FK_tbRolModulosPantallas_tbRoles_rol_Id] FOREIGN KEY([rol_Id])
    REFERENCES [Seguridad].[tbRoles] ([Rol_Id])
END
GO

-- FK: RolesUsuarios -> Roles
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Seguridad].[FK_tbRolesUsuarios_tbRoles_rol_Id]'))
BEGIN
    ALTER TABLE [Seguridad].[tbRolesUsuarios] WITH CHECK 
    ADD CONSTRAINT [FK_tbRolesUsuarios_tbRoles_rol_Id] FOREIGN KEY([rol_Id])
    REFERENCES [Seguridad].[tbRoles] ([Rol_Id])
END
GO

-- FK: RolesUsuarios -> Usuarios
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Seguridad].[FK_tbRolesUsuarios_tbUsuarios_usu_Id]'))
BEGIN
    ALTER TABLE [Seguridad].[tbRolesUsuarios] WITH CHECK 
    ADD CONSTRAINT [FK_tbRolesUsuarios_tbUsuarios_usu_Id] FOREIGN KEY([usu_Id])
    REFERENCES [Seguridad].[tbUsuarios] ([usu_Id])
END
GO

-- =============================================
-- PASO 3: DATOS INICIALES
-- =============================================

-- Insertar Componentes básicos
IF NOT EXISTS (SELECT * FROM [Seguridad].[tbComponentes])
BEGIN
    INSERT INTO [Seguridad].[tbComponentes] ([comp_Descripcion])
    VALUES 
        ('Portal Administrativo'),
        ('Portal Cliente'),
        ('Portal Empleado')
END
GO

-- Insertar Tipos de Eventos adicionales
IF NOT EXISTS (SELECT * FROM [Seguridad].[tbTipoEventos] WHERE [Tpevt_Descripcion] = 'Inicio de Sesión')
BEGIN
    INSERT INTO [Seguridad].[tbTipoEventos] ([Tpevt_Descripcion])
    VALUES 
        ('Inicio de Sesión'),
        ('Cierre de Sesión'),
        ('Intento de Acceso Fallido'),
        ('Cambio de Contraseña'),
        ('Creación de Usuario'),
        ('Modificación de Usuario'),
        ('Asignación de Rol'),
        ('Acceso a Pantalla'),
        ('Operación CRUD'),
        ('Error del Sistema')
END
GO

-- =============================================
-- PASO 4: PROCEDIMIENTOS ALMACENADOS
-- =============================================

-- SP: Login mejorado con registro de eventos
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Usuarios_Login_V2]
    @Usu_Nombre NVARCHAR(150),
    @Con_Hash NVARCHAR(255),
    @UserAgent NVARCHAR(MAX) = NULL,
    @DireccionIP NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @usu_Id INT;
    DECLARE @intentosFallidos INT;
    DECLARE @fechaBloqueo DATETIME;
    DECLARE @resultado INT = 0;
    
    -- Verificar si el usuario existe
    SELECT @usu_Id = usu_Id, 
           @intentosFallidos = ISNULL(usu_IntentosFallidos, 0),
           @fechaBloqueo = usu_FechaBloqueo
    FROM [Seguridad].[tbUsuarios]
    WHERE Usu_Nombre = @Usu_Nombre;
    
    -- Si no existe el usuario
    IF @usu_Id IS NULL
    BEGIN
        -- Registrar intento fallido
        INSERT INTO [Seguridad].[tbRegistroEventos]
        (Tpevt_Id, Evt_Detalles, Evt_UserAgent, Evt_DireccionIP, Evt_FechaCreacion)
        VALUES (3, 'Intento de acceso con usuario inexistente: ' + @Usu_Nombre, 
                @UserAgent, @DireccionIP, GETDATE());
        
        SELECT @resultado AS Resultado, 'Usuario no encontrado' AS Mensaje;
        RETURN;
    END
    
    -- Verificar si está bloqueado
    IF @fechaBloqueo IS NOT NULL AND DATEDIFF(MINUTE, @fechaBloqueo, GETDATE()) < 30
    BEGIN
        SELECT @resultado AS Resultado, 'Usuario bloqueado temporalmente' AS Mensaje;
        RETURN;
    END
    
    -- Verificar credenciales
    IF EXISTS (
        SELECT 1
        FROM [Seguridad].[tbUsuarios] u
        INNER JOIN [Refugio].[tbEmpleados] e ON u.Emp_Id = e.emp_Id
        WHERE u.usu_Id = @usu_Id 
            AND u.Usu_PasswordHash = @Con_Hash
            AND u.Usu_EsActivo = 1 
            AND ISNULL(u.Usu_Suspendido, 0) = 0
            AND ISNULL(u.Usu_EsEliminado, 0) = 0
            AND e.emp_EsActivo = 1
    )
    BEGIN
        -- Login exitoso
        UPDATE [Seguridad].[tbUsuarios]
        SET usu_Logueado = 1,
            usu_UltimoAcceso = GETDATE(),
            usu_IntentosFallidos = 0,
            usu_FechaBloqueo = NULL
        WHERE usu_Id = @usu_Id;
        
        -- Registrar evento de login
        INSERT INTO [Seguridad].[tbRegistroEventos]
        (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_UserAgent, Evt_DireccionIP, Evt_FechaCreacion)
        VALUES (1, @usu_Id, 'Inicio de sesión exitoso', @UserAgent, @DireccionIP, GETDATE());
        
        -- Retornar datos del usuario con sus permisos
        SELECT 
            u.usu_Id,
            u.Emp_Id,
            u.Usu_Nombre,
            p.per_PrimerNombre + ' ' + ISNULL(p.per_SegundoNombre, '') AS Emp_Nombres,
            p.per_ApellidoPaterno + ' ' + ISNULL(p.per_ApellidoMaterno, '') AS Emp_Apellidos,
            u.usu_ImagenPerfil,
            1 AS Resultado,
            'Login exitoso' AS Mensaje
        FROM [Seguridad].[tbUsuarios] u
        INNER JOIN [Refugio].[tbEmpleados] e ON u.Emp_Id = e.emp_Id
        INNER JOIN [General].[tbPersonas] p ON e.per_Id = p.per_Id 
        WHERE u.usu_Id = @usu_Id;
        
        -- Retornar roles del usuario
        SELECT DISTINCT
            ru.rol_Id,
            r.Rol_Descripcion
        FROM [Seguridad].[tbRolesUsuarios] ru
        INNER JOIN [Seguridad].[tbRoles] r ON ru.rol_Id = r.Rol_Id
        WHERE ru.usu_Id = @usu_Id AND r.Rol_EsActivo = 1;
        
    END
    ELSE
    BEGIN
        -- Login fallido
        UPDATE [Seguridad].[tbUsuarios]
        SET usu_IntentosFallidos = @intentosFallidos + 1,
            usu_FechaBloqueo = CASE WHEN @intentosFallidos + 1 >= 3 THEN GETDATE() ELSE NULL END
        WHERE usu_Id = @usu_Id;
        
        -- Registrar evento de intento fallido
        INSERT INTO [Seguridad].[tbRegistroEventos]
        (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_UserAgent, Evt_DireccionIP, Evt_FechaCreacion)
        VALUES (3, @usu_Id, 'Intento de acceso fallido', @UserAgent, @DireccionIP, GETDATE());
        
        SELECT @resultado AS Resultado, 
               CASE WHEN @intentosFallidos + 1 >= 3 
                    THEN 'Usuario bloqueado por múltiples intentos fallidos' 
                    ELSE 'Credenciales incorrectas' END AS Mensaje;
    END
END
GO

-- SP: Obtener permisos por rol (pantallas accesibles)
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_PantallasPorRol]
    @rol_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Componentes accesibles
    SELECT DISTINCT
        c.comp_Id,
        c.comp_Descripcion
    FROM [Seguridad].[tbRolModulosPantallas] rmp
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE rmp.rol_Id = @rol_Id AND mp.modpt_EsActivo = 1 AND m.Mod_EsActivo = 1;
    
    -- Módulos accesibles
    SELECT DISTINCT
        m.Mod_Id,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        m.Mod_Icono,
        m.Mod_Orden,
        c.comp_Id,
        c.comp_Descripcion
    FROM [Seguridad].[tbRolModulosPantallas] rmp
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE rmp.rol_Id = @rol_Id AND mp.modpt_EsActivo = 1 AND m.Mod_EsActivo = 1
    ORDER BY m.Mod_Orden, m.Mod_Nombre;
    
    -- Pantallas accesibles
    SELECT 
        mp.modpt_Id,
        mp.mod_Id,
        mp.modpt_Descripcion,
        mp.modpt_Url,
        mp.modpt_Icono,
        mp.modpt_Orden,
        m.Mod_Nombre,
        c.comp_Descripcion
    FROM [Seguridad].[tbRolModulosPantallas] rmp
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE rmp.rol_Id = @rol_Id AND mp.modpt_EsActivo = 1 AND m.Mod_EsActivo = 1
    ORDER BY mp.modpt_Orden, mp.modpt_Descripcion;
END
GO

-- SP: Obtener permisos por usuario (considerando múltiples roles)
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_PantallasPorUsuario]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Componentes accesibles
    SELECT DISTINCT
        c.comp_Id,
        c.comp_Descripcion
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE ru.usu_Id = @usu_Id AND mp.modpt_EsActivo = 1 AND m.Mod_EsActivo = 1;
    
    -- Módulos accesibles
    SELECT DISTINCT
        m.Mod_Id,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        m.Mod_Icono,
        m.Mod_Orden,
        c.comp_Id,
        c.comp_Descripcion
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE ru.usu_Id = @usu_Id AND mp.modpt_EsActivo = 1 AND m.Mod_EsActivo = 1
    ORDER BY m.Mod_Orden, m.Mod_Nombre;
    
    -- Pantallas accesibles con permisos consolidados
    SELECT DISTINCT
        mp.modpt_Id,
        mp.mod_Id,
        mp.modpt_Descripcion,
        mp.modpt_Url,
        mp.modpt_Icono,
        mp.modpt_Orden,
        m.Mod_Nombre,
        c.comp_Descripcion,
        STRING_AGG(p.Per_Nombre, ',') AS Permisos
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    LEFT JOIN [Seguridad].[tbRolModuloPermisos] rmpe ON rmp.rol_Id = rmpe.Rol_Id AND mp.mod_Id = rmpe.Mod_Id
    LEFT JOIN [Seguridad].[tbPermisos] p ON rmpe.Per_Id = p.Per_Id
    WHERE ru.usu_Id = @usu_Id AND mp.modpt_EsActivo = 1 AND m.Mod_EsActivo = 1
    GROUP BY mp.modpt_Id, mp.mod_Id, mp.modpt_Descripcion, mp.modpt_Url,
             mp.modpt_Icono, mp.modpt_Orden, m.Mod_Nombre, c.comp_Descripcion
    ORDER BY mp.modpt_Orden, mp.modpt_Descripcion;
END
GO

-- SP: Asignar rol a usuario
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RolesUsuarios_Insert]
    @rol_Id INT,
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si ya existe la asignación
    IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesUsuarios] WHERE rol_Id = @rol_Id AND usu_Id = @usu_Id)
    BEGIN
        INSERT INTO [Seguridad].[tbRolesUsuarios] (rol_Id, usu_Id)
        VALUES (@rol_Id, @usu_Id);
        
        -- Registrar evento
        INSERT INTO [Seguridad].[tbRegistroEventos]
        (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_FechaCreacion)
        VALUES (7, @usu_Id, 'Asignación de rol ID: ' + CAST(@rol_Id AS NVARCHAR(10)), GETDATE());
        
        SELECT SCOPE_IDENTITY() AS rol_usu_Id, 'Rol asignado exitosamente' AS Mensaje;
    END
    ELSE
    BEGIN
        SELECT 0 AS rol_usu_Id, 'El usuario ya tiene este rol asignado' AS Mensaje;
    END
END
GO

-- SP: Remover rol de usuario
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RolesUsuarios_Delete]
    @rol_Id INT,
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [Seguridad].[tbRolesUsuarios]
    WHERE rol_Id = @rol_Id AND usu_Id = @usu_Id;
    
    -- Registrar evento
    INSERT INTO [Seguridad].[tbRegistroEventos]
    (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_FechaCreacion)
    VALUES (7, @usu_Id, 'Remoción de rol ID: ' + CAST(@rol_Id AS NVARCHAR(10)), GETDATE());
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- SP: Registrar acceso a pantalla
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RegistrarAccesoPantalla]
    @usu_Id INT,
    @modpt_Id INT,
    @UserAgent NVARCHAR(MAX) = NULL,
    @DireccionIP NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @pantalla NVARCHAR(100);
    DECLARE @modulo NVARCHAR(100);
    DECLARE @componente NVARCHAR(50);
    
    -- Obtener información de la pantalla
    SELECT 
        @pantalla = mp.modpt_Descripcion,
        @modulo = m.Mod_Nombre,
        @componente = c.comp_Descripcion
    FROM [Seguridad].[tbModulosPantallas] mp
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE mp.modpt_Id = @modpt_Id;
    
    -- Registrar evento
    INSERT INTO [Seguridad].[tbRegistroEventos]
    (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_UserAgent, Evt_DireccionIP, 
     Evt_Pantalla, Evt_Modulo, Evt_Componente, Evt_FechaCreacion)
    VALUES (8, @usu_Id, 'Acceso a pantalla: ' + @pantalla, @UserAgent, @DireccionIP,
            @pantalla, @modulo, @componente, GETDATE());
END
GO

-- SP: Logout de usuario
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Usuarios_Logout]
    @usu_Id INT,
    @UserAgent NVARCHAR(MAX) = NULL,
    @DireccionIP NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Actualizar estado del usuario
    UPDATE [Seguridad].[tbUsuarios]
    SET usu_Logueado = 0
    WHERE usu_Id = @usu_Id;
    
    -- Registrar evento
    INSERT INTO [Seguridad].[tbRegistroEventos]
    (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_UserAgent, Evt_DireccionIP, Evt_FechaCreacion)
    VALUES (2, @usu_Id, 'Cierre de sesión', @UserAgent, @DireccionIP, GETDATE());
    
    SELECT 'Sesión cerrada exitosamente' AS Mensaje;
END
GO

-- =============================================
-- PASO 5: VISTAS ÚTILES
-- =============================================

-- Vista: Permisos completos por usuario
CREATE OR ALTER VIEW [Seguridad].[vw_PermisosUsuario]
AS
SELECT DISTINCT
    u.usu_Id,
    u.Usu_Nombre,
    p.per_PrimerNombre + ' ' + ISNULL(p.per_SegundoNombre, '') + ' ' + 
    p.per_ApellidoPaterno + ' ' + ISNULL(p.per_ApellidoMaterno, '') AS NombreCompleto,
    r.Rol_Id,
    r.Rol_Descripcion,
    c.comp_Id,
    c.comp_Descripcion,
    m.Mod_Id,
    m.Mod_Nombre,
    mp.modpt_Id,
    mp.modpt_Descripcion,
    mp.modpt_Url,
    pe.Per_Nombre AS Permiso
FROM [Seguridad].[tbUsuarios] u
INNER JOIN [Refugio].[tbEmpleados] e ON u.Emp_Id = e.emp_Id
INNER JOIN [General].[tbPersonas] p ON e.per_Id = p.per_Id
INNER JOIN [Seguridad].[tbRolesUsuarios] ru ON u.usu_Id = ru.usu_Id
INNER JOIN [Seguridad].[tbRoles] r ON ru.rol_Id = r.Rol_Id
INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON r.Rol_Id = rmp.rol_Id
INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
LEFT JOIN [Seguridad].[tbRolModuloPermisos] rmpe ON r.Rol_Id = rmpe.Rol_Id AND m.Mod_Id = rmpe.Mod_Id
LEFT JOIN [Seguridad].[tbPermisos] pe ON rmpe.Per_Id = pe.Per_Id
WHERE u.Usu_EsActivo = 1 
    AND ISNULL(u.Usu_Suspendido, 0) = 0
    AND ISNULL(u.Usu_EsEliminado, 0) = 0
    AND r.Rol_EsActivo = 1
    AND m.Mod_EsActivo = 1
    AND mp.modpt_EsActivo = 1;
GO

-- Vista: Resumen de roles y sus pantallas
CREATE OR ALTER VIEW [Seguridad].[vw_RolesPantallas]
AS
SELECT 
    r.Rol_Id,
    r.Rol_Descripcion,
    COUNT(DISTINCT rmp.modpt_Id) AS TotalPantallas,
    STRING_AGG(mp.modpt_Descripcion, ', ') WITHIN GROUP (ORDER BY mp.modpt_Descripcion) AS Pantallas
FROM [Seguridad].[tbRoles] r
LEFT JOIN [Seguridad].[tbRolModulosPantallas] rmp ON r.Rol_Id = rmp.rol_Id
LEFT JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
WHERE r.Rol_EsActivo = 1
GROUP BY r.Rol_Id, r.Rol_Descripcion;
GO

-- =============================================
-- PASO 6: DATOS DE EJEMPLO
-- =============================================

-- Insertar módulos de ejemplo para PetsHome
DECLARE @comp_Admin INT = (SELECT TOP 1 comp_Id FROM [Seguridad].[tbComponentes] WHERE comp_Descripcion = 'Portal Administrativo');

IF NOT EXISTS (SELECT * FROM [Seguridad].[tbModulos] WHERE Mod_Nombre = 'Gestión de Mascotas')
BEGIN
    INSERT INTO [Seguridad].[tbModulos] (Mod_Nombre, Mod_Descripcion, Mod_Icono, Mod_Orden, comp_Id)
    VALUES 
        ('Gestión de Mascotas', 'Administración de mascotas del refugio', 'fa-paw', 1, @comp_Admin),
        ('Gestión de Adopciones', 'Proceso de adopción de mascotas', 'fa-heart', 2, @comp_Admin),
        ('Gestión de Voluntarios', 'Administración de voluntarios', 'fa-users', 3, @comp_Admin),
        ('Inventario', 'Control de inventario del refugio', 'fa-boxes', 4, @comp_Admin),
        ('Reportes', 'Reportes y estadísticas', 'fa-chart-bar', 5, @comp_Admin),
        ('Configuración', 'Configuración del sistema', 'fa-cog', 6, @comp_Admin);
END
GO

-- Insertar pantallas de ejemplo
DECLARE @mod_Mascotas INT = (SELECT TOP 1 Mod_Id FROM [Seguridad].[tbModulos] WHERE Mod_Nombre = 'Gestión de Mascotas');
DECLARE @mod_Adopciones INT = (SELECT TOP 1 Mod_Id FROM [Seguridad].[tbModulos] WHERE Mod_Nombre = 'Gestión de Adopciones');
DECLARE @mod_Config INT = (SELECT TOP 1 Mod_Id FROM [Seguridad].[tbModulos] WHERE Mod_Nombre = 'Configuración');

IF @mod_Mascotas IS NOT NULL AND NOT EXISTS (SELECT * FROM [Seguridad].[tbModulosPantallas] WHERE mod_Id = @mod_Mascotas)
BEGIN
    INSERT INTO [Seguridad].[tbModulosPantallas] (mod_Id, modpt_Descripcion, modpt_Url, modpt_Icono, modpt_Orden)
    VALUES 
        (@mod_Mascotas, 'Listado de Mascotas', '/mascotas', 'fa-list', 1),
        (@mod_Mascotas, 'Registrar Mascota', '/mascotas/crear', 'fa-plus', 2),
        (@mod_Mascotas, 'Historial Médico', '/mascotas/historial-medico', 'fa-notes-medical', 3),
        (@mod_Mascotas, 'Vacunación', '/mascotas/vacunacion', 'fa-syringe', 4);
END

IF @mod_Adopciones IS NOT NULL AND NOT EXISTS (SELECT * FROM [Seguridad].[tbModulosPantallas] WHERE mod_Id = @mod_Adopciones)
BEGIN
    INSERT INTO [Seguridad].[tbModulosPantallas] (mod_Id, modpt_Descripcion, modpt_Url, modpt_Icono, modpt_Orden)
    VALUES 
        (@mod_Adopciones, 'Solicitudes de Adopción', '/adopciones/solicitudes', 'fa-file-alt', 1),
        (@mod_Adopciones, 'Proceso de Adopción', '/adopciones/proceso', 'fa-tasks', 2),
        (@mod_Adopciones, 'Historial de Adopciones', '/adopciones/historial', 'fa-history', 3);
END

IF @mod_Config IS NOT NULL AND NOT EXISTS (SELECT * FROM [Seguridad].[tbModulosPantallas] WHERE mod_Id = @mod_Config)
BEGIN
    INSERT INTO [Seguridad].[tbModulosPantallas] (mod_Id, modpt_Descripcion, modpt_Url, modpt_Icono, modpt_Orden)
    VALUES 
        (@mod_Config, 'Usuarios', '/configuracion/usuarios', 'fa-users-cog', 1),
        (@mod_Config, 'Roles y Permisos', '/configuracion/roles', 'fa-user-shield', 2),
        (@mod_Config, 'Parámetros del Sistema', '/configuracion/parametros', 'fa-sliders-h', 3);
END
GO

-- =============================================
-- PASO 7: TRIGGERS PARA AUDITORÍA
-- =============================================

-- Trigger para auditar cambios en usuarios
CREATE OR ALTER TRIGGER [Seguridad].[TRG_AuditarCambiosUsuarios]
ON [Seguridad].[tbUsuarios]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @usu_Id INT;
    DECLARE @detalles NVARCHAR(MAX) = '';
    
    -- Para cada usuario modificado
    DECLARE cur CURSOR FOR
    SELECT i.usu_Id
    FROM inserted i
    INNER JOIN deleted d ON i.usu_Id = d.usu_Id;
    
    OPEN cur;
    FETCH NEXT FROM cur INTO @usu_Id;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Construir detalles de cambios
        SELECT @detalles = 
            CASE WHEN i.Usu_Nombre != d.Usu_Nombre 
                 THEN @detalles + 'Nombre de usuario cambiado de ' + d.Usu_Nombre + ' a ' + i.Usu_Nombre + '; '
                 ELSE @detalles END +
            CASE WHEN i.Usu_EsActivo != d.Usu_EsActivo 
                 THEN @detalles + 'Estado activo cambiado a ' + CASE i.Usu_EsActivo WHEN 1 THEN 'Activo' ELSE 'Inactivo' END + '; '
                 ELSE @detalles END +
            CASE WHEN ISNULL(i.Usu_Suspendido,0) != ISNULL(d.Usu_Suspendido,0)
                 THEN @detalles + 'Estado suspendido cambiado a ' + CASE ISNULL(i.Usu_Suspendido,0) WHEN 1 THEN 'Suspendido' ELSE 'No suspendido' END + '; '
                 ELSE @detalles END
        FROM inserted i
        INNER JOIN deleted d ON i.usu_Id = d.usu_Id
        WHERE i.usu_Id = @usu_Id;
        
        -- Si hay cambios, registrar evento
        IF LEN(@detalles) > 0
        BEGIN
            INSERT INTO [Seguridad].[tbRegistroEventos]
            (Tpevt_Id, Evt_Usu_Id, Evt_Detalles, Evt_FechaCreacion)
            VALUES (6, @usu_Id, LEFT(@detalles, LEN(@detalles) - 2), GETDATE());
        END
        
        SET @detalles = '';
        FETCH NEXT FROM cur INTO @usu_Id;
    END
    
    CLOSE cur;
    DEALLOCATE cur;
END
GO

-- =============================================
-- PASO 8: FUNCIÓN PARA VERIFICAR PERMISOS
-- =============================================

-- Función para verificar si un usuario tiene acceso a una pantalla
CREATE OR ALTER FUNCTION [Seguridad].[FN_UsuarioTieneAccesoPantalla]
(
    @usu_Id INT,
    @modpt_Id INT
)
RETURNS BIT
AS
BEGIN
    DECLARE @tieneAcceso BIT = 0;
    
    IF EXISTS (
        SELECT 1
        FROM [Seguridad].[tbRolesUsuarios] ru
        INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
        INNER JOIN [Seguridad].[tbRoles] r ON ru.rol_Id = r.Rol_Id
        WHERE ru.usu_Id = @usu_Id 
            AND rmp.modpt_Id = @modpt_Id
            AND r.Rol_EsActivo = 1
    )
    BEGIN
        SET @tieneAcceso = 1;
    END
    
    RETURN @tieneAcceso;
END
GO

-- =============================================
-- PASO 9: PROCEDIMIENTO PARA MIGRAR DATOS EXISTENTES
-- =============================================

-- SP: Migrar permisos existentes al nuevo modelo
CREATE OR ALTER PROCEDURE [Seguridad].[PR_MigrarPermisosExistentes]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- 1. Migrar relación directa Rol-Usuario (si no existe ya la tabla RolesUsuarios)
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Seguridad].[tbUsuarios]') AND name = 'Rol_Id')
        BEGIN
            INSERT INTO [Seguridad].[tbRolesUsuarios] (rol_Id, usu_Id)
            SELECT DISTINCT Rol_Id, usu_Id
            FROM [Seguridad].[tbUsuarios]
            WHERE Rol_Id IS NOT NULL
                AND NOT EXISTS (
                    SELECT 1 FROM [Seguridad].[tbRolesUsuarios] ru 
                    WHERE ru.rol_Id = [Seguridad].[tbUsuarios].Rol_Id 
                        AND ru.usu_Id = [Seguridad].[tbUsuarios].usu_Id
                );
            
            PRINT 'Roles de usuarios migrados exitosamente';
        END
        
        -- 2. Limpiar tablas obsoletas si existen
        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Seguridad].[tbRolModulos]'))
        BEGIN
            -- Migrar datos de RolModulos a RolModulosPantallas si es necesario
            PRINT 'Tabla tbRolModulos encontrada - considerar migración manual si tiene datos importantes';
        END
        
        COMMIT TRANSACTION;
        PRINT 'Migración completada exitosamente';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Error en la migración: ' + ERROR_MESSAGE();
        THROW;
    END CATCH
END
GO

-- =============================================
-- PASO 10: PROCEDIMIENTO DE INICIALIZACIÓN
-- =============================================

-- SP: Crear rol de administrador con todos los permisos
CREATE OR ALTER PROCEDURE [Seguridad].[PR_CrearRolAdministrador]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @rol_Id INT;
    
    -- Crear o obtener rol de administrador
    IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRoles] WHERE Rol_Descripcion = 'Administrador')
    BEGIN
        INSERT INTO [Seguridad].[tbRoles] (Rol_Descripcion, Rol_EsActivo, Rol_EsEliminado)
        VALUES ('Administrador', 1, 0);
        
        SET @rol_Id = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        SELECT @rol_Id = Rol_Id FROM [Seguridad].[tbRoles] WHERE Rol_Descripcion = 'Administrador';
    END
    
    -- Asignar todas las pantallas al rol administrador
    INSERT INTO [Seguridad].[tbRolModulosPantallas] (modpt_Id, rol_Id)
    SELECT mp.modpt_Id, @rol_Id
    FROM [Seguridad].[tbModulosPantallas] mp
    WHERE NOT EXISTS (
        SELECT 1 FROM [Seguridad].[tbRolModulosPantallas] rmp 
        WHERE rmp.modpt_Id = mp.modpt_Id AND rmp.rol_Id = @rol_Id
    );
    
    -- Asignar todos los permisos al rol administrador
    INSERT INTO [Seguridad].[tbRolModuloPermisos] (Rol_Id, Mod_Id, Per_Id)
    SELECT @rol_Id, m.Mod_Id, p.Per_Id
    FROM [Seguridad].[tbModulos] m
    CROSS JOIN [Seguridad].[tbPermisos] p
    WHERE NOT EXISTS (
        SELECT 1 FROM [Seguridad].[tbRolModuloPermisos] rmp 
        WHERE rmp.Rol_Id = @rol_Id AND rmp.Mod_Id = m.Mod_Id AND rmp.Per_Id = p.Per_Id
    );
    
    PRINT 'Rol Administrador configurado con todos los permisos';
END
GO

-- Ejecutar inicialización
EXEC [Seguridad].[PR_CrearRolAdministrador];
GO

PRINT '================================================';
PRINT 'Script de actualización completado exitosamente';
PRINT '================================================';
PRINT '';
PRINT 'Próximos pasos recomendados:';
PRINT '1. Ejecutar SP [Seguridad].[PR_MigrarPermisosExistentes] si tienes datos existentes';
PRINT '2. Crear pantallas específicas para tu sistema en tbModulosPantallas';
PRINT '3. Asignar pantallas a roles usando tbRolModulosPantallas';
PRINT '4. Asignar roles a usuarios usando tbRolesUsuarios';
PRINT '5. Actualizar tu aplicación para usar los nuevos SPs de login y permisos';
PRINT '';