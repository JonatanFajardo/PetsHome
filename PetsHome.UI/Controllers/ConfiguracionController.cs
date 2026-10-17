using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Configuracion de modulos")]
    public class ConfiguracionController : BaseController
    {
        private readonly ConfiguracionService _ConfiguracionService;

        public ConfiguracionController(ConfiguracionService configuracionService)
        {
            _ConfiguracionService = configuracionService;
        }

        [Breadcrumb("Ajustes · Módulos", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Index()
        {
            var modulos = (await _ConfiguracionService.ListModulosAsync() ?? Enumerable.Empty<ModuloViewModel>())
                .OrderBy(m => m.modu_Orden)
                .ToList();

            ViewBag.Configuracion = await _ConfiguracionService.GetConfiguracionAsync();

            return View(modulos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PantallaAuthorize("Configuracion de modulos", "editar")]
        public async Task<IActionResult> SetModulo(string codigo, bool activo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Json(new { success = false, message = "Código de módulo inválido." });

            bool ok = await _ConfiguracionService.SetModuloActivoAsync(codigo, activo);

            return Json(new
            {
                success = ok,
                codigo = codigo,
                activo = activo,
                message = ok
                    ? "Cambio guardado. Se aplicará al volver a iniciar sesión."
                    : "No se pudo actualizar el módulo (puede ser un módulo núcleo protegido)."
            });
        }
    }
}
