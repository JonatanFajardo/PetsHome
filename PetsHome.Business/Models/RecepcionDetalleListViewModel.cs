using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para listar detalles de recepción en DataTable.
    /// </summary>
    public class RecepcionDetalleListViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del detalle de recepción.
        /// </summary>
        [Display(Name = "Id")]
        public int recdet_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la recepción.
        /// </summary>
        [Display(Name = "Id Recepción")]
        public int recep_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del item.
        /// </summary>
        [Display(Name = "Id Item")]
        public int itm_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del item.
        /// </summary>
        [Display(Name = "Item")]
        public string itm_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad.
        /// </summary>
        [Display(Name = "Cantidad")]
        public decimal recdet_Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece el precio unitario.
        /// </summary>
        [Display(Name = "Precio Unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? recdet_PrecioUnitario { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de vencimiento.
        /// </summary>
        [Display(Name = "Vencimiento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? recdet_FechaVencimiento { get; set; }

        /// <summary>
        /// Obtiene o establece el número de lote.
        /// </summary>
        [Display(Name = "Lote")]
        public string recdet_NumeroLote { get; set; }
    }
}
