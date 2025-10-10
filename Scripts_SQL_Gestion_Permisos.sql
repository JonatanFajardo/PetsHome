-- =============================================
-- Stored Procedures para Gestión de Permisos
-- =============================================

-- Listar todos los módulos
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Modulos_List]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Mod_Id,
        Mod_Nombre,
        Mod_Descripcion,
        Mod_Icono,
        Mod_Url,
        Mod_Orden,
        Mod_EsActivo,
        Mod_FechaCreacion
    FROM seguridad.tbModulos
    ORDER BY Mod_Orden, Mod_Nombre;
END
GO

-- Listar todos los permisos
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Permisos_List]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Per_Id,
        Per_Nombre,
        Per_Descripcion,
        Per_EsActivo
    FROM seguridad.tbPermisos
    WHERE Per_EsActivo = 1
    ORDER BY Per_Nombre;
END
GO

-- Crear módulo
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Modulos_Insert]
    @Mod_Nombre NVARCHAR(50),
    @Mod_Descripcion NVARCHAR(200),
    @Mod_Icono NVARCHAR(50),
    @Mod_Url NVARCHAR(200),
    @Mod_Orden INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si el módulo ya existe
        IF EXISTS (SELECT 1 FROM seguridad.tbModulos WHERE Mod_Nombre = @Mod_Nombre)
        BEGIN
            SELECT -1 AS CodeErrorInsert, 'El nombre del módulo ya existe' AS MsgErrorInsert;
            RETURN;
        END
        
        INSERT INTO seguridad.tbModulos (
            Mod_Nombre,
            Mod_Descripcion,
            Mod_Icono,
            Mod_Url,
            Mod_Orden,
            Mod_EsActivo,
            Mod_FechaCreacion
        )
        VALUES (
            @Mod_Nombre,
            @Mod_Descripcion,
            @Mod_Icono,
            @Mod_Url,
            @Mod_Orden,
            1,
            GETDATE()
        );
        
        SELECT SCOPE_IDENTITY() AS Mod_Id, 0 AS CodeErrorInsert, '' AS MsgErrorInsert;
    END TRY
    BEGIN CATCH
        SELECT -2 AS CodeErrorInsert, ERROR_MESSAGE() AS MsgErrorInsert;
    END CATCH
END
GO

-- Actualizar módulo
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Modulos_Update]
    @Mod_Id INT,
    @Mod_Nombre NVARCHAR(50),
    @Mod_Descripcion NVARCHAR(200),
    @Mod_Icono NVARCHAR(50),
    @Mod_Url NVARCHAR(200),
    @Mod_Orden INT,
    @Mod_EsActivo BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        UPDATE seguridad.tbModulos 
        SET Mod_Nombre = @Mod_Nombre,
            Mod_Descripcion = @Mod_Descripcion,
            Mod_Icono = @Mod_Icono,
            Mod_Url = @Mod_Url,
            Mod_Orden = @Mod_Orden,
            Mod_EsActivo = @Mod_EsActivo
        WHERE Mod_Id = @Mod_Id;
        
        SELECT 1 AS Success, 'Módulo actualizado correctamente' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

-- Crear permiso
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_Permisos_Insert]
    @Per_Nombre NVARCHAR(50),
    @Per_Descripcion NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si el permiso ya existe
        IF EXISTS (SELECT 1 FROM seguridad.tbPermisos WHERE Per_Nombre = @Per_Nombre)
        BEGIN
            SELECT -1 AS CodeErrorInsert, 'El nombre del permiso ya existe' AS MsgErrorInsert;
            RETURN;
        END
        
        INSERT INTO seguridad.tbPermisos (
            Per_Nombre,
            Per_Descripcion,
            Per_EsActivo
        )
        VALUES (
            @Per_Nombre,
            @Per_Descripcion,
            1
        );
        
        SELECT SCOPE_IDENTITY() AS Per_Id, 0 AS CodeErrorInsert, '' AS MsgErrorInsert;
    END TRY
    BEGIN CATCH
        SELECT -2 AS CodeErrorInsert, ERROR_MESSAGE() AS MsgErrorInsert;
    END CATCH
END
GO

-- Obtener permisos completos de un rol
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_RolModulosCompleto_List]
    @Rol_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        r.Rol_Id,
        r.Rol_Descripcion,
        m.Mod_Id,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        m.Mod_Icono,
        m.Mod_Url,
        m.Mod_Orden,
        CASE 
            WHEN rm.Rol_Id IS NOT NULL THEN 1 
            ELSE 0 
        END as TieneAcceso,
        ISNULL(STRING_AGG(p.Per_Nombre, ','), '') as Permisos
    FROM seguridad.tbRoles r
    CROSS JOIN seguridad.tbModulos m
    LEFT JOIN seguridad.tbRolModulos rm ON r.Rol_Id = rm.Rol_Id AND m.Mod_Id = rm.Mod_Id
    LEFT JOIN seguridad.tbRolModuloPermisos rmp ON r.Rol_Id = rmp.Rol_Id AND m.Mod_Id = rmp.Mod_Id
    LEFT JOIN seguridad.tbPermisos p ON rmp.Per_Id = p.Per_Id
    WHERE r.Rol_Id = @Rol_Id
        AND r.Rol_EsActivo = 1
        AND m.Mod_EsActivo = 1
    GROUP BY r.Rol_Id, r.Rol_Descripcion, m.Mod_Id, m.Mod_Nombre, m.Mod_Descripcion, 
             m.Mod_Icono, m.Mod_Url, m.Mod_Orden, rm.Rol_Id
    ORDER BY m.Mod_Orden, m.Mod_Nombre;
END
GO

-- Asignar módulo a rol
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_RolModulos_Insert]
    @Rol_Id INT,
    @Mod_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar si ya existe la asignación
        IF EXISTS (SELECT 1 FROM seguridad.tbRolModulos WHERE Rol_Id = @Rol_Id AND Mod_Id = @Mod_Id)
        BEGIN
            SELECT 1 AS Success, 'La asignación ya existe' AS Message;
            RETURN;
        END
        
        INSERT INTO seguridad.tbRolModulos (Rol_Id, Mod_Id, RolMod_FechaAsignacion)
        VALUES (@Rol_Id, @Mod_Id, GETDATE());
        
        SELECT 1 AS Success, 'Módulo asignado correctamente' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

-- Remover módulo de rol
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_RolModulos_Delete]
    @Rol_Id INT,
    @Mod_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Primero eliminar todos los permisos específicos del módulo
        DELETE FROM seguridad.tbRolModuloPermisos 
        WHERE Rol_Id = @Rol_Id AND Mod_Id = @Mod_Id;
        
        -- Luego eliminar la asignación del módulo
        DELETE FROM seguridad.tbRolModulos 
        WHERE Rol_Id = @Rol_Id AND Mod_Id = @Mod_Id;
        
        SELECT 1 AS Success, 'Módulo removido correctamente' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

-- Asignar permiso específico a rol-módulo
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_RolModuloPermisos_Insert]
    @Rol_Id INT,
    @Mod_Id INT,
    @Per_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Verificar que el rol tenga acceso al módulo
        IF NOT EXISTS (SELECT 1 FROM seguridad.tbRolModulos WHERE Rol_Id = @Rol_Id AND Mod_Id = @Mod_Id)
        BEGIN
            SELECT 0 AS Success, 'El rol no tiene acceso a este módulo' AS Message;
            RETURN;
        END
        
        -- Verificar si ya existe el permiso
        IF EXISTS (SELECT 1 FROM seguridad.tbRolModuloPermisos WHERE Rol_Id = @Rol_Id AND Mod_Id = @Mod_Id AND Per_Id = @Per_Id)
        BEGIN
            SELECT 1 AS Success, 'El permiso ya está asignado' AS Message;
            RETURN;
        END
        
        INSERT INTO seguridad.tbRolModuloPermisos (Rol_Id, Mod_Id, Per_Id, RolModPer_FechaAsignacion)
        VALUES (@Rol_Id, @Mod_Id, @Per_Id, GETDATE());
        
        SELECT 1 AS Success, 'Permiso asignado correctamente' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

-- Remover permiso específico de rol-módulo
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_RolModuloPermisos_Delete]
    @Rol_Id INT,
    @Mod_Id INT,
    @Per_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DELETE FROM seguridad.tbRolModuloPermisos 
        WHERE Rol_Id = @Rol_Id AND Mod_Id = @Mod_Id AND Per_Id = @Per_Id;
        
        SELECT 1 AS Success, 'Permiso removido correctamente' AS Message;
    END TRY
    BEGIN CATCH
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END
GO

-- Obtener menú para usuario específico
CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_MenuUsuario_List]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.usu_Id,
        u.Rol_Id,
        r.Rol_Descripcion,
        m.Mod_Id,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        m.Mod_Icono,
        m.Mod_Url,
        m.Mod_Orden,
        ISNULL(STRING_AGG(p.Per_Nombre, ','), '') as Permisos
    FROM seguridad.tbUsuarios u
    INNER JOIN seguridad.tbRoles r ON u.Rol_Id = r.Rol_Id
    INNER JOIN seguridad.tbRolModulos rm ON u.Rol_Id = rm.Rol_Id
    INNER JOIN seguridad.tbModulos m ON rm.Mod_Id = m.Mod_Id
    LEFT JOIN seguridad.tbRolModuloPermisos rmp ON u.Rol_Id = rmp.Rol_Id AND m.Mod_Id = rmp.Mod_Id
    LEFT JOIN seguridad.tbPermisos p ON rmp.Per_Id = p.Per_Id
    WHERE u.usu_Id = @usu_Id
        AND u.Usu_EsActivo = 1
        AND u.Usu_EsEliminado = 0
        AND r.Rol_EsActivo = 1
        AND m.Mod_EsActivo = 1
    GROUP BY u.usu_Id, u.Rol_Id, r.Rol_Descripcion, m.Mod_Id, m.Mod_Nombre, 
             m.Mod_Descripcion, m.Mod_Icono, m.Mod_Url, m.Mod_Orden
    ORDER BY m.Mod_Orden, m.Mod_Nombre;
END
GO