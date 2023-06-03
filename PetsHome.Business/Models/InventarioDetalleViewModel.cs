using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para el detalle de inventario.
    /// </summary>
    public partial class InventarioDetalleViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del detalle de inventario.
        /// </summary>
        [Key]
        [Display(Name = "Id inventario detalle")]
        public int invdet_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del inventario.
        /// </summary>
        [Display(Name = "Id inventario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? inv_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del item.
        /// </summary>
        [Display(Name = "Id Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? itm_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la existencia en el inventario.
        /// </summary>
        [Display(Name = "Existencia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? invdet_Existencia { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de entradas en el inventario.
        /// </summary>
        [Display(Name = "Entradas")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? invdet_Entradas { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de salidas en el inventario.
        /// </summary>
        [Display(Name = "Salidas")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? invdet_Salidas { get; set; }

        /// <summary>
        /// Obtiene o establece el stock actual en el inventario.
        /// </summary>
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? invdet_Stock { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el detalle de inventario.
        /// </summary>
        public int invdet_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el detalle de inventario.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? invdet_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del detalle de inventario.
        /// </summary>
        public DateTime invdet_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el detalle de inventario.
        /// </summary>
        public int? invdet_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el detalle de inventario.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? invdet_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del detalle de inventario.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? invdet_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.invdet_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}