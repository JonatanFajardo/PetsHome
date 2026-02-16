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
    public class GravedadRepository : IGenericRepository<tbGravedades>
    {
        public async Task<IEnumerable<PR_Medico_Gravedades_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_Gravedades_List]";
            return await DbApp.Select<PR_Medico_Gravedades_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_Gravedades_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Gravedades_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@grav_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Gravedades_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_Gravedades_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Gravedades_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@grav_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_Gravedades_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbGravedades entity)
        {
            entity.grav_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_Gravedades_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@grav_Descripcion", entity.grav_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@grav_EsActivo", entity.grav_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@grav_UsuarioCrea", entity.grav_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbGravedades entity)
        {
            entity.grav_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_Gravedades_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@grav_Id", entity.grav_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@grav_Descripcion", entity.grav_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@grav_EsActivo", entity.grav_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@grav_UsuarioModifica", entity.grav_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Gravedades_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@grav_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
