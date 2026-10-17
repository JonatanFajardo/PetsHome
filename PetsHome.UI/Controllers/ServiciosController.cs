using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de servicios")]
    public class ServiciosController : BaseController
    {
        private readonly ServiciosService _serviciosService;
        private readonly IMapper _mapper;

        public ServiciosController(ServiciosService serviciosService, IMapper mapper)
        {
            _serviciosService = serviciosService;
            _mapper = mapper;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> List()
        {
            var items = await _serviciosService.ListAsync();
            if (items != null)
            {
                return Json(new { data = items });
            }
            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        [PantallaAuthorize("Listado de servicios", "insertar")]
        public IActionResult Create() => View(new ServiciosFormViewModel());

        [PantallaAuthorize("Listado de servicios", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var item = await _serviciosService.FindAsync(id);
            if (item == null) return RedirectToAction("Index");
            return View("Create", _mapper.Map<ServiciosFormViewModel>(item));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var item = await _serviciosService.DetailAsync(id);
            if (item == null) return RedirectToAction("Index");
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ServiciosFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de servicios", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;
            bool ok = model.isEdit
                ? await _serviciosService.UpdateAsync(model, userId)
                : await _serviciosService.AddAsync(model, userId);

            if (ok)
            {
                ShowAlert(model.isEdit ? AlertMessaje.SuccessEdit : AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return View("Create", model);
        }

        [PantallaAuthorize("Listado de servicios", "eliminar")]
        public async Task<IActionResult> Remove(int serv_Id)
        {
            bool ok = await _serviciosService.RemoveAsync(serv_Id);
            ShowAlert(ok ? "Eliminado" : AlertMessaje.Error, ok ? AlertMessageType.Success : AlertMessageType.Error);
            return RedirectToAction("Index");
        }
    }
}
