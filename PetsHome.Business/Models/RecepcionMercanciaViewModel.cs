using Microsoft.AspNetCore.Mvc.Rendering;
using PetsHome.Common.InternalEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    public partial class RecepcionMercanciaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la recepción de mercancía.
        /// </summary>
        [Key]
        [Display(Name = "Id Recepción")]
        public int recep_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la recepción.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string recep_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la recepción.
        /// </summary>
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? recep_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de recepción.
        /// </summary>
        [Display(Name = "Tipo de Recepción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string recep_TipoRecepcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del origen.
        /// </summary>
        [Display(Name = "Origen")]
        public int? recep_OrigenId { get; set; }

        /// <summary>
        /// Obtiene o establece el número de documento.
        /// </summary>
        [Display(Name = "Número de Documento")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string recep_NumeroDocumento { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio.
        /// </summary>
        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del refugio.
        /// </summary>
        [Display(Name = "Refugio")]
        public string? refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la recepción.
        /// </summary>
        public int? recep_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la recepción.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? recep_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la recepción.
        /// </summary>
        public DateTime recep_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la recepción.
        /// </summary>
        public int? recep_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la recepción.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? recep_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la recepción.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? recep_FechaModifica { get; set; }
        public int CantidadItems { get; set; }
        public decimal TotalProductos { get; set; }
        public string TipoRecepcionDescripcion { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.recep_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        /// <summary>
        /// Obtiene o establece la lista desplegable de tipos de recepción.
        /// </summary>
        public SelectList tipoRecepcionList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de refugios.
        /// </summary>
        public SelectList refugioList { get; set; }

        /// <summary>
        /// Carga las listas desplegables con los datos proporcionados.
        /// </summary>
        /// <param name="tipoRecepcionDropdownResults">Resultados de la lista desplegable de tipos de recepción.</param>
        /// <param name="refugioDropdownResults">Resultados de la lista desplegable de refugios.</param>
        public void LoadDropDownList(IEnumerable<Dropdown> tipoRecepcionDropdownResults,
                                    IEnumerable<RefugioViewModel> refugioDropdownResults)
        {
            tipoRecepcionList = new SelectList(tipoRecepcionDropdownResults, "Value", "Text");
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
        }

        #endregion Dropdown
    }
}