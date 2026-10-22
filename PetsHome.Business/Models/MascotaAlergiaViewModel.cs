using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model de una alergia de mascota (listado y alta en el alert bar clínico).
    /// </summary>
    public class MascotaAlergiaViewModel
    {
        [Key]
        public int alrg_Id { get; set; }

        [Display(Name = "Mascota")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int masc_Id { get; set; }

        [Display(Name = "Sustancia / alérgeno")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(255)]
        public string alrg_Sustancia { get; set; }

        [Display(Name = "Reacción")]
        [StringLength(500)]
        public string alrg_Reaccion { get; set; }

        [Display(Name = "Gravedad")]
        public string alrg_Gravedad { get; set; } = "Media";   // Leve | Media | Alta

        public DateTime alrg_FechaCrea { get; set; }
    }
}
