using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{
    public partial class EmpleadoCargoViewModel
    {
        [Key]
        [Display(Name = "Id")]
        public int cag_Id { get; set; }

        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string cag_Descripcion { get; set; }

        [Display(Name = "Salario")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? cag_Salario { get; set; }

        public string EsActivo { get; set; }

        public int cag_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? cag_NombreUsuarioCrea { get; set; }

        public DateTime cag_FechaCrea { get; set; }

        public int? cag_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? cag_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? cag_FechaModifica { get; set; }


    }
}