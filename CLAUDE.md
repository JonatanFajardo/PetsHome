# PetsHome — Guía para Claude Code

## Arquitectura del proyecto

4 capas, sin Entity Framework (solo Dapper + Stored Procedures):

```
PetsHome.UI          → Controllers, Views, Filters (PantallaAuthorize)
PetsHome.Business    → Services, ViewModels, AutoMapper, ServiceConfiguration
PetsHome.Logic       → Repositories (IGenericRepository<T>)
PetsHome.Common      → Entities (tbX.cs, PR_Schema_Entity_OpResult.cs)
PetsHome.DataAccess  → DbApp helper (Select, Find, Detail, ExecuteWithResult, Dropdown)
```

Base de datos: SQL Server, schema `PETSHOMEDB`.
Schemas SQL: `Refugio`, `Medico`, `Inventario`, `Seguridad`, `General`.

---

## Convenciones clave

### Naming
- Tablas:       `tb{Entity}` — ej: `tbCitaMedica`
- SPs:          `[Schema].[PR_{Schema}_{Entity}_{Op}]` — ej: `[Medico].[PR_Medico_CitaMedica_List]`
- Result class: `PR_{Schema}_{Entity}_{Op}Result` — namespace `PetsHome.Common.Entities`
- ViewModels:   `{Entity}ListViewModel`, `{Entity}FormViewModel`, `{Entity}FindViewModel`, `{Entity}DetailViewModel`
- Prefijos:     cada entidad tiene un prefijo corto (ej: `cita_`, `masc_`, `vac_`)

### Audit fields (toda entidad los tiene)
```csharp
{prefix}_EsEliminado    bool
{prefix}_UsuarioCrea    int
{prefix}_FechaCrea      DateTime
{prefix}_UsuarioModifica int?
{prefix}_FechaModifica  DateTime?
```

### Seguridad
- `[Authorize]` + `[PantallaAuthorize("nombre pantalla")]` en clase del controller
- `[PantallaAuthorize("nombre pantalla", "insertar|editar|eliminar")]` en actions específicas
- Claims se actualizan solo al hacer login — cambios de permisos requieren re-login
- Si aparece "Acceso Denegado": ejecutar `FIX_asignar_pantallas_admin.sql` y re-loguear

### Registro de dependencias
- `ServiceConfiguration.cs` → `AddLogicLayer` para repos, `AddBusinessLogic` para services
- `MappingProfileExtensions.cs` → `CreateMap<PR_..._Result, ...ViewModel>().ReverseMap()`

### URLs en JS
No usar rutas hardcodeadas en archivos `.js`. Patrón `window._urls`:
```html
<script>
  window._urls = {
    accionController: '@Url.Action("Accion", "Controller")',
  };
</script>
<script src="~/js/pages/mi-pagina.js"></script>
```

---

## Scripts de automatización

### 1. `tools/map_project.py` — Contexto del proyecto
Escanea toda la solución y genera `PROJECT_CONTEXT.md` (arquitectura, controllers, services, repos, VMs, convenciones).

```bash
python tools/map_project.py
```

Ejecutar una vez por sesión si se hicieron cambios estructurales recientes.

---

### 2. `tools/scaffold_backend.py` — Backend completo en segundos

Genera 11 archivos y parchea 2 archivos compartidos automáticamente:

| Genera | Parchea automáticamente |
|--------|------------------------|
| SP SQL | `ServiceConfiguration.cs` (+AddScoped) |
| 4 Result classes | `MappingProfileExtensions.cs` (+CreateMap) |
| `tb{Entity}.cs` | |
| ViewModels (List/Form/Find/Detail/Dropdown) | |
| Repository | |
| Service | |
| Controller | |
| `Views/{Entity}/Index.cshtml` | |

**Si el archivo ya existe → lo omite (no sobreescribe trabajo existente).**

```bash
python tools/scaffold_backend.py MiEntidad \
    --schema Medico \
    --prefix ent \
    --pantalla "Listado de..." \
    --fields "ent_Nombre:string ent_Fecha:datetime ent_Activo:bit"
```

Tipos de campo: `string`, `int`, `int?`, `decimal`, `decimal?`, `datetime`, `datetime?`, `bit`, `bool`

Pasos manuales que quedan después del scaffold:
1. Ejecutar el `.sql` generado en SQL Server
2. Ejecutar `FIX_asignar_pantallas_admin.sql` si la pantalla es nueva
3. Agregar link al sidebar en `_Layout.cshtml`

---

### 3. `tools/html_to_razor.py` — HTML estático → Razor / Extraer CSS+JS

**Modo HTML** (diseño nuevo → vista Razor):
```bash
python tools/html_to_razor.py diseño.html \
    --controller MiEntidad \
    --action Index \
    --pantalla "Listado de..." \
    --slug mi-entidad
```

Genera: `Views/{Controller}/{Action}.cshtml` + `wwwroot/css/{slug}.css` + `wwwroot/js/pages/{slug}.js`
Las líneas con `@Url.Action` quedan como `window._urls` en script inline.

**Modo Razor** (extraer CSS/JS de una vista existente):
```bash
python tools/html_to_razor.py PetsHome.UI/Views/X/Vista.cshtml --slug mi-slug
```

- Extrae `<style>` → `wwwroot/css/{slug}.css`, reemplaza con `<link>`
- Extrae `<script>` inline → `wwwroot/js/pages/{slug}.js`
- Las líneas con `@` (Razor) se conservan como script inline en el `.cshtml`
- Modifica el `.cshtml` in-place

---

## Flujo para implementar una nueva pantalla

```
1. python tools/map_project.py                    ← contexto actualizado
2. python tools/scaffold_backend.py ...           ← genera los 11 archivos
3. python tools/html_to_razor.py diseño.html ...  ← convierte el HTML (si hay diseño)
4. Ejecutar el SP en SQL Server
5. Re-loguear si es pantalla nueva
6. Agregar link al sidebar
```

---

## Plantillas de prompt para sesiones nuevas

> **Regla general para Claude:** cuando el prompt indica scripts a ejecutar,
> Claude debe derivar TODOS los parámetros faltantes por sí solo (sin preguntar)
> usando las convenciones del proyecto y leyendo el HTML de diseño si existe.
> Solo preguntar si hay ambigüedad real que no se pueda resolver con las convenciones.

---

### Pantalla CRUD completa (`tools/scaffold_backend.py`)

```
Lee PROJECT_CONTEXT.md antes de hacer cualquier cambio.

## Nueva pantalla CRUD: <nombre-slug>

### Scripts a ejecutar
python tools/scaffold_backend.py <Entidad> \
    --schema <Schema> --prefix <prefijo> \
    --pantalla "<nombre legible>" --fields "<campo:tipo ...>"

# Si hay diseño HTML:
python tools/html_to_razor.py <archivo.html> --controller <Entidad> --action Index

python tools/bind_razor.py <Entidad> Index
# → genera bind_task_<Entidad>_Index.md → pegarlo en Claude.ai → vista conectada ✅

### Estado
- [ ] SP ejecutado en SQL Server
- [ ] Re-login para refrescar claims

### Lo que necesito hoy
<describe qué falta, o déjalo vacío para que Claude ejecute los scripts>
```

---

### Pantalla de solo lectura (`tools/scaffold.py`)
> Usar cuando la pantalla **no tiene CRUD propio**: reportes, dashboards, perfiles, vistas de detalle agregado, resúmenes, etc.

```
Lee PROJECT_CONTEXT.md antes de hacer cualquier cambio.

## Nueva pantalla: <nombre-slug>

### Scripts a ejecutar
python tools/scaffold.py <Entidad> \
    --schema <Schema> \
    --pantalla "<nombre legible>" \
    --grupo <GrupoSidebar> \
    [--param <campo:tipo>] \
    --section <Seccion1> "<campos>" \
    --section <Seccion2> "<campos>"

python tools/html_to_razor.py "<ruta/al/diseño.html>" \
    --controller <Entidad> --action Index

python tools/bind_razor.py <Entidad> Index
# → genera bind_task_<Entidad>_Index.md → pegarlo en Claude.ai → vista conectada ✅

### Estado
- [ ] SP completado y ejecutado en SQL Server
- [ ] Re-login para refrescar claims

### Lo que necesito hoy
<describe qué falta, o déjalo vacío para que Claude ejecute los scripts>
```

> **Cómo Claude infiere los parámetros cuando no se llenan:**
> - `<Entidad>` → PascalCase del slug (ej: `reporte-adopciones` → `ReporteAdopciones`)
> - `<Schema>` → del PROJECT_CONTEXT.md según la entidad relacionada
> - `<nombre legible>` → `"Reporte de adopciones"` (slug humanizado)
> - `<GrupoSidebar>` → del PROJECT_CONTEXT.md según el grupo del sidebar
> - `--section` y campos → leídos del HTML de diseño proporcionado

---

## Archivos de referencia útiles

| Archivo | Propósito |
|---------|-----------|
| `PROJECT_CONTEXT.md` | Mapa completo de la solución (regenerar con `tools/map_project.py`) |
| `Database/FIX_asignar_pantallas_admin.sql` | Asignar todas las pantallas al rol Administrador |
| `PetsHome.Business/ServiceConfiguration.cs` | Registro de repos y services |
| `PetsHome.Business/Extensions/MappingProfileExtensions.cs` | Mapeos AutoMapper |
| `PetsHome.UI/Views/Shared/_Layout.cshtml` | Layout principal, sidebar, secciones Styles/Scripts |
