using Microsoft.AspNetCore.Mvc.Rendering;
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
        public TimeSpan eve_HoraInicio { get; set; }
        public TimeSpan eve_HoraFinal { get; set; }

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime eve_Fecha { get; set; }
        public int emp_UsuarioCrea { get; set; }
        public DateTime emp_FechaCrea { get; set; }
        public int? emp_UsuarioModifica { get; set; }
        [Display(Name = "Fecha modificación")]
        public DateTime? emp_FechaModifica { get; set; }

        // Propiedades Extras
        public string refg_Nombre { get; set; }

        [Display(Name = "Usuario creación")]
        public string? emp_NombreUsuarioCrea { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? emp_NombreUsuarioModifica { get; set; }


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

        #region Dropdown
        public SelectList refugioList { get; set; }

        public void LoadDropDownList(IEnumerable<RefugioViewModel> refugioDropdownResults)
        {
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
        }
        #endregion Dropdown
    }
}
