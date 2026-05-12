using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de solicitudes")]
    public class SolicitudController : BaseController
    {
        private readonly SolicitudService _SolicitudService;
        private readonly MascotaService _mascotaService;

        public SolicitudController(SolicitudService SolicitudService, MascotaService mascotaService)
        {
            _SolicitudService = SolicitudService;
            _mascotaService = mascotaService;
        }

        [Breadcrumb("Solicitud", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index()
        {
            return View(new SolicitudFormViewModel());
        }

        [Breadcrumb("Crear", FromAction = "Index", FromController = typeof(SolicitudController))]
        [PantallaAuthorize("Listado de solicitudes", "insertar")]
        public async Task<IActionResult> Create(int? masc_Id)
        {
            // Si viene una mascota seleccionada, precargar datos de la mascota
            if (masc_Id.HasValue && masc_Id.Value > 0)
            {
                var pet = await _mascotaService.FindAsync(masc_Id.Value);
                if (pet != null)
                {
                    var model = new SolicitudFormViewModel
                    {
                        masc_Id = pet.masc_Id,
                        masc_Nombre = pet.masc_Nombre,
                        masc_Imagen = pet.masc_Imagen,
                        raza_Descripcion = pet.raza_Descripcion,
                        refg_Nombre = pet.refg_Nombre,
                        masc_EsAdoptado = pet.masc_EsAdoptado ?? false,
                        sol_Fecha = DateTime.Today
                    };
                    return View(model);
                }
            }

            return View(new SolicitudFormViewModel
            {
                sol_Fecha = DateTime.Today
            });
        }

        [Breadcrumb("Lista", FromAction = "Index", FromController = typeof(SolicitudController))]
        public async Task<IActionResult> List()
        {
            var itemListing = await _SolicitudService.ListAsync();
            if (itemListing != null)
            {
                return Json(new { data = itemListing });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        [Breadcrumb("Buscar", FromAction = "Index", FromController = typeof(SolicitudController))]
        [PantallaAuthorize("Listado de solicitudes", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _SolicitudService.FindAsync(id);
            if (itemSearched != null)
            {
                return View("Create", itemSearched);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        [Breadcrumb("Detalle", FromAction = "Index", FromController = typeof(SolicitudController))]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _SolicitudService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Solicitud no encontrada", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("Details", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        [Breadcrumb("Agregar", FromAction = "Index", FromController = typeof(SolicitudController))]
        public async Task<IActionResult> Add(SolicitudFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de solicitudes", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _SolicitudService.AddAsync(model, userId);
                if (!createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _SolicitudService.UpdateAsync(model, userId);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        [Breadcrumb("Cambiar Estado", FromAction = "Index", FromController = typeof(SolicitudController))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PantallaAuthorize("Listado de solicitudes", "editar")]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada." });

            var estadosValidos = new[] { "Pendiente", "En Revision", "Aprobada", "Rechazada" };
            if (!Array.Exists(estadosValidos, e => e.Equals(estado, StringComparison.OrdinalIgnoreCase)))
                return Json(new { success = false, message = "Estado no válido." });

            bool ok = await _SolicitudService.CambiarEstadoAsync(id, estado, CurrentUserId.Value);
            return Json(new { success = ok });
        }

        [Breadcrumb("Eliminar", FromAction = "Index", FromController = typeof(SolicitudController))]
        [PantallaAuthorize("Listado de solicitudes", "eliminar")]
        public async Task<IActionResult> Remove(int sol_Id)
        {
            Boolean deletedItem = await _SolicitudService.RemoveAsync(sol_Id);
            if (!deletedItem)
            {
                ShowAlert("Eliminado", AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }
    }
}
