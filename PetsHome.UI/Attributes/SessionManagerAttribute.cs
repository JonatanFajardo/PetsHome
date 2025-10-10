using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PetsHome.Business.Services;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetsHome.UI.Attributes
{
    /// <summary>
    /// Atributo para validar acceso a pantallas basado en sesiones
    /// Compatible con el sistema de permisos tipo AHM
    /// </summary>
    public class SessionManagerAttribute : ActionFilterAttribute
    {
        private readonly string _pantallaNombre;

        public SessionManagerAttribute()
        {
            _pantallaNombre = null;
        }

        public SessionManagerAttribute(string pantallaNombre)
        {
            _pantallaNombre = pantallaNombre;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sinAcceso = new RouteValueDictionary(new { action = "SinAcceso", controller = "Account" });
            var sesionExpirada = new RouteValueDictionary(new { action = "Login", controller = "Account" });

            try
            {
                // Verificar si el usuario está autenticado
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Si no se especifica pantalla, solo validar autenticación
                if (string.IsNullOrEmpty(_pantallaNombre))
                {
                    base.OnActionExecuting(context);
                    return;
                }

                // Obtener pantallas de la sesión
                string pantallas = context.HttpContext.Session.GetString("pantallas") ?? string.Empty;
                
                // Si no hay pantallas en sesión, denegar acceso
                if (string.IsNullOrEmpty(pantallas))
                {
                    context.Result = new RedirectToRouteResult(sinAcceso);
                    return;
                }

                // Verificar si la pantalla está en la lista de pantallas permitidas
                if (!pantallas.Contains(_pantallaNombre))
                {
                    context.Result = new RedirectToRouteResult(sinAcceso);
                    return;
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                // En caso de error, redirigir al login
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
        }
    }

    /// <summary>
    /// Atributo simplificado que solo valida si hay sesión activa
    /// </summary>
    public class RequireSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sesionExpirada = new RouteValueDictionary(new { action = "Login", controller = "Account" });

            try
            {
                // Verificar autenticación
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Verificar que exista el ID de usuario en sesión
                var usuarioId = context.HttpContext.Session.GetInt32("idUsuario");
                if (!usuarioId.HasValue)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
        }
    }

    /// <summary>
    /// Atributo que combina validación de permisos PetsHome y pantallas AHM
    /// </summary>
    public class HybridPermissionAttribute : ActionFilterAttribute
    {
        private readonly string _modulo;
        private readonly string _accion;
        private readonly string _pantalla;

        public HybridPermissionAttribute(string modulo, string accion, string pantalla = null)
        {
            _modulo = modulo;
            _accion = accion;
            _pantalla = pantalla;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sinAcceso = new RouteValueDictionary(new { action = "SinAcceso", controller = "Account" });
            var sesionExpirada = new RouteValueDictionary(new { action = "Login", controller = "Account" });

            try
            {
                // Verificar autenticación
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Verificar pantalla si se especifica
                if (!string.IsNullOrEmpty(_pantalla))
                {
                    string pantallas = context.HttpContext.Session.GetString("pantallas") ?? string.Empty;
                    if (string.IsNullOrEmpty(pantallas) || !pantallas.Contains(_pantalla))
                    {
                        context.Result = new RedirectToRouteResult(sinAcceso);
                        return;
                    }
                }

                // Verificar módulo si está disponible en sesión
                if (!string.IsNullOrEmpty(_modulo))
                {
                    string modulos = context.HttpContext.Session.GetString("modulos") ?? string.Empty;
                    if (!string.IsNullOrEmpty(modulos) && !modulos.Contains(_modulo))
                    {
                        context.Result = new RedirectToRouteResult(sinAcceso);
                        return;
                    }
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
        }
    }

    /// <summary>
    /// Atributo para validar permisos basado en sesiones - Sistema optimizado
    /// Verifica permisos desde la sesión sin consultas a base de datos
    /// </summary>
    public class SessionPermissionAttribute : ActionFilterAttribute
    {
        private readonly string _pantalla;
        private readonly string _permiso;

        public SessionPermissionAttribute(string pantalla, string permiso)
        {
            _pantalla = pantalla;
            _permiso = permiso;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sinAcceso = new RouteValueDictionary(new { action = "SinAcceso", controller = "Account" });
            var sesionExpirada = new RouteValueDictionary(new { action = "Login", controller = "Account" });

            try
            {
                // Verificar si el usuario está autenticado
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Obtener permisos desde la sesión
                string permisosJson = context.HttpContext.Session.GetString("permisos_pantallas");
                
                // Si no hay permisos en sesión, denegar acceso
                if (string.IsNullOrEmpty(permisosJson))
                {
                    context.Result = new RedirectToRouteResult(sinAcceso);
                    return;
                }

                // Obtener el servicio de autenticación para verificar permisos
                var authService = context.HttpContext.RequestServices.GetRequiredService<AuthService>();

                // Verificar el permiso usando el método de sesión
                bool tienePermiso = authService.CheckSessionPermission(permisosJson, _pantalla, _permiso);

                if (!tienePermiso)
                {
                    context.Result = new RedirectToRouteResult(sinAcceso);
                    return;
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                // En caso de error, redirigir al login
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
        }
    }

    /// <summary>
    /// Atributo híbrido que verifica permisos por pantalla con fallback a sistema anterior
    /// Útil para migración gradual del sistema
    /// </summary>
    public class HybridSessionPermissionAttribute : ActionFilterAttribute
    {
        private readonly string _pantalla;
        private readonly string _permiso;
        private readonly bool _useFallback;

        public HybridSessionPermissionAttribute(string pantalla, string permiso, bool useFallback = true)
        {
            _pantalla = pantalla;
            _permiso = permiso;
            _useFallback = useFallback;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sinAcceso = new RouteValueDictionary(new { action = "SinAcceso", controller = "Account" });
            var sesionExpirada = new RouteValueDictionary(new { action = "Login", controller = "Account" });

            try
            {
                // Verificar si el usuario está autenticado
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Intentar verificar con sistema de sesiones primero
                string permisosJson = context.HttpContext.Session.GetString("permisos_pantallas");
                
                if (!string.IsNullOrEmpty(permisosJson))
                {
                    var authService = context.HttpContext.RequestServices.GetRequiredService<AuthService>();
                    bool tienePermiso = authService.CheckSessionPermission(permisosJson, _pantalla, _permiso);

                    if (tienePermiso)
                    {
                        base.OnActionExecuting(context);
                        return;
                    }
                }

                // Fallback al sistema anterior si está habilitado
                if (_useFallback)
                {
                    string pantallas = context.HttpContext.Session.GetString("pantallas") ?? string.Empty;
                    
                    if (!string.IsNullOrEmpty(pantallas) && pantallas.Contains(_pantalla))
                    {
                        base.OnActionExecuting(context);
                        return;
                    }
                }

                // Si llegamos aquí, denegar acceso
                context.Result = new RedirectToRouteResult(sinAcceso);
            }
            catch (Exception)
            {
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
        }
    }

    /// <summary>
    /// Atributo para validar acceso a pantallas usando el nuevo sistema de seguridad extendida
    /// Verifica acceso por modpt_Id y registra accesos
    /// </summary>
    public class EnhancedScreenAccessAttribute : ActionFilterAttribute
    {
        private readonly int _modptId;
        private readonly bool _registerAccess;

        public EnhancedScreenAccessAttribute(int modptId, bool registerAccess = true)
        {
            _modptId = modptId;
            _registerAccess = registerAccess;
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var sinAcceso = new RouteValueDictionary(new { action = "SinAcceso", controller = "Account" });
            var sesionExpirada = new RouteValueDictionary(new { action = "Login", controller = "Account" });

            try
            {
                // Verificar si el usuario está autenticado
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Obtener el ID del usuario de la sesión
                var usuarioId = context.HttpContext.Session.GetInt32("idUsuario");
                if (!usuarioId.HasValue)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Obtener el servicio de autenticación
                var authService = context.HttpContext.RequestServices.GetRequiredService<AuthService>();

                // Verificar acceso a la pantalla
                var accessResult = await authService.VerificarAccesoPantallaAsync(usuarioId.Value, _modptId);
                
                if (!accessResult.Success || !(bool)accessResult.Data)
                {
                    context.Result = new RedirectToRouteResult(sinAcceso);
                    return;
                }

                // Registrar acceso si está habilitado
                if (_registerAccess)
                {
                    string userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
                    string ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                    
                    await authService.RegistrarAccesoPantallaAsync(usuarioId.Value, _modptId, userAgent, ipAddress);
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
        }
    }

    /// <summary>
    /// Atributo para validar acceso a pantallas por nombre (compatibilidad)
    /// con integración al nuevo sistema
    /// </summary>
    public class ScreenAccessByNameAttribute : ActionFilterAttribute
    {
        private readonly string _screenName;
        private readonly bool _registerAccess;

        public ScreenAccessByNameAttribute(string screenName, bool registerAccess = false)
        {
            _screenName = screenName;
            _registerAccess = registerAccess;
        }

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            var sinAcceso = new RouteValueDictionary(new { action = "SinAcceso", controller = "Account" });
            var sesionExpirada = new RouteValueDictionary(new { action = "Login", controller = "Account" });

            try
            {
                // Verificar si el usuario está autenticado
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Obtener el ID del usuario de la sesión
                var usuarioId = context.HttpContext.Session.GetInt32("idUsuario");
                if (!usuarioId.HasValue)
                {
                    context.Result = new RedirectToRouteResult(sesionExpirada);
                    return;
                }

                // Obtener el servicio de autenticación y permisos
                var authService = context.HttpContext.RequestServices.GetRequiredService<AuthService>();

                // Obtener pantallas del usuario
                var pantallasResult = await authService.GetPantallasPorUsuarioAsync(usuarioId.Value);
                
                if (!pantallasResult.Success)
                {
                    context.Result = new RedirectToRouteResult(sinAcceso);
                    return;
                }

                // Verificar si el usuario tiene acceso a la pantalla usando la clase tipada
                var pantallasData = (PantallasUsuarioResult)pantallasResult.Data;
                
                bool tieneAcceso = pantallasData.Pantallas.Any(p => 
                    (p.modpt_Descripcion?.Contains(_screenName) == true) || 
                    (p.modpt_Url?.Contains(_screenName) == true));

                if (!tieneAcceso)
                {
                    context.Result = new RedirectToRouteResult(sinAcceso);
                    return;
                }

                base.OnActionExecuting(context);
            }
            catch (Exception)
            {
                context.Result = new RedirectToRouteResult(sesionExpirada);
            }
        }
    }
}