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
    public class EmpleadosCargoController : BaseController
    {
        private readonly EmpleadosCargoService _empleadosCargoService;

        public IActionResult Index()
        {
            return View("~/Views/Catalogo/EmpleadosCargo/Index.cshtml");
        }

        public EmpleadosCargoController(EmpleadosCargoService empleadosCargoService)
        {
            _empleadosCargoService = empleadosCargoService;
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _empleadosCargoService.ListAsync();
            if (!object.ReferenceEquals(itemListing, null))
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
            var itemSearched = await _empleadosCargoService.FindAsync(id);
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
                var itemDetail = await _empleadosCargoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Cargo no encontrado", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                return View("~/Views/Catalogo/EmpleadosCargo/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> Add(EmpleadoCargoViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _empleadosCargoService.AddAsync(model, userId);
                if (createdItem)
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
                Boolean updatedItem = await _empleadosCargoService.UpdateAsync(model, userId);
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

        public async Task<IActionResult> Remove(int emp_id)
        {
            Boolean deletedItem = await _empleadosCargoService.RemoveAsync(emp_id);
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