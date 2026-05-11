#!/usr/bin/env python3
"""
Oculta el campo ID en todas las tablas DataTable de PetsHome.
Corrige bugs en datatable.init.js y datatable.catalogs.init.js.
"""

import os
import re

BASE = r"c:\Users\nayel\OneDrive\Documentos\GitHub\001 Proyectos Estables\PetsHome\PetsHome.UI\wwwroot\js"
PAGES = os.path.join(BASE, "pages")
DT_PATH = os.path.join(BASE, r"components\datatable")

# Ya tienen el ID oculto correctamente
ALREADY_HIDDEN = {"raza.js", "evento.js", "historial-medico.js"}


def read(path):
    with open(path, "r", encoding="utf-8") as f:
        return f.read()


def write(path, content):
    with open(path, "w", encoding="utf-8") as f:
        f.write(content)


def hide_id_in_page(filepath):
    filename = os.path.basename(filepath)
    content = read(filepath)
    lines = content.split("\n")
    new_lines = lines[:]
    modified = False
    id_found = False

    i = 0
    while i < len(new_lines) and not id_found:
        line = new_lines[i]

        # Busca el primer campo cuyo FieldName termina exactamente en _Id
        if re.search(r"FieldName\s*:\s*['\"][\w]+_Id['\"]", line):
            id_found = True

            if "Visibility: false" in line or "Visibility:false" in line:
                pass  # ya oculto

            elif "{" in line and "}" in line:
                # Entrada de una sola linea
                if "Visibility: true" in line:
                    new_lines[i] = line.replace("Visibility: true", "Visibility: false")
                else:
                    # Insertar Visibility: false antes del primer }
                    new_lines[i] = re.sub(r"\s*\}", ", Visibility: false}", line, count=1)
                modified = True

            else:
                # Entrada multilinea: buscar Visibility en las siguientes lineas
                j = i + 1
                changed = False
                while j < len(new_lines) and not changed:
                    jline = new_lines[j]
                    if "Visibility: false" in jline or "Visibility:false" in jline:
                        changed = True  # ya oculto
                    elif "Visibility: true" in jline or "Visibility:true" in jline:
                        new_lines[j] = (
                            jline.replace("Visibility: true", "Visibility: false")
                                 .replace("Visibility:true", "Visibility:false")
                        )
                        modified = True
                        changed = True
                    elif re.search(r"^\s*\},?\s*$", jline):
                        # Fin del objeto sin Visibility: insertar antes del cierre
                        indent = re.match(r"^(\s*)", jline).group(1) + "    "
                        new_lines.insert(j, indent + "Visibility: false,")
                        modified = True
                        changed = True
                    j += 1

        i += 1

    if modified:
        write(filepath, "\n".join(new_lines))
        print(f"  [OK] {filename}")
    elif id_found:
        print(f"  [--] {filename} (sin cambios)")
    else:
        print(f"  [??] {filename} (no se encontro campo _Id)")


def fix_datatable_init():
    filepath = os.path.join(DT_PATH, "datatable.init.js")
    content = read(filepath)
    original = content

    # Bug: visible = "false" (string) debe ser false (boolean)
    content = content.replace(
        'head[i][\'visible\'] = "false"',
        "head[i]['visible'] = false"
    )

    # Agregar stateSave: false en la inicializacion del DataTable
    if "stateSave: false" not in content:
        lines = content.split("\n")
        for i, line in enumerate(lines):
            if "columnDefs: obj.dataHeader(header)" in line:
                indent = re.match(r"^(\s*)", line).group(1)
                lines[i] = line.rstrip() + ","
                lines.insert(i + 1, indent + "stateSave: false")
                break
        content = "\n".join(lines)

    if content != original:
        write(filepath, content)
        print("  [OK] datatable.init.js")
    else:
        print("  [--] datatable.init.js (sin cambios)")


def fix_catalogs_init():
    filepath = os.path.join(DT_PATH, "datatable.catalogs.init.js")
    content = read(filepath)
    original = content

    # Corregir condicion erronea: ocultaba columnas con Visibility: true tambien
    content = content.replace(
        "if (_header[i].Visibility == false || _header[i].Visibility != undefined) {",
        "if (_header[i].Visibility === false) {"
    )

    if content != original:
        write(filepath, content)
        print("  [OK] datatable.catalogs.init.js")
    else:
        print("  [--] datatable.catalogs.init.js (sin cambios)")


def fix_partials_init():
    filepath = os.path.join(DT_PATH, "datatable.partials.init.js")
    if not os.path.exists(filepath):
        return

    content = read(filepath)
    original = content

    # Aplicar mismas correcciones si aplica
    content = content.replace(
        "if (_header[i].Visibility == false || _header[i].Visibility != undefined) {",
        "if (_header[i].Visibility === false) {"
    )
    content = content.replace(
        'head[i][\'visible\'] = "false"',
        "head[i]['visible'] = false"
    )
    if "stateSave: false" not in content:
        lines = content.split("\n")
        for i, line in enumerate(lines):
            if "columnDefs: obj.dataHeader(header)" in line:
                indent = re.match(r"^(\s*)", line).group(1)
                lines[i] = line.rstrip() + ","
                lines.insert(i + 1, indent + "stateSave: false")
                break
        content = "\n".join(lines)

    if content != original:
        write(filepath, content)
        print("  [OK] datatable.partials.init.js")
    else:
        print("  [--] datatable.partials.init.js (sin cambios)")


if __name__ == "__main__":
    print("=== Init files ===")
    fix_datatable_init()
    fix_catalogs_init()
    fix_partials_init()

    print("\n=== Page files ===")
    for filename in sorted(os.listdir(PAGES)):
        if not filename.endswith(".js"):
            continue
        if filename in ALREADY_HIDDEN:
            print(f"  [skip] {filename}")
            continue
        hide_id_in_page(os.path.join(PAGES, filename))

    print("\nListo.")
