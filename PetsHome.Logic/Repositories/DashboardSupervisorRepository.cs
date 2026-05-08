using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class DashboardSupervisorRepository
    {
        public async Task<PR_Supervisor_Dashboard_PillsResult> PillsAsync()
        {
            var result = await DbApp.Select<PR_Supervisor_Dashboard_PillsResult>("[Refugio].[PR_Supervisor_Dashboard_Pills]");
            return result?.FirstOrDefault() ?? new PR_Supervisor_Dashboard_PillsResult();
        }

        public async Task<PR_Supervisor_Dashboard_KPIsResult> KPIsAsync()
        {
            var result = await DbApp.Select<PR_Supervisor_Dashboard_KPIsResult>("[Refugio].[PR_Supervisor_Dashboard_KPIs]");
            return result?.FirstOrDefault() ?? new PR_Supervisor_Dashboard_KPIsResult();
        }

        public async Task<IEnumerable<PR_Supervisor_Dashboard_SolicitudesResult>> SolicitudesAsync()
            => await DbApp.Select<PR_Supervisor_Dashboard_SolicitudesResult>("[Refugio].[PR_Supervisor_Dashboard_Solicitudes]");

        public async Task<PR_Supervisor_Dashboard_EstadoMascotasResult> EstadoMascotasAsync()
        {
            var result = await DbApp.Select<PR_Supervisor_Dashboard_EstadoMascotasResult>("[Refugio].[PR_Supervisor_Dashboard_EstadoMascotas]");
            return result?.FirstOrDefault() ?? new PR_Supervisor_Dashboard_EstadoMascotasResult();
        }

        public async Task<IEnumerable<PR_Supervisor_Dashboard_EventosResult>> EventosAsync()
            => await DbApp.Select<PR_Supervisor_Dashboard_EventosResult>("[Refugio].[PR_Supervisor_Dashboard_Eventos]");

        public async Task<IEnumerable<PR_Supervisor_Dashboard_MovimientosInventarioResult>> MovimientosAsync()
            => await DbApp.Select<PR_Supervisor_Dashboard_MovimientosInventarioResult>("[Inventario].[PR_Supervisor_Dashboard_MovimientosInventario]");
    }
}
