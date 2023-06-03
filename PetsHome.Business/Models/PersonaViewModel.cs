using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class PersonaViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la persona.
        /// </summary>
        [Key]
        public int per_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la identidad de la persona.
        /// </summary>
        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(13, ErrorMessage = "Ingrese solamente 13 dígitos")]
        [MinLength(13, ErrorMessage = "El mínimo de dígitos es 13")]
        public string per_Identidad { get; set; }

        /// <summary>
        /// Obtiene o establece el primer nombre de la persona.
        /// </summary>
        [Display(Name = "Primer Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_PrimerNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el segundo nombre de la persona.
        /// </summary>
        [Display(Name = "Segundo Nombre")]
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_SegundoNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el apellido paterno de la persona.
        /// </summary>
        [Display(Name = "Apellido Paterno")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_ApellidoPaterno { get; set; }

        /// <summary>
        /// Obtiene o establece el apellido materno de la persona.
        /// </summary>
        [Display(Name = "Apellido Materno")]
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_ApellidoMaterno { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la persona.
        /// </summary>
        [Display(Name = "Fecha nacimiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime per_FechaNacimiento { get; set; }

        /// <summary>
        /// Obtiene o establece el domicilio de la persona.
        /// </summary>
        [Display(Name = "Domicilio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string per_Domicilio { get; set; }

        /// <summary>
        /// Obtiene o establece el teléfono de la persona.
        /// </summary>
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8, ErrorMessage = "Ingrese solamente 8 dígitos")]
        [MinLength(8, ErrorMessage = "El mínimo de dígitos es 8")]
        public string per_Telefono { get; set; }

        /// <summary>
        /// Obtiene o establece el correo de la persona.
        /// </summary>
        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string per_Correo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que crea la persona.
        /// </summary>
        public int per_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que crea la persona.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? per_NombreUsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de creación de la persona.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime per_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica la persona.
        /// </summary>
        public int? per_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica la persona.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? per_NombreUsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la persona.
        /// </summary>
        [Display(Name = "Fecha modificación")]
        public DateTime? per_FechaModifica { get; set; }

        // Propiedades Extras

        /// <summary>
        /// Obtiene o establece el nombre de usuario que realiza la modificación.
        /// </summary>
        [Display(Name = "Usuario modificación")]
        public string? usuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de usuario que crea la persona.
        /// </summary>
        [Display(Name = "Usuario creación")]
        public string? usuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el SelectList del refugio.
        /// </summary>
        public SelectList refugio { get; set; }

        /// <summary>
        /// Indica si el formulario se está editando.
        /// </summary>
        public bool isEdit
        {
            get
            {
                if (this.per_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}