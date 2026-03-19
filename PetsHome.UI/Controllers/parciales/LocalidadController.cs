using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    [Authorize]
    public class LocalidadController : BaseController
    {
        private readonly MunicipioService _municipioService;
        private readonly DepartamentoService _departamentoService;

        private readonly IMapper _mapper;

        public LocalidadController(MunicipioService municipioService, DepartamentoService departamentoService, IMapper mapper)
        {
            _municipioService = municipioService;
            _departamentoService = departamentoService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var departamentos = await _departamentoService.ListAsync();
            return View(departamentos);
        }

        public async Task<IActionResult> ListMunicipios(int id)
        {
            var listMunicipios = await _municipioService.ListIdAsync(id);
            return Json(new
            {
                data = listMunicipios
            });
        }

        public async Task<IActionResult> FormPartialDepartamento(int id)
        {
            if (id == 0)
            {
                var model = new DepartamentoFormViewModel();
                model.depto_Id = id;
                return View("FormPartialDepartamento", model);
            }
            else
            {
                var result = await _departamentoService.FindAsync(id);
                result.ListadoMunicipios = await _municipioService.ListIdAsync(id);
                return View("FormPartialDepartamento", result);
            }
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _departamentoService.ListAsync();
            return Json(new { data = itemListing });
        }

        public async Task<IActionResult> Add(DepartamentoFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            int userId = CurrentUserId.Value;

            if (!model.isEdit)
            {
                Boolean createdItem = await _departamentoService.AddAsync(model, userId);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("FormPartialDepartamento");
            }
            else
            {
                Boolean updatedItem = await _departamentoService.UpdateAsync(model, userId);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("FormPartialDepartamento", routeValues: new { id = model.depto_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> AddMunicipio(DepartamentoFormViewModel model)
        {
            if (!CurrentUserId.HasValue)
            {
                ShowAlert("Sesión expirada. Por favor, inicie sesión nuevamente.", AlertMessageType.Error);
                return RedirectToAction("Login", "Account");
            }

            int userId = CurrentUserId.Value;

            if (!model.Municipio.isEdit)
            {
                Boolean createdItem = await _municipioService.AddAsync(model.Municipio, userId);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("FormPartialDepartamento", routeValues: new { id = model.Municipio.depto_Id });
            }
            else
            {
                Boolean updatedItem = await _municipioService.UpdateAsync(model.Municipio, userId);
                if (!updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("FormPartialDepartamento", routeValues: new { id = model.Municipio.depto_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> FindMunicipio(int id)
        {
            var itemSearched = await _municipioService.FindAsync(id);
            if (itemSearched != null)
            {
                return Json(new { data = itemSearched, success = true });
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DetailDepartamento(int id)
        {
            var result = await _departamentoService.DetailAsync(id);
            if (result != null)
            {
                return View(nameof(DetailDepartamento), result);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

    }
}
