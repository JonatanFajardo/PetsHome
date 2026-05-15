using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class LandingController : Controller
    {
        private readonly MascotaService _mascotaService;
        private readonly EventoService _eventoService;
        private readonly SolicitudService _solicitudService;

        public LandingController(MascotaService mascotaService, EventoService eventoService, SolicitudService solicitudService)
        {
            _mascotaService = mascotaService;
            _eventoService = eventoService;
            _solicitudService = solicitudService;
        }

        public async Task<IActionResult> Index()
        {
            var todasMascotas = await _mascotaService.ListAsync();
            var todosEventos = await _eventoService.ListAsync();

            var disponibles = todasMascotas
                .Where(m => m.masc_EsAdoptado != true && m.masc_EsReservado != true)
                .Take(6)
                .ToList();

            var model = new LandingViewModel
            {
                Mascotas = disponibles,
                Eventos = todosEventos.Take(3).ToList(),
                TotalMascotas = todasMascotas.Count,
                TotalAdoptados = todasMascotas.Count(m => m.masc_EsAdoptado == true),
                TotalDisponibles = disponibles.Count
            };

            return View(model);
        }

        public async Task<IActionResult> Animales(string filtro = "todos")
        {
            var todasMascotas = await _mascotaService.ListAsync();

            var disponibles = todasMascotas
                .Where(m => m.masc_EsAdoptado != true && m.masc_EsReservado != true)
                .ToList();

            var model = new LandingAnimalesViewModel
            {
                Mascotas = disponibles,
                FiltroActivo = filtro ?? "todos"
            };

            return View(model);
        }

        public async Task<IActionResult> Animal(int id)
        {
            var mascota = await _mascotaService.DetailAsync(id);
            if (mascota == null)
                return RedirectToAction("Animales");

            var model = new LandingAnimalDetalleViewModel { Mascota = mascota };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Solicitar(int id)
        {
            var mascota = await _mascotaService.DetailAsync(id);
            if (mascota == null)
                return RedirectToAction("Animales");

            var model = new SolicitudFormViewModel
            {
                masc_Id = id,
                sol_Fecha = DateTime.Today
            };

            ViewBag.Mascota = mascota;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(SolicitudFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var mascota = await _mascotaService.DetailAsync(model.masc_Id);
                ViewBag.Mascota = mascota;
                return View(model);
            }

            model.sol_Fecha = DateTime.Today;
            var ok = await _solicitudService.AddAsync(model, 1);

            if (ok)
                return RedirectToAction("Gracias", new { nombre = model.sol_Nombres, masc_Id = model.masc_Id });

            ModelState.AddModelError("", "Ocurrió un error al enviar la solicitud. Intenta de nuevo.");
            var mascotaErr = await _mascotaService.DetailAsync(model.masc_Id);
            ViewBag.Mascota = mascotaErr;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Gracias(string nombre, int masc_Id)
        {
            var mascota = await _mascotaService.DetailAsync(masc_Id);
            ViewBag.Nombre = nombre;
            ViewBag.Mascota = mascota;
            return View();
        }
    }
}
