using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para mostrar el detalle de una vacuna.
    /// </summary>
    public class VacunaDetailsViewModel
    {
        [Key]
        [Display(Name = "Id vacuna")]
        public int vac_Id { get; set; }

        [Display(Name = "Descripción")]
        public string vac_Descripcion { get; set; }

        [Display(Name = "Especie")]
        public string vacu_Especie { get; set; }

        [Display(Name = "Dosis Recomendada")]
        public string vacu_DosisRecomendada { get; set; }

        [Display(Name = "Período de Refuerzo")]
        public string vacu_PeriodoRefuerzo { get; set; }

        public string EsActivo { get; set; }

        public int vac_UsuarioCrea { get; set; }

        public string vac_NombreUsuarioCrea { get; set; }

        public DateTime vac_FechaCrea { get; set; }

        public int? vac_UsuarioModifica { get; set; }

        public string vac_NombreUsuarioModifica { get; set; }

        public DateTime? vac_FechaModifica { get; set; }

        public bool vac_EsActivo => string.Equals(EsActivo, "Activo", StringComparison.OrdinalIgnoreCase);
    }
}
