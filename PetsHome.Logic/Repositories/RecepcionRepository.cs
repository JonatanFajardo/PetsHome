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
    /// Repositorio del modulo Recepcion / Sala de espera.
    /// Feed, no gate: encola citas del dia ("En espera") y permite transicionar su estado.
    /// Reutiliza los SPs creados en los scripts 53-55.
    /// </summary>
    public class RecepcionRepository
    {
        /// <summary>
        /// CheckIn: encola una cita del dia en estado "En espera".
        /// Devuelve el cita_Id generado (0 si fallo).
        /// </summary>
        public async Task<PR_Veterinaria_Recepcion_CheckInResult> CheckInAsync(
            int mascId, int? tipoConId, string motivo, string prioridad,
            int? vetId, int recepcionista, decimal? peso)
        {
            const string sqlQuery = "[Veterinaria].[PR_Veterinaria_Recepcion_CheckIn]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            p.Add("@tipoCon_Id", tipoConId, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_MotivoConsulta", motivo, DbType.String, ParameterDirection.Input);
            p.Add("@cita_Prioridad", string.IsNullOrWhiteSpace(prioridad) ? "Normal" : prioridad, DbType.String, ParameterDirection.Input, 12);
            p.Add("@vet_Id", vetId, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Recepcionista", recepcionista, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Peso", peso, DbType.Decimal, ParameterDirection.Input);
            return await DbApp.Select<PR_Veterinaria_Recepcion_CheckInResult>(sqlQuery, p);
        }

        /// <summary>
        /// Cola de sala de espera del dia (citas "En espera" / "En atencion").
        /// </summary>
        public async Task<IEnumerable<PR_Medico_SalaEspera_ListResult>> SalaEsperaAsync(DateTime? fecha, int? vetId)
        {
            const string sqlQuery = "[Medico].[PR_Medico_SalaEspera_List]";
            var p = new DynamicParameters();
            p.Add("@fecha", fecha, DbType.Date, ParameterDirection.Input);
            p.Add("@vet_Id", vetId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_SalaEspera_ListResult>(sqlQuery, p);
        }

        /// <summary>
        /// Dropdown de veterinarios (usuarios rol 4 activos) para el check-in.
        /// </summary>
        public async Task<IEnumerable<PR_Seguridad_Veterinarios_DropdownResult>> VeterinariosAsync()
        {
            const string sqlQuery = "[Seguridad].[PR_Seguridad_Veterinarios_Dropdown]";
            return await DbApp.Select<PR_Seguridad_Veterinarios_DropdownResult>(sqlQuery);
        }

        /// <summary>
        /// Transiciona el estado de una visita (En espera / En atencion / Finalizada / Cancelada / No asistio).
        /// </summary>
        public async Task<RequestResult> CambiarEstadoAsync(int citaId, string estado, int usuario)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Visita_CambiarEstado]";
            var p = new DynamicParameters();
            p.Add("@cita_Id", citaId, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Estado", estado, DbType.String, ParameterDirection.Input, 20);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, p);
        }
    }
}
