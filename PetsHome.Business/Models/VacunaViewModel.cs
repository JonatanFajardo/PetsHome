using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class VacunaViewModel
    {
        [Key]
        [Display(Name = "Id vacuna")]
        public int vac_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string vac_Descripcion { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool? esEliminado { get; set; }

        public int vac_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? vac_NombreUsuarioCrea { get; set; }

        public DateTime vac_FechaCrea { get; set; }
        public int? vac_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? vac_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? vac_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.vac_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}