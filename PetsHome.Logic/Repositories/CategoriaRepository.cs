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

        public async Task<Boolean> AddAsync(tbCategorias entity)
        {
            entity.cat_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Descripcion", entity.cat_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cat_UsuarioCrea", entity.cat_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }
        public async Task<Boolean> EditAsync(tbCategorias entity)
        {
            entity.cat_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Id", entity.cat_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cat_Descripcion", entity.cat_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cat_UsuarioCrea", entity.cat_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }


        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@cat_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

    }
}
