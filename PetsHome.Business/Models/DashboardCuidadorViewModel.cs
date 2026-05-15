using PetsHome.Common.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PetsHome.Business.Models
{
    public class DashboardCuidadorViewModel
    {
        public List<PR_Refugio_DashboardCuidador_MascotasActivasResult> MascotasActivas { get; set; }
            = new List<PR_Refugio_DashboardCuidador_MascotasActivasResult>();

        public List<PR_Medico_DashboardCuidador_CitasHoyResult> CitasHoy { get; set; }
            = new List<PR_Medico_DashboardCuidador_CitasHoyResult>();

        public List<PR_Medico_DashboardCuidador_AlertasActivasResult> AlertasActivas { get; set; }
            = new List<PR_Medico_DashboardCuidador_AlertasActivasResult>();

        public List<PR_Refugio_DashboardCuidador_SolicitudesPendientesResult> SolicitudesPendientes { get; set; }
            = new List<PR_Refugio_DashboardCuidador_SolicitudesPendientesResult>();

        public int TotalMascotas    => MascotasActivas?.Count ?? 0;
        public int TotalCitasHoy    => CitasHoy?.Count ?? 0;
        public int TotalAlertas     => AlertasActivas?.Count ?? 0;
        public int TotalSolicitudes => SolicitudesPendientes?.Count(s => s.sol_Estado == "Pendiente") ?? 0;
    }
}
