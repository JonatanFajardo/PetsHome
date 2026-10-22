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
