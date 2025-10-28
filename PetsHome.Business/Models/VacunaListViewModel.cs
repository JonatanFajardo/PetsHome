namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para mostrar vacunas en listados.
    /// </summary>
    public class VacunaListViewModel
    {
        public int vac_Id { get; set; }

        public string vac_Descripcion { get; set; }

        public string vacu_Especie { get; set; }

        public string vacu_DosisRecomendada { get; set; }

        public string vacu_PeriodoRefuerzo { get; set; }

        public string EsActivo { get; set; }

        public bool vac_EsActivo => string.Equals(EsActivo, "Activo", System.StringComparison.OrdinalIgnoreCase);
    }
}
