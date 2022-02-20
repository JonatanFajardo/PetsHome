using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PetsHome.Business.Data;
using PetsHome.Business.Extensions;
using PetsHome.Business.Helpers;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.Common.Entities;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class MascotaController : BaseController
    {
        private readonly MascotaService _mascotaService;
        private readonly IOptions<MascotaViewModel> _pathFile;
        public MascotaController(MascotaService mascotaService, IMapper mapper,
            IOptions<MascotaViewModel> options)
        {
            _mascotaService = mascotaService;
            _pathFile = options;

        }
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost("Mascotas/Agregar", Name = "save")]
        public IActionResult Create()
        {
            //tbMascotas tbMascotas = new tbMascotas() {
            //    masc_Imagen = "",
            //    masc_Nombre = "jona",
            //    raza_Id = 1,
            //    masc_Edad = 22,
            //    masc_Sexo = "M",
            //    masc_Peso = 13,
            //    masc_Talla = 43,
            //    masc_Color = "rojo",
            //    masc_Historia = "su historia",
            //    refg_Id = 1,
            //    proc_Id = 1,
            //    masc_UsuarioCrea = 1

            //};

            //_mascotaRepository.AddAsync(tbMascotas);
            //ViewBag.raza_Id()

            // Cargamos Dropdown
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
            model.masc_Imagen = await model.ImageFile.GetBytesAsync();

            if (!model.isEdit)
            {

                Boolean createdItem = await _mascotaService.AddAsync(model);
                Boolean validation = Validation.IsInsert(createdItem, ModelState.IsValid);
                if (!validation)
                {
                    ShowAlert("Insertado", AlertMessageType.Success);
                    //return View("Create");
                    return RedirectToAction("Create");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return RedirectToAction("Create");
                }
            }
            else
            {
                Boolean updatedItem = await _mascotaService.UpdateAsync(model);
                Boolean validation = Validation.IsUpdate(updatedItem, ModelState.IsValid);
                if (!validation)
                {
                    ShowAlert("Actualizado", AlertMessageType.Success);
                    return View("Index");
                }
                else
                {
                    ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                    return View("Index");
                }
            }

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

        public MascotaViewModel Dropdown(MascotaViewModel model)
        {
            //MascotaViewModel model = new MascotaViewModel();
            model.LoadDropDownList(_mascotaService.RazaDropdown(), Dropdownlist.LoadSexo(), _mascotaService.RefugioDropdown(), _mascotaService.ProcedenciaDropdown());
            return model;
        }

        //public MascotaViewModel RazaDropdown()
        //{
        //    MascotaViewModel model = new MascotaViewModel();
        //    var response = _mascotaRepository.RazaDropdown();
        //    model.LoadDropDownList(response);
        //    return model;
        //}

        //public MascotaViewModel RefugioDropdown()
        //{
        //    MascotaViewModel model = new MascotaViewModel();
        //    var response = _mascotaRepository.RefugioDropdown();
        //    model.LoadDropDownList(response);
        //    return model;
        //}
        //public MascotaViewModel ProcedenciaDropdown()
        //{
        //    MascotaViewModel model = new MascotaViewModel();
        //    var response = _mascotaRepository.ProcedenciaDropdown();
        //    model.LoadDropDownList(response);
        //    return model;
        //}
    }
}
