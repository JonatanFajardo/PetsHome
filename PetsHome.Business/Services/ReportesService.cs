using Microsoft.Extensions.Logging;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio de la pantalla /Reportes (replica /reports de OpenVPM).
    /// Orquesta los SP de reportes y, para ingresos, calcula el total del
    /// periodo anterior (mismo largo, terminando un dia antes de 'desde')
    /// para poder mostrar la comparacion.
    /// </summary>
    public class ReportesService
    {
        private readonly ReportesRepository _repo;
        private readonly ILogger<ReportesService> _logger;

        public ReportesService(ReportesRepository repo, ILogger<ReportesService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        /// <summary>
        /// Serie diaria del periodo + total del periodo + total del periodo
        /// inmediatamente anterior (mismo numero de dias) para comparar.
        /// </summary>
        public async Task<(decimal total, decimal totalAnterior, IEnumerable<PR_Veterinaria_Reportes_IngresosResult> serie)> IngresosAsync(DateTime desde, DateTime hasta)
        {
            try
            {
                var serie = (await _repo.IngresosAsync(desde, hasta))?.ToList()
                    ?? new List<PR_Veterinaria_Reportes_IngresosResult>();
                decimal total = serie.Sum(x => x.Monto);

                // Periodo anterior: mismo largo, terminando el dia antes de 'desde'.
                int largoDias = (hasta.Date - desde.Date).Days + 1;
                DateTime antHasta = desde.Date.AddDays(-1);
                DateTime antDesde = antHasta.AddDays(-(largoDias - 1));

                var serieAnterior = (await _repo.IngresosAsync(antDesde, antHasta))?.ToList()
                    ?? new List<PR_Veterinaria_Reportes_IngresosResult>();
                decimal totalAnterior = serieAnterior.Sum(x => x.Monto);

                return (total, totalAnterior, serie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (0m, 0m, new List<PR_Veterinaria_Reportes_IngresosResult>());
            }
        }

        /// <summary>Resumen de citas (una fila) + desglose por doctor (lista).</summary>
        public async Task<(PR_Medico_Reportes_CitasResult resumen, IEnumerable<PR_Medico_Reportes_CitasPorDoctorResult> porDoctor)> CitasAsync(DateTime desde, DateTime hasta)
        {
            try
            {
                var resumen = await _repo.CitasAsync(desde, hasta) ?? new PR_Medico_Reportes_CitasResult();
                var porDoctor = (await _repo.CitasPorDoctorAsync(desde, hasta))?.ToList()
                    ?? new List<PR_Medico_Reportes_CitasPorDoctorResult>();
                return (resumen, porDoctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return (new PR_Medico_Reportes_CitasResult(), new List<PR_Medico_Reportes_CitasPorDoctorResult>());
            }
        }

        /// <summary>Ranking de servicios mas usados en el periodo.</summary>
        public async Task<IEnumerable<PR_Veterinaria_Reportes_TopServiciosResult>> ServiciosAsync(DateTime desde, DateTime hasta)
        {
            try
            {
                return (await _repo.TopServiciosAsync(desde, hasta))?.ToList()
                    ?? new List<PR_Veterinaria_Reportes_TopServiciosResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<PR_Veterinaria_Reportes_TopServiciosResult>();
            }
        }

        /// <summary>Items con stock por debajo del minimo.</summary>
        public async Task<IEnumerable<PR_Inventario_Reportes_StockBajoResult>> InventarioAsync()
        {
            try
            {
                return (await _repo.StockBajoAsync())?.ToList()
                    ?? new List<PR_Inventario_Reportes_StockBajoResult>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<PR_Inventario_Reportes_StockBajoResult>();
            }
        }
    }
}
