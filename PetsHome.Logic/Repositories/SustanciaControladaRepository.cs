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
    /// Repositorio de la bitacora de sustancias controladas
    /// (Medico.tbSustanciaMovimiento). Registra movimientos, lista la
    /// bitacora y calcula el saldo por farmaco. Reusa los dropdowns de
    /// items (farmacos), mascotas y veterinarios (testigos) existentes.
    /// </summary>
    public class SustanciaControladaRepository
    {
        public async Task<IEnumerable<PR_Medico_SustanciaMovimiento_ListResult>> ListAsync(int? itmId)
        {
            const string sql = "[Medico].[PR_Medico_SustanciaMovimiento_List]";
            var p = new DynamicParameters();
            p.Add("@itm_Id", itmId, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_SustanciaMovimiento_ListResult>(sql, p);
        }

        public async Task<PR_Medico_SustanciaMovimiento_DetailResult> DetailAsync(int id)
        {
            const string sql = "[Medico].[PR_Medico_SustanciaMovimiento_Detail]";
            var p = new DynamicParameters();
            p.Add("@scm_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_Medico_SustanciaMovimiento_DetailResult>(sql, p);
        }

        public async Task<IEnumerable<PR_Medico_SustanciaSaldo_ListResult>> SaldosAsync()
        {
            const string sql = "[Medico].[PR_Medico_SustanciaSaldo_List]";
            return await DbApp.Select<PR_Medico_SustanciaSaldo_ListResult>(sql);
        }

        /// <summary>Registra un movimiento y devuelve el mensaje del SP (para mostrar el motivo del rechazo).</summary>
        public async Task<(int scm_Id, int CodeStatus, string MessageStatus)> RegistrarConMensajeAsync(tbSustanciaMovimiento entity)
        {
            const string sql = "[Medico].[PR_Medico_SustanciaMovimiento_RegistrarMovimiento]";
            var p = new DynamicParameters();
            p.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@scm_Tipo", entity.scm_Tipo, DbType.String, ParameterDirection.Input, 20);
            p.Add("@scm_Cantidad", entity.scm_Cantidad, DbType.Decimal, ParameterDirection.Input);
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@scm_TestigoId", entity.scm_TestigoId, DbType.Int32, ParameterDirection.Input);
            p.Add("@scm_Lote", entity.scm_Lote, DbType.String, ParameterDirection.Input, 64);
            p.Add("@scm_DeaSchedule", entity.scm_DeaSchedule, DbType.String, ParameterDirection.Input, 10);
            p.Add("@scm_Notas", entity.scm_Notas, DbType.String, ParameterDirection.Input, 1000);
            p.Add("@scm_UsuarioCrea", entity.scm_UsuarioCrea, DbType.Int32, ParameterDirection.Input);

            var row = await DbApp.Select<RegistrarRow>(sql, p);
            if (row == null) return (0, -100, "No se pudo registrar el movimiento.");
            return (row.scm_Id, row.CodeStatus, row.MessageStatus);
        }

        public async Task<RequestResult> RemoveAsync(int id, int usuario)
        {
            const string sql = "[Medico].[PR_Medico_SustanciaMovimiento_Delete]";
            var p = new DynamicParameters();
            p.Add("@scm_Id", id, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        // ── Dropdowns reutilizados ─────────────────
        public IEnumerable<PR_Inventario_Items_DropdownResult> FarmacosDropdown()
        {
            const string sql = "[Inventario].[PR_Inventario_Items_Dropdown]";
            return DbApp.Dropdown<PR_Inventario_Items_DropdownResult>(sql);
        }

        public IEnumerable<PR_Refugio_Mascotas_ListResult> MascotasDropdown()
        {
            const string sql = "[Refugio].[PR_Refugio_Mascotas_Dropdown]";
            return DbApp.Dropdown<PR_Refugio_Mascotas_ListResult>(sql);
        }

        public async Task<IEnumerable<PR_Seguridad_Veterinarios_DropdownResult>> TestigosAsync()
        {
            const string sql = "[Seguridad].[PR_Seguridad_Veterinarios_Dropdown]";
            return await DbApp.Select<PR_Seguridad_Veterinarios_DropdownResult>(sql);
        }

        private class RegistrarRow
        {
            public int CodeStatus { get; set; }
            public string MessageStatus { get; set; }
            public int scm_Id { get; set; }
        }
    }
}
