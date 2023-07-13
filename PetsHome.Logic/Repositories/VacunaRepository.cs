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
    public class VacunaRepository : IGenericRepository<tbVacunas>
    {
        public async Task<IEnumerable<PR_Refugio_Vacunas_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Vacunas_List]";
            return await DbApp.Select<PR_Refugio_Vacunas_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Vacunas_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Vacunas_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@vac_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Vacunas_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Vacunas_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Vacunas_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@vac_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Vacunas_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbVacunas entity)
        {
            entity.vac_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Vacunas_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@vac_Descripcion", entity.vac_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@vac_UsuarioCrea", entity.vac_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbVacunas entity)
        {
            entity.vac_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Vacunas_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@vac_Id", entity.vac_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vac_Descripcion", entity.vac_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@vac_UsuarioModifica", entity.vac_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Vacunas_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@vac_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}