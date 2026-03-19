using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    public class RazaController : BaseController
    {
        private readonly RazaService _razaService;

        public IActionResult Index()
        {
            return View("~/Views/Catalogo/Raza/Index.cshtml");
        }

        public RazaController(RazaService razaService)
        {
            _razaService = razaService;
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _razaService.ListAsync();
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
            var itemSearched = await _razaService.FindAsync(id);
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
                var itemDetail = await _razaService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Raza no encontrada", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                return View("~/Views/Catalogo/Raza/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(RazaFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _razaService.AddAsync(model, userId);
                return AjaxResult(createdItem);
            }
            else
            {
                Boolean updatedItem = await _razaService.UpdateAsync(model, userId);
                return AjaxResult(!updatedItem);
            }
        }

        public async Task<IActionResult> Remove(int raza_Id)
        {
            Boolean deletedItem = await _razaService.RemoveAsync(raza_Id);
            return AjaxResult(deletedItem);
        }

        [HttpGet]
        public async Task<IActionResult> ValidarDescripcion(string raza_Descripcion, int? raza_Id)
        {
            if (string.IsNullOrWhiteSpace(raza_Descripcion))
                return Json(true);

            bool existe = await _razaService.DescripcionExistsAsync(raza_Descripcion, raza_Id ?? 0);
            return Json(!existe);
        }
    }
}
