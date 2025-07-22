using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class RecepcionMercanciaController : BaseController
    {
        private readonly RecepcionMercanciaService _recepcionService;
        private readonly RefugioService _refugioService;
        private readonly IMapper _mapper;

        public RecepcionMercanciaController(
            RecepcionMercanciaService recepcionService,
            RefugioService refugioService,
            IMapper mapper)
        {
            _recepcionService = recepcionService;
            _refugioService = refugioService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var resultado = await _recepcionService.DetailAsync(id);
            if (resultado == null)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }

            return View(resultado);
        }

        public async Task<IActionResult> Create(int id)
        {
            if (id == 0)
            {
                var model = new RecepcionMercanciaViewModel();
                model.recep_Id = id;
                model.recep_Fecha = DateTime.Now;
                return View(nameof(Create), model);
            }
            else
            {
                var model = await _recepcionService.FindAsync(id);
                if (model == null)
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                return View(nameof(Create), model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecepcionMercanciaViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Warning);
                    return View(model);
                }

                bool resultado = false;

                if (model.isEdit)
                {
                    resultado = await _recepcionService.UpdateAsync(model);
                    if (resultado)
                    {
                        ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                    }
                }
                else
                {
                    resultado = await _recepcionService.AddAsync(model);
                    if (resultado)
                    {
                        ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                    }
                }

                if (resultado)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return View(model);
                }
            }
            catch (Exception)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool resultado = await _recepcionService.RemoveAsync(id);

                if (resultado)
                {
                    ShowAlert(AlertMessaje.SuccessDelete, AlertMessageType.Success);
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                }

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        #region API Methods

        [HttpGet]
        public async Task<IActionResult> List()
        {
             
            var itemListing = await _recepcionService.ListAsync();
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

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var recepcion = await _recepcionService.FindAsync(id);
                return Json(recepcion);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetRefugios()
        {
            try
            {
                var refugios = _refugioService.RefugioDropdown();
                var result = refugios.Select(r => new { value = r.refg_Id, text = r.refg_Nombre });
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #endregion
    }
}