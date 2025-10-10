using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public class RegistroUsuarioViewModel
    {
        [Required(ErrorMessage = "El empleado es requerido")]
        [Display(Name = "Empleado")]
        public int Emp_Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(150, ErrorMessage = "El nombre de usuario no puede exceder 150 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string usu_NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string usu_Contraseña { get; set; }

        [Required(ErrorMessage = "Confirme la contraseña")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("usu_Contraseña", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; }

        [Required(ErrorMessage = "El rol es requerido")]
        [Display(Name = "Rol")]
        public int Rol_Id { get; set; }

        // Propiedades adicionales para mostrar información
        [Display(Name = "Empleado")]
        public string Emp_NombreCompleto { get; set; }

        [Display(Name = "Rol")]
        public string Rol_Descripcion { get; set; }
    }
}