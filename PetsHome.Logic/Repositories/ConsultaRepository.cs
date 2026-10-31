using Dapper;
using PetsHome.Common;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio de apoyo para la pantalla "En consulta".
    /// La cita medica se persiste reutilizando CitaMedicaRepository; aqui
    /// solo viven operaciones que ese flujo no cubre (peso vivo, ultimo id).
    /// </summary>
    public class ConsultaRepository
    {
        /// <summary>
        /// Actualiza el peso vivo de la mascota (tbMascotas.masc_Peso).
        /// </summary>
        public async Task<RequestResult> ActualizarPesoAsync(int mascId, decimal peso, int usuarioModifica)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Consulta_ActualizarPeso]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            p.Add("@masc_Peso", peso, DbType.Decimal, ParameterDirection.Input);
            p.Add("@masc_UsuarioModifica", usuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, p);
        }

        /// <summary>
        /// Devuelve el id de la ultima cita registrada de la mascota.
        /// </summary>
        public async Task<int> UltimaCitaIdAsync(int mascId)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Consulta_UltimaCitaId]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            var result = await DbApp.SelectById<PR_Medico_Consulta_UltimaCitaIdResult>(sqlQuery, p);
            return result?.FirstOrDefault()?.cita_Id ?? 0;
        }

        /// <summary>
        /// Historial de nombres de medicamento ya recetados/aplicados (texto libre).
        /// </summary>
        public async Task<IEnumerable<PR_Medico_Consulta_MedicamentosUsadosResult>> MedicamentosUsadosAsync()
        {
            const string sqlQuery = "[Medico].[PR_Medico_Consulta_MedicamentosUsados]";
            return await DbApp.Select<PR_Medico_Consulta_MedicamentosUsadosResult>(sqlQuery);
        }

        // ── Firma de la nota clinica + addenda (correcciones append-only) ──

        /// <summary>
        /// Firma/bloquea la nota clinica de una cita (si no lo estaba ya).
        /// </summary>
        public async Task<RequestResult> FirmarNotaAsync(int citaId, int usuario)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Consulta_FirmarNota]";
            var p = new DynamicParameters();
            p.Add("@cita_Id", citaId, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, p);
        }

        /// <summary>
        /// Agrega una addenda (correccion posterior). Permitida incluso tras firmar.
        /// </summary>
        public async Task<RequestResult> AgregarAddendaAsync(int citaId, string texto, int usuario)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Consulta_AgregarAddenda]";
            var p = new DynamicParameters();
            p.Add("@cita_Id", citaId, DbType.Int32, ParameterDirection.Input);
            p.Add("@texto", texto, DbType.String, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sqlQuery, p);
        }

        /// <summary>
        /// Lista las addenda de una cita (mas antiguas primero, orden de lectura).
        /// </summary>
        public async Task<IEnumerable<PR_Medico_Consulta_AddendaResult>> ListAddendaAsync(int citaId)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Consulta_ListAddenda]";
            var p = new DynamicParameters();
            p.Add("@cita_Id", citaId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_Consulta_AddendaResult>(sqlQuery, p);
        }

        /// <summary>
        /// Estado de firma (bloqueo) de la nota clinica de una cita.
        /// </summary>
        public async Task<PR_Medico_Consulta_FirmaEstadoResult> FirmaEstadoAsync(int citaId)
        {
            const string sqlQuery = "[Medico].[PR_Medico_Consulta_FirmaEstado]";
            var p = new DynamicParameters();
            p.Add("@cita_Id", citaId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Consulta_FirmaEstadoResult>(sqlQuery, p);
        }
    }
}
