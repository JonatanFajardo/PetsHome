using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para una adopción.
    /// </summary>
    public partial class AdopcionViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la ficha de adopción.
        /// </summary>
        [Key]
        [Display(Name = "Id Ficha")]
        public int adop_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la solicitud.
        /// </summary>
        [Display(Name = "Id solicitud")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int sol_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la adopción.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "date")]
        public DateTime adop_FechaRegistro { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la adopción está aprobada.
        /// </summary>
        [Display(Name = "Es aprobado")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool adop_EsAprobado { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la adopción.
        /// </summary>
        public int adop_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la adopción.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? adop_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la adopción.
        /// </summary>
        public DateTime adop_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la adopción.
        /// </summary>
        public int? adop_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la adopción.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? adop_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la adopción.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? adop_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.adop_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}