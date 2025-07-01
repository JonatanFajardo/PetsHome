using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para un evento.
    /// </summary>
    public partial class EventoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del evento.
        /// </summary>
        [Key]
        public int eve_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del evento.
        /// </summary>
        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string eve_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio asociado al evento.
        /// </summary>
        public int refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de inicio del evento.
        /// </summary>
        public TimeSpan eve_HoraInicio { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de finalización del evento.
        /// </summary>
        public TimeSpan eve_HoraFinal { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha del evento.
        /// </summary>
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime eve_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el evento.
        /// </summary>
        public int emp_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del evento.
        /// </summary>
        public DateTime emp_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el evento.
        /// </summary>
        public int? emp_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del evento.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? emp_FechaModifica { get; set; }

        // Propiedades Extras

        /// <summary>
        /// Obtiene o establece el nombre del refugio asociado al evento.
        /// </summary>
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el evento.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? emp_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el evento.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? emp_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene un valor que indica si el formulario se está editando.
        /// </summary>
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

        /// <summary>
        /// Obtiene o establece la lista desplegable de refugios.
        /// </summary>
        public SelectList refugioList { get; set; }

        /// <summary>
        /// Carga los datos de la lista desplegable.
        /// </summary>
        /// <param name="refugioDropdownResults">Resultados de la lista desplegable de refugios.</param>
        public void LoadDropDownList(IEnumerable<RefugioViewModel> refugioDropdownResults)
        {
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
        }

        #endregion Dropdown
    }
}