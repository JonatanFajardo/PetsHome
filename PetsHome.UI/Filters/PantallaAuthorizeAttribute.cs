using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace PetsHome.UI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PantallaAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _pantallaNombre;
        private readonly string _operacion;

        public PantallaAuthorizeAttribute() { }

        public PantallaAuthorizeAttribute(string pantallaNombre)
        {
            _pantallaNombre = pantallaNombre;
        }

        public PantallaAuthorizeAttribute(string pantallaNombre, string operacion)
        {
            _pantallaNombre = pantallaNombre;
            _operacion = operacion;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (!string.IsNullOrEmpty(_pantallaNombre)
                && _pantallaNombre != "Home"
                && !TienePantalla(user, _pantallaNombre))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            if (!string.IsNullOrEmpty(_operacion)
                && !string.IsNullOrEmpty(_pantallaNombre)
                && !TienePermiso(user, _pantallaNombre, _operacion))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            // Pasar el nombre de la pantalla al ViewData para que el JS lo use
            if (!string.IsNullOrEmpty(_pantallaNombre) && context.Controller is Controller controller)
            {
                controller.ViewData["CurrentPantalla"] = _pantallaNombre;
            }

            base.OnActionExecuting(context);
        }

        public static bool TienePantalla(ClaimsPrincipal user, string pantallaNombre)
        {
            if (user == null || string.IsNullOrEmpty(pantallaNombre))
                return false;

            var pantallasClaim = user.FindFirst("Pantallas")?.Value;
            if (string.IsNullOrEmpty(pantallasClaim))
                return false;

            var lista = pantallasClaim.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return lista.Any(p => p.Trim().Equals(pantallaNombre, StringComparison.OrdinalIgnoreCase));
        }

        public static bool TienePermiso(ClaimsPrincipal user, string pantallaNombre, string operacion)
        {
            if (user == null || string.IsNullOrEmpty(pantallaNombre) || string.IsNullOrEmpty(operacion))
                return false;

            var permisosClaim = user.FindFirst("PermisosCRUD")?.Value;
            if (string.IsNullOrEmpty(permisosClaim))
                return false;

            try
            {
                using var doc = JsonDocument.Parse(permisosClaim);
                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var nombre = element.GetProperty("pantalla").GetString();
                    if (nombre != null && nombre.Equals(pantallaNombre, StringComparison.OrdinalIgnoreCase))
                    {
                        return operacion.ToLower() switch
                        {
                            "consultar" => element.GetProperty("consultar").GetBoolean(),
                            "insertar" => element.GetProperty("insertar").GetBoolean(),
                            "editar" => element.GetProperty("editar").GetBoolean(),
                            "eliminar" => element.GetProperty("eliminar").GetBoolean(),
                            _ => false
                        };
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
