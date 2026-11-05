using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Endpoints AJAX de procedimientos clinicos estructurados. NO es una pantalla
    /// propia: se consume desde "En consulta" (registro) y "Perfil medico de mascota"
    /// (historial). Por eso reutiliza esos permisos en vez de declarar una pantalla nueva.
    /// </summary>
    [Authorize]
    public class ProcedimientosController : BaseController
    {
        private readonly ProcedimientoService _service;

        public ProcedimientosController(ProcedimientoService service)
        {
            _service = service;
        }

        /// <summary>Procedimientos de una visita (cita) — panel de la consulta.</summary>
        [HttpGet]
        public async Task<IActionResult> PorCita(int citaId)
        {
            var lista = await _service.ListPorCitaAsync(citaId);
            return Json(lista.Select(ToJson));
        }

        /// <summary>Procedimientos de una mascota (historial de la ficha, si se pide por AJAX).</summary>
        [HttpGet]
        public async Task<IActionResult> PorMascota(int mascId)
        {
            var lista = await _service.ListPorMascotaAsync(mascId);
            return Json(lista.Select(ToJson));
        }

        /// <summary>Guarda (alta/edicion) un procedimiento estructurado. Encola el cobro si aplica.</summary>
        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] ProcedimientoGuardarDto dto)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "En consulta", "insertar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para registrar procedimientos." });

            var result = await _service.GuardarAsync(dto, CurrentUserId.Value);
            return Json(new
            {
                ok = result.Ok,
                procId = result.ProcId,
                mensaje = result.Mensaje,
                cargoEncolado = result.CargoEncolado
            });
        }

        /// <summary>Elimina (borrado logico) un procedimiento.</summary>
        [HttpPost]
        public async Task<IActionResult> Eliminar([FromForm] int procId)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "En consulta", "eliminar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para eliminar procedimientos." });

            var (ok, mensaje) = await _service.EliminarAsync(procId, CurrentUserId.Value);
            return Json(new { ok, mensaje });
        }

        private static object ToJson(ProcedimientoListViewModel p) => new
        {
            procId = p.proc_Id,
            nombre = p.proc_Nombre,
            descripcion = p.proc_Descripcion,
            realizadoPor = p.RealizadoPorNombre,
            anestesia = p.proc_Anestesia,
            duracion = p.proc_DuracionMinutos,
            notas = p.proc_Notas,
            fecha = p.proc_Fecha.ToString("dd MMM yyyy HH:mm"),
            servId = p.serv_Id,
            servicioNombre = p.ServicioNombre,
            servicioPrecio = p.ServicioPrecio
        };
    }
}
