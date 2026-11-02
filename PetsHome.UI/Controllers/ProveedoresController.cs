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
    [PantallaAuthorize("Listado de proveedores")]
    public class ProveedoresController : BaseController
    {
        private readonly ProveedorService _proveedorService;
        private readonly IMapper _mapper;

        public ProveedoresController(ProveedorService proveedorService, IMapper mapper)
        {
            _proveedorService = proveedorService;
            _mapper = mapper;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> List()
        {
            var items = await _proveedorService.ListAsync();
            if (items != null)
            {
                return Json(new { data = items });
            }
            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        [PantallaAuthorize("Listado de proveedores", "insertar")]
        public IActionResult Create() => View(new ProveedoresFormViewModel());

        [PantallaAuthorize("Listado de proveedores", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var item = await _proveedorService.FindAsync(id);
            if (item == null) return RedirectToAction("Index");
            return View("Create", _mapper.Map<ProveedoresFormViewModel>(item));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var item = await _proveedorService.DetailAsync(id);
            if (item == null) return RedirectToAction("Index");
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProveedoresFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de proveedores", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;
            bool ok = model.isEdit
                ? await _proveedorService.UpdateAsync(model, userId)
                : await _proveedorService.AddAsync(model, userId);

            if (ok)
            {
                ShowAlert(model.isEdit ? AlertMessaje.SuccessEdit : AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return View("Create", model);
        }

        [PantallaAuthorize("Listado de proveedores", "eliminar")]
        public async Task<IActionResult> Remove(int prov_Id)
        {
            bool ok = await _proveedorService.RemoveAsync(prov_Id);
            ShowAlert(ok ? "Eliminado" : AlertMessaje.Error, ok ? AlertMessageType.Success : AlertMessageType.Error);
            return RedirectToAction("Index");
        }
    }
}
