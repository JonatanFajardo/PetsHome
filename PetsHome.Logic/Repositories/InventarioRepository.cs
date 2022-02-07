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
    public class InventarioRepository : IGenericRepository<tbInventarios>
    {
        public async Task<IEnumerable<PR_Inventario_Inventarios_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Inventarios_List]";
            return await DbApp.Select<PR_Inventario_Inventarios_ListResult>(sqlQuery);
        }

        public async Task<PR_Inventario_Inventarios_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Inventarios_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@inv_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Inventario_Inventarios_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Inventario_Inventarios_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Inventarios_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@inv_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Inventario_Inventarios_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbInventarios entity)
        {
            entity.inv_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Inventarios_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@inv_Fecha", entity.inv_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@inv_UsuarioCrea", entity.inv_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbInventarios entity)
        {
            entity.inv_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Inventarios_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@inv_Id", entity.inv_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@inv_Fecha", entity.inv_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@inv_UsuarioCrea", entity.inv_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@inv_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
