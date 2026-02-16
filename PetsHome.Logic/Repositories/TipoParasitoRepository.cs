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
    public class TipoParasitoRepository : IGenericRepository<tbTiposParasito>
    {
        public async Task<IEnumerable<PR_Medico_TiposParasito_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposParasito_List]";
            return await DbApp.Select<PR_Medico_TiposParasito_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_TiposParasito_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposParasito_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoPar_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_TiposParasito_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_TiposParasito_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposParasito_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoPar_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_TiposParasito_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbTiposParasito entity)
        {
            entity.tipoPar_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_TiposParasito_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoPar_Descripcion", entity.tipoPar_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoPar_Categoria", entity.tipoPar_Categoria, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoPar_EsActivo", entity.tipoPar_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@tipoPar_UsuarioCrea", entity.tipoPar_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbTiposParasito entity)
        {
            entity.tipoPar_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_TiposParasito_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoPar_Id", entity.tipoPar_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@tipoPar_Descripcion", entity.tipoPar_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoPar_Categoria", entity.tipoPar_Categoria, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoPar_EsActivo", entity.tipoPar_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@tipoPar_UsuarioModifica", entity.tipoPar_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposParasito_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoPar_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        public IEnumerable<PR_Medico_TiposParasito_DropdownResult> TipoParasitoDropdown()
        {
            const string query = "[Medico].[PR_Medico_TiposParasito_Dropdown]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.Query<PR_Medico_TiposParasito_DropdownResult>(query, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}
