        using PetsHome.Common.Entities;
        using PetsHome.DataAccess.Extensions;
        using System.Collections.Generic;
        using System.Threading.Tasks;

        namespace PetsHome.Logic.Repositories
        {
            public class ReporteAdopcionesRepository
            {
        public async Task<IEnumerable<PR_Refugio_ReporteAdopciones_ResumenResult>> ResumenAsync()
{
    const string sql = "[Refugio].[PR_Refugio_ReporteAdopciones_Resumen]";
    return await DbApp.Select<PR_Refugio_ReporteAdopciones_ResumenResult>(sql);
}

public async Task<IEnumerable<PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult>> AdopcionesPorMesAsync()
{
    const string sql = "[Refugio].[PR_Refugio_ReporteAdopciones_AdopcionesPorMes]";
    return await DbApp.Select<PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult>(sql);
}

public async Task<IEnumerable<PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult>> EstadoSolicitudesAsync()
{
    const string sql = "[Refugio].[PR_Refugio_ReporteAdopciones_EstadoSolicitudes]";
    return await DbApp.Select<PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult>(sql);
}

public async Task<IEnumerable<PR_Refugio_ReporteAdopciones_TopRazasResult>> TopRazasAsync()
{
    const string sql = "[Refugio].[PR_Refugio_ReporteAdopciones_TopRazas]";
    return await DbApp.Select<PR_Refugio_ReporteAdopciones_TopRazasResult>(sql);
}

public async Task<IEnumerable<PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult>> AdopcionesRecientesAsync()
{
    const string sql = "[Refugio].[PR_Refugio_ReporteAdopciones_AdopcionesRecientes]";
    return await DbApp.Select<PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult>(sql);
}
            }
        }
