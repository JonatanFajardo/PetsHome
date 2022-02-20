using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{
    public partial class SalidaViewModel
    {
        [Key]
        [Display(Name = "Id salida")]
        public int sal_Id { get; set; }

        [Display(Name = "Id item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? item_Id { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string itm_Descripcion { get; set; }

        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? sal_Fecha { get; set; }

        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? sal_Cantidad { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool? esEliminado { get; set; }

        [Display(Name = "Fecha creación")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime fechaCreacion { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? fechaModificacion { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.sal_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        #endregion Dropdown
    }
}