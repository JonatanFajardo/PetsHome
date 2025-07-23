using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Data;
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

        public async Task<IActionResult> EditSalidas(int id)
        {
            if (id == 0)
            {
                var model = new SalidaViewModel();
                model.sal_Id = id;
                model.sal_Fecha = DateTime.Now;
                var drop = Dropdown(model);
                return View(nameof(EditSalidas), drop);
            }
            else
            {
                var model = await _salidaService.FindAsync(id);
                if (model == null)
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                var dropdown = Dropdown(model);
                return View("EditSalidas", dropdown);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SalidaViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _salidaService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);

                // Buscar la salida recién creada para obtener su ID
                var salidasList = await _salidaService.ListAsync();
                var salidaCreada = salidasList
                    .Where(s => s.sal_Descripcion == model.sal_Descripcion && s.sal_Fecha == model.sal_Fecha)
                    .OrderByDescending(s => s.sal_Id)
                    .FirstOrDefault();

                return salidaCreada != null
                    ? RedirectToAction("EditSalidas", new { id = salidaCreada.sal_Id })
                    : RedirectToAction("EditSalidas");
            }
            else
            {
                Boolean updatedItem = await _salidaService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("EditSalidas", new { id = model.sal_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
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

        /// <summary>
        /// Cargamos Dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SalidaViewModel Dropdown(SalidaViewModel model)
        {
            model.LoadDropDownList(Dropdownlist.LoadTipoSalida(), _refugioService.RefugioDropdown());
            return model;
        }
    }

    public class VerificarStockRequest
    {
        public int RefugioId { get; set; }
        public Dictionary<int, int> ItemsConCantidades { get; set; }
    }
}