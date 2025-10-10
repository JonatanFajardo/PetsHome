-- =============================================
-- Stored Procedures para el módulo de Login
-- =============================================
Seguridad.[PR_Seguridad_Usuarios_Login]  'admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918'
-- Procedimiento para login de usuario
CREATE OR ALTER PROCEDURE Seguridad.[PR_Seguridad_Usuarios_Login] 
    @Usu_Nombre NVARCHAR(150),
    @Con_Hash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.usu_Id,
        u.Emp_Id,
        u.Usu_Nombre,
        p.per_PrimerNombre + ' ' + p.per_SegundoNombre Emp_Nombres,
        p.per_ApellidoPaterno Emp_Apellidos,
        u.Rol_Id,
        r.Rol_Descripcion,
        -- r.Rol_Pantallas,
        u.Usu_EsActivo,
        u.Usu_Suspendido
        --c.Con_Hash,
        --c.Con_Salt
    FROM seguridad.tbUsuarios u
    INNER JOIN refugio.tbEmpleados e ON u.Emp_Id = e.Emp_Id
    INNER JOIN seguridad.tbRoles r ON u.Rol_Id = r.Rol_Id
	INNER JOIN General.tbPersonas p ON e.per_Id = p.per_Id 
    WHERE u.Usu_Nombre = @Usu_Nombre 
        AND u.Usu_EsActivo = 1 
        AND u.Usu_Suspendido = 0
        AND u.Usu_EsEliminado = 0
        AND u.Usu_PasswordHash = @Con_Hash
        --AND c.Con_EsActivo = 1;
END
GO


-- Procedimiento para obtener detalle de usuario
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Usuarios_Detail]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.usu_Id,
        u.Emp_Id,
        u.Usu_Nombre,
        p.per_PrimerNombre + ' ' + p.per_SegundoNombre Emp_Nombres,
        p.per_ApellidoPaterno Emp_Apellidos,
        u.Rol_Id,
        r.Rol_Descripcion,
        -- r.Rol_Pantallas,
        u.Usu_Ip,
        u.Usu_EsActivo,
        u.Usu_Suspendido,
        u.Usu_FechaCreacion,
        u.Usu_fechaModificacion
    FROM seguridad.tbUsuarios u
    INNER JOIN refugio.tbEmpleados e ON u.Emp_Id = e.Emp_Id
    INNER JOIN General.tbPersonas p ON e.per_Id = p.per_Id 
    INNER JOIN seguridad.tbRoles r ON u.Rol_Id = r.Rol_Id
    WHERE u.usu_Id = @usu_Id
        AND u.Usu_EsEliminado = 0;
END
GO

-- Procedimiento para listar roles
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Roles_List]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Rol_Id,
        Rol_Descripcion,
        -- Rol_Pantallas,
        Rol_EsActivo,
        Rol_FechaCreacion
    FROM seguridad.tbRoles
    WHERE Rol_EsEliminado = 0
        AND Rol_EsActivo = 1
    ORDER BY Rol_Descripcion;
END
GO

-- Procedimiento para crear contraseña
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Contrasenas_Insert]
    @Con_Hash NVARCHAR(255),
    @Con_Salt NVARCHAR(100),
    @Usu_UsuarioCreacion INT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO tbContrasenas (
        Con_Hash,
        Con_Salt,
        Con_FechaCreacion,
        Con_EsActivo,
        Con_EsEliminado,
        Usu_UsuarioCreacion
    )
    VALUES (
        @Con_Hash,
        @Con_Salt,
        GETDATE(),
        1,
        0,
        @Usu_UsuarioCreacion
    );
    
    SELECT SCOPE_IDENTITY() AS Con_Id;
END
GO

-- Procedimiento para crear usuario (actualizado para usar Usu_PasswordHash)
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Usuarios_Insert]
    @Emp_Id INT,
    @Usu_Nombre NVARCHAR(150),
    @Usu_PasswordHash NVARCHAR(255),
    @Rol_Id INT,
    @Usu_Ip NVARCHAR(45),
    @Usu_UsuarioCreacion INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si el usuario ya existe
        IF EXISTS (SELECT 1 FROM seguridad.tbUsuarios WHERE Usu_Nombre = @Usu_Nombre AND Usu_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeErrorInsert, 'El nombre de usuario ya existe' AS MsgErrorInsert;
            RETURN;
        END
        
        -- Verificar si el empleado ya tiene usuario
        IF EXISTS (SELECT 1 FROM seguridad.tbUsuarios WHERE Emp_Id = @Emp_Id AND Usu_EsEliminado = 0)
        BEGIN
            SELECT -2 AS CodeErrorInsert, 'El empleado ya tiene un usuario asignado' AS MsgErrorInsert;
            RETURN;
        END
        
        INSERT INTO seguridad.tbUsuarios (
            Emp_Id,
            Usu_Nombre,
            Usu_PasswordHash,
            Rol_Id,
            Usu_Ip,
            Usu_EsActivo,
            Usu_Suspendido,
            Usu_EsEliminado,
            Usu_FechaCreacion,
            Usu_UsuarioCreacion
        )
        VALUES (
            @Emp_Id,
            @Usu_Nombre,
            @Usu_PasswordHash,
            @Rol_Id,
            @Usu_Ip,
            1,
            0,
            0,
            GETDATE(),
            @Usu_UsuarioCreacion
        );
        
        SELECT SCOPE_IDENTITY() AS usu_Id, 0 AS CodeErrorInsert, '' AS MsgErrorInsert;
    END TRY
    BEGIN CATCH
        SELECT -3 AS CodeErrorInsert, ERROR_MESSAGE() AS MsgErrorInsert;
    END CATCH
END
GO

-- Procedimiento para actualizar último acceso del usuario
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Usuarios_UpdateLastAccess]
    @usu_Id INT,
    @Usu_Ip NVARCHAR(45)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE seguridad.tbUsuarios 
    SET Usu_Ip = @Usu_Ip,
        Usu_fechaModificacion = GETDATE()
    WHERE usu_Id = @usu_Id;
END
GO

-- Procedimiento para cambiar contraseña (actualizado para usar Usu_PasswordHash)
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Usuarios_ChangePassword]
    @usu_Id INT,
    @Usu_PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Actualizar contraseña directamente en tbUsuarios
        UPDATE seguridad.tbUsuarios 
        SET Usu_PasswordHash = @Usu_PasswordHash,
            Usu_fechaModificacion = GETDATE()
        WHERE usu_Id = @usu_Id;
        
        SELECT 1 AS Success, 'Contraseña actualizada correctamente' AS Message;
        
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Usuarios_GetPermissions]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.Mod_Id,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        m.Mod_Icono,
        m.Mod_Url,
        m.Mod_Orden,
        STRING_AGG(p.Per_Nombre, ',') as Permisos
    FROM seguridad.tbUsuarios u
    INNER JOIN seguridad.tbRolModulos rm ON u.Rol_Id = rm.Rol_Id
    INNER JOIN seguridad.tbModulos m ON rm.Mod_Id = m.Mod_Id
    LEFT JOIN seguridad.tbRolModuloPermisos rmp ON u.Rol_Id = rmp.Rol_Id AND m.Mod_Id = rmp.Mod_Id
    LEFT JOIN seguridad.tbPermisos p ON rmp.Per_Id = p.Per_Id
    WHERE u.usu_Id = @usu_Id
        AND u.Usu_EsActivo = 1
        AND m.Mod_EsActivo = 1
    GROUP BY m.Mod_Id, m.Mod_Nombre, m.Mod_Descripcion, m.Mod_Icono, m.Mod_Url, m.Mod_Orden
    ORDER BY m.Mod_Orden;
END
GO

-- Procedimiento para verificar si usuario existe
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Usuarios_Exists]
    @Usu_Nombre NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1) as Existe
    FROM seguridad.tbUsuarios
    WHERE Usu_Nombre = @Usu_Nombre 
        AND Usu_EsEliminado = 0;
END
GO

-- Procedimiento para verificar si empleado tiene usuario
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Empleados_TieneUsuario]
    @Emp_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1) as TieneUsuario
    FROM seguridad.tbUsuarios
    WHERE Emp_Id = @Emp_Id 
        AND Usu_EsEliminado = 0;
END
GO

-- Procedimiento para obtener permisos de rol
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Roles_GetPermissions]
    @Rol_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        m.Mod_Id,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        m.Mod_Icono,
        m.Mod_Url,
        m.Mod_Orden,
        STRING_AGG(p.Per_Nombre, ',') as Permisos
    FROM seguridad.tbRolModulos rm
    INNER JOIN seguridad.tbModulos m ON rm.Mod_Id = m.Mod_Id
    LEFT JOIN seguridad.tbRolModuloPermisos rmp ON rm.Rol_Id = rmp.Rol_Id AND m.Mod_Id = rmp.Mod_Id
    LEFT JOIN seguridad.tbPermisos p ON rmp.Per_Id = p.Per_Id
    WHERE rm.Rol_Id = @Rol_Id
        AND m.Mod_EsActivo = 1
    GROUP BY m.Mod_Id, m.Mod_Nombre, m.Mod_Descripcion, m.Mod_Icono, m.Mod_Url, m.Mod_Orden
    ORDER BY m.Mod_Orden;
END
GO

-- Procedimiento para verificar permiso específico
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_CheckPermission]
    @usu_Id INT,
    @Mod_Nombre NVARCHAR(100),
    @Per_Nombre NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN COUNT(1) > 0 THEN 1 
            ELSE 0 
        END as TienePermiso
    FROM seguridad.tbUsuarios u
    INNER JOIN seguridad.tbRolModulos rm ON u.Rol_Id = rm.Rol_Id
    INNER JOIN seguridad.tbModulos m ON rm.Mod_Id = m.Mod_Id
    INNER JOIN seguridad.tbRolModuloPermisos rmp ON u.Rol_Id = rmp.Rol_Id AND m.Mod_Id = rmp.Mod_Id
    INNER JOIN seguridad.tbPermisos p ON rmp.Per_Id = p.Per_Id
    WHERE u.usu_Id = @usu_Id
        AND m.Mod_Nombre = @Mod_Nombre
        AND p.Per_Nombre = @Per_Nombre
        AND u.Usu_EsActivo = 1
        AND m.Mod_EsActivo = 1;
END
GO