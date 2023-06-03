using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;

namespace PetsHome.UI.Controllers
{
    public class InventarioController : BaseController
    {
        private readonly RefugioService _refugioService;

        public InventarioController(RefugioService refugioService)
        {
            _refugioService = refugioService;
        }

        public IActionResult Index()
        {
            return View();
        }

    }
}