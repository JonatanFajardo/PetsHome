using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class TipoParasitoViewModel
    {
        [Key]
        [Display(Name = "Id tipo de parásito")]
        public int tipoPar_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string tipoPar_Descripcion { get; set; }

        [Display(Name = "Categoría")]
        [StringLength(50)]
        public string tipoPar_Categoria { get; set; }

        public string tipoPar_EsActivo { get; set; }

        [Display(Name = "Estado")]
        public bool tipoPar_EsActivoBool { get; set; }

        public int tipoPar_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string tipoPar_NombreUsuarioCrea { get; set; }

        public DateTime tipoPar_FechaCrea { get; set; }

        public int? tipoPar_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string tipoPar_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? tipoPar_FechaModifica { get; set; }

        public bool isEdit
        {
            get
            {
                if (this.tipoPar_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
