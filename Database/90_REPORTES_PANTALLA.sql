/*
=============================================
SCRIPT 90: Registro de la pantalla "Reportes"
AUTOR: Sistema PetsHome
FECHA: 2026-10-29
DESCRIPCION:
  Registra la pantalla "Reportes" en Seguridad.tbPantallas y la asigna al rol
  Administrador (Rol_Id = 1), igual que FIX_asignar_pantallas_admin.sql.
  - modu_Id = 1 (CORE): modulo nucleo siempre activo (Comun) -> pantalla transversal,
    no depende de un modulo opcional de Veterinaria.
  - pan_Grupo = 'Reportes' (metadata del grupo; el sidebar se arma con tiene("Reportes")).
  - pan_Id es IDENTITY -> no se fuerza; se referencia por pan_Descripcion.
  Idempotente: no duplica ni la pantalla ni la asignacion.
=============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE PETSHOMEDB
GO

-- 1. Registrar la pantalla "Reportes" si no existe
IF NOT EXISTS (SELECT 1 FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Reportes')
BEGIN
    INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo, pan_EsActivo, modu_Id)
    VALUES ('Reportes', 'Reportes', 1, 1);
    PRINT 'Pantalla "Reportes" registrada';
END
ELSE
BEGIN
    UPDATE [Seguridad].[tbPantallas]
    SET pan_EsActivo = 1, pan_Grupo = 'Reportes', modu_Id = 1
    WHERE pan_Descripcion = 'Reportes';
    PRINT 'Pantalla "Reportes" ya existia (actualizada)';
END
GO

-- 2. Asignar la pantalla "Reportes" al rol Administrador (Rol_Id = 1)
DECLARE @rol_Id INT = 1;
DECLARE @pan_Id INT = (SELECT pan_Id FROM [Seguridad].[tbPantallas] WHERE pan_Descripcion = 'Reportes');

IF NOT EXISTS (
    SELECT 1 FROM [Seguridad].[tbRolesPantallas]
    WHERE rol_Id = @rol_Id AND pan_Id = @pan_Id
)
BEGIN
    INSERT INTO [Seguridad].[tbRolesPantallas]
        (rol_Id, pan_Id, ropan_EsActivo, ropan_Consultar, ropan_Insertar, ropan_Editar, ropan_Eliminar)
    VALUES (@rol_Id, @pan_Id, 1, 1, 0, 0, 0);
    PRINT 'Pantalla "Reportes" asignada al rol Administrador';
END
ELSE
BEGIN
    UPDATE [Seguridad].[tbRolesPantallas]
    SET ropan_EsActivo = 1, ropan_Consultar = 1
    WHERE rol_Id = @rol_Id AND pan_Id = @pan_Id;
    PRINT 'Asignacion de "Reportes" al rol Administrador ya existia (reactivada)';
END
GO

-- 3. Verificar
SELECT p.pan_Id, p.pan_Descripcion, p.pan_Grupo, p.pan_EsActivo, p.modu_Id,
       rp.ropan_EsActivo, rp.ropan_Consultar
FROM [Seguridad].[tbPantallas] p
LEFT JOIN [Seguridad].[tbRolesPantallas] rp ON p.pan_Id = rp.pan_Id AND rp.rol_Id = 1
WHERE p.pan_Descripcion = 'Reportes';
GO
