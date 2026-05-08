        using PetsHome.Common.Entities;
        using PetsHome.DataAccess.Extensions;
        using System.Collections.Generic;
        using System.Threading.Tasks;

        namespace PetsHome.Logic.Repositories
        {
            public class ControlVacunacionRepository
            {
        public async Task<IEnumerable<PR_Medico_ControlVacunacion_DashboardResult>> DashboardAsync()
{
    const string sql = "[Medico].[PR_Medico_ControlVacunacion_Dashboard]";
    return await DbApp.Select<PR_Medico_ControlVacunacion_DashboardResult>(sql);
}

public async Task<IEnumerable<PR_Medico_ControlVacunacion_MatrizVacunacionResult>> MatrizVacunacionAsync()
{
    const string sql = "[Medico].[PR_Medico_ControlVacunacion_MatrizVacunacion]";
    return await DbApp.Select<PR_Medico_ControlVacunacion_MatrizVacunacionResult>(sql);
}
            }
        }
