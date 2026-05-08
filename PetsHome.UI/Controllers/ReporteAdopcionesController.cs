using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetDashboardAsync();
            return View(model);
        }
    }
}
