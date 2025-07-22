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
    public class RecepcionesDetallesRepository : IGenericRepository<tbRecepcionesDetalles>
    {
        public async Task<IEnumerable<tbRecepcionesDetalles>> ListByRecepcionAsync(int recepcionId)
        {
            const string sqlQuery = "[Inventario].[SP_tbRecepcionesDetalles_ByRecepcion]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", recepcionId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<tbRecepcionesDetalles>(sqlQuery, parameter);
        }

        public async Task<tbRecepcionesDetalles> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@recdet_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<tbRecepcionesDetalles>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbRecepcionesDetalles entity)
        {
            entity.recdet_UsuarioCrea = 1;
            entity.recdet_FechaCrea = DateTime.Now;
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", entity.recep_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recdet_Cantidad", entity.recdet_Cantidad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recdet_PrecioUnitario", entity.recdet_PrecioUnitario, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@recdet_FechaVencimiento", entity.recdet_FechaVencimiento, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@recdet_NumeroLote", entity.recdet_NumeroLote, DbType.String, ParameterDirection.Input);
            parameter.Add("@recdet_UsuarioCrea", entity.recdet_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbRecepcionesDetalles entity)
        {
            entity.recdet_UsuarioModifica = 1;
            entity.recdet_FechaModifica = DateTime.Now;
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@recdet_Id", entity.recdet_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_Id", entity.recep_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recdet_Cantidad", entity.recdet_Cantidad, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recdet_PrecioUnitario", entity.recdet_PrecioUnitario, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@recdet_FechaVencimiento", entity.recdet_FechaVencimiento, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@recdet_NumeroLote", entity.recdet_NumeroLote, DbType.String, ParameterDirection.Input);
            parameter.Add("@recdet_UsuarioModifica", entity.recdet_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesDetalles_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@recdet_Id", id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recdet_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        // Métodos requeridos por IGenericRepository pero no utilizados
        public Task<IEnumerable<tbRecepcionesDetalles>> ListAsync()
        {
            throw new NotImplementedException("Use ListByRecepcionAsync instead");
        }

        //Task<tbRecepcionesDetalles> IGenericRepository<tbRecepcionesDetalles>.FindAsync(int id)
        //{
        //    return FindAsync(id);
        //}

        //Task<Boolean> IGenericRepository<tbRecepcionesDetalles>.AddAsync(tbRecepcionesDetalles entity)
        //{
        //    return AddAsync(entity);
        //}

        //Task<Boolean> IGenericRepository<tbRecepcionesDetalles>.EditAsync(tbRecepcionesDetalles entity)
        //{
        //    return EditAsync(entity);
        //}

        //Task<Boolean> IGenericRepository<tbRecepcionesDetalles>.RemoveAsync(int id)
        //{
        //    return RemoveAsync(id);
        //}
    }
}