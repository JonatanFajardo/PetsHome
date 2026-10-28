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
    /// Repositorio del modulo Laboratorio: encabezado del examen (tbLaboratorio)
    /// y sus parametros con rango de referencia (tbLaboratorioDetalle).
    /// Aditivo: convive con el texto libre cita_ResultadosExamenes.
    /// </summary>
    public class LaboratorioRepository
    {
        // ── Encabezado ─────────────────────────────
        public async Task<IEnumerable<PR_Medico_Laboratorio_ListResult>> ListAsync(int? mascId, int? vetId)
        {
            const string sql = "[Medico].[PR_Medico_Laboratorio_List]";
            var p = new DynamicParameters();
            p.Add("@masc_Id", mascId, DbType.Int32, ParameterDirection.Input);
            p.Add("@vet_Id", vetId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_Laboratorio_ListResult>(sql, p);
        }

        public async Task<PR_Medico_Laboratorio_FindResult> FindAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_Laboratorio_Find]";
            var p = new DynamicParameters();
            p.Add("@lab_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Laboratorio_FindResult>(sql, p);
        }

        public async Task<PR_Medico_Laboratorio_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_Laboratorio_Detail]";
            var p = new DynamicParameters();
            p.Add("@lab_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_Laboratorio_DetailResult>(sql, p);
        }

        /// <summary>Inserta el encabezado y devuelve el lab_Id generado (0 si falla).</summary>
        public async Task<int> AddAsync(tbLaboratorio entity)
        {
            const string sql = "[Medico].[PR_Medico_Laboratorio_Insert]";
            var p = new DynamicParameters();
            // (parametros abajo)
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@lab_Examen", entity.lab_Examen, DbType.String, ParameterDirection.Input);
            p.Add("@lab_Tipo", entity.lab_Tipo, DbType.String, ParameterDirection.Input);
            p.Add("@lab_Estado", string.IsNullOrWhiteSpace(entity.lab_Estado) ? "Pendiente" : entity.lab_Estado, DbType.String, ParameterDirection.Input, 20);
            p.Add("@lab_Veterinario", entity.lab_Veterinario, DbType.Int32, ParameterDirection.Input);
            p.Add("@lab_Observaciones", entity.lab_Observaciones, DbType.String, ParameterDirection.Input);
            p.Add("@lab_UsuarioCrea", entity.lab_UsuarioCrea, DbType.Int32, ParameterDirection.Input);

            // El SP retorna { CodeStatus, MessageStatus, lab_Id }
            var row = await DbApp.Select<LaboratorioInsertRow>(sql, p);
            return (row != null && row.CodeStatus > 0) ? row.lab_Id : 0;
        }

        public async Task<RequestResult> EditAsync(tbLaboratorio entity)
        {
            const string sql = "[Medico].[PR_Medico_Laboratorio_Update]";
            var p = new DynamicParameters();
            p.Add("@lab_Id", entity.lab_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@lab_Examen", entity.lab_Examen, DbType.String, ParameterDirection.Input);
            p.Add("@lab_Tipo", entity.lab_Tipo, DbType.String, ParameterDirection.Input);
            p.Add("@lab_Veterinario", entity.lab_Veterinario, DbType.Int32, ParameterDirection.Input);
            p.Add("@lab_Observaciones", entity.lab_Observaciones, DbType.String, ParameterDirection.Input);
            p.Add("@lab_UsuarioModifica", entity.lab_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> CambiarEstadoAsync(int labId, string estado, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_Laboratorio_CambiarEstado]";
            var p = new DynamicParameters();
            p.Add("@lab_Id", labId, DbType.Int32, ParameterDirection.Input);
            p.Add("@lab_Estado", estado, DbType.String, ParameterDirection.Input, 20);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> RemoveAsync(int id, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_Laboratorio_Delete]";
            var p = new DynamicParameters();
            p.Add("@lab_Id", id, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        // ── Detalle / parametros ───────────────────
        public async Task<IEnumerable<PR_Medico_LaboratorioDetalle_ListResult>> DetalleListAsync(int labId)
        {
            const string sql = "[Medico].[PR_Medico_LaboratorioDetalle_List]";
            var p = new DynamicParameters();
            p.Add("@lab_Id", labId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_LaboratorioDetalle_ListResult>(sql, p);
        }

        public async Task<RequestResult> DetalleAddAsync(tbLaboratorioDetalle entity)
        {
            const string sql = "[Medico].[PR_Medico_LaboratorioDetalle_Insert]";
            var p = new DynamicParameters();
            p.Add("@lab_Id", entity.lab_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@labd_Parametro", entity.labd_Parametro, DbType.String, ParameterDirection.Input);
            p.Add("@labd_Valor", entity.labd_Valor, DbType.String, ParameterDirection.Input);
            p.Add("@labd_Unidad", entity.labd_Unidad, DbType.String, ParameterDirection.Input);
            p.Add("@labd_RangoMin", entity.labd_RangoMin, DbType.Decimal, ParameterDirection.Input);
            p.Add("@labd_RangoMax", entity.labd_RangoMax, DbType.Decimal, ParameterDirection.Input);
            p.Add("@labd_UsuarioCrea", entity.labd_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> DetalleRemoveAsync(int labdId, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_LaboratorioDetalle_Delete]";
            var p = new DynamicParameters();
            p.Add("@labd_Id", labdId, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        // ── Dropdown de mascotas (reutiliza SP existente) ─
        public IEnumerable<PR_Refugio_Mascotas_ListResult> MascotasDropdown()
        {
            const string sql = "[Refugio].[PR_Refugio_Mascotas_Dropdown]";
            return DbApp.Dropdown<PR_Refugio_Mascotas_ListResult>(sql);
        }

        private class LaboratorioInsertRow
        {
            public int CodeStatus { get; set; }
            public string MessageStatus { get; set; }
            public int lab_Id { get; set; }
        }
    }
}
