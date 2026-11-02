/*
=============================================
SCRIPT: 104_PROVEEDORES.sql
AUTOR:  Sistema PetsHome
FECHA:  2026-11-02

Catalogo de PROVEEDORES de inventario (quien vende la mercancia).
Replica el modelo "suppliers" de OpenVPM: nombre, email, telefono,
direccion, notas. Luego se asocia a la recepcion de mercancia (la compra).

Contenido:
  1. Tabla [Inventario].[tbProveedores] + indice por nombre
  2. Columna recep_ProveedorId en tbRecepcionesMercancia (nullable + FK)
  3. SPs CRUD + List + Find + Detail + Dropdown + Existe (patron CodeStatus/MessageStatus)
  4. Pantalla "Listado de proveedores" + permisos (Admin y Supervisor)

NOTA: idempotente, se puede re-ejecutar. GO entre lotes.
NOTA: tras correrlo, re-loguear para refrescar los claims de permisos.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- ============================================================
-- 1. Tabla de proveedores
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id
               WHERE s.name = 'Inventario' AND t.name = 'tbProveedores')
BEGIN
    CREATE TABLE [Inventario].[tbProveedores]
    (
        prov_Id            INT IDENTITY(1,1) NOT NULL,
        prov_Nombre        NVARCHAR(150)     NOT NULL,
        prov_Email         NVARCHAR(150)     NULL,
        prov_Telefono      NVARCHAR(40)      NULL,
        prov_Direccion     NVARCHAR(250)     NULL,
        prov_Notas         NVARCHAR(500)     NULL,
        prov_EsActivo      BIT               NOT NULL CONSTRAINT DF_tbProveedores_EsActivo    DEFAULT (1),
        prov_EsEliminado   BIT               NOT NULL CONSTRAINT DF_tbProveedores_EsEliminado DEFAULT (0),
        prov_UsuarioCrea   INT               NOT NULL,
        prov_FechaCrea     DATETIME          NOT NULL CONSTRAINT DF_tbProveedores_FechaCrea   DEFAULT (GETDATE()),
        prov_UsuarioModifica INT             NULL,
        prov_FechaModifica   DATETIME        NULL,
        CONSTRAINT PK_tbProveedores PRIMARY KEY CLUSTERED (prov_Id)
    )
    PRINT 'Tabla creada: Inventario.tbProveedores'
END
ELSE
    PRINT 'La tabla Inventario.tbProveedores ya existia, se omite el CREATE'
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_tbProveedores_Nombre'
               AND object_id = OBJECT_ID('[Inventario].[tbProveedores]'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_tbProveedores_Nombre
        ON [Inventario].[tbProveedores] (prov_EsEliminado, prov_Nombre)
    PRINT 'Indice creado: IX_tbProveedores_Nombre'
END
GO

-- ============================================================
-- 2. Ligar la recepcion de mercancia (la compra) al proveedor
--    Columna nullable, no rompe lo existente.
-- ============================================================
IF COL_LENGTH('[Inventario].[tbRecepcionesMercancia]', 'recep_ProveedorId') IS NULL
BEGIN
    ALTER TABLE [Inventario].[tbRecepcionesMercancia]
        ADD recep_ProveedorId INT NULL
    PRINT 'Columna agregada: tbRecepcionesMercancia.recep_ProveedorId'
END
ELSE
    PRINT 'La columna recep_ProveedorId ya existia, se omite el ALTER'
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tbRecepcionesMercancia_Proveedor')
   AND COL_LENGTH('[Inventario].[tbRecepcionesMercancia]', 'recep_ProveedorId') IS NOT NULL
BEGIN
    ALTER TABLE [Inventario].[tbRecepcionesMercancia]
        ADD CONSTRAINT FK_tbRecepcionesMercancia_Proveedor
        FOREIGN KEY (recep_ProveedorId) REFERENCES [Inventario].[tbProveedores] (prov_Id)
    PRINT 'FK creada: FK_tbRecepcionesMercancia_Proveedor'
END
GO

-- ============================================================
-- 3. Procedimientos almacenados (CRUD + List + Find + Detail + Dropdown + Existe)
-- ============================================================

-- ---- List ----
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Proveedores_List]
AS
BEGIN
    SET NOCOUNT ON
    SELECT  ROW_NUMBER() OVER(ORDER BY ISNULL(prov_FechaModifica, prov_FechaCrea) DESC) AS Fila,
            prov_Id,
            prov_Nombre,
            prov_Email,
            prov_Telefono,
            CASE WHEN prov_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS prov_EsActivo
    FROM    [Inventario].[tbProveedores]
    WHERE   prov_EsEliminado != 1
    ORDER BY ISNULL(prov_FechaModifica, prov_FechaCrea) DESC
END
GO

-- ---- Find (para pre-llenar el formulario en edicion) ----
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Proveedores_Find]
    @prov_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  prov_Id,
            prov_Nombre,
            prov_Email,
            prov_Telefono,
            prov_Direccion,
            prov_Notas,
            prov_EsActivo,
            prov_UsuarioCrea,
            usuarioCrea.usu_Nombre AS usuarioCrea,
            prov_FechaCrea,
            prov_UsuarioModifica,
            usuarioModifica.usu_Nombre AS usuarioModifica,
            prov_FechaModifica
    FROM    [Inventario].[tbProveedores] AS p
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea     ON p.prov_UsuarioCrea     = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON p.prov_UsuarioModifica = usuarioModifica.usu_Id
    WHERE   p.prov_EsEliminado != 1 AND p.prov_Id = @prov_Id
END
GO

-- ---- Detail (read-only) ----
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Proveedores_Detail]
    @prov_Id INT
AS
BEGIN
    SET NOCOUNT ON
    SELECT  prov_Id,
            prov_Nombre,
            prov_Email,
            prov_Telefono,
            prov_Direccion,
            prov_Notas,
            CASE WHEN prov_EsActivo = 1 THEN 'Activo' ELSE 'Inactivo' END AS prov_EsActivo,
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            prov_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            prov_FechaModifica
    FROM    [Inventario].[tbProveedores] AS p
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea     ON p.prov_UsuarioCrea     = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica ON p.prov_UsuarioModifica = usuarioModifica.usu_Id
    WHERE   p.prov_EsEliminado != 1 AND p.prov_Id = @prov_Id
END
GO

-- ---- Dropdown (elegir proveedor al registrar una recepcion) ----
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Proveedores_Dropdown]
AS
BEGIN
    SET NOCOUNT ON
    SELECT  prov_Id,
            prov_Nombre AS Descripcion
    FROM    [Inventario].[tbProveedores]
    WHERE   prov_EsEliminado != 1 AND prov_EsActivo = 1
    ORDER BY prov_Nombre
END
GO

-- ---- Existe (validacion de duplicados por nombre) ----
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Proveedores_Existe]
    @prov_Nombre NVARCHAR(150),
    @prov_Id     INT = 0
AS
BEGIN
    SET NOCOUNT ON
    IF EXISTS (
        SELECT 1 FROM [Inventario].[tbProveedores]
        WHERE prov_Nombre = @prov_Nombre
          AND prov_EsEliminado = 0
          AND prov_Id != @prov_Id
    )
        SELECT CAST(1 AS BIT) AS Existe
    ELSE
        SELECT CAST(0 AS BIT) AS Existe
END
GO

-- ---- Insert ----
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Proveedores_Insert]
    @prov_Nombre      NVARCHAR(150),
    @prov_Email       NVARCHAR(150) = NULL,
    @prov_Telefono    NVARCHAR(40)  = NULL,
    @prov_Direccion   NVARCHAR(250) = NULL,
    @prov_Notas       NVARCHAR(500) = NULL,
    @prov_EsActivo    BIT = 1,
    @prov_UsuarioCrea INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM [Inventario].[tbProveedores] WHERE prov_Nombre = @prov_Nombre AND prov_EsEliminado = 0)
        BEGIN
            SELECT -3 AS CodeStatus, 'Ya existe un proveedor con ese nombre.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            INSERT INTO [Inventario].[tbProveedores]
                (prov_Nombre, prov_Email, prov_Telefono, prov_Direccion, prov_Notas, prov_EsActivo, prov_UsuarioCrea, prov_FechaCrea)
            VALUES
                (@prov_Nombre, @prov_Email, @prov_Telefono, @prov_Direccion, @prov_Notas, @prov_EsActivo, @prov_UsuarioCrea, GETDATE())
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Proveedor creado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ---- Update ----
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Proveedores_Update]
    @prov_Id             INT,
    @prov_Nombre         NVARCHAR(150),
    @prov_Email          NVARCHAR(150) = NULL,
    @prov_Telefono       NVARCHAR(40)  = NULL,
    @prov_Direccion      NVARCHAR(250) = NULL,
    @prov_Notas          NVARCHAR(500) = NULL,
    @prov_EsActivo       BIT,
    @prov_UsuarioModifica INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [Inventario].[tbProveedores] WHERE prov_Id = @prov_Id AND prov_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'El proveedor no fue encontrado.' AS MessageStatus
            RETURN
        END
        IF EXISTS (SELECT 1 FROM [Inventario].[tbProveedores] WHERE prov_Nombre = @prov_Nombre AND prov_EsEliminado = 0 AND prov_Id != @prov_Id)
        BEGIN
            SELECT -3 AS CodeStatus, 'Ya existe un proveedor con ese nombre.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE [Inventario].[tbProveedores]
            SET prov_Nombre = @prov_Nombre,
                prov_Email = @prov_Email,
                prov_Telefono = @prov_Telefono,
                prov_Direccion = @prov_Direccion,
                prov_Notas = @prov_Notas,
                prov_EsActivo = @prov_EsActivo,
                prov_UsuarioModifica = @prov_UsuarioModifica,
                prov_FechaModifica = GETDATE()
            WHERE prov_Id = @prov_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Proveedor actualizado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ---- Delete (soft-delete, protege si tiene recepciones asociadas) ----
CREATE OR ALTER PROCEDURE [Inventario].[PR_Inventario_Proveedores_Delete]
    @prov_Id INT
AS
BEGIN
    SET NOCOUNT ON
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM [Inventario].[tbProveedores] WHERE prov_Id = @prov_Id AND prov_EsEliminado = 0)
        BEGIN
            SELECT -1 AS CodeStatus, 'El proveedor no fue encontrado.' AS MessageStatus
            RETURN
        END
        IF EXISTS (SELECT 1 FROM [Inventario].[tbRecepcionesMercancia] WHERE recep_ProveedorId = @prov_Id AND recep_EsEliminado = 0)
        BEGIN
            SELECT -2 AS CodeStatus, 'No se puede eliminar el proveedor porque tiene recepciones asociadas.' AS MessageStatus
            RETURN
        END
        BEGIN TRANSACTION
            UPDATE [Inventario].[tbProveedores] SET prov_EsEliminado = 1 WHERE prov_Id = @prov_Id
        COMMIT TRANSACTION
        SELECT 1 AS CodeStatus, 'Proveedor eliminado correctamente.' AS MessageStatus
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        SELECT -5 AS CodeStatus, ERROR_MESSAGE() AS MessageStatus
    END CATCH
END
GO

-- ============================================================
-- 4. Pantalla "Listado de proveedores" + permisos
--    Grupo Inventario (modu_Id 4), igual que Items y Recepciones.
--    Lo maneja quien maneja inventario: Administrador (1) y Supervisor (3).
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Listado de proveedores')
BEGIN
    INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo, pan_EsActivo, modu_Id)
    VALUES ('Listado de proveedores', 'Inventario', 1, 4)
    PRINT 'Pantalla creada: Listado de proveedores'
END
ELSE
    PRINT 'La pantalla ya existia, se omite el INSERT'
GO

DECLARE @pan INT = (SELECT pan_Id FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Listado de proveedores')

INSERT INTO [Seguridad].[tbRolesPantallas]
    (rol_Id, pan_Id, ropan_EsActivo, ropan_Consultar, ropan_Insertar, ropan_Editar, ropan_Eliminar)
SELECT r.Rol_Id, @pan, 0, 0, 0, 0, 0
FROM [Seguridad].[tbRoles] r
WHERE r.Rol_Id IN (1, 3)
  AND NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesPantallas] rp WHERE rp.rol_Id = r.Rol_Id AND rp.pan_Id = @pan)

-- Administrador y Supervisor: CRUD completo sobre el catalogo de proveedores
UPDATE [Seguridad].[tbRolesPantallas]
SET ropan_EsActivo = 1, ropan_Consultar = 1, ropan_Insertar = 1, ropan_Editar = 1, ropan_Eliminar = 1
WHERE pan_Id = @pan AND rol_Id IN (1, 3)

PRINT 'Matriz de permisos aplicada para Listado de proveedores'
GO

-- ---- Verificacion ----
SELECT r.Rol_Descripcion, rp.ropan_Consultar, rp.ropan_Insertar, rp.ropan_Editar, rp.ropan_Eliminar
FROM [Seguridad].[tbRolesPantallas] rp
JOIN [Seguridad].[tbRoles] r ON rp.rol_Id = r.Rol_Id
JOIN [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
WHERE p.pan_Descripcion = 'Listado de proveedores'
ORDER BY r.Rol_Id
GO
