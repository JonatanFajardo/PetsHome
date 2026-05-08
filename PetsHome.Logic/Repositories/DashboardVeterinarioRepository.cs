        using PetsHome.Common.Entities;
        using PetsHome.DataAccess.Extensions;
        using System.Collections.Generic;
        using System.Threading.Tasks;

        namespace PetsHome.Logic.Repositories
        {
            public class DashboardVeterinarioRepository
            {
        public async Task<IEnumerable<PR_Medico_DashboardVeterinario_AgendaHoyResult>> AgendaHoyAsync()
{
    const string sql = "[Medico].[PR_Medico_DashboardVeterinario_AgendaHoy]";
    return await DbApp.Select<PR_Medico_DashboardVeterinario_AgendaHoyResult>(sql);
}

public async Task<IEnumerable<PR_Medico_DashboardVeterinario_TratamientosActivosResult>> TratamientosActivosAsync()
{
    const string sql = "[Medico].[PR_Medico_DashboardVeterinario_TratamientosActivos]";
    return await DbApp.Select<PR_Medico_DashboardVeterinario_TratamientosActivosResult>(sql);
}

public async Task<IEnumerable<PR_Medico_DashboardVeterinario_AlertasVeterinarioResult>> AlertasVeterinarioAsync()
{
    const string sql = "[Medico].[PR_Medico_DashboardVeterinario_AlertasVeterinario]";
    return await DbApp.Select<PR_Medico_DashboardVeterinario_AlertasVeterinarioResult>(sql);
}

public async Task<IEnumerable<PR_Medico_DashboardVeterinario_ResumenMesResult>> ResumenMesAsync()
{
    const string sql = "[Medico].[PR_Medico_DashboardVeterinario_ResumenMes]";
    return await DbApp.Select<PR_Medico_DashboardVeterinario_ResumenMesResult>(sql);
}
            }
        }
