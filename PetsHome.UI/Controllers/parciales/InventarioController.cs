using Microsoft.AspNetCore.Mvc;

namespace PetsHome.UI.Controllers
{
    public class InventarioController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
