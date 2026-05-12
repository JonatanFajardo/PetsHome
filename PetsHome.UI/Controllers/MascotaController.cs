using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using Microsoft.Extensions.Options;
using PetsHome.Business.Data;
using PetsHome.Business.Extensions;
using PetsHome.Business.Helpers;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de mascotas")]
    public class MascotaController : BaseController
    {
        private readonly MascotaService _mascotaService;
        private readonly RefugioService _refugioService;
        private readonly IOptions<MascotaFormViewModel> _pathFile;

        public MascotaController(MascotaService mascotaService, RefugioService refugioService, IOptions<MascotaFormViewModel> options)
        {
            _mascotaService = mascotaService;
            _refugioService = refugioService;
            _pathFile = options;
        }

        [Breadcrumb("Mascota", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index()
        {
            return View();
        }

        [Breadcrumb("Crear", FromAction = "Index", FromController = typeof(MascotaController))]
        [PantallaAuthorize("Listado de mascotas", "insertar")]
        public IActionResult Create()
        {
            var model = new MascotaFormViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        [Breadcrumb("Lista", FromAction = "Index", FromController = typeof(MascotaController))]
        public async Task<IActionResult> List()
        {
            var itemListing = await _mascotaService.ListAsync();
            return Json(new { data = itemListing });
        }

        [Breadcrumb("Buscar", FromAction = "Index", FromController = typeof(MascotaController))]
        [PantallaAuthorize("Listado de mascotas", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            if (id != 0)
            {
                var itemSearched = await _mascotaService.FindAsync(id);
                var dropdown = Dropdown(itemSearched);
                string imgBase64Data = itemSearched.masc_Imagen.GetImage();
                ViewBag.ImageFile = string.Format("data:image/png;base64,{0}", imgBase64Data);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        [Breadcrumb("Detalle", FromAction = "Index", FromController = typeof(MascotaController))]
        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _mascotaService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Mascota no encontrada", AlertMessageType.Error);
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

        [Breadcrumb("Agregar", FromAction = "Index", FromController = typeof(MascotaController))]
        public async Task<IActionResult> Add(MascotaFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesi�n expirada. Por favor, inicie sesi�n nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de mascotas", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                bool createdItem = await _mascotaService.AddAsync(model, userId);
                bool validation = Validation.IsInsert(createdItem, ModelState.IsValid);
                if (!createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                bool updatedItem = await _mascotaService.UpdateAsync(model, userId);
                bool validation = Validation.IsUpdate(!updatedItem, ModelState.IsValid);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        [Breadcrumb("Eliminar", FromAction = "Index", FromController = typeof(MascotaController))]
        [PantallaAuthorize("Listado de mascotas", "eliminar")]
        public async Task<IActionResult> Remove(int masc_Id)
        {
            bool deletedItem = await _mascotaService.RemoveAsync(masc_Id);
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

        /// <summary>
        /// Cargamos Dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MascotaFormViewModel Dropdown(MascotaFormViewModel model)
        {
            model.LoadDropDownList(_mascotaService.RazaDropdown(), Dropdownlist.LoadSexo(), _refugioService.RefugioDropdown(), _mascotaService.ProcedenciaDropdown(), _mascotaService.TallaDropdown());
            return model;
        }
    }
}
