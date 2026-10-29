using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Pagos / abonos sobre facturas de clientes ([Veterinaria].[tbPagos]).
    /// Registrar (valida sobrepago, acumula fact_MontoPagado, salda la factura) y
    /// ListPorFactura (historial de pagos de una factura).
    /// </summary>
    public class PagoRepository
    {
        public async Task<PR_Veterinaria_Pago_RegistrarResult> RegistrarAsync(
            int fact_Id, decimal monto, string metodo, string referencia, string notas, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Pago_Registrar]";
            var p = new DynamicParameters();
            p.Add("@fact_Id", fact_Id, DbType.Int32, ParameterDirection.Input);
            p.Add("@pago_Monto", monto, DbType.Decimal, ParameterDirection.Input);
            p.Add("@pago_Metodo", metodo, DbType.String, ParameterDirection.Input);
            p.Add("@pago_Referencia", referencia, DbType.String, ParameterDirection.Input);
            p.Add("@pago_Notas", notas, DbType.String, ParameterDirection.Input);
            p.Add("@usuario", usuario, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Select<PR_Veterinaria_Pago_RegistrarResult>(sql, p);
        }

        public async Task<IEnumerable<PR_Veterinaria_Pago_ListPorFacturaResult>> ListPorFacturaAsync(int fact_Id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Pago_ListPorFactura]";
            var p = new DynamicParameters();
            p.Add("@fact_Id", fact_Id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Veterinaria_Pago_ListPorFacturaResult>(sql, p);
        }
    }
}
