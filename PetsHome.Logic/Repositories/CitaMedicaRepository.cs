using Dapper;
using PetsHome.Common;
using Microsoft.Data.SqlClient;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class CitaMedicaRepository : IGenericRepository<tbCitaMedica>
    {
        public async Task<IEnumerable<PR_Medico_CitaMedica_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_CitaMedica_List]";
            return await DbApp.Select<PR_Medico_CitaMedica_ListResult>(sqlQuery);
        }

        public async Task<PR_Medico_CitaMedica_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_CitaMedica_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_CitaMedica_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Medico_CitaMedica_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_CitaMedica_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_CitaMedica_DetailResult>(sqlQuery, parameter);
        }

        public async Task<RequestResult> AddAsync(tbCitaMedica entity)
        {
            entity.cita_UsuarioCrea = 1;
            const string sqlQuery = "[Medico].[PR_Medico_CitaMedica_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_FechaConsulta", entity.cita_FechaConsulta, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@tipoCon_Id", entity.tipoCon_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@grav_Id", entity.grav_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_MotivoConsulta", entity.cita_MotivoConsulta, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_Diagnostico", entity.cita_Diagnostico, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_Peso", entity.cita_Peso, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@cita_Temperatura", entity.cita_Temperatura, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@cita_FrecuenciaCardiaca", entity.cita_FrecuenciaCardiaca, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_FrecuenciaRespiratoria", entity.cita_FrecuenciaRespiratoria, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@com_Id", entity.com_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vac_Id", entity.vac_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_ProcedimientosRealizados", entity.cita_ProcedimientosRealizados, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_ResultadosExamenes", entity.cita_ResultadosExamenes, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_ProximaCita", entity.cita_ProximaCita, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@cita_MotivoProximaCita", entity.cita_MotivoProximaCita, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_UsuarioCrea", entity.cita_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> EditAsync(tbCitaMedica entity)
        {
            entity.cita_UsuarioModifica = 1;
            const string sqlQuery = "[Medico].[PR_Medico_CitaMedica_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_FechaConsulta", entity.cita_FechaConsulta, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@tipoCon_Id", entity.tipoCon_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@grav_Id", entity.grav_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_MotivoConsulta", entity.cita_MotivoConsulta, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_Diagnostico", entity.cita_Diagnostico, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_Peso", entity.cita_Peso, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@cita_Temperatura", entity.cita_Temperatura, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@cita_FrecuenciaCardiaca", entity.cita_FrecuenciaCardiaca, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_FrecuenciaRespiratoria", entity.cita_FrecuenciaRespiratoria, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@com_Id", entity.com_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vac_Id", entity.vac_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_ProcedimientosRealizados", entity.cita_ProcedimientosRealizados, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_ResultadosExamenes", entity.cita_ResultadosExamenes, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_ProximaCita", entity.cita_ProximaCita, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@cita_MotivoProximaCita", entity.cita_MotivoProximaCita, DbType.String, ParameterDirection.Input);
            parameter.Add("@cita_UsuarioModifica", entity.cita_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public async Task<RequestResult> RemoveAsync(int id)
        {
            const string sqlQuery = "[Medico].[PR_Medico_CitaMedica_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@cita_Id", id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@cita_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, parameter);
        }

        public IEnumerable<PR_Medico_CitaMedica_DropdownResult> Dropdown(int? masc_Id = null)
        {
            const string query = "[Medico].[PR_Medico_CitaMedica_Dropdown]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var parameter = new DynamicParameters();
                parameter.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
                var result = db.Query<PR_Medico_CitaMedica_DropdownResult>(query, parameter, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<PR_Medico_CitaMedica_CalendarioResult>> CalendarioAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            const string sqlQuery = "[Medico].[PR_Medico_CitaMedica_Calendario]";
            var parameter = new DynamicParameters();
            parameter.Add("@FechaInicio", fechaInicio.Date, DbType.Date, ParameterDirection.Input);
            parameter.Add("@FechaFin", fechaFin.Date, DbType.Date, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_CitaMedica_CalendarioResult>(sqlQuery, parameter);
        }

        public async Task SeedCalendarioAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_CitaMedica_SeedCalendario]";
            using var db = new SqlConnection(PetsHomeDbContext.ConnectionString);
            await db.ExecuteAsync(sqlQuery, commandType: CommandType.StoredProcedure, commandTimeout: 60);
        }

        public IEnumerable<PR_Refugio_Comportamiento_ListResult> ComportamientoList()
        {
            const string query = "[Refugio].[PR_Refugio_Comportamiento_List]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.Query<PR_Refugio_Comportamiento_ListResult>(query, commandType: CommandType.StoredProcedure);
                return result;
            }
        }
    }
}