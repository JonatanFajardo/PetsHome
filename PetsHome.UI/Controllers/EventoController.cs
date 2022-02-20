using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class EventoController : BaseController
    {
        private readonly EventoService _EventoService;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public IActionResult Index()
        {
            return View();
        }

        public EventoController(EventoService EventoService
        //                      IHttpContextAccessor httpContextAccessor
            )
        {
            _EventoService = EventoService;
            //  _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _EventoService.ListAsync();
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
            var itemSearched = await _EventoService.FindAsync(id);
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
            var itemDetail = await _EventoService.DetailAsync(id);
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
        public async Task<JsonResult> Add(EventoViewModel model)
        {
            //string pantallas = _httpContextAccessor.HttpContext.Session.GetString("pantallas");

            if (!model.isEdit)
            {

                Boolean createdItem = await _EventoService.AddAsync(model);
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
                Boolean updatedItem = await _EventoService.UpdateAsync(model);
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

        public async Task<IActionResult> Remove(int eve_Id)
        {
            Boolean deletedItem = await _EventoService.RemoveAsync(eve_Id);
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
