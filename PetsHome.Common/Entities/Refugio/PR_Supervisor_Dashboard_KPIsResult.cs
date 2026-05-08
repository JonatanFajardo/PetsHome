using System;

namespace PetsHome.Common.Entities
{
    public class PR_Supervisor_Dashboard_KPIsResult
    {
        public int kpi_MascotasActivas { get; set; }
        public int kpi_MascotasTendencia { get; set; }
        public int kpi_AdopcionesMes { get; set; }
        public int kpi_VacunasVencidas { get; set; }
        public int kpi_EventosProximos { get; set; }
        public string kpi_ProximoEventoNombre { get; set; }
        public DateTime? kpi_ProximoEventoFecha { get; set; }
    }
}
