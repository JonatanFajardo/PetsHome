using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Middleware
{

    /// <summary>
    /// Middleware para interceptar y mejorar errores globalmente
    /// </summary>
    public class DropDownErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DropDownErrorMiddleware> _logger;

        public DropDownErrorMiddleware(RequestDelegate next, ILogger<DropDownErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NullReferenceException ex)
            {
                // Verificar si es el error específico de SelectList
                if (ex.StackTrace?.Contains("GetListItemsWithValueField") == true &&
                    ex.StackTrace?.Contains("SelectTagHelper") == true)
                {
                    var friendlyMessage = "❌ ERROR DE SELECTLIST: Posible problema con nombres de propiedades en un dropdown.\n" +
                                        "🔍 Verifica que los nombres de las propiedades en el SelectList coincidan exactamente con las propiedades del modelo.\n" +
                                        "📝 Ejemplo: Si tu modelo tiene 'com_Descripcion', usa 'com_Descripcion' en el SelectList, no 'com_Nombre'.";

                    _logger.LogError(ex, friendlyMessage);

                    // Crear una excepción más amigable
                    throw new InvalidOperationException(friendlyMessage, ex);
                }

                throw; // Re-lanzar si no es el error que esperamos
            }
        }
    }
}
