-- ============================================================
-- Script 35: Reasignacion de pantallas y permisos por rol
-- ------------------------------------------------------------
-- Corrige la configuracion de tbRolesPantallas para que cada
-- rol pueda cumplir su funcion. Usa el SP de firma JSON:
--   [Seguridad].[PR_Seguridad_RolesPantallas_Save] @rol_Id, @permisosJson
-- que PRIMERO desactiva todas las pantallas del rol y luego
-- reactiva/inserta solo las del JSON (limpia filas dormidas).
--
-- Leyenda permisos: C=Consultar I=Insertar E=Editar D=Eliminar
--
-- Roles:
--   1 Administrador  -> sin cambios (ya tiene las 33 con CRUD)
--   2 Director        -> ELIMINADO: se limpian residuos
--   3 Supervisor      -> operaciones (inventario CRUD, admin, adopciones)
--   4 Veterinario     -> area medica completa (CRUD clinico + catalogos)
--   5 Cuidador        -> atencion animal (consulta + crear solicitudes)
--   6 Usuario Basico  -> acceso minimo publico (mascotas/eventos/solicitudes)
--
-- IMPORTANTE: los claims "Pantallas"/"PermisosCRUD" se cargan en
-- login; cada usuario afectado debe RE-LOGUEAR para ver los cambios.
-- ============================================================
USE PETSHOMEDB
GO
SET NOCOUNT ON
GO

PRINT '== Reasignando pantallas por rol =='
GO

-- ------------------------------------------------------------
-- ROL 3: SUPERVISOR
-- Inventario (recepciones, items) CRUD total; empleados, voluntarios,
-- cargos, refugios, localidades, categorias, eventos CRUD; mascotas (C);
-- adopciones y solicitudes (C+I+E); reporte de adopciones (C).
-- Se RETIRAN pantallas medicas (28, 29, 1029, 1030) al no listarlas.
-- ------------------------------------------------------------
DECLARE @sup NVARCHAR(MAX) = N'[
  {"pan_Id":2,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":3,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":4,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":5,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":6,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":7,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":8,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":9,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":10,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":1},
  {"pan_Id":11,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":12,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":13,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":1028,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0}
]'
EXEC [Seguridad].[PR_Seguridad_RolesPantallas_Save] @rol_Id = 3, @permisosJson = @sup
PRINT 'Rol 3 (Supervisor) reasignado.'
GO

-- ------------------------------------------------------------
-- ROL 4: VETERINARIO
-- Mascotas C+I+E (habilita Historial Medico, que esta gateado bajo
-- "Listado de mascotas"); citas, recetas, tratamientos, vacunas C+I+E;
-- 8 catalogos clinicos C+I+E; alertas, perfil medico, control vacunacion,
-- dashboard veterinario (C). Sin Eliminar (trazabilidad clinica).
-- Se retira "Reporte de adopciones" (1028) por estar fuera del dominio.
-- ------------------------------------------------------------
DECLARE @vet NVARCHAR(MAX) = N'[
  {"pan_Id":11,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":14,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":15,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":16,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":17,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":18,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":19,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":20,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":21,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":22,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":23,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":24,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":25,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":1,"ropan_Eliminar":0},
  {"pan_Id":28,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":29,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":1029,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":1030,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0}
]'
EXEC [Seguridad].[PR_Seguridad_RolesPantallas_Save] @rol_Id = 4, @permisosJson = @vet
PRINT 'Rol 4 (Veterinario) reasignado.'
GO

-- ------------------------------------------------------------
-- ROL 5: CUIDADOR
-- voluntarios (C, NUEVO), eventos (C), mascotas (C), adopciones (C, NUEVO),
-- solicitudes (C+I), citas (C), alertas (C), perfil medico (C),
-- dashboard cuidador (C). Se limpian las 3 filas dormidas (1028/1029/1030).
-- ------------------------------------------------------------
DECLARE @cui NVARCHAR(MAX) = N'[
  {"pan_Id":3,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":10,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":11,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":12,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":13,"ropan_Consultar":1,"ropan_Insertar":1,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":14,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":28,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":29,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":1031,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0}
]'
EXEC [Seguridad].[PR_Seguridad_RolesPantallas_Save] @rol_Id = 5, @permisosJson = @cui
PRINT 'Rol 5 (Cuidador) reasignado.'
GO

-- ------------------------------------------------------------
-- ROL 6: USUARIO BASICO
-- Acceso minimo publico: eventos (C), mascotas (C), solicitudes (C).
-- Se retiran todas las pantallas clinicas internas que tenia mal asignadas.
-- ------------------------------------------------------------
DECLARE @bas NVARCHAR(MAX) = N'[
  {"pan_Id":10,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":11,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0},
  {"pan_Id":13,"ropan_Consultar":1,"ropan_Insertar":0,"ropan_Editar":0,"ropan_Eliminar":0}
]'
EXEC [Seguridad].[PR_Seguridad_RolesPantallas_Save] @rol_Id = 6, @permisosJson = @bas
PRINT 'Rol 6 (Usuario Basico) reasignado.'
GO

-- ------------------------------------------------------------
-- ROL 2: DIRECTOR (eliminado) -> limpiar residuos
-- Desactiva todas sus pantallas y borra la fila huerfana de
-- tbRolesUsuarios (rol 2 <-> usu 3) que contradecia el Rol_Id del usuario.
-- ------------------------------------------------------------
EXEC [Seguridad].[PR_Seguridad_RolesPantallas_Save] @rol_Id = 2, @permisosJson = N'[]'
DELETE FROM [Seguridad].[tbRolesUsuarios] WHERE rol_Id = 2
PRINT 'Rol 2 (Director) limpiado.'
GO

-- ============================================================
-- VERIFICACION
-- ============================================================
PRINT ''
PRINT '== Pantallas activas por rol (despues) =='
SELECT  rp.rol_Id,
        r.Rol_Descripcion,
        p.pan_Grupo,
        p.pan_Id,
        p.pan_Descripcion,
        rp.ropan_Consultar AS C,
        rp.ropan_Insertar  AS I,
        rp.ropan_Editar    AS E,
        rp.ropan_Eliminar  AS D
FROM    [Seguridad].[tbRolesPantallas] rp
JOIN    [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
JOIN    [Seguridad].[tbRoles]     r ON rp.rol_Id = r.Rol_Id
WHERE   rp.ropan_EsActivo = 1
ORDER BY rp.rol_Id, p.pan_Grupo, p.pan_Id
GO

PRINT ''
PRINT '== Conteo de pantallas activas por rol =='
SELECT  r.Rol_Id,
        r.Rol_Descripcion,
        COUNT(rp.ropan_Id) AS PantallasActivas
FROM    [Seguridad].[tbRoles] r
LEFT JOIN [Seguridad].[tbRolesPantallas] rp
       ON rp.rol_Id = r.Rol_Id AND rp.ropan_EsActivo = 1
WHERE   ISNULL(r.Rol_EsEliminado,0) = 0
GROUP BY r.Rol_Id, r.Rol_Descripcion
ORDER BY r.Rol_Id
GO
