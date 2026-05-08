# PetsHome — Project Context
> Generado por `map_project.py` el 2026-04-24 23:50
> Leer este archivo al inicio de cada conversacion para evitar explorar el proyecto desde cero.

## Arquitectura General

- **Framework**: ASP.NET Core MVC (.NET Core)
- **ORM**: Dapper (sin EF Core) + Stored Procedures
- **BD**: SQL Server — base de datos `PETSHOMEDB`
- **Auth**: Cookie-based (8h), [Authorize] + [PantallaAuthorize]
- **Capas**:
  - `PetsHome.UI`         → Controllers, Views, Filters, Middleware
  - `PetsHome.Business`   → Services, Models (ViewModels), Extensions (AutoMapper)
  - `PetsHome.Logic`      → Repositories, Interfaces
  - `PetsHome.DataAccess` → PetsHomeDbContext (connection string), DbApp (helper)
  - `PetsHome.Common`     → Entities (SP result classes), InternalEntities

### Patron de acceso a datos (DbApp)
```csharp
// Lista sin params:      DbApp.Select<TResult>(sp)
// Lista con params:      DbApp.SelectById<TResult>(sp, dynamicParams)
// Primera fila:          DbApp.Find<TResult>(sp, params)
// Detalle:               DbApp.Detail<TResult>(sp, params)
// Escritura (retorna RequestResult): DbApp.ExecuteWithResult(sp, params)
// Dropdown sync:         DbApp.Dropdown<TResult>(sp)
```

### Autorizacion
```csharp
// A nivel de clase  → verifica que el usuario tenga la pantalla (consultar)
[PantallaAuthorize("Nombre de pantalla")]

// A nivel de metodo → verifica operacion especifica
[PantallaAuthorize("Nombre de pantalla", "insertar|editar|eliminar")]
```

### Schemas SQL
- `[Refugio]`    → Mascotas, Adopciones, Voluntarios, Eventos
- `[Medico]`     → CitaMedica, Recetas, Tratamientos, Catalogs medicos
- `[Inventario]` → Items, Recepciones, Existencias
- `[Seguridad]`  → Usuarios, Roles, Pantallas, RolesPantallas
- `[General]`    → Departamentos, Municipios

## Convenciones de Nomenclatura

### Archivos C#
| Tipo | Patron | Ejemplo |
|------|--------|---------|
| Controller    | `{Entity}Controller.cs`                | `CitaMedicaController.cs` |
| Service       | `{Entity}Service.cs`                   | `CitaMedicaService.cs` |
| Repository    | `{Entity}Repository.cs`                | `CitaMedicaRepository.cs` |
| SP Result     | `PR_{Schema}_{Entity}_{Op}Result.cs`   | `PR_Medico_CitaMedica_ListResult.cs` |
| ViewModel     | `{Entity}{Op}ViewModel.cs`             | `CitaMedicaFormViewModel.cs` |

### Stored Procedures
| Operacion | Patron |
|-----------|--------|
| List      | `[Schema].[PR_{Schema}_{Entity}_List]` |
| Find      | `[Schema].[PR_{Schema}_{Entity}_Find]` |
| Detail    | `[Schema].[PR_{Schema}_{Entity}_Detail]` |
| Insert    | `[Schema].[PR_{Schema}_{Entity}_Insert]` |
| Update    | `[Schema].[PR_{Schema}_{Entity}_Update]` |
| Delete    | `[Schema].[PR_{Schema}_{Entity}_Delete]` |
| Dropdown  | `[Schema].[PR_{Schema}_{Entity}_Dropdown]` |

### Tablas
`[Schema].[tb{EntityPascalCase}]`  — ej: `[Medico].[tbCitaMedica]`

### Columnas
`{prefijo}_{NombreCampo}` — ej: `cita_FechaConsulta`, `masc_Nombre`
- Soft delete: `{prefijo}_EsEliminado BIT DEFAULT 0`
- Auditoria: `{prefijo}_UsuarioCrea`, `{prefijo}_FechaCrea`, `{prefijo}_UsuarioModifica`, `{prefijo}_FechaModifica`

### ViewData tipico en vistas
```razor
@{
    ViewData["Title"]          = "Titulo de pagina";
    Layout                     = "~/Views/Shared/_Layout.cshtml";
    ViewData["CurrentPantalla"] = "Nombre de pantalla";  // para PantallaAuthorize JS
}
```

## Layout Compartido

**Archivo**: `PetsHome.UI/Views/Shared/_Layout.cshtml`

### CSS ya cargado en el layout (NO volver a incluir)
```
  ~/plugins/fontawesome-free-6.1.0-web/css/fontawesome.css
  ~/plugins/fontawesome-free-6.1.0-web/css/brands.css
  ~/plugins/fontawesome-free-6.1.0-web/css/solid.css
  ~/plugins/jvectormap/jquery-jvectormap-2.0.2.css
  ~/scss/bootstrap.min.css
  ~/css/jquery-ui.min.css
  ~/css/metisMenu.min.css
  ~/scss/app.min.css
  ~/scss/app-material.min.css
  ~/scss/icons.min.css
```

### CDNs ya en el layout
```
  https://fonts.googleapis.com/css?family=Quicksand:400,500,600,700&display=swap
  https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.1.1/css/all.min.css
  https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.1.1/css/regular.min.css
  https://cdnjs.cloudflare.com/ajax/libs/intro.js/7.2.0/introjs.min.css
```

### Estructura del body
```html
<!-- El @RenderBody() va dentro de: -->
<div id="content" class="main-content">
  <div class="layout-px-spacing">
    <div class="row layout-top-spacing">
      @RenderBody()   <!-- col-12 como wrapper tipico -->
    </div>
  </div>
</div>
```

### Secciones disponibles
```razor
@section Styles  { ... }   <!-- En el <head>, antes del cierre -->
@section Scripts { ... }   <!-- Antes del </body> -->
```

## Controllers

### `AccountController`
**Archivo**: `PetsHome.UI/Controllers/AccountController.cs`
**Acciones**: Login, Logout, AccessDenied

### `AdopcionController`
**Archivo**: `PetsHome.UI/Controllers/AdopcionController.cs`
**Pantalla**: `Listado de adopciones`
**Acciones**: Index, Create, List, Find, Detail, DetailByMascota, Add, Remove, ElegirAdoptante

### `BaseController`
**Archivo**: `PetsHome.UI/Controllers/BaseController.cs`
**Acciones**: ShowAlert, AjaxResult

### `CategoriaController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/CategoriaController.cs`
**Pantalla**: `Listado de categorias`
**Acciones**: Index, List, Find, Detail, Add, Remove, ValidarDescripcion

### `EmpleadosCargoController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/EmpleadosCargoController.cs`
**Pantalla**: `Listado de cargos`
**Acciones**: Index, List, Find, Detail, Add, Remove, ValidarDescripcion

### `GravedadController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/GravedadController.cs`
**Pantalla**: `Listado de gravedades`
**Acciones**: Index, List, Find, Detail, Add, Remove

### `ProcedenciaController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/ProcedenciaController.cs`
**Pantalla**: `Listado de procedencias`
**Acciones**: Index, List, Find, Detail, Add, Remove, ValidarDescripcion

### `RazaController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/RazaController.cs`
**Pantalla**: `Listado de razas`
**Acciones**: Index, List, Find, Detail, Add, Remove, ValidarDescripcion

### `TipoConsultaController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/TipoConsultaController.cs`
**Pantalla**: `Listado de tipos de consulta`
**Acciones**: Index, List, Find, Detail, Add, Remove

### `TipoEsterilizacionController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/TipoEsterilizacionController.cs`
**Pantalla**: `Listado de tipos de esterilizacion`
**Acciones**: Index, List, Find, Detail, Add, Remove

### `TipoMedicamentoController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/TipoMedicamentoController.cs`
**Pantalla**: `Listado de tipos de medicamento`
**Acciones**: Index, List, Find, Detail, Add, Remove

### `TipoParasitoController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/TipoParasitoController.cs`
**Pantalla**: `Listado de tipos de parasito`
**Acciones**: Index, List, Find, Detail, Add, Remove

### `VacunaController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/VacunaController.cs`
**Pantalla**: `Listado de vacunas`
**Acciones**: Index, List, Find, Detail, Add, Remove, ValidarDescripcion

### `ViaAdministracionController`
**Archivo**: `PetsHome.UI/Controllers/Catalogs/ViaAdministracionController.cs`
**Pantalla**: `Listado de vias de administracion`
**Acciones**: Index, List, Find, Detail, Add, Remove

### `CitaMedicaController`
**Archivo**: `PetsHome.UI/Controllers/CitaMedicaController.cs`
**Pantalla**: `Listado de citas medicas`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove, Calendario, CalendarioData, GetMascotasDropdown

### `EmpleadoController`
**Archivo**: `PetsHome.UI/Controllers/EmpleadoController.cs`
**Pantalla**: `Listado de empleados`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove, ValidarIdentidad

### `EventoController`
**Archivo**: `PetsHome.UI/Controllers/EventoController.cs`
**Pantalla**: `Listado de eventos`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove

### `HistorialMedicoController`
**Archivo**: `PetsHome.UI/Controllers/HistorialMedicoController.cs`
**Pantalla**: `Listado de mascotas`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove

### `ItemController`
**Archivo**: `PetsHome.UI/Controllers/ItemController.cs`
**Pantalla**: `Listado de items`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove, ValidarCodigo

### `MascotaController`
**Archivo**: `PetsHome.UI/Controllers/MascotaController.cs`
**Pantalla**: `Listado de mascotas`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove

### `LocalidadController`
**Archivo**: `PetsHome.UI/Controllers/Parciales/LocalidadController.cs`
**Pantalla**: `Listado de localidades`
**Acciones**: Index, ListMunicipios, FormPartialDepartamento, List, Add, AddMunicipio, FindMunicipio, DetailDepartamento

### `MunicipioController`
**Archivo**: `PetsHome.UI/Controllers/Parciales/MunicipioController.cs`
**Acciones**: Index

### `RecepcionMercanciaController`
**Archivo**: `PetsHome.UI/Controllers/Parciales/RecepcionMercanciaController.cs`
**Pantalla**: `Listado de recepciones`
**Acciones**: Index, List, Detail, DetailJson, ListDetalles, FormPartialRecepcion, Add, AddDetalle, FindDetalle, FindDetalleDetail, Remove, RemoveDetalle

### `RecetaController`
**Archivo**: `PetsHome.UI/Controllers/RecetaController.cs`
**Pantalla**: `Listado de recetas`
**Acciones**: Index, Create, List, Find, Detail, Add, Update, Remove

### `RefugioController`
**Archivo**: `PetsHome.UI/Controllers/RefugioController.cs`
**Pantalla**: `Listado de refugios`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove, ValidarNombre

### `RolesController`
**Archivo**: `PetsHome.UI/Controllers/RolesController.cs`
**Pantalla**: `Listado de roles`
**Acciones**: Index, List, Find, Create, Delete, Exist, PantallasList, PantallasByRol, SavePantallas

### `SolicitudController`
**Archivo**: `PetsHome.UI/Controllers/SolicitudController.cs`
**Pantalla**: `Listado de solicitudes`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove

### `TratamientoController`
**Archivo**: `PetsHome.UI/Controllers/TratamientoController.cs`
**Pantalla**: `Listado de tratamientos`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove

### `UsuariosController`
**Archivo**: `PetsHome.UI/Controllers/UsuariosController.cs`
**Pantalla**: `Listado de usuarios`
**Acciones**: Index, List, Find, Create, Delete, Exist, RolesDropdown

### `VoluntarioController`
**Archivo**: `PetsHome.UI/Controllers/VoluntarioController.cs`
**Pantalla**: `Listado de voluntarios`
**Acciones**: Index, Create, List, Find, Detail, Add, Remove, ValidarIdentidad

## Services

### `AdopcionService`
**Archivo**: `PetsHome.Business/Services/AdopcionService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `CategoriaService`
**Archivo**: `PetsHome.Business/Services/CategoriaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `CitaMedicaService`
**Archivo**: `PetsHome.Business/Services/CitaMedicaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `ComportamientosService`
**Archivo**: `PetsHome.Business/Services/ComportamientosService.cs`
**Metodos**: ComportamientoDropdown

### `DepartamentoService`
**Archivo**: `PetsHome.Business/Services/DepartamentoService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `EmpleadosCargoService`
**Archivo**: `PetsHome.Business/Services/EmpleadosCargoService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `EmpleadoService`
**Archivo**: `PetsHome.Business/Services/EmpleadoService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync

### `EventoService`
**Archivo**: `PetsHome.Business/Services/EventoService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync

### `GravedadService`
**Archivo**: `PetsHome.Business/Services/GravedadService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync, RemoveAsync

### `HistorialMedicoService`
**Archivo**: `PetsHome.Business/Services/HistorialMedicoService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `HomeService`
**Archivo**: `PetsHome.Business/Services/HomeService.cs`
**Metodos**: ObtenerUltimasAdopcionesAsync

### `InventariosDetalleService`
**Archivo**: `PetsHome.Business/Services/InventariosDetalleService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync, RemoveAsync

### `ItemService`
**Archivo**: `PetsHome.Business/Services/ItemService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync

### `MascotaService`
**Archivo**: `PetsHome.Business/Services/MascotaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync

### `MunicipioService`
**Archivo**: `PetsHome.Business/Services/MunicipioService.cs`
**Metodos**: ListIdAsync, FindAsync, AddAsync

### `PersonaService`
**Archivo**: `PetsHome.Business/Services/PersonaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync, RemoveAsync

### `ProcedenciaService`
**Archivo**: `PetsHome.Business/Services/ProcedenciaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `RecepcionDetalleService`
**Archivo**: `PetsHome.Business/Services/RecepcionDetalleService.cs`
**Metodos**: ListByRecepcionAsync, FindAsync, FindForDetailAsync

### `RecepcionMercanciaService`
**Archivo**: `PetsHome.Business/Services/RecepcionMercanciaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `RecetaService`
**Archivo**: `PetsHome.Business/Services/RecetaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync, RemoveAsync

### `RefugioService`
**Archivo**: `PetsHome.Business/Services/RefugioService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync

### `RolService`
**Archivo**: `PetsHome.Business/Services/RolService.cs`
**Metodos**: ListAsync, FindAsync, CreateAsync, EditAsync, DeleteAsync, ExistAsync, DropdownAsync, PantallasListAsync, PantallasByRolAsync, SavePantallasAsync, GetPantallasStringByRolAsync

### `SolicitudService`
**Archivo**: `PetsHome.Business/Services/SolicitudService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync

### `TipoConsultaService`
**Archivo**: `PetsHome.Business/Services/TipoConsultaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync

### `TipoEsterilizacionService`
**Archivo**: `PetsHome.Business/Services/TipoEsterilizacionService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync

### `TipoMedicamentoService`
**Archivo**: `PetsHome.Business/Services/TipoMedicamentoService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync, RemoveAsync

### `TipoParasitoService`
**Archivo**: `PetsHome.Business/Services/TipoParasitoService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync, RemoveAsync

### `TratamientoService`
**Archivo**: `PetsHome.Business/Services/TratamientoService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync

### `UsuarioService`
**Archivo**: `PetsHome.Business/Services/UsuarioService.cs`
**Metodos**: ListAsync, FindAsync, CreateAsync, EditAsync, DeleteAsync, ExistAsync, AuthenticateAsync

### `VacunaService`
**Archivo**: `PetsHome.Business/Services/VacunaService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync

### `ViaAdministracionService`
**Archivo**: `PetsHome.Business/Services/ViaAdministracionService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync

### `VoluntarioService`
**Archivo**: `PetsHome.Business/Services/VoluntarioService.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, UpdateAsync

## Repositories

### `AdopcionRepository`
**Archivo**: `PetsHome.Logic/Repositories/AdopcionRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, DetailByMascotaAsync, AddAsync, EditAsync
**SPs usados**: PR_Refugio_Adopciones_List, PR_Refugio_Adopciones_Find, PR_Refugio_Adopciones_Detail, PR_Refugio_Adopciones_Insert, PR_Refugio_Adopcion_Update, PR_Refugio_Adopciones_Delete

### `CategoriaRepository`
**Archivo**: `PetsHome.Logic/Repositories/CategoriaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Inventario_Categorias_List, PR_Inventario_Categorias_Find, PR_Inventario_Categorias_Detail, PR_Inventario_Categorias_Insert, PR_Inventario_Categorias_Update, PR_Inventario_Categorias_Delete, PR_Inventario_Categorias_Existe

### `CitaMedicaRepository`
**Archivo**: `PetsHome.Logic/Repositories/CitaMedicaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync
**SPs usados**: PR_Medico_CitaMedica_List, PR_Medico_CitaMedica_Find, PR_Medico_CitaMedica_Detail, PR_Medico_CitaMedica_Insert, PR_Medico_CitaMedica_Update, PR_Medico_CitaMedica_Delete, PR_Medico_CitaMedica_Dropdown, PR_Medico_CitaMedica_Calendario, PR_Refugio_Comportamiento_List

### `EmpleadoRepository`
**Archivo**: `PetsHome.Logic/Repositories/EmpleadoRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync
**SPs usados**: PR_Refugio_Empleados_List, PR_Refugio_Empleados_Find, PR_Refugio_Empleados_Detail, PR_Refugio_Empleados_Insert, PR_Refugio_Empleados_Update, PR_Refugio_Empleados_Delete, PR_Refugio_Empleados_Existe, PR_Refugio_EmpleadosCargos_Dropdown

### `EmpleadosCargoRepository`
**Archivo**: `PetsHome.Logic/Repositories/EmpleadosCargoRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Refugio_EmpleadosCargos_List, PR_Refugio_EmpleadosCargos_Find, PR_Refugio_EmpleadosCargos_Detail, PR_Refugio_EmpleadosCargos_Insert, PR_Refugio_EmpleadosCargos_Update, PR_Refugio_EmpleadosCargos_Delete, PR_Refugio_EmpleadosCargos_Existe

### `EventoLoggerRepository`
**Archivo**: `PetsHome.Logic/Repositories/EventoLoggerRepository.cs`
**Metodos**: Insert

### `EventoRepository`
**Archivo**: `PetsHome.Logic/Repositories/EventoRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Refugio_Eventos_List, PR_Refugio_Eventos_Find, PR_Refugio_Eventos_Detail, PR_Refugio_Eventos_Insert, PR_Refugio_Eventos_Update, PR_Refugio_Eventos_Delete

### `GravedadRepository`
**Archivo**: `PetsHome.Logic/Repositories/GravedadRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Medico_Gravedades_List, PR_Medico_Gravedades_Find, PR_Medico_Gravedades_Detail, PR_Medico_Gravedades_Insert, PR_Medico_Gravedades_Update, PR_Medico_Gravedades_Delete

### `HistorialMedicoRepository`
**Archivo**: `PetsHome.Logic/Repositories/HistorialMedicoRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Refugio_HistorialMedico_List, PR_Refugio_HistorialMedico_Find, PR_Refugio_HistorialMedico_Detail, PR_Refugio_HistorialMedico_Insert, PR_Refugio_HistorialMedico_Update, PR_Refugio_HistorialMedico_Delete

### `InventariosDetalleRepository`
**Archivo**: `PetsHome.Logic/Repositories/InventariosDetalleRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Inventario_InventarioDetalles_List, PR_Inventario_InventarioDetalles_Find, PR_Inventario_InventarioDetalles_Detail, PR_Inventario_InventarioDetalles_Insert, PR_Inventario_InventarioDetalles_Update, PR_Inventario_InventarioDetalles_Delete

### `ItemRepository`
**Archivo**: `PetsHome.Logic/Repositories/ItemRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Inventario_Items_List, PR_Inventario_Items_Find, PR_Inventario_Items_Detail, PR_Inventario_Items_Insert, PR_Inventario_Items_Update, PR_Inventario_Items_Delete, PR_Inventario_Items_Existe, PR_Inventario_Categorias_Dropdown, PR_Inventario_Items_Dropdown

### `LocalidadRepository`
**Archivo**: `PetsHome.Logic/Repositories/LocalidadRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_General_Departamentos_List, PR_General_Departamentos_Find, PR_General_Departamentos_Detail, PR_General_Departamentos_Insert, PR_General_Departamentos_Update, PR_General_Departamentos_Delete, PR_General_Departamentos_Dropdown

### `MascotaRepository`
**Archivo**: `PetsHome.Logic/Repositories/MascotaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Refugio_Mascotas_List, PR_Refugio_Mascotas_Find, PR_Refugio_Mascotas_Detail, PR_Refugio_Mascotas_Insert, PR_Refugio_Mascotas_Update, PR_Refugio_Mascotas_Delete, PR_Refugio_Raza_Dropdown, PR_Refugio_Procedencias_Dropdown

### `MunicipioRepository`
**Archivo**: `PetsHome.Logic/Repositories/MunicipioRepository.cs`
**Metodos**: ListAsync, ListIdAsync, AddAsync, FindAsync, DetailAsync, EditAsync
**SPs usados**: PR_General_Municipios_List, PR_General_Municipios_SelectbyDepartamento, PR_General_Municipios_Insert, PR_General_Municipios_Find, PR_General_Municipios_Detail, PR_General_Municipios_Update, PR_General_Municipios_Delete, PR_General_Municipios_Dropdown

### `PantallaRepository`
**Archivo**: `PetsHome.Logic/Repositories/PantallaRepository.cs`
**Metodos**: ListAsync, ByRolAsync, NombresByRolAsync, PermisosByRolAsync, SaveByRolAsync
**SPs usados**: PR_Seguridad_Pantallas_List, PR_Seguridad_Pantallas_ByRol, PR_Seguridad_Pantallas_NombresByRol, PR_Seguridad_Pantallas_PermisosByRol, PR_Seguridad_RolesPantallas_Save

### `PersonaRepository`
**Archivo**: `PetsHome.Logic/Repositories/PersonaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_General_Departamentos_List, PR_General_Departamentos_Find, PR_General_Departamentos_Detail, PR_General_Personas_Insert, PR_General_Personas_Update, PR_General_Departamentos, PR_General_Departamentos_Delete

### `ProcedenciaRepository`
**Archivo**: `PetsHome.Logic/Repositories/ProcedenciaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Refugio_Procedencias_List, PR_Refugio_Procedencias_Find, PR_Refugio_Procedencias_Detail, PR_Refugio_Procedencias_Insert, PR_Refugio_Procedencias_Update, PR_Refugio_Procedencias_Delete, PR_Refugio_Procedencias_Existe, PR_Refugio_Procedencias_Dropdown

### `RecepcionDetalleRepository`
**Archivo**: `PetsHome.Logic/Repositories/RecepcionDetalleRepository.cs`
**Metodos**: ListByRecepcionAsync, FindAsync, AddAsync
**SPs usados**: PR_Inventario_RecepcionesDetalles_ByRecepcion, PR_Inventario_RecepcionesDetalles_Find, PR_Inventario_RecepcionesDetalles_Insert, PR_Inventario_RecepcionesDetalles_Update, PR_Inventario_RecepcionesDetalles_Delete

### `RecepcionMercanciaRepository`
**Archivo**: `PetsHome.Logic/Repositories/RecepcionMercanciaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync
**SPs usados**: PR_Inventario_RecepcionesMercancia_List, PR_Inventario_RecepcionesMercancia_Find, PR_Inventario_RecepcionesMercancia_Detail, PR_Inventario_RecepcionesMercancia_Insert, PR_Inventario_RecepcionesMercancia_Update, PR_Inventario_RecepcionesMercancia_Delete

### `RecetaRepository`
**Archivo**: `PetsHome.Logic/Repositories/RecetaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Medico_Recetas_List, PR_Medico_Recetas_Find, PR_Medico_Recetas_Detail, PR_Medico_Recetas_Insert, PR_Medico_Recetas_Update, PR_Medico_Recetas_Delete, PR_Medico_Recetas_Dropdown

### `RefugioRepository`
**Archivo**: `PetsHome.Logic/Repositories/RefugioRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Refugio_Refugios_List, PR_Refugio_Refugios_Find, PR_Refugio_Refugios_Detail, PR_Refugio_Refugios_Insert, PR_Refugio_Refugios_Update, PR_Refugio_Refugios_Delete, PR_Refugio_Refugios_Existe, PR_Refugio_Refugio_Dropdown

### `RolRepository`
**Archivo**: `PetsHome.Logic/Repositories/RolRepository.cs`
**Metodos**: ListAsync, FindAsync, InsertAsync, UpdateAsync, DeleteAsync, ExistAsync, DropdownAsync
**SPs usados**: PR_Seguridad_Roles_List, PR_Seguridad_Roles_Find, PR_Seguridad_Roles_Insert, PR_Seguridad_Roles_Update, PR_Seguridad_Roles_Delete, PR_Seguridad_Roles_Exist, PR_Seguridad_Roles_Dropdown

### `SolicitudRepository`
**Archivo**: `PetsHome.Logic/Repositories/SolicitudRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Refugio_Solicitudes_List, PR_Refugio_Solicitudes_Find, PR_Refugio_Solicitudes_Detail, PR_Refugio_Solicitudes_Insert, PR_Refugio_Solicitudes_Update, PR_Refugio_Solicitudes_Delete

### `TipoConsultaRepository`
**Archivo**: `PetsHome.Logic/Repositories/TipoConsultaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Medico_TiposConsulta_List, PR_Medico_TiposConsulta_Find, PR_Medico_TiposConsulta_Detail, PR_Medico_TiposConsulta_Insert, PR_Medico_TiposConsulta_Update, PR_Medico_TiposConsulta_Delete

### `TipoEsterilizacionRepository`
**Archivo**: `PetsHome.Logic/Repositories/TipoEsterilizacionRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Medico_TiposEsterilizacion_List, PR_Medico_TiposEsterilizacion_Find, PR_Medico_TiposEsterilizacion_Detail, PR_Medico_TiposEsterilizacion_Insert, PR_Medico_TiposEsterilizacion_Update, PR_Medico_TiposEsterilizacion_Delete

### `TipoMedicamentoRepository`
**Archivo**: `PetsHome.Logic/Repositories/TipoMedicamentoRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Medico_TiposMedicamento_List, PR_Medico_TiposMedicamento_Find, PR_Medico_TiposMedicamento_Detail, PR_Medico_TiposMedicamento_Insert, PR_Medico_TiposMedicamento_Update, PR_Medico_TiposMedicamento_Delete

### `TipoParasitoRepository`
**Archivo**: `PetsHome.Logic/Repositories/TipoParasitoRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Medico_TiposParasito_List, PR_Medico_TiposParasito_Find, PR_Medico_TiposParasito_Detail, PR_Medico_TiposParasito_Insert, PR_Medico_TiposParasito_Update, PR_Medico_TiposParasito_Delete, PR_Medico_TiposParasito_Dropdown

### `TratamientoRepository`
**Archivo**: `PetsHome.Logic/Repositories/TratamientoRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync
**SPs usados**: PR_Medico_Tratamientos_List, PR_Medico_Tratamientos_Find, PR_Medico_Tratamientos_Detail, PR_Medico_Tratamientos_Insert, PR_Medico_Tratamientos_Update, PR_Medico_Tratamientos_Delete, PR_Medico_Tratamientos_Dropdown

### `UsuarioRepository`
**Archivo**: `PetsHome.Logic/Repositories/UsuarioRepository.cs`
**Metodos**: ListAsync, FindAsync, InsertAsync, UpdateAsync, DeleteAsync, ExistAsync
**SPs usados**: PR_Seguridad_Usuarios_List, PR_Seguridad_Usuarios_Find, PR_Seguridad_Usuarios_Insert, PR_Seguridad_Usuarios_Update, PR_Seguridad_Usuarios_Delete, PR_Seguridad_Usuarios_Exist

### `VacunaRepository`
**Archivo**: `PetsHome.Logic/Repositories/VacunaRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Refugio_Vacunas_List, PR_Refugio_Vacunas_Find, PR_Refugio_Vacunas_Detail, PR_Refugio_Vacunas_Insert, PR_Refugio_Vacunas_Update, PR_Refugio_Vacunas_Delete, PR_Refugio_Vacunas_Existe

### `ViaAdministracionRepository`
**Archivo**: `PetsHome.Logic/Repositories/ViaAdministracionRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync, RemoveAsync
**SPs usados**: PR_Medico_ViasAdministracion_List, PR_Medico_ViasAdministracion_Find, PR_Medico_ViasAdministracion_Detail, PR_Medico_ViasAdministracion_Insert, PR_Medico_ViasAdministracion_Update, PR_Medico_ViasAdministracion_Delete

### `VoluntarioRepository`
**Archivo**: `PetsHome.Logic/Repositories/VoluntarioRepository.cs`
**Metodos**: ListAsync, FindAsync, DetailAsync, AddAsync, EditAsync
**SPs usados**: PR_Refugio_Voluntarios_List, PR_Refugio_Voluntarios_Find, PR_Refugio_Voluntarios_Detail, PR_Refugio_Voluntarios_Insert, PR_Refugio_Voluntarios_Update, PR_Refugio_Voluntarios_Delete, PR_Refugio_Voluntarios_Existe

## ViewModels (Business/Models)

- **AdopcionDetails**: AdopcionDetailsViewModel
- **AdopcionView**: AdopcionViewModel
- **CategoriaView**: CategoriaViewModel
- **CitaMedica**: CitaMedicaCalendarioViewModel, CitaMedicaDetailViewModel, CitaMedicaFindViewModel, CitaMedicaFormViewModel, CitaMedicaListViewModel
- **ComportamientoView**: ComportamientoViewModel
- **DepartamentoDetails**: DepartamentoDetailsViewModel
- **DepartamentoForm**: DepartamentoFormViewModel
- **DepartamentoList**: DepartamentoListViewModel
- **DepartamentoView**: DepartamentoViewModel
- **EmpleadoCargo**: EmpleadoCargoViewModel
- **EmpleadoDetails**: EmpleadoDetailsViewModel
- **EmpleadoForm**: EmpleadoFormViewModel
- **EmpleadoList**: EmpleadoListViewModel
- **EspaciosAttribute**: EspaciosAttribute
- **EventoView**: EventoViewModel
- **GravedadView**: GravedadViewModel
- **HistorialMedico**: HistorialMedicoViewModel
- **HomeView**: HomeViewModel
- **ItemView**: ItemViewModel
- **LoginView**: LoginViewModel
- **MascotaDetails**: MascotaDetailsViewModel
- **MascotaDropdown**: MascotaDropdownViewModel
- **MascotaForm**: MascotaFormViewModel
- **MascotaList**: MascotaListViewModel
- **MunicipioDetails**: MunicipioDetailsViewModel
- **MunicipioForm**: MunicipioFormViewModel
- **MunicipioList**: MunicipioListViewModel
- **MunicipioView**: MunicipioViewModel
- **PantallaId**: PantallaIdViewModel
- **PantallaItem**: PantallaItemViewModel
- **PantallaPermiso**: PantallaPermisoViewModel
- **PersonaView**: PersonaViewModel
- **ProcedenciaView**: ProcedenciaViewModel
- **RazaDetails**: RazaDetailsViewModel
- **RazaDropdown**: RazaDropdownViewModel
- **RazaForm**: RazaFormViewModel
- **RazaList**: RazaListViewModel
- **RecepcionDetalle**: RecepcionDetalleFormViewModel, RecepcionDetalleListViewModel
- **RecepcionMercancia**: RecepcionMercanciaDetailsViewModel, RecepcionMercanciaFormViewModel, RecepcionMercanciaListViewModel
- **RecetaDetails**: RecetaDetailsViewModel
- **RecetaForm**: RecetaFormViewModel
- **RecetaList**: RecetaListViewModel
- **RecetaView**: RecetaViewModel
- **RefugioDetails**: RefugioDetailsViewModel
- **RefugioDropdown**: RefugioDropdownViewModel
- **RefugioForm**: RefugioFormViewModel
- **RefugioList**: RefugioListViewModel
- **RegistroVoluntariado**: RegistroVoluntariadoViewModel
- **RolCon**: RolConPantallasViewModel
- **RolDropdown**: RolDropdownViewModel
- **RolView**: RolViewModel
- **SalidaView**: SalidaViewModel
- **SolicitudDetails**: SolicitudDetailsViewModel
- **SolicitudForm**: SolicitudFormViewModel
- **SolicitudList**: SolicitudListViewModel
- **SolicitudView**: SolicitudViewModel
- **SubmenuView**: SubmenuViewModel
- **TipoConsulta**: TipoConsultaViewModel
- **TipoEsterilizacion**: TipoEsterilizacionViewModel
- **TipoMedicamento**: TipoMedicamentoViewModel
- **TipoParasito**: TipoParasitoViewModel
- **TratamientoDetails**: TratamientoDetailsViewModel
- **TratamientoForm**: TratamientoFormViewModel
- **TratamientoList**: TratamientoListViewModel
- **TratamientoView**: TratamientoViewModel
- **UsuarioCrud**: UsuarioCrudViewModel
- **UsuarioView**: UsuarioViewModel
- **VacunaDetails**: VacunaDetailsViewModel
- **VacunaDropdown**: VacunaDropdownViewModel
- **VacunaForm**: VacunaFormViewModel
- **VacunaList**: VacunaListViewModel
- **ViaAdministracion**: ViaAdministracionViewModel
- **VoluntarioDetails**: VoluntarioDetailsViewModel
- **VoluntarioForm**: VoluntarioFormViewModel
- **VoluntarioList**: VoluntarioListViewModel

## Result Entities (Common/Entities)

**Albergue**: PR_Albergue_Albergues_DeleteResult, PR_Albergue_EmpleadosCargos_DeleteResult, PR_Albergue_FichaAdopcion_DeleteResult, PR_Albergue_FichasMedicas_DeleteResult, PR_Albergue_Mascotas_DeleteResult, PR_Albergue_Procedencias_DeleteResult, PR_Albergue_Razas_DeleteResult, PR_Albergue_Solicitudes_DeleteResult, PR_Albergue_Vacunas_DeleteResult, PR_Albergue_Voluntarios_DeleteResult
**General**: PR_General_Departamentos_DetailResult, PR_General_Departamentos_DropdownResult, PR_General_Departamentos_FindResult, PR_General_Departamentos_ListResult, PR_General_Municipios_DetailResult, PR_General_Municipios_DropdownResult, PR_General_Municipios_FindResult, PR_General_Municipios_ListResult, PR_General_Municipios_SelectbyDepartamentoResult, tbDepartamentos, tbMunicipios, tbPersonas
**Inventario**: PR_Inventario_Categorias_DetailResult, PR_Inventario_Categorias_DropdownResult, PR_Inventario_Categorias_FindResult, PR_Inventario_Categorias_ListResult, PR_Inventario_Entradas_FindResult, PR_Inventario_EntradasDetalles_FindResult, PR_Inventario_Inventarios_DetailResult, PR_Inventario_Inventarios_FindResult, PR_Inventario_Inventarios_ListResult, PR_Inventario_Items_DetailResult, PR_Inventario_Items_DropdownResult, PR_Inventario_Items_FindResult, PR_Inventario_Items_ListResult, PR_Inventario_RecepcionesDetalles_FindResult, PR_Inventario_RecepcionesDetalles_ListResult, PR_Inventario_RecepcionesMercancia_DetailResult, PR_Inventario_RecepcionesMercancia_FindResult, PR_Inventario_RecepcionesMercancia_ListResult, tbCategorias, tbItems, tbRecepcionesDetalles, tbRecepcionesMercancia
**Medico**: PR_Medico_CitaMedica_CalendarioResult, PR_Medico_CitaMedica_DetailResult, PR_Medico_CitaMedica_DropdownResult, PR_Medico_CitaMedica_FindResult, PR_Medico_CitaMedica_ListResult, PR_Medico_Gravedades_DetailResult, PR_Medico_Gravedades_DropdownResult, PR_Medico_Gravedades_FindResult, PR_Medico_Gravedades_ListResult, PR_Medico_Recetas_DetailResult, PR_Medico_Recetas_DropdownResult, PR_Medico_Recetas_FindResult, PR_Medico_Recetas_ListResult, PR_Medico_TiposConsulta_DetailResult, PR_Medico_TiposConsulta_DropdownResult, PR_Medico_TiposConsulta_FindResult, PR_Medico_TiposConsulta_ListResult, PR_Medico_TiposEsterilizacion_DetailResult, PR_Medico_TiposEsterilizacion_DropdownResult, PR_Medico_TiposEsterilizacion_FindResult, PR_Medico_TiposEsterilizacion_ListResult, PR_Medico_TiposMedicamento_DetailResult, PR_Medico_TiposMedicamento_DropdownResult, PR_Medico_TiposMedicamento_FindResult, PR_Medico_TiposMedicamento_ListResult, PR_Medico_TiposParasito_DetailResult, PR_Medico_TiposParasito_DropdownResult, PR_Medico_TiposParasito_FindResult, PR_Medico_TiposParasito_ListResult, PR_Medico_Tratamientos_DetailResult, PR_Medico_Tratamientos_DropdownResult, PR_Medico_Tratamientos_FindResult, PR_Medico_Tratamientos_ListResult, PR_Medico_ViasAdministracion_DetailResult, PR_Medico_ViasAdministracion_DropdownResult, PR_Medico_ViasAdministracion_FindResult, PR_Medico_ViasAdministracion_ListResult, tbCitaMedica, tbGravedades, tbRecetas, tbTiposConsulta, tbTiposEsterilizacion, tbTiposMedicamento, tbTiposParasito, tbTratamientos, tbViasAdministracion
**Refugio**: PR_Refugio_Adopciones_DetailResult, PR_Refugio_Adopciones_FindResult, PR_Refugio_Adopciones_ListResult, PR_Refugio_CitaMedica_DetailResult, PR_Refugio_CitaMedica_FindResult, PR_Refugio_CitaMedica_ListResult, PR_Refugio_Comportamiento_ListResult, PR_Refugio_Empleados_DetailResult, PR_Refugio_Empleados_FindResult, PR_Refugio_Empleados_ListResult, PR_Refugio_EmpleadosCargos_DetailResult, PR_Refugio_EmpleadosCargos_DropdownResult, PR_Refugio_EmpleadosCargos_FindResult, PR_Refugio_EmpleadosCargos_ListResult, PR_Refugio_Eventos_DetailResult, PR_Refugio_Eventos_FindResult, PR_Refugio_Eventos_InsertResult, PR_Refugio_Eventos_ListResult, PR_Refugio_HistorialMedico_DetailResult, PR_Refugio_HistorialMedico_FindResult, PR_Refugio_HistorialMedico_ListResult, PR_Refugio_Mascotas_DetailResult, PR_Refugio_Mascotas_FindResult, PR_Refugio_Mascotas_ListResult, PR_Refugio_Procedencia_DropdownResult, PR_Refugio_Procedencias_DetailResult, PR_Refugio_Procedencias_FindResult, PR_Refugio_Procedencias_InsertResult, PR_Refugio_Procedencias_ListResult, PR_Refugio_Raza_DropdownResult, PR_Refugio_Razas_DetailResult, PR_Refugio_Razas_FindResult, PR_Refugio_Razas_ListResult, PR_Refugio_Refugio_DropdownResult, PR_Refugio_Refugios_DetailResult, PR_Refugio_Refugios_FindResult, PR_Refugio_Refugios_ListResult, PR_Refugio_Solicitudes_DetailResult, PR_Refugio_Solicitudes_FindResult, PR_Refugio_Solicitudes_ListResult, PR_Refugio_Vacunas_DetailResult, PR_Refugio_Vacunas_FindResult, PR_Refugio_Vacunas_ListResult, PR_Refugio_Voluntarios_DetailResult, PR_Refugio_Voluntarios_FindResult, PR_Refugio_Voluntarios_ListResult, tbAdopciones, tbComportamientos, tbEmpleados, tbEmpleadosCargos, tbEventos, tbEventos_tbVoluntarios, tbHistorialMedico, tbHistorialMedico_tbVacunas, tbMascotas, tbProcedencias, tbRazas, tbRefugios, tbSolicitudes, tbVacunas, tbVoluntarios
**Seguridad**: PR_Seguridad_Pantallas_ByRolResult, PR_Seguridad_Pantallas_ListResult, PR_Seguridad_Pantallas_NombresByRolResult, PR_Seguridad_Pantallas_PermisosByRolResult, PR_Seguridad_RegistroEventos_FindResult, PR_Seguridad_RegistroEventos_InsertResult, PR_Seguridad_RegistroEventos_SelectResult, PR_Seguridad_Roles_DropdownResult, PR_Seguridad_Roles_ExistResult, PR_Seguridad_Roles_FindResult, PR_Seguridad_Roles_ListResult, PR_Seguridad_RolesPantallas_SaveResult, PR_Seguridad_Usuarios_ExistResult, PR_Seguridad_Usuarios_FindResult, PR_Seguridad_Usuarios_ListResult, PR_Seguridad_Usuarios_LoginInResult, PR_Seguridad_Usuarios_LoginResult, PR_Seguridad_Usuarios_LogoutResult, tbPantallas, tbRegistroEventos, tbRolesPantallas, tbUsuarios

## Base de Datos — Scripts SQL

- `01_CREATE_TABLES_CATALOGO_MEDICO.sql  → tablas: [Medico].[tbTiposConsulta], [Medico].[tbGravedades], [Medico].[tbTiposMedicamento]`
- `02_INSERT_DATA_CATALOGO_MEDICO.sql`
- `03_SP_TIPOS_CONSULTA.sql`
- `04_SP_GRAVEDADES.sql`
- `05_SP_TIPOS_MEDICAMENTO.sql`
- `06_SP_VIAS_ADMINISTRACION.sql`
- `07_SP_TIPOS_PARASITO.sql`
- `08_SP_TIPOS_ESTERILIZACION.sql`
- `09_MIGRAR_CITAMEDICA_A_MEDICO.sql  → tablas: [Medico].[tbCitaMedica], [Medico].[tbCitaMedica_tbVacunas]`
- `10_CREATE_TABLE_RECETAS.sql  → tablas: [Medico].[tbRecetas]`
- `11_CREATE_TABLE_TRATAMIENTOS.sql  → tablas: [Medico].[tbTratamientos]`
- `12_SP_CITA_MEDICA.sql`
- `13_SP_CITA_MEDICA_CALENDARIO.sql`
- `13_SP_RECETAS.sql`
- `14_SP_TRATAMIENTOS.sql`
- `15_LIMPIAR_Y_POBLAR_DATOS_PORTAFOLIO.sql`
- `16_ESTANDARIZAR_SPS_TRANSACCIONES.sql`
- `17_SEGURIDAD_PANTALLAS_ROLES.sql  → tablas: [Seguridad].[tbPantallas], [Seguridad].[tbRolesPantallas]`
- `18_CORRECCION_ROLES_COLUMNAS.sql`
- `FIX_asignar_pantallas_admin.sql`
- `Update_All_List_OrderBy.sql`

### Pantallas registradas en tbPantallas
- `Home` (Home)
- `Listado de empleados` (Cuenta)
- `Listado de voluntarios` (Cuenta)
- `Listado de recepciones` (Inventario)
- `Listado de items` (Inventario)
- `Listado de cargos` (Administracion)
- `Listado de refugios` (Administracion)
- `Listado de localidades` (Administracion)
- `Listado de categorias` (Administracion)
- `Listado de eventos` (Administracion)
- `Listado de mascotas` (Adopcion)
- `Listado de adopciones` (Adopcion)
- `Listado de solicitudes` (Adopcion)
- `Listado de citas medicas` (Medicamento)
- `Listado de recetas` (Medicamento)
- `Listado de tratamientos` (Medicamento)
- `Listado de vacunas` (Medicamento)
- `Listado de procedencias` (Medicamento)
- `Listado de razas` (Medicamento)
- `Listado de tipos de consulta` (Medicamento)
- `Listado de gravedades` (Medicamento)
- `Listado de tipos de medicamento` (Medicamento)
- `Listado de vias de administracion` (Medicamento)
- `Listado de tipos de parasito` (Medicamento)
- `Listado de tipos de esterilizacion` (Medicamento)
- `Listado de usuarios` (Seguridad)
- `Listado de roles` (Seguridad)
