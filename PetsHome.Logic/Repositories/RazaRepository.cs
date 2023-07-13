using Dapper;
using Microsoft.Data.SqlClient;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class RazaRepository
    {
        public async Task<IEnumerable<PR_Refugio_Razas_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_List]";
            return await DbApp.Select<PR_Refugio_Razas_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Razas_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Razas_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Razas_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Razas_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbRazas entity)
        {
            entity.raza_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Descripcion", entity.raza_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_UsuarioCrea", entity.raza_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbRazas entity)
        {
            entity.raza_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Razas_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Id", entity.raza_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@raza_Descripcion", entity.raza_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@raza_UsuarioModifica", entity.raza_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public tbRazas Validation(string priod_Descripcion)
        {
            const string query = @"[Refugio].[UDP_Refugio_Razas_ValidacionUnique]";
            var parameters = new DynamicParameters();
            parameters.Add("@raza_Descripcion", priod_Descripcion, DbType.String, ParameterDirection.Input);
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.QueryFirstOrDefault<tbRazas>(query, parameters, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Razas_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@raza_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}