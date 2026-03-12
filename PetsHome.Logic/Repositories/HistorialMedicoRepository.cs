using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class HistorialMedicoRepository : IGenericRepository<tbHistorialMedico>
    {
        public async Task<IEnumerable<PR_Refugio_HistorialMedico_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_List]";
            return await DbApp.Select<PR_Refugio_HistorialMedico_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_HistorialMedico_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_HistorialMedico_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_HistorialMedico_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_HistorialMedico_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbHistorialMedico entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_Esterilizacion", entity.cita_Esterilizacion, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@cita_Comportamiento", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_SaludCuidado", entity.cita_SaludCuidado, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_InformacionAdicional", entity.cita_InformacionAdicional, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_UsuarioCrea", entity.cita_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbHistorialMedico entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_Esterilizacion", entity.cita_Esterilizacion, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@cita_Comportamiento", entity.cita_Id, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_SaludCuidado", entity.cita_SaludCuidado, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_InformacionAdicional", entity.cita_InformacionAdicional, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_UsuarioModifica", entity.cita_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_HistorialMedico_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }
    }
}