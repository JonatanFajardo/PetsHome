-- =============================================
-- Procedimientos Almacenados Complementarios para el Sistema de Seguridad
-- Sistema: PetsHome
-- Fecha: 2025-07-24
-- =============================================

USE [petshome-7-24-2025]
GO

-- =============================================
-- PROCEDIMIENTOS PARA COMPONENTES
-- =============================================

-- SP: Listar componentes
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Componentes_List]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        comp_Id,
        comp_Descripcion
    FROM [Seguridad].[tbComponentes]
    ORDER BY comp_Descripcion;
END
GO

-- SP: Insertar componente
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Componentes_Insert]
    @comp_Descripcion NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [Seguridad].[tbComponentes] (comp_Descripcion)
    VALUES (@comp_Descripcion);
    
    SELECT SCOPE_IDENTITY() AS comp_Id;
END
GO

-- SP: Actualizar componente
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Componentes_Update]
    @comp_Id INT,
    @comp_Descripcion NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [Seguridad].[tbComponentes]
    SET comp_Descripcion = @comp_Descripcion
    WHERE comp_Id = @comp_Id;
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- SP: Eliminar componente
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Componentes_Delete]
    @comp_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [Seguridad].[tbComponentes]
    WHERE comp_Id = @comp_Id;
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA MÓDULOS PANTALLAS
-- =============================================

-- SP: Listar pantallas por módulo
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_ModulosPantallas_List]
    @mod_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        mp.modpt_Id,
        mp.mod_Id,
        mp.modpt_Descripcion,
        mp.modpt_Url,
        mp.modpt_Icono,
        mp.modpt_Orden,
        mp.modpt_EsActivo,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        c.comp_Descripcion
    FROM [Seguridad].[tbModulosPantallas] mp
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE (@mod_Id IS NULL OR mp.mod_Id = @mod_Id)
    ORDER BY mp.modpt_Orden, mp.modpt_Descripcion;
END
GO

-- SP: Insertar pantalla
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_ModulosPantallas_Insert]
    @mod_Id INT,
    @modpt_Descripcion NVARCHAR(100),
    @modpt_Url NVARCHAR(200) = NULL,
    @modpt_Icono NVARCHAR(50) = NULL,
    @modpt_Orden INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO [Seguridad].[tbModulosPantallas] 
    (mod_Id, modpt_Descripcion, modpt_Url, modpt_Icono, modpt_Orden)
    VALUES (@mod_Id, @modpt_Descripcion, @modpt_Url, @modpt_Icono, @modpt_Orden);
    
    SELECT SCOPE_IDENTITY() AS modpt_Id;
END
GO

-- SP: Actualizar pantalla
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_ModulosPantallas_Update]
    @modpt_Id INT,
    @mod_Id INT,
    @modpt_Descripcion NVARCHAR(100),
    @modpt_Url NVARCHAR(200) = NULL,
    @modpt_Icono NVARCHAR(50) = NULL,
    @modpt_Orden INT = NULL,
    @modpt_EsActivo BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [Seguridad].[tbModulosPantallas]
    SET mod_Id = @mod_Id,
        modpt_Descripcion = @modpt_Descripcion,
        modpt_Url = @modpt_Url,
        modpt_Icono = @modpt_Icono,
        modpt_Orden = @modpt_Orden,
        modpt_EsActivo = @modpt_EsActivo
    WHERE modpt_Id = @modpt_Id;
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- SP: Eliminar pantalla
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_ModulosPantallas_Delete]
    @modpt_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [Seguridad].[tbModulosPantallas]
    WHERE modpt_Id = @modpt_Id;
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- SP: Obtener pantalla por ID
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_ModulosPantallas_Detail]
    @modpt_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        mp.modpt_Id,
        mp.mod_Id,
        mp.modpt_Descripcion,
        mp.modpt_Url,
        mp.modpt_Icono,
        mp.modpt_Orden,
        mp.modpt_EsActivo,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        c.comp_Descripcion
    FROM [Seguridad].[tbModulosPantallas] mp
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE mp.modpt_Id = @modpt_Id;
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA ROL MÓDULOS PANTALLAS
-- =============================================

-- SP: Listar asignaciones rol-pantallas
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RolModulosPantallas_List]
    @rol_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        rmp.rolpt_Id,
        rmp.modpt_Id,
        rmp.rol_Id,
        rmp.rolpt_FechaAsignacion,
        mp.modpt_Descripcion,
        mp.modpt_Url,
        m.Mod_Nombre,
        r.Rol_Descripcion
    FROM [Seguridad].[tbRolModulosPantallas] rmp
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbRoles] r ON rmp.rol_Id = r.Rol_Id
    WHERE (@rol_Id IS NULL OR rmp.rol_Id = @rol_Id)
    ORDER BY r.Rol_Descripcion, m.Mod_Nombre, mp.modpt_Descripcion;
END
GO

-- SP: Asignar pantalla a rol
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RolModulosPantallas_Insert]
    @modpt_Id INT,
    @rol_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Verificar si ya existe la asignación
    IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolModulosPantallas] WHERE modpt_Id = @modpt_Id AND rol_Id = @rol_Id)
    BEGIN
        INSERT INTO [Seguridad].[tbRolModulosPantallas] (modpt_Id, rol_Id)
        VALUES (@modpt_Id, @rol_Id);
        
        SELECT SCOPE_IDENTITY() AS rolpt_Id, 'Pantalla asignada al rol exitosamente' AS Mensaje;
    END
    ELSE
    BEGIN
        SELECT 0 AS rolpt_Id, 'El rol ya tiene acceso a esta pantalla' AS Mensaje;
    END
END
GO

-- SP: Remover pantalla de rol
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RolModulosPantallas_Delete]
    @modpt_Id INT,
    @rol_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM [Seguridad].[tbRolModulosPantallas]
    WHERE modpt_Id = @modpt_Id AND rol_Id = @rol_Id;
    
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA ROLES USUARIOS
-- =============================================

-- SP: Listar roles de usuarios
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RolesUsuarios_List]
    @usu_Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ru.rol_usu_Id,
        ru.rol_Id,
        ru.usu_Id,
        ru.rol_usu_FechaAsignacion,
        r.Rol_Descripcion,
        u.Usu_Nombre,
        p.per_PrimerNombre + ' ' + ISNULL(p.per_SegundoNombre, '') + ' ' + 
        p.per_ApellidoPaterno + ' ' + ISNULL(p.per_ApellidoMaterno, '') AS Emp_NombreCompleto
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRoles] r ON ru.rol_Id = r.Rol_Id
    INNER JOIN [Seguridad].[tbUsuarios] u ON ru.usu_Id = u.usu_Id
    INNER JOIN [Refugio].[tbEmpleados] e ON u.Emp_Id = e.emp_Id
    INNER JOIN [General].[tbPersonas] p ON e.per_Id = p.per_Id
    WHERE (@usu_Id IS NULL OR ru.usu_Id = @usu_Id)
        AND r.Rol_EsActivo = 1
        AND u.Usu_EsActivo = 1
    ORDER BY p.per_PrimerNombre, p.per_ApellidoPaterno, r.Rol_Descripcion;
END
GO

-- =============================================
-- PROCEDIMIENTOS MEJORADOS PARA MENÚS
-- =============================================

-- SP: Obtener menú completo por usuario con nueva estructura
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_MenuUsuarioCompleto_V2]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Componentes accesibles por el usuario
    SELECT DISTINCT
        c.comp_Id,
        c.comp_Descripcion
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    WHERE ru.usu_Id = @usu_Id 
        AND mp.modpt_EsActivo = 1 
        AND m.Mod_EsActivo = 1;
    
    -- Módulos accesibles por el usuario
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
    WHERE ru.usu_Id = @usu_Id 
        AND mp.modpt_EsActivo = 1 
        AND m.Mod_EsActivo = 1
    ORDER BY m.Mod_Orden, m.Mod_Nombre;
    
    -- Pantallas accesibles por el usuario con permisos
    SELECT DISTINCT
        mp.modpt_Id,
        mp.mod_Id,
        mp.modpt_Descripcion,
        mp.modpt_Url,
        mp.modpt_Icono,
        mp.modpt_Orden,
        m.Mod_Nombre,
        m.Mod_Descripcion,
        m.Mod_Icono AS Mod_Icono,
        m.Mod_Orden,
        c.comp_Id,
        c.comp_Descripcion,
        STRING_AGG(p.Per_Nombre, ',') AS Permisos
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN [Seguridad].[tbModulos] m ON mp.mod_Id = m.Mod_Id
    INNER JOIN [Seguridad].[tbComponentes] c ON m.comp_Id = c.comp_Id
    LEFT JOIN [Seguridad].[tbRolModuloPermisos] rmpe ON rmp.rol_Id = rmpe.Rol_Id AND mp.mod_Id = rmpe.Mod_Id
    LEFT JOIN [Seguridad].[tbPermisos] p ON rmpe.Per_Id = p.Per_Id
    WHERE ru.usu_Id = @usu_Id 
        AND mp.modpt_EsActivo = 1 
        AND m.Mod_EsActivo = 1
    GROUP BY mp.modpt_Id, mp.mod_Id, mp.modpt_Descripcion, mp.modpt_Url,
             mp.modpt_Icono, mp.modpt_Orden, m.Mod_Nombre, m.Mod_Descripcion,
             m.Mod_Icono, m.Mod_Orden, c.comp_Id, c.comp_Descripcion
    ORDER BY mp.modpt_Orden, mp.modpt_Descripcion;
END
GO

-- SP: Verificar acceso específico a pantalla
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_VerificarAccesoPantalla]
    @usu_Id INT,
    @modpt_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(*) AS TieneAcceso
    FROM [Seguridad].[tbRolesUsuarios] ru
    INNER JOIN [Seguridad].[tbRolModulosPantallas] rmp ON ru.rol_Id = rmp.rol_Id
    INNER JOIN [Seguridad].[tbModulosPantallas] mp ON rmp.modpt_Id = mp.modpt_Id
    WHERE ru.usu_Id = @usu_Id 
        AND rmp.modpt_Id = @modpt_Id
        AND mp.modpt_EsActivo = 1;
END
GO

-- =============================================
-- PROCEDIMIENTOS PARA GESTIÓN MASIVA DE PERMISOS
-- =============================================

-- SP: Asignar múltiples pantallas a un rol
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_AsignarPantallasRol]
    @rol_Id INT,
    @modpt_Ids NVARCHAR(MAX) -- IDs separados por comas
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Crear tabla temporal con los IDs
    CREATE TABLE #TempPantallas (modpt_Id INT);
    
    SET @SQL = 'INSERT INTO #TempPantallas (modpt_Id) SELECT value FROM STRING_SPLIT(''' + @modpt_Ids + ''', '','')';
    EXEC sp_executesql @SQL;
    
    -- Insertar solo las asignaciones que no existen
    INSERT INTO [Seguridad].[tbRolModulosPantallas] (modpt_Id, rol_Id)
    SELECT tp.modpt_Id, @rol_Id
    FROM #TempPantallas tp
    WHERE NOT EXISTS (
        SELECT 1 FROM [Seguridad].[tbRolModulosPantallas] rmp 
        WHERE rmp.modpt_Id = tp.modpt_Id AND rmp.rol_Id = @rol_Id
    );
    
    SELECT @@ROWCOUNT AS PantallasAsignadas;
    
    DROP TABLE #TempPantallas;
END
GO

-- SP: Remover múltiples pantallas de un rol
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RemoverPantallasRol]
    @rol_Id INT,
    @modpt_Ids NVARCHAR(MAX) -- IDs separados por comas
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Crear tabla temporal con los IDs
    CREATE TABLE #TempPantallas (modpt_Id INT);
    
    SET @SQL = 'INSERT INTO #TempPantallas (modpt_Id) SELECT value FROM STRING_SPLIT(''' + @modpt_Ids + ''', '','')';
    EXEC sp_executesql @SQL;
    
    -- Eliminar las asignaciones
    DELETE rmp
    FROM [Seguridad].[tbRolModulosPantallas] rmp
    INNER JOIN #TempPantallas tp ON rmp.modpt_Id = tp.modpt_Id
    WHERE rmp.rol_Id = @rol_Id;
    
    SELECT @@ROWCOUNT AS PantallasRemovidas;
    
    DROP TABLE #TempPantallas;
END
GO

PRINT '========================================';
PRINT 'Procedimientos complementarios creados exitosamente';
PRINT '========================================';