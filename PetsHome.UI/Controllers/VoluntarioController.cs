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
    [PantallaAuthorize("Listado de voluntarios")]
    public class VoluntarioController : BaseController
    {
        private readonly VoluntarioService _VoluntarioService;

        public VoluntarioController(VoluntarioService VoluntarioService)
        {
            _VoluntarioService = VoluntarioService;
        }

        [Breadcrumb("Voluntario", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index()
        {
            return View(new VoluntarioListViewModel());
        }

        [Breadcrumb("Crear", FromAction = "Index", FromController = typeof(VoluntarioController))]
        [PantallaAuthorize("Listado de voluntarios", "insertar")]
        public IActionResult Create()
        {
            return View(new VoluntarioFormViewModel());
        }

        [Breadcrumb("Lista", FromAction = "Index", FromController = typeof(VoluntarioController))]
        public async Task<IActionResult> List()
        {
            var itemListing = await _VoluntarioService.ListAsync();
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

        [Breadcrumb("Buscar", FromAction = "Index", FromController = typeof(VoluntarioController))]
        [PantallaAuthorize("Listado de voluntarios", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _VoluntarioService.FindAsync(id);
            if (itemSearched != null)
            {
                return View("Create", itemSearched);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(itemSearched, false);
            }
        }

        [Breadcrumb("Detalle", FromAction = "Index", FromController = typeof(VoluntarioController))]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _VoluntarioService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Voluntario no encontrado", AlertMessageType.Error);
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


        [Breadcrumb("Agregar", FromAction = "Index", FromController = typeof(VoluntarioController))]
        public async Task<IActionResult> Add(VoluntarioFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de voluntarios", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _VoluntarioService.AddAsync(model, userId);
                if (!createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _VoluntarioService.UpdateAsync(model, userId);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        [Breadcrumb("Eliminar", FromAction = "Index", FromController = typeof(VoluntarioController))]
        [PantallaAuthorize("Listado de voluntarios", "eliminar")]
        public async Task<IActionResult> Remove(int vol_Id)
        {
            Boolean deletedItem = await _VoluntarioService.RemoveAsync(vol_Id);
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

        [Breadcrumb("Validar Identidad", FromAction = "Index", FromController = typeof(VoluntarioController))]
        [HttpGet]
        public async Task<IActionResult> ValidarIdentidad(string per_Identidad, int? vol_Id)
        {
            if (string.IsNullOrWhiteSpace(per_Identidad))
                return Json(true);

            bool existe = await _VoluntarioService.IdentidadExistsAsync(per_Identidad, vol_Id ?? 0);
            return Json(!existe);
        }
    }
}
