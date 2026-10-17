using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Facturacion")]
    public class FacturacionController : BaseController
    {
        private readonly FacturacionService _facturacionService;
        private readonly ClientesService _clientesService;
        private readonly ServiciosService _serviciosService;

        public FacturacionController(FacturacionService facturacionService,
            ClientesService clientesService, ServiciosService serviciosService)
        {
            _facturacionService = facturacionService;
            _clientesService = clientesService;
            _serviciosService = serviciosService;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> ClientesDropdown()
        {
            var clientes = await _clientesService.ListAsync() ?? new System.Collections.Generic.List<ClientesListViewModel>();
            var data = clientes
                .OrderByDescending(c => c.cli_EsConsumidorFinal)   // Consumidor Final primero
                .ThenBy(c => c.cli_Nombres)
                .Select(c => new
                {
                    id = c.cli_Id,
                    nombre = (c.cli_Nombres + " " + (c.cli_Apellidos ?? "")).Trim()
                });
            return Json(data);
        }

        public async Task<IActionResult> ServiciosDropdown()
        {
            var servicios = await _serviciosService.ListAsync() ?? new System.Collections.Generic.List<ServiciosListViewModel>();
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

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] FacturaCrearViewModel model)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada. Inicie sesión nuevamente." });

            var result = await _facturacionService.CrearAsync(model, CurrentUserId.Value);
            return Json(new { success = result.success, factId = result.factId, message = result.message });
        }

        public async Task<IActionResult> List()
        {
            var facturas = await _facturacionService.ListAsync();
            return Json(new { data = facturas });
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var detalle = await _facturacionService.DetalleAsync(id);
            return Json(detalle);
        }
    }
}
