#!/usr/bin/env python3
"""
map_project.py
==============================================================
Escanea el proyecto PetsHome y genera PROJECT_CONTEXT.md,
un archivo compacto que resume toda la arquitectura, patrones
y convenciones del proyecto.

Objetivo: en la proxima conversacion con Claude, leer UN archivo
en lugar de lanzar un agente Explore que lee docenas de archivos.

Uso:
    python map_project.py
    python map_project.py --output docs/PROJECT_CONTEXT.md
==============================================================
"""

import argparse
import ast
import os
import re
from datetime import datetime
from pathlib import Path

BASE = Path(__file__).parent

# ============================================================
#  HELPERS
# ============================================================

def read_file(path: Path) -> str:
    try:
        return path.read_text(encoding='utf-8', errors='ignore')
    except Exception:
        return ''


def find_files(pattern: str, base: Path = BASE) -> list[Path]:
    return sorted(base.rglob(pattern))


def slugify_path(path: Path) -> str:
    return str(path.relative_to(BASE)).replace('\\', '/')


# ============================================================
#  EXTRACTORS
# ============================================================

def extract_cs_classes(content: str) -> list[dict]:
    """Extrae clases publicas y sus metodos publicos de un archivo .cs"""
    classes = []
    # Buscar clases
    class_pattern = re.compile(
        r'(?:public|internal)\s+(?:partial\s+)?(?:class|interface)\s+(\w+)'
        r'(?:\s*:\s*([^\n{]+))?'
    )
    method_pattern = re.compile(
        r'(?:public|protected)\s+'
        r'(?:async\s+)?'
        r'(?:Task<[^>]+>|Task|IActionResult|JsonResult|IEnumerable<[^>]+>|List<[^>]+>|[\w<>\[\]?]+)\s+'
        r'(\w+)\s*\('
    )

    for cm in class_pattern.finditer(content):
        name = cm.group(1)
        base_class = (cm.group(2) or '').strip()
        # Buscar metodos despues de la clase
        snippet = content[cm.start():]
        methods = [m.group(1) for m in method_pattern.finditer(snippet[:3000])]
        # Filtrar metodos de infraestructura
        methods = [m for m in methods if m not in ('ToString', 'GetHashCode', 'Equals', 'Dispose')]
        classes.append({'name': name, 'base': base_class, 'methods': methods[:20]})

    return classes


def extract_controller_actions(content: str) -> list[str]:
    """Extrae solo los nombres de acciones de un controller."""
    pattern = re.compile(
        r'(?:public|protected)\s+(?:async\s+)?'
        r'(?:Task<IActionResult>|IActionResult|Task<JsonResult>|JsonResult|async\s+Task)\s+'
        r'(\w+)\s*\('
    )
    return list(dict.fromkeys(m.group(1) for m in pattern.finditer(content)))


def extract_pantalla_authorize(content: str) -> list[str]:
    """Extrae los nombres de pantalla usados en PantallaAuthorize."""
    pattern = re.compile(r'PantallaAuthorize\("([^"]+)"')
    return list(dict.fromkeys(m.group(1) for m in pattern.finditer(content)))


def extract_db_sets(content: str) -> list[str]:
    """Extrae DbSet<T> del DbContext."""
    return re.findall(r'DbSet<(\w+)>', content)


def extract_sp_names(content: str) -> list[str]:
    """Extrae nombres de stored procedures referenciados en el codigo C#."""
    return list(dict.fromkeys(re.findall(r'\[[\w]+\]\.\[(PR_\w+)\]', content)))


def extract_sql_tables(sql: str) -> list[str]:
    """Extrae nombres de tablas CREATE TABLE de scripts SQL."""
    return re.findall(r'CREATE TABLE\s+\[?(\w+)\]?\.\[?(\w+)\]?', sql, re.IGNORECASE)


# ============================================================
#  SECCIONES DEL MAPA
# ============================================================

def seccion_arquitectura() -> str:
    lines = [
        '## Arquitectura General',
        '',
        '- **Framework**: ASP.NET Core MVC (.NET Core)',
        '- **ORM**: Dapper (sin EF Core) + Stored Procedures',
        '- **BD**: SQL Server — base de datos `PETSHOMEDB`',
        '- **Auth**: Cookie-based (8h), [Authorize] + [PantallaAuthorize]',
        '- **Capas**:',
        '  - `PetsHome.UI`         → Controllers, Views, Filters, Middleware',
        '  - `PetsHome.Business`   → Services, Models (ViewModels), Extensions (AutoMapper)',
        '  - `PetsHome.Logic`      → Repositories, Interfaces',
        '  - `PetsHome.DataAccess` → PetsHomeDbContext (connection string), DbApp (helper)',
        '  - `PetsHome.Common`     → Entities (SP result classes), InternalEntities',
        '',
        '### Patron de acceso a datos (DbApp)',
        '```csharp',
        '// Lista sin params:      DbApp.Select<TResult>(sp)',
        '// Lista con params:      DbApp.SelectById<TResult>(sp, dynamicParams)',
        '// Primera fila:          DbApp.Find<TResult>(sp, params)',
        '// Detalle:               DbApp.Detail<TResult>(sp, params)',
        '// Escritura (retorna RequestResult): DbApp.ExecuteWithResult(sp, params)',
        '// Dropdown sync:         DbApp.Dropdown<TResult>(sp)',
        '```',
        '',
        '### Autorizacion',
        '```csharp',
        '// A nivel de clase  → verifica que el usuario tenga la pantalla (consultar)',
        '[PantallaAuthorize("Nombre de pantalla")]',
        '',
        '// A nivel de metodo → verifica operacion especifica',
        '[PantallaAuthorize("Nombre de pantalla", "insertar|editar|eliminar")]',
        '```',
        '',
        '### Schemas SQL',
        '- `[Refugio]`    → Mascotas, Adopciones, Voluntarios, Eventos',
        '- `[Medico]`     → CitaMedica, Recetas, Tratamientos, Catalogs medicos',
        '- `[Inventario]` → Items, Recepciones, Existencias',
        '- `[Seguridad]`  → Usuarios, Roles, Pantallas, RolesPantallas',
        '- `[General]`    → Departamentos, Municipios',
        '',
    ]
    return '\n'.join(lines)


def seccion_layout() -> str:
    layout_path = BASE / 'PetsHome.UI/Views/Shared/_Layout.cshtml'
    content = read_file(layout_path)

    # Detectar lo que ya carga el layout
    cdn_links = re.findall(r'href="(https?://[^"]+)"', content)
    local_css  = re.findall(r'href="(~/[^"]+\.css)"', content)
    local_js   = re.findall(r'src="(~/[^"]+\.js)"', content)

    lines = [
        '## Layout Compartido',
        '',
        f'**Archivo**: `{slugify_path(layout_path)}`',
        '',
        '### CSS ya cargado en el layout (NO volver a incluir)',
        '```',
    ]
    for c in local_css[:10]:
        lines.append(f'  {c}')
    lines.append('```')
    lines.append('')
    lines.append('### CDNs ya en el layout')
    lines.append('```')
    for c in cdn_links[:8]:
        lines.append(f'  {c}')
    lines.append('```')
    lines.append('')
    lines.append('### Estructura del body')
    lines.append('```html')
    lines.append('<!-- El @RenderBody() va dentro de: -->')
    lines.append('<div id="content" class="main-content">')
    lines.append('  <div class="layout-px-spacing">')
    lines.append('    <div class="row layout-top-spacing">')
    lines.append('      @RenderBody()   <!-- col-12 como wrapper tipico -->')
    lines.append('    </div>')
    lines.append('  </div>')
    lines.append('</div>')
    lines.append('```')
    lines.append('')
    lines.append('### Secciones disponibles')
    lines.append('```razor')
    lines.append('@section Styles  { ... }   <!-- En el <head>, antes del cierre -->')
    lines.append('@section Scripts { ... }   <!-- Antes del </body> -->')
    lines.append('```')
    lines.append('')

    return '\n'.join(lines)


def seccion_controllers() -> str:
    lines = ['## Controllers', '']
    ctrl_dir = BASE / 'PetsHome.UI/Controllers'

    for f in sorted(ctrl_dir.rglob('*.cs')):
        if f.name.endswith('.bak'):
            continue
        content = read_file(f)
        actions = extract_controller_actions(content)
        pantallas = extract_pantalla_authorize(content)
        rel = slugify_path(f)

        if not actions:
            continue

        name = f.stem
        lines.append(f'### `{name}`')
        lines.append(f'**Archivo**: `{rel}`')
        if pantallas:
            lines.append(f'**Pantalla**: `{pantallas[0]}`')
        lines.append(f'**Acciones**: {", ".join(actions)}')
        lines.append('')

    return '\n'.join(lines)


def seccion_services() -> str:
    lines = ['## Services', '']
    svc_dir = BASE / 'PetsHome.Business/Services'

    for f in sorted(svc_dir.rglob('*.cs')):
        content = read_file(f)
        classes = extract_cs_classes(content)
        for cls in classes:
            if not cls['methods']:
                continue
            lines.append(f'### `{cls["name"]}`')
            lines.append(f'**Archivo**: `{slugify_path(f)}`')
            lines.append(f'**Metodos**: {", ".join(cls["methods"])}')
            lines.append('')

    return '\n'.join(lines)


def seccion_repositories() -> str:
    lines = ['## Repositories', '']
    repo_dir = BASE / 'PetsHome.Logic/Repositories'

    for f in sorted(repo_dir.rglob('*.cs')):
        content = read_file(f)
        sps = extract_sp_names(content)
        classes = extract_cs_classes(content)
        for cls in classes:
            if not cls['methods']:
                continue
            lines.append(f'### `{cls["name"]}`')
            lines.append(f'**Archivo**: `{slugify_path(f)}`')
            lines.append(f'**Metodos**: {", ".join(cls["methods"])}')
            if sps:
                lines.append(f'**SPs usados**: {", ".join(sps[:10])}')
            lines.append('')

    return '\n'.join(lines)


def seccion_viewmodels() -> str:
    lines = ['## ViewModels (Business/Models)', '']
    vm_dir = BASE / 'PetsHome.Business/Models'

    vm_names = []
    for f in sorted(vm_dir.rglob('*.cs')):
        content = read_file(f)
        names = re.findall(r'public\s+(?:partial\s+)?class\s+(\w+)', content)
        vm_names.extend(names)

    # Agrupar por prefijo
    groups: dict[str, list[str]] = {}
    for name in vm_names:
        prefix = re.match(r'^([A-Z][a-z]+(?:[A-Z][a-z]+)?)', name)
        key = prefix.group(1) if prefix else 'Otros'
        groups.setdefault(key, []).append(name)

    for key, names in sorted(groups.items()):
        lines.append(f'- **{key}**: {", ".join(names)}')

    lines.append('')
    return '\n'.join(lines)


def seccion_entities() -> str:
    lines = ['## Result Entities (Common/Entities)', '']
    ent_dir = BASE / 'PetsHome.Common/Entities'

    for schema_dir in sorted(ent_dir.iterdir()):
        if not schema_dir.is_dir():
            continue
        files = list(schema_dir.rglob('*.cs'))
        if not files:
            continue
        names = []
        for f in sorted(files):
            content = read_file(f)
            found = re.findall(r'public\s+(?:partial\s+)?class\s+(\w+)', content)
            names.extend(found)
        if names:
            lines.append(f'**{schema_dir.name}**: {", ".join(names)}')

    lines.append('')
    return '\n'.join(lines)


def seccion_database() -> str:
    lines = ['## Base de Datos — Scripts SQL', '']
    db_dir = BASE / 'Database'

    sql_files = sorted(db_dir.glob('*.sql'))
    for f in sql_files:
        tables = extract_sql_tables(read_file(f))
        desc = f.name
        if tables:
            t_list = ', '.join(f'[{s}].[{n}]' for s, n in tables[:3])
            desc += f'  → tablas: {t_list}'
        lines.append(f'- `{desc}`')

    lines.append('')
    lines.append('### Pantallas registradas en tbPantallas')
    pantallas_script = read_file(BASE / 'Database/17_SEGURIDAD_PANTALLAS_ROLES.sql')
    pantallas = re.findall(r"\('([^']+)',\s*'([^']+)'\)", pantallas_script)
    for nombre, grupo in pantallas:
        lines.append(f'- `{nombre}` ({grupo})')

    lines.append('')
    return '\n'.join(lines)


def seccion_convenciones() -> str:
    lines = [
        '## Convenciones de Nomenclatura',
        '',
        '### Archivos C#',
        '| Tipo | Patron | Ejemplo |',
        '|------|--------|---------|',
        '| Controller    | `{Entity}Controller.cs`                | `CitaMedicaController.cs` |',
        '| Service       | `{Entity}Service.cs`                   | `CitaMedicaService.cs` |',
        '| Repository    | `{Entity}Repository.cs`                | `CitaMedicaRepository.cs` |',
        '| SP Result     | `PR_{Schema}_{Entity}_{Op}Result.cs`   | `PR_Medico_CitaMedica_ListResult.cs` |',
        '| ViewModel     | `{Entity}{Op}ViewModel.cs`             | `CitaMedicaFormViewModel.cs` |',
        '',
        '### Stored Procedures',
        '| Operacion | Patron |',
        '|-----------|--------|',
        '| List      | `[Schema].[PR_{Schema}_{Entity}_List]` |',
        '| Find      | `[Schema].[PR_{Schema}_{Entity}_Find]` |',
        '| Detail    | `[Schema].[PR_{Schema}_{Entity}_Detail]` |',
        '| Insert    | `[Schema].[PR_{Schema}_{Entity}_Insert]` |',
        '| Update    | `[Schema].[PR_{Schema}_{Entity}_Update]` |',
        '| Delete    | `[Schema].[PR_{Schema}_{Entity}_Delete]` |',
        '| Dropdown  | `[Schema].[PR_{Schema}_{Entity}_Dropdown]` |',
        '',
        '### Tablas',
        '`[Schema].[tb{EntityPascalCase}]`  — ej: `[Medico].[tbCitaMedica]`',
        '',
        '### Columnas',
        '`{prefijo}_{NombreCampo}` — ej: `cita_FechaConsulta`, `masc_Nombre`',
        '- Soft delete: `{prefijo}_EsEliminado BIT DEFAULT 0`',
        '- Auditoria: `{prefijo}_UsuarioCrea`, `{prefijo}_FechaCrea`, `{prefijo}_UsuarioModifica`, `{prefijo}_FechaModifica`',
        '',
        '### ViewData tipico en vistas',
        '```razor',
        '@{',
        '    ViewData["Title"]          = "Titulo de pagina";',
        '    Layout                     = "~/Views/Shared/_Layout.cshtml";',
        '    ViewData["CurrentPantalla"] = "Nombre de pantalla";  // para PantallaAuthorize JS',
        '}',
        '```',
        '',
    ]
    return '\n'.join(lines)


# ============================================================
#  MAIN
# ============================================================

def build_map(output: Path) -> None:
    print(f'Escaneando proyecto en: {BASE}')

    secciones = [
        f'# PetsHome — Project Context',
        f'> Generado por `map_project.py` el {datetime.now().strftime("%Y-%m-%d %H:%M")}',
        f'> Leer este archivo al inicio de cada conversacion para evitar explorar el proyecto desde cero.',
        '',
        seccion_arquitectura(),
        seccion_convenciones(),
        seccion_layout(),
        seccion_controllers(),
        seccion_services(),
        seccion_repositories(),
        seccion_viewmodels(),
        seccion_entities(),
        seccion_database(),
    ]

    content = '\n'.join(secciones)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(content, encoding='utf-8')

    size_kb = output.stat().st_size / 1024
    lines   = content.count('\n')
    print(f'Generado: {output}')
    print(f'  Tamano: {size_kb:.1f} KB  |  Lineas: {lines}')
    print('')
    print('Sugerencia: al iniciar una conversacion con Claude, adjunta o menciona:')
    print(f'  "Lee {output.name} para entender el proyecto antes de hacer cambios"')


def main():
    parser = argparse.ArgumentParser(description='Genera PROJECT_CONTEXT.md del proyecto PetsHome.')
    parser.add_argument('--output', '-o', default='PROJECT_CONTEXT.md',
                        help='Ruta del archivo de salida (default: PROJECT_CONTEXT.md)')
    args = parser.parse_args()

    output = BASE / args.output
    build_map(output)


if __name__ == '__main__':
    main()
