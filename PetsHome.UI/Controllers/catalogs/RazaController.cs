using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class RazaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly RazaService _razaService;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public IActionResult Index()
        {
            return View();
        }

        public RazaController(RazaService razaService,
                              IMapper mapper
        //                      IHttpContextAccessor httpContextAccessor
            )
        {
            _razaService = razaService;
            _mapper = mapper;
            //  _httpContextAccessor = httpContextAccessor;
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
            var itemDetail = await _razaService.DetailAsync(id);
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
        public async Task<JsonResult> Add(RazaViewModel model)
        {
            //string pantallas = _httpContextAccessor.HttpContext.Session.GetString("pantallas");

            if (!model.isEdit)
            {

                Boolean createdItem = await _razaService.AddAsync(model);
                if (!createdItem)
                {
                    ShowAlert("Insertado", AlertMessageType.Success);
                    return AjaxResult(true);
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return AjaxResult(false);
                }
            }
            else
            {
                Boolean updatedItem = await _razaService.UpdateAsync(model);
                if (!updatedItem)
                {
                    ShowAlert("Modificado", AlertMessageType.Success);
                    return AjaxResult(true);
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return AjaxResult(false);
                    //return RedirectToAction("Index");
                }
            }

        }

        public async Task<IActionResult> Remove(int raza_Id)
        {
            Boolean deletedItem = await _razaService.RemoveAsync(raza_Id);
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
