using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class ViaAdministracionViewModel
    {
        [Key]
        [Display(Name = "Id vía de administración")]
        public int viaAdmin_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string viaAdmin_Descripcion { get; set; }

        public string EsActivo { get; set; }

        [Display(Name = "Estado")]
        public bool viaAdmin_EsActivo { get; set; }

        public int viaAdmin_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string viaAdmin_NombreUsuarioCrea { get; set; }

        public DateTime viaAdmin_FechaCrea { get; set; }

        public int? viaAdmin_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string viaAdmin_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? viaAdmin_FechaModifica { get; set; }

        public bool isEdit
        {
            get
            {
                if (this.viaAdmin_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
