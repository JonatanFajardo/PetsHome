using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class InventarioController : BaseController
    {
        private readonly InventarioService _inventarioService;
        private readonly RefugioService _refugioService;
        private readonly IMapper _mapper;

        public InventarioController(InventarioService inventarioService,
                                   RefugioService refugioService,
                                   IMapper mapper)
        {
            _inventarioService = inventarioService;
            _refugioService = refugioService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var resultado = await _inventarioService.DetailAsync(id);
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
                var model = new InventarioViewModel();
                model.inv_Id = id;
                return View(nameof(Create), model);
            }
            else
            {
                var result = await _inventarioService.FindAsync(id);
                return View(nameof(Create), result);
            }
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _inventarioService.ListAsync();
            return Json(new { data = itemListing });
        }

        public async Task<IActionResult> Add(InventarioViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _inventarioService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _inventarioService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _inventarioService.FindAsync(id);
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

        public async Task<IActionResult> Remove(InventarioViewModel model)
        {
            Boolean removedItem = await _inventarioService.RemoveAsync(model.inv_Id);
            if (removedItem)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }

            ShowAlert("Eliminado", AlertMessageType.Success);
            return RedirectToAction("Index");
        }
    }
}