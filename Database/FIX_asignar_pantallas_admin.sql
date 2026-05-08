-- ============================================================
-- FIX: Asignar todas las pantallas al rol Administrador
-- Ejecutar una vez si el rol no tiene pantallas asignadas
-- ============================================================
USE PETSHOMEDB
GO

-- Ver qué rol tiene tu usuario (para confirmar el ID)
SELECT u.usu_Id, u.Usu_Nombre, r.Rol_Id, r.Rol_Descripcion
FROM [Seguridad].[tbUsuarios] u
JOIN [Seguridad].[tbRoles] r ON u.Rol_Id = r.Rol_Id
GO

-- Ver cuántas pantallas tiene asignado cada rol ahora
SELECT r.Rol_Descripcion, COUNT(rp.pan_Id) AS PantallasAsignadas
FROM [Seguridad].[tbRoles] r
LEFT JOIN [Seguridad].[tbRolesPantallas] rp ON r.Rol_Id = rp.rol_Id AND rp.ropan_EsActivo = 1
GROUP BY r.Rol_Id, r.Rol_Descripcion
GO

-- ============================================================
-- ASIGNAR TODAS LAS PANTALLAS AL ROL ADMINISTRADOR (Rol_Id = 1)
-- Cambia el @rol_Id si tu rol admin tiene otro ID
-- ============================================================
DECLARE @rol_Id INT = 1   -- <-- cambia si es necesario

-- Insertar todas las pantallas que no estén ya asignadas
INSERT INTO [Seguridad].[tbRolesPantallas] (rol_Id, pan_Id, ropan_EsActivo)
SELECT @rol_Id, p.pan_Id, 1
FROM [Seguridad].[tbPantallas] p
WHERE p.pan_EsActivo = 1
  AND NOT EXISTS (
    SELECT 1 FROM [Seguridad].[tbRolesPantallas] rp
    WHERE rp.rol_Id = @rol_Id AND rp.pan_Id = p.pan_Id
  )

-- Reactivar las que estén desactivadas
UPDATE [Seguridad].[tbRolesPantallas]
SET ropan_EsActivo = 1
WHERE rol_Id = @rol_Id

PRINT 'Pantallas asignadas al rol ' + CAST(@rol_Id AS VARCHAR)
GO

-- Verificar resultado
SELECT p.pan_Descripcion, p.pan_Grupo
FROM [Seguridad].[tbRolesPantallas] rp
JOIN [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
WHERE rp.rol_Id = 1 AND rp.ropan_EsActivo = 1
ORDER BY p.pan_Grupo, p.pan_Descripcion
GO
