using Dapper;
using Microsoft.Data.SqlClient;
using PetsHome.Business.Models;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
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
        /// Obtiene el total de próximas citas médicas programadas.
        /// </summary>
        /// <returns>El número de citas médicas con próxima cita programada.</returns>
        private async Task<int> ObtenerProximasCitasMedicasAsync()
        {
            try
            {
                using (var connection = new SqlConnection(PetsHomeDbContext.ConnectionString))
                {
                    const string sqlQuery = @"
                        SELECT COUNT(*)
                        FROM Refugio.tbCitaMedica
                        WHERE medic_ProximaCita IS NOT NULL
                          AND medic_ProximaCita >= CAST(GETDATE() as DATE)
                          AND medic_EsEliminado = 0";

                    var resultado = await connection.ExecuteScalarAsync<int>(sqlQuery);
                    return resultado;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el total de donaciones recibidas.
        /// </summary>
        /// <returns>El número de donaciones activas.</returns>
        private async Task<int> ObtenerDonacionesRecibidasAsync()
        {
            try
            {
                using (var connection = new SqlConnection(PetsHomeDbContext.ConnectionString))
                {
                    const string sqlQuery = @"
                        SELECT COUNT(*)
                        FROM Refugio.tbDonaciones
                        WHERE dona_EsEliminado = 0";

                    var resultado = await connection.ExecuteScalarAsync<int>(sqlQuery);
                    return resultado;
                }
            }
            catch (Exception)
            {
                return 0;
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

                // Card 1: Obtener total de mascotas registradas
                var mascotas = await _mascotaService.ListAsync();
                viewModel.TotalMascotasRegistradas = mascotas?.Count() ?? 0;

                // Card 2: Obtener próximas citas médicas programadas
                viewModel.ProximasCitasMedicas = await ObtenerProximasCitasMedicasAsync();

                // Card 3: Obtener donaciones recibidas
                viewModel.DonacionesRecibidas = await ObtenerDonacionesRecibidasAsync();

                // Card 4: Obtener adopciones pendientes (estado = "Pendiente")
                var todasLasAdopciones = await _adopcionService.ListAsync();
                viewModel.AdopcionesPendientes = todasLasAdopciones?
                    .Count(a => a.adop_Estado == "Pendiente") ?? 0;

                // Obtener últimas adopciones para mostrar en el dashboard
                viewModel.UltimasAdopciones = await ObtenerUltimasAdopcionesAsync();

                return viewModel;
            }
            catch (Exception)
            {
                return new HomeViewModel();
            }
        }
    }
}
