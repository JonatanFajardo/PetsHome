using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class SalidaDetalleController : BaseController
    {
        private readonly SalidasDetallesService _salidaDetalleService;
        private readonly ItemService _itemService;
        private readonly IMapper _mapper;

        public SalidaDetalleController(
            SalidasDetallesService salidaDetalleService,
            ItemService itemService,
            IMapper mapper)
        {
            _salidaDetalleService = salidaDetalleService;
            _itemService = itemService;
            _mapper = mapper;
        }

        #region API Methods

        [HttpGet]
        public async Task<JsonResult> ListBySalida(int id)
        {
            try
            {
                var detalles = await _salidaDetalleService.ListBySalidaAsync(id);
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
                var detalle = await _salidaDetalleService.FindAsync(id);
                return Json(detalle);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetalle(SalidaDetalleViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _salidaDetalleService.AddAsync(model);
                if (!createdItem)
                    goto ErrorResult;
                ShowAlert("Detalle agregado", AlertMessageType.Success);
                return RedirectToAction("EditSalidas", "Salida", routeValues: new { id = model.sal_Id });
            }
            else
            {
                Boolean updatedItem = await _salidaDetalleService.UpdateAsync(model);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert("Detalle actualizado", AlertMessageType.Success);
                return RedirectToAction("EditSalidas", "Salida", routeValues: new { id = model.sal_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDetalle(int saldet_Id, int sal_Id)
        {
            try
            {
                bool resultado = await _salidaDetalleService.RemoveAsync(saldet_Id);
                
                if (resultado)
                {
                    ShowAlert("Detalle eliminado", AlertMessageType.Success);
                }
                else
                {
                    ShowAlert("Error al eliminar", AlertMessageType.Error);
                }

                return RedirectToAction("EditSalidas", "Salida", routeValues: new { id = sal_Id });
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message, AlertMessageType.Error);
                return RedirectToAction("EditSalidas", "Salida", routeValues: new { id = sal_Id });
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
            var itemSearched = await _salidaDetalleService.FindAsync(id);
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

        [HttpGet]
        public JsonResult GetStockDisponible(int itemId, int refugioId)
        {
            try
            {
                // Aquí deberías implementar la lógica para obtener stock disponible
                // Por ahora retorna un valor por defecto
                var stock = 100; // Implementar lógica real según tu sistema
                return Json(new { stock = stock });
            }
            catch (Exception ex)
            {
                return Json(new { stock = 0, error = ex.Message });
            }
        }

        #endregion
    }
}