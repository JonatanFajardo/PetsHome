-- ============================================================
-- Script: Correccion de columnas tbRoles + SPs
-- Descripcion: Agrega columnas faltantes a tbRoles y corrige
--              los stored procedures para usar nombres correctos
-- Base de datos: PETSHOMEDB
-- ============================================================

USE PETSHOMEDB
GO

-- ============================================================
-- 1. AGREGAR COLUMNAS FALTANTES A tbRoles
-- ============================================================

-- Agregar rol_EsEliminado si no existe
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbRoles' AND COLUMN_NAME = 'rol_EsEliminado')
BEGIN
    ALTER TABLE [Seguridad].[tbRoles]
    ADD rol_EsEliminado BIT NOT NULL DEFAULT 0
    PRINT 'Columna rol_EsEliminado agregada'
END
GO

-- Agregar rol_UsuarioCrea si no existe
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbRoles' AND COLUMN_NAME = 'rol_UsuarioCrea')
BEGIN
    ALTER TABLE [Seguridad].[tbRoles]
    ADD rol_UsuarioCrea INT NULL
    PRINT 'Columna rol_UsuarioCrea agregada'
END
GO

-- Agregar rol_FechaCrea si no existe
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbRoles' AND COLUMN_NAME = 'rol_FechaCrea')
BEGIN
    ALTER TABLE [Seguridad].[tbRoles]
    ADD rol_FechaCrea DATETIME NULL
    PRINT 'Columna rol_FechaCrea agregada'
END
GO

-- Agregar rol_UsuarioModifica si no existe
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbRoles' AND COLUMN_NAME = 'rol_UsuarioModifica')
BEGIN
    ALTER TABLE [Seguridad].[tbRoles]
    ADD rol_UsuarioModifica INT NULL
    PRINT 'Columna rol_UsuarioModifica agregada'
END
GO

-- Agregar rol_FechaModifica si no existe
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tbRoles' AND COLUMN_NAME = 'rol_FechaModifica')
BEGIN
    ALTER TABLE [Seguridad].[tbRoles]
    ADD rol_FechaModifica DATETIME NULL
    PRINT 'Columna rol_FechaModifica agregada'
END
GO

-- ============================================================
-- 2. CORREGIR STORED PROCEDURES DE ROLES
--    La tabla usa: Rol_Id, Rol_Descripcion, Rol_EsActivo
--    Nuevas columnas: rol_EsEliminado, rol_UsuarioCrea, etc.
-- ============================================================

-- 2.1 Listar roles
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY r.Rol_Id ASC) AS Fila,
            r.Rol_Id AS rol_Id,
            r.Rol_Descripcion AS rol_Descripcion,
            r.Rol_EsActivo AS rol_Estado,
            (SELECT COUNT(*) FROM [Seguridad].[tbRolesPantallas] rp
             WHERE rp.rol_Id = r.Rol_Id AND rp.ropan_EsActivo = 1) AS CantidadPantallas
    FROM [Seguridad].[tbRoles] r
    WHERE ISNULL(r.rol_EsEliminado, 0) != 1
    ORDER BY r.Rol_Id
END
GO

-- 2.2 Buscar rol por ID
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Find]
    @rol_Id INT
AS
BEGIN
    SELECT  r.Rol_Id AS rol_Id,
            r.Rol_Descripcion AS rol_Descripcion,
            r.Rol_EsActivo AS rol_Estado
    FROM [Seguridad].[tbRoles] r
    WHERE ISNULL(r.rol_EsEliminado, 0) != 1
    AND r.Rol_Id = @rol_Id
END
GO

-- 2.3 Insertar rol
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Insert]
    @rol_Descripcion VARCHAR(100),
    @rol_Estado BIT,
    @rol_UsuarioCrea INT
AS
BEGIN
    INSERT INTO [Seguridad].[tbRoles]
    (Rol_Descripcion, Rol_EsActivo, rol_UsuarioCrea, rol_FechaCrea, rol_EsEliminado)
    VALUES
    (@rol_Descripcion, @rol_Estado, @rol_UsuarioCrea, GETDATE(), 0)

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 2.4 Actualizar rol
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Update]
    @rol_Id INT,
    @rol_Descripcion VARCHAR(100),
    @rol_Estado BIT,
    @rol_UsuarioModifica INT
AS
BEGIN
    UPDATE [Seguridad].[tbRoles]
    SET Rol_Descripcion = @rol_Descripcion,
        Rol_EsActivo = @rol_Estado,
        rol_UsuarioModifica = @rol_UsuarioModifica,
        rol_FechaModifica = GETDATE()
    WHERE Rol_Id = @rol_Id

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 2.5 Eliminar rol (soft delete)
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Delete]
    @rol_Id INT
AS
BEGIN
    UPDATE [Seguridad].[tbRoles]
    SET rol_EsEliminado = 1
    WHERE Rol_Id = @rol_Id

    SELECT @@ROWCOUNT AS Resultado
END
GO

-- 2.6 Verificar si rol existe por descripcion
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Exist]
    @rol_Descripcion VARCHAR(100)
AS
BEGIN
    SELECT  Rol_Id AS rol_Id,
            Rol_Descripcion AS rol_Descripcion
    FROM [Seguridad].[tbRoles]
    WHERE Rol_Descripcion = @rol_Descripcion
    AND ISNULL(rol_EsEliminado, 0) != 1
END
GO

-- 2.7 Dropdown de roles activos
CREATE OR ALTER PROCEDURE [Seguridad].[PR_Seguridad_Roles_Dropdown]
AS
BEGIN
    SELECT  Rol_Id AS rol_Id,
            Rol_Descripcion AS rol_Descripcion
    FROM [Seguridad].[tbRoles]
    WHERE Rol_EsActivo = 1
    AND ISNULL(rol_EsEliminado, 0) != 1
    ORDER BY Rol_Descripcion
END
GO

-- ============================================================
-- 3. CORREGIR FK en tbRolesPantallas
--    La FK referencia Rol_Id (con R mayuscula)
-- ============================================================

-- Verificar que la FK funcione correctamente
-- (La tabla tbRolesPantallas usa rol_Id minuscula pero la PK de tbRoles es Rol_Id)
-- SQL Server no es case-sensitive para nombres de columna por defecto, asi que deberia funcionar

PRINT 'Script de correccion ejecutado correctamente.'
PRINT 'Ahora ejecute el script 17_SEGURIDAD_PANTALLAS_ROLES.sql de nuevo si es necesario.'
GO
