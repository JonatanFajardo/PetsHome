using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class AlertaMedicaRepository
    {
        public async Task<IEnumerable<PR_Medico_AlertaMedica_VacunasResult>> VacunasVencidasAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_AlertaMedica_VacunasVencidas]";
            return await DbApp.Select<PR_Medico_AlertaMedica_VacunasResult>(sqlQuery);
        }

        public async Task<IEnumerable<PR_Medico_AlertaMedica_TratamientosResult>> TratamientosPorVencerAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_AlertaMedica_TratamientosPorVencer]";
            return await DbApp.Select<PR_Medico_AlertaMedica_TratamientosResult>(sqlQuery);
        }

        public async Task<IEnumerable<PR_Medico_AlertaMedica_RecetasResult>> RecetasSinRevisionAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_AlertaMedica_RecetasSinRevision]";
            return await DbApp.Select<PR_Medico_AlertaMedica_RecetasResult>(sqlQuery);
        }

        public async Task<IEnumerable<PR_Medico_AlertaMedica_SinConsultaResult>> SinConsultaAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_AlertaMedica_SinConsulta]";
            return await DbApp.Select<PR_Medico_AlertaMedica_SinConsultaResult>(sqlQuery);
        }
    }
}
