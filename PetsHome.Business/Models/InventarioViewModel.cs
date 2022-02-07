using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{
    public partial class InventarioViewModel
    {
        [Key]
        [Display(Name = "Id Inventario")]
        public int inv_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? inv_Fecha { get; set; }

        public int? inv_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? inv_NombreUsuarioCrea { get; set; }

        public DateTime inv_FechaCrea { get; set; }

        public int? inv_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? inv_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? inv_FechaModifica { get; set; }
    }
}