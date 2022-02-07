using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PetsHome.Business.Models
{
    public partial class Refugio
    {

        [Key]
        public int refg_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Nombre { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Ubicacion { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(14)]
        public string refg_RTN { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(8)]
        public string refg_Telefono { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150)]
        public string refg_Correo { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500)]
        public string refg_InformacionAdicional { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool? esEliminado { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? fechaCreacion { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Column(TypeName = "datetime")]
        public DateTime? fechaModificacion { get; set; }


    }
}