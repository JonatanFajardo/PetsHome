using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    public partial class SalidaDetalleViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del detalle de salida.
        /// </summary>
        [Key]
        [Display(Name = "Id Detalle")]
        public int saldet_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la salida.
        /// </summary>
        [Display(Name = "Salida")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int sal_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del ítem.
        /// </summary>
        [Display(Name = "Ítem")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int itm_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del ítem.
        /// </summary>
        [Display(Name = "Ítem")]
        public string? itm_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el código del ítem.
        /// </summary>
        [Display(Name = "Código")]
        public string? itm_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de salida.
        /// </summary>
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int saldet_Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece el precio unitario.
        /// </summary>
        [Display(Name = "Precio Unitario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal saldet_PrecioUnitario { get; set; }

        /// <summary>
        /// Obtiene o establece el motivo específico de la salida.
        /// </summary>
        [Display(Name = "Motivo")]
        [StringLength(200, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string? saldet_Motivo { get; set; }

        /// <summary>
        /// Obtiene o establece el stock disponible del ítem.
        /// </summary>
        [Display(Name = "Stock Disponible")]
        public int StockDisponible { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el detalle.
        /// </summary>
        public int? saldet_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el detalle.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? saldet_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del detalle.
        /// </summary>
        public DateTime saldet_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el detalle.
        /// </summary>
        public int? saldet_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el detalle.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? saldet_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del detalle.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? saldet_FechaModifica { get; set; }

        /// <summary>
        /// Calcula el valor total del detalle.
        /// </summary>
        public decimal ValorTotal => saldet_Cantidad * saldet_PrecioUnitario;

        /// <summary>
        /// Indica si hay stock suficiente para la cantidad solicitada.
        /// </summary>
        public bool StockSuficiente => StockDisponible >= saldet_Cantidad;

        /// <summary>
        /// Indica si la cantidad solicitada excede el stock disponible.
        /// </summary>
        public bool ExcedeStock => saldet_Cantidad > StockDisponible;

        /// <summary>
        /// Obtiene el porcentaje del stock que representa esta salida.
        /// </summary>
        public double PorcentajeStock
        {
            get
            {
                if (StockDisponible == 0) return 0;
                return ((double)saldet_Cantidad / StockDisponible) * 100;
            }
        }

        /// <summary>
        /// Indica si esta salida representa más del 50% del stock disponible.
        /// </summary>
        public bool SalidaSignificativa => PorcentajeStock > 50;

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.saldet_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}