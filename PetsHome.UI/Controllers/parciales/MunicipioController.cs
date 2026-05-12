using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

namespace PetsHome.UI.Controllers
{
    public class MunicipioController : BaseController
    {
        [Breadcrumb("Municipio", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Index()
        {
            return View();
        }
    }
}