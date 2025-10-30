using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class ReportesAbandonoController : BaseController
    {
        private readonly ReportesAbandonoService _reportesAbandonoService;
        private readonly ReportantesTipoService _reportantesTipoService;
        private readonly RefugioService _refugioService;

        public ReportesAbandonoController(
            ReportesAbandonoService reportesAbandonoService,
            ReportantesTipoService reportantesTipoService,
            RefugioService refugioService)
        {
            _reportesAbandonoService = reportesAbandonoService;
            _reportantesTipoService = reportantesTipoService;
            _refugioService = refugioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var model = new ReportesAbandonoFormViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _reportesAbandonoService.ListAsync();
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
            var itemSearched = await _reportesAbandonoService.FindAsync(id);
            if (itemSearched != null)
            {
                var dropdown = Dropdown(itemSearched);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _reportesAbandonoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Reporte de abandono no encontrado", AlertMessageType.Error);
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

        public async Task<IActionResult> Add(ReportesAbandonoFormViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _reportesAbandonoService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _reportesAbandonoService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int repa_Id)
        {
            Boolean deletedItem = await _reportesAbandonoService.RemoveAsync(repa_Id);
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

        public ReportesAbandonoFormViewModel Dropdown(ReportesAbandonoFormViewModel model)
        {
            var reportantesTipoList = _reportantesTipoService.ListAsync().Result;
            var refugioList = _refugioService.RefugioDropdown();

            model.LoadDropDownList(reportantesTipoList, refugioList);
            return model;
        }
    }
}
