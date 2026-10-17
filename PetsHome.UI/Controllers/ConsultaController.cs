using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("En consulta")]
    public class ConsultaController : BaseController
    {
        private readonly ConsultaService _service;

        public ConsultaController(ConsultaService service)
        {
            _service = service;
        }

        [Breadcrumb("En Consulta", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Index(int mascId = 0, int? citaId = null)
        {
            // Sin mascId => estado vacio (elegir paciente). Ya NO se auto-carga
            // una mascota al azar: GetPantallaAsync devuelve SinPaciente=true + cola.
            var model = await _service.GetPantallaAsync(mascId, citaId ?? 0);
            return View(model);
        }

        /// <summary>
        /// El vet toma una visita de la sala de espera: la pasa a "En atencion".
        /// Devuelve { ok, mascId } para que el front abra /Consulta/Index?mascId=&citaId=.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Atender([FromForm] int citaId)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mascId = 0, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "En consulta", "editar"))
                return Json(new { ok = false, mascId = 0, mensaje = "No tiene permiso para atender consultas." });

            var (ok, mascId) = await _service.AtenderAsync(citaId, CurrentUserId.Value);
            return Json(new { ok, mascId });
        }

        /// <summary>
        /// Persiste la consulta (cita medica + peso) al Finalizar. Devuelve JSON { ok, citaId }.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] GuardarConsultaDto dto)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, citaId = 0, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "En consulta", "insertar"))
                return Json(new { ok = false, citaId = 0, mensaje = "No tiene permiso para registrar consultas." });

            var result = await _service.GuardarAsync(dto, CurrentUserId.Value);
            return Json(new
            {
                ok = result.Ok,
                citaId = result.CitaId,
                mensaje = result.Mensaje,
                faltantes = (result.Faltantes ?? new System.Collections.Generic.List<FaltanteStockDto>())
                    .Select(f => new { nombre = f.Nombre, disponible = f.Disponible, pedido = f.Pedido })
            });
        }
    }
}
