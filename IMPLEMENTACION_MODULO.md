# Guía de Implementación de Módulos en PetsHome

Esta guía proporciona los pasos exactos para implementar un nuevo módulo completo en el sistema PetsHome, siguiendo la arquitectura de 5 capas y la estructura estándar utilizada en módulos como Mascotas y Tratamiento.

## 📋 Pre-requisitos

Antes de comenzar la implementación del código, asegúrate de que la base de datos esté lista:

1. ✅ La tabla principal existe en la base de datos
2. ✅ Los 5 stored procedures principales están creados:
   - `PR_[Schema]_[Tabla]_List` - Listar registros
   - `PR_[Schema]_[Tabla]_Find` - Buscar por ID para edición
   - `PR_[Schema]_[Tabla]_Detail` - Obtener detalles con auditoría
   - `PR_[Schema]_[Tabla]_Insert` - Insertar nuevo registro
   - `PR_[Schema]_[Tabla]_Update` - Actualizar registro existente
   - `PR_[Schema]_[Tabla]_Delete` - Eliminar registro (borrado lógico)
3. ✅ Los stored procedures para dropdowns están creados (si aplica)

---

## 🏗️ Estructura de Implementación

Sigue estos pasos en orden para crear un módulo completo:

### PASO 1: Common Layer - Entidades

**Ubicación**: `PetsHome.Common/Entities/[Schema]/`

#### 1.1. Crear o verificar la entidad principal

**Archivo**: `tb[NombreTabla].cs`

```csharp
using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tb[NombreTabla]
    {
        // Propiedades principales
        public int [tabla]_Id { get; set; }
        public string [tabla]_Campo1 { get; set; }
        public string [tabla]_Campo2 { get; set; }
        // ... más campos según la tabla

        // Propiedades de auditoría
        public bool [tabla]_EsEliminado { get; set; }
        public int [tabla]_UsuarioCrea { get; set; }
        public DateTime [tabla]_FechaCrea { get; set; }
        public int? [tabla]_UsuarioModifica { get; set; }
        public DateTime? [tabla]_FechaModifica { get; set; }

        // Navegación (relaciones con otras entidades)
        public virtual tbUsuarios [tabla]_UsuarioCreaNavigation { get; set; }
        public virtual tbOtraEntidad OtraEntidad { get; set; }
    }
}
```

#### 1.2. Crear clases Result para stored procedures

**Archivos a crear**:

**a) `PR_[Schema]_[Tabla]_ListResult.cs`**
```csharp
using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_[Schema]_[Tabla]_ListResult
    {
        public int Fila { get; set; }
        public int [tabla]_Id { get; set; }
        public string [tabla]_Campo1 { get; set; }
        public string [tabla]_Campo2 { get; set; }
        // Campos adicionales que devuelve el SP de List
    }
}
```

**b) `PR_[Schema]_[Tabla]_FindResult.cs`**
```csharp
using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_[Schema]_[Tabla]_FindResult
    {
        public int [tabla]_Id { get; set; }
        public string [tabla]_Campo1 { get; set; }
        public string [tabla]_Campo2 { get; set; }
        // Todos los campos editables

        // Información de auditoría
        public int [tabla]_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime [tabla]_FechaCrea { get; set; }
        public int? [tabla]_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? [tabla]_FechaModifica { get; set; }
    }
}
```

**c) `PR_[Schema]_[Tabla]_DetailResult.cs`**
```csharp
using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_[Schema]_[Tabla]_DetailResult
    {
        public int [tabla]_Id { get; set; }
        public string [tabla]_Campo1 { get; set; }
        public string [tabla]_Campo2 { get; set; }
        // Campos con nombres descriptivos (joins)

        // Información de auditoría
        public string UsuarioCreacion { get; set; }
        public DateTime [tabla]_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? [tabla]_FechaModifica { get; set; }
    }
}
```

**d) `PR_[Schema]_[Tabla]_DropdownResult.cs` (opcional)**
```csharp
namespace PetsHome.Common.Entities
{
    public partial class PR_[Schema]_[Tabla]_DropdownResult
    {
        public int [tabla]_Id { get; set; }
        public string [tabla]_Descripcion { get; set; }
    }
}
```

---

### PASO 2: Logic Layer - Repository

**Ubicación**: `PetsHome.Logic/Repositories/`

**Archivo**: `[NombreTabla]Repository.cs`

```csharp
using Dapper;
using Microsoft.Data.SqlClient;
using PetsHome.Common.Entities;
using PetsHome.DataAccess;
using PetsHome.DataAccess.Extensions;
using PetsHome.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PetsHome.Logic.Repositories
{
    public class [NombreTabla]Repository : IGenericRepository<tb[NombreTabla]>
    {
        #region Consultas

        public async Task<IEnumerable<PR_[Schema]_[Tabla]_ListResult>> ListAsync()
        {
            const string sqlQuery = "[Schema].[PR_[Schema]_[Tabla]_List]";
            return await DbApp.Select<PR_[Schema]_[Tabla]_ListResult>(sqlQuery);
        }

        public async Task<PR_[Schema]_[Tabla]_FindResult> FindAsync(int id)
        {
            const string sqlQuery = "[Schema].[PR_[Schema]_[Tabla]_Find]";
            var parameter = new DynamicParameters();
            parameter.Add("@[tabla]_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Find<PR_[Schema]_[Tabla]_FindResult>(sqlQuery, parameter);
        }

        public async Task<PR_[Schema]_[Tabla]_DetailResult> DetailAsync(int id)
        {
            const string sqlQuery = "[Schema].[PR_[Schema]_[Tabla]_Detail]";
            var parameter = new DynamicParameters();
            parameter.Add("@[tabla]_Id", id, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Detail<PR_[Schema]_[Tabla]_DetailResult>(sqlQuery, parameter);
        }

        public async Task<Boolean> AddAsync(tb[NombreTabla] entity)
        {
            entity.[tabla]_UsuarioCrea = 1;
            const string sqlQuery = "[Schema].[PR_[Schema]_[Tabla]_Insert]";
            var parameter = new DynamicParameters();
            parameter.Add("@[tabla]_Campo1", entity.[tabla]_Campo1, DbType.String, ParameterDirection.Input);
            parameter.Add("@[tabla]_Campo2", entity.[tabla]_Campo2, DbType.String, ParameterDirection.Input);
            // Agregar todos los parámetros necesarios
            parameter.Add("@[tabla]_UsuarioCrea", entity.[tabla]_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Insert(sqlQuery, parameter);
        }

        public async Task<Boolean> EditAsync(tb[NombreTabla] entity)
        {
            entity.[tabla]_UsuarioModifica = 1;
            const string sqlQuery = "[Schema].[PR_[Schema]_[Tabla]_Update]";
            var parameter = new DynamicParameters();
            parameter.Add("@[tabla]_Id", entity.[tabla]_Id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@[tabla]_Campo1", entity.[tabla]_Campo1, DbType.String, ParameterDirection.Input);
            parameter.Add("@[tabla]_Campo2", entity.[tabla]_Campo2, DbType.String, ParameterDirection.Input);
            // Agregar todos los parámetros necesarios
            parameter.Add("@[tabla]_UsuarioModifica", entity.[tabla]_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Update(sqlQuery, parameter);
        }

        public async Task<Boolean> RemoveAsync(int id)
        {
            const string sqlQuery = "[Schema].[PR_[Schema]_[Tabla]_Delete]";
            var parameter = new DynamicParameters();
            parameter.Add("@[tabla]_Id", id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@[tabla]_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
            return await DbApp.Delete(sqlQuery, parameter);
        }

        #endregion

        #region Dropdown (opcional)

        public IEnumerable<PR_[Schema]_[Tabla]_DropdownResult> Dropdown()
        {
            const string query = "[Schema].[PR_[Schema]_[Tabla]_Dropdown]";
            using (var db = new SqlConnection(PetsHomeDbContext.ConnectionString))
            {
                var result = db.Query<PR_[Schema]_[Tabla]_DropdownResult>(query, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        #endregion
    }
}
```

---

### PASO 3: Business Layer - ViewModels

**Ubicación**: `PetsHome.Business/Models/`

#### 3.1. Crear ViewModel para Listado

**Archivo**: `[NombreTabla]ListViewModel.cs`

```csharp
using System;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo utilizado para representar los registros del listado de [tabla].
    /// </summary>
    public class [NombreTabla]ListViewModel
    {
        public int Fila { get; set; }
        public int [tabla]_Id { get; set; }
        public string [tabla]_Campo1 { get; set; }
        public string [tabla]_Campo2 { get; set; }
        // Propiedades que se muestran en el DataTable
    }
}
```

#### 3.2. Crear ViewModel para Formulario

**Archivo**: `[NombreTabla]FormViewModel.cs`

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo utilizado para crear o editar [tabla].
    /// </summary>
    public class [NombreTabla]FormViewModel
    {
        [Key]
        [Display(Name = "Id [tabla]")]
        public int [tabla]_Id { get; set; }

        [Display(Name = "Campo 1")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        public string [tabla]_Campo1 { get; set; }

        [Display(Name = "Campo 2")]
        public string [tabla]_Campo2 { get; set; }

        // Más propiedades editables

        // Propiedades de auditoría
        public int [tabla]_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string [tabla]_NombreUsuarioCrea { get; set; }

        public DateTime [tabla]_FechaCrea { get; set; }

        public int? [tabla]_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string [tabla]_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? [tabla]_FechaModifica { get; set; }

        public bool isEdit => [tabla]_Id != 0;

        #region Dropdown

        public SelectList EntidadRelacionadaList { get; set; }

        public void LoadDropDownList(
            IEnumerable<EntidadViewModel> entidadViewModels)
        {
            EntidadRelacionadaList = new SelectList(entidadViewModels, "entidad_Id", "entidad_Descripcion");
        }

        #endregion
    }
}
```

#### 3.3. Crear ViewModel para Detalles

**Archivo**: `[NombreTabla]DetailsViewModel.cs`

```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo utilizado para mostrar el detalle de [tabla].
    /// </summary>
    public class [NombreTabla]DetailsViewModel
    {
        [Key]
        [Display(Name = "Id [tabla]")]
        public int [tabla]_Id { get; set; }

        [Display(Name = "Campo 1")]
        public string [tabla]_Campo1 { get; set; }

        [Display(Name = "Campo 2")]
        public string [tabla]_Campo2 { get; set; }

        // Más propiedades de solo lectura con nombres descriptivos

        [Display(Name = "Usuario creación")]
        public string UsuarioCreacion { get; set; }

        [Display(Name = "Fecha creación")]
        public DateTime [tabla]_FechaCrea { get; set; }

        [Display(Name = "Usuario modificación")]
        public string UsuarioModificacion { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? [tabla]_FechaModifica { get; set; }
    }
}
```

---

### PASO 4: Business Layer - Service

**Ubicación**: `PetsHome.Business/Services/`

**Archivo**: `[NombreTabla]Service.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using PetsHome.Business.Models;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;

namespace PetsHome.Business.Services
{
    /// <summary>
    /// Servicio que gestiona [tabla].
    /// </summary>
    public class [NombreTabla]Service
    {
        private readonly [NombreTabla]Repository _[nombreTabla]Repository;
        private readonly ILogger<[NombreTabla]Service> _logger;
        private readonly IMapper _mapper;

        public [NombreTabla]Service(
            [NombreTabla]Repository [nombreTabla]Repository,
            ILogger<[NombreTabla]Service> logger,
            IMapper mapper)
        {
            _[nombreTabla]Repository = [nombreTabla]Repository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene una lista de todos los registros.
        /// </summary>
        public async Task<List<[NombreTabla]ListViewModel>> ListAsync()
        {
            try
            {
                IEnumerable<PR_[Schema]_[Tabla]_ListResult> mappedResult =
                    await _[nombreTabla]Repository.ListAsync();
                return _mapper.Map<List<[NombreTabla]ListViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Busca un registro por su identificador para su edición.
        /// </summary>
        public async Task<[NombreTabla]FormViewModel> FindAsync(int id)
        {
            try
            {
                PR_[Schema]_[Tabla]_FindResult mappedResult =
                    await _[nombreTabla]Repository.FindAsync(id);
                return _mapper.Map<[NombreTabla]FormViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Obtiene los detalles de un registro por su identificador.
        /// </summary>
        public async Task<[NombreTabla]DetailsViewModel> DetailAsync(int id)
        {
            try
            {
                PR_[Schema]_[Tabla]_DetailResult mappedResult =
                    await _[nombreTabla]Repository.DetailAsync(id);
                return _mapper.Map<[NombreTabla]DetailsViewModel>(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        /// <summary>
        /// Agrega un nuevo registro.
        /// </summary>
        public async Task<bool> AddAsync([NombreTabla]FormViewModel model)
        {
            try
            {
                tb[NombreTabla] mappedResult = _mapper.Map<tb[NombreTabla]>(model);
                return await _[nombreTabla]Repository.AddAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Actualiza un registro existente.
        /// </summary>
        public async Task<bool> UpdateAsync([NombreTabla]FormViewModel model)
        {
            try
            {
                tb[NombreTabla] mappedResult = _mapper.Map<tb[NombreTabla]>(model);
                return await _[nombreTabla]Repository.EditAsync(mappedResult);
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        /// <summary>
        /// Elimina un registro por su identificador.
        /// </summary>
        public async Task<bool> RemoveAsync(int id)
        {
            try
            {
                bool mappedResult = await _[nombreTabla]Repository.RemoveAsync(id);
                return mappedResult;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return true;
            }
        }

        #region Dropdown (opcional)

        public IEnumerable<[NombreTabla]ViewModel> [NombreTabla]Dropdown()
        {
            try
            {
                IEnumerable<PR_[Schema]_[Tabla]_DropdownResult> mappedResult =
                    _[nombreTabla]Repository.Dropdown();
                return _mapper.Map<List<[NombreTabla]ViewModel>>(mappedResult.ToList());
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return null;
            }
        }

        #endregion
    }
}
```

---

### PASO 5: Business Layer - AutoMapper

**Ubicación**: `PetsHome.Business/Extensions/MappingProfileExtensions.cs`

**Agregar dentro del constructor de la clase**:

```csharp
// Mapeos para [NombreTabla] - ViewModels separados
CreateMap<PR_[Schema]_[Tabla]_ListResult, [NombreTabla]ListViewModel>().ReverseMap();

CreateMap<PR_[Schema]_[Tabla]_DetailResult, [NombreTabla]DetailsViewModel>()
    .ForMember(dest => dest.UsuarioCreacion, opt => opt.MapFrom(src => src.UsuarioCreacion))
    .ForMember(dest => dest.UsuarioModificacion, opt => opt.MapFrom(src => src.UsuarioModificacion));

CreateMap<PR_[Schema]_[Tabla]_FindResult, [NombreTabla]FormViewModel>()
    .ForMember(dest => dest.[tabla]_NombreUsuarioCrea, opt => opt.MapFrom(src => src.usuarioCrea))
    .ForMember(dest => dest.[tabla]_NombreUsuarioModifica, opt => opt.MapFrom(src => src.usuarioModifica));

CreateMap<tb[NombreTabla], [NombreTabla]FormViewModel>().ReverseMap();

// Si hay dropdown
CreateMap<PR_[Schema]_[Tabla]_DropdownResult, [NombreTabla]ViewModel>().ReverseMap();
```

---

### PASO 6: UI Layer - Controller

**Ubicación**: `PetsHome.UI/Controllers/`

**Archivo**: `[NombreTabla]Controller.cs`

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetsHome.Business.Data;
using PetsHome.Business.Extensions;
using PetsHome.Business.Helpers;
using PetsHome.Business.Models;
using PetsHome.Business.Services;

namespace PetsHome.UI.Controllers
{
    public class [NombreTabla]Controller : BaseController
    {
        private readonly [NombreTabla]Service _[nombreTabla]Service;
        // Agregar servicios para dropdowns si es necesario

        public [NombreTabla]Controller([NombreTabla]Service [nombreTabla]Service)
        {
            _[nombreTabla]Service = [nombreTabla]Service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var model = new [NombreTabla]FormViewModel();
            var drop = Dropdown(model);
            return View(drop);
        }

        public async Task<IActionResult> List()
        {
            var itemListing = await _[nombreTabla]Service.ListAsync();
            return Json(new { data = itemListing });
        }

        public async Task<IActionResult> Find(int id)
        {
            if (id != 0)
            {
                var itemSearched = await _[nombreTabla]Service.FindAsync(id);
                var dropdown = Dropdown(itemSearched);
                return View("Create", dropdown);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id != 0)
            {
                var itemDetail = await _[nombreTabla]Service.DetailAsync(id);
                if (itemDetail == null)
                {
                    ShowAlert("Registro no encontrado", AlertMessageType.Error);
                    return RedirectToAction("Index");
                }
                return View("Details", itemDetail);
            }
            else
            {
                ShowAlert(AlertMessaje.Error, AlertMessageType.Error);
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Add([NombreTabla]FormViewModel model)
        {
            if (!model.isEdit)
            {
                bool createdItem = await _[nombreTabla]Service.AddAsync(model);
                bool validation = Validation.IsInsert(createdItem, ModelState.IsValid);
                if (createdItem)
                    goto ErrorResult;
                ShowAlert("Insertado", AlertMessageType.Success);
                return RedirectToAction("Create");
            }
            else
            {
                bool updatedItem = await _[nombreTabla]Service.UpdateAsync(model);
                bool validation = Validation.IsUpdate(updatedItem, ModelState.IsValid);
                if (updatedItem)
                    goto ErrorResult;

                ShowAlert("Actualizado", AlertMessageType.Success);
                return View("Index");
            }

        ErrorResult:
            return ShowAlert(AlertMessaje.Error, AlertMessageType.Error, model);
        }

        public async Task<IActionResult> Remove(int [tabla]_Id)
        {
            bool deletedItem = await _[nombreTabla]Service.RemoveAsync([tabla]_Id);
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
        public [NombreTabla]FormViewModel Dropdown([NombreTabla]FormViewModel model)
        {
            model.LoadDropDownList(
                // Agregar llamadas a servicios de dropdown
            );
            return model;
        }
    }
}
```

---

### PASO 7: UI Layer - Vistas

**Ubicación**: `PetsHome.UI/Views/[NombreTabla]/`

#### 7.1. Vista Index (Listado)

**Archivo**: `Index.cshtml`

```cshtml
@model [NombreTabla]ListViewModel
@{
    ViewData["Title"] = "[Nombre Tabla]";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

@section Styles {
    <link rel="stylesheet" href="~/css/modern-table-styles.css" />
}

<div class="col-12">
    <div class="pets-container">
    <!-- Header púrpura con todos los controles -->
    <div class="pets-header-full">
        <div class="pets-header-content">
            <div class="pets-title-section">
                <h2 class="text-white"><i class="fas fa-icon mr-2 text-white"></i>Gestión de [Nombre Tabla]</h2>
                <button class="btn-nueva-mascota" onclick="window.location.href='@Url.Action("Create")'">
                    <i class="fas fa-plus mr-2"></i>Nuevo Registro
                </button>
            </div>

            <div class="pets-controls">
                <div class="search-container">
                    <i class="fas fa-search"></i>
                    <input type="text" id="globalSearch" placeholder="Buscar...">
                </div>
                <div class="action-buttons">
                    <button class="btn-export-datatable" onclick="location.reload()">
                        Recargar
                    </button>
                    <div id="export-buttons-container" class="export-buttons-container">
                        <!-- Los botones de DataTable se moveran aqui -->
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Vista de Tabla -->
    <div id="tableView" class="pets-table-container">
        <table class="pets-table" id="datatable">
            <thead>
                <tr>
                    <th width="60">ID</th>
                    <th>Campo 1</th>
                    <th>Campo 2</th>
                    <th>Campo 3</th>
                    <th width="150" class="text-center">Acciones</th>
                </tr>
            </thead>
            <tbody>
                <!-- DataTable cargará los datos aquí -->
            </tbody>
        </table>
    </div>

    <!-- Modal Delete -->
    <div class="modal fade" id="delete-modal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        <i class="fas fa-exclamation-triangle mr-2"></i>Confirmar Eliminación
                    </h5>
                    <button type="button" class="close text-white" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <form asp-action="Remove" method="post" autocomplete="off"
                      data-ajax-begin="Catalogs.begin"
                      data-ajax-success="Catalogs.success"
                      data-ajax-failure="Catalogs.failure"
                      data-ajax-complete="Catalogs.complete">
                    <input type="hidden" asp-for="[tabla]_Id" id="delete-item-id">
                    <div class="modal-body text-center py-4">
                        <div class="mb-3">
                            <i class="fas fa-trash-alt text-danger" style="font-size: 48px;"></i>
                        </div>
                        <h5>¿Está seguro de eliminar este registro?</h5>
                        <p class="text-muted">Esta acción no se puede deshacer.</p>
                    </div>
                    <div class="modal-footer border-0">
                        <button type="button" class="btn btn-secondary px-4" data-dismiss="modal">
                            <i class="fas fa-times mr-2"></i>Cancelar
                        </button>
                        <button type="submit" class="btn btn-danger px-4">
                            <i class="fas fa-trash mr-2"></i>Eliminar
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>
</div>

@section Scripts {
<script src="~/js/components/datatable/datatable.init.js"></script>
<script src="~/js/pages/[nombreTabla].js"></script>
<script>
    // Configuración del DataTable
    [NombreTabla].datatable({
        urlList: "@Url.Action("List")",
        urlDetail: "@Url.Action("Detail")",
        urlInsert: "@Url.Action("Create")",
        urlUpdate: "@Url.Action("Find")"
    });

    // Búsqueda global
    $('#globalSearch').on('keyup', function() {
        $('#datatable').DataTable().search(this.value).draw();
    });

    // Función global para eliminar
    function delete[NombreTabla](id) {
        $('#delete-item-id').val(id);
        $('#delete-modal').modal('show');
    }
</script>
}
```

#### 7.2. Vista Create (Formulario)

**Archivo**: `Create.cshtml`

```cshtml
@model [NombreTabla]FormViewModel
@{
    ViewData["Title"] = "Registro";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

@section Styles {
    <link rel="stylesheet" href="~/css/modern-form-styles.css" />
    <link rel="stylesheet" href="~/css/modern-table-styles.css" />
}

<div class="col-12">
    <div class="pets-container">
        <!-- Header con gradiente -->
        <div class="pets-header-full">
            <div class="pets-header-content">
                <div class="pets-title-section">
                    <h2 class="text-white">
                        <i class="fas fa-icon mr-2 text-white"></i>Registro de [Nombre Tabla]
                    </h2>
                    <button class="btn-nueva-mascota" onclick="window.location.href='@Url.Action("Index")'">
                        <i class="fas fa-arrow-left mr-2"></i>Volver al Listado
                    </button>
                </div>
                <p class="text-white mb-0 mt-2" style="opacity: 0.9;">Complete la información del registro</p>
            </div>
        </div>

        <!-- Contenedor del formulario -->
        <div class="pets-body-full">
            <div class="container-fluid">

                <form asp-action="Add" asp-controller="[NombreTabla]" method="post" autocomplete="ON" id="[nombreTabla]Form">
                    <input class="form-control" asp-for="[tabla]_Id" type="hidden" />

                    <!-- Datos Principales -->
                    <div class="card">
                        <div class="card-header">
                            <h2 class="card-title">Datos principales</h2>
                            <p class="card-description">Información del registro</p>
                        </div>
                        <div class="card-content">
                            <div class="form-grid">
                                <!-- Campo 1 -->
                                <div class="form-group">
                                    <label asp-for="[tabla]_Campo1">Campo 1</label>
                                    <input class="form-control" asp-for="[tabla]_Campo1" type="text" placeholder="Ingrese..." />
                                    <span class="text-danger" asp-validation-for="[tabla]_Campo1"></span>
                                </div>

                                <!-- Campo 2 -->
                                <div class="form-group">
                                    <label asp-for="[tabla]_Campo2">Campo 2</label>
                                    <input class="form-control" asp-for="[tabla]_Campo2" type="text" placeholder="Ingrese..." />
                                    <span class="text-danger" asp-validation-for="[tabla]_Campo2"></span>
                                </div>

                                <!-- Dropdown (si aplica) -->
                                <div class="form-group">
                                    <label asp-for="entidad_Id">Entidad Relacionada</label>
                                    <div class="select-wrapper">
                                        <select class="form-control" asp-for="entidad_Id" asp-items="Model.EntidadRelacionadaList">
                                            <option value="">-- Seleccionar --</option>
                                        </select>
                                    </div>
                                    <span class="text-danger" asp-validation-for="entidad_Id"></span>
                                </div>

                                <!-- Agregar más campos según sea necesario -->
                            </div>
                        </div>
                    </div>

                    <!-- Botones de Acción -->
                    <div class="form-actions">
                        <a class="btn btn-outline" asp-action="Index">Cancelar</a>
                        <button type="submit" class="btn btn-primary"><i class="mdi mdi-content-save"></i> Guardar</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>

@section Scripts{
    <script src="~/js/load-dropdownlist.js"></script>
}
```

#### 7.3. Vista Details

**Archivo**: `Details.cshtml`

```cshtml
@model [NombreTabla]DetailsViewModel
@{
    ViewData["Title"] = "Detalle";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

@section Styles {
    <link rel="stylesheet" href="~/css/modern-form-styles.css" />
    <link rel="stylesheet" href="~/css/modern-table-styles.css" />
}

<div class="col-12">
    <div class="pets-container">
        <!-- Header con gradiente -->
        <div class="pets-header-full">
            <div class="pets-header-content">
                <div class="pets-title-section">
                    <h2 class="text-white">
                        <i class="fas fa-icon mr-2 text-white"></i>Detalle de [Nombre Tabla]
                    </h2>
                    <button class="btn-nueva-mascota" onclick="window.location.href='@Url.Action("Index")'">
                        <i class="fas fa-arrow-left mr-2"></i>Volver al Listado
                    </button>
                </div>
                <p class="text-white mb-0 mt-2" style="opacity: 0.9;">Información completa del registro #@Model.[tabla]_Id</p>
            </div>
        </div>

        <!-- Contenedor de detalles -->
        <div class="pets-body-full">
            <div class="container-fluid">

                <!-- Información Principal -->
                <div class="card">
                    <div class="card-header">
                        <h2 class="card-title">Información principal</h2>
                        <p class="card-description">Datos del registro</p>
                    </div>
                    <div class="card-content">
                        <dl class="row">
                            <dt class="col-sm-3">@Html.DisplayNameFor(model => model.[tabla]_Campo1)</dt>
                            <dd class="col-sm-9">@Html.DisplayFor(model => model.[tabla]_Campo1)</dd>

                            <dt class="col-sm-3">@Html.DisplayNameFor(model => model.[tabla]_Campo2)</dt>
                            <dd class="col-sm-9">@Html.DisplayFor(model => model.[tabla]_Campo2)</dd>

                            <!-- Agregar más campos -->
                        </dl>
                    </div>
                </div>

                <!-- Información de Auditoría -->
                <div class="card">
                    <div class="card-header">
                        <h2 class="card-title">
                            <i class="fas fa-info-circle mr-2"></i>Información de auditoría
                        </h2>
                        <p class="card-description">Registro de creación y modificación</p>
                    </div>
                    <div class="card-content">
                        <dl class="row">
                            <dt class="col-sm-3">@Html.DisplayNameFor(model => model.UsuarioCreacion)</dt>
                            <dd class="col-sm-9">@Html.DisplayFor(model => model.UsuarioCreacion)</dd>

                            <dt class="col-sm-3">@Html.DisplayNameFor(model => model.[tabla]_FechaCrea)</dt>
                            <dd class="col-sm-9">@Html.DisplayFor(model => model.[tabla]_FechaCrea)</dd>

                            @if (!string.IsNullOrEmpty(Model.UsuarioModificacion))
                            {
                                <dt class="col-sm-3">@Html.DisplayNameFor(model => model.UsuarioModificacion)</dt>
                                <dd class="col-sm-9">@Html.DisplayFor(model => model.UsuarioModificacion)</dd>

                                <dt class="col-sm-3">@Html.DisplayNameFor(model => model.[tabla]_FechaModifica)</dt>
                                <dd class="col-sm-9">@Html.DisplayFor(model => model.[tabla]_FechaModifica)</dd>
                            }
                        </dl>
                    </div>
                </div>

                <!-- Botones de Acción -->
                <div class="form-actions">
                    <a class="btn btn-outline" asp-action="Index">
                        <i class="fas fa-arrow-left mr-2"></i>Volver
                    </a>
                    <a class="btn btn-primary" asp-action="Find" asp-route-id="@Model.[tabla]_Id">
                        <i class="fas fa-edit mr-2"></i>Editar
                    </a>
                </div>
            </div>
        </div>
    </div>
</div>
```

---

### PASO 8: UI Layer - JavaScript

**Ubicación**: `PetsHome.UI/wwwroot/js/pages/`

**Archivo**: `[nombreTabla].js`

```javascript
var [NombreTabla] = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: '[tabla]_Id',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: '[tabla]_Campo1',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + (data || 'N/A') + '</div>';
                    }
                },
                {
                    FieldName: '[tabla]_Campo2',
                    Visibility: true
                },
                {
                    FieldName: '[tabla]_Campo3',
                    Visibility: true
                }
                // Agregar más columnas según sea necesario
            ];

            // Inicializar datatable
            datatable.init(Direction, header);
        });
    }

    return obj;

}());

// Función global para eliminar
function delete[NombreTabla](id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
```

---

### PASO 9: Registrar Servicios (si no están registrados)

**Ubicación**: `PetsHome.Business/ServiceConfiguration.cs`

En el método `AddBusinessLogic()`:

```csharp
// Registrar Repository
services.AddScoped<[NombreTabla]Repository>();

// Registrar Service
services.AddScoped<[NombreTabla]Service>();
```

---

## ✅ Checklist Final

Antes de probar, verifica que:

- [ ] Todos los archivos están creados en las ubicaciones correctas
- [ ] Los nombres de clases, variables y archivos siguen la convención
- [ ] Los using statements están completos en todos los archivos
- [ ] AutoMapper tiene todos los mapeos configurados
- [ ] Los servicios están registrados en ServiceConfiguration
- [ ] Las vistas tienen las referencias correctas a scripts y CSS
- [ ] El JavaScript tiene el nombre correcto del objeto
- [ ] Los stored procedures existen en la base de datos
- [ ] La tabla tiene los campos correctos

---

## 🧪 Pasos para Probar

1. **Compilar la solución**:
   ```bash
   dotnet build PetsHome.sln
   ```

2. **Ejecutar la aplicación**:
   ```bash
   dotnet run --project PetsHome.UI/PetsHome.UI.csproj
   ```

3. **Navegar** a `/[NombreTabla]` en el navegador

4. **Probar funcionalidades**:
   - ✅ Listado con DataTable
   - ✅ Búsqueda
   - ✅ Crear nuevo registro
   - ✅ Editar registro existente
   - ✅ Ver detalles
   - ✅ Eliminar registro
   - ✅ Exportar datos

---

## 📝 Notas Importantes

1. **Convención de Nombres**:
   - Tablas: `tb[NombreTabla]` (ej: `tbTratamientos`)
   - Campos: `[tabla]_[NombreCampo]` (ej: `trat_Medicamento`)
   - ViewModels: `[NombreTabla][Tipo]ViewModel` (ej: `TratamientoListViewModel`)
   - Repositories: `[NombreTabla]Repository`
   - Services: `[NombreTabla]Service`

2. **Archivos que requieren actualización manual**:
   - `ServiceConfiguration.cs` - Registrar servicios
   - `MappingProfileExtensions.cs` - Configurar AutoMapper
   - Dropdowns de entidades relacionadas

3. **Estilos CSS ya disponibles**:
   - `modern-table-styles.css` - Para tablas y listados
   - `modern-form-styles.css` - Para formularios

4. **Scripts JavaScript ya disponibles**:
   - `datatable.init.js` - Inicialización de DataTables
   - `load-dropdownlist.js` - Carga de dropdowns

---

## 🎯 Ejemplo Completo

Para ver un ejemplo completo funcionando, revisa los módulos:
- **Mascotas** (`Refugio.tbMascotas`)
- **Tratamiento** (`Medico.tbTratamientos`)

Ambos siguen exactamente esta estructura y pueden usarse como referencia.

---

**Última actualización**: 2025-01-06
**Autor**: Documentación del sistema PetsHome
