# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PetsHome is an ASP.NET Core 3.1 web application for managing pet shelters. It provides functionality for managing pets, medical appointments, adoptions, volunteers, inventory, and shelter operations. The application follows a multi-layered architecture with clear separation of concerns.

## Development Commands

### Build and Run
```bash
# Build the solution
dotnet build PetsHome.sln

# Run the application (from PetsHome.UI directory)
cd PetsHome.UI
dotnet run

# Restore packages
dotnet restore
```

### Database
- Uses SQL Server with Entity Framework Core
- Connection string configured in `appsettings.json`
- Database-first approach with stored procedures
- Entity classes are generated in `PetsHome.Common/Entities/`

## Architecture

### Layer Structure
The application follows a 4-layer architecture:

1. **PetsHome.UI** (04_WebUI_Layer)
   - ASP.NET Core MVC controllers and views
   - Default route: `{controller=citamedica}/{action=index}/{id?}`
   - Uses Razor runtime compilation
   - Custom middleware: `DropDownErrorMiddleware`

2. **PetsHome.Business** (03_Business_Layer)
   - Service layer with business logic
   - AutoMapper configuration
   - Service registration via dependency injection
   - ViewModels for data transfer

3. **PetsHome.Logic** (02_Logic_Layer)
   - Repository pattern implementation
   - Interfaces for data access abstraction
   - Generic repository interface `IGenericRepository<T>`

4. **PetsHome.DataAccess** (01_DataAccess_Layer)
   - Entity Framework DbContext
   - Database connection management

5. **PetsHome.Common** (00_Commons_Layer)
   - Entity classes generated from database
   - Result classes from stored procedures
   - Shared data structures

### Key Components

- **Dependency Injection**: Configured in `ServiceConfiguration.cs`
- **AutoMapper**: Used for object mapping between entities and ViewModels
- **Stored Procedures**: Database operations use stored procedures with result classes
- **File Management**: Pet images stored in configured file path
- **Logging**: Uses NLog for application logging

### Main Functional Areas

- **Pet Management**: Pets, breeds, behavior tracking
- **Medical Care**: Medical appointments, vaccines, medical history
- **Adoptions**: Adoption process and requests management
- **Volunteers**: Volunteer registration and management
- **Inventory**: Items, categories, entries and stock management
- **Administration**: Employees, shelters, and general location data

### Frontend
- Bootstrap-based responsive UI
- jQuery for client-side functionality
- DataTables for data grids
- Custom JavaScript files per functional area in `wwwroot/js/pages/`
- SCSS source files compiled to CSS


### Pantallas Grandes
Las siguientes views se consideran "pantallas grandes" y utilizan `components/datatable/datatable.init.js`:
- adopcion
- citamedica
- empleado
- evento
- mascota
- refugio
- solicitud
- voluntario

**Archivo común**: `components/datatable/datatable.init.js` - Configuración estándar para DataTables


### Pantallas Catalogo
Las siguientes views se consideran "pantallas catalogo" y utilizan `components/datatable/datatable.catalogs.init.js`:
- Categoria
- EmpleadosCargo
- Procedencia
- Raza
- Vacuna

**Archivo común**: `components/datatable/datatable.catalogs.init.js` - Configuración estándar para DataTables

## Nomenclatura y Convenciones

### Reglas de Nomenclatura
**IMPORTANTE**: Ser fiel a las nomenclaturas de los nombres de las clases, propiedades y procedimientos ya desarrollados para así nombrar los nuevos.

#### Entidades de Base de Datos
- Prefijo `tb` para tablas: `tbRecepciones`, `tbRecepcionesDetalles`, `tbItems`, `tbExistencias`
- Campos con prefijo de tabla: `recep_Id`, `recep_Fecha`, `recdet_Id`, `itm_Codigo`
- Stored procedures con prefijo `SP_`: `SP_tbRecepciones_List`, `SP_tbSalidas_Detail`

#### ViewModels
- Sufijo `ViewModel`: `RecepcionMercanciaViewModel`, `RecepcionDetalleViewModel`
- Propiedades mantienen nombres de entidad: `recep_Id`, `recep_Fecha`, `recdet_Cantidad`

#### Servicios
- Sufijo `Service`: `RecepcionMercanciaService`, `RecepcionesDetallesService`
- Métodos estándar: `AddAsync()`, `UpdateAsync()`, `RemoveAsync()`, `FindAsync()`, `ListAsync()`

#### Repositorios
- Sufijo `Repository`: `RecepcionesMercanciaRepository`, `RecepcionesDetallesRepository`
- Interfaces con prefijo `I`: `IRecepcionesMercanciaRepository`

#### Controladores
- Sufijo `Controller`: `RecepcionMercanciaController`, `RecepcionDetalleController`
- Métodos de acción estándar: `Create()`, `Detail()`, `List()`, `Add()`, `FindDetalle()`

#### Vistas y JavaScript
- Nombres de archivos coinciden con controlador: `recepcionmercancia.js`, `salida.js`
- IDs de elementos HTML: `datatable-detalles`, `edit-detalle-modal`, `delete-detalle-modal`
- Funciones JavaScript: `initDetallesTable()`, `showDetalleModal()`, `calculateTotal()`

#### Patrones Establecidos
- **Master-Detail**: Seguir patrón de `Localidad` (Departamento/Municipio)
- **CRUD Operations**: Usar `goto ErrorResult` para manejo de errores
- **Redirecciones**: `RedirectToAction("Create", new { id = model.Id })` después de guardar
- **Alertas**: `ShowAlert(AlertMessaje.SuccessSave, AlertMessageType.Success)`
