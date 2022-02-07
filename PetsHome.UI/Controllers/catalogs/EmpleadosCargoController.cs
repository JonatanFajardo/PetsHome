using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class EmpleadosCargoController : BaseController
    {
        private readonly EmpleadosCargoService _empleadosCargoService;
        private readonly IMapper _mapper;

        public IActionResult Index()
        {
            return View();
        }

        public EmpleadosCargoController(EmpleadosCargoService empleadosCargoService,
                                IMapper mapper)
        {
            _empleadosCargoService = empleadosCargoService;
            _mapper = mapper;
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _empleadosCargoService.ListAsync();
            if (!object.ReferenceEquals(itemListing, null))
            {
                return Json(new { data = itemListing });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Find(int id)
        {
            var itemSearched = await _empleadosCargoService.FindAsync(id);
            if (itemSearched != null)
            {
                return Json(new { item = itemSearched, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            var itemDetail = await _empleadosCargoService.DetailAsync(id);
            if (itemDetail != null)
            {
                return Json(new { item = itemDetail, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        //[SessionManager("")]
        public async Task<IActionResult> Add(EmpleadoCargoViewModel model)
        {
            if (model.cag_Id == 0)
            {
                bool createdItem = await _empleadosCargoService.AddAsync(model);
                if (!createdItem)
                {
                    ShowAlert("Insertado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                bool updatedItem = await _empleadosCargoService.UpdateAsync(model);
                if (!updatedItem)
                {
                    ShowAlert("Modificado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }

        }

        public async Task<IActionResult> Remove(int id)
        {
            if (ModelState.IsValid)
            {
                bool deletedItem = await _empleadosCargoService.RemoveAsync(id);
                if (!deletedItem)
                {
                    ShowAlert("Eliminado", AlertMessageType.Success);
                    return RedirectToAction("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View("Index");
            }
        }
    }
}
