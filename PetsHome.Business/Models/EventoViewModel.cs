using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetsHome.Business.Models
{
    public partial class EventoViewModel
    {
        [Key]
        public int eve_Id { get; set; }
        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string eve_Descripcion { get; set; }
        public int refg_Id { get; set; }
        public DateTime eve_HoraInicio { get; set; }
        public DateTime eve_HoraFinal { get; set; }
        public DateTime eve_Fecha { get; set; }
        public int emp_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? emp_NombreUsuarioCrea { get; set; }
        public DateTime emp_FechaCrea { get; set; }
        public int? emp_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? emp_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? emp_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.eve_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}
