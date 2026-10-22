using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model de un padecimiento / crónico de mascota (listado y alta).
    /// Estado almacenado sin tilde ('Cronico'); usar EstadoDisplay para la UI.
    /// </summary>
    public class MascotaPadecimientoViewModel
    {
        [Key]
        public int pade_Id { get; set; }

        [Display(Name = "Mascota")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int masc_Id { get; set; }

        [Display(Name = "Padecimiento")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(500)]
        public string pade_Nombre { get; set; }

        [Display(Name = "Estado")]
        public string pade_Estado { get; set; } = "Vigente";   // Vigente | Superado | Cronico

        [Display(Name = "Desde")]
        public DateTime? pade_Desde { get; set; }

        [Display(Name = "Hasta")]
        public DateTime? pade_Hasta { get; set; }

        public DateTime pade_FechaCrea { get; set; }

        /// <summary>Etiqueta con tilde para mostrar en pantalla.</summary>
        public string EstadoDisplay => pade_Estado == "Cronico" ? "Crónico" : pade_Estado;
    }
}
