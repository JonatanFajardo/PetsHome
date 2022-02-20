using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class InventariosDetalleRepository
    {
        //public async Task<IEnumerable<PR_Inventario_InventariosDetalles_ListResult>> ListAsync()
        //{
        //    const string sqlQuery = "[Inventario].[PR_Inventario_InventarioDetalles_List]";
        //    return await DbApp.Select<PR_Inventario_InventariosDetalles_ListResult>(sqlQuery);
        //}

        //public async Task<PR_Inventario_InventariosDetalles_FindResult> FindAsync(int id)
        //{
        //    const string sqlQuery = "[Inventario].[PR_Inventario_InventarioDetalles_Find]";
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@invdet_Id", id, DbType.Int32, ParameterDirection.Input);
        //    return await DbApp.Find<PR_Inventario_InventariosDetalles_FindResult>(sqlQuery, parameter);
        //}

        //public async Task<PR_Inventario_InventariosDetalles_DetailResult> DetailAsync(int id)
        //{
        //    const string sqlQuery = "[Inventario].[PR_Inventario_InventarioDetalles_Detail]";
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@invdet_Id", id, DbType.Int32, ParameterDirection.Input);
        //    return await DbApp.Detail<PR_Inventario_InventariosDetalles_DetailResult>(sqlQuery, parameter);
        //}


        //public async Task<Boolean> AddAsync(tbInventariosDetalles entity)
        //{
        //    entity.invdet_UsuarioCrea = 1;
        //    const string sqlQuery = "[Inventario].[PR_Inventario_InventarioDetalles_Insert]";
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
        //    parameter.Add("@inv_Id", entity.inv_Id, DbType.Int32, ParameterDirection.Input);
        //    parameter.Add("@invdet_Cantidad", entity.invdet_Cantidad, DbType.Int32, ParameterDirection.Input);
        //    parameter.Add("@invdet_UsuarioCrea", entity.invdet_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
        //    return await DbApp.Insert(sqlQuery, parameter);
        //}

        //public async Task<Boolean> EditAsync(tbInventariosDetalles entity)
        //{
        //    entity.invdet_UsuarioModifica = 1;
        //    const string sqlQuery = "[Inventario].[PR_Inventario_InventarioDetalles_Update]";
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@invdet_Id", entity.invdet_Id, DbType.Int32, ParameterDirection.Input);
        //    parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
        //    parameter.Add("@inv_Id", entity.inv_Id, DbType.Int32, ParameterDirection.Input);
        //    parameter.Add("@invdet_Cantidad", entity.invdet_Cantidad, DbType.Int32, ParameterDirection.Input);
        //    parameter.Add("@invdet_UsuarioModifica", entity.invdet_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
        //    return await DbApp.Update(sqlQuery, parameter);
        //}

        //public async Task<Boolean> RemoveAsync(int id)
        //{
        //    const string sqlQuery = "[Inventario].[PR_Inventario_InventarioDetalles_Delete]";
        //    var parameter = new DynamicParameters();
        //    parameter.Add("@invdet_Id", id, DbType.Int32, ParameterDirection.Input);
        //    return await DbApp.Delete(sqlQuery, parameter);
        //}
    }
}
