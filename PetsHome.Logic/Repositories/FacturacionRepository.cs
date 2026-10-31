using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio de facturacion / POS. La creacion de factura es transaccional
    /// (encabezado + detalle mixto + salidas de inventario) en un solo SP.
    /// </summary>
    public class FacturacionRepository
    {
        public async Task<PR_Veterinaria_Factura_CrearResult> CrearAsync(
            int? cliId, int? mascId, int? refgId, string numero,
            decimal impuestoPorc, int usuarioCrea, decimal factDescuento, string detalleJson)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Factura_Crear]";
            var p = new DynamicParameters();
            p.Add("@cli_Id", cliId, DbType.Int32);
            p.Add("@masc_Id", mascId, DbType.Int32);
            p.Add("@refg_Id", refgId, DbType.Int32);
            p.Add("@fact_Numero", numero, DbType.String);
            p.Add("@impuestoPorc", impuestoPorc, DbType.Decimal);
            p.Add("@usuarioCrea", usuarioCrea, DbType.Int32);
            p.Add("@fact_Descuento", factDescuento, DbType.Decimal);
            p.Add("@detalleJson", detalleJson, DbType.String, size: -1);
            return await DbApp.Find<PR_Veterinaria_Factura_CrearResult>(sql, p);
        }

        /// <summary>Servicios activos (con flag de exento) para el POS de facturacion.</summary>
        public async Task<IEnumerable<PR_Veterinaria_Servicio_DropdownFacturaResult>> ServiciosDropdownAsync()
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Servicio_DropdownFactura]";
            return await DbApp.Select<PR_Veterinaria_Servicio_DropdownFacturaResult>(sql);
        }

        /// <summary>Aplica una nota de credito o un reembolso sobre una factura.</summary>
        public async Task<PR_Veterinaria_Factura_AjustarResult> AjustarAsync(
            int factId, string tipo, decimal monto, string motivo, string claveOperacion, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Factura_Ajustar]";
            var p = new DynamicParameters();
            p.Add("@fact_Id", factId, DbType.Int32);
            p.Add("@tipo", tipo, DbType.String);
            p.Add("@monto", monto, DbType.Decimal);
            p.Add("@motivo", motivo, DbType.String);
            p.Add("@claveOperacion", claveOperacion, DbType.String);
            p.Add("@usuario", usuario, DbType.Int32);
            return await DbApp.Find<PR_Veterinaria_Factura_AjustarResult>(sql, p);
        }

        /// <summary>Historial de ajustes (creditos/reembolsos) de una factura.</summary>
        public async Task<IEnumerable<PR_Veterinaria_FacturaAjustes_ListResult>> AjustesListAsync(int factId)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_FacturaAjustes_List]";
            var p = new DynamicParameters();
            p.Add("@fact_Id", factId, DbType.Int32);
            return await DbApp.SelectById<PR_Veterinaria_FacturaAjustes_ListResult>(sql, p);
        }

        public async Task<IEnumerable<PR_Veterinaria_Facturas_ListResult>> ListAsync()
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Facturas_List]";
            return await DbApp.Select<PR_Veterinaria_Facturas_ListResult>(sql);
        }

        /// <summary>Totales de cuentas por cobrar (Por cobrar / Vencido / Cobrado este mes).</summary>
        public async Task<PR_Veterinaria_Facturas_ResumenARResult> ResumenARAsync()
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Facturas_ResumenAR]";
            var rows = await DbApp.Select<PR_Veterinaria_Facturas_ResumenARResult>(sql);
            return System.Linq.Enumerable.FirstOrDefault(rows) ?? new PR_Veterinaria_Facturas_ResumenARResult();
        }

        public async Task<PR_Veterinaria_Facturas_FindResult> FindAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Facturas_Find]";
            var p = new DynamicParameters();
            p.Add("@fact_Id", id, DbType.Int32);
            return await DbApp.Find<PR_Veterinaria_Facturas_FindResult>(sql, p);
        }

        public async Task<IEnumerable<PR_Veterinaria_FacturaDetalle_ListResult>> DetalleAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_FacturaDetalle_List]";
            var p = new DynamicParameters();
            p.Add("@fact_Id", id, DbType.Int32);
            return await DbApp.SelectById<PR_Veterinaria_FacturaDetalle_ListResult>(sql, p);
        }

        public async Task<IEnumerable<PR_Veterinaria_ProductosVendibles_DropdownResult>> ProductosVendiblesAsync()
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_ProductosVendibles_Dropdown]";
            return await DbApp.Select<PR_Veterinaria_ProductosVendibles_DropdownResult>(sql);
        }
    }
}
