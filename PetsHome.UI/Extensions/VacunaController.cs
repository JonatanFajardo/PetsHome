using Microsoft.AspNetCore.Mvc;

namespace PetsHome.UI.Extensions
{
    public class VacunaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
