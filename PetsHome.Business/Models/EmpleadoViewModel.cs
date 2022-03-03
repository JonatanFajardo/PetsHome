using Microsoft.AspNetCore.Mvc.Rendering;
using PetsHome.Common.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace PetsHome.Business.Models
{
    public partial class EmpleadoViewModel
    {
        [Key]
        public int emp_Id { get; set; }

        [Display(Name = "Codigo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string emp_Codigo { get; set; }
        
        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int refg_Id { get; set; }

        [Display(Name = "Cargo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int cag_Id { get; set; }

        public bool emp_EsActivo { get; set; }

        public int emp_UsuarioCrea { get; set; }

        public DateTime emp_FechaCrea { get; set; }
        public int? emp_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? emp_FechaModifica { get; set; }


        // Propiedades Extras
        public string emp_Nombres { get; set; }
        public string cag_Descripcion { get; set; }
        public string refg_Nombre { get; set; }
        public string esActivo { get; set; }

        //public string emp_EsActivo { get; set; }
        public string per_Identidad { get; set; }
        public virtual PersonaViewModel per { get; set; } = new PersonaViewModel();
       

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.emp_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown
        public SelectList refugioList { get; set; }
        public SelectList empleadoCargoList { get; set; }
        public SelectList procedenciaList { get; set; }

         public void LoadDropDownList(IEnumerable<RefugioViewModel> refugioDropdownResults,
                                        IEnumerable<EmpleadoCargoViewModel> empleadoCargoDropdownResults)
        {
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
            empleadoCargoList = new SelectList(empleadoCargoDropdownResults, "cag_Id", "cag_Descripcion");
            
        }
        #endregion Dropdown

    }
}