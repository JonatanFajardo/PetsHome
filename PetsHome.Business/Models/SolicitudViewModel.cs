using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{
    public partial class SolicitudViewModel
    {
        [Key]
        [Display(Name = "Id solicitud")]
        public int sol_Id { get; set; }

        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(13)]
        public string sol_Identidad { get; set; }

        [Display(Name = "Nombres")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string sol_Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string sol_Apellidos { get; set; }

        [Display(Name = "Telefono")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8)]
        public string sol_Telefono { get; set; }

        [Display(Name = "Correo")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string sol_Correo { get; set; }

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime sol_Fecha { get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int masc_Id { get; set; }

        // Propiedades de mascota
        public byte[] masc_Imagen { get; set; }

        [Display(Name = "Mascota")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string masc_Nombre { get; set; }
        public bool masc_EsAdoptado { get; set; }
        public string raza_Descripcion { get; set; }
        public string refg_Nombre { get; set; }

        public int sol_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? sol_NombreUsuarioCrea { get; set; }
        public DateTime sol_FechaCrea { get; set; }
        public int? sol_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? sol_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? sol_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.sol_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        #endregion Dropdown
    }
}