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
    public class SalidasDetallesRepository : IGenericRepository<tbSalidasDetalles>
    {
        public async Task<IEnumerable<tbSalidasDetalles>> ListBySalidaAsync(int salidaId)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_SalidasDetalles_BySalida]";
            var parameter = new DynamicParameters();
            parameter.Add("@sal_Id", salidaId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<tbSalidasDetalles>(sqlQuery, parameter);
        }

        public async Task<tbSalidasDetalles> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_SalidasDetalles_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@saldet_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<tbSalidasDetalles>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbSalidasDetalles entity)
        {
            entity.saldet_UsuarioCrea = 1;
            entity.saldet_FechaCrea = DateTime.Now;
            const string sqlQuery = "[Inventario].[PR_Inventario_SalidasDetalles_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@sal_Id", entity.sal_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@saldet_Cantidad", entity.saldet_Cantidad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@saldet_PrecioUnitario", entity.saldet_PrecioUnitario, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@saldet_Motivo", entity.saldet_Motivo, DbType.String, ParameterDirection.Input);
            parameter.Add("@saldet_UsuarioCrea", entity.saldet_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbSalidasDetalles entity)
        {
            entity.saldet_UsuarioModifica = 1;
            entity.saldet_FechaModifica = DateTime.Now;
            const string sqlQuery = "[Inventario].[PR_Inventario_SalidasDetalles_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@saldet_Id", entity.saldet_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sal_Id", entity.sal_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@saldet_Cantidad", entity.saldet_Cantidad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@saldet_PrecioUnitario", entity.saldet_PrecioUnitario, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@saldet_Motivo", entity.saldet_Motivo, DbType.String, ParameterDirection.Input);
            parameter.Add("@saldet_UsuarioModifica", entity.saldet_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_SalidasDetalles_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@saldet_Id", id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@saldet_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        // Métodos requeridos por IGenericRepository pero no utilizados
        public Task<IEnumerable<tbSalidasDetalles>> ListAsync()
        {
            throw new NotImplementedException("Use ListBySalidaAsync instead");
        }
    }
}