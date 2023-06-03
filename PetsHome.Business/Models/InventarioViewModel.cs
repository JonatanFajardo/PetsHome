using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    public partial class InventarioViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del inventario.
        /// </summary>
        [Key]
        [Display(Name = "Id Inventario")]
        public int inv_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha del inventario.
        /// </summary>
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? inv_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el inventario.
        /// </summary>
        public int? inv_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el inventario.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? inv_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del inventario.
        /// </summary>
        public DateTime inv_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el inventario.
        /// </summary>
        public int? inv_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el inventario.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? inv_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del inventario.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? inv_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.inv_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}