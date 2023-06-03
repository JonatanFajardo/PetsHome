using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para el detalle de una entrada.
    /// </summary>
    public class EntradaDetalleViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del detalle de entrada.
        /// </summary>
        public int entdet_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la entrada asociada al detalle.
        /// </summary>
        public int ent_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del item del detalle.
        /// </summary>
        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int itm_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del item del detalle.
        /// </summary>
        public string itm_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad del detalle.
        /// </summary>
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int entdet_Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el detalle de entrada.
        /// </summary>
        public int entdet_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el detalle de entrada.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? entdet_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del detalle de entrada.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime entdet_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el detalle de entrada.
        /// </summary>
        public int? entdet_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el detalle de entrada.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? entdet_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del detalle de entrada.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? entdet_FechaModifica { get; set; }

        #region Dropdown

        /// <summary>
        /// Obtiene o establece la lista desplegable de items.
        /// </summary>
        public SelectList itemList { get; set; }

        /// <summary>
        /// Carga los elementos de la lista desplegable de items.
        /// </summary>
        /// <param name="itemDropdownResults">Resultados de la lista desplegable de items.</param>
        public void LoadDropDownList(IEnumerable<ItemViewModel> itemDropdownResults)
        {
            itemList = new SelectList(itemDropdownResults, "itm_Id", "itm_Descripcion");
        }

        #endregion Dropdown

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.entdet_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}