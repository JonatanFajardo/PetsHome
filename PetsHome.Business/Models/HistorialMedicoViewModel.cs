using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class HistorialMedicoViewModel
    {
        [Key]
        [Display(Name = "Id ficha medica")]
        public int medic_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? masc_Id { get; set; }

        [Display(Name = "Esterilizacion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool? medic_Esterilizacion { get; set; }

        [Display(Name = "Comportamiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(255)]
        public int com_Id { get; set; }

        [Display(Name = "Salud Cuidado")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(255)]
        public string medic_SaludCuidado { get; set; }

        [Display(Name = "Informacion Adicional")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(255)]
        public string medic_InformacionAdicional { get; set; }

        public int medic_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? medic_NombreUsuarioCrea { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime medic_FechaCrea { get; set; }

        public int? medic_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? medic_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? medic_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.medic_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}