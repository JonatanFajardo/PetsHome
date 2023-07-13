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
    public class EmpleadosCargoRepository : IGenericRepository<tbEmpleadosCargos>
    {
        public async Task<IEnumerable<PR_Refugio_EmpleadosCargos_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_EmpleadosCargos_Lis]";
            return await DbApp.Select<PR_Refugio_EmpleadosCargos_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_EmpleadosCargos_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_EmpleadosCargos_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@cag_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_EmpleadosCargos_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_EmpleadosCargos_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_EmpleadosCargos_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@cag_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_EmpleadosCargos_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbEmpleadosCargos entity)
        {
            entity.cag_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_EmpleadosCargos_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@cag_Descripcion", entity.cag_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cag_Salario", entity.cag_Salario, DbType.Double, ParameterDirection.Input);
            parameter.Add("@cag_UsuarioCrea", entity.cag_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbEmpleadosCargos entity)
        {
            entity.cag_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_EmpleadosCargos_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@cag_Id", entity.cag_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cag_Descripcion", entity.cag_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@cag_Salario", entity.cag_Salario, DbType.Double, ParameterDirection.Input);
            parameter.Add("@cag_UsuarioModifica", entity.cag_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[General].[PR_General_EmpleadosCargos_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@cag_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}