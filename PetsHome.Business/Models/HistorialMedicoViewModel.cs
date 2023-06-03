using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para el historial médico de una mascota.
    /// </summary>
    public partial class HistorialMedicoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la ficha médica.
        /// </summary>
        [Key]
        [Display(Name = "Id ficha medica")]
        public int medic_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la mascota.
        /// </summary>
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? masc_Id { get; set; }

        /// <summary>
        /// Obtiene o establece si la mascota ha sido esterilizada.
        /// </summary>
        [Display(Name = "Esterilizacion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool? medic_Esterilizacion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del comportamiento.
        /// </summary>
        [Display(Name = "Comportamiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(255)]
        public int com_Id { get; set; }

        /// <summary>
        /// Obtiene o establece información sobre el cuidado de la salud de la mascota.
        /// </summary>
        [Display(Name = "Salud Cuidado")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(255)]
        public string medic_SaludCuidado { get; set; }

        /// <summary>
        /// Obtiene o establece información adicional sobre el historial médico de la mascota.
        /// </summary>
        [Display(Name = "Informacion Adicional")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(255)]
        public string medic_InformacionAdicional { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea el historial médico.
        /// </summary>
        public int medic_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea el historial médico.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? medic_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del historial médico.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime medic_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica el historial médico.
        /// </summary>
        public int? medic_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica el historial médico.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? medic_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del historial médico.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? medic_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public Boolean isEdit
        {
            get
            {
                if (this.medic_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}