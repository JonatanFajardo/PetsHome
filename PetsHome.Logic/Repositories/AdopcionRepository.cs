using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class AdopcionRepository : IGenericRepository<tbAdopciones>
    {
        public async Task<IEnumerable<PR_Refugio_Adopciones_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Adopciones_List]";
            return await DbApp.Select<PR_Refugio_Adopciones_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_Adopciones_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Adopciones_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Adopciones_FindResult>(sqlQuery, parameter);
        }

        public async Task<IEnumerable<PR_Refugio_Adopciones_DetailResult>> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Adopciones_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Refugio_Adopciones_DetailResult>(sqlQuery, parameter);
        }

        // Devuelve todas las solicitudes (solicitantes) para una mascota específica
        public async Task<PR_Refugio_Adopciones_DetailResult> DetailByMascotaAsync(int masc_Id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Adopciones_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Adopciones_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbAdopciones entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Adopciones_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@sol_Id", entity.sol_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@adop_Estado", entity.adop_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@adop_UsuarioCrea", entity.adop_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbAdopciones entity)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Adopcion_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@adop_Id", entity.adop_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sol_Id", entity.sol_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@adop_Estado", entity.adop_Estado, DbType.String, ParameterDirection.Input);
            parameter.Add("@adop_UsuarioModifica", entity.adop_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Adopciones_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@adop_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        // Solicitudes por mascota desde el SP de adopciones detail (para tabs en Adopción)
        public async Task<PR_Refugio_Adopciones_DetailResult> ListByMascotaFromAdopcionesDetailAsync(int masc_Id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Adopciones_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_Refugio_Adopciones_DetailResult>(sqlQuery, parameter);
        }
    }
}
