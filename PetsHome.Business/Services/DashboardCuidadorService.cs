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
    public class DashboardCuidadorService
    {
        private readonly DashboardCuidadorRepository _repository;
        private readonly ILogger<DashboardCuidadorService> _logger;

        public DashboardCuidadorService(DashboardCuidadorRepository repository, ILogger<DashboardCuidadorService> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<DashboardCuidadorViewModel> GetDashboardAsync()
        {
            try
            {
                var mascotas    = await _repository.MascotasActivasAsync();
                var citas       = await _repository.CitasHoyAsync();
                var alertas     = await _repository.AlertasActivasAsync();
                var solicitudes = await _repository.SolicitudesPendientesAsync();

                return new DashboardCuidadorViewModel
                {
                    MascotasActivas       = mascotas?.ToList()    ?? new List<PR_Refugio_DashboardCuidador_MascotasActivasResult>(),
                    CitasHoy              = citas?.ToList()       ?? new List<PR_Medico_DashboardCuidador_CitasHoyResult>(),
                    AlertasActivas        = alertas?.ToList()     ?? new List<PR_Medico_DashboardCuidador_AlertasActivasResult>(),
                    SolicitudesPendientes = solicitudes?.ToList() ?? new List<PR_Refugio_DashboardCuidador_SolicitudesPendientesResult>(),
                };
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new DashboardCuidadorViewModel();
            }
        }
    }
}
