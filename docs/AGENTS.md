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

---

## Development Guide: Adding New Fields to Existing Tables

This guide provides a comprehensive step-by-step process for adding new fields to any table in the PetsHome system. Follow this pattern to maintain consistency across the layered architecture.

### PHASE 1: DATABASE

#### 1.1. Add Columns to Table
```sql
ALTER TABLE [Schema].[tbTableName]
ADD field_NewField1 data_type size NULL/NOT NULL,
    field_NewField2 data_type size NULL/NOT NULL;
```

**Example with Razas:**
```sql
ALTER TABLE [Refugio].[tbRazas]
ADD raza_Tamano varchar(20) NULL,
    raza_TipoAnimal varchar(50) NULL,
    raza_TipoPelaje varchar(30) NULL,
    raza_ImagenUrl varchar(500) NULL;
```

#### 1.2. Update Existing Data (if applicable)
```sql
UPDATE [Schema].[tbTableName]
SET field_NewField1 = 'value',
    field_NewField2 = 'value'
WHERE condition;
```

**Example:**
```sql
UPDATE Refugio.tbRazas
SET raza_Tamano = 'Grande',
    raza_TipoAnimal = 'Perro',
    raza_TipoPelaje = 'Corto y denso'
WHERE raza_Id = 4;
```

#### 1.3. Update Stored Procedures

Update the 5 main stored procedures (if they exist):

**a) PR_[Schema]_[Table]_List** - Lists all records
```sql
ALTER PROCEDURE [Schema].[PR_Schema_Table_List]
AS
BEGIN
    SELECT  ROW_NUMBER() OVER(ORDER BY table_Id ASC) AS Fila,
            table_Id,
            table_ExistingField1,
            table_ExistingField2,
            table_NewField1,  -- ADD
            table_NewField2   -- ADD
    FROM [Schema].[tbTable]
    WHERE table_EsEliminado != 1
END
```

**b) PR_[Schema]_[Table]_Detail** - Detail with audit info
```sql
ALTER PROCEDURE [Schema].[PR_Schema_Table_Detail]
AS
BEGIN
    SELECT  table_Id,
            table_ExistingField1,
            table_ExistingField2,
            table_NewField1,  -- ADD
            table_NewField2,  -- ADD
            usuarioCrea.usu_Nombre AS UsuarioCreacion,
            table_FechaCrea,
            usuarioModifica.usu_Nombre AS UsuarioModificacion,
            table_FechaModifica
    FROM [Schema].[tbTable] AS table
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON table.table_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON table.table_UsuarioModifica = usuarioModifica.usu_Id
    WHERE table_EsEliminado != 1
END
```

**c) PR_[Schema]_[Table]_Find** - Find by ID
```sql
ALTER PROCEDURE [Schema].[PR_Schema_Table_Find]
@table_Id INT
AS
BEGIN
    SELECT  table_Id,
            table_ExistingField1,
            table_ExistingField2,
            table_NewField1,  -- ADD
            table_NewField2,  -- ADD
            table_UsuarioCrea,
            usuarioCrea.Usu_Nombre AS usuarioCrea,
            table_FechaCrea,
            table_UsuarioModifica,
            usuarioModifica.Usu_Nombre AS usuarioModifica,
            table_FechaModifica
    FROM [Schema].[tbTable] as table
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioCrea
        ON table.table_UsuarioCrea = usuarioCrea.usu_Id
    LEFT JOIN [Seguridad].[tbUsuarios] AS usuarioModifica
        ON table.table_UsuarioModifica = usuarioModifica.usu_Id
    WHERE table_EsEliminado != 1
    AND table_Id = @table_Id
END
```

**d) PR_[Schema]_[Table]_Insert** - Insert new record
```sql
ALTER PROCEDURE [Schema].[PR_Schema_Table_Insert]
    @table_ExistingField1 data_type,
    @table_ExistingField2 data_type,
    @table_NewField1 data_type,      -- ADD PARAMETER
    @table_NewField2 data_type,      -- ADD PARAMETER
    @table_UsuarioCrea INT
AS
BEGIN
    INSERT INTO [Schema].[tbTable]
    (
        table_ExistingField1,
        table_ExistingField2,
        table_NewField1,              -- ADD COLUMN
        table_NewField2,              -- ADD COLUMN
        table_UsuarioCrea,
        table_FechaCrea
    )
    VALUES
    (
        @table_ExistingField1,
        @table_ExistingField2,
        @table_NewField1,             -- ADD VALUE
        @table_NewField2,             -- ADD VALUE
        @table_UsuarioCrea,
        GETDATE()
    )
END
```

**e) PR_[Schema]_[Table]_Update** - Update record
```sql
ALTER PROCEDURE [Schema].[PR_Schema_Table_Update]
    @table_Id INT,
    @table_ExistingField1 data_type,
    @table_ExistingField2 data_type,
    @table_NewField1 data_type,      -- ADD PARAMETER
    @table_NewField2 data_type,      -- ADD PARAMETER
    @table_UsuarioModifica INT
AS
BEGIN
    UPDATE [Schema].[tbTable]
    SET table_ExistingField1 = @table_ExistingField1,
        table_ExistingField2 = @table_ExistingField2,
        table_NewField1 = @table_NewField1,        -- ADD
        table_NewField2 = @table_NewField2,        -- ADD
        table_UsuarioModifica = @table_UsuarioModifica,
        table_FechaModifica = GETDATE()
    WHERE table_Id = @table_Id
END
```

---

### PHASE 2: COMMON LAYER (PetsHome.Common)

#### 2.1. Update Entity Class
**Location:** `PetsHome.Common/Entities/[Schema]/tb[Table].cs`

```csharp
public class tbTable
{
    public int table_Id { get; set; }
    public string table_ExistingField1 { get; set; }
    public string table_ExistingField2 { get; set; }

    // ADD NEW PROPERTIES
    public string table_NewField1 { get; set; }
    public string table_NewField2 { get; set; }

    public int? table_UsuarioCrea { get; set; }
    public DateTime? table_FechaCrea { get; set; }
    public int? table_UsuarioModifica { get; set; }
    public DateTime? table_FechaModifica { get; set; }
}
```

#### 2.2. Update Stored Procedure Result Classes
**Location:** `PetsHome.Common/Entities/[Schema]/PR_[Schema]_[Table]_[Operation]Result.cs`

**For _ListResult:**
```csharp
public class PR_Schema_Table_ListResult
{
    public int Fila { get; set; }
    public int table_Id { get; set; }
    public string table_ExistingField1 { get; set; }
    public string table_ExistingField2 { get; set; }

    // ADD
    public string table_NewField1 { get; set; }
    public string table_NewField2 { get; set; }
}
```

**For _FindResult and _DetailResult:**
```csharp
public class PR_Schema_Table_FindResult
{
    public int table_Id { get; set; }
    public string table_ExistingField1 { get; set; }
    public string table_ExistingField2 { get; set; }

    // ADD
    public string table_NewField1 { get; set; }
    public string table_NewField2 { get; set; }

    public int? table_UsuarioCrea { get; set; }
    public string usuarioCrea { get; set; }
    public DateTime? table_FechaCrea { get; set; }
    public int? table_UsuarioModifica { get; set; }
    public string usuarioModifica { get; set; }
    public DateTime? table_FechaModifica { get; set; }
}
```

---

### PHASE 3: LOGIC LAYER (PetsHome.Logic)

#### 3.1. Update Repository
**Location:** `PetsHome.Logic/Repositories/[Table]Repository.cs`

**Update Insert method:**
```csharp
public RequestStatus Insert(tbTable item)
{
    string query = "[Schema].[PR_Schema_Table_Insert]";

    var parametros = new DynamicParameters();
    parametros.Add("@table_ExistingField1", item.table_ExistingField1);
    parametros.Add("@table_ExistingField2", item.table_ExistingField2);
    // ADD
    parametros.Add("@table_NewField1", item.table_NewField1);
    parametros.Add("@table_NewField2", item.table_NewField2);
    parametros.Add("@table_UsuarioCrea", item.table_UsuarioCrea);

    using var db = new SqlConnection(PetsHomeDbContext.ConnectionString);
    var result = db.Execute(query, parametros, commandType: CommandType.StoredProcedure);

    return new RequestStatus { CodeStatus = result, MessageStatus = result > 0 ? "Éxito" : "Error" };
}
```

**Update Update method:**
```csharp
public RequestStatus Update(tbTable item)
{
    string query = "[Schema].[PR_Schema_Table_Update]";

    var parametros = new DynamicParameters();
    parametros.Add("@table_Id", item.table_Id);
    parametros.Add("@table_ExistingField1", item.table_ExistingField1);
    parametros.Add("@table_ExistingField2", item.table_ExistingField2);
    // ADD
    parametros.Add("@table_NewField1", item.table_NewField1);
    parametros.Add("@table_NewField2", item.table_NewField2);
    parametros.Add("@table_UsuarioModifica", item.table_UsuarioModifica);

    using var db = new SqlConnection(PetsHomeDbContext.ConnectionString);
    var result = db.Execute(query, parametros, commandType: CommandType.StoredProcedure);

    return new RequestStatus { CodeStatus = result, MessageStatus = result > 0 ? "Éxito" : "Error" };
}
```

**Note:** The `List()`, `Find(id)`, and `Detail()` methods typically don't need changes as Dapper automatically maps to the updated Result classes.

---

### PHASE 4: BUSINESS LAYER (PetsHome.Business)

#### 4.1. Update ViewModel
**Location:** `PetsHome.Business/Models/[Table]ViewModel.cs`

```csharp
public class TableViewModel
{
    public int table_Id { get; set; }

    [Required(ErrorMessage = "El campo es requerido")]
    [Display(Name = "Campo Existente 1")]
    public string table_ExistingField1 { get; set; }

    [Display(Name = "Campo Existente 2")]
    public string table_ExistingField2 { get; set; }

    // ADD NEW PROPERTIES
    [Display(Name = "Nuevo Campo 1")]
    public string table_NewField1 { get; set; }

    [Display(Name = "Nuevo Campo 2")]
    public string table_NewField2 { get; set; }
}
```

#### 4.2. Update AutoMapper Configuration
**Location:** `PetsHome.Business/AutoMapperConfig.cs`

```csharp
// In the Configure method, find the entity mapping and update it:

CreateMap<tbTable, TableViewModel>()
    .ForMember(dest => dest.table_NewField1, opt => opt.MapFrom(src => src.table_NewField1))
    .ForMember(dest => dest.table_NewField2, opt => opt.MapFrom(src => src.table_NewField2))
    .ReverseMap();

CreateMap<PR_Schema_Table_ListResult, TableViewModel>()
    .ForMember(dest => dest.table_NewField1, opt => opt.MapFrom(src => src.table_NewField1))
    .ForMember(dest => dest.table_NewField2, opt => opt.MapFrom(src => src.table_NewField2));

CreateMap<PR_Schema_Table_FindResult, TableViewModel>()
    .ForMember(dest => dest.table_NewField1, opt => opt.MapFrom(src => src.table_NewField1))
    .ForMember(dest => dest.table_NewField2, opt => opt.MapFrom(src => src.table_NewField2));
```

**Note:** If AutoMapper doesn't find conflicts, mapping can be automatic without specifying `ForMember`.

#### 4.3. Update Service (if necessary)
**Location:** `PetsHome.Business/Services/[Table]Service.cs`

Generally doesn't need changes, but if there's specific business logic for the new fields, add it here.

---

### PHASE 5: PRESENTATION LAYER (PetsHome.UI)

#### 5.1. Update Create View
**Location:** `PetsHome.UI/Views/[Controller]/Create.cshtml`

```html
@model PetsHome.Business.Models.TableViewModel

<div class="form-group">
    <label asp-for="table_ExistingField1" class="control-label"></label>
    <input asp-for="table_ExistingField1" class="form-control" />
    <span asp-validation-for="table_ExistingField1" class="text-danger"></span>
</div>

<!-- ADD NEW FIELDS -->
<div class="form-group">
    <label asp-for="table_NewField1" class="control-label"></label>
    <input asp-for="table_NewField1" class="form-control" />
    <span asp-validation-for="table_NewField1" class="text-danger"></span>
</div>

<div class="form-group">
    <label asp-for="table_NewField2" class="control-label"></label>
    <input asp-for="table_NewField2" class="form-control" />
    <span asp-validation-for="table_NewField2" class="text-danger"></span>
</div>
```

#### 5.2. Update Edit View
**Location:** `PetsHome.UI/Views/[Controller]/Edit.cshtml`

Similar to Create.cshtml, add the same form fields.

#### 5.3. Update DataTable JavaScript
**Location:** `PetsHome.UI/wwwroot/js/pages/[table].js`

```javascript
var columns = [
    { "data": "table_Id", "name": "Id", "autoWidth": true },
    { "data": "table_ExistingField1", "name": "Campo 1", "autoWidth": true },
    { "data": "table_ExistingField2", "name": "Campo 2", "autoWidth": true },
    // ADD NEW COLUMNS
    { "data": "table_NewField1", "name": "Nuevo Campo 1", "autoWidth": true },
    { "data": "table_NewField2", "name": "Nuevo Campo 2", "autoWidth": true },
    {
        "data": "table_Id",
        "render": function (data) {
            return `<button class="btn btn-sm btn-primary" onclick="Edit(${data})">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="Delete(${data})">
                        <i class="fas fa-trash"></i>
                    </button>`;
        },
        "orderable": false,
        "searchable": false,
        "width": "90px"
    }
];
```

#### 5.4. Update Details View (optional)
**Location:** `PetsHome.UI/Views/[Controller]/Details.cshtml`

```html
<dl class="row">
    <dt class="col-sm-3">@Html.DisplayNameFor(model => model.table_ExistingField1)</dt>
    <dd class="col-sm-9">@Html.DisplayFor(model => model.table_ExistingField1)</dd>

    <!-- ADD -->
    <dt class="col-sm-3">@Html.DisplayNameFor(model => model.table_NewField1)</dt>
    <dd class="col-sm-9">@Html.DisplayFor(model => model.table_NewField1)</dd>

    <dt class="col-sm-3">@Html.DisplayNameFor(model => model.table_NewField2)</dt>
    <dd class="col-sm-9">@Html.DisplayFor(model => model.table_NewField2)</dd>
</dl>
```

---

### PHASE 6: TESTING AND VALIDATION

#### 6.1. Build the Project
```bash
dotnet build PetsHome.sln
```

#### 6.2. Run the Application
```bash
dotnet run --project PetsHome.UI/PetsHome.UI.csproj
```

#### 6.3. Test Functionality
- [ ] List records (verify new fields display)
- [ ] Create new record (verify new fields save)
- [ ] Edit existing record (verify new fields update)
- [ ] View details (verify new fields display)
- [ ] Export data (verify new fields are included)

---

## COMPLETE CHECKLIST

### Database
- [ ] Add columns to table with ALTER TABLE
- [ ] Update existing data (if applicable)
- [ ] Update PR_List
- [ ] Update PR_Detail
- [ ] Update PR_Find
- [ ] Update PR_Insert
- [ ] Update PR_Update
- [ ] Test all stored procedures

### PetsHome.Common
- [ ] Update tb[Table] class
- [ ] Update PR_[Table]_ListResult
- [ ] Update PR_[Table]_FindResult
- [ ] Update PR_[Table]_DetailResult (if exists)

### PetsHome.Logic
- [ ] Update Insert method in Repository
- [ ] Update Update method in Repository
- [ ] Verify List, Find, Detail methods (usually automatic)

### PetsHome.Business
- [ ] Update [Table]ViewModel
- [ ] Update AutoMapperConfig
- [ ] Update Service (if business logic needed)

### PetsHome.UI
- [ ] Update Create.cshtml
- [ ] Update Edit.cshtml
- [ ] Update Details.cshtml (optional)
- [ ] Update [table].js (DataTable columns)
- [ ] Update Controller (if necessary)

### Testing
- [ ] Build without errors
- [ ] Test create new record
- [ ] Test edit existing record
- [ ] Test list with new fields
- [ ] Test details with new fields
- [ ] Test export (if applicable)

---

## IMPORTANT NOTES

1. **Naming Convention:** Maintain the `[table]_[FieldName]` pattern in the database
2. **Data Types:** Ensure consistency between SQL Server and C#
3. **NULL vs NOT NULL:** Consider if the field is required before deciding
4. **AutoMapper:** In many cases it maps automatically if names match
5. **Validations:** Add `[Required]`, `[StringLength]`, etc. as needed
6. **Display Names:** Use `[Display(Name = "...")]` for user-friendly names in UI
7. **Transaction Safety:** Consider using transactions for complex updates
8. **Backward Compatibility:** Ensure existing functionality continues to work after changes

This guide applies to any table in the system following the same architectural pattern.
