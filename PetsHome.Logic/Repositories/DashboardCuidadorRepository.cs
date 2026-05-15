using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class DashboardCuidadorRepository
    {
        public async Task<IEnumerable<PR_Refugio_DashboardCuidador_MascotasActivasResult>> MascotasActivasAsync()
        {
            const string sql = "[Refugio].[PR_Refugio_DashboardCuidador_MascotasActivas]";
            return await DbApp.Select<PR_Refugio_DashboardCuidador_MascotasActivasResult>(sql);
        }

        public async Task<IEnumerable<PR_Medico_DashboardCuidador_CitasHoyResult>> CitasHoyAsync()
        {
            const string sql = "[Medico].[PR_Medico_DashboardCuidador_CitasHoy]";
            return await DbApp.Select<PR_Medico_DashboardCuidador_CitasHoyResult>(sql);
        }

        public async Task<IEnumerable<PR_Medico_DashboardCuidador_AlertasActivasResult>> AlertasActivasAsync()
        {
            const string sql = "[Medico].[PR_Medico_DashboardCuidador_AlertasActivas]";
            return await DbApp.Select<PR_Medico_DashboardCuidador_AlertasActivasResult>(sql);
        }

        public async Task<IEnumerable<PR_Refugio_DashboardCuidador_SolicitudesPendientesResult>> SolicitudesPendientesAsync()
        {
            const string sql = "[Refugio].[PR_Refugio_DashboardCuidador_SolicitudesPendientes]";
            return await DbApp.Select<PR_Refugio_DashboardCuidador_SolicitudesPendientesResult>(sql);
        }
    }
}
