using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using PetsHome.Business.Services;
using PetsHome.UI.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    [PantallaAuthorize("Control de vacunacion")]
    public class ControlVacunacionController : BaseController
    {
        private readonly ControlVacunacionService _service;

        public ControlVacunacionController(ControlVacunacionService service)
        {
            _service = service;
        }

        [Breadcrumb("ControlVacunacion", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index() => View();

        [Breadcrumb("Matriz Data", FromAction = "Index", FromController = typeof(ControlVacunacionController))]
        public async Task<IActionResult> MatrizData()
        {
            var filas = await _service.MatrizVacunacionAsync();

            // Vacunas únicas (columnas)
            var vacunas = filas
                .GroupBy(r => r.vac_Id)
                .Select(g => new {
                    vac_Id     = g.Key,
                    vac_Nombre = g.First().vac_Nombre,
                    especies   = g.Select(x => x.masc_Especie).Distinct().ToList()
                })
                .OrderBy(v => v.vac_Nombre)
                .ToList();

            // Mascotas con sus vacunas anidadas
            var mascotas = filas
                .GroupBy(r => r.masc_Id)
                .Select(g => new {
                    masc_Id      = g.Key,
                    masc_Nombre  = g.First().masc_Nombre,
                    masc_Especie = g.First().masc_Especie,
                    raz_Nombre   = g.First().masc_Raza,
                    refg_Nombre  = g.First().masc_Refugio,
                    vacunas = g
                        .Where(r => r.vac_Id > 0)
                        .Select(r => new {
                            vac_Id                = r.vac_Id,
                            cvac_Estado           = r.cvac_Estado,
                            cvac_FechaAplicacion  = r.cvac_FechaAplicacion,
                            cvac_FechaVencimiento = r.cvac_FechaVencimiento
                        }).ToList()
                })
                .OrderBy(p => p.masc_Nombre)
                .ToList();

            return Json(new { vacunas, mascotas });
        }
    }
}
