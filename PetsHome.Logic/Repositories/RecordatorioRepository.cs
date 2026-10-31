using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio del modulo RECORDATORIOS / RECALLS (care-reminders de OpenVPM).
    /// Recordatorio = tarea clinica interna "a esta mascota le toca algo en tal
    /// fecha" (refuerzo de vacuna, seguimiento). No envia correo ni SMS.
    /// Auto-generacion idempotente desde vacunas / proxima cita.
    /// </summary>
    public class RecordatorioRepository
    {
        public async Task<IEnumerable<PR_Medico_Recordatorios_ListResult>> ListAsync(string estado, DateTime? desde, DateTime? hasta, int? mascId)
        {
            const string sql = "[Medico].[PR_Medico_Recordatorios_List]";
            var p = new DynamicParameters();
            p.Add("@estado", estado, DbType.String, ParameterDirection.Input, 20);
            p.Add("@desde", desde, DbType.Date, ParameterDirection.Input);
            p.Add("@hasta", hasta, DbType.Date, ParameterDirection.Input);
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_Recordatorios_ListResult>(sql, p);
        }

        public async Task<PR_Medico_Recordatorios_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_Recordatorios_Detail]";
            var p = new DynamicParameters();
            p.Add("@rec_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_Recordatorios_DetailResult>(sql, p);
        }

        /// <summary>Auto-genera recordatorios pendientes desde la data real. Devuelve cuantos creo.</summary>
        public async Task<int> GenerarAsync(int usuario)
        {
            const string sql = "[Medico].[PR_Medico_Recordatorios_Generar]";
            var p = new DynamicParameters();
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            var row = await DbApp.Select<GenerarRow>(sql, p);
            return (row != null && row.CodeStatus > 0) ? row.Generados : 0;
        }

        public async Task<RequestResult> MarcarHechoAsync(int recId, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_Recordatorios_MarcarHecho]";
            var p = new DynamicParameters();
            p.Add("@rec_Id", recId, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> DescartarAsync(int recId, int usuario, string nota)
        {
            const string sql = "[Medico].[PR_Medico_Recordatorios_Descartar]";
            var p = new DynamicParameters();
            p.Add("@rec_Id", recId, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            p.Add("@nota", nota, DbType.String, ParameterDirection.Input, 300);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> MarcarEnviadoAsync(int recId, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_Recordatorios_MarcarEnviado]";
            var p = new DynamicParameters();
            p.Add("@rec_Id", recId, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        private class GenerarRow
        {
            public int CodeStatus { get; set; }
            public string MessageStatus { get; set; }
            public int Generados { get; set; }
        }
    }
}
