using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class DashboardAdminService
    {
        private readonly DashboardAdminRepository        _repository;
        private readonly ILogger<DashboardAdminService> _logger;

        public DashboardAdminService(DashboardAdminRepository repository, ILogger<DashboardAdminService> logger)
        {
            _repository = repository;
            _logger     = logger;
        }

        public async Task<DashboardAdminViewModel> GetDashboardAsync()
        {
            var vm = new DashboardAdminViewModel();
            try
            {
                var kpisTask        = SafeOne(() => _repository.KPIsAsync(),
                                              new PR_General_DashboardAdmin_KPIsResult());
                var tendenciasTask  = SafeList(() => _repository.TendenciasAsync());
                var estadosTask     = SafeList(() => _repository.MascotasEstadoAsync());
                var razasTask       = SafeList(() => _repository.TopRazasAsync());
                var citasTask       = SafeList(() => _repository.CitasHoyAsync());
                var solicitudesTask = SafeList(() => _repository.SolicitudesPendientesAsync());
                var usuariosTask    = SafeList(() => _repository.UsuariosPorRolAsync());
                var heatmapTask     = SafeList(() => _repository.HeatmapCitasAsync());
                var inventarioTask  = SafeList(() => _repository.InventarioAlertasAsync());
                var embudoTask      = SafeList(() => _repository.EmbudoAdopcionAsync());

                await Task.WhenAll(kpisTask, tendenciasTask, estadosTask, razasTask,
                                   citasTask, solicitudesTask, usuariosTask, heatmapTask,
                                   inventarioTask, embudoTask);

                vm.KPIs                  = kpisTask.Result;
                vm.Tendencias            = tendenciasTask.Result;
                vm.MascotasEstado        = estadosTask.Result;
                vm.TopRazas              = razasTask.Result;
                vm.CitasHoy              = citasTask.Result;
                vm.SolicitudesPendientes = solicitudesTask.Result;
                vm.UsuariosPorRol        = usuariosTask.Result;
                vm.HeatmapCitas          = heatmapTask.Result;
                vm.InventarioAlertas     = inventarioTask.Result;
                vm.EmbudoAdopcion        = embudoTask.Result;
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error cargando dashboard admin");
            }
            return vm;
        }

        private async Task<T> SafeOne<T>(Func<Task<T>> fn, T fallback)
        {
            try   { return await fn(); }
            catch (Exception ex) { _logger.LogWarning(ex, "Dashboard admin query skipped"); return fallback; }
        }

        private async Task<List<T>> SafeList<T>(Func<Task<IEnumerable<T>>> fn)
        {
            try
            {
                var result = await fn();
                return result != null ? new List<T>(result) : new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Dashboard admin query skipped");
                return new List<T>();
            }
        }
    }
}
