using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Lista de espera POR DEMANDA (waitlist): pacientes sin cupo todavia que
    /// piden "aviseme si se libera un espacio". Distinto de la sala de espera
    /// del dia (Tablero de Recepcion). Feed, no gate: sugiere a quien llamar,
    /// nunca bloquea el agendado normal.
    /// </summary>
    [Authorize]
    [PantallaAuthorize("Lista de espera")]
    public class ListaEsperaController : BaseController
    {
        private readonly ListaEsperaService _service;
        private readonly IMapper _mapper;

        public ListaEsperaController(ListaEsperaService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [Breadcrumb("Lista de espera", FromAction = "Index", FromController = typeof(HomeController))]
        public async Task<IActionResult> Index()
        {
            // Tipos de consulta para el filtro del buscador "¿a quien llamar?"
            ViewBag.TiposConsulta = await _service.TipoConsultaDropdown();
            return View();
        }

        public async Task<IActionResult> List(int? mascId = null, int? tipoConId = null, string estado = null)
        {
            var items = await _service.ListAsync(mascId, tipoConId, estado);
            if (items != null)
                return Json(new { data = items });

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            return RedirectToAction("Index");
        }

        /// <summary>¿A quien llamar? Devuelve los que encajan en un hueco (fecha + tipo), FIFO.</summary>
        public async Task<IActionResult> Sugerencias(DateTime? fecha = null, int? tipoConId = null)
        {
            var items = await _service.SugerenciasAsync(fecha, tipoConId);
            return Json(new { data = items ?? new System.Collections.Generic.List<ListaEsperaSugerenciaViewModel>() });
        }

        [Breadcrumb("Anotar en lista de espera", FromAction = "Index", FromController = typeof(ListaEsperaController))]
        [PantallaAuthorize("Lista de espera", "insertar")]
        public async Task<IActionResult> Create()
        {
            var model = new ListaEsperaFormViewModel
            {
                MascotaList = _service.MascotaDropdown(),
                TipoConsultaList = await _service.TipoConsultaDropdown()
            };
            return View(model);
        }

        [PantallaAuthorize("Lista de espera", "editar")]
        public async Task<IActionResult> Find(int id)
        {
            var item = await _service.FindAsync(id);
            if (item == null) return RedirectToAction("Index");

            // Solo mientras sigue esperando es editable; agendada/cancelada es historico.
            if (item.lesp_Estado != "Esperando")
            {
                ShowAlert("Solo se puede editar mientras sigue esperando.", AlertMessageType.Warning);
                return RedirectToAction("Detail", new { id });
            }

            var model = _mapper.Map<ListaEsperaFormViewModel>(item);
            model.MascotaList = _service.MascotaDropdown();
            model.TipoConsultaList = await _service.TipoConsultaDropdown();
            return View("Create", model);
        }

        [Breadcrumb("Detalle de lista de espera", FromAction = "Index", FromController = typeof(ListaEsperaController))]
        public async Task<IActionResult> Detail(int id)
        {
            var item = await _service.DetailAsync(id);
            if (item == null)
            {
                ShowAlert("Registro no encontrado", AlertMessageType.Error);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ListaEsperaFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            var operacion = model.isEdit ? "editar" : "insertar";
            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Lista de espera", operacion))
                return RedirectToAction("AccessDenied", "Account");

            int userId = CurrentUserId.Value;

            if (model.isEdit)
            {
                bool ok = await _service.UpdateAsync(model, userId);
                if (ok)
                {
                    ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                    return RedirectToAction("Detail", new { id = model.lesp_Id });
                }
            }
            else
            {
                int newId = await _service.AddAsync(model, userId);
                if (newId > 0)
                {
                    ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                    return RedirectToAction("Detail", new { id = newId });
                }
            }

            ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
            model.MascotaList = _service.MascotaDropdown();
            model.TipoConsultaList = await _service.TipoConsultaDropdown();
            return View("Create", model);
        }

        /// <summary>Esperando -> Agendada (se le dio cupo) o -> Cancelada (ya no quiere).</summary>
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int lesp_Id, string estado)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { ok = false, mensaje = "Sesión expirada, inicie sesión nuevamente." });

            if (!PantallaAuthorizeAttribute.TienePermiso(User, "Lista de espera", "editar"))
                return Json(new { ok = false, mensaje = "No tiene permiso para cambiar el estado." });

            if (estado != "Agendada" && estado != "Cancelada")
                return Json(new { ok = false, mensaje = "Estado destino no válido." });

            bool ok = await _service.CambiarEstadoAsync(lesp_Id, estado, CurrentUserId.Value);
            return Json(new { ok, mensaje = ok ? "Estado actualizado." : "No se pudo cambiar el estado (solo los que siguen esperando)." });
        }

        [HttpPost]
        [PantallaAuthorize("Lista de espera", "eliminar")]
        public async Task<IActionResult> Remove(int lesp_Id)
        {
            int userId = CurrentUserId ?? 0;
            bool ok = await _service.RemoveAsync(lesp_Id, userId);
            ShowAlert(ok ? "Eliminado de la lista de espera" : AlertMessaje.Error, ok ? AlertMessageType.Success : AlertMessageType.Error);
            return RedirectToAction("Index");
        }
    }
}
