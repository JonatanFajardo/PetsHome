using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    /// <summary>
    /// Componente transversal de "banderas rojas" clínicas (alergias + padecimientos).
    /// Se consume vía AJAX desde el Perfil Médico y En Consulta (mínima interferencia:
    /// un solo alert bar reutilizable, no se duplica por pantalla).
    /// </summary>
    [Authorize]
    public class BanderaClinicaController : BaseController
    {
        private readonly MascotaAlergiaService _alergiaService;
        private readonly MascotaPadecimientoService _padecimientoService;

        public BanderaClinicaController(MascotaAlergiaService alergiaService, MascotaPadecimientoService padecimientoService)
        {
            _alergiaService = alergiaService;
            _padecimientoService = padecimientoService;
        }

        /// <summary>Devuelve el alert bar renderizado para una mascota.</summary>
        [HttpGet]
        public async Task<IActionResult> Bar(int mascId)
        {
            var vm = new BanderasClinicasViewModel
            {
                masc_Id = mascId,
                Alergias = await _alergiaService.ListAsync(mascId),
                Padecimientos = await _padecimientoService.ListAsync(mascId)
            };
            return PartialView("_AlertBarClinica", vm);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAlergia([FromForm] MascotaAlergiaViewModel model)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada, inicie sesión nuevamente." });
            if (model == null || model.masc_Id <= 0 || string.IsNullOrWhiteSpace(model.alrg_Sustancia))
                return Json(new { success = false, message = "La sustancia es requerida." });

            var ok = await _alergiaService.AddAsync(model, CurrentUserId.Value);
            return Json(new { success = ok });
        }

        [HttpPost]
        public async Task<IActionResult> CorregirAlergia([FromForm] int alrgId)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada, inicie sesión nuevamente." });

            var ok = await _alergiaService.CorregirAsync(alrgId, CurrentUserId.Value);
            return Json(new { success = ok });
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPadecimiento([FromForm] MascotaPadecimientoViewModel model)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada, inicie sesión nuevamente." });
            if (model == null || model.masc_Id <= 0 || string.IsNullOrWhiteSpace(model.pade_Nombre))
                return Json(new { success = false, message = "El nombre del padecimiento es requerido." });

            var ok = await _padecimientoService.AddAsync(model, CurrentUserId.Value);
            return Json(new { success = ok });
        }

        [HttpPost]
        public async Task<IActionResult> CorregirPadecimiento([FromForm] int padeId)
        {
            if (!CurrentUserId.HasValue)
                return Json(new { success = false, message = "Sesión expirada, inicie sesión nuevamente." });

            var ok = await _padecimientoService.CorregirAsync(padeId, CurrentUserId.Value);
            return Json(new { success = ok });
        }
    }
}
