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
    /// Repositorio del modulo Lista de espera POR DEMANDA (waitlist):
    /// pacientes SIN cupo todavia que piden "aviseme si se libera un espacio".
    /// Distinto de la sala de espera del dia (tbCitaMedica 'En espera').
    /// Alimenta/sugiere (feed, no gate); nunca bloquea el agendado normal.
    /// </summary>
    public class ListaEsperaRepository
    {
        public async Task<IEnumerable<PR_Medico_ListaEspera_ListResult>> ListAsync(int? mascId, int? tipoConId, string estado)
        {
            const string sql = "[Medico].[PR_Medico_ListaEspera_List]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            p.Add("@tipoCon_Id", tipoConId, DbType.Int32, ParameterDirection.Input);
            p.Add("@estado", estado, DbType.String, ParameterDirection.Input, 20);
            return await DbApp.SelectById<PR_Medico_ListaEspera_ListResult>(sql, p);
        }

        public async Task<IEnumerable<PR_Medico_ListaEspera_SugerenciaResult>> SugerenciasAsync(System.DateTime? fecha, int? tipoConId)
        {
            const string sql = "[Medico].[PR_Medico_ListaEspera_SugerenciasParaHueco]";
            var p = new DynamicParameters();
            p.Add("@fecha", fecha, DbType.Date, ParameterDirection.Input);
            p.Add("@tipoCon_Id", tipoConId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_ListaEspera_SugerenciaResult>(sql, p);
        }

        public async Task<PR_Medico_ListaEspera_FindResult> FindAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_ListaEspera_Find]";
            var p = new DynamicParameters();
            p.Add("@lesp_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_ListaEspera_FindResult>(sql, p);
        }

        public async Task<PR_Medico_ListaEspera_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_ListaEspera_Detail]";
            var p = new DynamicParameters();
            p.Add("@lesp_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_ListaEspera_DetailResult>(sql, p);
        }

        /// <summary>Inserta el registro y devuelve el lesp_Id generado (0 si falla).</summary>
        public async Task<int> AddAsync(tbListaEspera entity)
        {
            const string sql = "[Medico].[PR_Medico_ListaEspera_Insert]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cli_Id", entity.cli_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@tipoCon_Id", entity.tipoCon_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@lesp_FechaDeseada", entity.lesp_FechaDeseada, DbType.Date, ParameterDirection.Input);
            p.Add("@lesp_Prioridad", entity.lesp_Prioridad, DbType.String, ParameterDirection.Input, 20);
            p.Add("@lesp_Notas", entity.lesp_Notas, DbType.String, ParameterDirection.Input, 1000);
            p.Add("@lesp_UsuarioCrea", entity.lesp_UsuarioCrea, DbType.Int32, ParameterDirection.Input);

            var row = await DbApp.Select<ListaEsperaInsertRow>(sql, p);
            return (row != null && row.CodeStatus > 0) ? row.lesp_Id : 0;
        }

        public async Task<RequestResult> EditAsync(tbListaEspera entity)
        {
            const string sql = "[Medico].[PR_Medico_ListaEspera_Update]";
            var p = new DynamicParameters();
            p.Add("@lesp_Id", entity.lesp_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@tipoCon_Id", entity.tipoCon_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@lesp_FechaDeseada", entity.lesp_FechaDeseada, DbType.Date, ParameterDirection.Input);
            p.Add("@lesp_Prioridad", entity.lesp_Prioridad, DbType.String, ParameterDirection.Input, 20);
            p.Add("@lesp_Notas", entity.lesp_Notas, DbType.String, ParameterDirection.Input, 1000);
            p.Add("@lesp_UsuarioModifica", entity.lesp_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> CambiarEstadoAsync(int lespId, string estado, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_ListaEspera_CambiarEstado]";
            var p = new DynamicParameters();
            p.Add("@lesp_Id", lespId, DbType.Int32, ParameterDirection.Input);
            p.Add("@estado", estado, DbType.String, ParameterDirection.Input, 20);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> RemoveAsync(int id, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_ListaEspera_Delete]";
            var p = new DynamicParameters();
            p.Add("@lesp_Id", id, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        // ── Dropdown de mascotas (reutiliza SP existente) ─
        public IEnumerable<PR_Refugio_Mascotas_ListResult> MascotasDropdown()
        {
            const string sql = "[Refugio].[PR_Refugio_Mascotas_Dropdown]";
            return DbApp.Dropdown<PR_Refugio_Mascotas_ListResult>(sql);
        }

        private class ListaEsperaInsertRow
        {
            public int CodeStatus { get; set; }
            public string MessageStatus { get; set; }
            public int lesp_Id { get; set; }
        }
    }
}
