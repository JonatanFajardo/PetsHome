using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class VoluntarioController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly VoluntarioService _VoluntarioService;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public IActionResult Index()
        {
            return View();
        }

        public VoluntarioController(VoluntarioService VoluntarioService,
                              IMapper mapper
        //                      IHttpContextAccessor httpContextAccessor
            )
        {
            _VoluntarioService = VoluntarioService;
            _mapper = mapper;
            //  _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _VoluntarioService.ListAsync();
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
            var itemSearched = await _VoluntarioService.FindAsync(id);
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
            var itemDetail = await _VoluntarioService.DetailAsync(id);
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
        public async Task<JsonResult> Add(VoluntarioViewModel model)
        {
            //string pantallas = _httpContextAccessor.HttpContext.Session.GetString("pantallas");

            if (!model.isEdit)
            {

                Boolean createdItem = await _VoluntarioService.AddAsync(model);
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
                Boolean updatedItem = await _VoluntarioService.UpdateAsync(model);
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

        public async Task<IActionResult> Remove(int vol_Id)
        {
            Boolean deletedItem = await _VoluntarioService.RemoveAsync(vol_Id);
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
