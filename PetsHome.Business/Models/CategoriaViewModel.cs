using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para una categoría.
    /// </summary>
    public partial class CategoriaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la categoría.
        /// </summary>
        [Key]
        [Display(Name = "Id")]
        public int cat_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la categoría.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string cat_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la categoría.
        /// </summary>
        public int cat_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la categoría.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la categoría.
        /// </summary>
        [Display(Name = "Fecha creación")]
        public DateTime cat_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la categoría.
        /// </summary>
        public int? cat_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la categoría.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? cat_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la categoría.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? cat_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.cat_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}