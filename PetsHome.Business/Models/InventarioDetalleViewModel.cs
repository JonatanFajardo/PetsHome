using System;
using System.ComponentModel.DataAnnotations;
namespace PetsHome.Business.Models
{
    public partial class InventarioDetalleViewModel
    {
        [Key]
        [Display(Name = "Id inventario detalle")]
        public int invDet_Id { get; set; }

        [Display(Name = "Id inventario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? inv_Id { get; set; }

        [Display(Name = "Id Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? itm_Id { get; set; }

        [Display(Name = "Existencia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? invDet_Existencia { get; set; }

        [Display(Name = "Entradas")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? invDet_Entradas { get; set; }

        [Display(Name = "Salidas")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? invDet_Salidas { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? invDet_Stock { get; set; }

        public int invdet_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? invdet_NombreUsuarioCrea { get; set; }

        public DateTime invdet_FechaCrea { get; set; }

        public int? invdet_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? invdet_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]

        public DateTime? invdet_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.invDet_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        #endregion Dropdown

    }
}