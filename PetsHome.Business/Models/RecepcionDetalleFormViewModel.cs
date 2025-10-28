using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para formularios de creación y edición de detalles de recepción.
    /// </summary>
    public class RecepcionDetalleFormViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del detalle de recepción.
        /// </summary>
        [Key]
        [Display(Name = "Id Detalle")]
        public int recdet_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la recepción.
        /// </summary>
        [Display(Name = "Id Recepción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int recep_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del item.
        /// </summary>
        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo item es requerido")]
        public int itm_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad.
        /// </summary>
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo cantidad es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal recdet_Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece el precio unitario.
        /// </summary>
        [Display(Name = "Precio Unitario")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal? recdet_PrecioUnitario { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de vencimiento.
        /// </summary>
        [Display(Name = "Fecha de Vencimiento")]
        [DataType(DataType.Date)]
        public DateTime? recdet_FechaVencimiento { get; set; }

        /// <summary>
        /// Obtiene o establece el número de lote.
        /// </summary>
        [Display(Name = "Número de Lote")]
        [StringLength(50, ErrorMessage = "El número de lote debe tener un máximo de {1} caracteres")]
        public string recdet_NumeroLote { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el detalle.
        /// </summary>
        public int? recdet_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el detalle.
        /// </summary>
        public int? recdet_UsuarioModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                return this.recdet_Id != 0;
            }
        }
    }
}
