using Dapper;
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
        public async Task<IEnumerable<PR_Refugio_CitaMedica_ListResult>> ListAsyncs()
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_CitaMedica_List]";
            return await DbApp.Select<PR_Refugio_CitaMedica_ListResult>(sqlQuery);
        }

        public async Task<PR_Refugio_CitaMedica_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_CitaMedica_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@medic_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Refugio_CitaMedica_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_Refugio_CitaMedica_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_CitaMedica_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@medic_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Refugio_CitaMedica_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tbCitaMedica entity)
        {
            entity.medic_UsuarioCrea = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_CitaMedica_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@com_Id", entity.com_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_FechaConsulta", entity.medic_FechaConsulta, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@medic_TipoConsulta", entity.medic_TipoConsulta, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_MotivoConsulta", entity.medic_MotivoConsulta, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_Diagnostico", entity.medic_Diagnostico, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_Peso", entity.medic_Peso, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_Temperatura", entity.medic_Temperatura, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_FrecuenciaCardiaca", entity.medic_FrecuenciaCardiaca, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_FrecuenciaRespiratoria", entity.medic_FrecuenciaRespiratoria, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vac_Id", entity.vac_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_MedicamentosRecetados", entity.medic_MedicamentosRecetados, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_Dosificacion", entity.medic_Dosificacion, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_ProcedimientosRealizados", entity.medic_ProcedimientosRealizados, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_ResultadosExamenes", entity.medic_ResultadosExamenes, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_ProximaCita", entity.medic_ProximaCita, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@medic_MotivoProximaCita", entity.medic_MotivoProximaCita, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_UsuarioCrea", entity.medic_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tbCitaMedica entity)
        {
            entity.medic_UsuarioModifica = 1;
            const string sqlQuery = "[Refugio].[PR_Refugio_CitaMedica_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@medic_Id", entity.medic_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@com_Id", entity.com_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_FechaConsulta", entity.medic_FechaConsulta, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@medic_TipoConsulta", entity.medic_TipoConsulta, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_MotivoConsulta", entity.medic_MotivoConsulta, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_Diagnostico", entity.medic_Diagnostico, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_Peso", entity.medic_Peso, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_Temperatura", entity.medic_Temperatura, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_FrecuenciaCardiaca", entity.medic_FrecuenciaCardiaca, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_FrecuenciaRespiratoria", entity.medic_FrecuenciaRespiratoria, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@vac_Id", entity.vac_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@medic_MedicamentosRecetados", entity.medic_MedicamentosRecetados, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_Dosificacion", entity.medic_Dosificacion, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_ProcedimientosRealizados", entity.medic_ProcedimientosRealizados, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_ResultadosExamenes", entity.medic_ResultadosExamenes, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_ProximaCita", entity.medic_ProximaCita, DbType.DateTime, ParameterDirection.Input);
            parameter.Add("@medic_MotivoProximaCita", entity.medic_MotivoProximaCita, DbType.String, ParameterDirection.Input);
            parameter.Add("@medic_UsuarioModifica", entity.medic_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Refugio].[PR_Refugio_CitaMedica_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@medic_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
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