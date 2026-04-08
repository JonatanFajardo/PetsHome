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
    [PantallaAuthorize("Listado de vias de administracion")]
    public class ViaAdministracionController : BaseController
    {
        private readonly ViaAdministracionService _viaAdministracionService;

        public ViaAdministracionController(ViaAdministracionService viaAdministracionService)
        {
            _viaAdministracionService = viaAdministracionService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Catalogo/ViaAdministracion/Index.cshtml");
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _viaAdministracionService.ListAsync();
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
            var itemSearched = await _viaAdministracionService.FindAsync(id);
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
                var itemDetail = await _viaAdministracionService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Vía de administración no encontrada", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("~/Views/Catalogo/ViaAdministracion/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(ViaAdministracionViewModel model)
        {
            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de vias de administracion", operacion))
                return RedirectToAction("AccessDenied", "Account");

            if (!model.isEdit)
            {
                Boolean createdItem = await _viaAdministracionService.AddAsync(model);
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
                Boolean updatedItem = await _viaAdministracionService.UpdateAsync(model);
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

        [PantallaAuthorize("Listado de vias de administracion", "eliminar")]
        public async Task<IActionResult> Remove(int viaAdmin_Id)
        {
            Boolean deletedItem = await _viaAdministracionService.RemoveAsync(viaAdmin_Id);
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
