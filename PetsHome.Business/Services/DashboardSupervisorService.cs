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
    public class DashboardSupervisorService
    {
        private readonly DashboardSupervisorRepository _repository;
        private readonly ILogger<DashboardSupervisorService> _logger;

        public DashboardSupervisorService(DashboardSupervisorRepository repository, ILogger<DashboardSupervisorService> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<DashboardSupervisorViewModel> GetDashboardAsync()
        {
            try
            {
                var pills         = await _repository.PillsAsync();
                var kpis          = await _repository.KPIsAsync();
                var solicitudes   = await _repository.SolicitudesAsync();
                var estadoMasc    = await _repository.EstadoMascotasAsync();
                var eventos       = await _repository.EventosAsync();
                var movimientos   = await _repository.MovimientosAsync();

                return new DashboardSupervisorViewModel
                {
                    Pills         = pills,
                    KPIs          = kpis,
                    Solicitudes   = solicitudes?.ToList() ?? new List<PR_Supervisor_Dashboard_SolicitudesResult>(),
                    EstadoMascotas = estadoMasc,
                    Eventos       = eventos?.ToList() ?? new List<PR_Supervisor_Dashboard_EventosResult>(),
                    Movimientos   = movimientos?.ToList() ?? new List<PR_Supervisor_Dashboard_MovimientosInventarioResult>(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new DashboardSupervisorViewModel();
            }
        }
    }
}
