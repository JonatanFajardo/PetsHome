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

        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }

        // GET: HomeController
        [DefaultBreadcrumb("Inicio")]
        public async Task<ActionResult> Index()
        {
            var homeViewModel = await _homeService.ObtenerEstadisticasDashboardAsync();
            return View(homeViewModel);
        }
    }
}