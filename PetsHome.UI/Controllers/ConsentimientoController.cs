using AutoMapper;
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
    [Authorize]
    [PantallaAuthorize("Listado de consentimientos")]
    public class ConsentimientoController : BaseController
    {
        private readonly ConsentimientoService _service;
        private readonly IMapper _mapper;

        public ConsentimientoController(ConsentimientoService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [Breadcrumb("Consentimientos", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index() => View();

        public async Task<IActionResult> List(int? mascId = null, string estado = null)
        {
            var items = await _service.ListAsync(mascId, null, estado);
            if (items != null)
                return Json(new { data = items });

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        [Breadcrumb("Nuevo consentimiento", FromAction = "Index", FromController = typeof(ConsentimientoController))]
        [PantallaAuthorize("Listado de consentimientos", "insertar")]
        public IActionResult Create()
        {
            var model = new ConsentimientoFormViewModel
            {
                MascotaList = _service.MascotaDropdown()
            };
            return View(model);
        }

        [PantallaAuthorize("Listado de consentimientos", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var item = await _service.FindAsync(id);
            if (item == null) return RedirectToAction("Index");

            // Un consentimiento firmado/rechazado es inmutable: no se edita, se ve el detalle.
            if (item.cons_Estado != "Pendiente")
            {
                ShowAlert("Un consentimiento firmado o rechazado no se puede editar.", AlertMessageType.Warning);
                return RedirectToAction("Detail", new { id });
            }

            var model = _mapper.Map<ConsentimientoFormViewModel>(item);
            model.MascotaList = _service.MascotaDropdown();
            return View("Create", model);
        }

        [Breadcrumb("Detalle del consentimiento", FromAction = "Index", FromController = typeof(ConsentimientoController))]
        public async Task<IActionResult> Detail(int id)
        {
            var item = await _service.DetailAsync(id);
            if (item == null)
            {
                ShowAlert("Consentimiento no encontrado", AlertMessageType.Error);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ConsentimientoFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de consentimientos", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (model.isEdit)
            {
                bool ok = await _service.UpdateAsync(model, userId);
                if (ok)
                {
                    ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                    return RedirectToAction("Detail", new { id = model.cons_Id });
                }
            }
            else
            {
                int newId = await _service.AddAsync(model, userId);
                if (newId > 0)
                {
                    ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                    return RedirectToAction("Detail", new { id = newId });
                }
            }

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            model.MascotaList = _service.MascotaDropdown();
            return View("Create", model);
        }

        /// <summary>Firma (requiere nombre del firmante) o rechaza un consentimiento pendiente.</summary>
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int cons_Id, string estado, string firmante)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesión expirada, inicie sesión nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de consentimientos", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para cambiar el estado." });

            if (estado == "Firmado" && string.IsNullOrWhiteSpace(firmante))
                return Json(new { ok = false, mensaje = "Debe indicar el nombre de quien firma." });

            bool ok = await _service.CambiarEstadoAsync(cons_Id, estado, firmante, CurrentUserId.Value);
            return Json(new { ok, mensaje = ok ? "Estado actualizado." : "No se pudo cambiar el estado (solo pendientes)." });
        }

        [HttpPost]
        [PantallaAuthorize("Listado de consentimientos", "eliminar")]
        public async Task<IActionResult> Remove(int cons_Id)
        {
            int userId = CurrentUserId ?? 0;
            bool ok = await _service.RemoveAsync(cons_Id, userId);
            ShowAlert(ok ? "Anulado" : AlertMessaje.Error, ok ? AlertMessageType.Success : AlertMessageType.Error);
            return RedirectToAction("Index");
        }
    }
}
