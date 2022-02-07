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
    public class SolicitudeRepository : IGenericRepository<tbSolicitudes>
    {
        public async Task<IEnumerable<PR_Refugio_Solicitudes_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Solicitudes_List]";
            return await DbApp.Select<PR_Refugio_Solicitudes_ListResult>(sqlQuery);
        }
        public async Task<PR_Refugio_Solicitudes_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Solicitudes_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@sol_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_Solicitudes_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_Solicitudes_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_Solicitudes_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@sol_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_Solicitudes_DetailResult>(sqlQuery, parameter);
        }


        public async Task<Boolean> AddAsync(tbSolicitudes entity)
        {
            entity.sol_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Solicitudes_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@sol_Identidad", entity.sol_Identidad, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Nombres", entity.sol_Nombres, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Apellidos", entity.sol_Apellidos, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Telefono", entity.sol_Telefono, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Correo", entity.sol_Correo, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Fecha", entity.sol_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sol_UsuarioCrea", entity.sol_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }
        public async Task<Boolean> EditAsync(tbSolicitudes entity)
        {
            entity.sol_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_Solicitudes_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@sol_Id", entity.sol_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sol_Identidad", entity.sol_Identidad, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Nombres", entity.sol_Nombres, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Apellidos", entity.sol_Apellidos, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Telefono", entity.sol_Telefono, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Correo", entity.sol_Correo, DbType.String, ParameterDirection.Input);
            parameter.Add("@sol_Fecha", entity.sol_Fecha, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@sol_UsuarioCrea", entity.sol_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_Departamentos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@sol_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
