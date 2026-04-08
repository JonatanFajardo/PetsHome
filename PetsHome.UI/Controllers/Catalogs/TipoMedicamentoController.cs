using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using PetsHome.UI.Models;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de tipos de medicamento")]
    public class TipoMedicamentoController : BaseController
    {
        private readonly TipoMedicamentoService _tipoMedicamentoService;

        public TipoMedicamentoController(TipoMedicamentoService tipoMedicamentoService)
        {
            _tipoMedicamentoService = tipoMedicamentoService;
        }

        public IActionResult Index()
        {
            return View("~/Views/Catalogo/TipoMedicamento/Index.cshtml");
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _tipoMedicamentoService.ListAsync();
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

        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _tipoMedicamentoService.FindAsync(id);
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

        [ActionName("Details")]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _tipoMedicamentoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Tipo de medicamento no encontrado", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("~/Views/Catalogo/TipoMedicamento/Details.cshtml", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(TipoMedicamentoViewModel model)
        {
            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de tipos de medicamento", operacion))
                return RedirectToAction("AccessDenied", "Account");

            if (!model.isEdit)
            {
                Boolean createdItem = await _tipoMedicamentoService.AddAsync(model);
                if (!createdItem)
                {
                    ShowAlert("Insertado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Boolean updatedItem = await _tipoMedicamentoService.UpdateAsync(model);
                if (!updatedItem)
                {
                    ShowAlert("Modificado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
        }

        [PantallaAuthorize("Listado de tipos de medicamento", "eliminar")]
        public async Task<IActionResult> Remove(int tipoMed_Id)
        {
            Boolean deletedItem = await _tipoMedicamentoService.RemoveAsync(tipoMed_Id);
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
    }
}
