using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Models;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class ReportantesTipoController : BaseController
    {
        private readonly ReportantesTipoService _reportantesTipoService;

        public ReportantesTipoController(ReportantesTipoService reportantesTipoService)
        {
            _reportantesTipoService = reportantesTipoService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Catalogo/ReportantesTipo/Index.cshtml");
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _reportantesTipoService.ListAsync();
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
            var itemSearched = await _reportantesTipoService.FindAsync(id);
            if (itemSearched != null)
            {
                return AjaxResult(itemSearched, true);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(itemSearched, true);
            }
        }

        [ActionName("Details")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _reportantesTipoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Tipo de reportante no encontrado", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                return View("~/Views/Catalogo/ReportantesTipo/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(ReportantesTipoFormViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _reportantesTipoService.AddAsync(model);
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
                Boolean updatedItem = await _reportantesTipoService.UpdateAsync(model);
                if (!updatedItem)
                {
                    ShowAlert("Modificado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return AjaxResult(false);
                }
            }
        }

        public async Task<IActionResult> Remove(int reptip_Id)
        {
            Boolean deletedItem = await _reportantesTipoService.RemoveAsync(reptip_Id);
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
