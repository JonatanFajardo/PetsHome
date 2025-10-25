using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo utilizado para mostrar el detalle de una mascota.
    /// </summary>
    public class MascotaDetailsViewModel
    {
        [Key]
        [Display(Name = "Id mascota")]
        public int masc_Id { get; set; }

        public byte[] masc_Imagen { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string masc_Nombre { get; set; }

        [Display(Name = "Raza")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string raza_Descripcion { get; set; }

        [Display(Name = "Edad")]
        public int? masc_Edad { get; set; }

        [Display(Name = "Sexo")]
        public string masc_Sexo { get; set; }

        [Display(Name = "Peso")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? masc_Peso { get; set; }

        [Display(Name = "Talla")]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? masc_Talla { get; set; }

        [Display(Name = "Color")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string masc_Color { get; set; }

        [Display(Name = "Historia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500)]
        public string masc_Historia { get; set; }

        [Display(Name = "Refugio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string refg_Nombre { get; set; }

        [Display(Name = "Procedencia")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        public string proc_Descripcion { get; set; }

        public bool? masc_EsAdoptado { get; set; }

        public bool? masc_EsReservado { get; set; }

        [Display(Name = "Usuario creación")]
        public string? NombreUsuarioCrea { get; set; }

        public DateTime masc_masc_FechaCrea { get; set; }

        public int? masc_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? masc_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? masc_FechaModifica { get; set; }
    }
}
