using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de autenticación de usuarios
    /// </summary>
    public class AccountController : BaseController
    {
        private readonly UsuarioService _usuarioService;
        private readonly RolService _rolService;

        public AccountController(UsuarioService usuarioService, RolService rolService)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
        }

        #region Login

        /// <summary>
        /// Muestra la página de login
        /// </summary>
        /// <param name="returnUrl">URL de retorno después del login</param>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            // Si el usuario ya está autenticado, redirigir al home
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Procesa el formulario de login
        /// </summary>
        /// <param name="model">Modelo de login con credenciales</param>
        /// <param name="returnUrl">URL de retorno</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            try
            {
                // Autenticar usuario
                System.Diagnostics.Debug.WriteLine("=== LOGIN ATTEMPT: " + model.Username + " ===");
                var usuario = await _usuarioService.AuthenticateAsync(model.Username, model.Password);
                System.Diagnostics.Debug.WriteLine("=== LOGIN RESULT: " + (usuario != null ? "SUCCESS - " + usuario.usu_NombreCompleto : "NULL") + " ===");

                if (usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos");
                    ShowAlert("Usuario o contraseña incorrectos", AlertMessageType.Error);
                    return View(model);
                }

                // Crear claims del usuario
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.usu_Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.usu_NombreUsuario),
                    new Claim(ClaimTypes.GivenName, usuario.usu_NombreCompleto),
                    new Claim("EmployeeId", usuario.Emp_Id.ToString()),
                };

                // Agregar rol si existe
                if (usuario.rol_Id.HasValue)
                {
                    claims.Add(new Claim(ClaimTypes.Role, usuario.rol_Descripcion ?? "Usuario"));
                    claims.Add(new Claim("RoleId", usuario.rol_Id.Value.ToString()));
                }

                // Cargar pantallas del rol como claim
                if (usuario.rol_Id.HasValue)
                {
                    var pantallas = await _rolService.GetPantallasStringByRolAsync(usuario.rol_Id.Value);
                    claims.Add(new Claim("Pantallas", pantallas ?? ""));

                    var permisosJson = await _rolService.GetPermisosJsonByRolAsync(usuario.rol_Id.Value);
                    claims.Add(new Claim("PermisosCRUD", permisosJson ?? "[]"));
                }

                // Agregar imagen de perfil si existe
                if (!string.IsNullOrEmpty(usuario.usu_ImagenPerfil))
                {
                    claims.Add(new Claim("ProfileImage", usuario.usu_ImagenPerfil));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    // Sin marcar "Recordarme": 8 horas (jornada laboral)
                    // Con "Recordarme": 7 días
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
                };

                // Iniciar sesión
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                ShowAlert($"Bienvenido, {usuario.usu_NombreCompleto}!", AlertMessageType.Success);

                // Redirigir a la URL de retorno o al home
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("=== LOGIN EXCEPTION: " + ex.ToString() + " ===");
                ModelState.AddModelError(string.Empty, "Error al iniciar sesión. Intente nuevamente.");
                ShowAlert("Error al iniciar sesión. Intente nuevamente.", AlertMessageType.Error);
                // Log del error aquí
                return View(model);
            }
        }

        #endregion

        #region Logout

        /// <summary>
        /// Cierra la sesión del usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Obtener el ID del usuario
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Marcar como deslogueado en la base de datos
                    await _usuarioService.LogoutAsync(userId);
                }

                // Cerrar sesión en ASP.NET Core
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                ShowAlert("Sesión cerrada exitosamente", AlertMessageType.Success);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ShowAlert("Error al cerrar sesión", AlertMessageType.Error);
                // Log del error aquí
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion

        #region Acceso Denegado

        /// <summary>
        /// Página de acceso denegado
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion
    }
}
