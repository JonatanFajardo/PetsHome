using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public partial class MunicipioViewModel
    {
        [Key]
        [Display(Name = "Id")]
        public int mpio_Id { get; set; }

        [Display(Name = "Código de municipio")]
        public string mpio_Codigo { get; set; }

        [Display(Name = "Municipio")]
        [Required(ErrorMessage = "El campo municipio es requerido")]
        [StringLength(100, ErrorMessage = "El municipio debe tener un máximo de {1} caracteres")]
        public string mpio_Descripcion { get; set; }

        [Display(Name = "Id dep")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int depto_Id { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo código es requerido")]
        //[StringLength(2, MinimumLength = 2, ErrorMessage = "El código debe tener {1} caracteres")]
        [RegularExpression(@"^\d*$", ErrorMessage = "Solo se permiten números")]
        public string depto_Codigo { get; set; }

        //public string codigo { get; set; }
        [Display(Name = "Usuario creación")]
        public int? mpio_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string UsuarioCreacion { get; set; }

        [Display(Name = "Fecha creación")]
        public DateTime? mpio_FechaCrea { get; set; }

        [Display(Name = "Usuario modificación")]
        public int? mpio_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string UsuarioModificacion { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? mpio_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.mpio_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}