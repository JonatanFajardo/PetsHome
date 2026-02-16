using Dapper;
using Microsoft.Data.SqlClient;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class TipoMedicamentoRepository : IGenericRepository<tbTiposMedicamento>
    {
        public async Task<IEnumerable<PR_Medico_TiposMedicamento_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposMedicamento_List]";
            return await DbApp.Select<PR_Medico_TiposMedicamento_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_TiposMedicamento_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposMedicamento_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoMed_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_TiposMedicamento_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_TiposMedicamento_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposMedicamento_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoMed_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_TiposMedicamento_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbTiposMedicamento entity)
        {
            entity.tipoMed_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_TiposMedicamento_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoMed_Descripcion", entity.tipoMed_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoMed_EsActivo", entity.tipoMed_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@tipoMed_UsuarioCrea", entity.tipoMed_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbTiposMedicamento entity)
        {
            entity.tipoMed_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_TiposMedicamento_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoMed_Id", entity.tipoMed_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@tipoMed_Descripcion", entity.tipoMed_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoMed_EsActivo", entity.tipoMed_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@tipoMed_UsuarioModifica", entity.tipoMed_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposMedicamento_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoMed_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        #region Dropdown

        public IEnumerable<PR_Medico_TiposMedicamento_ListResult> TipoMedicamentoDropdown()
        {
            const string query = "[Medico].[PR_Medico_TiposMedicamento_List]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.Query<PR_Medico_TiposMedicamento_ListResult>(query, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        #endregion Dropdown
    }
}
