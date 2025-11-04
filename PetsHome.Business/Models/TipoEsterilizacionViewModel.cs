using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class TipoEsterilizacionViewModel
    {
        [Key]
        [Display(Name = "Id tipo de esterilización")]
        public int tipoEst_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string tipoEst_Descripcion { get; set; }

        [Display(Name = "Sexo")]
        [StringLength(10)]
        public string tipoEst_Sexo { get; set; }

        public int tipoEst_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string tipoEst_NombreUsuarioCrea { get; set; }

        public DateTime tipoEst_FechaCrea { get; set; }

        public int? tipoEst_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string tipoEst_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? tipoEst_FechaModifica { get; set; }

        public bool isEdit
        {
            get
            {
                if (this.tipoEst_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
