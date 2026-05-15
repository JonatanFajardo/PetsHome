using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class DashboardAdminRepository
    {
        public async Task<PR_General_DashboardAdmin_KPIsResult> KPIsAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_KPIs]";
            var results = await DbApp.Select<PR_General_DashboardAdmin_KPIsResult>(sql);
            return results?.FirstOrDefault() ?? new PR_General_DashboardAdmin_KPIsResult();
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_TendenciasResult>> TendenciasAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_Tendencias]";
            return await DbApp.Select<PR_General_DashboardAdmin_TendenciasResult>(sql);
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_MascotasEstadoResult>> MascotasEstadoAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_MascotasEstado]";
            return await DbApp.Select<PR_General_DashboardAdmin_MascotasEstadoResult>(sql);
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_TopRazasResult>> TopRazasAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_TopRazas]";
            return await DbApp.Select<PR_General_DashboardAdmin_TopRazasResult>(sql);
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_CitasHoyResult>> CitasHoyAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_CitasHoy]";
            return await DbApp.Select<PR_General_DashboardAdmin_CitasHoyResult>(sql);
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_SolicitudesPendientesResult>> SolicitudesPendientesAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_SolicitudesPendientes]";
            return await DbApp.Select<PR_General_DashboardAdmin_SolicitudesPendientesResult>(sql);
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_UsuariosPorRolResult>> UsuariosPorRolAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_UsuariosPorRol]";
            return await DbApp.Select<PR_General_DashboardAdmin_UsuariosPorRolResult>(sql);
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_HeatmapCitasResult>> HeatmapCitasAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_HeatmapCitas]";
            return await DbApp.Select<PR_General_DashboardAdmin_HeatmapCitasResult>(sql);
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_InventarioAlertaResult>> InventarioAlertasAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_InventarioAlertas]";
            return await DbApp.Select<PR_General_DashboardAdmin_InventarioAlertaResult>(sql);
        }

        public async Task<IEnumerable<PR_General_DashboardAdmin_EmbudoResult>> EmbudoAdopcionAsync()
        {
            const string sql = "[General].[PR_General_DashboardAdmin_EmbudoAdopcion]";
            return await DbApp.Select<PR_General_DashboardAdmin_EmbudoResult>(sql);
        }
    }
}
