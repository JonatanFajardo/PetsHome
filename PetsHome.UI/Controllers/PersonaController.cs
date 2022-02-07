using Microsoft.AspNetCore.Mvc;

namespace PetsHome.UI.Controllers
{
    public class PersonaController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
