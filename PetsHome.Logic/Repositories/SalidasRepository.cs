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
    public class SalidasRepository : IGenericRepository<tbSalidas>
    {
        public async Task<IEnumerable<PR_tbSalidas_List>> ListAsync()
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Salidas_List]";
            return await DbApp.Select<PR_tbSalidas_List>(sqlQuery);
        }

        public async Task<dynamic> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Salidas_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@sal_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<dynamic>(sqlQuery, parameter);
        }

        public async Task<dynamic> DetailAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Salidas_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@sal_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<dynamic>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbSalidas entity)
        {
            entity.sal_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Salidas_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@sal_Descripcion", entity.sal_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@sal_Fecha", entity.sal_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@sal_TipoSalida", entity.sal_TipoSalida, DbType.String, ParameterDirection.Input);
            parameter.Add("@sal_DestinoId", entity.sal_DestinoId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sal_NumeroDocumento", entity.sal_NumeroDocumento, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sal_UsuarioCrea", entity.sal_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbSalidas entity)
        {
            entity.sal_UsuarioModifica = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Salidas_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@sal_Id", entity.sal_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sal_Descripcion", entity.sal_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@sal_Fecha", entity.sal_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@sal_TipoSalida", entity.sal_TipoSalida, DbType.String, ParameterDirection.Input);
            parameter.Add("@sal_DestinoId", entity.sal_DestinoId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sal_NumeroDocumento", entity.sal_NumeroDocumento, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sal_UsuarioModifica", entity.sal_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Salidas_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@sal_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}