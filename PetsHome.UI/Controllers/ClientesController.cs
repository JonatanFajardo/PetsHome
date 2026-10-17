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
    [PantallaAuthorize("Listado de clientes")]
    public class ClientesController : BaseController
    {
        private readonly ClientesService _clientesService;
        private readonly IMapper _mapper;

        public ClientesController(ClientesService clientesService, IMapper mapper)
        {
            _clientesService = clientesService;
            _mapper = mapper;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> List()
        {
            var items = await _clientesService.ListAsync();
            if (items != null)
            {
                return Json(new { data = items });
            }
            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        [PantallaAuthorize("Listado de clientes", "insertar")]
        public IActionResult Create() => View(new ClientesFormViewModel());

        [PantallaAuthorize("Listado de clientes", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var item = await _clientesService.FindAsync(id);
            if (item == null) return RedirectToAction("Index");
            return View("Create", _mapper.Map<ClientesFormViewModel>(item));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var item = await _clientesService.DetailAsync(id);
            if (item == null) return RedirectToAction("Index");
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ClientesFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de clientes", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;
            bool ok = model.isEdit
                ? await _clientesService.UpdateAsync(model, userId)
                : await _clientesService.AddAsync(model, userId);

            if (ok)
            {
                ShowAlert(model.isEdit ? AlertMessaje.SuccessEdit : AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return View("Create", model);
        }

        [PantallaAuthorize("Listado de clientes", "eliminar")]
        public async Task<IActionResult> Remove(int cli_Id)
        {
            bool ok = await _clientesService.RemoveAsync(cli_Id);
            ShowAlert(ok ? "Eliminado" : AlertMessaje.Error, ok ? AlertMessageType.Success : AlertMessageType.Error);
            return RedirectToAction("Index");
        }
    }
}
