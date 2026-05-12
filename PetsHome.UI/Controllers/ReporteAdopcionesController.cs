using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Reporte de adopciones")]
    public class ReporteAdopcionesController : BaseController
    {
        private readonly ReporteAdopcionesService _service;

        public ReporteAdopcionesController(ReporteAdopcionesService service)
        {
            _service = service;
        }

        [Breadcrumb("ReporteAdopciones", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Index()
        {
            var model = await _service.GetDashboardAsync();
            return View(model);
        }
    }
}
