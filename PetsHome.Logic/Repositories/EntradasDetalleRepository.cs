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
        public async Task<IEnumerable<PR_Inventario_EntradasDetalles_ListResult>> ListAsync()
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_List]";
            return await DbApp.Select<PR_Inventario_EntradasDetalles_ListResult>(sqlQuery);
        }

        public async Task<Boolean> AddAsync(tbEntradasDetalles entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Boolean> EditAsync(tbEntradasDetalles entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
