using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class SolicitudController : BaseController
    {
        private readonly SolicitudService _SolicitudService;

        public SolicitudController(SolicitudService SolicitudService
            )
        {
            _SolicitudService = SolicitudService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _SolicitudService.ListAsync();
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

        public async void Find(int id)
        {
            var itemSearched = await _SolicitudService.FindAsync(id);
            if (itemSearched != null)
            {
                Detail(id);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var itemDetail = await _SolicitudService.DetailAsync(id);
            if (itemDetail != null)
            {
                return View("Detail", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(SolicitudViewModel model)
        {

            if (!model.isEdit)
            {
                Boolean createdItem = await _SolicitudService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _SolicitudService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int sol_Id)
        {
            Boolean deletedItem = await _SolicitudService.RemoveAsync(sol_Id);
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