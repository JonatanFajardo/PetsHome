using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// POS de cobros pendientes por dispensación (Mejoras v3 - Fase 3).
    /// Lista la cola dispensar→cobrar y permite Facturar (por cliente) o Condonar.
    /// </summary>
    [Authorize]
    public class CobrosController : BaseController
    {
        private readonly CargoDispensacionService _cargoService;

        public CobrosController(CargoDispensacionService cargoService)
        {
            _cargoService = cargoService;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> ListPendientes()
        {
            var items = await _cargoService.ListPendientesAsync();
            return Json(new { data = items });
        }

        [HttpPost]
        public async Task<IActionResult> FacturarCliente([FromForm] int cliId)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada." });
            var ok = await _cargoService.FacturarClienteAsync(cliId, CurrentUserId.Value);
            return Json(new { success = ok, message = ok ? "Factura generada." : "No se pudo facturar." });
        }

        [HttpPost]
        public async Task<IActionResult> Condonar([FromForm] int cargoId, [FromForm] string motivo)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada." });
            if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 5)
                return Json(new { success = false, message = "El motivo debe tener al menos 5 caracteres." });
            var ok = await _cargoService.CondonarAsync(cargoId, motivo.Trim(), CurrentUserId.Value);
            return Json(new { success = ok, message = ok ? "Cargo condonado." : "No se pudo condonar." });
        }
    }
}
