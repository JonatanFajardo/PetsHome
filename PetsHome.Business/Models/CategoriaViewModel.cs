using System;
using System.ComponentModel.DataAnnotations;
namespace PetsHome.Business.Models
{
    public partial class CategoriaViewModel
    {
        [Key]
        [Display(Name = "Id")]
        public int cat_Id { get; set; }

        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string cat_Descripcion { get; set; }

        public int cat_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? NombreUsuarioCrea { get; set; }

        [Display(Name = "")]
        public DateTime cat_FechaCrea { get; set; }

        public int? cat_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? cat_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? cat_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.cat_Id == 0)
                    return false;
                else
                    return true;
            }
        }

    }
}