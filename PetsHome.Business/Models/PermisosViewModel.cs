using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public class ModuloViewModel
    {
        public int Mod_Id { get; set; }
        
        [Required(ErrorMessage = "El nombre del módulo es requerido")]
        [Display(Name = "Nombre del Módulo")]
        public string Mod_Nombre { get; set; }
        
        [Display(Name = "Descripción")]
        public string Mod_Descripcion { get; set; }
        
        [Display(Name = "Icono")]
        public string Mod_Icono { get; set; }
        
        [Display(Name = "URL")]
        public string Mod_Url { get; set; }
        
        [Display(Name = "Orden")]
        public int? Mod_Orden { get; set; }
        
        [Display(Name = "Activo")]
        public bool Mod_EsActivo { get; set; }
    }

    public class PermisoViewModel
    {
        public int Per_Id { get; set; }
        
        [Required(ErrorMessage = "El nombre del permiso es requerido")]
        [Display(Name = "Nombre del Permiso")]
        public string Per_Nombre { get; set; }
        
        [Display(Name = "Descripción")]
        public string Per_Descripcion { get; set; }
        
        [Display(Name = "Activo")]
        public bool Per_EsActivo { get; set; }
    }

    public class RolModuloPermisoViewModel
    {
        public int RolModPer_Id { get; set; }
        public int Rol_Id { get; set; }
        public int Mod_Id { get; set; }
        public int Per_Id { get; set; }
        
        [Display(Name = "Rol")]
        public string Rol_Descripcion { get; set; }
        
        [Display(Name = "Módulo")]
        public string Mod_Nombre { get; set; }
        
        [Display(Name = "Permiso")]
        public string Per_Nombre { get; set; }
        
        public bool TienePermiso { get; set; }
    }

    public class GestionPermisosViewModel
    {
        [Display(Name = "Rol")]
        public int Rol_Id { get; set; }
        
        public string Rol_Descripcion { get; set; }
        
        public List<ModuloPermisosViewModel> Modulos { get; set; } = new List<ModuloPermisosViewModel>();
        
        // Para dropdowns
        public List<dynamic> RolesDropdown { get; set; } = new List<dynamic>();
    }

    public class ModuloPermisosViewModel
    {
        public int Mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public bool TieneAcceso { get; set; }
        
        public List<PermisoCheckViewModel> Permisos { get; set; } = new List<PermisoCheckViewModel>();
    }

    public class PermisoCheckViewModel
    {
        public int Per_Id { get; set; }
        public string Per_Nombre { get; set; }
        public string Per_Descripcion { get; set; }
        public bool Seleccionado { get; set; }
    }

    public class MenuItemViewModel
    {
        public int Mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public string Mod_Url { get; set; }
        public int Mod_Orden { get; set; }
        public List<string> Permisos { get; set; } = new List<string>();
        public bool TieneAcceso { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        public string TipoItem { get; set; } = "MODULE"; // MODULE o SUBMODULE
        public int Mod_Padre { get; set; } = 0; // ID del módulo padre
        public List<MenuItemViewModel> SubModulos { get; set; } = new List<MenuItemViewModel>();
    }

    public class MenuViewModel
    {
        public List<MenuItemViewModel> MenuItems { get; set; } = new List<MenuItemViewModel>();
        public string UsuarioNombre { get; set; }
        public string RolDescripcion { get; set; }
    }
}