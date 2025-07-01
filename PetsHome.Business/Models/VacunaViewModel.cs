using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista para una vacuna.
    /// </summary>
    public partial class VacunaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la vacuna.
        /// </summary>
        [Key]
        [Display(Name = "Id vacuna")]
        public int vac_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la vacuna.
        /// </summary>
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string vac_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece si la vacuna ha sido eliminada.
        /// </summary>
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool? esEliminado { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la vacuna.
        /// </summary>

        public int vac_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la vacuna.
        /// </summary>

        [Display(Name = "Usuario creación")]
        public string? vac_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la vacuna.
        /// </summary>
        public DateTime vac_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la vacuna.
        /// </summary>
        public int? vac_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la vacuna.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? vac_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la vacuna.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? vac_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.vac_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}