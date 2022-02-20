using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{

    public partial class PersonaViewModel
    {
        [Key]
        public int per_Id { get; set; }

        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(13)]
        public string per_Identidad { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_Apellidos { get; set; }

        [Display(Name = "Fecha nacimiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "date")]
        public DateTime? per_FechaNacimiento { get; set; }

        [Display(Name = "Domicilio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string per_Domicilio { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8)]
        public string per_Telefono { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string per_Correo { get; set; }

        public int per_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? per_NombreUsuarioCrea { get; set; }
        public DateTime per_FechaCrea { get; set; }
        public int? per_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? per_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? per_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
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