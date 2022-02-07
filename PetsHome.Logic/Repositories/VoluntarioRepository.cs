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
    public class VoluntarioRepository : IGenericRepository<tbVoluntarios>
    {
        public async Task<IEnumerable<PR_Refugio_Voluntarios_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Voluntarios_List]";
            return await DbApp.Select<PR_Refugio_Voluntarios_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Voluntarios_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Voluntarios_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@vol_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Voluntarios_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Voluntarios_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Voluntarios_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@vol_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Voluntarios_DetailResult>(sqlQuery, parameter);
        }


        public async Task<Boolean> AddAsync(tbVoluntarios entity)
        {
            entity.vol_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Voluntarios_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@vol_HorasTrabajadas", entity.vol_HorasTrabajadas, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@per_Id", entity.per_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vol_Recurrente", entity.vol_Recurrente, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@vol_UsuarioCrea", entity.vol_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbVoluntarios entity)
        {
            entity.vol_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Voluntarios_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@vol_Id", entity.vol_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vol_HorasTrabajadas", entity.vol_HorasTrabajadas, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@per_Id", entity.per_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vol_Recurrente", entity.vol_Recurrente, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@vol_UsuarioCrea", entity.vol_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@vol_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
