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
    [PantallaAuthorize("Listado de mascotas")]
    public class HistorialMedicoController : BaseController
    {
        private readonly HistorialMedicoService _HistorialMedicoService;
        public HistorialMedicoController(HistorialMedicoService HistorialMedicoService)
        {
            _HistorialMedicoService = HistorialMedicoService;
        }

        [Breadcrumb("HistorialMedico", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index()
        {
            return View();
        }

        [Breadcrumb("Crear", FromAction = "Index", FromController = typeof(HistorialMedicoController))]
        [PantallaAuthorize("Listado de mascotas", "insertar")]
        public IActionResult Create()
        {
            return View();
        }

        [Breadcrumb("Lista", FromAction = "Index", FromController = typeof(HistorialMedicoController))]
        public async Task<IActionResult> List()
        {
            var itemListing = await _HistorialMedicoService.ListAsync();
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

        [Breadcrumb("Buscar", FromAction = "Index", FromController = typeof(HistorialMedicoController))]
        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _HistorialMedicoService.FindAsync(id);
            if (itemSearched != null)
            {
                return AjaxResult(itemSearched, true);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(itemSearched, false);
            }
        }

        [Breadcrumb("Detalle", FromAction = "Index", FromController = typeof(HistorialMedicoController))]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _HistorialMedicoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Historial médico no encontrado", AlertMessageType.Error);
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

        [Breadcrumb("Agregar", FromAction = "Index", FromController = typeof(HistorialMedicoController))]
        public async Task<IActionResult> Add(HistorialMedicoViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de mascotas", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _HistorialMedicoService.AddAsync(model, userId);
                if (!createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _HistorialMedicoService.UpdateAsync(model, userId);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        [Breadcrumb("Eliminar", FromAction = "Index", FromController = typeof(HistorialMedicoController))]
        [PantallaAuthorize("Listado de mascotas", "eliminar")]
        public async Task<IActionResult> Remove(int cita_Id)
        {
            Boolean deletedItem = await _HistorialMedicoService.RemoveAsync(cita_Id);
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