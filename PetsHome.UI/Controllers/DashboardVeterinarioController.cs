using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Dashboard veterinario")]
    public class DashboardVeterinarioController : BaseController
    {
        private readonly DashboardVeterinarioService _service;

        public DashboardVeterinarioController(DashboardVeterinarioService service)
        {
            _service = service;
        }

        [Breadcrumb("DashboardVeterinario", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Index()
        {
            var model = await _service.GetDashboardAsync();
            return View(model);
        }
    }
}
