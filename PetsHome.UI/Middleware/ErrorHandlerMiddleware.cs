using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetsHome.UI.Middleware
{
    /// <summary>
    /// Middleware para manejar errores globalmente en la aplicación
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
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
            catch (Exception error)
            {
                await HandleExceptionAsync(context, error);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorDetails
            {
                Message = exception.Message,
                Path = context.Request.Path
            };

            switch (exception)
            {
                case ArgumentNullException _:
                case ArgumentException _:
                    // Argumentos inválidos
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    _logger.LogWarning(exception, "Error de validación: {Message} | Path: {Path}", exception.Message, context.Request.Path);
                    break;

                case KeyNotFoundException _:
                    // Recurso no encontrado
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = response.StatusCode;
                    _logger.LogWarning(exception, "Recurso no encontrado: {Message} | Path: {Path}", exception.Message, context.Request.Path);
                    break;

                case UnauthorizedAccessException _:
                    // No autorizado
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.StatusCode = response.StatusCode;
                    _logger.LogWarning(exception, "Acceso no autorizado: {Message} | Path: {Path}", exception.Message, context.Request.Path);
                    break;

                default:
                    // Error interno del servidor
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Ha ocurrido un error interno en el servidor. Por favor contacte al administrador.";
                    _logger.LogError(exception, "Error interno del servidor: {Message} | Path: {Path} | StackTrace: {StackTrace}",
                        exception.Message, context.Request.Path, exception.StackTrace);
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Modelo para respuestas de error
    /// </summary>
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }
    }
}
