using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetsHome.Business.Data;
using PetsHome.Business.Extensions;
using PetsHome.Business.Helpers;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class MascotaController : BaseController
    {
        private readonly MascotaService _mascotaService;
        private readonly RefugioService _refugioService;
        private readonly IOptions<MascotaViewModel> _pathFile;

        public MascotaController(MascotaService mascotaService,
            RefugioService refugioService,
            IOptions<MascotaViewModel> options)
        {
            _mascotaService = mascotaService;
            _refugioService = refugioService;
            _pathFile = options;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var model = new MascotaViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _mascotaService.ListAsync();
            return Json(new { data = itemListing });
        }

        public async Task<IActionResult> Find(int id)
        {
            if (id != 0)
            {
                var itemSearched = await _mascotaService.FindAsync(id);
                var dropdown = Dropdown(itemSearched);
                string imgBase64Data = itemSearched.masc_Imagen.GetImage();
                ViewBag.ImageFile = string.Format("data:image/png;base64,{0}", imgBase64Data);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add(MascotaViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _mascotaService.AddAsync(model);
                Boolean validation = Validation.IsInsert(createdItem, ModelState.IsValid);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                Boolean updatedItem = await _mascotaService.UpdateAsync(model);
                Boolean validation = Validation.IsUpdate(updatedItem, ModelState.IsValid);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return View("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int masc_Id)
        {
            Boolean deletedItem = await _mascotaService.RemoveAsync(masc_Id);
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

        /// <summary>
        /// Cargamos Dropdown
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MascotaViewModel Dropdown(MascotaViewModel model)
        {
            model.LoadDropDownList(_mascotaService.RazaDropdown(), Dropdownlist.LoadSexo(), _refugioService.RefugioDropdown(), _mascotaService.ProcedenciaDropdown());
            return model;
        }

    }
}