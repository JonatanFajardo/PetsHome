/*
=============================================
SCRIPT: Pantalla "Listado de laboratorio" + permisos por rol
AUTOR:  Sistema PetsHome
FECHA:  2026-10-27
MATRIZ rol x funcion:
  Administrador (1): Consultar, Insertar, Editar, Eliminar
  Veterinario   (4): Consultar, Insertar, Editar            (no elimina)
  Recepcionista (7): Consultar, Insertar                    (solo registra la solicitud)
  Resto de roles: sin acceso
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE PETSHOMEDB
GO

-- 1. Crear la pantalla si no existe (grupo Medicamento = Clinica, modu_Id 3)
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Listado de laboratorio')
BEGIN
    INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo, pan_EsActivo, modu_Id)
    VALUES ('Listado de laboratorio', 'Medicamento', 1, 3)
    PRINT 'Pantalla creada: Listado de laboratorio'
END
ELSE
    PRINT 'La pantalla ya existia, se omite el INSERT'
GO

DECLARE @pan INT = (SELECT pan_Id FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Listado de laboratorio')

-- 2. Insertar filas rol-pantalla faltantes (sin permisos), luego fijar la matriz
INSERT INTO [Seguridad].[tbRolesPantallas]
    (rol_Id, pan_Id, ropan_EsActivo, ropan_Consultar, ropan_Insertar, ropan_Editar, ropan_Eliminar)
SELECT r.Rol_Id, @pan, 0, 0, 0, 0, 0
FROM [Seguridad].[tbRoles] r
WHERE r.Rol_Id IN (1, 4, 7)
  AND NOT EXISTS (SELECT 1 FROM [Seguridad].[tbRolesPantallas] rp WHERE rp.rol_Id = r.Rol_Id AND rp.pan_Id = @pan)

-- Administrador: todo
UPDATE [Seguridad].[tbRolesPantallas]
SET ropan_EsActivo = 1, ropan_Consultar = 1, ropan_Insertar = 1, ropan_Editar = 1, ropan_Eliminar = 1
WHERE pan_Id = @pan AND rol_Id = 1

-- Veterinario: consulta + inserta + edita (no elimina)
UPDATE [Seguridad].[tbRolesPantallas]
SET ropan_EsActivo = 1, ropan_Consultar = 1, ropan_Insertar = 1, ropan_Editar = 1, ropan_Eliminar = 0
WHERE pan_Id = @pan AND rol_Id = 4

-- Recepcionista: consulta + inserta (solo registra la solicitud)
UPDATE [Seguridad].[tbRolesPantallas]
SET ropan_EsActivo = 1, ropan_Consultar = 1, ropan_Insertar = 1, ropan_Editar = 0, ropan_Eliminar = 0
WHERE pan_Id = @pan AND rol_Id = 7

PRINT 'Matriz de permisos aplicada para Listado de laboratorio'
GO

-- 3. Verificar
SELECT r.Rol_Descripcion, rp.ropan_Consultar, rp.ropan_Insertar, rp.ropan_Editar, rp.ropan_Eliminar
FROM [Seguridad].[tbRolesPantallas] rp
JOIN [Seguridad].[tbRoles] r ON rp.rol_Id = r.Rol_Id
JOIN [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
WHERE p.pan_Descripcion = 'Listado de laboratorio'
ORDER BY r.Rol_Id
GO
