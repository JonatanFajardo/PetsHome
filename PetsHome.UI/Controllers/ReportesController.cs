using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

        // ══════════════════════ EXPORTAR CSV ══════════════════════
        // Un endpoint por reporte: reusa el mismo Service de la vista (no duplica
        // consultas) y devuelve un .csv con BOM UTF-8 para que Excel respete los
        // acentos. Separador coma; los decimales van con punto (cultura invariante)
        // para no chocar con la coma de columnas.

        [HttpGet]
        public async Task<IActionResult> IngresosCsv(DateTime desde, DateTime hasta)
        {
            var r = await _reportesService.IngresosAsync(desde, hasta);
            var rows = new List<string[]>
            {
                new[] { "Fecha", "Monto (L.)" }
            };
            foreach (var s in r.serie)
                rows.Add(new[] { s.Fecha.ToString("yyyy-MM-dd"), Dec(s.Monto) });
            rows.Add(new string[0]);
            rows.Add(new[] { "Total del periodo (L.)", Dec(r.total) });
            rows.Add(new[] { "Periodo anterior (L.)", Dec(r.totalAnterior) });
            return Csv(rows, "reporte_ingresos");
        }

        [HttpGet]
        public async Task<IActionResult> CitasCsv(DateTime desde, DateTime hasta)
        {
            var r = await _reportesService.CitasAsync(desde, hasta);
            var rows = new List<string[]>
            {
                new[] { "Resumen", "Cantidad" },
                new[] { "Total", r.resumen.Total.ToString(CultureInfo.InvariantCulture) },
                new[] { "Atendidas", r.resumen.Atendidas.ToString(CultureInfo.InvariantCulture) },
                new[] { "No-show (no se presentaron)", r.resumen.NoShow.ToString(CultureInfo.InvariantCulture) },
                new[] { "Canceladas", r.resumen.Canceladas.ToString(CultureInfo.InvariantCulture) },
                new string[0],
                new[] { "Doctor", "Cantidad" }
            };
            foreach (var d in r.porDoctor)
                rows.Add(new[] { d.Doctor, d.Cantidad.ToString(CultureInfo.InvariantCulture) });
            return Csv(rows, "reporte_citas");
        }

        [HttpGet]
        public async Task<IActionResult> ServiciosCsv(DateTime desde, DateTime hasta)
        {
            var items = await _reportesService.ServiciosAsync(desde, hasta);
            var rows = new List<string[]>
            {
                new[] { "Servicio", "Cantidad", "Ingreso (L.)" }
            };
            foreach (var s in items)
                rows.Add(new[] { s.Servicio, s.Cantidad.ToString(CultureInfo.InvariantCulture), Dec(s.Ingreso) });
            return Csv(rows, "reporte_servicios");
        }

        [HttpGet]
        public async Task<IActionResult> InventarioCsv()
        {
            var stockBajo = await _reportesService.InventarioAsync();
            var rows = new List<string[]>
            {
                new[] { "Producto", "Stock", "Minimo" }
            };
            foreach (var s in stockBajo)
                rows.Add(new[] { s.Item, Dec(s.Stock), Dec(s.Minimo) });
            return Csv(rows, "reporte_inventario");
        }

        /// <summary>Decimal con punto y 2 decimales (cultura invariante).</summary>
        private static string Dec(decimal v) => v.ToString("0.00", CultureInfo.InvariantCulture);

        /// <summary>
        /// Arma el CSV (comillas en cada campo, coma como separador, saltos CRLF)
        /// con BOM UTF-8 y lo devuelve como archivo descargable con la fecha de hoy.
        /// </summary>
        private FileContentResult Csv(IEnumerable<string[]> rows, string nombreBase)
        {
            var sb = new StringBuilder();
            foreach (var row in rows)
            {
                sb.Append(string.Join(",", row.Select(EscaparCsv)));
                sb.Append("\r\n");
            }

            // BOM UTF-8 para que Excel abra bien los acentos.
            var bom = new byte[] { 0xEF, 0xBB, 0xBF };
            var cuerpo = Encoding.UTF8.GetBytes(sb.ToString());
            var bytes = new byte[bom.Length + cuerpo.Length];
            Buffer.BlockCopy(bom, 0, bytes, 0, bom.Length);
            Buffer.BlockCopy(cuerpo, 0, bytes, bom.Length, cuerpo.Length);

            var nombre = $"{nombreBase}_{DateTime.Now:yyyyMMdd}.csv";
            return File(bytes, "text/csv", nombre);
        }

        private static string EscaparCsv(string v)
        {
            v = v ?? string.Empty;
            return "\"" + v.Replace("\"", "\"\"") + "\"";
        }
    }
}
