-- ============================================================
-- Script: Sistema de Seguridad - Pantallas y Roles
-- Descripcion: Crea tablas de pantallas y roles-pantallas,
--              stored procedures y datos iniciales
-- Base de datos: PETSHOMEDB
-- ============================================================

USE PETSHOMEDB
GO

-- ============================================================
-- 1. CREAR TABLA tbPantallas
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbPantallas' AND schema_id = SCHEMA_ID('Seguridad'))
BEGIN
    CREATE TABLE [Seguridad].[tbPantallas](
        pan_Id INT IDENTITY(1,1) NOT NULL,
        pan_Descripcion VARCHAR(100) NOT NULL,
        pan_Grupo VARCHAR(50) NOT NULL,
        pan_EsActivo BIT NOT NULL DEFAULT 1,
        CONSTRAINT PK_tbPantallas PRIMARY KEY (pan_Id)
    )
END
GO

-- ============================================================
-- 2. CREAR TABLA tbRolesPantallas
-- ============================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tbRolesPantallas' AND schema_id = SCHEMA_ID('Seguridad'))
BEGIN
    CREATE TABLE [Seguridad].[tbRolesPantallas](
        ropan_Id INT IDENTITY(1,1) NOT NULL,
        rol_Id INT NOT NULL,
        pan_Id INT NOT NULL,
        ropan_EsActivo BIT NOT NULL DEFAULT 1,
        CONSTRAINT PK_tbRolesPantallas PRIMARY KEY (ropan_Id),
        CONSTRAINT FK_RolesPantallas_Roles FOREIGN KEY (rol_Id) REFERENCES [Seguridad].[tbRoles](rol_Id),
        CONSTRAINT FK_RolesPantallas_Pantallas FOREIGN KEY (pan_Id) REFERENCES [Seguridad].[tbPantallas](pan_Id)
    )
END
GO

-- ============================================================
-- 3. INSERT PANTALLAS INICIALES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbPantallas])
BEGIN
    INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo) VALUES
    ('Home', 'Home'),
    ('Listado de empleados', 'Cuenta'),
    ('Listado de voluntarios', 'Cuenta'),
    ('Listado de recepciones', 'Inventario'),
    ('Listado de items', 'Inventario'),
    ('Listado de cargos', 'Administracion'),
    ('Listado de refugios', 'Administracion'),
    ('Listado de localidades', 'Administracion'),
    ('Listado de categorias', 'Administracion'),
    ('Listado de eventos', 'Administracion'),
    ('Listado de mascotas', 'Adopcion'),
    ('Listado de adopciones', 'Adopcion'),
    ('Listado de solicitudes', 'Adopcion'),
    ('Listado de citas medicas', 'Medicamento'),
    ('Listado de recetas', 'Medicamento'),
    ('Listado de tratamientos', 'Medicamento'),
    ('Listado de vacunas', 'Medicamento'),
    ('Listado de procedencias', 'Medicamento'),
    ('Listado de razas', 'Medicamento'),
    ('Listado de tipos de consulta', 'Medicamento'),
    ('Listado de gravedades', 'Medicamento'),
    ('Listado de tipos de medicamento', 'Medicamento'),
    ('Listado de vias de administracion', 'Medicamento'),
    ('Listado de tipos de parasito', 'Medicamento'),
    ('Listado de tipos de esterilizacion', 'Medicamento'),
    ('Listado de usuarios', 'Seguridad'),
    ('Listado de roles', 'Seguridad')
END
GO

-- ============================================================
-- 4. STORED PROCEDURES - PANTALLAS
-- ============================================================

-- 4.1 Listar todas las pantallas
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Pantallas_List]
AS
BEGIN
    SELECT  pan_Id,
            pan_Descripcion,
            pan_Grupo
    FROM [Seguridad].[tbPantallas]
    WHERE pan_EsActivo = 1
    ORDER BY pan_Grupo, pan_Descripcion
END
GO

-- 4.2 Pantallas asignadas a un rol (solo IDs)
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Pantallas_ByRol]
    @rol_Id INT
AS
BEGIN
    SELECT  rp.pan_Id
    FROM [Seguridad].[tbRolesPantallas] rp
    WHERE rp.rol_Id = @rol_Id
    AND rp.ropan_EsActivo = 1
END
GO

-- 4.3 Obtener nombres de pantallas por rol (para login)
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Pantallas_NombresByRol]
    @rol_Id INT
AS
BEGIN
    SELECT  p.pan_Descripcion
    FROM [Seguridad].[tbRolesPantallas] rp
    INNER JOIN [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
    WHERE rp.rol_Id = @rol_Id
    AND rp.ropan_EsActivo = 1
    AND p.pan_EsActivo = 1
END
GO

-- 4.4 Guardar pantallas de un rol (borra y reinserta)
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_RolesPantallas_Save]
    @rol_Id INT,
    @pantallaIds VARCHAR(MAX) -- IDs separados por coma: "1,2,3,5"
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

        -- Desactivar todas las pantallas del rol
        UPDATE [Seguridad].[tbRolesPantallas]
        SET ropan_EsActivo = 0
        WHERE rol_Id = @rol_Id

        -- Insertar/reactivar las nuevas
        IF @pantallaIds IS NOT NULL AND @pantallaIds != ''
        BEGIN
            -- Usar tabla temporal para parsear los IDs
            DECLARE @PantallaTable TABLE (pan_Id INT)

            INSERT INTO @PantallaTable (pan_Id)
            SELECT CAST(value AS INT)
            FROM STRING_SPLIT(@pantallaIds, ',')
            WHERE RTRIM(LTRIM(value)) != ''

            -- Para cada pantalla, insertar o reactivar
            MERGE [Seguridad].[tbRolesPantallas] AS target
            USING @PantallaTable AS source
            ON target.rol_Id = @rol_Id AND target.pan_Id = source.pan_Id
            WHEN MATCHED THEN
                UPDATE SET ropan_EsActivo = 1
            WHEN NOT MATCHED THEN
                INSERT (rol_Id, pan_Id, ropan_EsActivo)
                VALUES (@rol_Id, source.pan_Id, 1);
        END

        COMMIT TRANSACTION
        SELECT 1 AS Resultado
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION
        SELECT 0 AS Resultado
    END CATCH
END
GO

-- ============================================================
-- 5. STORED PROCEDURES - ROLES
-- ============================================================

-- 5.1 Listar roles
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY r.rol_Id ASC) AS Fila,
            r.rol_Id,
            r.rol_Descripcion,
            r.rol_Estado,
            (SELECT COUNT(*) FROM [Seguridad].[tbRolesPantallas] rp
             WHERE rp.rol_Id = r.rol_Id AND rp.ropan_EsActivo = 1) AS CantidadPantallas
    FROM [Seguridad].[tbRoles] r
    WHERE r.rol_EsEliminado != 1
    ORDER BY r.rol_Id
END
GO

-- 5.2 Buscar rol por ID
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Find]
    @rol_Id INT
AS
BEGIN
    SELECT  r.rol_Id,
            r.rol_Descripcion,
            r.rol_Estado
    FROM [Seguridad].[tbRoles] r
    WHERE r.rol_EsEliminado != 1
    AND r.rol_Id = @rol_Id
END
GO

-- 5.3 Insertar rol
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Insert]
    @rol_Descripcion VARCHAR(100),
    @rol_Estado BIT,
    @rol_UsuarioCrea INT
AS
BEGIN
    INSERT INTO [Seguridad].[tbRoles]
    (rol_Descripcion, rol_Estado, rol_UsuarioCrea, rol_FechaCrea, rol_EsEliminado)
    VALUES
    (@rol_Descripcion, @rol_Estado, @rol_UsuarioCrea, GETDATE(), 0)

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 5.4 Actualizar rol
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Update]
    @rol_Id INT,
    @rol_Descripcion VARCHAR(100),
    @rol_Estado BIT,
    @rol_UsuarioModifica INT
AS
BEGIN
    UPDATE [Seguridad].[tbRoles]
    SET rol_Descripcion = @rol_Descripcion,
        rol_Estado = @rol_Estado,
        rol_UsuarioModifica = @rol_UsuarioModifica,
        rol_FechaModifica = GETDATE()
    WHERE rol_Id = @rol_Id

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 5.5 Eliminar rol (soft delete)
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Delete]
    @rol_Id INT
AS
BEGIN
    UPDATE [Seguridad].[tbRoles]
    SET rol_EsEliminado = 1
    WHERE rol_Id = @rol_Id

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 5.6 Verificar si rol existe por descripcion
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Exist]
    @rol_Descripcion VARCHAR(100)
AS
BEGIN
    SELECT  rol_Id,
            rol_Descripcion
    FROM [Seguridad].[tbRoles]
    WHERE rol_Descripcion = @rol_Descripcion
    AND rol_EsEliminado != 1
END
GO

-- 5.7 Dropdown de roles activos
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Dropdown]
AS
BEGIN
    SELECT  rol_Id,
            rol_Descripcion
    FROM [Seguridad].[tbRoles]
    WHERE rol_Estado = 1
    AND rol_EsEliminado != 1
    ORDER BY rol_Descripcion
END
GO

-- ============================================================
-- 6. STORED PROCEDURES - USUARIOS CRUD
-- ============================================================

-- 6.1 Listar usuarios
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Usuarios_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY u.usu_Id ASC) AS Fila,
            u.usu_Id,
            u.Usu_Nombre,
            r.rol_Descripcion,
            ISNULL(u.Usu_EsActivo, 1) AS Usu_EsActivo
    FROM [Seguridad].[tbUsuarios] u
    LEFT JOIN [Seguridad].[tbRoles] r ON u.Rol_Id = r.rol_Id
    WHERE ISNULL(u.Usu_EsEliminado, 0) != 1
    ORDER BY u.usu_Id
END
GO

-- 6.2 Buscar usuario por ID
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Usuarios_Find]
    @usu_Id INT
AS
BEGIN
    SELECT  u.usu_Id,
            u.Usu_Nombre,
            u.Emp_Id,
            u.Rol_Id,
            ISNULL(u.Usu_EsActivo, 1) AS Usu_EsActivo
    FROM [Seguridad].[tbUsuarios] u
    WHERE ISNULL(u.Usu_EsEliminado, 0) != 1
    AND u.usu_Id = @usu_Id
END
GO

-- 6.3 Insertar usuario
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Usuarios_Insert]
    @Usu_Nombre VARCHAR(100),
    @Emp_Id INT,
    @Rol_Id INT,
    @Con_Id INT,
    @Usu_EsActivo BIT
AS
BEGIN
    INSERT INTO [Seguridad].[tbUsuarios]
    (Usu_Nombre, Emp_Id, Rol_Id, Con_Id, Usu_EsActivo, Usu_EsEliminado, Usu_FechaCreacion)
    VALUES
    (@Usu_Nombre, @Emp_Id, @Rol_Id, @Con_Id, @Usu_EsActivo, 0, GETDATE())

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 6.4 Actualizar usuario
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Usuarios_Update]
    @usu_Id INT,
    @Usu_Nombre VARCHAR(100),
    @Emp_Id INT,
    @Rol_Id INT,
    @Usu_EsActivo BIT
AS
BEGIN
    UPDATE [Seguridad].[tbUsuarios]
    SET Usu_Nombre = @Usu_Nombre,
        Emp_Id = @Emp_Id,
        Rol_Id = @Rol_Id,
        Usu_EsActivo = @Usu_EsActivo,
        Usu_fechaModificacion = GETDATE()
    WHERE usu_Id = @usu_Id

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 6.5 Eliminar usuario (soft delete)
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Usuarios_Delete]
    @usu_Id INT
AS
BEGIN
    UPDATE [Seguridad].[tbUsuarios]
    SET Usu_EsEliminado = 1
    WHERE usu_Id = @usu_Id

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 6.6 Verificar si usuario existe por nombre
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Usuarios_Exist]
    @Usu_Nombre VARCHAR(100)
AS
BEGIN
    SELECT  usu_Id,
            Usu_Nombre
    FROM [Seguridad].[tbUsuarios]
    WHERE Usu_Nombre = @Usu_Nombre
    AND ISNULL(Usu_EsEliminado, 0) != 1
END
GO

PRINT 'Script de Seguridad ejecutado correctamente.'
GO
