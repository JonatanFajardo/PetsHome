USE PETSHOMEDB
GO

-- ============================================================
-- USUARIOS POR ROL — PetsHome
-- Contraseña de todos los usuarios: petshome123
-- Hash SHA256: UPPER(CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'petshome123'), 2))
--
-- Para cambiar un nombre de usuario editar el valor de @usu_Nombre
-- en el bloque correspondiente a cada rol.
--
-- Roles disponibles:
--   1 = Administrador   → admin         (ya existe, no se toca)
--   2 = Director        → director
--   3 = Supervisor      → supervisor
--   4 = Veterinario     → veterinario
--   5 = Cuidador        → cuidador
--   6 = Usuario Básico  → recepcion
-- ============================================================

DECLARE @hash NVARCHAR(64) =
    UPPER(CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'petshome123'), 2))

-- Helper: inserta en tbRolesUsuarios si no existe
-- (se repite por restricción de DECLARE dentro de bloques IF en T-SQL)

-- ── ROL 2: Director ── (emp 8 = Miguel Caballero)
DECLARE @nombre_director NVARCHAR(50) = 'director'   -- ← editar nombre aquí
DECLARE @id_director INT
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_director AND ISNULL(Usu_EsEliminado,0)=0)
BEGIN
    INSERT INTO [Seguridad].[tbUsuarios]
        (Emp_Id, Usu_Nombre, Rol_Id, Usu_PasswordHash, Usu_EsActivo,
         Usu_Suspendido, Usu_EsEliminado, usu_Logueado, usu_IntentosFallidos,
         Usu_FechaCreacion)
    VALUES (8, @nombre_director, 2, @hash, 1, 0, 0, 0, 0, GETDATE())
    SET @id_director = SCOPE_IDENTITY()
    PRINT 'Usuario creado: ' + @nombre_director + '  (Rol: Director, Emp: 8 - Miguel Caballero)'
END
ELSE
BEGIN
    SELECT @id_director = usu_Id FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_director AND ISNULL(Usu_EsEliminado,0)=0
    PRINT 'Ya existe usuario: ' + @nombre_director
END
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesUsuarios] WHERE usu_Id = @id_director AND rol_Id = 2)
    INSERT INTO [Seguridad].[tbRolesUsuarios] (rol_Id, usu_Id, rol_usu_FechaAsignacion) VALUES (2, @id_director, GETDATE())

-- ── ROL 3: Supervisor ── (emp 5 = Ana Flores)
DECLARE @nombre_supervisor NVARCHAR(50) = 'supervisor'   -- ← editar nombre aquí
DECLARE @id_supervisor INT
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_supervisor AND ISNULL(Usu_EsEliminado,0)=0)
BEGIN
    INSERT INTO [Seguridad].[tbUsuarios]
        (Emp_Id, Usu_Nombre, Rol_Id, Usu_PasswordHash, Usu_EsActivo,
         Usu_Suspendido, Usu_EsEliminado, usu_Logueado, usu_IntentosFallidos,
         Usu_FechaCreacion)
    VALUES (5, @nombre_supervisor, 3, @hash, 1, 0, 0, 0, 0, GETDATE())
    SET @id_supervisor = SCOPE_IDENTITY()
    PRINT 'Usuario creado: ' + @nombre_supervisor + '  (Rol: Supervisor, Emp: 5 - Ana Flores)'
END
ELSE
BEGIN
    SELECT @id_supervisor = usu_Id FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_supervisor AND ISNULL(Usu_EsEliminado,0)=0
    PRINT 'Ya existe usuario: ' + @nombre_supervisor
END
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesUsuarios] WHERE usu_Id = @id_supervisor AND rol_Id = 3)
    INSERT INTO [Seguridad].[tbRolesUsuarios] (rol_Id, usu_Id, rol_usu_FechaAsignacion) VALUES (3, @id_supervisor, GETDATE())

-- ── ROL 4: Veterinario ── (emp 2 = Maria Rodriguez)
DECLARE @nombre_veterinario NVARCHAR(50) = 'veterinario'   -- ← editar nombre aquí
DECLARE @id_veterinario INT
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_veterinario AND ISNULL(Usu_EsEliminado,0)=0)
BEGIN
    INSERT INTO [Seguridad].[tbUsuarios]
        (Emp_Id, Usu_Nombre, Rol_Id, Usu_PasswordHash, Usu_EsActivo,
         Usu_Suspendido, Usu_EsEliminado, usu_Logueado, usu_IntentosFallidos,
         Usu_FechaCreacion)
    VALUES (2, @nombre_veterinario, 4, @hash, 1, 0, 0, 0, 0, GETDATE())
    SET @id_veterinario = SCOPE_IDENTITY()
    PRINT 'Usuario creado: ' + @nombre_veterinario + '  (Rol: Veterinario, Emp: 2 - Maria Rodriguez)'
END
ELSE
BEGIN
    SELECT @id_veterinario = usu_Id FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_veterinario AND ISNULL(Usu_EsEliminado,0)=0
    PRINT 'Ya existe usuario: ' + @nombre_veterinario
END
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesUsuarios] WHERE usu_Id = @id_veterinario AND rol_Id = 4)
    INSERT INTO [Seguridad].[tbRolesUsuarios] (rol_Id, usu_Id, rol_usu_FechaAsignacion) VALUES (4, @id_veterinario, GETDATE())

-- ── ROL 5: Cuidador ── (emp 3 = Jose Hernandez)
DECLARE @nombre_cuidador NVARCHAR(50) = 'cuidador'   -- ← editar nombre aquí
DECLARE @id_cuidador INT
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_cuidador AND ISNULL(Usu_EsEliminado,0)=0)
BEGIN
    INSERT INTO [Seguridad].[tbUsuarios]
        (Emp_Id, Usu_Nombre, Rol_Id, Usu_PasswordHash, Usu_EsActivo,
         Usu_Suspendido, Usu_EsEliminado, usu_Logueado, usu_IntentosFallidos,
         Usu_FechaCreacion)
    VALUES (3, @nombre_cuidador, 5, @hash, 1, 0, 0, 0, 0, GETDATE())
    SET @id_cuidador = SCOPE_IDENTITY()
    PRINT 'Usuario creado: ' + @nombre_cuidador + '  (Rol: Cuidador, Emp: 3 - Jose Hernandez)'
END
ELSE
BEGIN
    SELECT @id_cuidador = usu_Id FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_cuidador AND ISNULL(Usu_EsEliminado,0)=0
    PRINT 'Ya existe usuario: ' + @nombre_cuidador
END
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesUsuarios] WHERE usu_Id = @id_cuidador AND rol_Id = 5)
    INSERT INTO [Seguridad].[tbRolesUsuarios] (rol_Id, usu_Id, rol_usu_FechaAsignacion) VALUES (5, @id_cuidador, GETDATE())

-- ── ROL 6: Usuario Básico ── (emp 4 = Fernando Ramos)
DECLARE @nombre_basico NVARCHAR(50) = 'recepcion'   -- ← editar nombre aquí
DECLARE @id_basico INT
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_basico AND ISNULL(Usu_EsEliminado,0)=0)
BEGIN
    INSERT INTO [Seguridad].[tbUsuarios]
        (Emp_Id, Usu_Nombre, Rol_Id, Usu_PasswordHash, Usu_EsActivo,
         Usu_Suspendido, Usu_EsEliminado, usu_Logueado, usu_IntentosFallidos,
         Usu_FechaCreacion)
    VALUES (4, @nombre_basico, 6, @hash, 1, 0, 0, 0, 0, GETDATE())
    SET @id_basico = SCOPE_IDENTITY()
    PRINT 'Usuario creado: ' + @nombre_basico + '  (Rol: Usuario Básico, Emp: 4 - Fernando Ramos)'
END
ELSE
BEGIN
    SELECT @id_basico = usu_Id FROM [Seguridad].[tbUsuarios] WHERE Usu_Nombre = @nombre_basico AND ISNULL(Usu_EsEliminado,0)=0
    PRINT 'Ya existe usuario: ' + @nombre_basico
END
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesUsuarios] WHERE usu_Id = @id_basico AND rol_Id = 6)
    INSERT INTO [Seguridad].[tbRolesUsuarios] (rol_Id, usu_Id, rol_usu_FechaAsignacion) VALUES (6, @id_basico, GETDATE())

GO

-- ── Verificación final ──────────────────────────────────────
SELECT
    u.usu_Id          AS id,
    u.Usu_Nombre      AS usuario,
    r.Rol_Descripcion AS rol,
    p.per_PrimerNombre + ' ' + p.per_ApellidoPaterno AS empleado,
    u.Usu_EsActivo    AS activo
FROM [Seguridad].[tbUsuarios] u
INNER JOIN [Seguridad].[tbRolesUsuarios] ru ON ru.usu_Id = u.usu_Id
INNER JOIN [Seguridad].[tbRoles]    r ON r.Rol_Id  = ru.rol_Id
INNER JOIN [Refugio].[tbEmpleados]  e ON e.emp_Id  = u.Emp_Id
INNER JOIN [General].[tbPersonas]   p ON p.per_Id  = e.per_Id
WHERE ISNULL(u.Usu_EsEliminado,0) = 0
ORDER BY ru.rol_Id
GO

PRINT '=========================================='
PRINT 'Credenciales de acceso:'
PRINT '   admin        / admin123'
PRINT '   director     / petshome123'
PRINT '   supervisor   / petshome123'
PRINT '   veterinario  / petshome123'
PRINT '   cuidador     / petshome123'
PRINT '   recepcion    / petshome123'
PRINT '=========================================='
GO
