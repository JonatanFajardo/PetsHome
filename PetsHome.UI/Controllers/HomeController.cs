using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeService _homeService;

        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }

        // GET: HomeController
        public async Task<ActionResult> Index()
        {
            var homeViewModel = await _homeService.ObtenerEstadisticasDashboardAsync();
            return View(homeViewModel);
        }
    }
}