-- =============================================
-- Stored Procedure: PR_Seguridad_GetUserPermissions
-- Descripción: Obtiene todos los permisos de un usuario por pantalla para almacenar en sesión
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_GetUserPermissions]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        mp.modpt_Descripcion AS Pantalla,
        p.Per_Nombre AS Permiso,
        m.Mod_Nombre AS Modulo,
        mp.modpt_Id
    FROM seguridad.tbUsuarios u
    INNER JOIN seguridad.tbRolModulosPantallas rmp ON u.Rol_Id = rmp.rol_Id
    INNER JOIN seguridad.tbModulosPantallas mp ON rmp.modpt_Id = mp.modpt_Id
    INNER JOIN seguridad.tbModulos m ON mp.mod_Id = m.Mod_Id
    INNER JOIN seguridad.tbRolModuloPermisos rmp2 ON u.Rol_Id = rmp2.Rol_Id AND m.Mod_Id = rmp2.Mod_Id
    INNER JOIN seguridad.tbPermisos p ON rmp2.Per_Id = p.Per_Id
    WHERE u.usu_Id = @usu_Id
        AND u.Usu_EsActivo = 1
        AND ISNULL(u.Usu_Suspendido, 0) = 0
        AND ISNULL(u.Usu_EsEliminado, 0) = 0
        AND mp.modpt_EsActivo = 1
        AND m.Mod_EsActivo = 1
        AND p.Per_EsActivo = 1
    ORDER BY m.Mod_Orden, mp.modpt_Descripcion, p.Per_Nombre;
END
GO

-- =============================================
-- Stored Procedure: PR_Seguridad_GetUserPantallas  
-- Descripción: Obtiene solo las pantallas permitidas para un usuario (compatible con sistema actual)
-- =============================================

CREATE OR ALTER PROCEDURE [dbo].[PR_Seguridad_GetUserPantallas]
    @usu_Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT
        mp.modpt_Descripcion AS Pantalla
    FROM seguridad.tbUsuarios u
    INNER JOIN seguridad.tbRolModulosPantallas rmp ON u.Rol_Id = rmp.rol_Id
    INNER JOIN seguridad.tbModulosPantallas mp ON rmp.modpt_Id = mp.modpt_Id
    WHERE u.usu_Id = @usu_Id
        AND u.Usu_EsActivo = 1
        AND ISNULL(u.Usu_Suspendido, 0) = 0
        AND ISNULL(u.Usu_EsEliminado, 0) = 0
        AND mp.modpt_EsActivo = 1
    ORDER BY mp.modpt_Descripcion;
END
GO