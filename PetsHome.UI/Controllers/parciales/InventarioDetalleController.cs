using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class InventarioDetalleController : BaseController
    {
        private readonly InventariosDetalleService _inventariosDetalleService;
        private readonly IMapper _mapper;

        public InventarioDetalleController(InventariosDetalleService inventariosDetalleService,
                                          IMapper mapper)
        {
            _inventariosDetalleService = inventariosDetalleService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var resultado = await _inventariosDetalleService.DetailAsync(id);
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
                var model = new InventarioDetalleViewModel();
                model.invdet_Id = id;
                return View(nameof(Create), model);
            }
            else
            {
                var result = await _inventariosDetalleService.FindAsync(id);
                return View(nameof(Create), result);
            }
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _inventariosDetalleService.ListAsync();
            return Json(new { data = itemListing });
        }

        public async Task<IActionResult> Add(InventarioDetalleViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _inventariosDetalleService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _inventariosDetalleService.UpdateAsync(model);
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
            var itemSearched = await _inventariosDetalleService.FindAsync(id);
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

        public async Task<IActionResult> Remove(InventarioDetalleViewModel model)
        {
            Boolean removedItem = await _inventariosDetalleService.RemoveAsync(model.invdet_Id);
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