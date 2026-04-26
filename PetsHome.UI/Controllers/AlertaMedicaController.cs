using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Listado de alertas medicas")]
    public class AlertaMedicaController : BaseController
    {
        private readonly AlertaMedicaService _service;

        public AlertaMedicaController(AlertaMedicaService service)
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
