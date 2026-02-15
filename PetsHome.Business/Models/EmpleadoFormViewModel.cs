using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para crear o editar empleados.
    /// </summary>
    public class EmpleadoFormViewModel
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

        [Display(Name = "Estado")]
        public bool emp_EsActivo { get; set; }

        public int emp_UsuarioCrea { get; set; }

        public DateTime emp_FechaCrea { get; set; }

        public int? emp_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? emp_FechaModifica { get; set; }

        public string cag_Descripcion { get; set; }

        public string refg_Nombre { get; set; }

        public string esActivo { get; set; }

        public PersonaViewModel per { get; set; } = new PersonaViewModel();

        public bool isEdit => emp_Id != 0;

        #region Dropdown

        public SelectList refugioList { get; set; }

        public SelectList empleadoCargoList { get; set; }

        public void LoadDropDownList(IEnumerable<RefugioDropdownViewModel> refugioDropdownResults,
                                     IEnumerable<EmpleadoCargoViewModel> empleadoCargoDropdownResults)
        {
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
            empleadoCargoList = new SelectList(empleadoCargoDropdownResults, "cag_Id", "cag_Descripcion");
        }

        #endregion Dropdown
    }
}
