USE PETSHOMEDB
GO

-- ============================================================
-- Asignar pantallas al rol Cuidador (rol_Id = 5)
-- Pantallas: mascotas, solicitudes, citas, eventos, alertas, perfil medico
-- Permisos: solo Consultar en todo, excepto Solicitudes (Consultar + Insertar)
-- ============================================================

DECLARE @json NVARCHAR(MAX) = N'[
  {"pan_Id":10,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":11,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":13,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":14,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":28,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":29,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0}
]'

EXEC [Seguridad].[PR_Seguridad_RolesPantallas_Save] @rol_Id = 5, @permisosJson = @json
GO

-- Verificacion
SELECT p.pan_Id, p.pan_Descripcion, rp.ropan_Consultar, rp.ropan_Insertar, rp.ropan_Editar, rp.ropan_Eliminar
FROM   [Seguridad].[tbRolesPantallas] rp
JOIN   [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
WHERE  rp.rol_Id = 5 AND rp.ropan_EsActivo = 1
ORDER BY p.pan_Grupo, p.pan_Descripcion
GO
