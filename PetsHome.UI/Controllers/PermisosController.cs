using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Permission("USUARIOS", "READ")]
    public class PermisosController : BaseController
    {
        private readonly PermisosService _permisosService;
        private readonly AuthService _authService;

        public PermisosController(PermisosService permisosService, AuthService authService)
        {
            _permisosService = permisosService;
            _authService = authService;
        }

        #region Gestión de Módulos

        public IActionResult Modulos()
        {
            return View();
        }

        [Permission("USUARIOS", "READ")]
        public async Task<IActionResult> ModulosList()
        {
            var result = await _permisosService.GetModulosAsync();
            if (result.Success)
            {
                return Json(new { data = result.Data });
            }
            else
            {
                return Json(new { data = new object[] { } });
            }
        }

        [Permission("USUARIOS", "CREATE")]
        public IActionResult CreateModulo()
        {
            var model = new ModuloViewModel();
            return View(model);
        }

        [HttpPost]
        [Permission("USUARIOS", "CREATE")]
        public async Task<IActionResult> CreateModulo(ModuloViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _permisosService.CreateModuloAsync(model);
                if (result.Success)
                {
                    ShowAlert(result.Message, AlertMessageType.Success);
                    return RedirectToAction("Modulos");
                }
                else
                {
                    ShowAlert(result.Message, AlertMessageType.Error);
                }
            }

            return View(model);
        }

        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> EditModulo(int id)
        {
            var modulosResult = await _permisosService.GetModulosAsync();
            if (modulosResult.Success)
            {
                var modulos = (System.Collections.Generic.List<ModuloViewModel>)modulosResult.Data;
                var modulo = modulos.FirstOrDefault(m => m.Mod_Id == id);
                if (modulo != null)
                {
                    return View("CreateModulo", modulo);
                }
            }

            ShowAlert("Módulo no encontrado", AlertMessageType.Error);
            return RedirectToAction("Modulos");
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> UpdateModulo(ModuloViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _permisosService.UpdateModuloAsync(model);
                if (result.Success)
                {
                    ShowAlert(result.Message, AlertMessageType.Success);
                    return RedirectToAction("Modulos");
                }
                else
                {
                    ShowAlert(result.Message, AlertMessageType.Error);
                }
            }

            return View("CreateModulo", model);
        }

        #endregion

        #region Gestión de Permisos por Rol

        public async Task<IActionResult> GestionPermisos(int rolId = 1)
        {
            try
            {
                var result = await _permisosService.GetGestionPermisosAsync(rolId);
                if (result.Success)
                {
                    return View(result.Data);
                }
                else
                {
                    ShowAlert(result.Message, AlertMessageType.Error);
                    return View(new GestionPermisosViewModel());
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Error al cargar permisos: {ex.Message}", AlertMessageType.Error);
                return View(new GestionPermisosViewModel());
            }
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> GuardarPermisos(GestionPermisosViewModel model)
        {
            try
            {
                var result = await _permisosService.GuardarPermisosRolAsync(model.Rol_Id, model.Modulos);
                if (result.Success)
                {
                    return Json(new { success = true, message = result.Message });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al guardar permisos: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CargarPermisosRol(int rolId)
        {
            try
            {
                var result = await _permisosService.GetGestionPermisosAsync(rolId);
                if (result.Success)
                {
                    return Json(new { success = true, data = result.Data });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al cargar permisos: {ex.Message}" });
            }
        }

        #endregion

        #region API para Menús Dinámicos

        [HttpGet]
        public async Task<IActionResult> GetMenuUsuario()
        {
            try
            {
                // Obtener el ID del usuario desde la sesión/claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int usuarioId))
                {
                    return Json(new { success = false, message = "Usuario no autenticado" });
                }

                var result = await _permisosService.GetMenuUsuarioAsync(usuarioId);
                if (result.Success)
                {
                    return Json(new { success = true, data = result.Data });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al obtener menú: {ex.Message}" });
            }
        }

        #endregion
    }
}