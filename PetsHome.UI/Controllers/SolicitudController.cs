using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class SolicitudController : BaseController
    {
        private readonly SolicitudService _SolicitudService;
        private readonly MascotaService _mascotaService;

        public SolicitudController(SolicitudService SolicitudService,
            MascotaService mascotaService
            )
        {
            _SolicitudService = SolicitudService;
            _mascotaService = mascotaService;
        }

        public IActionResult Index()
        {
            return View(new SolicitudFormViewModel());
        }

        public async Task<IActionResult> Create(int? masc_Id)
        {
            // Si viene una mascota seleccionada, precargar datos de la mascota
            if (masc_Id.HasValue && masc_Id.Value > 0)
            {
                var pet = await _mascotaService.FindAsync(masc_Id.Value);
                if (pet != null)
                {
                    var model = new SolicitudFormViewModel
                    {
                        masc_Id = pet.masc_Id,
                        masc_Nombre = pet.masc_Nombre,
                        masc_Imagen = pet.masc_Imagen,
                        raza_Descripcion = pet.raza_Descripcion,
                        refg_Nombre = pet.refg_Nombre,
                        masc_EsAdoptado = pet.masc_EsAdoptado ?? false,
                        sol_Fecha = DateTime.Today
                    };
                    return View(model);
                }
            }

            return View(new SolicitudFormViewModel
            {
                sol_Fecha = DateTime.Today
            });
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _SolicitudService.ListAsync();
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
            var itemSearched = await _SolicitudService.FindAsync(id);
            if (itemSearched != null)
            {
                return View("Create", itemSearched);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _SolicitudService.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Solicitud no encontrada", AlertMessageType.Error);
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

        public async Task<IActionResult> Add(SolicitudFormViewModel model, int userId)
        {

            if (!model.isEdit)
            {
                Boolean createdItem = await _SolicitudService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success);
                return RedirectToAction("Index");
            }
            else
            {
                Boolean updatedItem = await _SolicitudService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert(AlertMessaje.SuccessEdit, AlertMessageType.Success);
                return RedirectToAction("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int sol_Id)
        {
            Boolean deletedItem = await _SolicitudService.RemoveAsync(sol_Id);
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
    }
}
