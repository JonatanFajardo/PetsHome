using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class ProcedenciaController : BaseController
    {
        private readonly ProcedenciaService _procedenciaService;
        private readonly IMapper _mapper;

        public IActionResult Index()
        {
            return View();
        }

        public ProcedenciaController(ProcedenciaService procedenciaService,
                                IMapper mapper)
        {
            _procedenciaService = procedenciaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _procedenciaService.ListAsync();
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
            var itemSearched = await _procedenciaService.FindAsync(id);
            if (itemSearched != null)
            {
                return Json(new { item = itemSearched, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var itemDetail = await _procedenciaService.DetailAsync(id);
            if (itemDetail != null)
            {
                return Json(new { item = itemDetail, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        //[SessionManager("")]
        public async Task<IActionResult> Add(ProcedenciaViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _procedenciaService.AddAsync(model);
                if (!createdItem)
                {
                    ShowAlert("Insertado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Boolean updatedItem = await _procedenciaService.UpdateAsync(model);
                if (!updatedItem)
                {
                    ShowAlert("Modificado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }

        }

        public async Task<IActionResult> Remove(int proc_Id)
        {
            if (ModelState.IsValid)
            {
                Boolean deletedItem = await _procedenciaService.RemoveAsync(proc_Id);
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
            else
            {
                return View("Index");
            }
        }
    }
}
