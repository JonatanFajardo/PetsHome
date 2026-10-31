using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Presupuestos / estimados: cotizacion que, una vez Aceptada, se convierte
    /// en factura real. Pantalla de solo DINERO (modulo Facturacion y caja).
    /// </summary>
    [Authorize]
    [PantallaAuthorize("Presupuestos")]
    public class EstimadosController : BaseController
    {
        private readonly EstimadoService _estimadoService;
        private readonly ClientesService _clientesService;
        private readonly ServiciosService _serviciosService;
        private readonly FacturacionService _facturacionService;

        public EstimadosController(EstimadoService estimadoService, ClientesService clientesService,
            ServiciosService serviciosService, FacturacionService facturacionService)
        {
            _estimadoService = estimadoService;
            _clientesService = clientesService;
            _serviciosService = serviciosService;
            _facturacionService = facturacionService;
        }

        public IActionResult Index() => View();

        public IActionResult Create() => View();

        public IActionResult Detail(int id)
        {
            ViewData["estId"] = id;
            return View();
        }

        // ─── Dropdowns (reusan los servicios ya existentes) ───────────────
        public async Task<IActionResult> ClientesDropdown()
        {
            var clientes = await _clientesService.ListAsync() ?? new List<ClientesListViewModel>();
            var data = clientes
                .OrderByDescending(c => c.cli_EsConsumidorFinal)
                .ThenBy(c => c.cli_Nombres)
                .Select(c => new { id = c.cli_Id, nombre = (c.cli_Nombres + " " + (c.cli_Apellidos ?? "")).Trim() });
            return Json(data);
        }

        public async Task<IActionResult> ServiciosDropdown()
        {
            var servicios = await _serviciosService.ListAsync() ?? new List<ServiciosListViewModel>();
            var data = servicios.Select(s => new { id = s.serv_Id, nombre = s.serv_Nombre, precio = s.serv_Precio });
            return Json(data);
        }

        public async Task<IActionResult> ProductosDropdown()
        {
            var prods = await _facturacionService.ProductosVendiblesAsync();
            var data = (prods ?? Enumerable.Empty<Common.Entities.PR_Veterinaria_ProductosVendibles_DropdownResult>())
                .Select(p => new { id = p.itm_Id, nombre = p.Descripcion, precio = p.itm_Precio });
            return Json(data);
        }

        // ─── Datos ─────────────────────────────────────────────────────────
        public async Task<IActionResult> List(string estado)
        {
            var estimados = await _estimadoService.ListAsync(estado);
            return Json(new { data = estimados });
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var detalle = await _estimadoService.DetalleAsync(id);
            return Json(detalle);
        }

        // ─── Escritura ───────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] EstimadoFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada. Inicie sesión nuevamente." });

            var result = await _estimadoService.GuardarAsync(model, CurrentUserId.Value);
            return Json(new { success = result.success, estId = result.estId, message = result.message });
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado([FromBody] CambioEstadoRequest req)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada. Inicie sesión nuevamente." });

            var result = await _estimadoService.CambiarEstadoAsync(req.estId, req.estado, CurrentUserId.Value);
            return Json(new { success = result.success, message = result.message });
        }

        [HttpPost]
        public async Task<IActionResult> Convertir([FromBody] ConvertirRequest req)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada. Inicie sesión nuevamente." });

            var result = await _estimadoService.ConvertirAFacturaAsync(req.estId, CurrentUserId.Value);
            return Json(new { success = result.success, factId = result.factId, message = result.message });
        }

        public class CambioEstadoRequest
        {
            public int estId { get; set; }
            public string estado { get; set; }
        }

        public class ConvertirRequest
        {
            public int estId { get; set; }
        }
    }
}
