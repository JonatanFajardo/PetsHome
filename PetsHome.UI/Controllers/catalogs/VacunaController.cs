using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using PetsHome.UI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de vacunas")]
    public class VacunaController : BaseController
    {
        private readonly VacunaService _vacunaService;

        [Breadcrumb("Vacuna", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index()
        {
            return View("~/Views/Catalogo/Vacuna/Index.cshtml", new VacunaFormViewModel());
        }

        public VacunaController(VacunaService vacunaService)
        {
            _vacunaService = vacunaService;
        }

        [Breadcrumb("Lista", FromAction = "Index", FromController = typeof(VacunaController))]
        public async Task<IActionResult> List()
        {
            var itemListing = await _vacunaService.ListAsync();
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

        [Breadcrumb("Buscar", FromAction = "Index", FromController = typeof(VacunaController))]
        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _vacunaService.FindAsync(id);
            if (itemSearched != null)
            {
                return Json(new { item = itemSearched, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        [Breadcrumb("Detalle", FromAction = "Index", FromController = typeof(VacunaController))]
        [ActionName("Details")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _vacunaService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Vacuna no encontrada", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                return View("~/Views/Catalogo/Vacuna/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }


        [Breadcrumb("Agregar", FromAction = "Index", FromController = typeof(VacunaController))]
        public async Task<IActionResult> Add(VacunaFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de vacunas", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _vacunaService.AddAsync(model, userId);
                if (!createdItem)
                {
                    ShowAlert("Insertado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Boolean updatedItem = await _vacunaService.UpdateAsync(model, userId);
                if (!updatedItem)
                {
                    ShowAlert("Modificado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
        }

        [Breadcrumb("Eliminar", FromAction = "Index", FromController = typeof(VacunaController))]
        [PantallaAuthorize("Listado de vacunas", "eliminar")]
        public async Task<IActionResult> Remove(int vac_Id)
        {
            Boolean deletedItem = await _vacunaService.RemoveAsync(vac_Id);
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

        [Breadcrumb("Validar Descripcion", FromAction = "Index", FromController = typeof(VacunaController))]
        [HttpGet]
        public async Task<IActionResult> ValidarDescripcion(string vac_Descripcion, int? vac_Id)
        {
            if (string.IsNullOrWhiteSpace(vac_Descripcion))
                return Json(true);

            bool existe = await _vacunaService.DescripcionExistsAsync(vac_Descripcion, vac_Id ?? 0);
            return Json(!existe);
        }
    }
}
