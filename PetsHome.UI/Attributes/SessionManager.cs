using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
namespace PetsHome.UI.Attributes
{
    public class SessionManager : ActionFilterAttribute
    {
        private readonly string _pantallaNombre;
        public SessionManager() { }
        public SessionManager(string pantallaNombre)
        {
            _pantallaNombre = pantallaNombre;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            RouteValueDictionary sinAcceso = new RouteValueDictionary(new { action = "AccessDenied", controller = "Account" });
            RouteValueDictionary sesionExpirada = new RouteValueDictionary(new { action = "Login", controller = "Account" });

            // Verificar si el usuario está autenticado
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(sesionExpirada);
                return;
            }

            // Si no se especifica módulo, permitir acceso (para Home, etc.)
            if (string.IsNullOrEmpty(_pantallaNombre) || _pantallaNombre == "Home")
            {
                return;
            }

            string modulos = string.Empty;
            var session = context.HttpContext.Session.GetString("modulos");
            if (string.IsNullOrEmpty(session))
            {
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
            else
            {
                modulos = session;
                if (!modulos.Contains(_pantallaNombre))
                    context.Result = new RedirectToRouteResult(sinAcceso);
            }
        }
    }
}
