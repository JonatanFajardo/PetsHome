using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class EmpleadoController : BaseController
    {
        private readonly EmpleadoService _EmpleadoService;
        //private readonly IHttpContextAccessor _httpContextAccessor;


        public EmpleadoController(EmpleadoService EmpleadoService
        //                      IHttpContextAccessor httpContextAccessor
            )
        {
            _EmpleadoService = EmpleadoService;
            //  _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var model = new EmpleadoViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }
        public async Task<IActionResult> List()
        {
            var itemListing = await _EmpleadoService.ListAsync();
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
            var itemSearched = await _EmpleadoService.FindAsync(id);
            if (itemSearched != null)
            {
                var dropdown = Dropdown(itemSearched);
                return View("Create", dropdown);
                //return Json(new { item = result, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(itemSearched, true);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var itemDetail = await _EmpleadoService.DetailAsync(id);
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

        //[SessionManager("")]
        public async Task<IActionResult> Add(EmpleadoViewModel model)
        {
            //string pantallas = _httpContextAccessor.HttpContext.Session.GetString("pantallas");

            if (!model.isEdit)
            {
                Boolean createdItem = await _EmpleadoService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _EmpleadoService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);

        }

        public async Task<IActionResult> Remove(int Empleado_Id)
        {
            Boolean deletedItem = await _EmpleadoService.RemoveAsync(Empleado_Id);
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

        public EmpleadoViewModel Dropdown(EmpleadoViewModel model)
        {
            //MascotaViewModel model = new MascotaViewModel();
            model.LoadDropDownList(_EmpleadoService.RefugioDropdown(), _EmpleadoService.EmpleadoCargoDropdown());
            return model;
        }
        //public EmpleadoViewModel Dropdown()
        //{
        //    EmpleadoViewModel itemListing = _EmpleadoService.RefugioDropdown();
        //    return itemListing;
        //}
    }
}
