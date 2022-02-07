using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Services;

namespace PetsHome.UI.Controllers
{
    public class LocalidadController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly MunicipioService _MunicipioService;
        private readonly DepartamentoService _departamentoService;
        private readonly IMapper _mapper;

        //private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalidadController(MunicipioService MunicipioService,
                                    DepartamentoService departamentoService,
                                    IMapper mapper
            //                        IHttpContextAccessor httpContextAccessor
            )
        {
            _MunicipioService = MunicipioService;
            _departamentoService = departamentoService;
            _mapper = mapper;

            //_httpContextAccessor = httpContextAccessor;
        }

        ////#region Departamento
        //////[SessionManager("Listado Departamentos")]
        ////public async Task<IActionResult> ListDepartamento()
        ////{
        ////    var result = _departamentoService.ListAsync();
        ////    if (result != null)
        ////    {
        ////        ShowAlert("Insertado", AlertMessageType.Success);
        ////        return Json(new { data = result });
        ////    }
        ////    else
        ////    {
        ////        ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
        ////        return RedirectToAction("Index");
        ////    }
        ////}


        //////[SessionManager("Registro Departamentos")]
        //////[HttpPost("Departamentos/Editar", Name = "SaveDepartamentos")]
        ////public async Task<IActionResult> Editar(DepartamentoViewModel model)
        ////{
        ////    string message = "";
        ////    var mapModel = _mapper.Map<tbDepartamentos>(model);
        ////    Boolean validation = Validation.IsInsert(model.EditarDepartamento.depto_Id, ModelState.IsValid);
        ////    //var id = _httpContextAccessor.HttpContext.Session.GetInt32("idUsuario");
        ////    if (!validation)
        ////    {
        ////        bool result = await _departamentoService.AddAsync(mapModel);
        ////        if (!result)
        ////        {
        ////            ShowAlert("Registro ingresado correctamente.", AlertMessageType.Success);
        ////            return View("Create");
        ////        }
        ////        else
        ////        {
        ////            ShowAlert("No se pudo realizar la acción.", AlertMessageType.Error);
        ////            return View("Create");
        ////        }
        ////    }
        ////    else
        ////    {
        ////        bool result = await _departamentoService.Edit(mapModel);
        ////        if (!result)
        ////        {
        ////            ShowAlert("No se pudo realizar la acción.", AlertMessageType.Success);
        ////            return View("Create");
        ////        }
        ////        else
        ////        {
        ////            ShowAlert("Registro actualizado correctamente.", AlertMessageType.Error);
        ////            return RedirectToAction("EditDepartamento", routeValues: new { id = model.EditarDepartamento.depto_Id });

        ////        }
        ////    }
        ////    //var type = execute.ToString().StartsWith("-1") ? AlertMessageType.Error : AlertMessageType.Success;
        ////    //ShowAlert(message, type);
        ////    //return RedirectToAction("EditDepartamento", routeValues: new { id = execute });
        ////}


        //////[SessionManager("Registro Departamentos")]
        ////public async Task<IActionResult> EditDepartamento(int id)
        ////{
        ////    DepartamentoViewModel model = new DepartamentoViewModel();
        ////    //Boolean validation = validation.Controller();
        ////    if (ModelState.IsValid)
        ////    {
        ////        if (id == 0)
        ////        {
        ////            model.EditarDepartamento.depto_Id = id;
        ////        }
        ////        else
        ////        {
        ////            var result = await _departamentoService.FindAsync(id);
        ////            if (result == null)
        ////            {
        ////                return View(nameof(Index));
        ////            }
        ////            model.EditarDepartamento = _mapper.Map<EditarDepartamento>(result);
        ////        }
        ////        return View(nameof(EditDepartamento), model);
        ////    }
        ////    else
        ////    {
        ////        return View(nameof(EditDepartamento), model);
        ////    }
        ////}

        //////[SessionManager("Listado Departamentos")]
        ////[AcceptVerbs("GET", "POST")]
        ////public async Task<IActionResult> ExistDepartamentoCodigo(int depto_Id, string depto_Codigo)
        ////{
        ////    var result = await _departamentoService.ExistCodigo(depto_Codigo);

        ////    //if (ModelState.IsValid)
        ////    //{
        ////    if (result != null)
        ////    {
        ////        if (result.depto_Id == depto_Id)
        ////        {
        ////            return Json(true);
        ////        }
        ////        else
        ////        {
        ////            return Json($"Registro existente");
        ////        }
        ////    }
        ////    return Json(true);
        ////    //}
        ////}

        //////[SessionManager("Listado Departamentos")]
        ////public async Task<IActionResult> ExistDepartamentoDescripcion(int depto_Id, string depto_Descripcion)
        ////{
        ////    var result = await _departamentoService.ExistDescripcion(depto_Descripcion.Trim());
        ////    if (result != null)
        ////    {
        ////        if (result.depto_Id == depto_Id)
        ////        {
        ////            return Json(true);
        ////        }
        ////        else
        ////        {
        ////            return Json($"Registro existente");
        ////        }
        ////    }
        ////    return Json(true);
        ////}

        ////#endregion Departamento


        //#region Municipio
        ////[SessionManager("Listado Municipio")]
        //public async Task<IActionResult> ListMunicipio(int id)
        //{
        //    //var result = _MunicipioService.List().Where(x => x.depto_Id == id);
        //    //return Json(new
        //    //{
        //    //    data = listMunicipio
        //    //});


        //    var result = await _MunicipioService.List().Where(x => x.depto_Id == id);
        //    if (result != null)
        //    {
        //        ShowAlert("Insertado", AlertMessageType.Success);
        //        return Json(new { data = result });
        //    }
        //    else
        //    {
        //        ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
        //        return RedirectToAction("Index");
        //    }
        //}

        ////[SessionManager("Listado Municipio")]
        //public async Task<IActionResult> ListMunicipioDetalle(int id)
        //{
        //    var listMunicipio = await _MunicipioService.FindAsync(id);
        //    return Json(new { data = listMunicipio });
        //}


        ////[SessionManager("Modificar Departamentos")]
        //public async Task<IActionResult> EditDepartamento()
        //{
        //    if (ModelState.IsValid)
        //    {

        //    }
        //}



        ////[SessionManager("Registro Municipio")]
        //public async Task<IActionResult> Acciones(string codigo, string codigoDept, int mpio_Id, int idDep, string municipio)
        //{
        //    var mpio_Codigo = codigoDept + codigo;
        //    var result = 0;
        //    //var id = _httpContextAccessor.HttpContext.Session.GetInt32("idUsuario");
        //    var validacio = _MunicipioService.MunicipioExist(mpio_Codigo);
        //    if (mpio_Id == 0)
        //    {
        //        if (validacio == null)
        //        {
        //            result = _MunicipioService.AddAsync(mpio_Codigo, municipio.Trim(), idDep, (int)id);
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
        //            result = _MunicipioService.Edit(mpio_Id, mpio_Codigo, municipio.Trim(), idDep, (int)id);
        //        }
        //        else
        //        {
        //            return Json("Registro existente");
        //        }
        //    }
        //    return Json(result);
        //}

        //public async Task<IActionResult> Delete(int id)
        //{
        //    var dependencyValidation = _departamentoService.ValidationMunicipio(id);
        //    if (dependencyValidation.Count() == 0)
        //    {
        //        var result = _departamentoService.RemoveAsync(id);
        //        return Json(result);
        //    }
        //    return Json(dependencyValidation.Count());
        //}

        ////[SessionManager("Listado Municipio")]
        //public IActionResult GetMunicipio(int id)
        //{
        //    var municipio = _MunicipioService.ListMunicipio().FirstOrDefault(x => x.mpio_Id == id);
        //    return AjaxResult(municipio, true);
        //}

        ////[SessionManager("Listado Departamentos")]
        //public async Task<IActionResult> DepartamentosDetail(int id)
        //{
        //    var departamento = _departamentoService.FindAsync(id);
        //    if (departamento == null)
        //    {
        //        return View(nameof(Index));
        //    }
        //    var municipio = _MunicipioService.FindAsync(id);
        //    var listado = new List<MunicipioViewModel>();
        //    if (municipio == null)
        //    {
        //        return View(nameof(Index));
        //    }
        //    var model = new DepartamentoViewModel();
        //    model.EditarDepartamento.depto_Id = id;
        //    model.EditarDepartamento.depto_Codigo = departamento.depto_Codigo;
        //    model.EditarDepartamento.depto_Descripcion = departamento.depto_Descripcion;
        //    model.EditarDepartamento.depto_UsuarioCrea = departamento.depto_UsuarioCrea;
        //    model.EditarDepartamento.UsuarioCreacion = departamento.UsuarioCreacion;
        //    model.EditarDepartamento.depto_FechaCrea = departamento.depto_FechaCrea;
        //    model.EditarDepartamento.depto_UsuarioModifica = departamento.depto_UsuarioModifica;
        //    model.EditarDepartamento.UsuarioModificacion = departamento.UsuarioModificacion;
        //    model.EditarDepartamento.depto_FechaModifica = departamento.depto_FechaModifica;
        //    foreach (var item in municipio)
        //    {
        //        listado.AddAsync(new EditarMunicipio()
        //        {
        //            mpio_Codigo = item.mpio_Codigo,
        //            mpio_Descripcion = item.mpio_Descripcion,
        //            mpio_UsuarioCrea = item.mpio_UsuarioCrea,
        //            UsuarioCreacion = item.UsuarioCreacion,
        //            mpio_FechaCrea = item.mpio_FechaCrea,
        //            mpio_UsuarioModifica = item.mpio_UsuarioModifica,
        //            UsuarioModificacion = item.UsuarioModificacion,
        //            mpio_FechaModifica = item.mpio_FechaModifica
        //        });
        //    }
        //    model.ListadoMunicipio = listado;
        //    return View(nameof(DetailDepartamento), model);
        //}

        ////[SessionManager("Listado Departamentos")]
        //public async Task<IActionResult> DetailDepartamento()
        //{
        //    return View();
        //}

        ////[SessionManager("Modificar Municipio")]
        //public async Task<IActionResult> DeleteMunicipio(int id)
        //{
        //    var validationPersonas = _MunicipioService.ValidationPersonas(id);
        //    var validationSucursales = _MunicipioService.ValidationSucursales(id);
        //    var validationProveedores = _MunicipioService.ValidationProveedores(id);
        //    var validationTotal = validationPersonas.Count() + validationSucursales.Count() + validationProveedores.Count();
        //    if (validationTotal == 0)
        //    {
        //        var result = _MunicipioService.Delete(id);
        //        return Json(result);
        //    }
        //    return Json(validationTotal);
        //}


        //public IEnumerable<tbDepartamentos> ValidationMunicipios(int id)
        //{
        //    const string query = @"UDP_Gral_tbDepartamentos_DependencyMunicipios";
        //    var parameters = new DynamicParameters();
        //    parameters.AddAsync("@depto_Id", id, DbType.Int32, ParameterDirection.Input);
        //    using (var db = new SqlConnection(InstaHelpDbContext.ConnectionString))
        //    {
        //        var result = db.Query<tbDepartamentos>(query, parameters, commandType: CommandType.StoredProcedure).ToList();
        //        return result;
        //    }
        //}

        //public IEnumerable<UDP_Gral_tbDepartamentos_FindResult> DetailsDepartamentos()
        //{
        //    const string query = @"UDP_Gral_tbDepartamentos_Find";
        //    var parameters = new DynamicParameters();
        //    using (var db = new SqlConnection(InstaHelpDbContext.ConnectionString))
        //    {
        //        var resultado = db.Query<UDP_Gral_tbDepartamentos_FindResult>(query, parameters, commandType: CommandType.StoredProcedure);
        //        return resultado;
        //    }
        //}

        //#endregion Municipio

    }
}
