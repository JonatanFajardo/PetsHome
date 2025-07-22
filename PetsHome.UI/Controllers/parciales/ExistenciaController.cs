using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.Logic.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class ExistenciaController : BaseController
    {
        private readonly ExistenciasService _existenciasService;
        private readonly RefugioService _refugioService;
        private readonly ItemService _itemService;
        private readonly IMapper _mapper;

        public ExistenciaController(
            ExistenciasService existenciasService,
            RefugioService refugioService,
            ItemService itemService,
            IMapper mapper)
        {
            _existenciasService = existenciasService;
            _refugioService = refugioService;
            _itemService = itemService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var model = await _existenciasService.DetailAsync(id);
                if (model == null)
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ShowAlert($"Error al cargar el detalle: {ex.Message}", AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public IActionResult StockBajo()
        {
            ViewData["Title"] = "Ítems con Stock Bajo";
            ViewData["FilterType"] = "stockbajo";
            return View("Index");
        }

        public IActionResult SinStock()
        {
            ViewData["Title"] = "Ítems Sin Stock";
            ViewData["FilterType"] = "sinstock";
            return View("Index");
        }

        public IActionResult Reportes()
        {
            return View();
        }

        #region API Methods

        [HttpGet]
        public async Task<JsonResult> List()
        {
            try
            {
                var lista = await _existenciasService.ListAsync();
                return Json(new { data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetStockBajo()
        {
            try
            {
                // El filtrado se puede hacer en el cliente por ahora
                var lista = await _existenciasService.ListAsync();
                return Json(new { data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSinStock()
        {
            try
            {
                // El filtrado se puede hacer en el cliente por ahora
                var lista = await _existenciasService.ListAsync();
                return Json(new { data = lista });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetByItemAndRefugio(int itemId, int refugioId)
        {
            try
            {
                var existencia = await _existenciasService.GetByItemAndRefugioAsync(itemId, refugioId);
                return Json(existencia);
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

        [HttpGet]
        public JsonResult GetItems()
        {
            try
            {
                var items = _itemService.ItemDropdown();
                var result = items.Select(i => new { value = i.itm_Id, text = i.itm_Descripcion });
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateStock([FromBody] UpdateStockRequest request)
        {
            try
            {
                var resultado = await _existenciasService.UpdateStockAsync(
                    request.ItemId, 
                    request.RefugioId, 
                    request.NuevoStock);
                
                if (resultado)
                {
                    return Json(new { success = true, message = "Stock actualizado correctamente" });
                }
                else
                {
                    return Json(new { success = false, message = "Error al actualizar el stock" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion
    }

    public class UpdateStockRequest
    {
        public int ItemId { get; set; }
        public int RefugioId { get; set; }
        public int NuevoStock { get; set; }
    }
}