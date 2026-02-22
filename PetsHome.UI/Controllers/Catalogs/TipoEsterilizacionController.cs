using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Models;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class TipoEsterilizacionController : BaseController
    {
        private readonly TipoEsterilizacionService _tipoEsterilizacionService;

        public TipoEsterilizacionController(TipoEsterilizacionService tipoEsterilizacionService)
        {
            _tipoEsterilizacionService = tipoEsterilizacionService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Catalogo/TipoEsterilizacion/Index.cshtml");
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _tipoEsterilizacionService.ListAsync();
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
            var itemSearched = await _tipoEsterilizacionService.FindAsync(id);
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

        [ActionName("Details")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _tipoEsterilizacionService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Tipo de esterilización no encontrado", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("~/Views/Catalogo/TipoEsterilizacion/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(TipoEsterilizacionViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _tipoEsterilizacionService.AddAsync(model);
                if (createdItem)
                {
                    ShowAlert("Insertado", AlertMessageType.Success);
                    return AjaxResult(true);
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return AjaxResult(false);
                }
            }
            else
            {
                Boolean updatedItem = await _tipoEsterilizacionService.UpdateAsync(model);
                if (updatedItem)
                {
                    ShowAlert("Modificado", AlertMessageType.Success);
                    return AjaxResult(true);
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return AjaxResult(false);
                }
            }
        }

        public async Task<IActionResult> Remove(int tipoEst_Id)
        {
            Boolean deletedItem = await _tipoEsterilizacionService.RemoveAsync(tipoEst_Id);
            if (deletedItem)
            {
                ShowAlert("Eliminado", AlertMessageType.Success);
                return AjaxResult(true);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(false);
            }
        }
    }
}
