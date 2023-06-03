using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista para la salida de un item.
    /// </summary>
    public partial class SalidaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la salida.
        /// </summary>
        [Key]
        [Display(Name = "Id salida")]
        public int sal_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del item.
        /// </summary>
        [Display(Name = "Id item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? item_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del item.
        /// </summary>
        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string itm_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la salida.
        /// </summary>
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? sal_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de items en la salida.
        /// </summary>
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? sal_Cantidad { get; set; }

        /// <summary>
        /// Obtiene o establece si el registro ha sido eliminado.
        /// </summary>
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool? esEliminado { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del registro.
        /// </summary>
        [Display(Name = "Fecha creación")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime fechaCreacion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del registro.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? fechaModificacion { get; set; }

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
    }
}