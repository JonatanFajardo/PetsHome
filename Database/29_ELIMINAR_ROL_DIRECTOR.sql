USE PETSHOMEDB
GO

-- ============================================================
-- Eliminar rol Director (rol_Id = 2)
-- - Soft-delete del usuario 'director'
-- - Soft-delete del rol
-- - Desactivar sus asignaciones de pantallas
-- ============================================================

-- 1. Soft-delete usuario director
UPDATE [Seguridad].[tbUsuarios]
SET    Usu_EsEliminado = 1,
       Usu_EsActivo    = 0
WHERE  Usu_Nombre = 'director'
AND    ISNULL(Usu_EsEliminado, 0) = 0

PRINT 'Usuario director desactivado: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' fila(s)'
GO

-- 2. Desactivar pantallas asignadas al rol Director
UPDATE [Seguridad].[tbRolesPantallas]
SET    ropan_EsActivo = 0
WHERE  rol_Id = 2

PRINT 'Pantallas del Director desactivadas: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' fila(s)'
GO

-- 3. Soft-delete del rol Director
UPDATE [Seguridad].[tbRoles]
SET    rol_EsEliminado = 1,
       rol_Estado      = 0
WHERE  rol_Id = 2

PRINT 'Rol Director eliminado: ' + CAST(@@ROWCOUNT AS VARCHAR) + ' fila(s)'
GO

-- Verificacion final — no debe aparecer Director
SELECT r.rol_Id, r.rol_Descripcion, r.rol_Estado, r.rol_EsEliminado
FROM   [Seguridad].[tbRoles] r
WHERE  r.rol_Id = 2
GO
