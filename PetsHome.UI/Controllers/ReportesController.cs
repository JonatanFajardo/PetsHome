using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Pantalla /Reportes (replica /reports de OpenVPM): 4 reportes
    /// (Ingresos, Citas, Servicios, Inventario). La vista consume los
    /// endpoints JSON via query string (desde/hasta en yyyy-MM-dd).
    /// </summary>
    [Authorize]
    [PantallaAuthorize("Reportes")]
    public class ReportesController : BaseController
    {
        private readonly ReportesService _reportesService;

        public ReportesController(ReportesService reportesService)
        {
            _reportesService = reportesService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Reportes";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Ingresos(DateTime desde, DateTime hasta)
        {
            var r = await _reportesService.IngresosAsync(desde, hasta);
            return Json(new
            {
                total = r.total,
                totalAnterior = r.totalAnterior,
                serie = r.serie.Select(s => new
                {
                    fecha = s.Fecha.ToString("yyyy-MM-dd"),
                    monto = s.Monto
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> Citas(DateTime desde, DateTime hasta)
        {
            var r = await _reportesService.CitasAsync(desde, hasta);
            return Json(new
            {
                total = r.resumen.Total,
                atendidas = r.resumen.Atendidas,
                noShow = r.resumen.NoShow,
                canceladas = r.resumen.Canceladas,
                porDoctor = r.porDoctor.Select(d => new
                {
                    doctor = d.Doctor,
                    cantidad = d.Cantidad
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> Servicios(DateTime desde, DateTime hasta)
        {
            var items = await _reportesService.ServiciosAsync(desde, hasta);
            return Json(new
            {
                items = items.Select(s => new
                {
                    servicio = s.Servicio,
                    cantidad = s.Cantidad,
                    ingreso = s.Ingreso
                })
            });
        }

        [HttpGet]
        public async Task<IActionResult> Inventario()
        {
            var stockBajo = await _reportesService.InventarioAsync();
            return Json(new
            {
                stockBajo = stockBajo.Select(s => new
                {
                    item = s.Item,
                    stock = s.Stock,
                    minimo = s.Minimo
                })
            });
        }
    }
}
