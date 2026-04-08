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
    [PantallaAuthorize("Listado de usuarios")]
    public class UsuariosController : BaseController
    {
        private readonly UsuarioService _usuarioService;
        private readonly RolService _rolService;

        public UsuariosController(UsuarioService usuarioService, RolService rolService)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            var result = await _usuarioService.ListAsync();
            return Json(new { data = result });
        }

        public async Task<IActionResult> Find(int id)
        {
            var result = await _usuarioService.FindAsync(id);
            return AjaxResult(result, result != null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsuarioCrudViewModel model)
        {
            var operacion = model.usu_Id == 0 ? "insertar" : "editar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de usuarios", operacion))
                return RedirectToAction("AccessDenied", "Account");

            if (model.usu_Id == 0)
            {
                bool result = await _usuarioService.CreateAsync(model);
                return AjaxResult(result);
            }
            else
            {
                bool result = await _usuarioService.EditAsync(model);
                return AjaxResult(result);
            }
        }

        [HttpPost]
        [PantallaAuthorize("Listado de usuarios", "eliminar")]
        public async Task<IActionResult> Delete(int usu_Id)
        {
            bool result = await _usuarioService.DeleteAsync(usu_Id);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Exist(int? usu_Id, string Usu_Nombre)
        {
            if (string.IsNullOrWhiteSpace(Usu_Nombre))
                return Json("El nombre de usuario es requerido");

            var result = await _usuarioService.ExistAsync(Usu_Nombre);
            if (result != null)
            {
                return (result.usu_Id == usu_Id) ? Json(true) : Json("El nombre de usuario ya existe.");
            }
            return Json(true);
        }

        public async Task<IActionResult> RolesDropdown()
        {
            var result = await _rolService.DropdownAsync();
            return Json(new { data = result });
        }
    }
}
