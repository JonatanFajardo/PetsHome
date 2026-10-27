using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Tablero de Recepcion / Sala de espera. Feed, no gate: recepcion encola
    /// y supervisa la cola del dia; el vet la atiende desde su propia lente.
    /// </summary>
    [Authorize]
    public class RecepcionController : BaseController
    {
        private readonly RecepcionService _service;

        public RecepcionController(RecepcionService service)
        {
            _service = service;
        }

        [Breadcrumb("Recepcion", FromAction = "Index", FromController = typeof(HomeController))]
        [PantallaAuthorize("Recepcion")]
        public async Task<IActionResult> Index()
        {
            var model = await _service.GetTableroAsync(vetId: ResolverVetLente(null));
            return View(model);
        }

        /// <summary>
        /// Resuelve el "lente por rol" de la cola:
        ///  - Si el llamador pasa un vetId explicito, se respeta.
        ///  - Si no, y el usuario logueado es veterinario (RoleId 4), filtra por su propio id
        ///    (su cola = sus citas + las sin asignar). No expone ids al cliente.
        ///  - En cualquier otro rol (recepcion/admin), null -> ve toda la cola.
        /// </summary>
        private int? ResolverVetLente(int? vetIdExplicito)
        {
            if (vetIdExplicito.HasValue && vetIdExplicito.Value > 0)
                return vetIdExplicito;

            var roleId = User.FindFirst("RoleId")?.Value;
            if (roleId == "4" && CurrentUserId.HasValue)
                return CurrentUserId.Value;

            return null;
        }

        /// <summary>GET /Recepcion/Tablero -> vista Kanban (whiteboard) del dia.</summary>
        [Breadcrumb("Tablero", FromAction = "Index", FromController = typeof(HomeController))]
        [PantallaAuthorize("Recepcion")]
        public IActionResult Tablero() => View();

        /// <summary>
        /// GET /Recepcion/WhiteboardData -> JSON del tablero del dia (3 estados: espera/atencion/finalizada).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> WhiteboardData(int? vetId = null)
        {
            var cola = await _service.WhiteboardAsync(null, ResolverVetLente(vetId));
            return Json(cola.ConvertAll(c => new
            {
                citaId = c.CitaId,
                mascId = c.MascId,
                mascotaNombre = c.MascotaNombre,
                duenoNombre = c.DuenoNombre,
                esInterno = c.EsInterno,
                motivo = c.Motivo,
                tipoConsulta = c.TipoConsulta,
                prioridad = c.Prioridad,
                minutosEspera = c.MinutosEspera,
                estado = c.Estado,
                vetAsignado = c.VetAsignado
            }));
        }

        /// <summary>
        /// GET /Recepcion/SalaEspera -> JSON de la cola del dia (alimenta widgets de dashboards).
        /// Sin vetId: el backend aplica el lente del usuario logueado (vet ve su cola; admin/recepcion ven todo).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> SalaEspera(int? vetId = null)
        {
            var cola = await _service.SalaEsperaAsync(null, ResolverVetLente(vetId));
            return Json(cola.ConvertAll(c => new
            {
                citaId = c.CitaId,
                mascId = c.MascId,
                mascotaNombre = c.MascotaNombre,
                duenoNombre = c.DuenoNombre,
                esInterno = c.EsInterno,
                motivo = c.Motivo,
                tipoConsulta = c.TipoConsulta,
                prioridad = c.Prioridad,
                horaLlegada = c.HoraLlegada,
                minutosEspera = c.MinutosEspera,
                estado = c.Estado,
                vetAsignado = c.VetAsignado
            }));
        }

        /// <summary>
        /// POST /Recepcion/CheckIn -> encola una visita "En espera". Devuelve { ok, citaId }.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, citaId = 0, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Recepcion", "insertar"))
                return Json(new { ok = false, citaId = 0, mensaje = "No tiene permiso para registrar ingresos." });

            var result = await _service.CheckInAsync(dto, CurrentUserId.Value);
            return Json(new { ok = result.Ok, citaId = result.CitaId, mensaje = result.Mensaje });
        }

        /// <summary>
        /// GET /Recepcion/Veterinarios -> JSON [{ id, nombre }] para el selector opcional del check-in.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Veterinarios()
        {
            var vets = await _service.VeterinariosAsync();
            return Json(vets.ConvertAll(v => new { id = v.Id, nombre = v.Nombre }));
        }

        /// <summary>
        /// POST /Recepcion/CambiarEstado -> transiciona una visita (cancelar, no asistio, etc.).
        /// Devuelve { ok }.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CambiarEstado([FromForm] int citaId, [FromForm] string estado)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Recepcion", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para cambiar el estado." });

            var ok = await _service.CambiarEstadoAsync(citaId, estado, CurrentUserId.Value);
            return Json(new { ok });
        }
    }
}
