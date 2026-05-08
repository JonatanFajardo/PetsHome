using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class ReporteAdopcionesService
    {
        private readonly ReporteAdopcionesRepository _repository;
        private readonly ILogger<ReporteAdopcionesService> _logger;

        public ReporteAdopcionesService(ReporteAdopcionesRepository repository, ILogger<ReporteAdopcionesService> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<ReporteAdopcionesViewModel> GetDashboardAsync()
        {
            try
            {
                var resumen = await _repository.ResumenAsync();
        var adopcionesPorMes = await _repository.AdopcionesPorMesAsync();
        var estadoSolicitudes = await _repository.EstadoSolicitudesAsync();
        var topRazas = await _repository.TopRazasAsync();
        var adopcionesRecientes = await _repository.AdopcionesRecientesAsync();

                return new ReporteAdopcionesViewModel
                {
                    Resumen = resumen?.ToList() ?? new List<PR_Refugio_ReporteAdopciones_ResumenResult>(),
            AdopcionesPorMes = adopcionesPorMes?.ToList() ?? new List<PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult>(),
            EstadoSolicitudes = estadoSolicitudes?.ToList() ?? new List<PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult>(),
            TopRazas = topRazas?.ToList() ?? new List<PR_Refugio_ReporteAdopciones_TopRazasResult>(),
            AdopcionesRecientes = adopcionesRecientes?.ToList() ?? new List<PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult>(),
                };
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new ReporteAdopcionesViewModel();
            }
        }
    }
}
