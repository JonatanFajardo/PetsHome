using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    public class RefugioController : BaseController
    {
        private readonly RefugioService _RefugioService;
        private readonly DepartamentoService _departamentoService;
        private readonly MunicipioService _municipioService;

        public RefugioController(RefugioService RefugioService,
                                DepartamentoService DepartamentoService,
                                MunicipioService MunicipioService
            )
        {
            _RefugioService = RefugioService;
            _departamentoService = DepartamentoService;
            _municipioService = MunicipioService;
        }

        public IActionResult Index()
        {
            return View(new RefugioFormViewModel());
        }

        public IActionResult Create()
        {
            var model = new RefugioFormViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _RefugioService.ListAsync();
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
            var itemSearched = await _RefugioService.FindAsync(id);
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
                var itemDetail = await _RefugioService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Refugio no encontrado", AlertMessageType.Error);
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

        public async Task<IActionResult> Add(RefugioFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _RefugioService.AddAsync(model, userId);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _RefugioService.UpdateAsync(model, userId);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int refg_Id)
        {
            Boolean deletedItem = await _RefugioService.RemoveAsync(refg_Id);
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

        public RefugioFormViewModel Dropdown(RefugioFormViewModel model)
        {
            model.LoadDropDownList(
                _departamentoService.DepartamentoDropdown(),
                _municipioService.MunicipioDropdown());
            return model;
        }
    }
}
