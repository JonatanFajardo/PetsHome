using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class ProcedenciaViewModel
    {
        [Key]
        [Display(Name = "Id procedencia")]
        public int proc_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string proc_Descripcion { get; set; }

        public int proc_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? proc_NombreUsuarioCrea { get; set; }

        public DateTime proc_FechaCrea { get; set; }
        public int? proc_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? proc_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? proc_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.proc_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}