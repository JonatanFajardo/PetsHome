#!/usr/bin/env python3
"""
scaffold_backend.py
==============================================================
Genera el backend completo para una nueva pantalla/entidad
en PetsHome siguiendo exactamente las convenciones del proyecto:

  SP (SQL)  →  Result class  →  ViewModels  →
  Repository  →  Service  →  Controller

Uso:
    python scaffold_backend.py <EntityName> [opciones]

Ejemplo completo:
    python scaffold_backend.py Mascota \\
        --schema Refugio \\
        --prefix masc \\
        --pantalla "Listado de mascotas" \\
        --fields "masc_Nombre:string masc_FechaNac:datetime? masc_Peso:decimal?"

Tipos de campo soportados:
    string, int, int?, decimal, decimal?, datetime, datetime?, bit
==============================================================
"""

import argparse
import os
import re
import textwrap
from datetime import datetime
from pathlib import Path

BASE = Path(__file__).parent

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
    'bit?':      'BIT',
    'bool':      'BIT',
    'bool?':     'BIT',
}

DB_TYPES = {
    'string':    'DbType.String',
    'int':       'DbType.Int32',
    'int?':      'DbType.Int32',
    'decimal':   'DbType.Decimal',
    'decimal?':  'DbType.Decimal',
    'datetime':  'DbType.DateTime',
    'datetime?': 'DbType.DateTime',
    'bit':       'DbType.Boolean',
    'bool':      'DbType.Boolean',
}


def cs_type(t: str) -> str:
    return CS_TYPES.get(t.lower(), 'string')

def sql_type(t: str) -> str:
    return SQL_TYPES.get(t.lower(), 'NVARCHAR(200)')

def db_type(t: str) -> str:
    return DB_TYPES.get(t.lower(), 'DbType.String')

def is_nullable(t: str) -> bool:
    return t.endswith('?') or t.lower() in ('string',)


# ============================================================
#  PARSEO DE CAMPOS
# ============================================================

def parse_fields(fields_str: str) -> list[dict]:
    """
    Parsea "campo1:tipo campo2:tipo?" → list of dicts
    El primer campo se trata como PK (int ID).
    """
    fields = []
    for part in fields_str.strip().split():
        if ':' in part:
            name, ftype = part.split(':', 1)
        else:
            name, ftype = part, 'string'
        fields.append({'name': name, 'type': ftype.lower()})
    return fields


# ============================================================
#  GENERADORES
# ============================================================

def gen_sp(entity: str, schema: str, prefix: str, fields: list[dict]) -> str:
    """Genera el archivo SQL con todos los SPs de la entidad."""
    table   = f'[{schema}].[tb{entity}]'
    pk_col  = f'{prefix}_Id'
    pk_param = f'@{prefix}_Id'
    all_cols = [f['name'] for f in fields]
    write_cols = [f['name'] for f in fields]  # todos excepto el ID son escritos

    # Parámetros para INSERT/UPDATE
    def param_line(f):
        null = ' = NULL' if is_nullable(f['type']) else ''
        return f"    @{f['name']} {sql_type(f['type'])}{null}"

    insert_cols = ',\n        '.join(all_cols + [f'{prefix}_UsuarioCrea', f'{prefix}_FechaCrea'])
    insert_vals = ',\n        '.join(
        [f'@{c}' for c in all_cols] + [f'@{prefix}_UsuarioCrea', 'GETDATE()']
    )

    set_lines = ',\n        '.join(
        [f'{c} = @{c}' for c in all_cols] +
        [f'{prefix}_UsuarioModifica = @{prefix}_UsuarioModifica',
         f'{prefix}_FechaModifica = GETDATE()']
    )

    params_insert = ',\n'.join(param_line(f) for f in fields)
    params_update = params_insert + f',\n    @{prefix}_UsuarioModifica INT'

    return textwrap.dedent(f"""\
        /*
        =============================================
        SCRIPT: Stored Procedures para tb{entity}
        GENERADO POR: scaffold_backend.py
        FECHA: {datetime.now().strftime("%Y-%m-%d")}
        =============================================
        */

        USE PETSHOMEDB
        GO

        -- ============================================
        -- PR_{schema}_{entity}_List
        -- ============================================
        CREATE OR ALTER PROCEDURE [{schema}].[PR_{schema}_{entity}_List]
        AS
        BEGIN
            SET NOCOUNT ON
            SELECT
                ROW_NUMBER() OVER(ORDER BY e.{pk_col} DESC) AS Fila,
                {', '.join(f'e.{f["name"]}' for f in [{'name': pk_col}] + fields)}
            FROM {table} AS e
            WHERE e.{prefix}_EsEliminado = 0
            ORDER BY e.{pk_col} DESC
        END
        GO

        -- ============================================
        -- PR_{schema}_{entity}_Find
        -- ============================================
        CREATE OR ALTER PROCEDURE [{schema}].[PR_{schema}_{entity}_Find]
            {pk_param} INT
        AS
        BEGIN
            SET NOCOUNT ON
            SELECT
                {', '.join(f'e.{f["name"]}' for f in [{'name': pk_col}] + fields)}
            FROM {table} AS e
            WHERE e.{pk_col} = {pk_param}
            AND e.{prefix}_EsEliminado = 0
        END
        GO

        -- ============================================
        -- PR_{schema}_{entity}_Detail
        -- ============================================
        CREATE OR ALTER PROCEDURE [{schema}].[PR_{schema}_{entity}_Detail]
            {pk_param} INT
        AS
        BEGIN
            SET NOCOUNT ON
            SELECT
                {', '.join(f'e.{f["name"]}' for f in [{'name': pk_col}] + fields)},
                usuCrea.usu_Nombre AS UsuarioCreacion,
                e.{prefix}_FechaCrea,
                usuMod.usu_Nombre  AS UsuarioModificacion,
                e.{prefix}_FechaModifica
            FROM {table} AS e
            LEFT JOIN [Seguridad].[tbUsuarios] usuCrea ON e.{prefix}_UsuarioCrea    = usuCrea.usu_Id
            LEFT JOIN [Seguridad].[tbUsuarios] usuMod  ON e.{prefix}_UsuarioModifica = usuMod.usu_Id
            WHERE e.{pk_col} = {pk_param}
            AND e.{prefix}_EsEliminado = 0
        END
        GO

        -- ============================================
        -- PR_{schema}_{entity}_Insert
        -- ============================================
        CREATE OR ALTER PROCEDURE [{schema}].[PR_{schema}_{entity}_Insert]
        {params_insert},
            @{prefix}_UsuarioCrea INT
        AS
        BEGIN
            SET NOCOUNT ON
            INSERT INTO {table} (
                {insert_cols}
            ) VALUES (
                {insert_vals}
            )
            SELECT 1 AS CodeStatus, 'Registro insertado' AS MessageStatus
        END
        GO

        -- ============================================
        -- PR_{schema}_{entity}_Update
        -- ============================================
        CREATE OR ALTER PROCEDURE [{schema}].[PR_{schema}_{entity}_Update]
            {pk_param} INT,
        {params_update}
        AS
        BEGIN
            SET NOCOUNT ON
            UPDATE {table}
            SET
                {set_lines}
            WHERE {pk_col} = {pk_param}
            SELECT 1 AS CodeStatus, 'Registro actualizado' AS MessageStatus
        END
        GO

        -- ============================================
        -- PR_{schema}_{entity}_Delete  (soft delete)
        -- ============================================
        CREATE OR ALTER PROCEDURE [{schema}].[PR_{schema}_{entity}_Delete]
            {pk_param} INT,
            @{prefix}_UsuarioModifica INT
        AS
        BEGIN
            SET NOCOUNT ON
            UPDATE {table}
            SET {prefix}_EsEliminado = 1,
                {prefix}_UsuarioModifica = @{prefix}_UsuarioModifica,
                {prefix}_FechaModifica   = GETDATE()
            WHERE {pk_col} = {pk_param}
            SELECT 1 AS CodeStatus, 'Registro eliminado' AS MessageStatus
        END
        GO

        -- ============================================
        -- PR_{schema}_{entity}_Dropdown
        -- ============================================
        CREATE OR ALTER PROCEDURE [{schema}].[PR_{schema}_{entity}_Dropdown]
        AS
        BEGIN
            SET NOCOUNT ON
            SELECT
                e.{pk_col},
                e.{fields[0]['name'] if fields else pk_col} AS Descripcion
            FROM {table} AS e
            WHERE e.{prefix}_EsEliminado = 0
            ORDER BY 2
        END
        GO

        PRINT 'SPs de {entity} creados correctamente.'
        GO
    """)


def gen_result_class(entity: str, schema: str, prefix: str, fields: list[dict], op: str) -> str:
    """Genera una clase de resultado para un SP."""
    props = []
    if op == 'List':
        props.append('    public int Fila { get; set; }')

    pk_name = f'{prefix}_Id'
    props.append(f'    public int {pk_name} {{ get; set; }}')

    for f in fields:
        cst = cs_type(f['type'])
        props.append(f'    public {cst} {f["name"]} {{ get; set; }}')

    if op == 'Detail':
        props += [
            '    public string UsuarioCreacion { get; set; }',
            f'    public DateTime {prefix}_FechaCrea {{ get; set; }}',
            '    public string UsuarioModificacion { get; set; }',
            f'    public DateTime? {prefix}_FechaModifica {{ get; set; }}',
        ]

    body = '\n'.join(props)
    return textwrap.dedent(f"""\
        using System;

        namespace PetsHome.Common.Entities
        {{
            public partial class PR_{schema}_{entity}_{op}Result
            {{
        {body}
            }}
        }}
    """)


def gen_dropdown_result(entity: str, schema: str, prefix: str) -> str:
    return textwrap.dedent(f"""\
        namespace PetsHome.Common.Entities
        {{
            public partial class PR_{schema}_{entity}_DropdownResult
            {{
                public int {prefix}_Id {{ get; set; }}
                public string Descripcion {{ get; set; }}
            }}
        }}
    """)


def gen_viewmodels(entity: str, schema: str, prefix: str, fields: list[dict], pantalla: str) -> str:
    """Genera un archivo con los 4 ViewModels principales (List, Form, Find, Detail)."""
    pk_name = f'{prefix}_Id'

    def props(include_select=False):
        lines = [f'        public int {pk_name} {{ get; set; }}']
        for f in fields:
            cst = cs_type(f['type'])
            lines.append(f'        public {cst} {f["name"]} {{ get; set; }}')
        return '\n'.join(lines)

    return textwrap.dedent(f"""\
        using Microsoft.AspNetCore.Mvc.Rendering;
        using System;
        using System.Collections.Generic;
        using System.ComponentModel.DataAnnotations;

        namespace PetsHome.Business.Models
        {{
            // ─── Lista (DataTable) ─────────────────────────
            public partial class {entity}ListViewModel
            {{
        {props()}
            }}

            // ─── Formulario Create/Edit ────────────────────
            public partial class {entity}FormViewModel
            {{
        {props()}

                public bool isEdit => {pk_name} > 0;
            }}

            // ─── Busqueda (para pre-llenar el form en edicion) ─
            public partial class {entity}FindViewModel
            {{
        {props()}
            }}

            // ─── Detalle (read-only) ───────────────────────
            public partial class {entity}DetailViewModel
            {{
        {props()}

                public string UsuarioCreacion      {{ get; set; }}
                public DateTime FechaCrea          {{ get; set; }}
                public string UsuarioModificacion  {{ get; set; }}
                public DateTime? FechaModifica     {{ get; set; }}
            }}

            // ─── Dropdown ─────────────────────────────────
            public partial class {entity}DropdownViewModel
            {{
                public int {pk_name} {{ get; set; }}
                public string Descripcion {{ get; set; }}
            }}
        }}
    """)


def gen_repository(entity: str, schema: str, prefix: str, fields: list[dict]) -> str:
    pk_name = f'{prefix}_Id'

    def add_params(fields):
        lines = []
        for f in fields:
            lines.append(
                f'            parameter.Add("@{f["name"]}", entity.{f["name"]}, '
                f'{db_type(f["type"])}, ParameterDirection.Input);'
            )
        return '\n'.join(lines)

    insert_params = add_params(fields)
    update_params = add_params(fields)

    return textwrap.dedent(f"""\
        using Dapper;
        using PetsHome.Common;
        using PetsHome.Common.Entities;
        using PetsHome.DataAccess;
        using PetsHome.DataAccess.Extensions;
        using PetsHome.Logic.Interfaces;
        using System;
        using System.Collections.Generic;
        using System.Data;
        using System.Threading.Tasks;

        namespace PetsHome.Logic.Repositories
        {{
            public class {entity}Repository : IGenericRepository<tb{entity}>
            {{
                public async Task<IEnumerable<PR_{schema}_{entity}_ListResult>> ListAsync()
                {{
                    const string sql = "[{schema}].[PR_{schema}_{entity}_List]";
                    return await DbApp.Select<PR_{schema}_{entity}_ListResult>(sql);
                }}

                public async Task<PR_{schema}_{entity}_FindResult> FindAsync(int id)
                {{
                    const string sql = "[{schema}].[PR_{schema}_{entity}_Find]";
                    var p = new DynamicParameters();
                    p.Add("@{pk_name}", id, DbType.Int32, ParameterDirection.Input);
                    return await DbApp.Find<PR_{schema}_{entity}_FindResult>(sql, p);
                }}

                public async Task<PR_{schema}_{entity}_DetailResult> DetailAsync(int id)
                {{
                    const string sql = "[{schema}].[PR_{schema}_{entity}_Detail]";
                    var p = new DynamicParameters();
                    p.Add("@{pk_name}", id, DbType.Int32, ParameterDirection.Input);
                    return await DbApp.Detail<PR_{schema}_{entity}_DetailResult>(sql, p);
                }}

                public async Task<RequestResult> AddAsync(tb{entity} entity)
                {{
                    const string sql = "[{schema}].[PR_{schema}_{entity}_Insert]";
                    var parameter = new DynamicParameters();
        {insert_params}
                    parameter.Add("@{prefix}_UsuarioCrea", entity.{prefix}_UsuarioCrea, DbType.Int32, ParameterDirection.Input);
                    return await DbApp.ExecuteWithResult(sql, parameter);
                }}

                public async Task<RequestResult> EditAsync(tb{entity} entity)
                {{
                    const string sql = "[{schema}].[PR_{schema}_{entity}_Update]";
                    var parameter = new DynamicParameters();
                    parameter.Add("@{pk_name}", entity.{pk_name}, DbType.Int32, ParameterDirection.Input);
        {update_params}
                    parameter.Add("@{prefix}_UsuarioModifica", entity.{prefix}_UsuarioModifica, DbType.Int32, ParameterDirection.Input);
                    return await DbApp.ExecuteWithResult(sql, parameter);
                }}

                public async Task<RequestResult> RemoveAsync(int id)
                {{
                    const string sql = "[{schema}].[PR_{schema}_{entity}_Delete]";
                    var p = new DynamicParameters();
                    p.Add("@{pk_name}", id, DbType.Int32, ParameterDirection.Input);
                    p.Add("@{prefix}_UsuarioModifica", 1, DbType.Int32, ParameterDirection.Input);
                    return await DbApp.ExecuteWithResult(sql, p);
                }}
            }}
        }}
    """)


def gen_service(entity: str, schema: str) -> str:
    return textwrap.dedent(f"""\
        using AutoMapper;
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
                private readonly {entity}Repository _repo;
                private readonly ILogger<{entity}Service> _logger;
                private readonly IMapper _mapper;

                public {entity}Service({entity}Repository repo,
                    ILogger<{entity}Service> logger, IMapper mapper)
                {{
                    _repo   = repo;
                    _logger = logger;
                    _mapper = mapper;
                }}

                public async Task<List<{entity}ListViewModel>> ListAsync()
                {{
                    try
                    {{
                        var result = await _repo.ListAsync();
                        return _mapper.Map<List<{entity}ListViewModel>>(result.ToList());
                    }}
                    catch (Exception ex) {{ _logger.LogError(ex, ex.Message); return null; }}
                }}

                public async Task<{entity}FindViewModel> FindAsync(int id)
                {{
                    try
                    {{
                        var result = await _repo.FindAsync(id);
                        return _mapper.Map<{entity}FindViewModel>(result);
                    }}
                    catch (Exception ex) {{ _logger.LogError(ex, ex.Message); return null; }}
                }}

                public async Task<{entity}DetailViewModel> DetailAsync(int id)
                {{
                    try
                    {{
                        var result = await _repo.DetailAsync(id);
                        return _mapper.Map<{entity}DetailViewModel>(result);
                    }}
                    catch (Exception ex) {{ _logger.LogError(ex, ex.Message); return null; }}
                }}

                public async Task<bool> AddAsync({entity}FormViewModel model)
                {{
                    try
                    {{
                        var entity = _mapper.Map<tb{entity}>(model);
                        return (await _repo.AddAsync(entity)).Success;
                    }}
                    catch (Exception ex) {{ _logger.LogError(ex, ex.Message); return false; }}
                }}

                public async Task<bool> UpdateAsync({entity}FormViewModel model)
                {{
                    try
                    {{
                        var entity = _mapper.Map<tb{entity}>(model);
                        return (await _repo.EditAsync(entity)).Success;
                    }}
                    catch (Exception ex) {{ _logger.LogError(ex, ex.Message); return false; }}
                }}

                public async Task<bool> RemoveAsync(int id)
                {{
                    try
                    {{
                        return (await _repo.RemoveAsync(id)).Success;
                    }}
                    catch (Exception ex) {{ _logger.LogError(ex, ex.Message); return false; }}
                }}
            }}
        }}
    """)


def gen_controller(entity: str, schema: str, prefix: str, pantalla: str) -> str:
    pk_name = f'{prefix}_Id'
    return textwrap.dedent(f"""\
        using AutoMapper;
        using Microsoft.AspNetCore.Authorization;
        using Microsoft.AspNetCore.Mvc;
        using PetsHome.Business.Models;
        using PetsHome.Business.Services;
        using PetsHome.UI.Filters;
        using System.Threading.Tasks;

        namespace PetsHome.UI.Controllers
        {{
            [Authorize]
            [PantallaAuthorize("{pantalla}")]
            public class {entity}Controller : BaseController
            {{
                private readonly {entity}Service _{entity.lower()}Service;
                private readonly IMapper _mapper;

                public {entity}Controller({entity}Service {entity.lower()}Service, IMapper mapper)
                {{
                    _{entity.lower()}Service = {entity.lower()}Service;
                    _mapper = mapper;
                }}

                public IActionResult Index() => View();

                public async Task<IActionResult> List()
                {{
                    var items = await _{entity.lower()}Service.ListAsync();
                    return items != null ? Json(new {{ data = items }}) : RedirectToAction("Index");
                }}

                [PantallaAuthorize("{pantalla}", "insertar")]
                public IActionResult Create() => View(new {entity}FormViewModel());

                [PantallaAuthorize("{pantalla}", "editar")]
                public async Task<IActionResult> Find(int id)
                {{
                    var item = await _{entity.lower()}Service.FindAsync(id);
                    if (item == null) return RedirectToAction("Index");
                    return View("Create", _mapper.Map<{entity}FormViewModel>(item));
                }}

                public async Task<IActionResult> Detail(int id)
                {{
                    var item = await _{entity.lower()}Service.DetailAsync(id);
                    if (item == null) return RedirectToAction("Index");
                    return View(item);
                }}

                [HttpPost]
                public async Task<IActionResult> Add({entity}FormViewModel model)
                {{
                    if (!CurrentUserId.HasValue) return RedirectToAction("Login", "Account");

                    var op = model.isEdit ? "editar" : "insertar";
                    if (!PantallaAuthorizeAttribute.TienePermiso(User, "{pantalla}", op))
                        return RedirectToAction("AccessDenied", "Account");

                    bool ok = model.isEdit
                        ? await _{entity.lower()}Service.UpdateAsync(model)
                        : await _{entity.lower()}Service.AddAsync(model);

                    ShowAlert(ok ? AlertMessaje.SuccessSave : AlertMessaje.Error,
                              ok ? AlertMessageType.Success  : AlertMessageType.Error);
                    return ok ? RedirectToAction("Index") : View("Create", model);
                }}

                [PantallaAuthorize("{pantalla}", "eliminar")]
                public async Task<IActionResult> Remove(int {pk_name})
                {{
                    bool ok = await _{entity.lower()}Service.RemoveAsync({pk_name});
                    ShowAlert(ok ? "Eliminado" : AlertMessaje.Error,
                              ok ? AlertMessageType.Success : AlertMessageType.Error);
                    return RedirectToAction("Index");
                }}
            }}
        }}
    """)


def gen_entity_class(entity: str, schema: str, prefix: str, fields: list[dict]) -> str:
    """Genera tb{entity}.cs con los campos de la entidad + audit fields."""
    pk = f'{prefix}_Id'
    lines = [f'        public int {pk} {{ get; set; }}']
    for f in fields:
        lines.append(f'        public {cs_type(f["type"])} {f["name"]} {{ get; set; }}')
    # Audit fields (siempre presentes segun convencion del proyecto)
    lines += [
        f'        public bool {prefix}_EsEliminado {{ get; set; }}',
        f'        public int {prefix}_UsuarioCrea {{ get; set; }}',
        f'        public DateTime {prefix}_FechaCrea {{ get; set; }}',
        f'        public int? {prefix}_UsuarioModifica {{ get; set; }}',
        f'        public DateTime? {prefix}_FechaModifica {{ get; set; }}',
    ]
    body = '\n'.join(lines)
    return textwrap.dedent(f"""\
        using System;

        namespace PetsHome.Common.Entities
        {{
            public partial class tb{entity}
            {{
        {body}
            }}
        }}
    """)


def gen_index_view(entity: str, pantalla: str) -> str:
    return textwrap.dedent(f"""\
        @model {entity}ListViewModel
        @{{
            ViewData["Title"] = "{entity}";
            Layout = "~/Views/Shared/_Layout.cshtml";
            ViewData["CurrentPantalla"] = "{pantalla}";
        }}

        @section Styles {{
            <link rel="stylesheet" href="~/css/modern-table-styles.css" />
        }}

        <div class="col-12">
            <div class="pets-container">
                <div class="pets-header-full">
                    <div class="pets-header-content">
                        <div class="pets-title-section">
                            <h2 class="text-white"><i class="fas fa-list mr-2"></i>{entity}</h2>
                            <button class="btn-nueva-mascota btn-insertar"
                                    onclick="window.location.href='@Url.Action("Create")'">
                                <i class="fas fa-plus mr-2"></i>Nuevo
                            </button>
                        </div>
                        <div class="pets-controls">
                            <div class="search-container">
                                <i class="fas fa-search"></i>
                                <input type="text" id="globalSearch" placeholder="Buscar...">
                            </div>
                        </div>
                    </div>
                </div>

                <div class="pets-table-container">
                    <table class="pets-table" id="datatable">
                        <thead>
                            <tr>
                                <th>ID</th>
                                {{/* TODO: agregar columnas segun campos */}}
                                <th class="text-center">Acciones</th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>

        @section Scripts {{
        <script src="~/js/components/datatable/datatable.init.js"></script>
        <script>
            // TODO: implementar init del DataTable
            // {entity}.datatable({{
            //     urlList:   "@Url.Action("List")",
            //     urlDetail: "@Url.Action("Detail")",
            //     urlInsert: "@Url.Action("Create")",
            //     urlUpdate: "@Url.Action("Find")"
            // }});
            $('#globalSearch').on('keyup', function() {{
                $('#datatable').DataTable().search(this.value).draw();
            }});
        </script>
        }}
    """)


# ============================================================
#  PATCH DE ARCHIVOS COMPARTIDOS
# ============================================================

def patch_service_config(entity: str, base: Path) -> None:
    """
    Inserta AddScoped<{entity}Repository>() y AddScoped<{entity}Service>()
    en ServiceConfiguration.cs si aun no estan presentes.
    Es idempotente: no duplica si ya existe el registro.
    """
    sc_path = base / 'PetsHome.Business/ServiceConfiguration.cs'
    if not sc_path.exists():
        print(f'  [WARN]  ServiceConfiguration.cs no encontrado en {sc_path}')
        return

    content = sc_path.read_text(encoding='utf-8')
    changed = False

    # --- Repository en AddLogicLayer ---
    repo_line = f'            services.AddScoped<{entity}Repository>();'
    if repo_line in content:
        print(f'  [SKIP]  {entity}Repository ya registrado en ServiceConfiguration.cs')
    else:
        # Insertar antes del comentario de IUrlHelper (ultimo bloque de AddLogicLayer)
        anchor_repo = '            //https://www.it-swarm.dev'
        if anchor_repo in content:
            content = content.replace(anchor_repo, f'{repo_line}\n{anchor_repo}', 1)
            changed = True
            print(f'  [PATCH] ServiceConfiguration.cs  +AddScoped<{entity}Repository>()')
        else:
            print(f'  [WARN]  ServiceConfiguration.cs: anchor de Repository no encontrado')

    # --- Service en AddBusinessLogic ---
    svc_line = f'            services.AddScoped<{entity}Service>();'
    if svc_line in content:
        print(f'  [SKIP]  {entity}Service ya registrado en ServiceConfiguration.cs')
    else:
        # Insertar antes del bloque de configuracion de AutoMapper
        anchor_svc = '            /// Auto Mapper Configurations'
        if anchor_svc in content:
            content = content.replace(anchor_svc, f'{svc_line}\n{anchor_svc}', 1)
            changed = True
            print(f'  [PATCH] ServiceConfiguration.cs  +AddScoped<{entity}Service>()')
        else:
            print(f'  [WARN]  ServiceConfiguration.cs: anchor de Service no encontrado')

    if changed:
        sc_path.write_text(content, encoding='utf-8')


def patch_mapping_profile(entity: str, schema: str, prefix: str, base: Path) -> None:
    """
    Inserta los CreateMap<> estandar para la entidad en MappingProfileExtensions.cs
    si aun no existen. Es idempotente.
    """
    mp_path = base / 'PetsHome.Business/Extensions/MappingProfileExtensions.cs'
    if not mp_path.exists():
        print(f'  [WARN]  MappingProfileExtensions.cs no encontrado en {mp_path}')
        return

    content = mp_path.read_text(encoding='utf-8')

    # Verificar si ya existen mapeos para esta entidad
    marker = f'PR_{schema}_{entity}_ListResult'
    if marker in content:
        print(f'  [SKIP]  {entity} ya tiene mapeos en MappingProfileExtensions.cs')
        return

    # Bloque de mapeos a insertar
    new_maps = '\n'.join([
        '',
        f'            // Mapeos para {schema} - {entity}',
        f'            CreateMap<PR_{schema}_{entity}_ListResult, {entity}ListViewModel>().ReverseMap();',
        f'            CreateMap<PR_{schema}_{entity}_FindResult, {entity}FindViewModel>().ReverseMap();',
        f'            CreateMap<PR_{schema}_{entity}_DetailResult, {entity}DetailViewModel>().ReverseMap();',
        f'            CreateMap<PR_{schema}_{entity}_DropdownResult, {entity}DropdownViewModel>().ReverseMap();',
        f'            CreateMap<tb{entity}, {entity}FormViewModel>().ReverseMap();',
    ])

    # Insertar antes del cierre del constructor (unico '        }' de 8 espacios en el archivo)
    closing = '\n        }'
    idx = content.rfind(closing)
    if idx != -1:
        content = content[:idx] + new_maps + content[idx:]
        mp_path.write_text(content, encoding='utf-8')
        print(f'  [PATCH] MappingProfileExtensions.cs  +{entity} (5 CreateMap entries)')
    else:
        print(f'  [WARN]  MappingProfileExtensions.cs: no se encontro el cierre del constructor')


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


def write_all(entity: str, schema: str, prefix: str, pantalla: str, fields: list[dict]) -> None:
    sep = '=' * 60

    print(f'\n{sep}')
    print(f'  scaffold_backend.py')
    print(sep)
    print(f'  Entidad : {entity}  |  Schema: {schema}  |  Prefijo: {prefix}')
    print(f'  Pantalla: {pantalla}')
    print(f'  Campos  : {", ".join(f["name"] for f in fields)}')
    print(f'{"-"*60}\n')

    created = []
    skipped = []

    def write(path: Path, content: str, label: str):
        """Escribe el archivo solo si no existe. Si ya existe, lo omite."""
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

    # 1. SQL
    n = next_db_script_number(BASE / 'Database')
    sp_path = BASE / 'Database' / f'{n:02d}_SP_{schema.upper()}_{entity.upper()}.sql'
    write(sp_path, gen_sp(entity, schema, prefix, fields), 'SQL')

    # 2. Result classes
    common_dir = BASE / 'PetsHome.Common/Entities' / schema
    for op in ['List', 'Find', 'Detail']:
        write(common_dir / f'PR_{schema}_{entity}_{op}Result.cs',
              gen_result_class(entity, schema, prefix, fields, op), f'Result/{op}')
    write(common_dir / f'PR_{schema}_{entity}_DropdownResult.cs',
          gen_dropdown_result(entity, schema, prefix), 'Result/Dropdown')

    # 3. Entity class (tb{Entity}.cs)
    write(common_dir / f'tb{entity}.cs',
          gen_entity_class(entity, schema, prefix, fields), 'Entity')

    # 4. ViewModels
    write(BASE / 'PetsHome.Business/Models' / f'{entity}ViewModels.cs',
          gen_viewmodels(entity, schema, prefix, fields, pantalla), 'ViewModels')

    # 5. Repository
    write(BASE / 'PetsHome.Logic/Repositories' / f'{entity}Repository.cs',
          gen_repository(entity, schema, prefix, fields), 'Repository')

    # 6. Service
    write(BASE / 'PetsHome.Business/Services' / f'{entity}Service.cs',
          gen_service(entity, schema), 'Service')

    # 7. Controller
    write(BASE / 'PetsHome.UI/Controllers' / f'{entity}Controller.cs',
          gen_controller(entity, schema, prefix, pantalla), 'Controller')

    # 8. View
    write(BASE / 'PetsHome.UI/Views' / entity / 'Index.cshtml',
          gen_index_view(entity, pantalla), 'View/Index')

    # --- AUTO-PATCH ARCHIVOS COMPARTIDOS ---
    print(f'\n{"-"*60}')
    print('  Patching archivos compartidos...')
    patch_service_config(entity, BASE)
    patch_mapping_profile(entity, schema, prefix, BASE)

    print(f'\n  Creados  : {len(created)}  ({", ".join(created) if created else "ninguno"})')
    print(f'  Omitidos : {len(skipped)}  ({", ".join(skipped) if skipped else "ninguno"})')
    print(f'\n  Pasos manuales restantes:')
    print(f'    1. Ejecutar {sp_path.name} en SQL Server')
    print(f'    2. Ejecutar FIX_asignar_pantallas_admin.sql si la pantalla es nueva')
    print(f'    3. Agregar link al sidebar en _sidebar.cshtml')
    print(f'\n{sep}\n')


# ============================================================
#  CLI
# ============================================================

def main():
    parser = argparse.ArgumentParser(
        description='Genera el backend completo para una nueva entidad en PetsHome.',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=textwrap.dedent("""\
            Ejemplo:
              python scaffold_backend.py Vacuna \\
                  --schema Refugio \\
                  --prefix vac \\
                  --pantalla "Listado de vacunas" \\
                  --fields "vac_Nombre:string vac_Descripcion:string? vac_EsActivo:bit"
        """)
    )
    parser.add_argument('entity',    help='Nombre de la entidad en PascalCase (ej: CitaMedica)')
    parser.add_argument('--schema',  '-s', required=True,
                        help='Schema SQL (Refugio, Medico, Inventario, Seguridad)')
    parser.add_argument('--prefix',  '-x', required=True,
                        help='Prefijo de columnas (ej: para cita_Id usar "cita")')
    parser.add_argument('--pantalla','-p', default='',
                        help='Nombre de pantalla en el sistema de seguridad')
    parser.add_argument('--fields',  '-f', default='',
                        help='Campos: "campo1:tipo campo2:tipo?" (sin el ID, se agrega auto)')

    args = parser.parse_args()

    fields = parse_fields(args.fields) if args.fields else []
    write_all(
        entity=args.entity,
        schema=args.schema,
        prefix=args.prefix,
        pantalla=args.pantalla,
        fields=fields,
    )


if __name__ == '__main__':
    main()
