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
    public class ProcedenciaRepository : IGenericRepository<tbProcedencias>
    {
        public async Task<IEnumerable<PR_Refugio_Procedencias_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Procedencias_List]";
            return await DbApp.Select<PR_Refugio_Procedencias_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Procedencias_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Procedencias_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@proc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Procedencias_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Procedencias_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Procedencias_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@proc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Procedencias_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbProcedencias entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Procedencias_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@proc_Descripcion", entity.proc_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@proc_EsActivo", entity.proc_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@proc_UsuarioCrea", entity.proc_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbProcedencias entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Procedencias_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@proc_Id", entity.proc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@proc_Descripcion", entity.proc_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@proc_EsActivo", entity.proc_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@proc_UsuarioModifica", entity.proc_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Procedencias_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@proc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public virtual async Task<bool> DescripcionExistsAsync(string descripcion, int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Procedencias_Existe]";
            var parameter = new DynamicParameters();
            parameter.Add("@proc_Descripcion", descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@proc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<bool>(sqlQuery, parameter);
        }

        #region Dropdown

        /// <summary>
        /// Obtiene una lista de procedencias para dropdowns.
        /// </summary>
        /// <returns>Lista de procedencias para dropdowns.</returns>
        public IEnumerable<PR_Refugio_Procedencia_DropdownResult> ProcedenciaDropdown()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Procedencias_Dropdown]";
            return DbApp.Dropdown<PR_Refugio_Procedencia_DropdownResult>(sqlQuery);
        }

        #endregion Dropdown
    }
}