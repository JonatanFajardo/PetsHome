using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class EmpleadoController : BaseController
    {
        private readonly EmpleadoService _empleadoService;
        private readonly IMapper _mapper;
        public EmpleadoController(EmpleadoService empleadoService, IMapper mapper)
        {
            _empleadoService = empleadoService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        // Vista Create
        public IActionResult Create()
        {
            return View();
        }

        // Vista Detail
        public IActionResult Detail()
        {
            return View();
        }
        public async Task<IActionResult> List()
        {
            var result = await _empleadoService.ListAsync();
            return Json(new { data = result });
        }
    }
}
