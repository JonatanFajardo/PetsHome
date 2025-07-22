using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    public partial class RecepcionDetalleViewModel
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
        [Display(Name = "Recepción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int recep_Id { get; set; }

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
        /// Obtiene o establece la cantidad recibida.
        /// </summary>
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int recdet_Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece el precio unitario.
        /// </summary>
        [Display(Name = "Precio Unitario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal recdet_PrecioUnitario { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de vencimiento.
        /// </summary>
        [Display(Name = "Fecha Vencimiento")]
        [Column(TypeName = "datetime")]
        public DateTime? recdet_FechaVencimiento { get; set; }

        /// <summary>
        /// Obtiene o establece el número de lote.
        /// </summary>
        [Display(Name = "Número de Lote")]
        [StringLength(50, ErrorMessage = "El campo {0} no puede exceder {1} caracteres")]
        public string? recdet_NumeroLote { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el detalle.
        /// </summary>
        public int? recdet_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el detalle.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? recdet_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del detalle.
        /// </summary>
        public DateTime recdet_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el detalle.
        /// </summary>
        public int? recdet_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el detalle.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? recdet_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del detalle.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? recdet_FechaModifica { get; set; }

        /// <summary>
        /// Calcula el valor total del detalle.
        /// </summary>
        public decimal ValorTotal => recdet_Cantidad * recdet_PrecioUnitario;

        /// <summary>
        /// Indica si el producto tiene fecha de vencimiento próxima (30 días).
        /// </summary>
        public bool VencimientoProximo
        {
            get
            {
                if (!recdet_FechaVencimiento.HasValue) return false;
                return recdet_FechaVencimiento.Value <= DateTime.Now.AddDays(30);
            }
        }

        /// <summary>
        /// Indica si el producto está vencido.
        /// </summary>
        public bool Vencido
        {
            get
            {
                if (!recdet_FechaVencimiento.HasValue) return false;
                return recdet_FechaVencimiento.Value < DateTime.Now;
            }
        }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.recdet_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}