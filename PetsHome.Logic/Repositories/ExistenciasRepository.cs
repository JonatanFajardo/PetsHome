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
    public class ExistenciasRepository : IGenericRepository<tbExistencias>
    {
        public async Task<IEnumerable<dynamic>> ListAsync()
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Existencias_List]";
            return await DbApp.Select<dynamic>(sqlQuery);
        }

        public async Task<dynamic> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Existencias_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@exist_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<dynamic>(sqlQuery, parameter);
        }

        public async Task<dynamic> DetailAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Existencias_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@exist_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<dynamic>(sqlQuery, parameter);
        }

        /// <summary>
        /// Obtiene las existencias actuales por ítem y refugio
        /// </summary>
        public async Task<dynamic> GetByItemAndRefugioAsync(int itemId, int refugioId)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Existencias_GetByItemRefugio]";
            var parameter = new DynamicParameters();
            parameter.Add("@itm_Id", itemId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<dynamic>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbExistencias entity)
        {
            entity.exist_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Existencias_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_Stock", entity.exist_Stock, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_StockMinimo", entity.exist_StockMinimo, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_StockMaximo", entity.exist_StockMaximo, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_UsuarioCrea", entity.exist_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbExistencias entity)
        {
            entity.exist_UsuarioModifica = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Existencias_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@exist_Id", entity.exist_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_Stock", entity.exist_Stock, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_StockMinimo", entity.exist_StockMinimo, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_StockMaximo", entity.exist_StockMaximo, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_UsuarioModifica", entity.exist_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        /// <summary>
        /// Actualiza el stock de un ítem específico
        /// </summary>
        public async Task<Boolean> UpdateStockAsync(int itemId, int refugioId, int nuevoStock)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Existencias_UpdateStock]";
            var parameter = new DynamicParameters();
            parameter.Add("@itm_Id", itemId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@refg_Id", refugioId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_Stock", nuevoStock, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@exist_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Existencias_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@exist_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}