using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para crear o editar voluntarios.
    /// </summary>
    public class VoluntarioFormViewModel
    {
        [Key]
        [Display(Name = "Id voluntario")]
        public int vol_Id { get; set; }

        [Display(Name = "Horas Trabajadas")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, 10000, ErrorMessage = "Las {0} deben estar entre {1} y {2}")]
        public int? vol_HorasTrabajadas { get; set; }

        [Display(Name = "Id")]
        public int? per_Id { get; set; }

        [Display(Name = "Recurrente")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public bool vol_Recurrente { get; set; }

        public string estado { get; set; }

        public string per_Apellidos { get; set; }

        public string per_Identidad { get; set; }

        public PersonaViewModel per { get; set; } = new PersonaViewModel();

        public int vol_UsuarioCrea { get; set; }

        public string? vol_NombreUsuarioCrea { get; set; }

        public DateTime vol_FechaCrea { get; set; }

        public int? vol_UsuarioModifica { get; set; }

        public string? vol_NombreUsuarioModifica { get; set; }

        public DateTime? vol_FechaModifica { get; set; }

        public string vol_Nombres { get; set; }

        public bool isEdit => vol_Id != 0;
    }
}
