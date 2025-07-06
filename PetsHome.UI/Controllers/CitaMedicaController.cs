using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.Common.Entities;
using PetsHome.Common.InternalEntities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class CitaMedicaController : BaseController
    {
        private readonly CitaMedicaService _HistorialMedicoService;
        private readonly MascotaService _mascotaService;
        private readonly ComportamientosService _comportamientosService;
     
 
        public CitaMedicaController(CitaMedicaService historialMedicoService, MascotaService mascotaService, ComportamientosService comportamientosService)
        {
            _HistorialMedicoService = historialMedicoService;
            _mascotaService = mascotaService;
            _comportamientosService = comportamientosService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var model = new CitaMedicaViewModel();
            //Dropdown(model);
            var drop = Dropdown(model);
            
            return View(drop);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _HistorialMedicoService.ListAsync();
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
            var itemSearched = await _HistorialMedicoService.FindAsync(id);
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

        public async Task<IActionResult> Detail(int id)
        {
            var itemDetail = await _HistorialMedicoService.DetailAsync(id);
            if (itemDetail != null)
            {
                return AjaxResult(itemDetail, true);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(CitaMedicaViewModel model)
        {


            if (!model.isEdit)
            {
                Boolean createdItem = await _HistorialMedicoService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _HistorialMedicoService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

            ErrorResult:
                return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int medic_Id)
        {
            Boolean deletedItem = await _HistorialMedicoService.RemoveAsync(medic_Id);
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

        //public List<PR_Refugio_Mascotas_DropdownResult> Dropdown()
        //{
        //    var mascotaList = _HistorialMedicoService.MascotaDropdown();
        //    //model.LoadDropDownList(mascotaList);
        //    return mascotaList.ToList();
        //}


        public CitaMedicaViewModel Dropdown(CitaMedicaViewModel model)
        {
            if (model == null)
                model = new CitaMedicaViewModel(); // ← protección

            var mascotaList = _HistorialMedicoService.MascotaDropdown();

            IEnumerable<ComportamientoViewModel> comp = _comportamientosService.ComportamientoDropdown();
            model.LoadDropDownList(comp);

            return model;
        }

        [HttpGet]
        public JsonResult GetMascotasDropdown()
        {
            try
            {
                var mascotas = _HistorialMedicoService.MascotaDropdown();
                return Json(mascotas);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = ex.Message });
            }
        }

    }
}