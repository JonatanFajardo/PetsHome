using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{
    public partial class ItemViewModel
    {
        [Key]
        public int itm_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string itm_Codigo { get; set; }

        [Display(Name = "Item")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string itm_Descripcion { get; set; }

        [Display(Name = "Id categoría")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? cat_Id { get; set; }

        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string cat_Descripcion { get; set; }

        [Display(Name = "Id refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? refg_Id { get; set; }

        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Nombre { get; set; }

        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? itm_Precio { get; set; }

        [Display(Name = "Entrada")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? itm_Entrada { get; set; }

        [Display(Name = "Salida")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? itm_Salida { get; set; }

        [Display(Name = "Stock")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int? itm_Stock { get; set; }

        public int itm_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? itm_NombreUsuarioCrea { get; set; }


        public DateTime itm_FechaCrea { get; set; }

        public int? itm_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? itm_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? itm_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.itm_Id == 0)
                    return false;
                else
                    return true;
            }
        }

        #region Dropdown

        #endregion Dropdown
    }
}