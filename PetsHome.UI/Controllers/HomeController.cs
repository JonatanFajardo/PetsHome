using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using SmartBreadcrumbs.Attributes;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly HomeService _homeService;
        private readonly DashboardVeterinarioService _dashboardVetService;
        private readonly DashboardSupervisorService _dashboardSupService;
        private readonly DashboardCuidadorService _dashboardCuidService;

        public HomeController(HomeService homeService,
            DashboardVeterinarioService dashboardVetService,
            DashboardSupervisorService dashboardSupService,
            DashboardCuidadorService dashboardCuidService)
        {
            _homeService         = homeService;
            _dashboardVetService = dashboardVetService;
            _dashboardSupService = dashboardSupService;
            _dashboardCuidService = dashboardCuidService;
        }

        [DefaultBreadcrumb("Inicio")]
        public async Task<ActionResult> Index()
        {
            var roleIdClaim = User.FindFirst("RoleId")?.Value;
            if (int.TryParse(roleIdClaim, out int rolId))
            {
                switch (rolId)
                {
                    case 3: // Supervisor
                        var modelSup = await _dashboardSupService.GetDashboardAsync();
                        return View("~/Views/DashboardSupervisor/Index.cshtml", modelSup);
                    case 4: // Veterinario
                        var modelVet = await _dashboardVetService.GetDashboardAsync();
                        return View("~/Views/DashboardVeterinario/Index.cshtml", modelVet);
                    case 5: // Cuidador
                        var modelCuid = await _dashboardCuidService.GetDashboardAsync();
                        return View("~/Views/DashboardCuidador/Index.cshtml", modelCuid);
                }
            }

            var homeViewModel = await _homeService.ObtenerEstadisticasDashboardAsync();
            return View(homeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefrescarFechasDemo()
        {
            var roleIdClaim = User.FindFirst("RoleId")?.Value;
            if (!int.TryParse(roleIdClaim, out int rolId) || rolId != 1)
                return Json(new { ok = false, msg = "Solo el administrador puede ejecutar esta acción." });

            var ok = await _homeService.RefrescarFechasDemoAsync();
            return Json(new
            {
                ok,
                msg = ok ? "Fechas del demo actualizadas correctamente." : "Ocurrió un error al refrescar las fechas."
            });
        }
    }
}
