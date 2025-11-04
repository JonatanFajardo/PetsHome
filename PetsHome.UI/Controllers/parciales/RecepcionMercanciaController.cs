using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Controlador para gestionar las recepciones de mercancía.
    /// </summary>
    public class RecepcionMercanciaController : BaseController
    {
        private readonly RecepcionMercanciaService _recepcionService;
        private readonly RecepcionDetalleService _detalleService;
        private readonly RefugioService _refugioService;
        private readonly ItemService _itemService;
        private readonly ProcedenciaService _procedenciaService;
        private readonly IMapper _mapper;

        public RecepcionMercanciaController(RecepcionMercanciaService recepcionService,
                                           RecepcionDetalleService detalleService,
                                           RefugioService refugioService,
                                           ItemService itemService,
                                           ProcedenciaService procedenciaService,
                                           IMapper mapper)
        {
            _recepcionService = recepcionService;
            _detalleService = detalleService;
            _refugioService = refugioService;
            _itemService = itemService;
            _procedenciaService = procedenciaService;
            _mapper = mapper;
        }

        /// <summary>
        /// Vista principal del índice de recepciones.
        /// </summary>
        /// <returns>Vista Index.</returns>
        public IActionResult Index() => View();

        /// <summary>
        /// Lista todas las recepciones de mercancía.
        /// </summary>
        /// <returns>JSON con la lista de recepciones.</returns>
        public async Task<IActionResult> List()
        {
            var itemListing = await _recepcionService.ListAsync();
            return Json(new { data = itemListing });
        }

        /// <summary>
        /// Obtiene el detalle de una recepción de mercancía para mostrar en vista.
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>Vista DetailRecepcion con el detalle completo.</returns>
        public async Task<IActionResult> Detail(int id)
        {
            var itemDetail = await _recepcionService.DetailAsync(id);
            if (itemDetail != null)
            {
                ViewBag.Detalles = await _detalleService.ListByRecepcionAsync(id);
                return View("DetailRecepcion", itemDetail);
            }
            else
            {
                ShowAlert("No se encontró la recepción", AlertMessageType.Error);
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Obtiene el detalle de una recepción de mercancía en formato JSON (para AJAX).
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>JSON con el detalle de la recepción.</returns>
        public async Task<IActionResult> DetailJson(int id)
        {
            var itemDetail = await _recepcionService.DetailAsync(id);
            if (itemDetail != null)
            {
                return Json(new { item = itemDetail, success = true });
            }
            else
            {
                return Json(new { success = false, message = "No se encontró la recepción" });
            }
        }

        /// <summary>
        /// Lista los detalles de una recepción específica.
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>JSON con la lista de detalles.</returns>
        public async Task<IActionResult> ListDetalles(int id)
        {
            var listDetalles = await _detalleService.ListByRecepcionAsync(id);
            return Json(new { data = listDetalles });
        }

        /// <summary>
        /// Vista de edición/creación de recepción de mercancía.
        /// </summary>
        /// <param name="id">ID de la recepción. 0 para crear nueva.</param>
        /// <returns>Vista FormPartialRecepcion.</returns>
        public async Task<IActionResult> FormPartialRecepcion(int id)
        {
            if (id == 0)
            {
                var model = new RecepcionMercanciaFormViewModel
                {
                    recep_Id = id,
                    recep_Fecha = DateTime.Now
                };
                ViewBag.Refugios = _refugioService.RefugioDropdown();
                ViewBag.Procedencias = _procedenciaService.ProcedenciaDropdown();
                ViewBag.Items = _itemService.ItemDropdown();
                return View(nameof(FormPartialRecepcion), model);
            }
            else
            {
                var result = await _recepcionService.FindAsync(id);
                result.ListadoDetalles = await _detalleService.ListByRecepcionAsync(id);
                ViewBag.Refugios = _refugioService.RefugioDropdown();
                ViewBag.Procedencias = _procedenciaService.ProcedenciaDropdown();
                ViewBag.Items = _itemService.ItemDropdown();
                return View(nameof(FormPartialRecepcion), result);
            }
        }

        /// <summary>
        /// Agrega o actualiza una recepción de mercancía.
        /// </summary>
        /// <param name="model">Modelo de la recepción.</param>
        /// <returns>Redireccionamiento según resultado.</returns>
        [HttpPost]
        public async Task<IActionResult> Add(RecepcionMercanciaFormViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _recepcionService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Recepción insertada correctamente", AlertMessageType.Success);
                return RedirectToAction(nameof(FormPartialRecepcion));
            }
            else
            {
                Boolean updatedItem = await _recepcionService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Recepción actualizada correctamente", AlertMessageType.Success);
                return RedirectToAction(nameof(FormPartialRecepcion), routeValues: new { id = model.recep_Id });
            }

        ErrorResult:
            ViewBag.Refugios = _refugioService.RefugioDropdown();
            ViewBag.Procedencias = _procedenciaService.ProcedenciaDropdown();
            ViewBag.Items = _itemService.ItemDropdown();
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        /// <summary>
        /// Agrega o actualiza un detalle de recepción.
        /// </summary>
        /// <param name="model">Modelo del formulario que contiene el detalle.</param>
        /// <returns>Redireccionamiento según resultado.</returns>
        [HttpPost]
        public async Task<IActionResult> AddDetalle(RecepcionMercanciaFormViewModel model)
        {
            if (!model.Detalle.isEdit)
            {
                Boolean createdItem = await _detalleService.AddAsync(model.Detalle);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Detalle insertado correctamente", AlertMessageType.Success);
                return RedirectToAction(nameof(FormPartialRecepcion), routeValues: new { id = model.Detalle.recep_Id });
            }
            else
            {
                Boolean updatedItem = await _detalleService.UpdateAsync(model.Detalle);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Detalle actualizado correctamente", AlertMessageType.Success);
                return RedirectToAction(nameof(FormPartialRecepcion), routeValues: new { id = model.Detalle.recep_Id });
            }

        ErrorResult:
            ViewBag.Refugios = _refugioService.RefugioDropdown();
            ViewBag.Procedencias = _procedenciaService.ProcedenciaDropdown();
            ViewBag.Items = _itemService.ItemDropdown();
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        /// <summary>
        /// Busca un detalle de recepción por su ID (para editar).
        /// </summary>
        /// <param name="id">ID del detalle.</param>
        /// <returns>JSON con el detalle encontrado.</returns>
        public async Task<IActionResult> FindDetalle(int id)
        {
            var itemSearched = await _detalleService.FindAsync(id);
            if (itemSearched != null)
            {
                return Json(new { data = itemSearched.Detalle, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Busca un detalle de recepción por su ID (para ver detalles con descripción del item).
        /// </summary>
        /// <param name="id">ID del detalle.</param>
        /// <returns>JSON con el detalle encontrado incluyendo descripción del item.</returns>
        public async Task<IActionResult> FindDetalleDetail(int id)
        {
            var itemSearched = await _detalleService.FindForDetailAsync(id);
            if (itemSearched != null)
            {
                return Json(new { data = itemSearched, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Elimina una recepción de mercancía.
        /// </summary>
        /// <param name="id">ID de la recepción.</param>
        /// <returns>Redireccionamiento según resultado.</returns>
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            Boolean deletedItem = await _recepcionService.RemoveAsync(id);
            if (!deletedItem)
            {
                ShowAlert("Recepción eliminada correctamente", AlertMessageType.Success);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Elimina un detalle de recepción.
        /// </summary>
        /// <param name="recdet_Id">ID del detalle.</param>
        /// <param name="recep_Id">ID de la recepción padre.</param>
        /// <returns>Redireccionamiento según resultado.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveDetalle(int recdet_Id, int recep_Id)
        {
            Boolean deletedItem = await _detalleService.RemoveAsync(recdet_Id);
            if (!deletedItem)
            {
                ShowAlert("Detalle eliminado correctamente", AlertMessageType.Success);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            }
            return RedirectToAction(nameof(FormPartialRecepcion), new { id = recep_Id });
        }
    }
}
