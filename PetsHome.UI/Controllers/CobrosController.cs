using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using System;
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
        private readonly FacturacionService _facturacionService;

        public CobrosController(CargoDispensacionService cargoService, FacturacionService facturacionService)
        {
            _cargoService = cargoService;
            _facturacionService = facturacionService;
        }

        public IActionResult Index() => View();

        /// <summary>
        /// Parsea un monto en dinero sin depender de la cultura del servidor.
        /// El front envia "302.50" (punto decimal); con la cultura local el binder
        /// de formularios podria tomar el "." como separador de miles (-> 30250).
        /// </summary>
        private static decimal ParseMoneyInvariant(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0m;
            s = s.Trim();
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.InvariantCulture, out var v)) return v;
            if (decimal.TryParse(s, System.Globalization.NumberStyles.Number,
                    System.Globalization.CultureInfo.CurrentCulture, out v)) return v;
            return 0m;
        }

        /// <summary>Comprobante imprimible de la factura generada al cobrar.</summary>
        [HttpGet]
        public async Task<IActionResult> Comprobante(int id)
        {
            var model = await _facturacionService.ComprobanteAsync(id);
            if (model?.Header == null) return NotFound();
            return View(model);
        }

        /// <summary>Pendientes agrupados POR VISITA (cita). Fuente de la pantalla /Cobros.</summary>
        [HttpGet]
        public async Task<IActionResult> ListPorVisita()
        {
            var items = await _cargoService.ListPendientesPorVisitaAsync();
            return Json(new { data = items });
        }

        /// <summary>
        /// Cobra/resuelve UNA visita según disposición:
        /// 'Pagado' (factura saldada) | 'PorCobrar' (factura con vencimiento) | 'SinCargo' (condona con motivo).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> FacturarVisita([FromForm] int citaId, [FromForm] string disposicion,
            [FromForm] DateTime? vencimiento, [FromForm] string motivo,
            [FromForm] string metodo = null, [FromForm] string abonoInicial = null)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada." });
            // abonoInicial solo aplica en 'PorCobrar'; 0 o negativo => sin abono.
            decimal abonoVal = ParseMoneyInvariant(abonoInicial);
            decimal? abono = abonoVal > 0 ? abonoVal : (decimal?)null;
            var (ok, factId, message) = await _cargoService.FacturarVisitaAsync(
                citaId, disposicion, vencimiento,
                string.IsNullOrWhiteSpace(motivo) ? null : motivo.Trim(),
                CurrentUserId.Value,
                string.IsNullOrWhiteSpace(metodo) ? null : metodo.Trim(),
                abono);
            return Json(new { success = ok, factId, message });
        }

        /// <summary>
        /// Registra un pago/abono sobre una factura existente (abonar despues del checkout).
        /// Rechaza sobrepago. Devuelve { ok, saldo, estado, mensaje }.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarPago([FromForm] int factId, [FromForm] string monto,
            [FromForm] string metodo, [FromForm] string referencia, [FromForm] string notas)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, saldo = 0m, estado = (string)null, mensaje = "Sesión expirada." });
            var (ok, saldo, estado, mensaje) = await _facturacionService.RegistrarPagoAsync(
                factId, ParseMoneyInvariant(monto), metodo, referencia, notas, CurrentUserId.Value);
            return Json(new { ok, saldo, estado, mensaje });
        }

        [HttpGet]
        public async Task<IActionResult> ListPendientes()
        {
            var items = await _cargoService.ListPendientesAsync();
            return Json(new { data = items });
        }

        /// <summary>DEPRECADO: cobraba toda la deuda del cliente cross-visita. Usar FacturarVisita.</summary>
        [HttpPost]
        [Obsolete("El cobro ahora es por visita. Usar FacturarVisita.")]
        public async Task<IActionResult> FacturarCliente([FromForm] int cliId)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada." });
            var factId = await _cargoService.FacturarClienteAsync(cliId, CurrentUserId.Value);
            return Json(new { success = factId > 0, factId, message = factId > 0 ? "Factura generada." : "No se pudo facturar." });
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
