using PetsHome.Common.Entities;
using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    public class DashboardSupervisorViewModel
    {
        public PR_Supervisor_Dashboard_PillsResult Pills { get; set; }
            = new PR_Supervisor_Dashboard_PillsResult();
        public PR_Supervisor_Dashboard_KPIsResult KPIs { get; set; }
            = new PR_Supervisor_Dashboard_KPIsResult();
        public List<PR_Supervisor_Dashboard_SolicitudesResult> Solicitudes { get; set; }
            = new List<PR_Supervisor_Dashboard_SolicitudesResult>();
        public PR_Supervisor_Dashboard_EstadoMascotasResult EstadoMascotas { get; set; }
            = new PR_Supervisor_Dashboard_EstadoMascotasResult();
        public List<PR_Supervisor_Dashboard_EventosResult> Eventos { get; set; }
            = new List<PR_Supervisor_Dashboard_EventosResult>();
        public List<PR_Supervisor_Dashboard_MovimientosInventarioResult> Movimientos { get; set; }
            = new List<PR_Supervisor_Dashboard_MovimientosInventarioResult>();
    }
}
