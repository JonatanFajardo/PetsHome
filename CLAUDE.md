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
