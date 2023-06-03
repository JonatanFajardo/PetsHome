using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista para el registro de voluntariado en un evento.
    /// </summary>
    public partial class RegistroVoluntariadoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del evento.
        /// </summary>
        [Key]
        [Display(Name = "Id evento")]
        public int eve_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del voluntario.
        /// </summary>
        [Display(Name = "Id voluntario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? vol_Id { get; set; }

        /// <summary>
        /// Obtiene o establece los nombres del voluntario.
        /// </summary>
        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_Nombres { get; set; }

        /// <summary>
        /// Obtiene o establece los apellidos del voluntario.
        /// </summary>
        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_Apellidos { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de inicio del voluntariado.
        /// </summary>
        [Display(Name = "Hora inicio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "time(2)")]
        public TimeSpan? rvo_HoraInicio { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de finalización del voluntariado.
        /// </summary>
        [Display(Name = "Hora final")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "time(2)")]
        public TimeSpan? rvo_HoraFinal { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha del voluntariado.
        /// </summary>
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "date")]
        public DateTime? rvo_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación del registro.
        /// </summary>
        public DateTime? fechaCreacion { get; set; }

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
                if (this.eve_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}