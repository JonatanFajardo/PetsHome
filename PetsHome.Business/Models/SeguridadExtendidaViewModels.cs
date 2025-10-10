using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PetsHome.Common.Entities;

namespace PetsHome.Business.Models
{
    // ===== COMPONENTES =====
    public class ComponenteViewModel
    {
        public int comp_Id { get; set; }
        
        [Required(ErrorMessage = "La descripción del componente es requerida")]
        [Display(Name = "Descripción del Componente")]
        [StringLength(50, ErrorMessage = "La descripción no puede exceder 50 caracteres")]
        public string comp_Descripcion { get; set; }
        
        // Propiedades calculadas
        public int TotalModulos { get; set; }
        public List<ModuloSimpleViewModel> Modulos { get; set; } = new List<ModuloSimpleViewModel>();
    }

    // ===== MÓDULOS PANTALLAS =====
    public class ModuloPantallaViewModel
    {
        public int modpt_Id { get; set; }
        
        [Required(ErrorMessage = "El módulo es requerido")]
        [Display(Name = "Módulo")]
        public int mod_Id { get; set; }
        
        [Required(ErrorMessage = "La descripción de la pantalla es requerida")]
        [Display(Name = "Descripción de la Pantalla")]
        [StringLength(100, ErrorMessage = "La descripción no puede exceder 100 caracteres")]
        public string modpt_Descripcion { get; set; }
        
        [Display(Name = "URL")]
        [StringLength(200, ErrorMessage = "La URL no puede exceder 200 caracteres")]
        public string modpt_Url { get; set; }
        
        [Display(Name = "Icono")]
        [StringLength(50, ErrorMessage = "El icono no puede exceder 50 caracteres")]
        public string modpt_Icono { get; set; }
        
        [Display(Name = "Orden")]
        public int? modpt_Orden { get; set; }
        
        [Display(Name = "Activo")]
        public bool modpt_EsActivo { get; set; }
        
        // Propiedades de navegación
        [Display(Name = "Módulo")]
        public string Mod_Nombre { get; set; }
        
        [Display(Name = "Descripción del Módulo")]
        public string Mod_Descripcion { get; set; }
        
        [Display(Name = "Componente")]
        public string comp_Descripcion { get; set; }
    }

    // ===== ROL MÓDULOS PANTALLAS =====
    public class RolModuloPantallaViewModel
    {
        public int rolpt_Id { get; set; }
        public int modpt_Id { get; set; }
        public int rol_Id { get; set; }
        public DateTime rolpt_FechaAsignacion { get; set; }
        
        // Propiedades de navegación
        [Display(Name = "Pantalla")]
        public string modpt_Descripcion { get; set; }
        
        [Display(Name = "URL")]
        public string modpt_Url { get; set; }
        
        [Display(Name = "Módulo")]
        public string Mod_Nombre { get; set; }
        
        [Display(Name = "Rol")]
        public string Rol_Descripcion { get; set; }
    }

    // ===== ROLES USUARIOS =====
    public class RolUsuarioViewModel
    {
        public int rol_usu_Id { get; set; }
        public int rol_Id { get; set; }
        public int usu_Id { get; set; }
        public DateTime rol_usu_FechaAsignacion { get; set; }
        
        // Propiedades de navegación
        [Display(Name = "Rol")]
        public string Rol_Descripcion { get; set; }
        
        [Display(Name = "Usuario")]
        public string Usu_Nombre { get; set; }
        
        [Display(Name = "Empleado")]
        public string Emp_NombreCompleto { get; set; }
    }

    // ===== GESTIÓN AVANZADA DE PERMISOS =====
    public class GestionPermisosAvanzadaViewModel
    {
        [Display(Name = "Rol")]
        public int Rol_Id { get; set; }
        
        public string Rol_Descripcion { get; set; }
        
        [Display(Name = "Componente")]
        public int? comp_Id { get; set; }
        
        public List<ComponentePermisoViewModel> Componentes { get; set; } = new List<ComponentePermisoViewModel>();
        
        // Para dropdowns
        public List<dynamic> RolesDropdown { get; set; } = new List<dynamic>();
        public List<dynamic> ComponentesDropdown { get; set; } = new List<dynamic>();
    }

    public class ComponentePermisoViewModel
    {
        public int comp_Id { get; set; }
        public string comp_Descripcion { get; set; }
        public bool TieneAcceso { get; set; }
        
        public List<ModuloPantallaPermisoViewModel> Modulos { get; set; } = new List<ModuloPantallaPermisoViewModel>();
    }

    public class ModuloPantallaPermisoViewModel
    {
        public int Mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public bool ModuloTieneAcceso { get; set; }
        
        public List<PantallaPantallaPermisoViewModel> Pantallas { get; set; } = new List<PantallaPantallaPermisoViewModel>();
        public List<PermisoCheckViewModel> PermisosModulo { get; set; } = new List<PermisoCheckViewModel>();
    }

    public class PantallaPantallaPermisoViewModel
    {
        public int modpt_Id { get; set; }
        public string modpt_Descripcion { get; set; }
        public string modpt_Url { get; set; }
        public string modpt_Icono { get; set; }
        public int? modpt_Orden { get; set; }
        public bool TieneAcceso { get; set; }
    }

    // ===== GESTIÓN DE USUARIOS MÚLTIPLES ROLES =====
    public class UsuarioRolesViewModel
    {
        public int usu_Id { get; set; }
        
        [Display(Name = "Usuario")]
        public string Usu_Nombre { get; set; }
        
        [Display(Name = "Empleado")]
        public string Emp_NombreCompleto { get; set; }
        
        public List<RolAsignacionViewModel> RolesDisponibles { get; set; } = new List<RolAsignacionViewModel>();
        public List<RolAsignacionViewModel> RolesAsignados { get; set; } = new List<RolAsignacionViewModel>();
    }

    public class RolAsignacionViewModel
    {
        public int Rol_Id { get; set; }
        public string Rol_Descripcion { get; set; }
        public bool Asignado { get; set; }
        public DateTime? FechaAsignacion { get; set; }
    }

    // ===== MENÚ EXTENDIDO CON COMPONENTES Y PANTALLAS =====
    public class MenuExtendidoViewModel
    {
        public List<ComponenteMenuViewModel> Componentes { get; set; } = new List<ComponenteMenuViewModel>();
        public string UsuarioNombre { get; set; }
        public List<string> RolesDescripcion { get; set; } = new List<string>();
        public string ImagenPerfil { get; set; }
    }

    public class ComponenteMenuViewModel
    {
        public int comp_Id { get; set; }
        public string comp_Descripcion { get; set; }
        public List<ModuloMenuExtendidoViewModel> Modulos { get; set; } = new List<ModuloMenuExtendidoViewModel>();
    }

    public class ModuloMenuExtendidoViewModel
    {
        public int Mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public string Mod_Icono { get; set; }
        public int? Mod_Orden { get; set; }
        public List<string> Permisos { get; set; } = new List<string>();
        public bool TieneAcceso { get; set; }
        public bool PuedeCrear { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeEliminar { get; set; }
        
        public List<PantallaMenuViewModel> Pantallas { get; set; } = new List<PantallaMenuViewModel>();
    }

    public class PantallaMenuViewModel
    {
        public int modpt_Id { get; set; }
        public string modpt_Descripcion { get; set; }
        public string modpt_Url { get; set; }
        public string modpt_Icono { get; set; }
        public int? modpt_Orden { get; set; }
        public bool TieneAcceso { get; set; }
    }

    // ===== VIEWMODELS AUXILIARES =====
    public class ModuloSimpleViewModel
    {
        public int Mod_Id { get; set; }
        public string Mod_Nombre { get; set; }
        public string Mod_Descripcion { get; set; }
        public int TotalPantallas { get; set; }
    }

    public class AsignacionMasivaViewModel
    {
        [Required(ErrorMessage = "El rol es requerido")]
        [Display(Name = "Rol")]
        public int Rol_Id { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar al menos una pantalla")]
        [Display(Name = "Pantallas")]
        public List<int> PantallasIds { get; set; } = new List<int>();
        
        [Display(Name = "Operación")]
        public string Operacion { get; set; } = "ASIGNAR"; // ASIGNAR o REMOVER
        
        // Para el dropdown
        public List<dynamic> RolesDropdown { get; set; } = new List<dynamic>();
        public List<PantallaSelectorViewModel> PantallasDisponibles { get; set; } = new List<PantallaSelectorViewModel>();
    }

    public class PantallaSelectorViewModel
    {
        public int modpt_Id { get; set; }
        public string modpt_Descripcion { get; set; }
        public string Mod_Nombre { get; set; }
        public string comp_Descripcion { get; set; }
        public bool Seleccionado { get; set; }
    }

    // ===== REPORTES Y ESTADÍSTICAS =====
    public class ReporteSeguridadViewModel
    {
        public int TotalComponentes { get; set; }
        public int TotalModulos { get; set; }
        public int TotalPantallas { get; set; }
        public int TotalRoles { get; set; }
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosSuspendidos { get; set; }
        
        public List<RolEstadisticaViewModel> EstadisticasPorRol { get; set; } = new List<RolEstadisticaViewModel>();
        public List<ComponenteEstadisticaViewModel> EstadisticasPorComponente { get; set; } = new List<ComponenteEstadisticaViewModel>();
    }

    public class RolEstadisticaViewModel
    {
        public int Rol_Id { get; set; }
        public string Rol_Descripcion { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalPantallasAsignadas { get; set; }
        public int TotalPermisosAsignados { get; set; }
    }

    public class ComponenteEstadisticaViewModel
    {
        public int comp_Id { get; set; }
        public string comp_Descripcion { get; set; }
        public int TotalModulos { get; set; }
        public int TotalPantallas { get; set; }
        public int TotalUsuariosConAcceso { get; set; }
    }

    // ===== CLASES DE RESULTADO PARA EVITAR DYNAMIC =====
    
    /// <summary>
    /// Resultado del login extendido con usuarios y roles
    /// </summary>
    public class LoginExtendidoResult
    {
        public UsuarioViewModel Usuario { get; set; }
        public List<PR_Seguridad_Usuarios_Login_V2RolesResult> Roles { get; set; }
    }

    /// <summary>
    /// Resultado de pantallas por usuario/rol con componentes, módulos y pantallas
    /// </summary>
    public class PantallasUsuarioResult
    {
        public List<PR_Seguridad_PantallasPorRol_ComponentesResult> Componentes { get; set; }
        public List<PR_Seguridad_PantallasPorRol_ModulosResult> Modulos { get; set; }
        public List<PR_Seguridad_PantallasPorRol_PantallasResult> Pantallas { get; set; }
    }

    /// <summary>
    /// Resultado de permisos de sesión para evitar reflexión
    /// </summary>
    public class PermisosSessionResult
    {
        public string PermisosJson { get; set; }
        public string PantallasString { get; set; }
        public Dictionary<string, List<string>> PermisosPorPantalla { get; set; }
    }
}