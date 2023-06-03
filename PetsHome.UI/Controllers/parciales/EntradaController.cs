using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class EntradaController : BaseController
    {
        private readonly EntradasDetalleService _entradaDetalleService;
        private readonly EntradaService _entradaService;
        private readonly RefugioService _refugioService;
        private readonly ItemService _itemService;

        public EntradaController(EntradasDetalleService entradaDetalleService,
                                    EntradaService entradaService,
                                    RefugioService refugioService,
                                    ItemService itemService)
        {
            _entradaDetalleService = entradaDetalleService;
            _entradaService = entradaService;
            _refugioService = refugioService;
            _itemService = itemService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListEntradasDetalles(int id)
        {
            var listEntradasDetalles = await _entradaDetalleService.ListIdAsync(id);
            return Json(new
            {
                data = listEntradasDetalles
            });
        }

        // Redirecciona a la vista parcial de departamentos
        public async Task<IActionResult> EditEntradas(int id)
        {
            if (id == 0)
            {
                var model = new EntradaViewModel();
                //Asignamos el valor de 0 para registrar un nuevo departamento
                model.ent_Id = id;
                var drop = Dropdown(model);
                return View(nameof(EditEntradas), drop);
            }
            else
            {
                var result = await _entradaService.FindAsync(id);
                result.ListadoEntradasDetalles = await _entradaDetalleService.ListIdAsync(id);
                var drop = Dropdown(result);
                var drop2 = ItemDropdown(drop);
                return View(nameof(EditEntradas), drop2);
            }
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _entradaService.ListAsync();
            return Json(new { data = itemListing });
        }

        /// <summary>
        /// Método que crea o edita un nuevo departamento
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> Add(EntradaViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _entradaService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("EditEntradas");
            }
            else
            {
                Boolean updatedItem = await _entradaService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("EditEntradas", routeValues: new { id = model.ent_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        /// <summary>
        /// Añade un nuevo o edita un municipio
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddEntradaDetalle(EntradaViewModel model)
        {
            if (!model.EntradaDetalle.isEdit)
            {
                Boolean createdItem = await _entradaDetalleService.AddAsync(model.EntradaDetalle);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("EditEntradas", routeValues: new { id = model.EntradaDetalle.ent_Id });
            }
            else
            {
                Boolean updatedItem = await _entradaDetalleService.UpdateAsync(model.EntradaDetalle);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("EditEntradas", routeValues: new { id = model.EntradaDetalle.ent_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> FindEntradaDetalle(int id)
        {
            var itemSearched = await _entradaDetalleService.FindAsync(id);
            var drop = ItemDropdown(itemSearched);
            if (itemSearched != null)
            {
                return Json(new { item = drop, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public EntradaViewModel Dropdown(EntradaViewModel model)
        {
            model.LoadDropDownList(_refugioService.RefugioDropdown());
            return model;
        }

        public EntradaViewModel ItemDropdown(EntradaViewModel model)
        {
            model.EntradaDetalle.LoadDropDownList(_itemService.ItemDropdown());
            return model;
        }


    }
}