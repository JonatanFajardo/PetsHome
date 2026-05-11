#!/usr/bin/env python3
"""
scaffold.py
==============================================================
Genera el backend completo para pantallas de SOLO LECTURA
(vistas, perfiles, reportes, resúmenes — sin CRUD).

Diferencia con scaffold_backend.py (que genera CRUD completo):
  - Sin tabla propia ni entidad CRUD
  - Multiples secciones, cada una con su SP y Result class
  - Un ViewModel agregado que une todas las secciones
  - Controller con solo Index() que pasa el ViewModel a la vista
  - Sin AutoMapper (las result entities van directo al ViewModel)
  - SQL genera stubs de los SPs (los JOINs se completan manualmente)
  - Soporte para --param: parametros compartidos por todos los SPs
    (ej: masc_Id para perfiles de detalle por entidad)

Uso:
    python scaffold.py <EntityName> [opciones]

Ejemplo sin --param (pantalla global):
    python scaffold.py AlertaMedica \\
        --schema Medico \\
        --pantalla "Listado de alertas medicas" \\
        --section VacunasVencidas \\
            "masc_Id:int MascotaNombre:string Raza:string Edad:string VacunaNombre:string FechaUltimaVacuna:datetime DiasVencida:int" \\
        --section SinConsulta \\
            "masc_Id:int MascotaNombre:string Raza:string UltimaVisita:datetime? TiempoSinConsulta:string"

Ejemplo con --param (pantalla de detalle filtrada por ID):
    python scaffold.py PerfilMedico \\
        --schema Medico \\
        --pantalla "Perfil medico de mascota" \\
        --param masc_Id:int \\
        --section FichaMascota \\
            "masc_Id:int masc_Nombre:string Raza:string TotalCitas:int TratamientosActivos:int" \\
        --section UltimasCitas \\
            "cita_Id:int cita_FechaConsulta:datetime TipoConsulta:string Veterinario:string"

Tipos de campo soportados:
    string, int, int?, decimal, decimal?, datetime, datetime?, bit, bool, bool?
==============================================================
"""

import argparse
import re
import textwrap
from datetime import datetime
from pathlib import Path

BASE = Path(__file__).parent

# Connection string leida del appsettings.json (Integrated Security, sin credenciales)
_CONN_STR = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "Server=(localdb)\\MSSQLLocalDB;"
    "Database=PETSHOMEDB;"
    "Trusted_Connection=yes;"
)


def ejecutar_sql(sql_path: Path) -> bool:
    """
    Ejecuta un archivo SQL en SQL Server via pyodbc.
    Divide el script por GO (como sqlcmd) y ejecuta cada bloque.
    Retorna True si todo fue bien, False si hubo error.
    """
    try:
        import pyodbc
    except ImportError:
        print('  [WARN]  pyodbc no instalado — ejecuta el SQL manualmente.')
        print('          pip install pyodbc')
        return False

    try:
        conn = pyodbc.connect(_CONN_STR, autocommit=True)
        cursor = conn.cursor()
        script = sql_path.read_text(encoding='utf-8')

        # Dividir por GO (igual que sqlcmd)
        blocks = re.split(r'^\s*GO\s*$', script, flags=re.MULTILINE | re.IGNORECASE)
        for block in blocks:
            block = block.strip()
            if block:
                cursor.execute(block)

        cursor.close()
        conn.close()
        return True
    except Exception as e:
        print(f'  [ERROR] Al ejecutar {sql_path.name}: {e}')
        return False

# ============================================================
#  MAPEO DE TIPOS
# ============================================================

CS_TYPES = {
    'string':    'string',
    'int':       'int',
    'int?':      'int?',
    'decimal':   'decimal',
    'decimal?':  'decimal?',
    'datetime':  'DateTime',
    'datetime?': 'DateTime?',
    'bit':       'bool',
    'bit?':      'bool?',
    'bool':      'bool',
    'bool?':     'bool?',
}

SQL_TYPES = {
    'string':    'NVARCHAR(200)',
    'int':       'INT',
    'int?':      'INT',
    'decimal':   'DECIMAL(10,2)',
    'decimal?':  'DECIMAL(10,2)',
    'datetime':  'DATETIME',
    'datetime?': 'DATETIME',
    'bit':       'BIT',
    'bool':      'BIT',
    'bool?':     'BIT',
}

DBTYPE_MAP = {
    'string':    'DbType.String',
    'int':       'DbType.Int32',
    'int?':      'DbType.Int32',
    'decimal':   'DbType.Decimal',
    'decimal?':  'DbType.Decimal',
    'datetime':  'DbType.DateTime',
    'datetime?': 'DbType.DateTime',
    'bit':       'DbType.Boolean',
    'bool':      'DbType.Boolean',
    'bool?':     'DbType.Boolean',
}


def cs_type(t: str) -> str:
    return CS_TYPES.get(t.lower(), 'string')

def sql_type(t: str) -> str:
    return SQL_TYPES.get(t.lower(), 'NVARCHAR(200)')

def db_type(t: str) -> str:
    return DBTYPE_MAP.get(t.lower(), 'DbType.String')


# ============================================================
#  PARSEO DE CAMPOS, SECCIONES Y PARAMETROS
# ============================================================

def parse_fields(fields_str: str) -> list:
    """'campo1:tipo campo2:tipo?' → [{'name': ..., 'type': ...}]"""
    fields = []
    for part in fields_str.strip().split():
        if ':' in part:
            name, ftype = part.split(':', 1)
        else:
            name, ftype = part, 'string'
        fields.append({'name': name, 'type': ftype.lower()})
    return fields


def parse_sections(raw: list) -> list:
    """
    raw = [['NombreSeccion', 'campo1:tipo campo2:tipo'], ...]
    → [{'name': 'NombreSeccion', 'fields': [...]}, ...]
    """
    sections = []
    for pair in raw:
        name   = pair[0]
        fields = parse_fields(pair[1]) if len(pair) > 1 else []
        sections.append({'name': name, 'fields': fields})
    return sections


def parse_params(raw: list) -> list:
    """
    raw = ['masc_Id:int', 'otro:string'] (de --param, action='append')
    → [{'name': 'masc_Id', 'type': 'int', 'cs_type': 'int', 'db_type': 'DbType.Int32', 'sql_type': 'INT'}]
    """
    if not raw:
        return []
    params = []
    for token in raw:
        if ':' in token:
            name, ftype = token.split(':', 1)
        else:
            name, ftype = token, 'int'
        ftype = ftype.lower()
        params.append({
            'name':     name,
            'type':     ftype,
            'cs_type':  cs_type(ftype),
            'db_type':  db_type(ftype),
            'sql_type': sql_type(ftype),
        })
    return params


def _camel(name: str) -> str:
    """Convierte PascalCase o snake_case a camelCase para variables C#."""
    if '_' in name:
        parts = name.split('_')
        return parts[0].lower() + ''.join(p.title() for p in parts[1:])
    return name[0].lower() + name[1:] if name else name


# ============================================================
#  GENERADORES SQL
# ============================================================

def gen_sp(entity: str, schema: str, sections: list, params: list = None) -> str:
    """
    Genera un archivo SQL con un SP stub por seccion.
    Incluye declaracion de parametros si se proporcionaron --param.
    """
    today = datetime.now().strftime('%Y-%m-%d')
    params = params or []
    blocks = []

    for sec in sections:
        sp_name = f'PR_{schema}_{entity}_{sec["name"]}'
        col_list = ',\n        '.join(
            f'{f["name"]}  -- {sql_type(f["type"])}' for f in sec['fields']
        )

        # Parametros del SP
        if params:
            param_lines = ',\n    '.join(f'@{p["name"]} {p["sql_type"]}' for p in params)
            param_decl  = f'\n    {param_lines}\n'
            where_hint  = '\n                -- TODO: filtrar por parametro(s):\n                -- AND t.{} = @{}'.format(
                params[0]['name'], params[0]['name']
            )
        else:
            param_decl = ''
            where_hint = ''

        blocks.append(textwrap.dedent(f"""\
            -- ============================================
            -- {sp_name}
            -- ============================================
            CREATE OR ALTER PROCEDURE [{schema}].[{sp_name}]{param_decl}
            AS
            BEGIN
                SET NOCOUNT ON

                SELECT
                    {col_list}
                -- TODO: completar FROM, JOINs y WHERE{where_hint}
                FROM [?].[?]
                WHERE 1=1
                ORDER BY 1
            END
            GO
        """))

    body = '\n'.join(blocks)
    print_lines = '\n'.join(
        "PRINT '    PR_{schema}_{entity}_{name}'".format(
            schema=schema, entity=entity, name=s['name']
        )
        for s in sections
    )

    return textwrap.dedent(f"""\
        /*
        =============================================
        SCRIPT: Stored Procedures para {entity} (Dashboard)
        GENERADO POR: scaffold.py
        FECHA: {today}
        =============================================
        */

        USE PETSHOMEDB
        GO

        {body}
        PRINT '=========================================='
        PRINT 'SPs {entity} CREADOS'
        PRINT '=========================================='
        {print_lines}
        PRINT '=========================================='
        GO
    """)


# ============================================================
#  GENERADORES C#
# ============================================================

def gen_result_class(entity: str, schema: str, sec: dict) -> str:
    """Una clase de resultado por seccion."""
    props = '\n'.join(
        f'        public {cs_type(f["type"])} {f["name"]} {{ get; set; }}'
        for f in sec['fields']
    )
    class_name = f'PR_{schema}_{entity}_{sec["name"]}Result'
    return textwrap.dedent(f"""\
        using System;

        namespace PetsHome.Common.Entities
        {{
            public class {class_name}
            {{
        {props}
            }}
        }}
    """)


def gen_viewmodel(entity: str, schema: str, sections: list) -> str:
    """
    ViewModel agregado: una lista por seccion + propiedades Total* calculadas.
    Sin AutoMapper: las result entities se usan directamente.
    """
    using_list = 'using PetsHome.Common.Entities;\nusing System.Collections.Generic;'

    list_props = '\n\n'.join(
        f'        public List<PR_{schema}_{entity}_{s["name"]}Result> {s["name"]} {{ get; set; }}\n'
        f'            = new List<PR_{schema}_{entity}_{s["name"]}Result>();'
        for s in sections
    )

    total_props = '\n'.join(
        f'        public int Total{s["name"]} => {s["name"]}?.Count ?? 0;'
        for s in sections
    )

    sum_totals = ' + '.join(f'Total{s["name"]}' for s in sections)

    return textwrap.dedent(f"""\
        {using_list}

        namespace PetsHome.Business.Models
        {{
            public class {entity}ViewModel
            {{
                // ── Secciones ────────────────────────────────────
        {list_props}

                // ── Conteos calculados ────────────────────────────
        {total_props}
                public int TotalAlertas => {sum_totals};
            }}
        }}
    """)


def gen_repository(entity: str, schema: str, sections: list, params: list = None) -> str:
    """
    Un metodo async por seccion.
    Si hay --param, usa DynamicParameters; si no, llamada simple sin params.
    """
    params = params or []
    has_params = bool(params)

    if has_params:
        # Firma del metodo: (int mascId, ...)
        sig = ', '.join(f'{p["cs_type"]} {_camel(p["name"])}' for p in params)
        # Bloque DynamicParameters compartido
        dyn_lines = ['                var p = new DynamicParameters();'] + [
            f'                p.Add("@{p["name"]}", {_camel(p["name"])}, {p["db_type"]}, ParameterDirection.Input);'
            for p in params
        ]
        dyn_block = '\n'.join(dyn_lines)

        methods = '\n\n'.join(
            textwrap.dedent(f"""\
                    public async Task<IEnumerable<PR_{schema}_{entity}_{s["name"]}Result>> {s["name"]}Async({sig})
                    {{
                        const string sql = "[{schema}].[PR_{schema}_{entity}_{s["name"]}]";
            {dyn_block}
                        return await DbApp.SelectById<PR_{schema}_{entity}_{s["name"]}Result>(sql, p);
                    }}""")
            for s in sections
        )
        extra_usings = '\nusing Dapper;\nusing System.Data;'
    else:
        methods = '\n\n'.join(
            textwrap.dedent(f"""\
                    public async Task<IEnumerable<PR_{schema}_{entity}_{s["name"]}Result>> {s["name"]}Async()
                    {{
                        const string sql = "[{schema}].[PR_{schema}_{entity}_{s["name"]}]";
                        return await DbApp.Select<PR_{schema}_{entity}_{s["name"]}Result>(sql);
                    }}""")
            for s in sections
        )
        extra_usings = ''

    return textwrap.dedent(f"""\
        using PetsHome.Common.Entities;
        using PetsHome.DataAccess.Extensions;
        using System.Collections.Generic;
        using System.Threading.Tasks;{extra_usings}

        namespace PetsHome.Logic.Repositories
        {{
            public class {entity}Repository
            {{
        {methods}
            }}
        }}
    """)


def gen_service(entity: str, schema: str, sections: list, params: list = None) -> str:
    """Llama todos los metodos del repo y los une en el ViewModel agregado."""
    params = params or []
    has_params = bool(params)

    if has_params:
        sig          = ', '.join(f'{p["cs_type"]} {_camel(p["name"])}' for p in params)
        call_args    = ', '.join(_camel(p['name']) for p in params)
    else:
        sig       = ''
        call_args = ''

    calls = '\n'.join(
        f'                var {_camel(s["name"])} = await _repository.{s["name"]}Async({call_args});'
        for s in sections
    )
    assigns = '\n'.join(
        f'                    {s["name"]} = {_camel(s["name"])}?.ToList() ?? new List<PR_{schema}_{entity}_{s["name"]}Result>(),'
        for s in sections
    )

    return textwrap.dedent(f"""\
        using Microsoft.Extensions.Logging;
        using PetsHome.Business.Models;
        using PetsHome.Common.Entities;
        using PetsHome.Logic.Repositories;
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;

        namespace PetsHome.Business.Services
        {{
            public class {entity}Service
            {{
                private readonly {entity}Repository _repository;
                private readonly ILogger<{entity}Service> _logger;

                public {entity}Service({entity}Repository repository, ILogger<{entity}Service> logger)
                {{
                    _repository = repository;
                    _logger     = logger;
                }}

                public async Task<{entity}ViewModel> GetDashboardAsync({sig})
                {{
                    try
                    {{
        {calls}

                        return new {entity}ViewModel
                        {{
        {assigns}
                        }};
                    }}
                    catch (Exception error)
                    {{
                        _logger.LogError(error, error.Message);
                        return new {entity}ViewModel();
                    }}
                }}
            }}
        }}
    """)


def gen_fix_pantalla(entity: str, pantalla: str, grupo: str) -> str:
    """Genera el script SQL para registrar la pantalla en tbPantallas y asignarla a todos los roles."""
    return textwrap.dedent(f"""\
        -- ============================================================
        -- FIX: Registrar pantalla "{pantalla}"
        --      y asignarla a todos los roles activos
        -- Ejecutar UNA sola vez despues de crear los SPs
        -- GENERADO POR: scaffold.py
        -- ============================================================
        USE PETSHOMEDB
        GO

        -- 1. Insertar la pantalla si no existe
        IF NOT EXISTS (
            SELECT 1 FROM [Seguridad].[tbPantallas]
            WHERE pan_Descripcion = '{pantalla}'
        )
        BEGIN
            INSERT INTO [Seguridad].[tbPantallas] (pan_Descripcion, pan_Grupo, pan_EsActivo)
            VALUES ('{pantalla}', '{grupo}', 1)
            PRINT 'Pantalla creada: {pantalla}'
        END
        ELSE
            PRINT 'La pantalla ya existia, se omite el INSERT'
        GO

        -- 2. Asignarla a todos los roles que no la tengan aun
        INSERT INTO [Seguridad].[tbRolesPantallas] (rol_Id, pan_Id, ropan_EsActivo)
        SELECT r.Rol_Id, p.pan_Id, 1
        FROM [Seguridad].[tbRoles] r
        CROSS JOIN [Seguridad].[tbPantallas] p
        WHERE p.pan_Descripcion = '{pantalla}'
          AND NOT EXISTS (
              SELECT 1 FROM [Seguridad].[tbRolesPantallas] rp
              WHERE rp.rol_Id = r.Rol_Id AND rp.pan_Id = p.pan_Id
          )

        PRINT 'Pantalla asignada a los roles que no la tenian'
        GO

        -- 3. Verificar resultado
        SELECT r.Rol_Descripcion, p.pan_Descripcion, p.pan_Grupo, rp.ropan_EsActivo
        FROM [Seguridad].[tbRolesPantallas] rp
        JOIN [Seguridad].[tbRoles] r     ON rp.rol_Id = r.Rol_Id
        JOIN [Seguridad].[tbPantallas] p ON rp.pan_Id = p.pan_Id
        WHERE p.pan_Descripcion = '{pantalla}'
        GO
    """)


def gen_controller(entity: str, pantalla: str, params: list = None) -> str:
    """
    Controller de solo lectura (Index).
    Si hay --param, el action los recibe desde la URL y los pasa al service.
    """
    params   = params or []
    has_params = bool(params)

    if has_params:
        action_sig    = ', '.join(f'{p["cs_type"]} {_camel(p["name"])}' for p in params)
        service_args  = ', '.join(_camel(p['name']) for p in params)
        index_action  = f'Index({action_sig})'
        service_call  = f'await _service.GetDashboardAsync({service_args})'
    else:
        index_action = 'Index()'
        service_call = 'await _service.GetDashboardAsync()'

    return textwrap.dedent(f"""\
        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;
        using PetsHome.Business.Services;
        using PetsHome.UI.Filters;
        using System.Threading.Tasks;

        namespace PetsHome.UI.Controllers
        {{
            [Authorize]
            [PantallaAuthorize("{pantalla}")]
            public class {entity}Controller : BaseController
            {{
                private readonly {entity}Service _service;

                public {entity}Controller({entity}Service service)
                {{
                    _service = service;
                }}

                public async Task<IActionResult> {index_action}
                {{
                    var model = {service_call};
                    return View(model);
                }}
            }}
        }}
    """)


def gen_index_view(entity: str, pantalla: str, schema: str, sections: list, params: list = None) -> str:
    """Vista base con el @model correcto y placeholders por seccion."""
    params = params or []
    section_placeholders = '\n'.join(
        f'  {{/* SECCION: {s["name"]} — usar @foreach (var item in Model.{s["name"]}) */}}'
        for s in sections
    )
    section_titles = '\n'.join(
        f'    //   Model.{s["name"]}         (List<PR_{schema}_{entity}_{s["name"]}Result>)\n'
        f'    //   Model.Total{s["name"]}     (int)'
        for s in sections
    )
    param_note = ''
    if params:
        param_note = '\n    // Parametros recibidos en la URL:\n' + '\n'.join(
            f'    //   {p["name"]} ({p["cs_type"]}) — pasado por QueryString o ruta'
            for p in params
        )

    return textwrap.dedent(f"""\
        @model PetsHome.Business.Models.{entity}ViewModel
        @{{
            ViewData["Title"] = "{entity}";
            Layout = "~/Views/Shared/_Layout.cshtml";
            ViewData["CurrentPantalla"] = "{pantalla}";
        }}

        @*
            Propiedades del modelo disponibles:{param_note}
        {section_titles}
            Model.TotalAlertas   (int)
        *@

        @* TODO: reemplazar este contenido con html_to_razor.py si tienes un diseno HTML *@
        <div class="col-12">
            <h2>{entity}</h2>
            <p>Total: @Model.TotalAlertas</p>

        {section_placeholders}
        </div>
    """)


# ============================================================
#  PATCH DE ARCHIVOS COMPARTIDOS
# ============================================================

def patch_service_config(entity: str) -> None:
    sc_path = BASE / 'PetsHome.Business/ServiceConfiguration.cs'
    if not sc_path.exists():
        print(f'  [WARN]  ServiceConfiguration.cs no encontrado')
        return

    content = sc_path.read_text(encoding='utf-8')
    changed = False

    repo_line = f'            services.AddScoped<{entity}Repository>();'
    if repo_line not in content:
        anchor = '            //https://www.it-swarm.dev'
        if anchor in content:
            content = content.replace(anchor, f'{repo_line}\n{anchor}', 1)
            changed = True
            print(f'  [PATCH] ServiceConfiguration.cs  +AddScoped<{entity}Repository>()')
        else:
            print(f'  [WARN]  Anchor de Repository no encontrado en ServiceConfiguration.cs')
    else:
        print(f'  [SKIP]  {entity}Repository ya registrado')

    svc_line = f'            services.AddScoped<{entity}Service>();'
    if svc_line not in content:
        anchor = '            /// Auto Mapper Configurations'
        if anchor in content:
            content = content.replace(anchor, f'{svc_line}\n{anchor}', 1)
            changed = True
            print(f'  [PATCH] ServiceConfiguration.cs  +AddScoped<{entity}Service>()')
        else:
            print(f'  [WARN]  Anchor de Service no encontrado en ServiceConfiguration.cs')
    else:
        print(f'  [SKIP]  {entity}Service ya registrado')

    if changed:
        sc_path.write_text(content, encoding='utf-8')


# ============================================================
#  ESCRITURA DE ARCHIVOS
# ============================================================

def next_db_script_number(db_dir: Path) -> int:
    nums = []
    for f in db_dir.glob('*.sql'):
        m = re.match(r'^(\d+)', f.name)
        if m:
            nums.append(int(m.group(1)))
    return (max(nums) + 1) if nums else 20


def patch_sidebar(entity: str, pantalla: str, grupo: str) -> None:
    """
    Inserta el link al sidebar (_sidebar.cshtml):
      - Agrega tiene("{pantalla}") a la condicion del grupo
      - Agrega el <li> con el ActionLink
    Es idempotente: no duplica si ya existe.
    """
    sidebar_path = BASE / 'PetsHome.UI/Views/Shared/_sidebar.cshtml'
    if not sidebar_path.exists():
        print(f'  [WARN]  _sidebar.cshtml no encontrado')
        return

    content = sidebar_path.read_text(encoding='utf-8')

    # --- Condicion del grupo ---
    tiene_call = f'tiene("{pantalla}")'
    if tiene_call in content:
        print(f'  [SKIP]  Sidebar: condicion "{pantalla}" ya existe')
    else:
        # Busca el primer @if (tiene( del sidebar y agrega la condicion al principio
        first_tiene = f'@if (tiene('
        idx = content.find(first_tiene)
        if idx != -1:
            content = content[:idx + len('@if (')] + f'{tiene_call} || ' + content[idx + len('@if ('):]
            print(f'  [PATCH] _sidebar.cshtml  +condicion tiene("{pantalla}")')
        else:
            print(f'  [WARN]  Sidebar: no se encontro patron @if (tiene(')

    # --- Link <li> ---
    action_link = f'@Html.ActionLink("{entity}", "Index", "{entity}")'
    if action_link in content:
        print(f'  [SKIP]  Sidebar: link {entity} ya existe')
    else:
        # Insertar antes del primer @if (tiene( dentro del submenu del grupo
        collapse_id = f'id="{grupo}"'
        idx_collapse = content.find(collapse_id)
        if idx_collapse != -1:
            idx_first_item = content.find('@if (tiene(', idx_collapse)
            if idx_first_item != -1:
                new_li = (
                    f'                        @if (tiene("{pantalla}"))\n'
                    f'                        {{\n'
                    f'                            <li>@Html.ActionLink("{entity}", "Index", "{entity}")</li>\n'
                    f'                        }}\n'
                    f'                        '
                )
                content = content[:idx_first_item] + new_li + content[idx_first_item:]
                print(f'  [PATCH] _sidebar.cshtml  +link {entity}')
            else:
                print(f'  [WARN]  Sidebar: no se encontro primer item del grupo {grupo}')
        else:
            print(f'  [WARN]  Sidebar: no se encontro grupo "{grupo}" (id="{grupo}")')

    sidebar_path.write_text(content, encoding='utf-8')


def write_all(entity: str, schema: str, pantalla: str, grupo: str,
              sections: list, params: list = None) -> None:
    params = params or []
    sep = '=' * 60

    print(f'\n{sep}')
    print(f'  scaffold.py')
    print(sep)
    print(f'  Entidad : {entity}  |  Schema: {schema}  |  Grupo sidebar: {grupo}')
    print(f'  Pantalla: {pantalla}')
    if params:
        print(f'  Params  : {", ".join(p["name"] + ":" + p["type"] for p in params)}')
    print(f'  Secciones ({len(sections)}): {", ".join(s["name"] for s in sections)}')
    print(f'{"-"*60}\n')

    created = []
    skipped = []

    def write(path: Path, content: str, label: str):
        path.parent.mkdir(parents=True, exist_ok=True)
        if path.exists():
            kb = path.stat().st_size / 1024
            skipped.append(label)
            print(f'  [SKIP] {path.relative_to(BASE)}  (ya existe, {kb:.1f} KB)')
        else:
            path.write_text(content, encoding='utf-8')
            kb = path.stat().st_size / 1024
            created.append(label)
            print(f'  [OK]   {path.relative_to(BASE)}  ({kb:.1f} KB)')

    # 1a. SQL (stubs de SPs)
    n       = next_db_script_number(BASE / 'Database')
    sp_path = BASE / 'Database' / f'{n:02d}_SP_{schema.upper()}_{entity.upper()}_DASHBOARD.sql'
    write(sp_path, gen_sp(entity, schema, sections, params), 'SQL')

    # 1b. FIX SQL (INSERT pantalla + asignar roles)
    fix_path = BASE / 'Database' / f'FIX_{entity.upper()}_pantalla.sql'
    write(fix_path, gen_fix_pantalla(entity, pantalla, grupo), 'SQL/FIX-Pantalla')

    # 2. Result classes (una por seccion)
    common_dir = BASE / 'PetsHome.Common/Entities' / schema
    for sec in sections:
        write(
            common_dir / f'PR_{schema}_{entity}_{sec["name"]}Result.cs',
            gen_result_class(entity, schema, sec),
            f'Result/{sec["name"]}'
        )

    # 3. ViewModel agregado
    write(
        BASE / 'PetsHome.Business/Models' / f'{entity}ViewModel.cs',
        gen_viewmodel(entity, schema, sections),
        'ViewModel'
    )

    # 4. Repository
    write(
        BASE / 'PetsHome.Logic/Repositories' / f'{entity}Repository.cs',
        gen_repository(entity, schema, sections, params),
        'Repository'
    )

    # 5. Service
    write(
        BASE / 'PetsHome.Business/Services' / f'{entity}Service.cs',
        gen_service(entity, schema, sections, params),
        'Service'
    )

    # 6. Controller
    write(
        BASE / 'PetsHome.UI/Controllers' / f'{entity}Controller.cs',
        gen_controller(entity, pantalla, params),
        'Controller'
    )

    # 7. Vista base
    write(
        BASE / 'PetsHome.UI/Views' / entity / 'Index.cshtml',
        gen_index_view(entity, pantalla, schema, sections, params),
        'View/Index'
    )

    # 8. Patch ServiceConfiguration.cs
    print(f'\n{"-"*60}')
    print('  Patching ServiceConfiguration.cs...')
    patch_service_config(entity)

    # 9. Patch _sidebar.cshtml
    print(f'\n{"-"*60}')
    print('  Patching _sidebar.cshtml...')
    patch_sidebar(entity, pantalla, grupo)

    # 10. Ejecutar FIX SQL automaticamente (pantalla + roles)
    print(f'\n{"-"*60}')
    print(f'  Ejecutando {fix_path.name} en SQL Server...')
    ok = ejecutar_sql(fix_path)
    if ok:
        print(f'  [OK]   Pantalla "{pantalla}" registrada en tbPantallas y asignada a roles')
    else:
        print(f'  [WARN] Ejecuta manualmente: {fix_path.name}')

    # Resumen
    print(f'\n  Creados  : {len(created)}  ({", ".join(created) if created else "ninguno"})')
    print(f'  Omitidos : {len(skipped)}  ({", ".join(skipped) if skipped else "ninguno"})')
    print(f'\n  Pasos manuales restantes:')
    print(f'    1. Completar los SELECT en {sp_path.name} y ejecutarlo en SQL Server')
    print(f'    2. Si tienes diseno HTML: python html_to_razor.py diseno.html --controller {entity} --action Index')
    print(f'    3. Cerrar sesion y volver a entrar para refrescar los claims del sidebar')
    print(f'\n{sep}\n')


# ============================================================
#  CLI
# ============================================================

def main():
    parser = argparse.ArgumentParser(
        description='Genera el backend de un dashboard (read-only) para PetsHome.',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=textwrap.dedent("""\
            Ejemplo sin params (lista agregada):
              python scaffold_dashboard.py AlertaMedica \\
                  --schema Medico \\
                  --pantalla "Listado de alertas medicas" \\
                  --section VacunasVencidas "masc_Id:int MascotaNombre:string DiasVencida:int" \\
                  --section TratamientosPorVencer "trat_Id:int MascotaNombre:string DiasRestantes:int"

            Ejemplo con --param (dashboard de detalle por entidad):
              python scaffold_dashboard.py PerfilMedico \\
                  --schema Medico \\
                  --pantalla "Perfil medico de mascota" \\
                  --param masc_Id:int \\
                  --section FichaMascota "masc_Id:int masc_Nombre:string Raza:string TotalCitas:int" \\
                  --section UltimasCitas "cita_Id:int cita_FechaConsulta:datetime TipoConsulta:string Diagnostico:string"
        """)
    )

    parser.add_argument('entity',
                        help='Nombre en PascalCase (ej: AlertaMedica, PerfilMedico)')
    parser.add_argument('--schema', '-s', required=True,
                        help='Schema SQL (Medico, Refugio, Inventario, Seguridad)')
    parser.add_argument('--pantalla', '-p', default='',
                        help='Nombre de pantalla en el sistema de seguridad')
    parser.add_argument('--grupo', '-g', default='Medicamento',
                        help='Grupo del sidebar. Default: Medicamento')
    parser.add_argument('--param',
                        action='append', metavar='CAMPO:TIPO',
                        help='Parametro de entrada compartido por todos los SPs (ej: masc_Id:int). '
                             'Puede repetirse: --param masc_Id:int --param otro:string')
    parser.add_argument('--section', '-sec',
                        action='append', nargs=2,
                        metavar=('NOMBRE', 'CAMPOS'),
                        required=True,
                        help='Seccion: --section NombrePascal "campo1:tipo campo2:tipo?"'
                             '  (puede repetirse N veces)')

    args = parser.parse_args()
    sections = parse_sections(args.section)
    params   = parse_params(args.param or [])

    write_all(
        entity=args.entity,
        schema=args.schema,
        pantalla=args.pantalla,
        grupo=args.grupo,
        sections=sections,
        params=params,
    )


if __name__ == '__main__':
    main()
