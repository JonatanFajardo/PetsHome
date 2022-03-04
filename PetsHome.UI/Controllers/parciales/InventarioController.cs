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
        //public MascotaViewModel Dropdown(MascotaViewModel model)
        //{
        //    model.LoadDropDownList(_mascotaService.RazaDropdown(), Dropdownlist.LoadSexo(), _RefugioService.RefugioDropdown(), _mascotaService.ProcedenciaDropdown());
        //    return model;
        //}
    }
}
