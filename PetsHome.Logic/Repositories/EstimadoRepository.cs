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
    /// Repositorio de estimados / presupuestos. La creacion y actualizacion son
    /// transaccionales (encabezado + lineas) en un solo SP. La conversion a
    /// factura inserta directo en las tablas de factura desde las lineas del
    /// presupuesto (SP propio, no toca los SP de factura).
    /// </summary>
    public class EstimadoRepository
    {
        public async Task<PR_Veterinaria_Estimado_CrearResult> CrearAsync(
            int? cliId, int? mascId, System.DateTime? vigencia, string notas, int usuarioCrea, string detalleJson)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Estimado_Crear]";
            var p = new DynamicParameters();
            p.Add("@cli_Id", cliId, DbType.Int32);
            p.Add("@masc_Id", mascId, DbType.Int32);
            p.Add("@est_Vigencia", vigencia, DbType.Date);
            p.Add("@est_Notas", notas, DbType.String);
            p.Add("@usuarioCrea", usuarioCrea, DbType.Int32);
            p.Add("@detalleJson", detalleJson, DbType.String, size: -1);
            return await DbApp.Find<PR_Veterinaria_Estimado_CrearResult>(sql, p);
        }

        public async Task<RequestResult> ActualizarAsync(
            int estId, int? cliId, int? mascId, System.DateTime? vigencia, string notas, int usuario, string detalleJson)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Estimado_Actualizar]";
            var p = new DynamicParameters();
            p.Add("@est_Id", estId, DbType.Int32);
            p.Add("@cli_Id", cliId, DbType.Int32);
            p.Add("@masc_Id", mascId, DbType.Int32);
            p.Add("@est_Vigencia", vigencia, DbType.Date);
            p.Add("@est_Notas", notas, DbType.String);
            p.Add("@usuario", usuario, DbType.Int32);
            p.Add("@detalleJson", detalleJson, DbType.String, size: -1);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<IEnumerable<PR_Veterinaria_Estimados_ListResult>> ListAsync(string estado)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Estimados_List]";
            var p = new DynamicParameters();
            p.Add("@est_Estado", string.IsNullOrWhiteSpace(estado) ? null : estado, DbType.String);
            return await DbApp.SelectById<PR_Veterinaria_Estimados_ListResult>(sql, p);
        }

        public async Task<PR_Veterinaria_Estimados_FindResult> FindAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Estimados_Find]";
            var p = new DynamicParameters();
            p.Add("@est_Id", id, DbType.Int32);
            return await DbApp.Find<PR_Veterinaria_Estimados_FindResult>(sql, p);
        }

        public async Task<IEnumerable<PR_Veterinaria_EstimadoDetalle_ListResult>> DetalleAsync(int id)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_EstimadoDetalle_List]";
            var p = new DynamicParameters();
            p.Add("@est_Id", id, DbType.Int32);
            return await DbApp.SelectById<PR_Veterinaria_EstimadoDetalle_ListResult>(sql, p);
        }

        public async Task<RequestResult> CambiarEstadoAsync(int estId, string estado, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Estimado_CambiarEstado]";
            var p = new DynamicParameters();
            p.Add("@est_Id", estId, DbType.Int32);
            p.Add("@est_Estado", estado, DbType.String);
            p.Add("@usuario", usuario, DbType.Int32);
            return await DbApp.ExecuteWithResult(sql, p);
        }

        public async Task<PR_Veterinaria_Estimado_ConvertirResult> ConvertirAFacturaAsync(int estId, int usuario)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Estimado_ConvertirAFactura]";
            var p = new DynamicParameters();
            p.Add("@est_Id", estId, DbType.Int32);
            p.Add("@usuario", usuario, DbType.Int32);
            return await DbApp.Find<PR_Veterinaria_Estimado_ConvertirResult>(sql, p);
        }
    }
}
