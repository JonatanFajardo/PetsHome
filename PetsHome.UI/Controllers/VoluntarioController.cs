using Microsoft.AspNetCore.Mvc;

namespace PetsHome.UI.Controllers
{
    public class VoluntarioController : BaseController
    {
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
    }
}
