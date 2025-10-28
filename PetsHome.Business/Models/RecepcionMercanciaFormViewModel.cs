using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para formularios de creación y edición de recepciones de mercancía.
    /// </summary>
    public class RecepcionMercanciaFormViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la recepción.
        /// </summary>
        [Key]
        [Display(Name = "Id Recepción")]
        public int recep_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la recepción.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo descripción es requerido")]
        [StringLength(500, ErrorMessage = "La descripción debe tener un máximo de {1} caracteres")]
        public string recep_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la recepción.
        /// </summary>
        [Display(Name = "Fecha de Recepción")]
        [Required(ErrorMessage = "El campo fecha es requerido")]
        [DataType(DataType.Date)]
        public DateTime recep_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del refugio.
        /// </summary>
        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo refugio es requerido")]
        public int refg_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el tipo de recepción.
        /// </summary>
        [Display(Name = "Tipo de Recepción")]
        [Required(ErrorMessage = "El campo tipo de recepción es requerido")]
        [StringLength(50, ErrorMessage = "El tipo de recepción debe tener un máximo de {1} caracteres")]
        public string recep_TipoRecepcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del origen/procedencia.
        /// </summary>
        [Display(Name = "Origen")]
        public int? recep_OrigenId { get; set; }

        /// <summary>
        /// Obtiene o establece el número de documento asociado.
        /// </summary>
        [Display(Name = "Número de Documento")]
        [StringLength(50, ErrorMessage = "El número de documento debe tener un máximo de {1} caracteres")]
        public string recep_NumeroDocumento { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la recepción.
        /// </summary>
        public int? recep_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la recepción.
        /// </summary>
        public int? recep_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de detalles asociados a la recepción.
        /// </summary>
        public List<RecepcionDetalleListViewModel> ListadoDetalles { get; set; }

        /// <summary>
        /// Obtiene o establece el detalle asociado a la recepción (para formulario anidado).
        /// </summary>
        public RecepcionDetalleFormViewModel Detalle { get; set; } = new RecepcionDetalleFormViewModel();

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                return this.recep_Id != 0;
            }
        }
    }
}
