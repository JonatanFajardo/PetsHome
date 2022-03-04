using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class ItemRepository
    {
        public async Task<IEnumerable<PR_Inventario_Items_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Items_List]";
            return await DbApp.Select<PR_Inventario_Items_ListResult>(sqlQuery);
        }

        public async Task<PR_Inventario_Items_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Items_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@itm_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Inventario_Items_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Inventario_Items_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Items_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@itm_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Inventario_Items_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbItems entity)
        {
            entity.itm_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Items_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@itm_Codigo", entity.itm_Codigo, DbType.String, ParameterDirection.Input);
            parameter.Add("@itm_Descripcion", entity.itm_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cat_Id", entity.cat_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Precio", entity.itm_Precio, DbType.Double, ParameterDirection.Input);
            parameter.Add("@itm_UsuarioCrea", entity.itm_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbItems entity)
        {
            entity.itm_UsuarioModifica = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Items_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Codigo", entity.itm_Codigo, DbType.String, ParameterDirection.Input);
            parameter.Add("@itm_Descripcion", entity.itm_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cat_Id", entity.cat_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Precio", entity.itm_Precio, DbType.Double, ParameterDirection.Input);
            parameter.Add("@itm_UsuarioModifica", entity.itm_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Items_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@itm_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
        #region Dropdown
        public IEnumerable<PR_Inventario_Categorias_DropdownResult> CategoriaDropdown()
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Categorias_Dropdown]";
            return DbApp.Dropdown<PR_Inventario_Categorias_DropdownResult>(sqlQuery);
        }
        #endregion Dropdown
    }
}
