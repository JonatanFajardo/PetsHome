using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using SmartBreadcrumbs.Attributes;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Dashboard cuidador")]
    public class DashboardCuidadorController : BaseController
    {
        private readonly DashboardCuidadorService _service;

        public DashboardCuidadorController(DashboardCuidadorService service)
        {
            _service = service;
        }

        [Breadcrumb("DashboardCuidador", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Index()
        {
            var model = await _service.GetDashboardAsync();
            return View(model);
        }
    }
}
