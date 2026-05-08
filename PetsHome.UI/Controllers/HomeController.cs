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

        public HomeController(HomeService homeService,
            DashboardVeterinarioService dashboardVetService,
            DashboardSupervisorService dashboardSupService)
        {
            _homeService       = homeService;
            _dashboardVetService = dashboardVetService;
            _dashboardSupService = dashboardSupService;
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
                }
            }

            var homeViewModel = await _homeService.ObtenerEstadisticasDashboardAsync();
            return View(homeViewModel);
        }
    }
}
