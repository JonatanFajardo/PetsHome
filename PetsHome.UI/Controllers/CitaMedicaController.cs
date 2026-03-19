using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
using Microsoft.AspNetCore.Authorization;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    public class CitaMedicaController : BaseController
    {
        private readonly CitaMedicaService _HistorialMedicoService;
        private readonly MascotaService _mascotaService;
        private readonly ComportamientosService _comportamientosService;
        private readonly VacunaService _vacunaService;
        private readonly IMapper _mapper;


        public CitaMedicaController(CitaMedicaService historialMedicoService, MascotaService mascotaService, ComportamientosService comportamientosService, VacunaService vacunaService, IMapper mapper)
        {
            _HistorialMedicoService = historialMedicoService;
            _mascotaService = mascotaService;
            _comportamientosService = comportamientosService;
            _vacunaService = vacunaService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var model = new CitaMedicaFormViewModel();
            var drop = DropdownForm(model);

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
                // Map CitaMedicaFindViewModel to CitaMedicaFormViewModel using AutoMapper
                var formModel = _mapper.Map<CitaMedicaFormViewModel>(itemSearched);
                var dropdown = DropdownForm(formModel);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(itemSearched, false);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var resultado = await _HistorialMedicoService.DetailAsync(id);
            if (resultado == null)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }

            return View(resultado);
        } 

        public async Task<IActionResult> Add(CitaMedicaFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada, por favor inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            if (!model.isEdit)
            {
                Boolean createdItem = await _HistorialMedicoService.AddAsync(model);
                if (!createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _HistorialMedicoService.UpdateAsync(model);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

            ErrorResult:
                return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int cita_Id)
        {
            Boolean deletedItem = await _HistorialMedicoService.RemoveAsync(cita_Id);
            if (deletedItem)
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


        public CitaMedicaFindViewModel Dropdown(CitaMedicaFindViewModel model)
        {
            if (model == null)
                model = new CitaMedicaFindViewModel(); // ← protección

            var mascotaList = _HistorialMedicoService.MascotaDropdown();
            model.MascotaList = new SelectList(mascotaList, "masc_Id", "masc_Nombre");

            model.LoadDropDownList(_comportamientosService.ComportamientoDropdown(), _vacunaService.VacunaDropdown());

            return model;
        }

        public CitaMedicaFormViewModel DropdownForm(CitaMedicaFormViewModel model)
        {
            if (model == null)
                model = new CitaMedicaFormViewModel();

            var mascotaList = _HistorialMedicoService.MascotaDropdown();
            model.MascotaList = new SelectList(mascotaList ?? new List<MascotaDropdownViewModel>(), "masc_Id", "masc_Nombre");

            model.LoadDropDownList(_comportamientosService.ComportamientoDropdown(), _vacunaService.VacunaDropdown());

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


