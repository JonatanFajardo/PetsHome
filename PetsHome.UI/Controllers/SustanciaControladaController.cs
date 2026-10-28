using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Bitacora regulatoria de sustancias controladas: movimientos
    /// (Recibido / Administrado / Desechado / Devuelto), saldo por farmaco
    /// y registro con exigencia de testigo (desecho) y paciente (administracion).
    /// </summary>
    [Authorize]
    [PantallaAuthorize("Sustancias controladas")]
    public class SustanciaControladaController : BaseController
    {
        private readonly SustanciaControladaService _service;

        public SustanciaControladaController(SustanciaControladaService service)
        {
            _service = service;
        }

        [Breadcrumb("Sustancias controladas", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Index()
        {
            var model = new SustanciaControladaIndexViewModel
            {
                Saldos = await _service.SaldosAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> List(int? itmId = null)
        {
            var items = await _service.ListAsync(itmId);
            if (items != null)
                return Json(new { data = items });

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        [Breadcrumb("Nuevo movimiento", FromAction = "Index", FromController = typeof(SustanciaControladaController))]
        [PantallaAuthorize("Sustancias controladas", "insertar")]
        public async Task<IActionResult> Create()
        {
            var model = await BuildFormAsync(new SustanciaMovimientoFormViewModel());
            return View(model);
        }

        [Breadcrumb("Detalle del movimiento", FromAction = "Index", FromController = typeof(SustanciaControladaController))]
        public async Task<IActionResult> Detail(int id)
        {
            var item = await _service.DetailAsync(id);
            if (item == null)
            {
                ShowAlert("Movimiento no encontrado", AlertMessageType.Error);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [HttpPost]
        [PantallaAuthorize("Sustancias controladas", "insertar")]
        public async Task<IActionResult> Add(SustanciaMovimientoFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var (ok, mensaje, newId) = await _service.RegistrarAsync(model, CurrentUserId.Value);
                if (ok)
                {
                    ShowAlert(mensaje, AlertMessageType.Success);
                    return RedirectToAction("Detail", new { id = newId });
                }

                // El SP rechazo por testigo / paciente / cantidad / saldo: mostrar el motivo.
                ModelState.AddModelError(string.Empty, mensaje);
                ShowAlert(mensaje, AlertMessageType.Error);
            }

            return View("Create", await BuildFormAsync(model));
        }

        [HttpPost]
        [PantallaAuthorize("Sustancias controladas", "eliminar")]
        public async Task<IActionResult> Remove(int scm_Id)
        {
            int userId = CurrentUserId ?? 0;
            bool ok = await _service.RemoveAsync(scm_Id, userId);
            ShowAlert(ok ? "Movimiento anulado" : AlertMessaje.Error, ok ? AlertMessageType.Success : AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        private async Task<SustanciaMovimientoFormViewModel> BuildFormAsync(SustanciaMovimientoFormViewModel model)
        {
            model.FarmacoList = _service.FarmacoDropdown();
            model.MascotaList = _service.MascotaDropdown();
            model.TestigoList = await _service.TestigoDropdownAsync();
            return model;
        }
    }
}
