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
    [PantallaAuthorize("Listado de roles")]
    public class RolesController : BaseController
    {
        private readonly RolService _rolService;

        public RolesController(RolService rolService)
        {
            _rolService = rolService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            var result = await _rolService.ListAsync();
            return Json(new { data = result });
        }

        public async Task<IActionResult> Find(int id)
        {
            var result = await _rolService.FindAsync(id);
            return AjaxResult(result, result != null);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RolViewModel model)
        {
            var operacion = model.rol_Id == 0 ? "insertar" : "editar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Listado de roles", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId ?? 0;

            if (model.rol_Id == 0)
            {
                bool result = await _rolService.CreateAsync(model, userId);
                return AjaxResult(result);
            }
            else
            {
                bool result = await _rolService.EditAsync(model, userId);
                return AjaxResult(result);
            }
        }

        [HttpPost]
        [PantallaAuthorize("Listado de roles", "eliminar")]
        public async Task<IActionResult> Delete(int rol_Id)
        {
            bool result = await _rolService.DeleteAsync(rol_Id);
            return AjaxResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Exist(int? rol_Id, string rol_Descripcion)
        {
            if (string.IsNullOrWhiteSpace(rol_Descripcion))
                return Json("La descripcion es requerida");

            var result = await _rolService.ExistAsync(rol_Descripcion);
            if (result != null)
            {
                return (result.rol_Id == rol_Id) ? Json(true) : Json("El rol ya existe.");
            }
            return Json(true);
        }

        public async Task<IActionResult> PantallasList()
        {
            var result = await _rolService.PantallasListAsync();
            return Json(new { data = result });
        }

        public async Task<IActionResult> PantallasByRol(int id)
        {
            var result = await _rolService.PantallasByRolAsync(id);
            return Json(new { data = result });
        }

        [HttpPost]
        [PantallaAuthorize("Listado de roles", "editar")]
        public async Task<IActionResult> SavePantallas(int rolId, string permisosJson)
        {
            bool result = await _rolService.SavePantallasAsync(rolId, permisosJson ?? "[]");
            if (result)
            {
                return Json(new { success = true, message = "Pantallas asignadas exitosamente. Los cambios se veran reflejados en el proximo inicio de sesion." });
            }
            return Json(new { success = false, message = "Error al asignar pantallas." });
        }
    }
}
