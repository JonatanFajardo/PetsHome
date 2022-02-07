using Microsoft.AspNetCore.Mvc;

namespace PetsHome.UI.Controllers
{
    public class EntradaController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
