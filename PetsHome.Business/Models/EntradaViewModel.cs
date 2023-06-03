using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para una entrada.
    /// </summary>
    public partial class EntradaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la entrada.
        /// </summary>
        [Key]
        [Display(Name = "Id entrada")]
        public int ent_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio asociado a la entrada.
        /// </summary>
        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del refugio asociado a la entrada.
        /// </summary>
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del item asociado a la entrada.
        /// </summary>
        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? item_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del item asociado a la entrada.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? item_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la entrada.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string ent_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la entrada.
        /// </summary>
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? ent_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de la entrada.
        /// </summary>
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? ent_Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la entrada.
        /// </summary>
        public int? ent_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la entrada.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? ent_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la entrada.
        /// </summary>
        public DateTime ent_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la entrada.
        /// </summary>
        public int? ent_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la entrada.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? ent_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la entrada.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? ent_FechaModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de detalles de entradas.
        /// </summary>
        public List<EntradaDetalleViewModel> ListadoEntradasDetalles { get; set; }

        /// <summary>
        /// Obtiene o establece el detalle de entrada.
        /// </summary>
        public EntradaDetalleViewModel EntradaDetalle { get; set; } = new EntradaDetalleViewModel();

        /// <summary>
        /// Obtiene un valor que indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.ent_Id == 0)
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