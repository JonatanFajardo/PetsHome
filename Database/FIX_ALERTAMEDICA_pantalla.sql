-- ============================================================
-- FIX: Registrar pantalla "Listado de alertas medicas"
--      y asignarla a todos los roles activos
-- Ejecutar UNA sola vez despues de crear los SPs
-- GENERADO POR: scaffold_dashboard.py
-- ============================================================
USE PETSHOMEDB
GO

-- 1. Insertar la pantalla si no existe
IF NOT EXISTS (
    SELECT 1 FROM [Seguridad].[tbPantallas]
    WHERE pan_Descripcion = 'Listado de alertas medicas'
)
BEGIN
    INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo, pan_EsActivo)
    VALUES ('Listado de alertas medicas', 'Medicamento', 1)
    PRINT 'Pantalla creada: Listado de alertas medicas'
END
ELSE
    PRINT 'La pantalla ya existia, se omite el INSERT'
GO

-- 2. Asignarla a todos los roles que no la tengan aun
INSERT INTO [Seguridad].[tbRolesPantallas] (rol_Id, pan_Id, ropan_EsActivo)
SELECT r.Rol_Id, p.pan_Id, 1
FROM [Seguridad].[tbRoles] r
CROSS JOIN [Seguridad].[tbPantallas] p
WHERE p.pan_Descripcion = 'Listado de alertas medicas'
  AND NOT EXISTS (
      SELECT 1 FROM [Seguridad].[tbRolesPantallas] rp
      WHERE rp.rol_Id = r.Rol_Id AND rp.pan_Id = p.pan_Id
  )

PRINT 'Pantalla asignada a los roles que no la tenian'
GO

-- 3. Verificar resultado
SELECT r.Rol_Descripcion, p.pan_Descripcion, p.pan_Grupo, rp.ropan_EsActivo
FROM [Seguridad].[tbRolesPantallas] rp
JOIN [Seguridad].[tbRoles] r     ON rp.rol_Id = r.Rol_Id
JOIN [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
WHERE p.pan_Descripcion = 'Listado de alertas medicas'
GO
