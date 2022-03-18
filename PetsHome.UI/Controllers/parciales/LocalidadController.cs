using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.Logic.Repositories;
using System;
using System.Threading.Tasks;

namespace PetsHome.UI.Controllers
{
    public class LocalidadController : BaseController
    {
        private readonly MunicipioService _municipioService;
        private readonly DepartamentoService _departamentoService;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;


        public LocalidadController(MunicipioService municipioService,
                                    DepartamentoService departamentoService,
                                    //IHttpContextAccessor httpContextAccessor,
                                    IMapper mapper)
        {
            _municipioService = municipioService;
            _departamentoService = departamentoService;
            //_httpContextAccessor = httpContextAccessor;
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

        //public ActionResult EditDepartamentos()
        //{
        //    return View();
        //}

        //[HttpPost("Localidad/EditDepartamentos", Name = "save")]
        // 
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
                //var model = new DepartamentoViewModel();
                //model.Municipio.depto_Codigo = result.depto_Codigo;
                //model.Municipio.depto_Id = result.depto_Id;
                //var model = new DepartamentoViewModel();
                return View(nameof(EditDepartamentos), result);
            }
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _departamentoService.ListAsync();
            return Json(new { data = itemListing });
        }

        //public async Task<IActionResult> Find(int id)
        //{

        //    if (id != 0)
        //    {
        //        var itemSearched = await _departamentoService.FindAsync(id);
        //        //var dropdown = Dropdown(itemSearched);
        //        return View(nameof(EditDepartamento), itemSearched);
        //    }
        //    else
        //    {
        //        ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
        //        return RedirectToAction("Index");
        //    }
        //}

        //private object Dropdown(DepartamentoViewModel itemSearched)
        //{
        //    throw new NotImplementedException();
        //}

        
        public async Task<IActionResult> Add(DepartamentoViewModel model)
        {
            if (!model.isEdit)
            {
                Boolean createdItem = await _departamentoService.AddAsync(model);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                //return View("Create");
                return RedirectToAction("EditDepartamentos");
            }
            else
            {
                Boolean updatedItem = await _departamentoService.UpdateAsync(model);
                //Boolean validation = Validation.IsUpdate(updatedItem, ModelState.IsValid);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("EditDepartamentos", routeValues: new { id = model.depto_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        //[HttpPost("Departamentos/AddMunicipio", Name = "SaveMunicipio")]
        public async Task<IActionResult> AddMunicipio(DepartamentoViewModel model)
        {
            if (!model.Municipio.isEdit)
            {
                Boolean createdItem = await _municipioService.AddAsync(model.Municipio);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                //return View("Create");
                return RedirectToAction("EditDepartamentos", routeValues: new { id = model.depto_Id });
            }
            else
            {
                Boolean updatedItem = await _municipioService.UpdateAsync(model.Municipio);
                //Boolean validation = Validation.IsUpdate(updatedItem, ModelState.IsValid);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return RedirectToAction("EditDepartamentos", routeValues: new { id = model.depto_Id });
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        //public async Task<IActionResult> Remove(int masc_Id)
        //{
        //    Boolean deletedItem = await _departamentoService.RemoveAsync(masc_Id);
        //    if (!deletedItem)
        //    {
        //        ShowAlert("Eliminado", AlertMessageType.Success);
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
        //        return RedirectToAction("Index");
        //    }
        //}




        //[SessionManager("Registro Departamentos")]
        //public ActionResult EditDepartamento(int id)
        //{
        //    if (id == 0)
        //    {
        //        var model = new DepartamentoViewModel();
        //        model.depto_Id = id;
        //        return View(nameof(EditDepartamentos), model);
        //    }
        //    else
        //    {
        //        var result = _departamentoService.ListAsync();
        //        if (result == null)
        //        {
        //            return View(nameof(Index));
        //        }
        //        var model = new DepartamentoViewModel();
        //        ViewBag.depto_Id = id;
        //        model = _mapper.Map<DepartamentoViewModel>(result);
        //        model.EditarMunicipio = _mapper.Map<MunicipioViewModel>(result);
        //        //model.EditarDepartamento.depto_Id = id;
        //        //model.EditarDepartamento.depto_Codigo = result.depto_Codigo;
        //        //model.EditarDepartamento.depto_Descripcion = result.depto_Descripcion;
        //        //model.EditarMunicipio.depto_Id = result.depto_Id;
        //        //model.EditarMunicipio.codigoDept = result.depto_Codigo;
        //        return View(nameof(EditDepartamentos), model);
        //    }
        //}

        ////[SessionManager("Listado Municipios")]


        ////[SessionManager("Listado Municipios")]
        //public IActionResult ListMunicipiosDetalle(int id)
        //{
        //    var listMunicipios = _municipioService.DetailsMunicipios(id);
        //    return Json(new
        //    {
        //        data = listMunicipios
        //    });
        //}


        ////[SessionManager("Modificar Departamentos")]
        //public ActionResult EditDepartamentos()
        //{
        //    return View();
        //}

        ////[SessionManager("Registro Departamentos")]
        //[HttpPost("Departamentos/Editar", Name = "SaveDepartamentos")]
        //public ActionResult Editar(EditarDepartamento model)
        //{
        //    string message = "";
        //    int execute = 0;
        //    var id = _httpContextAccessor.HttpContext.Session.GetInt32("idUsuario");
        //    if (ModelState.IsValid)
        //    {
        //        if (model.depto_Id == 0)
        //        {
        //            execute = _departamentoService.Insert(model.depto_Codigo, model.depto_Descripcion.Trim(), (int)id);
        //            message = execute.ToString().StartsWith("-1") ? "No se pudo realizar la acción." : "Registro ingresado correctamente.";
        //            if (execute == -1)
        //            {
        //                ShowAlert("Registro existente", AlertMessageType.Error);
        //                return RedirectToAction("EditDepartamento");
        //            }
        //        }
        //        else
        //        {
        //            execute = _departamentoService.Update(model.depto_Id, model.depto_Codigo, model.depto_Descripcion.Trim(), (int)id);
        //            message = execute.ToString().StartsWith("-1") ? "No se pudo realizar la acción." : "Registro actualizado correctamente.";
        //            if (execute == -1)
        //            {
        //                ShowAlert("Registro existente", AlertMessageType.Error);
        //                return RedirectToAction("EditDepartamento", routeValues: new { id = model.depto_Id });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        ShowAlert("No se pudo realizar la acción.", AlertMessageType.Error);
        //        return RedirectToAction("EditDepartamento");
        //    }
        //    var type = execute.ToString().StartsWith("-1") ? AlertMessageType.Error : AlertMessageType.Success;
        //    ShowAlert(message, type);
        //    return RedirectToAction("EditDepartamento", routeValues: new { id = execute });
        //}

        ////[SessionManager("Registro Municipios")]
        //public IActionResult Acciones(string codigo, string codigoDept, int mpio_Id, int idDep, string municipio)
        //{
        //    var mpio_Codigo = codigoDept + codigo;
        //    var result = 0;
        //    var id = _httpContextAccessor.HttpContext.Session.GetInt32("idUsuario");
        //    var validacio = _municipioService.MunicipiosExist(mpio_Codigo);
        //    if (mpio_Id == 0)
        //    {
        //        if (validacio == null)
        //        {
        //            result = _municipioService.Insert(mpio_Codigo, municipio.Trim(), idDep, (int)id);
        //        }
        //        else
        //        {
        //            return Json("Registro existente");
        //        }
        //    }
        //    else
        //    {
        //        if (validacio == null || validacio.mpio_Id == mpio_Id)
        //        {
        //            result = _municipioService.Update(mpio_Id, mpio_Codigo, municipio.Trim(), idDep, (int)id);
        //        }
        //        else
        //        {
        //            return Json("Registro existente");
        //        }
        //    }
        //    return Json(result);
        //}

        //public IActionResult Delete(int id)
        //{
        //    var dependencyValidation = _departamentoService.ValidationMunicipios(id);
        //    if (dependencyValidation.Count() == 0)
        //    {
        //        var result = _departamentoService.Delete(id);
        //        return Json(result);
        //    }
        //    return Json(dependencyValidation.Count());
        //}

        ////[SessionManager("Listado Municipios")]
        //public IActionResult GetMunicipio(int id)
        //{
        //    var municipio = _municipioService.ListMunicipios().FirstOrDefault(x => x.mpio_Id == id);
        //    return AjaxResult(municipio, true);
        //}

        ////[SessionManager("Listado Departamentos")]
        //public IActionResult DepartamentosDetail(int id)
        //{
        //    var result = _departamentoService.DetailsDepartamentos()
        //       .FirstOrDefault(x => x.depto_Id == id);
        //    if (result == null)
        //    {
        //        return View(nameof(Index));
        //    }
        //    var municipio = _municipioService.DetailsMunicipios(id);
        //    var listado = new List<MunicipioViewModel>();
        //    if (municipio == null)
        //    {
        //        return View(nameof(Index));
        //    }
        //    var model = new DepartamentoViewModel();
        //    model.EditarDepartamento = _mapper.Map<EditarDepartamento>(result);
        //    //foreach (var item in municipio)
        //    //{
        //    //    listado.Add(new EditarMunicipio()
        //    //    {
        //    //        mpio_Codigo = item.mpio_Codigo,
        //    //        mpio_Descripcion = item.mpio_Descripcion,
        //    //        mpio_UsuarioCrea = item.mpio_UsuarioCrea,
        //    //        UsuarioCreacion = item.UsuarioCreacion,
        //    //        mpio_FechaCrea = item.mpio_FechaCrea,
        //    //        mpio_UsuarioModifica = item.mpio_UsuarioModifica,
        //    //        UsuarioModificacion = item.UsuarioModificacion,
        //    //        mpio_FechaModifica = item.mpio_FechaModifica
        //    //    });
        //    //}
        //    model.ListadoMunicipios = listado;
        //    return View(nameof(DetailDepartamento), model);
        //}

        ////[SessionManager("Listado Departamentos")]
        //public IActionResult DetailDepartamento()
        //{
        //    return View();
        //}

        ////[SessionManager("Modificar Municipios")]
        //public IActionResult DeleteMunicipios(int id)
        //{
        //    var validationPersonas = _municipioService.ValidationPersonas(id);
        //    var validationSucursales = _municipioService.ValidationSucursales(id);
        //    var validationProveedores = _municipioService.ValidationProveedores(id);
        //    var validationTotal = validationPersonas.Count() + validationSucursales.Count() + validationProveedores.Count();
        //    if (validationTotal == 0)
        //    {
        //        var result = _municipioService.Delete(id);
        //        return Json(result);
        //    }
        //    return Json(validationTotal);
        //}

    }
}
