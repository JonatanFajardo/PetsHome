using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Procedimientos clinicos estructurados ([Medico].[tbProcedimientos]).
    /// Adaptado de OpenVPM "procedures". Aditivo: convive con el texto libre
    /// Medico.tbCitaMedica.cita_ProcedimientosRealizados.
    /// </summary>
    public class ProcedimientoRepository
    {
        /// <summary>Historial de procedimientos de una mascota (ficha del paciente).</summary>
        public async Task<IEnumerable<PR_Medico_Procedimiento_ListResult>> ListPorMascotaAsync(int mascId)
        {
            const string sql = "[Medico].[PR_Medico_Procedimiento_List]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_Procedimiento_ListResult>(sql, p);
        }

        /// <summary>Procedimientos de una visita (cita), para el panel de la consulta.</summary>
        public async Task<IEnumerable<PR_Medico_Procedimiento_ListResult>> ListPorCitaAsync(int citaId)
        {
            const string sql = "[Medico].[PR_Medico_Procedimiento_ListPorCita]";
            var p = new DynamicParameters();
            p.Add("@cita_Id", citaId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_Procedimiento_ListResult>(sql, p);
        }

        public async Task<PR_Medico_Procedimiento_FindResult> FindAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_Procedimiento_Find]";
            var p = new DynamicParameters();
            p.Add("@proc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Procedimiento_FindResult>(sql, p);
        }

        public async Task<PR_Medico_Procedimiento_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_Procedimiento_Detail]";
            var p = new DynamicParameters();
            p.Add("@proc_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_Procedimiento_DetailResult>(sql, p);
        }

        /// <summary>Inserta un procedimiento y devuelve el proc_Id generado (0 si falla/invalido).</summary>
        public async Task<(int procId, string mensaje)> AddReturningIdAsync(tbProcedimientos entity)
        {
            const string sql = "[Medico].[PR_Medico_Procedimiento_Insert]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@proc_Nombre", entity.proc_Nombre, DbType.String, ParameterDirection.Input, 255);
            p.Add("@proc_Descripcion", entity.proc_Descripcion, DbType.String, ParameterDirection.Input);
            p.Add("@proc_RealizadoPor", entity.proc_RealizadoPor, DbType.Int32, ParameterDirection.Input);
            p.Add("@proc_Anestesia", entity.proc_Anestesia, DbType.String, ParameterDirection.Input, 2000);
            p.Add("@proc_DuracionMinutos", entity.proc_DuracionMinutos, DbType.Int32, ParameterDirection.Input);
            p.Add("@proc_Notas", entity.proc_Notas, DbType.String, ParameterDirection.Input);
            p.Add("@serv_Id", entity.serv_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@proc_UsuarioCrea", entity.proc_UsuarioCrea, DbType.Int32, ParameterDirection.Input);

            // El SP retorna { CodeStatus, MessageStatus, proc_Id }
            var row = await DbApp.Select<ProcedimientoInsertRow>(sql, p);
            if (row != null && row.CodeStatus > 0)
                return (row.proc_Id, row.MessageStatus);
            return (0, row?.MessageStatus ?? "No se pudo registrar el procedimiento.");
        }

        public async Task<RequestResult> EditAsync(tbProcedimientos entity)
        {
            const string sql = "[Medico].[PR_Medico_Procedimiento_Update]";
            var p = new DynamicParameters();
            p.Add("@proc_Id", entity.proc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@proc_Nombre", entity.proc_Nombre, DbType.String, ParameterDirection.Input, 255);
            p.Add("@proc_Descripcion", entity.proc_Descripcion, DbType.String, ParameterDirection.Input);
            p.Add("@proc_RealizadoPor", entity.proc_RealizadoPor, DbType.Int32, ParameterDirection.Input);
            p.Add("@proc_Anestesia", entity.proc_Anestesia, DbType.String, ParameterDirection.Input, 2000);
            p.Add("@proc_DuracionMinutos", entity.proc_DuracionMinutos, DbType.Int32, ParameterDirection.Input);
            p.Add("@proc_Notas", entity.proc_Notas, DbType.String, ParameterDirection.Input);
            p.Add("@serv_Id", entity.serv_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@proc_UsuarioModifica", entity.proc_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> RemoveAsync(int id, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_Procedimiento_Delete]";
            var p = new DynamicParameters();
            p.Add("@proc_Id", id, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        /// <summary>Fila cruda del SELECT de retorno del Insert (CodeStatus/MessageStatus/proc_Id).</summary>
        private class ProcedimientoInsertRow
        {
            public int CodeStatus { get; set; }
            public string MessageStatus { get; set; }
            public int proc_Id { get; set; }
        }
    }
}
