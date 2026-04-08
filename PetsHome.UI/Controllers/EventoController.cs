using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de eventos")]
    public class EventoController : BaseController
    {
        private readonly EventoService _EventoService;
        private readonly RefugioService _RefugioService;
        public EventoController(EventoService eventoService, RefugioService refugioService)
        {
            _EventoService = eventoService;
            _RefugioService = refugioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [PantallaAuthorize("Listado de eventos", "insertar")]
        public IActionResult Create()
        {
            var model = new EventoViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _EventoService.ListAsync();
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

        [PantallaAuthorize("Listado de eventos", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _EventoService.FindAsync(id);
            if (itemSearched != null)
            {
                var dropdown = Dropdown(itemSearched);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(itemSearched, false);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _EventoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Evento no encontrado", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("Details", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }


        public async Task<IActionResult> Add(EventoViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de eventos", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _EventoService.AddAsync(model, userId);
                if (!createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _EventoService.UpdateAsync(model, userId);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        [PantallaAuthorize("Listado de eventos", "eliminar")]
        public async Task<IActionResult> Remove(int eve_Id)
        {
            Boolean deletedItem = await _EventoService.RemoveAsync(eve_Id);
            if (!deletedItem)
            {
                ShowAlert("Eliminado", AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public EventoViewModel Dropdown(EventoViewModel model)
        {
            model.LoadDropDownList(_RefugioService.RefugioDropdown());
            return model;
        }
    }
}