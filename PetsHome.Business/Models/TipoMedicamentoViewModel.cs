using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class TipoMedicamentoViewModel
    {
        [Key]
        [Display(Name = "Id tipo de medicamento")]
        public int tipoMed_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string tipoMed_Descripcion { get; set; }

        public string tipoMed_EsActivo { get; set; }

        [Display(Name = "Estado")]
        public bool tipoMed_EsActivoBool { get; set; }

        public int tipoMed_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string tipoMed_NombreUsuarioCrea { get; set; }

        public DateTime tipoMed_FechaCrea { get; set; }

        public int? tipoMed_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string tipoMed_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? tipoMed_FechaModifica { get; set; }

        public bool isEdit
        {
            get
            {
                if (this.tipoMed_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
