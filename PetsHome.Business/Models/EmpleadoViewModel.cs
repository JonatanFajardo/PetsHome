using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para un empleado.
    /// </summary>
    public partial class EmpleadoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del empleado.
        /// </summary>
        [Key]
        public int emp_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del empleado.
        /// </summary>
        [Display(Name = "Codigo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string emp_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio al que pertenece el empleado.
        /// </summary>
        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del cargo del empleado.
        /// </summary>
        [Display(Name = "Cargo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int cag_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el estado del empleado.
        /// </summary>
        public bool emp_EsActivo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el empleado.
        /// </summary>
        public int emp_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del empleado.
        /// </summary>
        public DateTime emp_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el empleado.
        /// </summary>
        public int? emp_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del empleado.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? emp_FechaModifica { get; set; }

        /// <summary>
        /// Obtiene o establece los nombres del empleado.
        /// </summary>
        public string emp_Nombres { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del cargo del empleado.
        /// </summary>
        public string cag_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del refugio al que pertenece el empleado.
        /// </summary>
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el estado del empleado como texto.
        /// </summary>
        public string esActivo { get; set; }

        /// <summary>
        /// Obtiene o establece la identidad de la persona asociada al empleado.
        /// </summary>
        public string per_Identidad { get; set; }

        /// <summary>
        /// Obtiene o establece la información de la persona asociada al empleado.
        /// </summary>
        public virtual PersonaViewModel per { get; set; } = new PersonaViewModel();

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
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

        /// <summary>
        /// Obtiene o establece la lista desplegable de refugios.
        /// </summary>
        public SelectList refugioList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de cargos de empleados.
        /// </summary>
        public SelectList empleadoCargoList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de procedencias.
        /// </summary>
        public SelectList procedenciaList { get; set; }

        /// <summary>
        /// Carga los elementos de las listas desplegables.
        /// </summary>
        /// <param name="refugioDropdownResults">Resultados de la lista desplegable de refugios.</param>
        /// <param name="empleadoCargoDropdownResults">Resultados de la lista desplegable de cargos de empleados.</param>
        public void LoadDropDownList(IEnumerable<RefugioViewModel> refugioDropdownResults,
                                       IEnumerable<EmpleadoCargoViewModel> empleadoCargoDropdownResults)
        {
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
            empleadoCargoList = new SelectList(empleadoCargoDropdownResults, "cag_Id", "cag_Descripcion");
        }

        #endregion Dropdown
    }
}