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
    /// Cola dispensar→cobrar ([Veterinaria].[tbCargoDispensacion]).
    /// Ops: Encolar (idempotente) / ListPendientes / Facturar / Condonar / Reabrir.
    /// </summary>
    public class CargoDispensacionRepository
    {
        public async Task<IEnumerable<PR_Veterinaria_CargoDispensacion_ListPendientesResult>> ListPendientesAsync(int? cli_Id = null)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_ListPendientes]";
            var p = new DynamicParameters();
            p.Add("@cli_Id", cli_Id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Veterinaria_CargoDispensacion_ListPendientesResult>(sql, p);
        }

        /// <summary>Pendientes agrupables POR VISITA (cita): incluye cita/mascota/cliente/fecha.</summary>
        public async Task<IEnumerable<PR_Veterinaria_CargoDispensacion_ListPendientesResult>> ListPendientesPorVisitaAsync()
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_ListPendientesPorVisita]";
            return await DbApp.Select<PR_Veterinaria_CargoDispensacion_ListPendientesResult>(sql);
        }

        /// <summary>Factura/resuelve SOLO los cargos de UNA visita según disposición. Devuelve fact_Id (null si SinCargo).</summary>
        public async Task<PR_Veterinaria_CargoDispensacion_FacturarResult> FacturarVisitaAsync(int citaId, string disposicion, System.DateTime? vencimiento, string motivo, int usuario, string pagoMetodo = null, decimal? abonoInicial = null)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_FacturarVisita]";
            var p = new DynamicParameters();
            p.Add("@cita_Id", citaId, DbType.Int32, ParameterDirection.Input);
            p.Add("@disposicion", disposicion, DbType.String, ParameterDirection.Input);
            p.Add("@vencimiento", vencimiento, DbType.Date, ParameterDirection.Input);
            p.Add("@motivo", motivo, DbType.String, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            p.Add("@pago_Metodo", pagoMetodo, DbType.String, ParameterDirection.Input);
            p.Add("@abono_Inicial", abonoInicial, DbType.Decimal, ParameterDirection.Input);
            return await DbApp.Select<PR_Veterinaria_CargoDispensacion_FacturarResult>(sql, p);
        }

        public async Task<RequestResult> EncolarAsync(tbCargoDispensacion entity)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_Encolar]";
            var p = new DynamicParameters();
            p.Add("@receta_Id", entity.receta_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@masc_Id", entity.masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cli_Id", entity.cli_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Id", entity.cita_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@itm_Id", entity.itm_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_Cantidad", entity.cargo_Cantidad, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_PrecioUnitSnap", entity.cargo_PrecioUnitSnap, DbType.Decimal, ParameterDirection.Input);
            p.Add("@cargo_DescripcionSnap", entity.cargo_DescripcionSnap, DbType.String, ParameterDirection.Input);
            p.Add("@cargo_UsuarioCrea", entity.cargo_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        /// <summary>
        /// Encola desde el flujo de consulta: omite los snapshots de precio/descripción
        /// para que el SP los capture automáticamente desde tbItems. Idempotente por receta.
        /// </summary>
        public async Task<RequestResult> EncolarDesdeConsultaAsync(int receta_Id, int masc_Id, int cli_Id, int? cita_Id, int itm_Id, int cantidad, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_Encolar]";
            var p = new DynamicParameters();
            p.Add("@receta_Id", receta_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cli_Id", cli_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Id", cita_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@itm_Id", itm_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_Cantidad", cantidad, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_UsuarioCrea", usuario, DbType.Int32, ParameterDirection.Input);
            // price/desc omitidos -> el SP los toma de tbItems (snapshot)
            return await DbApp.ExecuteWithResult(sql, p);
        }

        /// <summary>
        /// Encola un cargo de SERVICIO (vacuna/laboratorio/procedimiento) desde consulta.
        /// Snapshot automatico de precio/nombre desde tbServicios. Idempotente por (cita_Id, serv_Id).
        /// </summary>
        public async Task<RequestResult> EncolarServicioDesdeConsultaAsync(int serv_Id, int masc_Id, int cli_Id, int? cita_Id, int cantidad, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_EncolarServicio]";
            var p = new DynamicParameters();
            p.Add("@serv_Id", serv_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@masc_Id", masc_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cli_Id", cli_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cita_Id", cita_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_Cantidad", cantidad, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_UsuarioCrea", usuario, DbType.Int32, ParameterDirection.Input);
            // price/desc omitidos -> el SP los toma de tbServicios (snapshot)
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> FacturarAsync(int cargo_Id, int fact_Id, int fdet_Id, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_Facturar]";
            var p = new DynamicParameters();
            p.Add("@cargo_Id", cargo_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@fact_Id", fact_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@fdet_Id", fdet_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_ResueltoPor", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        /// <summary>Crea UNA factura con todos los cargos pendientes del cliente y los marca Facturado. Devuelve el id de factura.</summary>
        public async Task<PR_Veterinaria_CargoDispensacion_FacturarResult> FacturarClienteAsync(int cli_Id, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_FacturarCliente]";
            var p = new DynamicParameters();
            p.Add("@cli_Id", cli_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_Veterinaria_CargoDispensacion_FacturarResult>(sql, p);
        }

        public async Task<RequestResult> CondonarAsync(int cargo_Id, string motivo, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_Condonar]";
            var p = new DynamicParameters();
            p.Add("@cargo_Id", cargo_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_MotivoResolucion", motivo, DbType.String, ParameterDirection.Input);
            p.Add("@cargo_ResueltoPor", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<RequestResult> ReabrirAsync(int cargo_Id, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_CargoDispensacion_Reabrir]";
            var p = new DynamicParameters();
            p.Add("@cargo_Id", cargo_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@cargo_UsuarioModifica", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.ExecuteWithResult(sql, p);
        }
    }
}
