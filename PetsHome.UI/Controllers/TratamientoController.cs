using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Data;
using PetsHome.Business.Extensions;
using PetsHome.Business.Helpers;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de tratamientos")]
    public class TratamientoController : BaseController
    {
        private readonly TratamientoService _tratamientoService;
        private readonly MascotaService _mascotaService;
        private readonly TipoParasitoService _tipoParasitoService;
        private readonly TipoMedicamentoService _tipoMedicamentoService;
        private readonly ViaAdministracionService _viaAdministracionService;

        public TratamientoController(
            TratamientoService tratamientoService,
            MascotaService mascotaService,
            TipoParasitoService tipoParasitoService,
            TipoMedicamentoService tipoMedicamentoService,
            ViaAdministracionService viaAdministracionService)
        {
            _tratamientoService = tratamientoService;
            _mascotaService = mascotaService;
            _tipoParasitoService = tipoParasitoService;
            _tipoMedicamentoService = tipoMedicamentoService;
            _viaAdministracionService = viaAdministracionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [PantallaAuthorize("Listado de tratamientos", "insertar")]
        public IActionResult Create()
        {
            var model = new TratamientoFormViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _tratamientoService.ListAsync();
            return Json(new { data = itemListing });
        }

        [PantallaAuthorize("Listado de tratamientos", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            if (id != 0)
            {
                var itemSearched = await _tratamientoService.FindAsync(id);
                var dropdown = Dropdown(itemSearched);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _tratamientoService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Tratamiento no encontrado", AlertMessageType.Error);
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

        public async Task<IActionResult> Add(TratamientoFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada, por favor inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de tratamientos", operacion))
                return RedirectToAction("AccessDenied", "Account");

            if (!model.isEdit)
            {
                bool createdItem = await _tratamientoService.AddAsync(model);
                bool validation = Validation.IsInsert(createdItem, ModelState.IsValid);
                if (!createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                bool updatedItem = await _tratamientoService.UpdateAsync(model);
                bool validation = Validation.IsUpdate(!updatedItem, ModelState.IsValid);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return View("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        [PantallaAuthorize("Listado de tratamientos", "eliminar")]
        public async Task<IActionResult> Remove(int trat_Id)
        {
            bool deletedItem = await _tratamientoService.RemoveAsync(trat_Id);
            if (deletedItem)
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

        /// <summary>
        /// Cargamos Dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TratamientoFormViewModel Dropdown(TratamientoFormViewModel model)
        {
            model.LoadDropDownList(
                _mascotaService.MascotaDropdown(),
                new System.Collections.Generic.List<object>(), // CitaMedica - puede implementarse después
                new System.Collections.Generic.List<object>(), // Receta - puede implementarse después
                _tipoParasitoService.TipoParasitoDropdown(),
                _tipoMedicamentoService.TipoMedicamentoDropdown(),
                _viaAdministracionService.ViaAdministracionDropdown()
            );
            return model;
        }
    }
}
