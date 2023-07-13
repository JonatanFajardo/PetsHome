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
    public class EntradasDetalleRepository : IGenericRepository<tbEntradasDetalles>
    {
        public async Task<IEnumerable<PR_Inventario_EntradasDetalles_ListResult>> ListIdAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_EntradasDetalles_SelectByEntrada]";
            var parameter = new DynamicParameters();
            parameter.Add("@ent_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Inventario_EntradasDetalles_ListResult>(sqlQuery, parameter);
        }

        public async Task<PR_Inventario_EntradasDetalles_FindResult> FindAsync(int id)
        {
            const string sqlQuery = @"[Inventario].[PR_Inventario_EntradasDetalles_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@ent_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_Inventario_EntradasDetalles_FindResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbEntradasDetalles entity)
        {
            entity.entdet_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_EntradasDetalles_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@ent_Id", entity.ent_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@entdet_Cantidad", entity.entdet_Cantidad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@entdet_UsuarioCrea", entity.entdet_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbEntradasDetalles entity)
        {
            entity.entdet_UsuarioModifica = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_EntradasDetalles_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@entdet_Id", entity.entdet_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@ent_Id", entity.ent_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@entdet_Cantidad", entity.entdet_Cantidad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@entdet_UsuarioModifica", entity.entdet_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}