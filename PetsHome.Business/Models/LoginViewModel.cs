using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para el formulario de login
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Usuario")]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Recordar sesión
        /// </summary>
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// URL de retorno después del login
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
