using PetsHome.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.Business.Services
{
    public class HomeService
    {
        private readonly AdopcionService _adopcionService;
        private readonly MascotaService _mascotaService;

        public HomeService(AdopcionService adopcionService, MascotaService mascotaService)
        {
            _adopcionService = adopcionService;
            _mascotaService = mascotaService;
        }

        /// <summary>
        /// Obtiene las últimas 5 adopciones de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene las últimas 5 adopciones.</returns>
        public async Task<List<AdopcionViewModel>> ObtenerUltimasAdopcionesAsync()
        {
            try
            {
                var todasLasAdopciones = await _adopcionService.ListAsync();

                if (todasLasAdopciones == null)
                    return new List<AdopcionViewModel>();

                // Obtener las últimas 5 adopciones ordenadas por fecha descendente
                return todasLasAdopciones
                    .OrderByDescending(a => a.adop_FechaCrea)
                    .Take(5)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<AdopcionViewModel>();
            }
        }

        /// <summary>
        /// Obtiene las estadísticas del dashboard de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el modelo de vista con las estadísticas.</returns>
        public async Task<HomeViewModel> ObtenerEstadisticasDashboardAsync()
        {
            try
            {
                var viewModel = new HomeViewModel();

                // Obtener total de mascotas registradas
                var mascotas = await _mascotaService.ListAsync();
                viewModel.TotalMascotasRegistradas = mascotas?.Count() ?? 0;

                // Obtener últimas adopciones
                viewModel.UltimasAdopciones = await ObtenerUltimasAdopcionesAsync();

                // Obtener adopciones pendientes (estado = "Pendiente" o null)
                var todasLasAdopciones = await _adopcionService.ListAsync();
                viewModel.AdopcionesPendientes = todasLasAdopciones?
                    .Count(a => string.IsNullOrEmpty(a.adop_Estado) || a.adop_Estado == "Pendiente") ?? 0;

                // TODO: Implementar obtención de citas hoy cuando exista el servicio
                viewModel.CitasHoy = 0;

                // TODO: Implementar obtención de medicamentos por vencer cuando exista el servicio
                viewModel.MedicamentosPorVencer = 0;

                return viewModel;
            }
            catch (Exception)
            {
                return new HomeViewModel();
            }
        }
    }
}
