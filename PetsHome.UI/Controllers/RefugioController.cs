using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class RefugioController : BaseController
    {
        private readonly RefugioService _RefugioService;
        private readonly DepartamentoService _departamentoService;
        private readonly MunicipioService _municipioService;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            var model = new RefugioViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        public RefugioController(RefugioService RefugioService,
                                DepartamentoService DepartamentoService,
                                MunicipioService MunicipioService
        //                      IHttpContextAccessor httpContextAccessor
            )
        {
            _RefugioService = RefugioService;
            _departamentoService = DepartamentoService;
            _municipioService = MunicipioService;
            //  _httpContextAccessor = httpContextAccessor;
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
                //return Json(new { item = result, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var itemDetail = await _RefugioService.DetailAsync(id);
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
        public async Task<IActionResult> Add(RefugioViewModel model)
        {
            //string pantallas = _httpContextAccessor.HttpContext.Session.GetString("pantallas");

            if (!model.isEdit)
            {
                Boolean createdItem = await _RefugioService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _RefugioService.UpdateAsync(model);
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

        public RefugioViewModel Dropdown(RefugioViewModel model)
        {
            model.LoadDropDownList(
                _departamentoService.DepartamentoDropdown(), 
                _municipioService.MunicipioDropdown() );
            return model;
        }


    }
}
