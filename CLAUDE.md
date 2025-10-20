# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PetsHome is an ASP.NET Core 3.1 MVC application for managing a pet shelter (refugio de mascotas). The system handles adoptions, medical records, inventory, employees, volunteers, and events for pets in shelters.

## Architecture

The solution follows a layered architecture with 5 projects organized in solution folders:

### Layer Structure (Bottom to Top)

1. **PetsHome.Common** (00_Commons_Layer)
   - Shared utilities and common code
   - No external dependencies

2. **PetsHome.DataAccess** (01_DataAccess_Layer)
   - Database context and connection management
   - Uses Dapper for data access and Entity Framework Core for SQL Server
   - `PetsHomeDbContext` manages connection strings statically
   - Dependencies: Dapper, Microsoft.EntityFrameworkCore.SqlServer

3. **PetsHome.Logic** (02_Logic_Layer)
   - Repository pattern implementation
   - Contains repositories for all entities (e.g., MascotaRepository, AdopcionRepository, EmpleadoRepository)
   - Repositories handle direct data access

4. **PetsHome.Business** (03_Business_Layer)
   - Service layer with business logic
   - ViewModels for data transfer
   - AutoMapper configuration for entity-ViewModel mapping
   - Service classes correspond to repositories (e.g., MascotaService, AdopcionService)
   - Dependencies: AutoMapper, NLog, SixLabors.ImageSharp (for image processing)

5. **PetsHome.UI** (04_WebUI_Layer)
   - ASP.NET Core MVC web application
   - Controllers organized in subdirectories:
     - `Controllers/` - Main controllers (Home, Mascota, Refugio, etc.)
     - `Controllers/catalogs/` - Catalog controllers (Categoria, Raza, Vacuna, etc.)
     - `Controllers/parciales/` - Partial/detail controllers (Entrada, InventarioDetalle, etc.)
   - Frontend uses jQuery DataTables with custom initialization scripts in `wwwroot/js/pages/`
   - Uses Serilog for logging
   - Runtime Razor compilation enabled for development

### Dependency Injection Configuration

Services are registered via extension methods in `PetsHome.Business/ServiceConfiguration.cs`:
- `AddLogicLayer(connectionString)` - Registers all repositories and configures database connection
- `AddBusinessLogic()` - Registers all services and AutoMapper configuration

These are called in `Startup.cs:ConfigureServices()`.

### Key Patterns

- **Repository Pattern**: Logic layer provides repositories, Business layer provides services
- **ViewModel Pattern**: Business layer Models are ViewModels for UI data transfer
- **BaseController**: All controllers inherit from `BaseController` which provides:
  - `ShowAlert(text, type)` - Display alerts using TempData
  - `AjaxResult(model, success)` - Standard JSON response format for AJAX calls
- **Static DbContext**: Connection string is set statically via `PetsHomeDbContext.BuildConnectionString()`

## Development Commands

### Build and Run

```bash
# Build the entire solution
dotnet build PetsHome.sln

# Run the web application (from PetsHome.UI directory)
dotnet run --project PetsHome.UI/PetsHome.UI.csproj

# Build in Release mode
dotnet build PetsHome.sln -c Release
```

### Restore Dependencies

```bash
# Restore all NuGet packages
dotnet restore PetsHome.sln
```

### Database Configuration

Database connection is configured in `PetsHome.UI/appsettings.json`:
- Connection string key: `PetsHomeConnectionString`
- Currently configured for local SQL Server: `DESKTOP-06VA2CI`
- Database name: `PETSHOMEDB`

### Logging

The application uses Serilog configured in `appsettings.json`:
- Console logging enabled
- File logging to `Logs/mascota-log-.txt` with daily rolling interval
- Specific log levels can be configured per namespace (e.g., MascotaService uses Debug level)
- ErrorHandlerMiddleware is present but currently commented out in Startup.cs

## Frontend Architecture

### DataTables Integration

Each entity has a corresponding JavaScript file in `wwwroot/js/pages/` (e.g., `mascota.js`, `adopcion.js`, `empleado.js`):
- Custom DataTable initialization scripts
- AJAX-based CRUD operations
- Export functionality integration
- Files in git status show recent UI modernization work

### Screen Types and DataTable Initialization

The application screens are divided into three categories, each using different DataTable initialization scripts:

#### Catalog Screens (use `datatable.catalogs.init.js`)
Simple catalog/lookup tables with basic CRUD operations:
- Categoria (Category)
- EmpleadosCargo (Employee Positions)
- Procedencia (Origin/Source)
- Raza (Breed)
- Vacuna (Vaccine)

#### Partial Screens (use `datatable.partials.init.js`)
Detail/child entity screens typically embedded in master-detail relationships:
- Entrada (Entry)
- EntradasDetalle (Entry Details)
- Inventario (Inventory)
- InventarioDetalle (Inventory Details)
- Localidad (Locality)
- Municipio (Municipality)

#### Main Screens (use `datatable.init.js`)
Full-featured entity management screens with complex functionality:
- Adopcion (Adoption)
- Base
- Empleado (Employee)
- Evento (Event)
- HistorialMedico (Medical History)
- Home
- Item
- Mascota (Pet)
- Persona (Person)
- Refugio (Shelter)
- Solicitud (Application/Request)
- Voluntario (Volunteer)

### Shared Components

- `wwwroot/js/components/datatable/datatable.init.js` - Main entity DataTable initialization
- `wwwroot/js/components/datatable/datatable.catalogs.init.js` - Catalog-specific DataTable setup
- `wwwroot/js/components/datatable/datatable.partials.init.js` - Partial/detail entity DataTable setup

### Recent UI Work

Based on recent commits and modified files:
- UI modernization in progress (branch: nueva-interfaz)
- Modern design being applied across catalog pages
- Export buttons and card views being added
- Python scripts in root directory appear to be automation tools for UI updates (not part of main application)

## Common Entities

The system manages these core entities:
- **Mascota** (Pet): Main entity with photo support, linked to refugio and medical history
- **Adopcion** (Adoption): Adoption records
- **Refugio** (Shelter): Shelter locations
- **Empleado** (Employee): Staff members with cargo (position/role)
- **Voluntario** (Volunteer): Volunteer records
- **HistorialMedico** (Medical History): Pet medical records with vacunas (vaccines)
- **Evento** (Event): Shelter events
- **Inventario** (Inventory): Inventory management with entries and details
- **Solicitud** (Request/Application): Adoption applications

## File Organization Notes

- ViewModels configuration: `MascotaViewModel` has a file path configuration in appsettings: `Filepath:pathMascotaImage`
- Image storage location: `C:\PetsHome_Files\Mascotas`
- Controllers use Razor views with the same name pattern (e.g., AdopcionController → Views/Adopcion/)

## Technology Stack

- .NET Core 3.1 (project targets `netcoreapp3.1`)
- ASP.NET Core MVC with Razor views
- Entity Framework Core 5.0.11
- Dapper 2.0.90
- AutoMapper 11.0.1
- Serilog 9.0.0
- jQuery DataTables (frontend)
- SixLabors.ImageSharp 1.0.4 (image processing)

## Notes

- The application runs on .NET 9 SDK but targets .NET Core 3.1 runtime
- Error handling middleware exists but is currently disabled in Startup.cs
- Python scripts in root are likely development automation tools, not part of the deployed application
- Recent work focuses on UI modernization across all catalog and main entity pages
