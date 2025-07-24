using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using PetsHome.Common.Entities;

namespace PetsHome.UI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // Si ya está autenticado, redirigir al dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                var result = await _authService.LoginAsync(model, ipAddress);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }

                var usuario = (UsuarioViewModel)result.Data;

                // Crear claims para la autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.usu_Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Usu_Nombre),
                    new Claim("FullName", usuario.Emp_NombreCompleto),
                    new Claim(ClaimTypes.Role, usuario.Rol_Descripcion),
                    new Claim("RoleId", usuario.Rol_Id.ToString()),
                    new Claim("EmployeeId", usuario.Emp_Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    RedirectUri = returnUrl
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                // Obtener permisos del usuario para la sesión
                var permissionsResult = await _authService.GetUsuarioPermissionsAsync(usuario.usu_Id);
                string modulosPermitidos = "";
                
                if (permissionsResult.Success)
                {
                    var modulos = (List<PR_Seguridad_Usuarios_GetPermissionsResult>)permissionsResult.Data;
                    modulosPermitidos = string.Join(",", modulos.Select(m => m.Mod_Nombre));
                }

                // Guardar información en sesión
                HttpContext.Session.SetString("modulos", modulosPermitidos);
                HttpContext.Session.SetString("usuario", usuario.Usu_Nombre);
                HttpContext.Session.SetString("nombreCompleto", usuario.Emp_NombreCompleto);

                // Log del evento de login
                EventLogger.Login($"Usuario {usuario.Usu_Nombre} - IP: {ipAddress}");

                ShowAlert("Login exitoso", AlertMessageType.Success);

                // Redirigir a la URL de retorno o al dashboard
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error interno del servidor");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            string usuario = HttpContext.Session.GetString("usuario") ?? "Desconocido";
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            // Log del evento de logout
            EventLogger.Logout($"Usuario {usuario} - IP: {ipAddress}");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            ShowAlert("Sesión cerrada correctamente", AlertMessageType.Info);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var result = await _authService.GetUsuarioDetailAsync(usuarioId);

            if (!result.Success)
            {
                ShowAlert("Error al cargar el perfil", AlertMessageType.Error);
                return RedirectToAction("Index", "Home");
            }

            var usuario = (UsuarioViewModel)result.Data;
            return View(usuario);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(CambiarContrasenaViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var result = await _authService.ChangePasswordAsync(usuarioId, model);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }

                ShowAlert("Contraseña cambiada exitosamente", AlertMessageType.Success);
                return RedirectToAction("Profile");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error interno del servidor");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            // Solo administradores pueden registrar usuarios
            if (!User.IsInRole("Administrador"))
            {
                return RedirectToAction("AccessDenied");
            }

            var rolesResult = await _authService.GetRolesAsync();
            ViewBag.Roles = rolesResult.Data;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroUsuarioViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            // Solo administradores pueden registrar usuarios
            if (!User.IsInRole("Administrador"))
            {
                return RedirectToAction("AccessDenied");
            }

            if (!ModelState.IsValid)
            {
                var rolesResult = await _authService.GetRolesAsync();
                ViewBag.Roles = rolesResult.Data;
                return View(model);
            }

            try
            {
                int usuarioCreacion = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

                var result = await _authService.CreateUsuarioAsync(model, usuarioCreacion, ipAddress);

                if (!result.Success)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    var rolesResult = await _authService.GetRolesAsync();
                    ViewBag.Roles = rolesResult.Data;
                    return View(model);
                }

                ShowAlert("Usuario registrado exitosamente", AlertMessageType.Success);
                return RedirectToAction("Index", "Home");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error interno del servidor");
                var rolesResult = await _authService.GetRolesAsync();
                ViewBag.Roles = rolesResult.Data;
                return View(model);
            }
        }
    }
}