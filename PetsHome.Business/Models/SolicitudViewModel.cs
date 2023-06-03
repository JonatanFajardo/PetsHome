using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista para una solicitud.
    /// </summary>
    public partial class SolicitudViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la solicitud.
        /// </summary>
        [Key]
        [Display(Name = "Id solicitud")]
        public int sol_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la identidad de la persona que realiza la solicitud.
        /// </summary>
        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(13)]
        public string sol_Identidad { get; set; }

        /// <summary>
        /// Obtiene o establece los nombres de la persona que realiza la solicitud.
        /// </summary>
        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string sol_Nombres { get; set; }

        /// <summary>
        /// Obtiene o establece los apellidos de la persona que realiza la solicitud.
        /// </summary>
        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string sol_Apellidos { get; set; }

        /// <summary>
        /// Obtiene o establece el número de teléfono de la persona que realiza la solicitud.
        /// </summary>
        [Display(Name = "Telefono")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8)]
        public string sol_Telefono { get; set; }

        /// <summary>
        /// Obtiene o establece el correo electrónico de la persona que realiza la solicitud.
        /// </summary>
        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string sol_Correo { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de la solicitud.
        /// </summary>
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime sol_Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la mascota en la solicitud.
        /// </summary>
        [Display(Name = "Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int masc_Id { get; set; }

        // Propiedades de mascota
        public byte[] masc_Imagen { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la mascota en la solicitud.
        /// </summary>
        [Display(Name = "Mascota")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string masc_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>

        public bool masc_EsAdoptado { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>
        public string raza_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>
        public string refg_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>

        public int sol_UsuarioCrea { get; set; }
        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>

        [Display(Name = "Usuario creación")]
        public string? sol_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>

        public DateTime sol_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>
        public int? sol_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>

        [Display(Name = "Usuario modificación")]
        public string? sol_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la mascota en la solicitud.
        /// </summary>

        [Display(Name = "Fecha modificación")]
        public DateTime? sol_FechaModifica { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.sol_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}