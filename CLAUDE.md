# PetsHome — Guía para Claude Code

## Estilo de respuesta
Responder corto, en una sola frase cuando se pueda. Ampliar solo si el usuario lo pide.

**Preferir SIEMPRE lo visual sobre el texto largo.** El usuario lee rápido y odia los muros de texto. Formato por defecto:
- **Diagramas ASCII** en bloque de código para flujos, decisiones y árboles (`│ ▼ ├ └ ┌ ┐`).
- **Mockups ASCII** (cajas `┌─┐`) para mostrar cómo se vería una pantalla: ANTES / DESPUÉS.
- **Opciones en lista de 1 línea** con etiqueta de esfuerzo `[poco]/[medio]/[mucho]` y marcar la recomendada con `★`.
- **Tablas compactas** en vez de párrafos cuando se comparan cosas.
- Mínimo texto alrededor: una frase antes del visual, una pregunta concreta al final. Nada de explicaciones largas salvo que las pida.

## Mínima interferencia (aplica a TODO, no solo dropdowns)
Interferir lo menos posible en lo que ya está estructurado. Antes de construir algo nuevo, reutilizar lo que ya existe y funciona.

- **No reemplazar lo que ya funciona.** Si ya hay un mecanismo establecido (carga de datos, dropdown, modal, pantalla, endpoint, componente), NO reescribirlo ni cambiarlo por uno propio.
- **Mejorar encima, no sustituir.** Preferir una capa ligera (un JS pequeño, un plugin que YA está incluido —ej. `select2`—) que potencie lo existente, en vez de un componente nuevo paralelo.
- **Reutilizar pantallas/modales/flujos existentes.** Ej.: un botón "Agregar" que redirige al modal que YA existe en otra pantalla, en lugar de clonar ese modal o crear un endpoint nuevo.
- **Tocar lo mínimo.** Evitar editar archivos compartidos, agregar dependencias o crear endpoints si el flujo ya se resuelve con lo que hay.
- **Ante la duda, preguntar.** Si no está claro qué mecanismo existente usar, preguntar antes de inventar uno.

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
Schemas SQL: `Refugio`, `Medico`, `Inventario`, `Seguridad`, `General`, `Veterinaria`, `Rescate`, `Ventas`.

---

## Cómo pensar este proyecto (principios de diseño — leer ANTES de modelar)

Pensar PRIMERO el dominio real → luego entidades → luego pantallas. Si hay ambigüedad de negocio, **preguntar la regla temprano**; no proponer modelos a medias ni "arreglar" conflatando conceptos.

1. **Separar dominios, no conflar.** Si dos conceptos se parecen (ej. *adoptante* vs *dueño*), sepáralos limpio — cada uno en su lane — en lugar de unirlos con reglas de prioridad. Tres dominios DISTINTOS aquí:
   - **Adopción** (`tbAdopciones`, *adoptante*): hecho histórico de que una persona adoptó un animal del refugio. **NO entra al flujo clínico.**
   - **Interno (refugio/rescate)**: animal de la organización, **sin dueño personal**; responsable = el departamento (refugio/rescate).
   - **Clínica (cliente externo)**: mascota con dueño = `Cliente` (`Veterinaria.tbClientes`), vía `tbMascotas.cli_Id`.
   En consulta un paciente es SOLO **interno** (`cli_Id` null) o **de cliente** (`cli_Id` set). *Adoptante nunca aparece en lo clínico.*
2. **Aditivo, no reemplazo.** Lo nuevo se suma sin romper/reemplazar lo existente (adoptante intacto; se agrega `cli_Id`).
3. **Opcional y degradable.** Roles/módulos nuevos deben poder apagarse y el sistema seguir funcionando. Usar el config de módulos existente (`tbModulosConfig.modu_EsActivo`; áreas `Comun/Refugio/Veterinaria/Ventas`) en vez de dependencias duras. Ej: recepción OFF → el vet se auto-sirve.
4. **Feed, no gate.** Un rol de apoyo (recepción) rellena/encola; nunca es un paso obligatorio para que otra cosa funcione.
5. **Un dato, múltiples lentes por rol.** Una sola fuente de verdad (ej. cola = citas de hoy "En espera") que cada rol ve distinto (recepción gestiona · vet atiende · admin supervisa), no pantallas separadas.
6. **Verificar contra los datos reales antes de recomendar.** Revisar esquema/SP/config vivo con `sqlcmd` antes de afirmar; no asumir.
7. **Analizar utilidad y solapamiento antes de construir una pantalla** (cuando aplique — pantalla nueva, ampliar una existente, o dudar si vale la pena). No preguntar "¿cómo la hago?" sino "¿debe existir y para quién?". Hacer este chequeo ANTES de escribir código y reportarlo visual (tablas/mockups):
   - **Utilidad por rol:** tabla rol × valor (★/★★/★★★) + por qué. Si a casi nadie le sirve, decirlo.
   - **Solapamiento:** ¿alguna pantalla ya cumple objetivo similar? Mapear quién-hace-qué en el grupo. Marcar lo que sería DUPLICADO.
   - **Escala/volumen:** ¿el enfoque aguanta el universo real de datos? (ej. matriz de manada fija ≠ universo abierto de clientes → por demanda, no volcado completo).
   - **Recomendación honesta:** incluso "no construir / reusar X / recortar alcance". Decir el valor ÚNICO que justifica la pantalla, o admitir que no lo hay.

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
| `docs/HANDOFF_EN_CONSULTA.md` | **Traspaso del módulo "En Consulta" + inventario de recetas**: estado actual, plan pendiente, contratos, modelo de datos, plan de agentes. Leer para continuar ese trabajo. |
