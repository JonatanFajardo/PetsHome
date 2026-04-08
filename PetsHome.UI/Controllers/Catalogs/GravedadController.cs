using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using PetsHome.UI.Models;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de gravedades")]
    public class GravedadController : BaseController
    {
        private readonly GravedadService _gravedadService;

        public GravedadController(GravedadService gravedadService)
        {
            _gravedadService = gravedadService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Catalogo/Gravedad/Index.cshtml");
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _gravedadService.ListAsync();
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

        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _gravedadService.FindAsync(id);
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

        [ActionName("Details")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _gravedadService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Gravedad no encontrada", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("~/Views/Catalogo/Gravedad/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(GravedadViewModel model)
        {
            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de gravedades", operacion))
                return RedirectToAction("AccessDenied", "Account");

            if (!model.isEdit)
            {
                Boolean createdItem = await _gravedadService.AddAsync(model);
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
                Boolean updatedItem = await _gravedadService.UpdateAsync(model);
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

        [PantallaAuthorize("Listado de gravedades", "eliminar")]
        public async Task<IActionResult> Remove(int grav_Id)
        {
            Boolean deletedItem = await _gravedadService.RemoveAsync(grav_Id);
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
