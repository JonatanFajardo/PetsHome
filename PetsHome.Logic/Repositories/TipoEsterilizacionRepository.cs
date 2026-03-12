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
    public class TipoEsterilizacionRepository : IGenericRepository<tbTiposEsterilizacion>
    {
        public async Task<IEnumerable<PR_Medico_TiposEsterilizacion_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposEsterilizacion_List]";
            return await DbApp.Select<PR_Medico_TiposEsterilizacion_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_TiposEsterilizacion_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposEsterilizacion_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoEst_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_TiposEsterilizacion_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_TiposEsterilizacion_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposEsterilizacion_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoEst_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_TiposEsterilizacion_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbTiposEsterilizacion entity)
        {
            entity.tipoEst_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_TiposEsterilizacion_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoEst_Descripcion", entity.tipoEst_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoEst_Sexo", entity.tipoEst_Sexo, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoEst_EsActivo", entity.tipoEst_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@tipoEst_UsuarioCrea", entity.tipoEst_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbTiposEsterilizacion entity)
        {
            entity.tipoEst_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_TiposEsterilizacion_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoEst_Id", entity.tipoEst_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@tipoEst_Descripcion", entity.tipoEst_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoEst_Sexo", entity.tipoEst_Sexo, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoEst_EsActivo", entity.tipoEst_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@tipoEst_UsuarioModifica", entity.tipoEst_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposEsterilizacion_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoEst_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }
    }
}
