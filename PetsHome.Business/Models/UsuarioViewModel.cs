using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para Usuario autenticado
    /// </summary>
    public class UsuarioViewModel
    {
        /// <summary>
        /// ID del usuario
        /// </summary>
        [Key]
        public int usu_Id { get; set; }

        /// <summary>
        /// ID del empleado asociado
        /// </summary>
        public int Emp_Id { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Display(Name = "Usuario")]
        public string usu_NombreUsuario { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        [Display(Name = "Nombre Completo")]
        public string usu_NombreCompleto { get; set; }

        /// <summary>
        /// Primer nombre
        /// </summary>
        public string per_PrimerNombre { get; set; }

        /// <summary>
        /// Apellido paterno
        /// </summary>
        public string per_ApellidoPaterno { get; set; }

        /// <summary>
        /// ID del rol
        /// </summary>
        public int? rol_Id { get; set; }

        /// <summary>
        /// Descripción del rol
        /// </summary>
        [Display(Name = "Rol")]
        public string rol_Descripcion { get; set; }

        /// <summary>
        /// Estado del usuario
        /// </summary>
        public bool usu_Estado { get; set; }

        /// <summary>
        /// Ruta de la imagen de perfil
        /// </summary>
        public string usu_ImagenPerfil { get; set; }

        /// <summary>
        /// Indica si el usuario está logueado
        /// </summary>
        public bool? usu_Logueado { get; set; }
    }
}
