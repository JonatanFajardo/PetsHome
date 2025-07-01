using Microsoft.AspNetCore.Mvc;

namespace PetsHome.UI.Controllers
{
    public class MunicipioController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}