using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Permission("SOLICITUDES", "READ")]
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

        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _SolicitudService.FindAsync(id);
            if (itemSearched != null)
            {
                Detail(id);
                return View("Create", itemSearched);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(itemSearched, true);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var resultado = await _SolicitudService.DetailAsync(id);
            if (resultado == null)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }

            return View(resultado);
        }

        public async Task<IActionResult> Add(SolicitudViewModel model)
        { 
            // Eliminar guiones y espacios
            model.sol_Identidad = model.sol_Identidad.Replace("-", "").Replace(" ", "");
            model.sol_Telefono = model.sol_Telefono.Replace("-", "").Replace(" ", "");

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