using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Recordatorios / recalls (care-reminders de OpenVPM): cola de seguimiento
    /// clinico "a esta mascota le toca algo en tal fecha" (refuerzo de vacuna,
    /// seguimiento). Feed, no gate: el boton "Generar" rellena la cola desde la
    /// data real; nunca bloquea nada. No envia correo ni SMS.
    /// </summary>
    [Authorize]
    [PantallaAuthorize("Recordatorios")]
    public class RecordatoriosController : BaseController
    {
        private readonly RecordatorioService _service;

        public RecordatoriosController(RecordatorioService service)
        {
            _service = service;
        }

        [Breadcrumb("Recordatorios", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index() => View();

        /// <summary>JSON del listado con filtros por estado / rango de fechas / mascota.</summary>
        public async Task<IActionResult> List(string estado = null, DateTime? desde = null, DateTime? hasta = null, int? mascId = null)
        {
            var items = await _service.ListAsync(estado, desde, hasta, mascId);
            if (items != null)
                return Json(new { data = items });

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        [Breadcrumb("Detalle de recordatorio", FromAction = "Index", FromController = typeof(RecordatoriosController))]
        public async Task<IActionResult> Detail(int id)
        {
            var item = await _service.DetailAsync(id);
            if (item == null)
            {
                ShowAlert("Recordatorio no encontrado", AlertMessageType.Error);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        /// <summary>POST /Recordatorios/Generar -> auto-genera recordatorios pendientes. Devuelve { ok, generados, mensaje }.</summary>
        [HttpPost]
        public async Task<IActionResult> Generar()
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, generados = 0, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Recordatorios", "insertar"))
                return Json(new { ok = false, generados = 0, mensaje = "No tiene permiso para generar recordatorios." });

            int generados = await _service.GenerarAsync(CurrentUserId.Value);
            if (generados < 0)
                return Json(new { ok = false, generados = 0, mensaje = "No se pudieron generar los recordatorios." });

            return Json(new
            {
                ok = true,
                generados,
                mensaje = generados == 0
                    ? "No hay recordatorios nuevos por generar; todo esta al dia."
                    : "Se generaron " + generados + " recordatorio(s) nuevo(s)."
            });
        }

        /// <summary>POST /Recordatorios/MarcarHecho -> Pendiente/Enviado -> Hecho.</summary>
        [HttpPost]
        public async Task<IActionResult> MarcarHecho(int rec_Id)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Recordatorios", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para actualizar recordatorios." });

            bool ok = await _service.MarcarHechoAsync(rec_Id, CurrentUserId.Value);
            return Json(new { ok, mensaje = ok ? "Recordatorio marcado como hecho." : "No se pudo marcar como hecho." });
        }

        /// <summary>POST /Recordatorios/Descartar -> Pendiente/Enviado -> Descartado (con nota opcional).</summary>
        [HttpPost]
        public async Task<IActionResult> Descartar(int rec_Id, string nota = null)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Recordatorios", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para actualizar recordatorios." });

            bool ok = await _service.DescartarAsync(rec_Id, CurrentUserId.Value, nota);
            return Json(new { ok, mensaje = ok ? "Recordatorio descartado." : "No se pudo descartar." });
        }

        /// <summary>POST /Recordatorios/MarcarEnviado -> Pendiente -> Enviado (se aviso al responsable).</summary>
        [HttpPost]
        public async Task<IActionResult> MarcarEnviado(int rec_Id)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesion expirada, inicie sesion nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Recordatorios", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para actualizar recordatorios." });

            bool ok = await _service.MarcarEnviadoAsync(rec_Id, CurrentUserId.Value);
            return Json(new { ok, mensaje = ok ? "Recordatorio marcado como enviado." : "No se pudo marcar como enviado." });
        }
    }
}
