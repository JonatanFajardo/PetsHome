using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
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
    [PantallaAuthorize("Listado de tipos de parasito")]
    public class TipoParasitoController : BaseController
    {
        private readonly TipoParasitoService _tipoParasitoService;

        public TipoParasitoController(TipoParasitoService tipoParasitoService)
        {
            _tipoParasitoService = tipoParasitoService;
        }

        [Breadcrumb("TipoParasito", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index()
        {
            return View("~/Views/Catalogo/TipoParasito/Index.cshtml");
        }

        [Breadcrumb("Lista", FromAction = "Index", FromController = typeof(TipoParasitoController))]
        public async Task<IActionResult> List()
        {
            var itemListing = await _tipoParasitoService.ListAsync();
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

        [Breadcrumb("Buscar", FromAction = "Index", FromController = typeof(TipoParasitoController))]
        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _tipoParasitoService.FindAsync(id);
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

        [Breadcrumb("Detalle", FromAction = "Index", FromController = typeof(TipoParasitoController))]
        [ActionName("Details")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _tipoParasitoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Tipo de parásito no encontrado", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("~/Views/Catalogo/TipoParasito/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        [Breadcrumb("Agregar", FromAction = "Index", FromController = typeof(TipoParasitoController))]
        public async Task<IActionResult> Add(TipoParasitoViewModel model)
        {
            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de tipos de parasito", operacion))
                return RedirectToAction("AccessDenied", "Account");

            if (!model.isEdit)
            {
                Boolean createdItem = await _tipoParasitoService.AddAsync(model);
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
                Boolean updatedItem = await _tipoParasitoService.UpdateAsync(model);
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

        [Breadcrumb("Eliminar", FromAction = "Index", FromController = typeof(TipoParasitoController))]
        [PantallaAuthorize("Listado de tipos de parasito", "eliminar")]
        public async Task<IActionResult> Remove(int tipoPar_Id)
        {
            Boolean deletedItem = await _tipoParasitoService.RemoveAsync(tipoPar_Id);
            if (!deletedItem)
            {
                ShowAlert("Eliminado", AlertMessageType.Success);
                return AjaxResult(true);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(true);
            }
        }
    }
}
