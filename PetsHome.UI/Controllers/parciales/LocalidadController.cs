using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class LocalidadController : BaseController
    {
        private readonly MunicipioService _municipioService;
        private readonly DepartamentoService _departamentoService;

        private readonly IMapper _mapper;

        public LocalidadController(MunicipioService municipioService,
                                    DepartamentoService departamentoService,
                                    IMapper mapper)
        {
            _municipioService = municipioService;
            _departamentoService = departamentoService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListMunicipios(int id)
        {
            var listMunicipios = await _municipioService.ListIdAsync(id);
            return Json(new
            {
                data = listMunicipios
            });
        }

        public async Task<IActionResult> EditDepartamentos(int id)
        {
            if (id == 0)
            {
                var model = new DepartamentoViewModel();
                model.depto_Id = id;
                return View(nameof(EditDepartamentos), model);
            }
            else
            {
                var result = await _departamentoService.FindAsync(id);
                result.ListadoMunicipios = await _municipioService.ListIdAsync(id);
                return View(nameof(EditDepartamentos), result);
            }
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _departamentoService.ListAsync();
            return Json(new { data = itemListing });
        }

        public async Task<IActionResult> Add(DepartamentoViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _departamentoService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("EditDepartamentos");
            }
            else
            {
                Boolean updatedItem = await _departamentoService.UpdateAsync(model);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("EditDepartamentos", routeValues: new { id = model.depto_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> AddMunicipio(DepartamentoViewModel model)
        {
            if (!model.Municipio.isEdit)
            {
                Boolean createdItem = await _municipioService.AddAsync(model.Municipio);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("EditDepartamentos", routeValues: new { id = model.Municipio.depto_Id });
            }
            else
            {
                Boolean updatedItem = await _municipioService.UpdateAsync(model.Municipio);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("EditDepartamentos", routeValues: new { id = model.Municipio.depto_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> FindMunicipio(int id)
        {
            var itemSearched = await _municipioService.FindAsync(id);
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

    }
}