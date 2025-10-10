using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public class UsuarioViewModel
    {
        public int usu_Id { get; set; }

        [Required(ErrorMessage = "El empleado es requerido")]
        [Display(Name = "Empleado")]
        public int Emp_Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(150, ErrorMessage = "El nombre de usuario no puede exceder 150 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string Usu_Nombre { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        [Display(Name = "Rol")]
        public int Rol_Id { get; set; }

        [Display(Name = "Dirección IP")]
        public string Usu_Ip { get; set; }

        [Display(Name = "Activo")]
        public bool? Usu_EsActivo { get; set; }

        [Display(Name = "Suspendido")]
        public bool? Usu_Suspendido { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime? Usu_FechaCreacion { get; set; }

        [Display(Name = "Fecha de Modificación")]
        public DateTime? Usu_fechaModificacion { get; set; }

        [Display(Name = "Imagen de Perfil")]
        public string usu_ImagenPerfil { get; set; }

        [Display(Name = "Logueado")]
        public bool? usu_Logueado { get; set; }

        [Display(Name = "Último Acceso")]
        public DateTime? usu_UltimoAcceso { get; set; }

        [Display(Name = "Intentos Fallidos")]
        public int? usu_IntentosFallidos { get; set; }

        [Display(Name = "Fecha de Bloqueo")]
        public DateTime? usu_FechaBloqueo { get; set; }

        // Propiedades adicionales para mostrar información relacionada
        [Display(Name = "Empleado")]
        public string Emp_NombreCompleto { get; set; }

        [Display(Name = "Rol")]
        public string Rol_Descripcion { get; set; }

    }
}