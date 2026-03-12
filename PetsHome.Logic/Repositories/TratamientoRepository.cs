using Dapper;
using PetsHome.Common;
using Microsoft.Data.SqlClient;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class TratamientoRepository : IGenericRepository<tbTratamientos>
    {
        public async Task<IEnumerable<PR_Medico_Tratamientos_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_Tratamientos_List]";
            return await DbApp.Select<PR_Medico_Tratamientos_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_Tratamientos_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Tratamientos_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@trat_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Tratamientos_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_Tratamientos_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Tratamientos_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@trat_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_Tratamientos_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbTratamientos entity)
        {
            entity.trat_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_Tratamientos_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_Id", entity.receta_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@tipoPar_Id", entity.tipoPar_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@trat_ParasitoDetectado", entity.trat_ParasitoDetectado, DbType.String, ParameterDirection.Input);
            parameter.Add("@trat_Medicamento", entity.trat_Medicamento, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoMed_Id", entity.tipoMed_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@viaAdmin_Id", entity.viaAdmin_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@trat_FechaAplicacion", entity.trat_FechaAplicacion, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@trat_AplicadoPor", entity.trat_AplicadoPor, DbType.String, ParameterDirection.Input);
            parameter.Add("@trat_ProximaDosis", entity.trat_ProximaDosis, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@trat_Estado", entity.trat_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@trat_Observaciones", entity.trat_Observaciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@trat_UsuarioCrea", entity.trat_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbTratamientos entity)
        {
            entity.trat_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_Tratamientos_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@trat_Id", entity.trat_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@receta_Id", entity.receta_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@tipoPar_Id", entity.tipoPar_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@trat_ParasitoDetectado", entity.trat_ParasitoDetectado, DbType.String, ParameterDirection.Input);
            parameter.Add("@trat_Medicamento", entity.trat_Medicamento, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoMed_Id", entity.tipoMed_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@viaAdmin_Id", entity.viaAdmin_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@trat_FechaAplicacion", entity.trat_FechaAplicacion, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@trat_AplicadoPor", entity.trat_AplicadoPor, DbType.String, ParameterDirection.Input);
            parameter.Add("@trat_ProximaDosis", entity.trat_ProximaDosis, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@trat_Estado", entity.trat_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@trat_Observaciones", entity.trat_Observaciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@trat_UsuarioModifica", entity.trat_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Tratamientos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@trat_Id", id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@trat_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public IEnumerable<PR_Medico_Tratamientos_DropdownResult> Dropdown(int? masc_Id = null)
        {
            const string query = "[Medico].[PR_Medico_Tratamientos_Dropdown]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
                var result = db.Query<PR_Medico_Tratamientos_DropdownResult>(query, parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
