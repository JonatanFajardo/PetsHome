using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class IngresoController : BaseController
    {
        private readonly IngresoService _ingresoService;
        private readonly ReportesAbandonoService _reportesAbandonoService;
        private readonly RefugioService _refugioService;

        public IngresoController(
            IngresoService ingresoService,
            ReportesAbandonoService reportesAbandonoService,
            RefugioService refugioService)
        {
            _ingresoService = ingresoService;
            _reportesAbandonoService = reportesAbandonoService;
            _refugioService = refugioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var model = new IngresoFormViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _ingresoService.ListAsync();
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
            var itemSearched = await _ingresoService.FindAsync(id);
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
                var itemDetail = await _ingresoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Ingreso no encontrado", AlertMessageType.Error);
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

        public async Task<IActionResult> Add(IngresoFormViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _ingresoService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _ingresoService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int ingr_Id)
        {
            Boolean deletedItem = await _ingresoService.RemoveAsync(ingr_Id);
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

        public IngresoFormViewModel Dropdown(IngresoFormViewModel model)
        {
            var reportesList = _reportesAbandonoService.ListAsync().Result;
            var refugioList = _refugioService.RefugioDropdown();

            model.LoadDropDownList(reportesList, refugioList);
            return model;
        }
    }
}
