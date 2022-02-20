using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{
    public partial class AdopcionViewModel
    {
        [Key]
        [Display(Name = "Id Ficha")]
        public int adop_Id { get; set; }

        [Display(Name = "Id solicitud")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int sol_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "date")]
        public DateTime adop_FechaRegistro { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool adop_EsAprobado { get; set; }

        public int adop_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? adop_NombreUsuarioCrea { get; set; }
        public DateTime adop_FechaCrea { get; set; }
        public int? adop_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? adop_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? adop_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
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

        #region Dropdown

        #endregion Dropdown
    }
}