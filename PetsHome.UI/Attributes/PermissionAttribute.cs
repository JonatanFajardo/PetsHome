using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PetsHome.Business.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetsHome.UI.Attributes
{
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _modulo;
        private readonly string _permiso;

        public PermissionAttribute(string modulo, string permiso)
        {
            _modulo = modulo;
            _permiso = permiso;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Verificar si el usuario está autenticado
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            try
            {
                // Obtener el ID del usuario
                var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int usuarioId))
                {
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                    return;
                }

                // Obtener el servicio de autenticación
                var authService = context.HttpContext.RequestServices.GetRequiredService<AuthService>();

                // Verificar el permiso
                var result = await authService.CheckPermissionAsync(usuarioId, _modulo, _permiso);

                if (!result.Success || !(bool)result.Data)
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }
            catch (Exception)
            {
                // En caso de error, denegar acceso
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }
        }
    }
}