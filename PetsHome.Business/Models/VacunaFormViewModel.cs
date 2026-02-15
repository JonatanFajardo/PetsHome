using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para crear o editar vacunas.
    /// </summary>
    public class VacunaFormViewModel
    {
        [Key]
        [Display(Name = "Id vacuna")]
        public int vac_Id { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100)]
        public string vac_Descripcion { get; set; }

        [Display(Name = "Especie")]
        [StringLength(50)]
        public string vacu_Especie { get; set; }

        [Display(Name = "Dosis Recomendada")]
        [StringLength(100)]
        public string vacu_DosisRecomendada { get; set; }

        [Display(Name = "Período de Refuerzo")]
        [StringLength(50)]
        public string vacu_PeriodoRefuerzo { get; set; }

        [Display(Name = "Estado")]
        public bool vac_EsActivo { get; set; }

        public int vac_UsuarioCrea { get; set; }

        public string? vac_NombreUsuarioCrea { get; set; }

        public DateTime vac_FechaCrea { get; set; }

        public int? vac_UsuarioModifica { get; set; }

        public string? vac_NombreUsuarioModifica { get; set; }

        public DateTime? vac_FechaModifica { get; set; }

        public bool isEdit => vac_Id != 0;
    }
}
