using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio de solo-lectura para el motor de sugerencias clinicas
    /// ([Medico].[tbSugerenciaClinica]).
    /// </summary>
    public class SugerenciaClinicaRepository
    {
        /// <summary>
        /// Devuelve las reglas cuya palabra clave aparece en el texto del diagnostico.
        /// </summary>
        public async Task<IEnumerable<PR_Medico_Sugerencia_PorTextoResult>> PorTextoAsync(string texto)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Sugerencia_PorTexto]";
            var p = new DynamicParameters();
            p.Add("@texto", texto, DbType.String, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_Sugerencia_PorTextoResult>(sqlQuery, p);
        }
    }
}
