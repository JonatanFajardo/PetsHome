using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class TipoConsultaRepository : IGenericRepository<tbTiposConsulta>
    {
        public async Task<IEnumerable<PR_Medico_TiposConsulta_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposConsulta_List]";
            return await DbApp.Select<PR_Medico_TiposConsulta_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_TiposConsulta_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposConsulta_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoCon_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_TiposConsulta_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_TiposConsulta_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposConsulta_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoCon_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_TiposConsulta_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbTiposConsulta entity)
        {
            entity.tipoCon_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_TiposConsulta_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoCon_Descripcion", entity.tipoCon_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoCon_EsActivo", entity.tipoCon_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@tipoCon_UsuarioCrea", entity.tipoCon_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbTiposConsulta entity)
        {
            entity.tipoCon_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_TiposConsulta_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoCon_Id", entity.tipoCon_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@tipoCon_Descripcion", entity.tipoCon_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@tipoCon_EsActivo", entity.tipoCon_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@tipoCon_UsuarioModifica", entity.tipoCon_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_TiposConsulta_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@tipoCon_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
