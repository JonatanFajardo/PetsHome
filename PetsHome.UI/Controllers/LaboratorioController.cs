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
    [PantallaAuthorize("Listado de laboratorio")]
    public class LaboratorioController : BaseController
    {
        private readonly LaboratorioService _service;
        private readonly IMapper _mapper;

        public LaboratorioController(LaboratorioService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Lente por rol: si el veterinario (RoleId 4) esta viendo la pantalla,
        /// filtra por su propio id (sus examenes + los sin asignar). Admin/recepcion ven todo.
        /// </summary>
        private int? ResolverVetLente()
        {
            var roleId = User.FindFirst("RoleId")?.Value;
            if (roleId == "4" && CurrentUserId.HasValue)
                return CurrentUserId.Value;
            return null;
        }

        [Breadcrumb("Laboratorio", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index() => View();

        public async Task<IActionResult> List(int? mascId = null)
        {
            var items = await _service.ListAsync(mascId, ResolverVetLente());
            if (items != null)
                return Json(new { data = items });

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        [Breadcrumb("Nuevo examen", FromAction = "Index", FromController = typeof(LaboratorioController))]
        [PantallaAuthorize("Listado de laboratorio", "insertar")]
        public IActionResult Create()
        {
            var model = new LaboratorioFormViewModel
            {
                MascotaList = _service.MascotaDropdown()
            };
            return View(model);
        }

        [PantallaAuthorize("Listado de laboratorio", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var item = await _service.FindAsync(id);
            if (item == null) return RedirectToAction("Index");
            var model = _mapper.Map<LaboratorioFormViewModel>(item);
            model.MascotaList = _service.MascotaDropdown();
            return View("Create", model);
        }

        [Breadcrumb("Detalle del examen", FromAction = "Index", FromController = typeof(LaboratorioController))]
        public async Task<IActionResult> Detail(int id)
        {
            var item = await _service.DetailAsync(id);
            if (item == null)
            {
                ShowAlert("Examen no encontrado", AlertMessageType.Error);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add(LaboratorioFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de laboratorio", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (model.isEdit)
            {
                bool ok = await _service.UpdateAsync(model, userId);
                if (ok)
                {
                    ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                    return RedirectToAction("Detail", new { id = model.lab_Id });
                }
            }
            else
            {
                // Si quien registra es veterinario (RoleId 4) y no se asignó otro, el examen queda a su nombre.
                if (!model.lab_Veterinario.HasValue && User.FindFirst("RoleId")?.Value == "4")
                    model.lab_Veterinario = userId;

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

        /// <summary>Avanza el estado del examen (Pendiente -> Completado -> Revisado).</summary>
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int lab_Id, string estado)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesión expirada, inicie sesión nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de laboratorio", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para cambiar el estado." });

            bool ok = await _service.CambiarEstadoAsync(lab_Id, estado, CurrentUserId.Value);
            return Json(new { ok });
        }

        [HttpPost]
        [PantallaAuthorize("Listado de laboratorio", "eliminar")]
        public async Task<IActionResult> Remove(int lab_Id)
        {
            int userId = CurrentUserId ?? 0;
            bool ok = await _service.RemoveAsync(lab_Id, userId);
            ShowAlert(ok ? "Eliminado" : AlertMessaje.Error, ok ? AlertMessageType.Success : AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        // ── Parametros / resultados ────────────────
        [HttpPost]
        public async Task<IActionResult> AgregarParametro(LaboratorioDetalleViewModel model)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesión expirada, inicie sesión nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de laboratorio", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para registrar resultados." });

            bool ok = await _service.DetalleAddAsync(model, CurrentUserId.Value);
            return Json(new { ok });
        }

        [HttpPost]
        public async Task<IActionResult> QuitarParametro(int labd_Id)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesión expirada, inicie sesión nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de laboratorio", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para quitar resultados." });

            bool ok = await _service.DetalleRemoveAsync(labd_Id, CurrentUserId.Value);
            return Json(new { ok });
        }
    }
}
