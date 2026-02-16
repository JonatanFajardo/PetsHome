using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class GravedadViewModel
    {
        [Key]
        [Display(Name = "Id gravedad")]
        public int grav_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string grav_Descripcion { get; set; }

        public string EsActivo { get; set; }

        [Display(Name = "Estado")]
        public bool grav_EsActivo { get; set; }

        public int grav_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string grav_NombreUsuarioCrea { get; set; }

        public DateTime grav_FechaCrea { get; set; }

        public int? grav_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string grav_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? grav_FechaModifica { get; set; }

        public bool isEdit
        {
            get
            {
                if (this.grav_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
