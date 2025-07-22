using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    public partial class ExistenciaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la existencia.
        /// </summary>
        [Key]
        [Display(Name = "Id Existencia")]
        public int exist_Id { get; set; }

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
        /// Obtiene o establece la categoría del ítem.
        /// </summary>
        [Display(Name = "Categoría")]
        public string? cat_Descripcion { get; set; }

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
        /// Obtiene o establece el stock actual.
        /// </summary>
        [Display(Name = "Stock Actual")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int exist_Stock { get; set; }

        /// <summary>
        /// Obtiene o establece el stock mínimo.
        /// </summary>
        [Display(Name = "Stock Mínimo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        public int exist_StockMinimo { get; set; }

        /// <summary>
        /// Obtiene o establece el stock máximo.
        /// </summary>
        [Display(Name = "Stock Máximo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El stock máximo debe ser mayor a 0")]
        public int exist_StockMaximo { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de última actualización.
        /// </summary>
        [Display(Name = "Última Actualización")]
        public DateTime exist_FechaActualizacion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la existencia.
        /// </summary>
        public int? exist_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la existencia.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? exist_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la existencia.
        /// </summary>
        public DateTime exist_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la existencia.
        /// </summary>
        public int? exist_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la existencia.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? exist_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la existencia.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? exist_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el stock está por debajo del mínimo.
        /// </summary>
        public bool StockBajo => exist_Stock <= exist_StockMinimo;

        /// <summary>
        /// Indica el estado del stock.
        /// </summary>
        public string EstadoStock
        {
            get
            {
                if (exist_Stock == 0) return "Sin Stock";
                if (exist_Stock <= exist_StockMinimo) return "Stock Bajo";
                if (exist_Stock >= exist_StockMaximo) return "Stock Alto";
                return "Stock Normal";
            }
        }

        /// <summary>
        /// Obtiene el color del badge según el estado del stock.
        /// </summary>
        public string ColorEstado
        {
            get
            {
                if (exist_Stock == 0) return "danger";
                if (exist_Stock <= exist_StockMinimo) return "warning";
                if (exist_Stock >= exist_StockMaximo) return "info";
                return "success";
            }
        }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.exist_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}