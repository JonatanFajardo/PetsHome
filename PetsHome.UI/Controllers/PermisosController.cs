using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Attributes;
using System;
using System.Collections.Generic;
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

        #region Funcionalidades Extendidas

        #region Gestión de Componentes

        [Permission("USUARIOS", "READ")]
        public IActionResult Componentes()
        {
            return View();
        }

        [Permission("USUARIOS", "READ")]
        public async Task<IActionResult> ComponentesList()
        {
            var result = await _permisosService.GetComponentesAsync();
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
        public IActionResult CreateComponente()
        {
            var model = new ComponenteViewModel();
            return View(model);
        }

        [HttpPost]
        [Permission("USUARIOS", "CREATE")]
        public async Task<IActionResult> CreateComponente(ComponenteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _permisosService.CreateComponenteAsync(model);
                if (result.Success)
                {
                    ShowAlert(result.Message, AlertMessageType.Success);
                    return RedirectToAction("Componentes");
                }
                else
                {
                    ShowAlert(result.Message, AlertMessageType.Error);
                }
            }

            return View(model);
        }

        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> EditComponente(int id)
        {
            var componentesResult = await _permisosService.GetComponentesAsync();
            if (componentesResult.Success)
            {
                var componentes = (System.Collections.Generic.List<ComponenteViewModel>)componentesResult.Data;
                var componente = componentes.FirstOrDefault(c => c.comp_Id == id);
                if (componente != null)
                {
                    return View("CreateComponente", componente);
                }
            }

            ShowAlert("Componente no encontrado", AlertMessageType.Error);
            return RedirectToAction("Componentes");
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> UpdateComponente(ComponenteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _permisosService.UpdateComponenteAsync(model);
                if (result.Success)
                {
                    ShowAlert(result.Message, AlertMessageType.Success);
                    return RedirectToAction("Componentes");
                }
                else
                {
                    ShowAlert(result.Message, AlertMessageType.Error);
                }
            }

            return View("CreateComponente", model);
        }

        [HttpPost]
        [Permission("USUARIOS", "DELETE")]
        public async Task<IActionResult> DeleteComponente(int id)
        {
            try
            {
                var result = await _permisosService.DeleteComponenteAsync(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar componente: {ex.Message}" });
            }
        }

        #endregion

        #region Gestión de Pantallas

        [Permission("USUARIOS", "READ")]
        public IActionResult Pantallas(int? modId = null)
        {
            ViewBag.ModuloId = modId;
            return View();
        }

        [Permission("USUARIOS", "READ")]
        public async Task<IActionResult> PantallasList(int? modId = null)
        {
            var result = await _permisosService.GetModulosPantallasAsync(modId);
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
        public async Task<IActionResult> CreatePantalla(int? modId = null)
        {
            var model = new ModuloPantallaViewModel();
            if (modId.HasValue)
                model.mod_Id = modId.Value;

            // Cargar módulos para dropdown
            var modulosResult = await _permisosService.GetModulosAsync();
            if (modulosResult.Success)
            {
                ViewBag.Modulos = modulosResult.Data;
            }

            return View(model);
        }

        [HttpPost]
        [Permission("USUARIOS", "CREATE")]
        public async Task<IActionResult> CreatePantalla(ModuloPantallaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _permisosService.CreateModuloPantallaAsync(model);
                if (result.Success)
                {
                    ShowAlert(result.Message, AlertMessageType.Success);
                    return RedirectToAction("Pantallas", new { modId = model.mod_Id });
                }
                else
                {
                    ShowAlert(result.Message, AlertMessageType.Error);
                }
            }

            // Recargar módulos para dropdown
            var modulosResult = await _permisosService.GetModulosAsync();
            if (modulosResult.Success)
            {
                ViewBag.Modulos = modulosResult.Data;
            }

            return View(model);
        }

        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> EditPantalla(int id)
        {
            var pantallasResult = await _permisosService.GetModulosPantallasAsync();
            if (pantallasResult.Success)
            {
                var pantallas = (System.Collections.Generic.List<ModuloPantallaViewModel>)pantallasResult.Data;
                var pantalla = pantallas.FirstOrDefault(p => p.modpt_Id == id);
                if (pantalla != null)
                {
                    // Cargar módulos para dropdown
                    var modulosResult = await _permisosService.GetModulosAsync();
                    if (modulosResult.Success)
                    {
                        ViewBag.Modulos = modulosResult.Data;
                    }

                    return View("CreatePantalla", pantalla);
                }
            }

            ShowAlert("Pantalla no encontrada", AlertMessageType.Error);
            return RedirectToAction("Pantallas");
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> UpdatePantalla(ModuloPantallaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _permisosService.UpdateModuloPantallaAsync(model);
                if (result.Success)
                {
                    ShowAlert(result.Message, AlertMessageType.Success);
                    return RedirectToAction("Pantallas", new { modId = model.mod_Id });
                }
                else
                {
                    ShowAlert(result.Message, AlertMessageType.Error);
                }
            }

            // Recargar módulos para dropdown
            var modulosResult = await _permisosService.GetModulosAsync();
            if (modulosResult.Success)
            {
                ViewBag.Modulos = modulosResult.Data;
            }

            return View("CreatePantalla", model);
        }

        [HttpPost]
        [Permission("USUARIOS", "DELETE")]
        public async Task<IActionResult> DeletePantalla(int id)
        {
            try
            {
                var result = await _permisosService.DeleteModuloPantallaAsync(id);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al eliminar pantalla: {ex.Message}" });
            }
        }

        #endregion

        #region Gestión de Permisos por Pantallas

        [Permission("USUARIOS", "READ")]
        public async Task<IActionResult> GestionPermisosPantallas(int rolId = 1)
        {
            try
            {
                // Obtener roles para dropdown
                var rolesResult = await _authService.GetRolesAsync();
                if (rolesResult.Success)
                {
                    ViewBag.Roles = rolesResult.Data;
                }

                // Obtener asignaciones del rol
                var asignacionesResult = await _permisosService.GetRolModulosPantallasAsync(rolId);
                if (asignacionesResult.Success)
                {
                    ViewBag.RolId = rolId;
                    return View(asignacionesResult.Data);
                }
                else
                {
                    ShowAlert(asignacionesResult.Message, AlertMessageType.Error);
                    return View(new List<RolModuloPantallaViewModel>());
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Error al cargar permisos de pantallas: {ex.Message}", AlertMessageType.Error);
                return View(new List<RolModuloPantallaViewModel>());
            }
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> AsignarPantallaRol(int modptId, int rolId)
        {
            try
            {
                var result = await _permisosService.AsignarPantallaRolAsync(modptId, rolId);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al asignar pantalla: {ex.Message}" });
            }
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> RemoverPantallaRol(int modptId, int rolId)
        {
            try
            {
                var result = await _permisosService.RemoverPantallaRolAsync(modptId, rolId);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al remover pantalla: {ex.Message}" });
            }
        }

        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> AsignacionMasivaPantallas()
        {
            var model = new AsignacionMasivaViewModel();

            // Cargar roles para dropdown
            var rolesResult = await _authService.GetRolesAsync();
            if (rolesResult.Success)
            {
                model.RolesDropdown = rolesResult.Data;
            }

            // Cargar pantallas disponibles
            var pantallasResult = await _permisosService.GetModulosPantallasAsync();
            if (pantallasResult.Success)
            {
                var pantallas = (System.Collections.Generic.List<ModuloPantallaViewModel>)pantallasResult.Data;
                model.PantallasDisponibles = pantallas.Select(p => new PantallaSelectorViewModel
                {
                    modpt_Id = p.modpt_Id,
                    modpt_Descripcion = p.modpt_Descripcion,
                    Mod_Nombre = p.Mod_Nombre,
                    comp_Descripcion = p.comp_Descripcion,
                    Seleccionado = false
                }).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> ProcesarAsignacionMasiva(AsignacionMasivaViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _permisosService.AsignacionMasivaPantallasAsync(model);
                    if (result.Success)
                    {
                        ShowAlert(result.Message, AlertMessageType.Success);
                        return RedirectToAction("GestionPermisosPantallas", new { rolId = model.Rol_Id });
                    }
                    else
                    {
                        ShowAlert(result.Message, AlertMessageType.Error);
                    }
                }

                // Recargar datos para la vista
                var rolesResult = await _authService.GetRolesAsync();
                if (rolesResult.Success)
                {
                    model.RolesDropdown = rolesResult.Data;
                }

                var pantallasResult = await _permisosService.GetModulosPantallasAsync();
                if (pantallasResult.Success)
                {
                    var pantallas = (System.Collections.Generic.List<ModuloPantallaViewModel>)pantallasResult.Data;
                    model.PantallasDisponibles = pantallas.Select(p => new PantallaSelectorViewModel
                    {
                        modpt_Id = p.modpt_Id,
                        modpt_Descripcion = p.modpt_Descripcion,
                        Mod_Nombre = p.Mod_Nombre,
                        comp_Descripcion = p.comp_Descripcion,
                        Seleccionado = model.PantallasIds.Contains(p.modpt_Id)
                    }).ToList();
                }

                return View("AsignacionMasivaPantallas", model);
            }
            catch (Exception ex)
            {
                ShowAlert($"Error en asignación masiva: {ex.Message}", AlertMessageType.Error);
                return RedirectToAction("AsignacionMasivaPantallas");
            }
        }

        #endregion

        #region Gestión de Roles por Usuario

        [Permission("USUARIOS", "READ")]
        public async Task<IActionResult> RolesUsuarios(int? usuId = null)
        {
            try
            {
                var result = await _permisosService.GetRolesUsuariosAsync(usuId);
                if (result.Success)
                {
                    ViewBag.UsuarioId = usuId;
                    return View(result.Data);
                }
                else
                {
                    ShowAlert(result.Message, AlertMessageType.Error);
                    return View(new List<RolUsuarioViewModel>());
                }
            }
            catch (Exception ex)
            {
                ShowAlert($"Error al cargar roles de usuarios: {ex.Message}", AlertMessageType.Error);
                return View(new List<RolUsuarioViewModel>());
            }
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> AsignarRolUsuario(int rolId, int usuId)
        {
            try
            {
                var result = await _permisosService.AsignarRolUsuarioAsync(rolId, usuId);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al asignar rol: {ex.Message}" });
            }
        }

        [HttpPost]
        [Permission("USUARIOS", "UPDATE")]
        public async Task<IActionResult> RemoverRolUsuario(int rolId, int usuId)
        {
            try
            {
                var result = await _permisosService.RemoverRolUsuarioAsync(rolId, usuId);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al remover rol: {ex.Message}" });
            }
        }

        #endregion

        #region Menús Extendidos y APIs

        [HttpGet]
        public async Task<IActionResult> GetMenuExtendidoUsuario()
        {
            try
            {
                // Obtener el ID del usuario desde la sesión/claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int usuarioId))
                {
                    return Json(new { success = false, message = "Usuario no autenticado" });
                }

                var result = await _permisosService.GetMenuExtendidoUsuarioAsync(usuarioId);
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
                return Json(new { success = false, message = $"Error al obtener menú extendido: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerificarAccesoPantalla(int modptId)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int usuarioId))
                {
                    return Json(new { success = false, message = "Usuario no autenticado" });
                }

                var result = await _permisosService.VerificarAccesoPantallaAsync(usuarioId, modptId);
                if (result.Success)
                {
                    return Json(new { success = true, tieneAcceso = result.Data });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al verificar acceso: {ex.Message}" });
            }
        }

        #endregion

        #endregion
    }
}