using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public class RolViewModel
    {
        [Key]
        public int rol_Id { get; set; }

        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(100)]
        [Display(Name = "Descripcion")]
        public string rol_Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool rol_Estado { get; set; }

        public int CantidadPantallas { get; set; }
    }

    public class RolDropdownViewModel
    {
        public int rol_Id { get; set; }
        public string rol_Descripcion { get; set; }
    }

    public class RolConPantallasViewModel
    {
        public int rol_Id { get; set; }
        public string pantallaIds { get; set; }
    }
}
