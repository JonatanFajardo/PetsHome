using System;

namespace PetsHome.Common.Entities
{
    public class PR_Seguridad_Modulos_ListResult
    {
        public int Mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public string Mod_Url { get; set; }
        public int? Mod_Orden { get; set; }
        public bool Mod_EsActivo { get; set; }
        public DateTime Mod_FechaCreacion { get; set; }
    }

    public class PR_Seguridad_Permisos_ListResult
    {
        public int Per_Id { get; set; }
        public string Per_Nombre { get; set; }
        public string Per_Descripcion { get; set; }
        public bool Per_EsActivo { get; set; }
    }

    public class PR_Seguridad_RolModuloPermisos_ListResult
    {
        public int RolModPer_Id { get; set; }
        public int Rol_Id { get; set; }
        public int Mod_Id { get; set; }
        public int Per_Id { get; set; }
        public string Rol_Descripcion { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public string Per_Nombre { get; set; }
        public string Per_Descripcion { get; set; }
        public DateTime RolModPer_FechaAsignacion { get; set; }
    }

    public class PR_Seguridad_RolModulosCompleto_ListResult
    {
        public int Rol_Id { get; set; }
        public string Rol_Descripcion { get; set; }
        public int Mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public string Mod_Url { get; set; }
        public int? Mod_Orden { get; set; }
        public bool TieneAcceso { get; set; }
        public string Permisos { get; set; } // STRING_AGG de permisos
    }

    public class PR_Seguridad_MenuUsuario_ListResult
    {
        public int usu_Id { get; set; }
        public int Rol_Id { get; set; }
        public string Rol_Descripcion { get; set; }
        public int Mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public string Mod_Url { get; set; }
        public int? Mod_Orden { get; set; }
        public string Permisos { get; set; } // STRING_AGG de permisos disponibles
    }

    public class PR_Seguridad_ModuloInsertResult
    {
        public int Mod_Id { get; set; }
        public int CodeErrorInsert { get; set; }
        public string MsgErrorInsert { get; set; }
    }

    public class PR_Seguridad_PermisoInsertResult
    {
        public int Per_Id { get; set; }
        public int CodeErrorInsert { get; set; }
        public string MsgErrorInsert { get; set; }
    }
}