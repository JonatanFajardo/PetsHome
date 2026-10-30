using Dapper;
using PetsHome.Common.Entities;
using PetsHome.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    /// <summary>
    /// Repositorio de solo lectura para la pantalla /Reportes (replica /reports de OpenVPM).
    /// Cada metodo delega a un SP que agrega datos del periodo indicado.
    /// </summary>
    public class ReportesRepository
    {
        /// <summary>Serie diaria de ingresos del periodo (una fila por dia).</summary>
        public async Task<IEnumerable<PR_Veterinaria_Reportes_IngresosResult>> IngresosAsync(DateTime desde, DateTime hasta)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Reportes_Ingresos]";
            var p = new DynamicParameters();
            p.Add("@desde", desde, DbType.Date, ParameterDirection.Input);
            p.Add("@hasta", hasta, DbType.Date, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Veterinaria_Reportes_IngresosResult>(sql, p);
        }

        /// <summary>Resumen de citas del periodo (una sola fila: Total/Atendidas/NoShow/Canceladas).</summary>
        public async Task<PR_Medico_Reportes_CitasResult> CitasAsync(DateTime desde, DateTime hasta)
        {
            const string sql = "[Medico].[PR_Medico_Reportes_Citas]";
            var p = new DynamicParameters();
            p.Add("@desde", desde, DbType.Date, ParameterDirection.Input);
            p.Add("@hasta", hasta, DbType.Date, ParameterDirection.Input);
            return await DbApp.Find<PR_Medico_Reportes_CitasResult>(sql, p);
        }

        /// <summary>Desglose de citas por doctor en el periodo.</summary>
        public async Task<IEnumerable<PR_Medico_Reportes_CitasPorDoctorResult>> CitasPorDoctorAsync(DateTime desde, DateTime hasta)
        {
            const string sql = "[Medico].[PR_Medico_Reportes_CitasPorDoctor]";
            var p = new DynamicParameters();
            p.Add("@desde", desde, DbType.Date, ParameterDirection.Input);
            p.Add("@hasta", hasta, DbType.Date, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Medico_Reportes_CitasPorDoctorResult>(sql, p);
        }

        /// <summary>Ranking de servicios mas usados en el periodo.</summary>
        public async Task<IEnumerable<PR_Veterinaria_Reportes_TopServiciosResult>> TopServiciosAsync(DateTime desde, DateTime hasta)
        {
            const string sql = "[Veterinaria].[PR_Veterinaria_Reportes_TopServicios]";
            var p = new DynamicParameters();
            p.Add("@desde", desde, DbType.Date, ParameterDirection.Input);
            p.Add("@hasta", hasta, DbType.Date, ParameterDirection.Input);
            return await DbApp.SelectById<PR_Veterinaria_Reportes_TopServiciosResult>(sql, p);
        }

        /// <summary>Items con stock por debajo del minimo (sin filtro de fecha).</summary>
        public async Task<IEnumerable<PR_Inventario_Reportes_StockBajoResult>> StockBajoAsync()
        {
            const string sql = "[Inventario].[PR_Inventario_Reportes_StockBajo]";
            return await DbApp.Select<PR_Inventario_Reportes_StockBajoResult>(sql);
        }
    }
}
