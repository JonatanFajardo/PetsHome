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
    public class RecepcionesMercanciaRepository : IGenericRepository<tbRecepcionesMercancia>
    {
        public async Task<IEnumerable<SP_tbRecepcionesMercancia_List>> ListAsync()
        {
            const string sqlQuery = "[Inventario].[SP_tbRecepcionesMercancia_List]";
            return await DbApp.Select<SP_tbRecepcionesMercancia_List>(sqlQuery);
        }

        public async Task<dynamic> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesMercancia_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<dynamic>(sqlQuery, parameter);
        }

        public async Task<dynamic> DetailAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_RecepcionesMercancia_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<dynamic>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbRecepcionesMercancia entity)
        {
            entity.recep_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[SP_tbRecepcionesMercancia_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Descripcion", entity.recep_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_Fecha", entity.recep_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@recep_TipoRecepcion", entity.recep_TipoRecepcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_OrigenId", entity.recep_OrigenId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_NumeroDocumento", entity.recep_NumeroDocumento, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_UsuarioCrea", entity.recep_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbRecepcionesMercancia entity)
        {
            entity.recep_UsuarioModifica = 1;
            const string sqlQuery = "[Inventario].[SP_tbRecepcionesMercancia_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", entity.recep_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_Descripcion", entity.recep_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_Fecha", entity.recep_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@recep_TipoRecepcion", entity.recep_TipoRecepcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@recep_OrigenId", entity.recep_OrigenId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_NumeroDocumento", entity.recep_NumeroDocumento, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@recep_UsuarioModifica", entity.recep_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_RecepcionesMercancia_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@recep_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}