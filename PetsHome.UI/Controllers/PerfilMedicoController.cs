using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Linq;
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

        // Certificado de vacunacion imprimible (Layout = null en la vista).
        [HttpGet]
        public async Task<IActionResult> Certificado(int mascId)
        {
            if (mascId <= 0)
                mascId = await _service.GetRandomMascIdAsync();

            var model = await _service.GetCertificadoAsync(mascId);
            return View(model);
        }

        // Recordatorios in-app: vacunas con refuerzo proximo o vencido.
        [HttpGet]
        public async Task<IActionResult> ProximosRefuerzos(int? mascId)
        {
            var refuerzos = await _service.GetProximosRefuerzosAsync(mascId);
            var payload = refuerzos.Select(r => new
            {
                mascotaNombre   = r.MascotaNombre,
                vacunaNombre    = r.VacunaNombre,
                proximoRefuerzo = r.ProximoRefuerzo,
                estado          = r.Estado
            });
            return Json(payload);
        }
    }
}
