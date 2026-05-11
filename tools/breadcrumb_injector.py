"""
breadcrumb_injector.py
======================
Escanea controladores ASP.NET MVC (.cs) e inyecta atributos [Breadcrumb]
de SmartBreadcrumbs automáticamente.

Criterios para detectar un action method válido:
  1. Es public (no private/protected/internal)
  2. Retorna IActionResult o Task<IActionResult>  (o ViewResult, JsonResult, etc.)
  3. NO es constructor (mismo nombre que la clase)
  4. NO tiene ya un [Breadcrumb(...)] arriba
  5. NO es override de métodos base (OnActionExecuting, etc.)

Uso:
  python breadcrumb_injector.py --dir ./Controllers --dry-run
  python breadcrumb_injector.py --dir ./Controllers
  python breadcrumb_injector.py --file CategoriaController.cs



  # Ver qué haría SIN tocar nada
python breadcrumb_injector.py --file CategoriaController.cs --dry-run

# Procesar un archivo (crea .bak automático)
python breadcrumb_injector.py --file CategoriaController.cs

# Procesar TODA la carpeta Controllers
python breadcrumb_injector.py --dir ./Controllers

# Sin backup
python breadcrumb_injector.py --dir ./Controllers --no-backup

# Forzar reemplazo aunque ya tenga [Breadcrumb]
python breadcrumb_injector.py --file CategoriaController.cs --force
"""


import re
import sys
import argparse
from pathlib import Path

# ── Tipos de retorno que se consideran "action method" ──────────────────────
ACTION_RETURN_TYPES = {
    "IActionResult", "Task<IActionResult>",
    "ActionResult", "Task<ActionResult>",
    "ViewResult", "Task<ViewResult>",
    "JsonResult", "Task<JsonResult>",
    "PartialViewResult", "Task<PartialViewResult>",
    "RedirectResult", "Task<RedirectResult>",
    "FileResult", "Task<FileResult>",
}

# ── Métodos que NO queremos tocar aunque cumplan los criterios ───────────────
SKIP_METHODS = {
    "OnActionExecuting", "OnActionExecuted",
    "OnResultExecuting", "OnResultExecuted",
    "Dispose",
}

# ── Nombres de acción → etiqueta legible ────────────────────────────────────
ACTION_LABELS = {
    "Index":    "Listado",
    "List":     "Lista",
    "Details":  "Detalle",
    "Detail":   "Detalle",
    "Add":      "Agregar",
    "Create":   "Crear",
    "Edit":     "Editar",
    "Update":   "Actualizar",
    "Remove":   "Eliminar",
    "Delete":   "Eliminar",
    "Find":     "Buscar",
    "Search":   "Buscar",
    "View":     "Ver",
    "Export":   "Exportar",
    "Import":   "Importar",
    "Print":    "Imprimir",
    "Download": "Descargar",
    "Upload":   "Subir",
}

# ── Regex principales ────────────────────────────────────────────────────────
RE_CLASS = re.compile(
    r'public\s+class\s+(\w+Controller)\s*[:(]',
    re.MULTILINE
)

RE_METHOD = re.compile(
    r'^(?P<indent>[ \t]*)'                       # indentación
    r'(?P<attrs>(?:\[.*?\]\s*)*)'                # atributos existentes (puede ser vacío)
    r'(?P<mods>(?:public|async|virtual|override|static)\s+)+'  # modificadores
    r'(?P<ret>\S[\w<>, \[\]?]+?)\s+'             # tipo de retorno
    r'(?P<name>\w+)\s*\(',                       # nombre del método
    re.MULTILINE
)

RE_BREADCRUMB = re.compile(r'\[Breadcrumb\(', re.IGNORECASE)


def get_label(action_name: str) -> str:
    """Convierte 'GestionCargos' → 'Gestión Cargos', o usa la tabla ACTION_LABELS."""
    if action_name in ACTION_LABELS:
        return ACTION_LABELS[action_name]
    # CamelCase → palabras separadas
    words = re.sub(r'(?<=[a-z])(?=[A-Z])', ' ', action_name)
    return words


def extract_controller_name(class_name: str) -> str:
    """'CategoriaController' → 'Categoria'"""
    return class_name.replace("Controller", "")


def inject_breadcrumbs(source: str, dry_run: bool = False, force: bool = False) -> tuple[str, list[str]]:
    """
    Recibe el contenido de un archivo .cs y devuelve:
      - el contenido modificado
      - lista de mensajes de lo que se hizo / se haría
    """
    log = []

    # Detectar nombre del controlador
    cls_match = RE_CLASS.search(source)
    if not cls_match:
        log.append("  ⚠  No se encontró ninguna clase Controller.")
        return source, log

    controller_class = cls_match.group(1)
    controller_name  = extract_controller_name(controller_class)
    log.append(f"  Controlador: {controller_class}")

    # Con --force: limpiar TODAS las líneas [Breadcrumb(...)] del source primero,
    # así la lógica de inserción siguiente trabaja siempre sobre un source limpio.
    if force:
        cleaned_lines = [
            l for l in source.splitlines(keepends=True)
            if not RE_BREADCRUMB.search(l)
        ]
        source = "".join(cleaned_lines)

    lines = source.splitlines(keepends=True)
    result_lines = lines[:]  # copia que iremos modificando
    offset = 0               # compensar índices al insertar líneas

    for m in RE_METHOD.finditer(source):
        method_name = m.group("name")
        ret_type    = m.group("ret").strip()
        existing    = m.group("attrs")
        indent      = m.group("indent")

        # ── Filtros ─────────────────────────────────────────────────────────

        # 1. Tipo de retorno no es un action
        if not any(rt in ret_type for rt in ACTION_RETURN_TYPES):
            continue

        # 2. Es constructor
        if method_name == controller_class or method_name == controller_name:
            continue

        # 3. Está en la lista de exclusiones
        if method_name in SKIP_METHODS:
            continue

        # 4. Ya tiene [Breadcrumb] (solo posible si force=False)
        if RE_BREADCRUMB.search(existing):
            log.append(f"  ✓  {method_name} — ya tiene [Breadcrumb], se omite.")
            continue

        # ── Construir atributo ───────────────────────────────────────────────
        label = get_label(method_name)

        if method_name == "Index":
            # El Index del controlador es raíz de la sección
            attr = f'[Breadcrumb("{controller_name}")]'
        else:
            # Los demás apuntan al Index del mismo controlador
            attr = (
                f'[Breadcrumb("{label}", '
                f'FromAction = "Index", '
                f'FromController = typeof({controller_class}))]'
            )

        # ── Línea donde está la declaración del método ───────────────────────
        line_no = source[:m.start()].count('\n')  # 0-based
        insert_at = line_no + offset

        new_line = f"{indent}{attr}\n"

        if not dry_run:
            result_lines.insert(insert_at, new_line)
            offset += 1

        log.append(f"  {'[DRY-RUN] ' if dry_run else ''}+ {method_name}  →  {attr}")

    return "".join(result_lines), log


def process_file(path: Path, dry_run: bool, backup: bool, force: bool = False) -> None:
    print(f"\n📄 {path}")
    source = path.read_text(encoding="utf-8")
    modified, log = inject_breadcrumbs(source, dry_run=dry_run, force=force)

    for msg in log:
        print(msg)

    if not dry_run and modified != source:
        if backup:
            bak = path.with_suffix(".cs.bak")
            bak.write_text(source, encoding="utf-8")
            print(f"  💾 Backup: {bak.name}")
        path.write_text(modified, encoding="utf-8")
        print(f"  ✅ Guardado.")
    elif not dry_run:
        print(f"  ℹ  Sin cambios.")


def main():
    parser = argparse.ArgumentParser(description="Inyecta [Breadcrumb] en controladores MVC.")
    group = parser.add_mutually_exclusive_group(required=True)
    group.add_argument("--dir",  type=Path, help="Carpeta con controladores .cs")
    group.add_argument("--file", type=Path, help="Un controlador específico")
    parser.add_argument("--dry-run", action="store_true",
                        help="Solo muestra qué haría, sin modificar archivos")
    parser.add_argument("--no-backup", action="store_true",
                        help="No crear archivos .bak antes de modificar")
    parser.add_argument("--force", action="store_true",
                        help="Reemplaza [Breadcrumb] existentes en lugar de omitirlos")
    args = parser.parse_args()

    backup = not args.no_backup

    if args.dry_run:
        print("🔍 MODO DRY-RUN — no se escribirá ningún archivo\n")

    if args.force:
        print("⚠  MODO FORCE — se reemplazarán [Breadcrumb] existentes\n")

    if args.file:
        if not args.file.exists():
            print(f"❌ Archivo no encontrado: {args.file}")
            sys.exit(1)
        process_file(args.file, args.dry_run, backup, force=args.force)

    elif args.dir:
        cs_files = sorted(args.dir.rglob("*Controller.cs"))
        if not cs_files:
            print(f"❌ No se encontraron *Controller.cs en {args.dir}")
            sys.exit(1)
        print(f"🔎 Encontrados {len(cs_files)} controladores en {args.dir}")
        for f in cs_files:
            process_file(f, args.dry_run, backup, force=args.force)

    print("\n✔ Proceso completado.")


if __name__ == "__main__":
    main()
