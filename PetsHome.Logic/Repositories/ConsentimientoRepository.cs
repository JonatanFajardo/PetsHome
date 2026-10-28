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
    /// Repositorio del modulo Consentimientos: autorizaciones que el dueño
    /// firma para un procedimiento (General, Cirugia, Anestesia, Eutanasia).
    /// Bitacora poco mutable: firmado/rechazado no se edita, solo se anula.
    /// </summary>
    public class ConsentimientoRepository
    {
        public async Task<IEnumerable<PR_Medico_Consentimiento_ListResult>> ListAsync(int? mascId, int? cliId, string estado)
        {
            const string sql = "[Medico].[PR_Medico_Consentimiento_List]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            p.Add("@cli_Id", cliId, DbType.Int32, ParameterDirection.Input);
            p.Add("@estado", estado, DbType.String, ParameterDirection.Input, 20);
            return await DbApp.SelectById<PR_Medico_Consentimiento_ListResult>(sql, p);
        }

        public async Task<PR_Medico_Consentimiento_FindResult> FindAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_Consentimiento_Find]";
            var p = new DynamicParameters();
            p.Add("@cons_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Consentimiento_FindResult>(sql, p);
        }

        public async Task<PR_Medico_Consentimiento_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_Consentimiento_Detail]";
            var p = new DynamicParameters();
            p.Add("@cons_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_Consentimiento_DetailResult>(sql, p);
        }

        /// <summary>Inserta el consentimiento y devuelve el cons_Id generado (0 si falla).</summary>
        public async Task<int> AddAsync(tbConsentimiento entity)
        {
            const string sql = "[Medico].[PR_Medico_Consentimiento_Insert]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cli_Id", entity.cli_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cons_Tipo", entity.cons_Tipo, DbType.String, ParameterDirection.Input, 20);
            p.Add("@cons_Texto", entity.cons_Texto, DbType.String, ParameterDirection.Input, -1);
            p.Add("@cons_Notas", entity.cons_Notas, DbType.String, ParameterDirection.Input, 1000);
            p.Add("@cons_UsuarioCrea", entity.cons_UsuarioCrea, DbType.Int32, ParameterDirection.Input);

            var row = await DbApp.Select<ConsentimientoInsertRow>(sql, p);
            return (row != null && row.CodeStatus > 0) ? row.cons_Id : 0;
        }

        public async Task<RequestResult> EditAsync(tbConsentimiento entity)
        {
            const string sql = "[Medico].[PR_Medico_Consentimiento_Update]";
            var p = new DynamicParameters();
            p.Add("@cons_Id", entity.cons_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cons_Tipo", entity.cons_Tipo, DbType.String, ParameterDirection.Input, 20);
            p.Add("@cons_Texto", entity.cons_Texto, DbType.String, ParameterDirection.Input, -1);
            p.Add("@cons_Notas", entity.cons_Notas, DbType.String, ParameterDirection.Input, 1000);
            p.Add("@cons_UsuarioModifica", entity.cons_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> CambiarEstadoAsync(int consId, string estado, string firmante, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_Consentimiento_CambiarEstado]";
            var p = new DynamicParameters();
            p.Add("@cons_Id", consId, DbType.Int32, ParameterDirection.Input);
            p.Add("@cons_Estado", estado, DbType.String, ParameterDirection.Input, 20);
            p.Add("@firmante", firmante, DbType.String, ParameterDirection.Input, 150);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> RemoveAsync(int id, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_Consentimiento_Delete]";
            var p = new DynamicParameters();
            p.Add("@cons_Id", id, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        // ── Dropdown de mascotas (reutiliza SP existente) ─
        public IEnumerable<PR_Refugio_Mascotas_ListResult> MascotasDropdown()
        {
            const string sql = "[Refugio].[PR_Refugio_Mascotas_Dropdown]";
            return DbApp.Dropdown<PR_Refugio_Mascotas_ListResult>(sql);
        }

        private class ConsentimientoInsertRow
        {
            public int CodeStatus { get; set; }
            public string MessageStatus { get; set; }
            public int cons_Id { get; set; }
        }
    }
}
