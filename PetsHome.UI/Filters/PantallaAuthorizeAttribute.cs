using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace PetsHome.UI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PantallaAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _pantallaNombre;

        public PantallaAuthorizeAttribute() { }

        public PantallaAuthorizeAttribute(string pantallaNombre)
        {
            _pantallaNombre = pantallaNombre;
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
    }
}
