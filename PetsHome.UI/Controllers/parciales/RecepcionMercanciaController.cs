using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Data;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [SessionManagerAttribute("Listado de recepciones")]
    public class RecepcionMercanciaController : BaseController
    {
        private readonly RecepcionMercanciaService _recepcionService;
        private readonly RecepcionesDetallesService _recepcionDetalleService;
        private readonly RefugioService _refugioService;
        private readonly IMapper _mapper;

        public RecepcionMercanciaController(
            RecepcionMercanciaService recepcionService,
            RecepcionesDetallesService recepcionesDetallesService,
            RefugioService refugioService,
            IMapper mapper)
        {
            _recepcionService = recepcionService;
            _recepcionDetalleService = recepcionesDetallesService;
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

        public async Task<IActionResult> EditRecepciones(int id)
        {
            if (id == 0)
            {
                var model = new RecepcionMercanciaViewModel();
                model.recep_Id = id;
                model.recep_Fecha = DateTime.Now;
                var drop = Dropdown(model);
                return View(nameof(EditRecepciones), drop);
            }
            else
            {
                var model = await _recepcionService.FindAsync(id);
                if (model == null)
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                var dropdown = Dropdown(model);
                return View("EditRecepciones", dropdown);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RecepcionMercanciaViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _recepcionService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);

                // Buscar la recepción recién creada para obtener su ID
                var recepcionesList = await _recepcionService.ListAsync();
                var recepcionCreada = recepcionesList
                    .Where(r => r.recep_Descripcion == model.recep_Descripcion && r.recep_Fecha == model.recep_Fecha)
                    .OrderByDescending(r => r.recep_Id)
                    .FirstOrDefault();

                return recepcionCreada != null
                    ? RedirectToAction("EditRecepciones", new { id = recepcionCreada.recep_Id })
                    : RedirectToAction("EditRecepciones");
            }
            else
            {
                Boolean updatedItem = await _recepcionService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("EditRecepciones", new { id = model.recep_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
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

        [HttpGet("ListByRecepcion/{id}")]
        public async Task<JsonResult> ListByRecepcion(int id)
        {
            try
            {
                var itemListing = await _recepcionDetalleService.ListByRecepcionAsync(id);

                return Json(new { data = itemListing });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, success = false });
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

        /// <summary>
        /// Cargamos Dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RecepcionMercanciaViewModel Dropdown(RecepcionMercanciaViewModel model)
        {
            model.LoadDropDownList(Dropdownlist.LoadTipoRecepcion(), _refugioService.RefugioDropdown());
            return model;
        }

    }
}