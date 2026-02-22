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
    public class CategoriaController : BaseController
    {
        private readonly CategoriaService _CategoriaService;

        public IActionResult Index()
        {
            return View("~/Views/Catalogo/Categoria/Index.cshtml");
        }

        public CategoriaController(CategoriaService CategoriaService)
        {
            _CategoriaService = CategoriaService;
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _CategoriaService.ListAsync();
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
            var itemSearched = await _CategoriaService.FindAsync(id);
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
                var itemDetail = await _CategoriaService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Categoría no encontrada", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                return View("~/Views/Catalogo/Categoria/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(CategoriaViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _CategoriaService.AddAsync(model, userId);
                return AjaxResult(createdItem);
            }
            else
            {
                Boolean updatedItem = await _CategoriaService.UpdateAsync(model, userId);
                return AjaxResult(updatedItem);
            }
        }

        public async Task<IActionResult> Remove(int cat_Id)
        {
            Boolean deletedItem = await _CategoriaService.RemoveAsync(cat_Id);
            return AjaxResult(deletedItem);
        }
    }
}
