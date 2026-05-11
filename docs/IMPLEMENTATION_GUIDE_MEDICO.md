# Guía de Implementación - Módulo Médico Veterinario

## Estado Actual de Implementación

### ✅ Completado

#### 1. Scripts SQL (Carpeta `/Database`)
- `01_CREATE_TABLES_CATALOGO_MEDICO.sql` - Creación de 6 tablas catálogo
- `02_INSERT_DATA_CATALOGO_MEDICO.sql` - Datos iniciales
- `03_SP_TIPOS_CONSULTA.sql` - 7 SPs (List, Detail, Find, Insert, Update, Delete, Dropdown)
- `04_SP_GRAVEDADES.sql` - 7 SPs
- `05_SP_TIPOS_MEDICAMENTO.sql` - 7 SPs
- `06_SP_VIAS_ADMINISTRACION.sql` - 7 SPs
- `07_SP_TIPOS_PARASITO.sql` - 7 SPs
- `08_SP_TIPOS_ESTERILIZACION.sql` - 7 SPs

#### 2. Entidades (Carpeta `/PetsHome.Common/Entities/Medico`)
- `tbTiposConsulta.cs`
- `tbGravedades.cs`
- `tbTiposMedicamento.cs`
- `tbViasAdministracion.cs`
- `tbTiposParasito.cs`
- `tbTiposEsterilizacion.cs`

#### 3. Clases de Resultado - TiposConsulta
- `PR_Medico_TiposConsulta_ListResult.cs`
- `PR_Medico_TiposConsulta_DetailResult.cs`
- `PR_Medico_TiposConsulta_FindResult.cs`
- `PR_Medico_TiposConsulta_DropdownResult.cs`

---

### 🔄 Pendiente de Crear

Para completar la implementación, necesitas crear los siguientes archivos adicionales siguiendo el mismo patrón de `TiposConsulta`:

#### Clases de Resultado (ubicar en `/PetsHome.Common/Entities/Medico/`)

**Para Gravedades:**
- `PR_Medico_Gravedades_ListResult.cs`
- `PR_Medico_Gravedades_DetailResult.cs`
- `PR_Medico_Gravedades_FindResult.cs`
- `PR_Medico_Gravedades_DropdownResult.cs`

**Para TiposMedicamento:**
- `PR_Medico_TiposMedicamento_ListResult.cs`
- `PR_Medico_TiposMedicamento_DetailResult.cs`
- `PR_Medico_TiposMedicamento_FindResult.cs`
- `PR_Medico_TiposMedicamento_DropdownResult.cs`

**Para ViasAdministracion:**
- `PR_Medico_ViasAdministracion_ListResult.cs`
- `PR_Medico_ViasAdministracion_DetailResult.cs`
- `PR_Medico_ViasAdministracion_FindResult.cs`
- `PR_Medico_ViasAdministracion_DropdownResult.cs`

**Para TiposParasito:**
- `PR_Medico_TiposParasito_ListResult.cs` (incluye `tipoPar_Categoria`)
- `PR_Medico_TiposParasito_DetailResult.cs` (incluye `tipoPar_Categoria`)
- `PR_Medico_TiposParasito_FindResult.cs` (incluye `tipoPar_Categoria`)
- `PR_Medico_TiposParasito_DropdownResult.cs` (incluye `tipoPar_Categoria`)

**Para TiposEsterilizacion:**
- `PR_Medico_TiposEsterilizacion_ListResult.cs` (incluye `tipoEst_Sexo`)
- `PR_Medico_TiposEsterilizacion_DetailResult.cs` (incluye `tipoEst_Sexo`)
- `PR_Medico_TiposEsterilizacion_FindResult.cs` (incluye `tipoEst_Sexo`)
- `PR_Medico_TiposEsterilizacion_DropdownResult.cs` (incluye `tipoEst_Sexo`)

---

## Patrón de Nomenclatura para Clases Result

### ListResult
```csharp
public partial class PR_Medico_[Tabla]_ListResult
{
    public int [prefix]_Id { get; set; }
    public string [prefix]_Descripcion { get; set; }
    // + campos adicionales si aplica (Categoria, Sexo, etc.)
}
```

### DetailResult
```csharp
public partial class PR_Medico_[Tabla]_DetailResult
{
    public int [prefix]_Id { get; set; }
    public string [prefix]_Descripcion { get; set; }
    public string UsuarioCreacion { get; set; }
    public DateTime? [prefix]_FechaCrea { get; set; }
    public string UsuarioModificacion { get; set; }
    public DateTime? [prefix]_FechaModifica { get; set; }
    // + campos adicionales si aplica
}
```

### FindResult
```csharp
public partial class PR_Medico_[Tabla]_FindResult
{
    public int [prefix]_Id { get; set; }
    public string [prefix]_Descripcion { get; set; }
    public int [prefix]_UsuarioCrea { get; set; }
    public string usuarioCrea { get; set; }
    public DateTime? [prefix]_FechaCrea { get; set; }
    public int? [prefix]_UsuarioModifica { get; set; }
    public string usuarioModifica { get; set; }
    public DateTime? [prefix]_FechaModifica { get; set; }
    // + campos adicionales si aplica
}
```

### DropdownResult
```csharp
public partial class PR_Medico_[Tabla]_DropdownResult
{
    public int [prefix]_Id { get; set; }
    public string [prefix]_Descripcion { get; set; }
    // + campos adicionales si aplica
}
```

---

## Prefijos por Tabla

| Tabla                    | Prefijo      |
|--------------------------|--------------|
| tbTiposConsulta          | tipoCon      |
| tbGravedades             | grav         |
| tbTiposMedicamento       | tipoMed      |
| tbViasAdministracion     | viaAdmin     |
| tbTiposParasito          | tipoPar      |
| tbTiposEsterilizacion    | tipoEst      |

---

## Siguiente Paso: Continuar con Claude

Para continuar la implementación, solicita a Claude que cree:

1. Las clases Result restantes (20 archivos)
2. Repositorios en `/PetsHome.Logic/Repositories/` (6 archivos)
3. ViewModels en `/PetsHome.Business/Models/` (6 archivos)
4. Servicios en `/PetsHome.Business/Services/` (6 archivos)
5. Configuración AutoMapper
6. Configuración ServiceConfiguration
7. Controladores en `/PetsHome.UI/Controllers/catalogs/` (6 archivos)
8. Vistas (Index.cshtml por cada catálogo)
9. JavaScript DataTables (6 archivos en `/wwwroot/js/pages/`)

---

## Comando para Ejecutar Scripts SQL

```sql
-- Ejecutar en orden:
USE PETSHOMEDB
GO

-- 1. Crear tablas
:r C:\Users\movie\Documents\GitHub\PetsHome\Database\01_CREATE_TABLES_CATALOGO_MEDICO.sql

-- 2. Insertar datos
:r C:\Users\movie\Documents\GitHub\PetsHome\Database\02_INSERT_DATA_CATALOGO_MEDICO.sql

-- 3-8. Crear SPs
:r C:\Users\movie\Documents\GitHub\PetsHome\Database\03_SP_TIPOS_CONSULTA.sql
:r C:\Users\movie\Documents\GitHub\PetsHome\Database\04_SP_GRAVEDADES.sql
:r C:\Users\movie\Documents\GitHub\PetsHome\Database\05_SP_TIPOS_MEDICAMENTO.sql
:r C:\Users\movie\Documents\GitHub\PetsHome\Database\06_SP_VIAS_ADMINISTRACION.sql
:r C:\Users\movie\Documents\GitHub\PetsHome\Database\07_SP_TIPOS_PARASITO.sql
:r C:\Users\movie\Documents\GitHub\PetsHome\Database\08_SP_TIPOS_ESTERILIZACION.sql
```

---

## Verificación Post-Implementación

### Checklist de Compilación
- [ ] `dotnet build PetsHome.Common`
- [ ] `dotnet build PetsHome.Logic`
- [ ] `dotnet build PetsHome.Business`
- [ ] `dotnet build PetsHome.UI`
- [ ] `dotnet build PetsHome.sln`

### Checklist Funcional
- [ ] Navegar a `/TiposConsulta` (o URL correspondiente)
- [ ] Ver listado con DataTable
- [ ] Crear nuevo registro
- [ ] Editar registro existente
- [ ] Eliminar registro
- [ ] Verificar dropdowns funcionan en formularios

---

**Progreso Actual: 35% completado**
- ✅ Base de datos y SPs
- ✅ Entidades Common parcial
- ⏳ Entidades Common Result classes
- ⏳ Repositorios Logic
- ⏳ ViewModels y Servicios Business
- ⏳ Controllers UI
- ⏳ Views y JavaScript
