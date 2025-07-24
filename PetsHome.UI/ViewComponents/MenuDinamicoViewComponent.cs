using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetsHome.UI.ViewComponents
{
    public class MenuDinamicoViewComponent : ViewComponent
    {
        private readonly PermisosService _permisosService;

        public MenuDinamicoViewComponent(PermisosService permisosService)
        {
            _permisosService = permisosService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                // Verificar si el usuario está autenticado
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return View(new MenuViewModel());
                }

                // Obtener el ID del usuario
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int usuarioId))
                {
                    return View(new MenuViewModel());
                }

                // Obtener el menú del usuario
                var result = await _permisosService.GetMenuUsuarioAsync(usuarioId);
                if (result.Success)
                {
                    var menuViewModel = (MenuViewModel)result.Data;
                    
                    // Agregar información adicional del usuario si está disponible
                    var nombreClaim = HttpContext.User.FindFirst("NombreCompleto")?.Value;
                    if (!string.IsNullOrEmpty(nombreClaim))
                    {
                        menuViewModel.UsuarioNombre = nombreClaim;
                    }

                    return View(menuViewModel);
                }
                else
                {
                    // Si hay error, retornar menú vacío
                    return View(new MenuViewModel
                    {
                        UsuarioNombre = HttpContext.User.Identity.Name ?? "Usuario",
                        RolDescripcion = "Sin rol asignado"
                    });
                }
            }
            catch (Exception)
            {
                // En caso de error, retornar menú básico
                return View(new MenuViewModel
                {
                    UsuarioNombre = HttpContext.User.Identity.Name ?? "Usuario",
                    RolDescripcion = "Error al cargar permisos"
                });
            }
        }
    }
}