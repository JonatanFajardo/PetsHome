using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// LoginViewModel compatible con AHM_INSTA_HELP_ADM
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [Display(Name = "Usuario")]
        public string usu_NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string usu_Contraseña { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}