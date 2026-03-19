using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    public class AdopcionController : BaseController
    {
        private readonly AdopcionService _AdopcionService;
        private readonly SolicitudService _SolicitudService;
        private readonly MascotaService _MascotaService;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public AdopcionController(AdopcionService AdopcionService,
            SolicitudService SolicitudService,
            MascotaService MascotaService
        )
        {
            _AdopcionService = AdopcionService;
            _SolicitudService = SolicitudService;
            _MascotaService = MascotaService;
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _AdopcionService.ListAsync();
            if (itemListing != null)
            {
                return Json(new { data = itemListing });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _AdopcionService.FindAsync(id);
            if (itemSearched != null)
            {
                return AjaxResult(itemSearched, true);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return AjaxResult(itemSearched, true);
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _AdopcionService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Adopción no encontrada", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("Details", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        // Detalle de adopción por mascota con tabs de solicitantes
        [HttpGet]
        public async Task<IActionResult> DetailByMascota(int masc_Id)
        {
            if (masc_Id <= 0)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }

            // Seguir el patrón de servicios existente: usar SolicitudService
            var solicitantesDeMascota = await ObtenerSolicitantesPorMascotaAsync(masc_Id);

            if (solicitantesDeMascota == null || solicitantesDeMascota.Count == 0)
            {
                ShowAlert("Esta mascota no tiene solicitantes", AlertMessageType.Info);
                return RedirectToAction("Index");
            }

            return View("Detail", solicitantesDeMascota);
        }


        public async Task<IActionResult> Add(AdopcionViewModel model, int userId)
        {

            if (!model.isEdit)
            {
                Boolean createdItem = await _AdopcionService.AddAsync(model, userId);
                if (!createdItem)
                {
                    ShowAlert("Insertado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Boolean updatedItem = await _AdopcionService.UpdateAsync(model, userId);
                if (!updatedItem)
                {
                    ShowAlert("Modificado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
        }

        public async Task<IActionResult> Remove(int adop_Id)
        {
            Boolean deletedItem = await _AdopcionService.RemoveAsync(adop_Id);
            if (!deletedItem)
            {
                ShowAlert("Eliminado", AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ElegirAdoptante(int sol_Id, int masc_Id)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            int userId = CurrentUserId.Value;
            var solicitantesDeMascota = new List<AdopcionDetailsViewModel>();

            try
            {
                if (sol_Id <= 0)
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    if (masc_Id > 0)
                        solicitantesDeMascota = await ObtenerSolicitantesPorMascotaAsync(masc_Id);
                    return View("Detail", solicitantesDeMascota);
                }

                var solicitudDetalle = await _SolicitudService.DetailAsync(sol_Id);
                if (solicitudDetalle == null)
                {
                    ShowAlert("Solicitud no encontrada", AlertMessageType.Error);
                    if (masc_Id > 0)
                        solicitantesDeMascota = await ObtenerSolicitantesPorMascotaAsync(masc_Id);
                    return View("Detail", solicitantesDeMascota);
                }

                masc_Id = solicitudDetalle.masc_Id;
                solicitantesDeMascota = await ObtenerSolicitantesPorMascotaAsync(masc_Id);

                var crearAdopcion = new AdopcionViewModel
                {
                    sol_Id = sol_Id,
                    adop_Estado = "Aprobado",
                    adop_FechaRegistro = DateTime.Today
                };

                Boolean adopcionCreada = await _AdopcionService.AddAsync(crearAdopcion, userId);
                if (adopcionCreada)
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return View("Detail", solicitantesDeMascota);
                }

                foreach (var solicitante in solicitantesDeMascota)
                {
                    if (solicitante.sol_Id == sol_Id)
                        continue;

                    var rechazo = new AdopcionViewModel
                    {
                        sol_Id = solicitante.sol_Id,
                        adop_Estado = "Rechazado",
                        adop_FechaRegistro = DateTime.Today
                    };

                    await _AdopcionService.AddAsync(rechazo, userId);
                }

                var mascotaSeleccionada = await _MascotaService.FindAsync(masc_Id);
                if (mascotaSeleccionada != null)
                {
                    mascotaSeleccionada.masc_EsAdoptado = true;
                    mascotaSeleccionada.masc_EsReservado = false;
                    mascotaSeleccionada.masc_UsuarioModifica = userId;
                    var resultadoMascota = await _MascotaService.UpdateAsync(mascotaSeleccionada, userId);
                    if (resultadoMascota)
                    {
                        ShowAlert("No se pudo actualizar el estado de la mascota", AlertMessageType.Warning);
                    }
                }

                ShowAlert("Adoptante elegido y demas solicitantes rechazados", AlertMessageType.Success);

                var solicitantesActualizados = await ObtenerSolicitantesPorMascotaAsync(masc_Id);
                return View("Detail", solicitantesActualizados);
            }
            catch (Exception)
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                if (masc_Id > 0)
                {
                    solicitantesDeMascota = await ObtenerSolicitantesPorMascotaAsync(masc_Id);
                }

                return View("Detail", solicitantesDeMascota);
            }
        }

        private async Task<List<AdopcionDetailsViewModel>> ObtenerSolicitantesPorMascotaAsync(int masc_Id)
        {
            if (masc_Id <= 0)
            {
                return new List<AdopcionDetailsViewModel>();
            }

            var detalle = await _AdopcionService.DetailAsync(masc_Id);

            if (detalle == null || !detalle.Any())
            {
                return new List<AdopcionDetailsViewModel>();
            }

            return detalle.ToList();
        }
    }
}
