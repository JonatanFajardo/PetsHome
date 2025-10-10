using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Attributes;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Controller para la gestión de reportes del sistema PetsHome
    /// </summary>
    [SessionManagerAttribute("Ver dashboard de reportes")]
    public class ReportesController : BaseController
    {
        private readonly ReportesService _reportesService;

        public ReportesController(ReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        /// <summary>
        /// Vista principal del módulo de reportes con dashboard
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = await _reportesService.GetDashboardAsync();
                return View(dashboard);
            }
            catch (Exception ex)
            {
                //ShowErrorMessage("Error al cargar el dashboard de reportes: " + ex.Message);
                return View(new ReportesDashboardViewModel());
            }
        }

        /// <summary>
        /// Vista de reporte de mascotas por raza
        /// </summary>
        public async Task<IActionResult> MascotasPorRaza()
        {
            try
            {
                var reporte = await _reportesService.GetMascotasPorRazaAsync();
                return View(reporte);
            }
            catch (Exception ex)
            {
                //ShowErrorMessage("Error al cargar el reporte de mascotas por raza: " + ex.Message);
                return View();
            }
        }

        /// <summary>
        /// Vista de reporte de adopciones por mes
        /// </summary>
        public async Task<IActionResult> AdopcionesPorMes()
        {
            try
            {
                var reporte = await _reportesService.GetAdopcionesPorMesAsync();
                return View(reporte);
            }
            catch (Exception ex)
            {
                //ShowErrorMessage("Error al cargar el reporte de adopciones por mes: " + ex.Message);
                return View();
            }
        }

        /// <summary>
        /// Vista de reporte de voluntarios
        /// </summary>
        public async Task<IActionResult> Voluntarios()
        {
            try
            {
                var reporte = await _reportesService.GetReporteVoluntariosAsync();
                return View(reporte);
            }
            catch (Exception ex)
            {
                //ShowErrorMessage("Error al cargar el reporte de voluntarios: " + ex.Message);
                return View();
            }
        }

        /// <summary>
        /// Vista de reporte de inventario
        /// </summary>
        public async Task<IActionResult> Inventario()
        {
            try
            {
                var reporte = await _reportesService.GetReporteInventarioAsync();
                return View(reporte);
            }
            catch (Exception ex)
            {
                //ShowErrorMessage("Error al cargar el reporte de inventario: " + ex.Message);
                return View();
            }
        }

        /// <summary>
        /// Vista de reporte de eventos
        /// </summary>
        public async Task<IActionResult> Eventos(DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var reporte = await _reportesService.GetReporteEventosAsync(fechaInicio, fechaFin);
                ViewBag.FechaInicio = fechaInicio;
                ViewBag.FechaFin = fechaFin;
                return View(reporte);
            }
            catch (Exception ex)
            {
                //ShowErrorMessage("Error al cargar el reporte de eventos: " + ex.Message);
                return View();
            }
        }

        /// <summary>
        /// Vista de reporte de salud de mascotas
        /// </summary>
        public async Task<IActionResult> SaludMascotas()
        {
            try
            {
                var reporte = await _reportesService.GetReporteSaludMascotasAsync();
                return View(reporte);
            }
            catch (Exception ex)
            {
                //ShowErrorMessage("Error al cargar el reporte de salud de mascotas: " + ex.Message);
                return View();
            }
        }

        /// <summary>
        /// API endpoint para obtener datos del dashboard en JSON
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetDashboardData()
        {
            try
            {
                var dashboard = await _reportesService.GetDashboardAsync();
                return Json(dashboard);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al obtener datos del dashboard: " + ex.Message });
            }
        }

        /// <summary>
        /// API endpoint para obtener datos de mascotas por raza en JSON
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetMascotasPorRazaData()
        {
            try
            {
                var reporte = await _reportesService.GetMascotasPorRazaAsync();
                return Json(reporte);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al obtener datos de mascotas por raza: " + ex.Message });
            }
        }

        /// <summary>
        /// API endpoint para obtener datos de adopciones por mes en JSON
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetAdopcionesPorMesData()
        {
            try
            {
                var reporte = await _reportesService.GetAdopcionesPorMesAsync();
                return Json(reporte);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al obtener datos de adopciones por mes: " + ex.Message });
            }
        }

        /// <summary>
        /// API endpoint para obtener datos de citas médicas por tipo en JSON
        /// </summary>
        [HttpGet]
        public async Task<JsonResult> GetCitasMedicasPorTipoData()
        {
            try
            {
                var reporte = await _reportesService.GetCitasMedicasPorTipoAsync();
                return Json(reporte);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al obtener datos de citas médicas por tipo: " + ex.Message });
            }
        }
    }
}