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
        private readonly IMapper _mapper;
        private readonly RefugioService _RefugioService;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public IActionResult Index()
        {
            return View();
        }

        public RefugioController(RefugioService RefugioService,
                              IMapper mapper
        //                      IHttpContextAccessor httpContextAccessor
            )
        {
            _RefugioService = RefugioService;
            _mapper = mapper;
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
        public async Task<JsonResult> Add(RefugioViewModel model)
        {
            //string pantallas = _httpContextAccessor.HttpContext.Session.GetString("pantallas");

            if (!model.isEdit)
            {

                Boolean createdItem = await _RefugioService.AddAsync(model);
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
                Boolean updatedItem = await _RefugioService.UpdateAsync(model);
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
    }
}
