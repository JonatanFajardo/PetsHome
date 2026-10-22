using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio de alergias de mascota ([Medico].[tbMascotaAlergia]).
    /// Operaciones: List por mascota / Insert / Corregir (soft-delete).
    /// </summary>
    public class MascotaAlergiaRepository
    {
        public async Task<IEnumerable<PR_Medico_MascotaAlergia_ListResult>> ListAsync(int masc_Id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_MascotaAlergia_List]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_MascotaAlergia_ListResult>(sqlQuery, p);
        }

        public async Task<RequestResult> AddAsync(tbMascotaAlergia entity)
        {
            const string sqlQuery = "[Medico].[PR_Medico_MascotaAlergia_Insert]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@alrg_Sustancia", entity.alrg_Sustancia, DbType.String, ParameterDirection.Input);
            p.Add("@alrg_Reaccion", entity.alrg_Reaccion, DbType.String, ParameterDirection.Input);
            p.Add("@alrg_Gravedad", entity.alrg_Gravedad, DbType.String, ParameterDirection.Input);
            p.Add("@alrg_UsuarioCrea", entity.alrg_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, p);
        }

        public async Task<RequestResult> CorregirAsync(int alrg_Id, int usuarioModifica)
        {
            const string sqlQuery = "[Medico].[PR_Medico_MascotaAlergia_Corregir]";
            var p = new DynamicParameters();
            p.Add("@alrg_Id", alrg_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@alrg_UsuarioModifica", usuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, p);
        }
    }
}
