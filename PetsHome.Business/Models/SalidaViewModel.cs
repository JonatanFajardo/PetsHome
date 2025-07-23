using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetsHome.Business.Data;
using PetsHome.Common.InternalEntities;

namespace PetsHome.Business.Models
{
    public partial class SalidaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la salida.
        /// </summary>
        [Key]
        [Display(Name = "Id Salida")]
        public int sal_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la salida.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string sal_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la salida.
        /// </summary>
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? sal_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de salida.
        /// </summary>
        [Display(Name = "Tipo de Salida")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string sal_TipoSalida { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del destino.
        /// </summary>
        [Display(Name = "Destino")]
        public int? sal_DestinoId { get; set; }

        /// <summary>
        /// Obtiene o establece el número de documento.
        /// </summary>
        [Display(Name = "Número de Documento")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string sal_NumeroDocumento { get; set; }

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
        /// Obtiene o establece el ID del usuario que crea la salida.
        /// </summary>
        public int? sal_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la salida.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? sal_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la salida.
        /// </summary>
        public DateTime sal_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la salida.
        /// </summary>
        public int? sal_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la salida.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? sal_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la salida.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? sal_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.sal_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        /// <summary>
        /// Lista desplegable de tipos de salida.
        /// </summary>
        public SelectList tipoSalidaList { get; set; }

        /// <summary>
        /// Lista desplegable de refugios.
        /// </summary>
        public SelectList refugioList { get; set; }

        /// <summary>
        /// Carga las listas desplegables para los formularios.
        /// </summary>
        /// <param name="tipoSalidaDropdownResults">Resultados de la lista desplegable de tipos de salida.</param>
        /// <param name="refugioDropdownResults">Resultados de la lista desplegable de refugios.</param>
        public void LoadDropDownList(IEnumerable<Dropdown> tipoSalidaDropdownResults,
                                    IEnumerable<RefugioViewModel> refugioDropdownResults)
        {
            tipoSalidaList = new SelectList(tipoSalidaDropdownResults, "Value", "Text");
            refugioList = new SelectList(refugioDropdownResults, "refg_Id", "refg_Nombre");
        }

        #endregion Dropdown
    }
}