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
    public class ViaAdministracionRepository : IGenericRepository<tbViasAdministracion>
    {
        public async Task<IEnumerable<PR_Medico_ViasAdministracion_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_ViasAdministracion_List]";
            return await DbApp.Select<PR_Medico_ViasAdministracion_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_ViasAdministracion_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_ViasAdministracion_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@viaAdmin_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_ViasAdministracion_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_ViasAdministracion_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_ViasAdministracion_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@viaAdmin_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_ViasAdministracion_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbViasAdministracion entity)
        {
            entity.viaAdmin_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_ViasAdministracion_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@viaAdmin_Descripcion", entity.viaAdmin_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@viaAdmin_EsActivo", entity.viaAdmin_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@viaAdmin_UsuarioCrea", entity.viaAdmin_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbViasAdministracion entity)
        {
            entity.viaAdmin_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_ViasAdministracion_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@viaAdmin_Id", entity.viaAdmin_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@viaAdmin_Descripcion", entity.viaAdmin_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@viaAdmin_EsActivo", entity.viaAdmin_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@viaAdmin_UsuarioModifica", entity.viaAdmin_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_ViasAdministracion_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@viaAdmin_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        #region Dropdown

        public IEnumerable<PR_Medico_ViasAdministracion_ListResult> ViaAdministracionDropdown()
        {
            const string query = "[Medico].[PR_Medico_ViasAdministracion_List]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.Query<PR_Medico_ViasAdministracion_ListResult>(query, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        #endregion Dropdown
    }
}
