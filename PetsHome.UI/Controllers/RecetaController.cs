using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de recetas")]
    public class RecetaController : BaseController
    {
        private readonly RecetaService _recetaService;
        private readonly CitaMedicaService _citaMedicaService;
        private readonly TipoMedicamentoService _tipoMedicamentoService;
        private readonly ViaAdministracionService _viaAdministracionService;

        public RecetaController(
            RecetaService recetaService,
            CitaMedicaService citaMedicaService,
            TipoMedicamentoService tipoMedicamentoService,
            ViaAdministracionService viaAdministracionService)
        {
            _recetaService = recetaService;
            _citaMedicaService = citaMedicaService;
            _tipoMedicamentoService = tipoMedicamentoService;
            _viaAdministracionService = viaAdministracionService;
        }

        [Breadcrumb("Receta", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index()
        {
            return View();
        }

        [Breadcrumb("Crear", FromAction = "Index", FromController = typeof(RecetaController))]
        [PantallaAuthorize("Listado de recetas", "insertar")]
        public async Task<IActionResult> Create()
        {
            var model = new RecetaFormViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        [Breadcrumb("Lista", FromAction = "Index", FromController = typeof(RecetaController))]
        public async Task<IActionResult> List()
        {
            var itemListing = await _recetaService.ListAsync();
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

        [Breadcrumb("Buscar", FromAction = "Index", FromController = typeof(RecetaController))]
        [PantallaAuthorize("Listado de recetas", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _recetaService.FindAsync(id);
            if (itemSearched != null)
            {
                var dropdown = Dropdown(itemSearched);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        [Breadcrumb("Detalle", FromAction = "Index", FromController = typeof(RecetaController))]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _recetaService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Receta no encontrada", AlertMessageType.Error);
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

        [Breadcrumb("Agregar", FromAction = "Index", FromController = typeof(RecetaController))]
        public async Task<IActionResult> Add(RecetaFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada, por favor inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de recetas", operacion))
                return RedirectToAction("AccessDenied", "Account");

            if (!model.isEdit)
            {
                Boolean createdItem = await _recetaService.AddAsync(model);
                if (!createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _recetaService.UpdateAsync(model);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        [Breadcrumb("Actualizar", FromAction = "Index", FromController = typeof(RecetaController))]
        [PantallaAuthorize("Listado de recetas", "editar")]
        public async Task<IActionResult> Update(RecetaFormViewModel model)
        {
            Boolean updatedItem = await _recetaService.UpdateAsync(model);
            if (updatedItem)
            {
                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                var dropdown = Dropdown(model);
                return View("Create", dropdown);
            }
        }

        [Breadcrumb("Eliminar", FromAction = "Index", FromController = typeof(RecetaController))]
        [PantallaAuthorize("Listado de recetas", "eliminar")]
        public async Task<IActionResult> Remove(int receta_Id)
        {
            Boolean deletedItem = await _recetaService.RemoveAsync(receta_Id);
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

        public RecetaFormViewModel Dropdown(RecetaFormViewModel model)
        {
            var mascotas = _citaMedicaService.MascotaDropdown();
            var citas = _citaMedicaService.CitaMedicaDropdown();
            var tiposMedicamento = _tipoMedicamentoService.TipoMedicamentoDropdown();
            var viasAdministracion = _viaAdministracionService.ViaAdministracionDropdown();

            model.LoadDropDownList(
                mascotas,
                citas,
                tiposMedicamento,
                viasAdministracion
            );
            return model;
        }
    }
}
