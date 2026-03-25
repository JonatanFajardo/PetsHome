using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public class UsuarioCrudViewModel
    {
        [Key]
        public int usu_Id { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(100)]
        [Display(Name = "Nombre de usuario")]
        public string Usu_Nombre { get; set; }

        [Display(Name = "Empleado")]
        public int Emp_Id { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [Display(Name = "Rol")]
        public int Rol_Id { get; set; }

        [Display(Name = "Rol")]
        public string rol_Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool Usu_EsActivo { get; set; }
    }
}
