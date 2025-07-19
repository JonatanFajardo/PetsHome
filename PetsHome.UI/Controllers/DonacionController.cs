using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.Business.Helpers;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class DonacionController : BaseController
    {
        private readonly DonacionService _donacionService;

        public DonacionController(DonacionService donacionService)
        {
            _donacionService = donacionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var model = new DonacionViewModel();
            model.dona_FechaDonacion = DateTime.Now.Date; // Establecer fecha por defecto
            var drop = Dropdown(model);
            return View(drop);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var resultado = await _donacionService.DetailAsync(id);
            if (resultado == null)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }

            return View(resultado);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _donacionService.ListAsync();
            return Json(new { data = itemListing });
        }

        public async Task<IActionResult> Find(int id)
        {
            if (id != 0)
            {
                var itemSearched = await _donacionService.FindAsync(id);
                var dropdown = Dropdown(itemSearched);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(DonacionViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _donacionService.AddAsync(model);
                Boolean validation = Validation.IsInsert(createdItem, ModelState.IsValid);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _donacionService.UpdateAsync(model);
                Boolean validation = Validation.IsUpdate(updatedItem, ModelState.IsValid);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return View("Index");
            }

        ErrorResult:
            var modelWithDropdowns = Dropdown(model);
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, modelWithDropdowns);
        }

        public async Task<IActionResult> Remove(int dona_Id)
        {
            Boolean deletedItem = await _donacionService.RemoveAsync(dona_Id);
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

        /// <summary>
        /// Cargamos Dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DonacionViewModel Dropdown(DonacionViewModel model)
        {
            model.LoadDropDownList(
                _donacionService.RefugioDropdown(),
                _donacionService.TiposDonacionDropdown(),
                _donacionService.EstadosDropdown()
            );
            return model;
        }
    }
}