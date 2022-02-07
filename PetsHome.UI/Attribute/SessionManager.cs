using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
namespace PetsHome.UI.Attribute
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

            string pantallas = string.Empty;
            var session = context.HttpContext.Session.GetString("pantallas");
            if (string.IsNullOrEmpty(session))
            {
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
            else
            {
                pantallas = session;
                if (!pantallas.Contains(_pantallaNombre) && _pantallaNombre != "Home")
                    context.Result = new RedirectToRouteResult(sinAcceso);
            }
        }
    }
}
