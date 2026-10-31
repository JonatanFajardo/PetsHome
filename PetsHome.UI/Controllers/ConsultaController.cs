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
        /// Picker de servicios facturables por categoria (Vacuna/Laboratorio/Procedimiento/Consulta).
        /// GET /Consulta/Servicios?categoria=Vacuna → JSON [{ id, nombre, precio, categoria }].
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Servicios(string categoria = null)
        {
            var lista = await _service.ServiciosDropdownAsync(categoria);
            return Json(lista.Select(s => new
            {
                id = s.serv_Id,
                nombre = s.serv_Nombre,
                precio = s.serv_Precio,
                categoria = s.serv_Categoria
            }));
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
                requiereCobro = result.RequiereCobro,
                faltantes = (result.Faltantes ?? new System.Collections.Generic.List<FaltanteStockDto>())
                    .Select(f => new { nombre = f.Nombre, disponible = f.Disponible, pedido = f.Pedido })
            });
        }

        /// <summary>
        /// Firma/bloquea la nota clinica de una visita. Despues de firmar, el texto
        /// (SOAP/diagnostico) queda inmutable y solo se corrige con addenda.
        /// POST /Consulta/Firmar → JSON { ok, mensaje }.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Firmar([FromForm] int citaId)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "En consulta", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para firmar notas." });

            var (ok, mensaje) = await _service.FirmarNotaAsync(citaId, CurrentUserId.Value);
            return Json(new { ok, mensaje });
        }

        /// <summary>
        /// Agrega una correccion (addenda) a la nota. Permitida incluso tras firmar.
        /// POST /Consulta/AgregarAddenda → JSON { ok, mensaje }.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AgregarAddenda([FromForm] int citaId, [FromForm] string texto)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "En consulta", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para agregar correcciones." });

            var (ok, mensaje) = await _service.AgregarAddendaAsync(citaId, texto, CurrentUserId.Value);
            return Json(new { ok, mensaje });
        }

        /// <summary>
        /// Lista las addenda (correcciones) de una visita, mas antiguas primero.
        /// GET /Consulta/Addenda?citaId= → JSON [{ texto, fecha, autor }].
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Addenda(int citaId)
        {
            var lista = await _service.ListAddendaAsync(citaId);
            return Json(lista.Select(a => new
            {
                texto = a.add_Texto,
                fecha = a.add_Fecha.ToString("dd MMM yyyy HH:mm"),
                autor = string.IsNullOrWhiteSpace(a.AutorNombre) ? "—" : a.AutorNombre
            }));
        }
    }
}
