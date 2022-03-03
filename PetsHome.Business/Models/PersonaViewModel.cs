using Microsoft.AspNetCore.Mvc.Rendering;
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
        [MaxLength(13, ErrorMessage = "Ingrese solamente 13 digitos")]
        [MinLength(13, ErrorMessage = "El mínimo de dígitos es 13")]
        public string per_Identidad { get; set; }

        [Display(Name = "PrimerNombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_PrimerNombre { get; set; }

        [Display(Name = "Segundo Nombre")]
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_SegundoNombre { get; set; }

        [Display(Name = "ApellidoPaterno")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_ApellidoPaterno { get; set; }

        [Display(Name = "Apellido Materno")]
        //[Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string per_ApellidoMaterno { get; set; }

        [Display(Name = "Fecha nacimiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime per_FechaNacimiento { get; set; }

        [Display(Name = "Domicilio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string per_Domicilio { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8, ErrorMessage = "Ingrese solamente 8 digitos")]
        [MinLength(8, ErrorMessage = "El mínimo de dígitos es 8")]
        public string per_Telefono { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string per_Correo { get; set; }

        public int per_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? per_NombreUsuarioCrea { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime per_FechaCrea { get; set; }

        public int? per_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? per_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? per_FechaModifica { get; set; }

        // Propiedades Extras

        [Display(Name = "Usuario modificación")]
        public string? usuarioModifica { get; set; }
        [Display(Name = "Usuario creación")]
        public string? usuarioCrea { get; set; }

        public SelectList refugio { get; set; }

        //public string cag_Descripcion { get; set; }
        
        //public string per_Domicilio { get; set; }
        //public string per_Telefono { get; set; }
        //public string per_Correo { get; set; }

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

        //public class PersonasDetailViewModel
        //{

        //}
    }
}