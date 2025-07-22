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
    public class SalidaController : BaseController
    {
        private readonly SalidaService _salidaService;
        private readonly RefugioService _refugioService;
        private readonly IMapper _mapper;

        public SalidaController(
            SalidaService salidaService,
            RefugioService refugioService,
            IMapper mapper)
        {
            _salidaService = salidaService;
            _refugioService = refugioService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var resultado = await _salidaService.DetailAsync(id);
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
                var model = new SalidaViewModel();
                model.sal_Id = id;
                model.sal_Fecha = DateTime.Now;
                return View(nameof(Create), model);
            }
            else
            {
                var model = await _salidaService.FindAsync(id);
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
        public async Task<IActionResult> Create(SalidaViewModel model)
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
                    resultado = await _salidaService.UpdateAsync(model);
                    if (resultado)
                    {
                        ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                    }
                }
                else
                {
                    resultado = await _salidaService.AddAsync(model);
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
                bool resultado = await _salidaService.RemoveAsync(id);

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
        public async Task<JsonResult> List()
        {
            try
            {
                var lista = await _salidaService.ListAsync();
                return Json(new { data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var salida = await _salidaService.FindAsync(id);
                return Json(salida);
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

        [HttpPost]
        public async Task<JsonResult> VerificarStock([FromBody] VerificarStockRequest request)
        {
            try
            {
                var disponible = await _salidaService.VerificarDisponibilidadStock(
                    request.RefugioId, 
                    request.ItemsConCantidades);
                
                return Json(new { disponible = disponible });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #endregion
    }

    public class VerificarStockRequest
    {
        public int RefugioId { get; set; }
        public Dictionary<int, int> ItemsConCantidades { get; set; }
    }
}