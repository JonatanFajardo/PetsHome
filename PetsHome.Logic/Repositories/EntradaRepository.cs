using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class EntradaRepository
    {
        public async Task<IEnumerable<PR_Inventario_Entradas_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Entradas_List]";
            return await DbApp.Select<PR_Inventario_Entradas_ListResult>(sqlQuery);
        }

        public async Task<PR_Inventario_Entradas_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Entradas_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@ent_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Inventario_Entradas_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Inventario_Entradas_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Inventario].[PR_Inventario_Entradas_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@ent_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Inventario_Entradas_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbEntradas entity)
        {
            entity.ent_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Entradas_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@ent_Descripcion", entity.ent_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@ent_Fecha", entity.ent_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@ent_UsuarioCrea", entity.ent_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbEntradas entity)
        {
            entity.ent_UsuarioCrea = 1;
            const string sqlQuery = "[Inventario].[PR_Inventario_Entradas_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@ent_Id", entity.ent_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@ent_Descripcion", entity.ent_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@ent_Fecha", entity.ent_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@ent_UsuarioModifica", entity.ent_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@ent_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
