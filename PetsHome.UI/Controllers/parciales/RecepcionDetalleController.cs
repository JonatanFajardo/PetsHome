using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class RecepcionDetalleController : BaseController
    {
        private readonly RecepcionesDetallesService _recepcionDetalleService;
        private readonly ItemService _itemService;
        private readonly IMapper _mapper;

        public RecepcionDetalleController(
            RecepcionesDetallesService recepcionDetalleService,
            ItemService itemService,
            IMapper mapper)
        {
            _recepcionDetalleService = recepcionDetalleService;
            _itemService = itemService;
            _mapper = mapper;
        }

        #region API Methods

        [HttpGet]
        public async Task<JsonResult> ListByRecepcion(int id)
        {
            try
            {
                var detalles = await _recepcionDetalleService.ListByRecepcionAsync(id);
                return Json(new { data = detalles });
            }
            catch (Exception ex)
            {
                return Json(new { data = new object[0], error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var detalle = await _recepcionDetalleService.FindAsync(id);
                return Json(detalle);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetalle(RecepcionDetalleViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _recepcionDetalleService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Detalle agregado", AlertMessageType.Success);
                return RedirectToAction("EditRecepciones", "RecepcionMercancia", routeValues: new { id = model.recep_Id });
            }
            else
            {
                Boolean updatedItem = await _recepcionDetalleService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Detalle actualizado", AlertMessageType.Success);
                return RedirectToAction("EditRecepciones", "RecepcionMercancia", routeValues: new { id = model.recep_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDetalle(int recdet_Id, int recep_Id)
        {
            try
            {
                bool resultado = await _recepcionDetalleService.RemoveAsync(recdet_Id);
                
                if (resultado)
                {
                    ShowAlert("Detalle eliminado", AlertMessageType.Success);
                }
                else
                {
                    ShowAlert("Error al eliminar", AlertMessageType.Error);
                }

                return RedirectToAction("EditRecepciones", "RecepcionMercancia", routeValues: new { id = recep_Id });
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message, AlertMessageType.Error);
                return RedirectToAction("EditRecepciones", "RecepcionMercancia", routeValues: new { id = recep_Id });
            }
        }

        [HttpGet]
        public JsonResult GetItems()
        {
            try
            {
                var items = _itemService.ItemDropdown();
                return Json(items);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> FindDetalle(int id)
        {
            var itemSearched = await _recepcionDetalleService.FindAsync(id);
            if (itemSearched != null)
            {
                return Json(new { item = itemSearched, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return Json(new { success = false, message = "Detalle no encontrado" });
            }
        }

        #endregion
    }
}