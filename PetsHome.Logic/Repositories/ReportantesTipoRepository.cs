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
    public class ReportantesTipoRepository : IGenericRepository<tbReportantesTipo>
    {
        public async Task<IEnumerable<PR_Rescate_ReportantesTipo_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportantesTipo_List]";
            return await DbApp.Select<PR_Rescate_ReportantesTipo_ListResult>(sqlQuery);
        }

        public async Task<PR_Rescate_ReportantesTipo_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportantesTipo_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@reptip_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Rescate_ReportantesTipo_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Rescate_ReportantesTipo_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportantesTipo_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@reptip_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Rescate_ReportantesTipo_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbReportantesTipo entity)
        {
            entity.reptip_UsuarioCrea = 1;
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportantesTipo_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@reptip_Descripcion", entity.reptip_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@reptip_EsActivo", entity.reptip_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@reptip_UsuarioCrea", entity.reptip_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbReportantesTipo entity)
        {
            entity.reptip_UsuarioModifica = 1;
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportantesTipo_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@reptip_Id", entity.reptip_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@reptip_Descripcion", entity.reptip_Descripcion, DbType.String, ParameterDirection.Input);
            parameter.Add("@reptip_EsActivo", entity.reptip_EsActivo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@reptip_UsuarioModifica", entity.reptip_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportantesTipo_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@reptip_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        public async Task<IEnumerable<tbReportantesTipo>> DropdownAsync()
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportantesTipo_List]";
            return await DbApp.Select<tbReportantesTipo>(sqlQuery);
        }
    }
}
