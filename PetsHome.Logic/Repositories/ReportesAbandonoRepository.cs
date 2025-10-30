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
    public class ReportesAbandonoRepository : IGenericRepository<tbReportesAbandono>
    {
        public async Task<IEnumerable<PR_Rescate_ReportesAbandono_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportesAbandono_List]";
            return await DbApp.Select<PR_Rescate_ReportesAbandono_ListResult>(sqlQuery);
        }

        public async Task<PR_Rescate_ReportesAbandono_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportesAbandono_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@repa_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Rescate_ReportesAbandono_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Rescate_ReportesAbandono_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportesAbandono_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@repa_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Rescate_ReportesAbandono_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbReportesAbandono entity)
        {
            entity.repa_UsuarioCrea = 1;
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportesAbandono_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@reptip_Id", entity.reptip_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@repa_NombreReportante", entity.repa_NombreReportante, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_TelefonoContacto", entity.repa_TelefonoContacto, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_Email", entity.repa_Email, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_FechaReporte", entity.repa_FechaReporte, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@repa_UbicacionIncidente", entity.repa_UbicacionIncidente, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_DescripcionAnimal", entity.repa_DescripcionAnimal, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_EstadoAtencion", entity.repa_EstadoAtencion, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_Observaciones", entity.repa_Observaciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_EsAnonimo", entity.repa_EsAnonimo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@repa_UsuarioCrea", entity.repa_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbReportesAbandono entity)
        {
            entity.repa_UsuarioModifica = 1;
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportesAbandono_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@repa_Id", entity.repa_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@reptip_Id", entity.reptip_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@repa_NombreReportante", entity.repa_NombreReportante, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_TelefonoContacto", entity.repa_TelefonoContacto, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_Email", entity.repa_Email, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_FechaReporte", entity.repa_FechaReporte, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@repa_UbicacionIncidente", entity.repa_UbicacionIncidente, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_DescripcionAnimal", entity.repa_DescripcionAnimal, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_EstadoAtencion", entity.repa_EstadoAtencion, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_Observaciones", entity.repa_Observaciones, DbType.String, ParameterDirection.Input);
            parameter.Add("@repa_EsAnonimo", entity.repa_EsAnonimo, DbType.Boolean, ParameterDirection.Input);
            parameter.Add("@refg_Id", entity.refg_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@repa_UsuarioModifica", entity.repa_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Rescate].[PR_Rescate_ReportesAbandono_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@repa_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }
    }
}
