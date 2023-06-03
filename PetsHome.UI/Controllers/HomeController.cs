using Microsoft.AspNetCore.Mvc;

namespace PetsHome.UI.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomeController
        public ActionResult Index()
        {
            return View();
        }
    }
}