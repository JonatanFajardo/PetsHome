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
    public class CategoriaRepository : IGenericRepository<tbCategorias>
    {
        public async Task<IEnumerable<PR_Inventario_Categorias_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_List]";
            return await DbApp.Select<PR_Inventario_Categorias_ListResult>(sqlQuery);
        }

        public async Task<PR_Inventario_Categorias_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Inventario_Categorias_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Inventario_Categorias_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Inventario_Categorias_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbCategorias entity)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Descripcion", entity.cat_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cat_EsActivo", entity.cat_EsActivo ?? true, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@cat_UsuarioCrea", entity.cat_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbCategorias entity)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Id", entity.cat_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cat_Descripcion", entity.cat_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cat_EsActivo", entity.cat_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@cat_UsuarioModifica", entity.cat_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public virtual async Task<bool> DescripcionExistsAsync(string descripcion, int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Existe]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Descripcion", descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cat_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<bool>(sqlQuery, parameter);
        }
    }
}