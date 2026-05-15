using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Perfil medico de mascota")]
    public class PerfilMedicoController : BaseController
    {
        private readonly PerfilMedicoService _service;

        public PerfilMedicoController(PerfilMedicoService service)
        {
            _service = service;
        }

        [Breadcrumb("PerfilMedico", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Index(int mascId)
        {
            if (mascId <= 0)
                mascId = await _service.GetRandomMascIdAsync();

            var model = await _service.GetDashboardAsync(mascId);
            return View(model);
        }
    }
}
