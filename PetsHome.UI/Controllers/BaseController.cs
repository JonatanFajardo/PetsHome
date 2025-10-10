using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using System;
using System.Security.Claims;

namespace PetsHome.UI.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Muestra alertas en el navegador
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        protected void ShowAlert(string text, AlertMessageType type)
        {
            var message = new AlertMessageExtensions
            {
                Text = text,
                Type = type
            };
            TempData.Put("ShowAlert", message);
        }

        protected IActionResult ShowAlert(string text, AlertMessageType type, dynamic model)
        {
            var message = new AlertMessageExtensions
            {
                Text = text,
                Type = type
            };
            TempData.Put("ShowAlert", message);
            return View("Create", model);
        }

        public JsonResult AjaxResult(dynamic model, Boolean success)
        {
            return Json(new { item = model, success = success });
        }

        public JsonResult AjaxResult(Boolean success)
        {
            return Json(new { success = success });
        }

        #region Métodos de utilidad para compatibilidad con sistema AHM

        /// <summary>
        /// Obtiene el ID del usuario actual desde la sesión
        /// </summary>
        protected int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("idUsuario") ?? 
                   (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : (int?)null);
        }

        /// <summary>
        /// Obtiene el nombre del usuario actual desde la sesión
        /// </summary>
        protected string GetCurrentUserName()
        {
            return HttpContext.Session.GetString("usu_NombreUsuario") ?? 
                   User.Identity.Name ?? "Desconocido";
        }

        /// <summary>
        /// Obtiene el ID del rol actual desde la sesión
        /// </summary>
        protected int? GetCurrentRoleId()
        {
            return HttpContext.Session.GetInt32("idrol") ?? 
                   (int.TryParse(User.FindFirst("RoleId")?.Value, out int roleId) ? roleId : (int?)null);
        }

        /// <summary>
        /// Verifica si el usuario tiene acceso a una pantalla específica
        /// </summary>
        protected bool HasAccessToPantalla(string pantallaNombre)
        {
            if (string.IsNullOrEmpty(pantallaNombre))
                return true;

            string pantallas = HttpContext.Session.GetString("pantallas") ?? string.Empty;
            return pantallas.Contains(pantallaNombre);
        }

        /// <summary>
        /// Verifica si el usuario tiene acceso a un módulo específico
        /// </summary>
        protected bool HasAccessToModulo(string moduloNombre)
        {
            if (string.IsNullOrEmpty(moduloNombre))
                return true;

            string modulos = HttpContext.Session.GetString("modulos") ?? string.Empty;
            return modulos.Contains(moduloNombre);
        }

        /// <summary>
        /// Retorna JSON con resultado estándar para Ajax
        /// </summary>
        protected JsonResult JsonSuccess(string message = "Operación exitosa", object data = null)
        {
            return Json(new { success = true, message = message, data = data });
        }

        /// <summary>
        /// Retorna JSON con error estándar para Ajax
        /// </summary>
        protected JsonResult JsonError(string message = "Error en la operación", object data = null)
        {
            return Json(new { success = false, message = message, data = data });
        }

        /// <summary>
        /// Redirige a la pantalla de sin acceso
        /// </summary>
        protected IActionResult RedirectToSinAcceso()
        {
            return RedirectToAction("SinAcceso", "Account");
        }

        /// <summary>
        /// Redirige al login si no está autenticado
        /// </summary>
        protected IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        #endregion
    }
}