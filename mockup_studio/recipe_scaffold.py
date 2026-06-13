"""
RecipeScaffold — Genera receta de manual base a partir de las vistas Razor
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Lee Views/{Controller}/Index.cshtml y Create.cshtml para extraer:
  - Botón "Nuevo" del listado
  - Campos del formulario en orden (input/select/textarea)
  - Botón submit
Genera manuales/{nombre}.json con valores de ejemplo según el tipo.

Uso:
  python recipe_scaffold.py Empleado
  python recipe_scaffold.py Empleado --nombre empleados   (slug del archivo)
"""

import re
import sys
import json
import argparse
from pathlib import Path

if sys.stdout.encoding and sys.stdout.encoding.lower() != "utf-8":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
        sys.stderr.reconfigure(encoding="utf-8")
    except Exception:
        pass

_HERE = Path(__file__).parent.resolve()
PROJECT_ROOT = _HERE.parent
VIEWS_DIR = PROJECT_ROOT / "PetsHome.UI" / "Views"


# ──────────────────────────────────────────────────────────
# VALORES DE EJEMPLO POR HEURÍSTICA
# ──────────────────────────────────────────────────────────
def valor_ejemplo(field_name: str, input_type: str) -> str:
    fn = field_name.lower()
    t = input_type.lower()

    # Por nombre de campo (heurística por sufijo)
    if "correo" in fn or "email" in fn:        return "ejemplo@correo.com"
    if "telefono" in fn or "phone" in fn:      return "55512345"
    if "identidad" in fn or "dpi" in fn:       return "0801199012345"
    if "codigo" in fn:                          return "COD-001"
    if "nombre" in fn:                          return "Juan"
    if "apellido" in fn:                        return "Pérez"
    if "domicilio" in fn or "direccion" in fn: return "Av. Siempre Viva 742"
    if "historia" in fn or "descripcion" in fn:return "Texto de ejemplo."
    if "color" in fn:                           return "Café"
    if "edad" in fn:                            return "3"
    if "peso" in fn:                            return "12"
    if "precio" in fn or "monto" in fn:        return "100"
    if "cantidad" in fn:                        return "5"

    # Por tipo HTML
    if t == "email":     return "ejemplo@correo.com"
    if t == "tel":       return "55512345"
    if t == "number":    return "1"
    if t == "date":      return "2000-01-01"
    if t == "datetime-local": return "2025-01-01T10:00"

    return "Ejemplo"


# ──────────────────────────────────────────────────────────
# PARSER DEL .cshtml
# ──────────────────────────────────────────────────────────
# Captura el label completo, ignorando spans/iconos internos.
RX_LABEL = re.compile(
    r'<label[^>]*asp-for="([^"]+)"[^>]*>(.*?)</label>',
    re.IGNORECASE | re.DOTALL,
)
RX_TAGS_INTERIORES = re.compile(r'<[^>]+>')

# Captura el tag completo (input/select/textarea) con asp-for
RX_FIELD = re.compile(
    r'<(input|select|textarea)\b([^>]*)\basp-for="([^"]+)"([^>]*)>',
    re.IGNORECASE,
)

RX_TYPE = re.compile(r'\btype="([^"]+)"', re.IGNORECASE)


def labels_por_campo(html: str) -> dict:
    out = {}
    for m in RX_LABEL.finditer(html):
        raw = m.group(2)
        limpio = RX_TAGS_INTERIORES.sub("", raw)            # quitar <span>, <i>, etc.
        limpio = re.sub(r"\s+", " ", limpio).strip()        # colapsar espacios
        limpio = limpio.replace("*", "").strip()
        if limpio:
            out[m.group(1)] = limpio
    return out


def extraer_campos(html: str) -> list:
    """Retorna lista ordenada de dicts: {name, kind, type, label}."""
    labels = labels_por_campo(html)
    campos = []
    for m in RX_FIELD.finditer(html):
        kind = m.group(1).lower()                 # input | select | textarea
        attrs = m.group(2) + " " + m.group(4)
        name  = m.group(3)
        tmatch = RX_TYPE.search(attrs)
        itype = tmatch.group(1).lower() if tmatch else ("text" if kind == "input" else kind)

        # Saltar campos invisibles o booleanos especiales
        if itype in ("hidden", "file", "checkbox"):
            continue

        campos.append({
            "name":  name,
            "kind":  kind,                                  # input/select/textarea
            "type":  itype,
            "label": labels.get(name, name),
        })
    return campos


def detectar_boton_nuevo(index_html: str) -> str:
    """Busca un <button> cuyo tag mencione 'Create' y tenga clase asignable."""
    for m in re.finditer(r'<button\b([^>]*)>', index_html, re.IGNORECASE):
        attrs = m.group(1)
        if "Create" not in attrs:
            continue
        cls = re.search(r'class="([^"]+)"', attrs)
        if cls:
            primary = cls.group(1).split()[0]
            return f".{primary}"
    return "a[href*='/Create']"


def detectar_buscador(index_html: str) -> str | None:
    m = re.search(r'<input[^>]*id="(globalSearch|search\w*)"', index_html, re.IGNORECASE)
    return f"#{m.group(1)}" if m else None


def detectar_acciones_extra(index_html: str) -> list:
    """Detecta botones .action-btn.btn-XXX en las filas que no sean view/edit/delete.
    Devuelve lista de dicts: {clase, titulo}.
    """
    estandar = {"btn-view", "btn-edit", "btn-delete"}
    extras = []
    vistos = set()
    for m in re.finditer(
        r'<button[^>]*class="action-btn\s+(btn-[a-z\-]+)"[^>]*title="([^"]+)"',
        index_html, re.IGNORECASE,
    ):
        cls, titulo = m.group(1), m.group(2)
        if cls in estandar or cls in vistos:
            continue
        vistos.add(cls)
        extras.append({"clase": cls, "titulo": titulo})
    return extras


# ──────────────────────────────────────────────────────────
# SIDEBAR
# ──────────────────────────────────────────────────────────
def detectar_sidebar(controller: str) -> tuple[str | None, str | None, str | None]:
    """Retorna (grupo_id, selector-grupo, selector-link)."""
    sidebar_path = VIEWS_DIR / "Shared" / "_sidebar.cshtml"
    if not sidebar_path.exists():
        return None, None, None
    text = sidebar_path.read_text(encoding="utf-8", errors="ignore")
    lines = text.splitlines()

    link_line = -1
    for i, ln in enumerate(lines):
        if f'"Index", "{controller}"' in ln:
            link_line = i
            break
    if link_line < 0:
        return None, None, f"a[href='/{controller}']"

    grupo_id = None
    for j in range(link_line, -1, -1):
        m = re.search(r'href="#(\w+)"\s+data-toggle="collapse"', lines[j])
        if m:
            grupo_id = m.group(1)
            break

    grupo_sel = f"a[href='#{grupo_id}']" if grupo_id else None
    return grupo_id, grupo_sel, f"a[href='/{controller}']"


# ──────────────────────────────────────────────────────────
# GENERAR RECETA
# ──────────────────────────────────────────────────────────
def generar_receta(controller: str, nombre_slug: str) -> dict:
    index_path  = VIEWS_DIR / controller / "Index.cshtml"
    create_path = VIEWS_DIR / controller / "Create.cshtml"
    if not index_path.exists() or not create_path.exists():
        raise FileNotFoundError(f"Faltan vistas en Views/{controller}/")

    index_html  = index_path.read_text(encoding="utf-8", errors="ignore")
    create_html = create_path.read_text(encoding="utf-8", errors="ignore")

    grupo_id, _, link_sel = detectar_sidebar(controller)
    boton_nuevo    = detectar_boton_nuevo(index_html)
    buscador_sel   = detectar_buscador(index_html)
    acciones_extra = detectar_acciones_extra(index_html)
    campos         = extraer_campos(create_html)

    titulo_humano = re.sub(r"([A-Z])", r" \1", controller).strip()
    titulo_lower  = titulo_humano.lower()

    # ── Acceder: un solo paso, con el menú expandido ─────────────
    paso_acceder = {
        "descripcion": f"Desde el menú lateral, ingresa a '{titulo_humano}' para abrir el listado.",
        "url": f"/{controller}/Index",
        "click": link_sel,
    }
    if grupo_id:
        paso_acceder["expandir_menu"] = [f"#{grupo_id}"]
    pasos_listado = [paso_acceder]

    # ── Crear: form completo ─────────────────────────────────────
    pasos_crear = [{
        "descripcion": "En la pantalla de listado, haz clic en el botón 'Nuevo' para abrir el formulario.",
        "url": f"/{controller}/Index",
        "click": boton_nuevo,
        "esperar_selector": boton_nuevo,
    }]
    for c in campos:
        sel = f"[name='{c['name']}']"
        if c["kind"] == "select":
            desc = f"Selecciona una opción en el desplegable '{c['label']}'."
        elif c["kind"] == "textarea":
            desc = f"Escribe el contenido en el campo '{c['label']}'."
        else:
            desc = f"Completa el campo '{c['label']}'."
        paso = {
            "descripcion": desc,
            "url": f"/{controller}/Create",
            "click": sel,
        }
        if c["kind"] == "select":
            paso["select"] = { sel: 1 }
        else:
            paso["fill"] = { sel: valor_ejemplo(c["name"], c["type"]) }
        pasos_crear.append(paso)
    pasos_crear.append({
        "descripcion": "Cuando todos los campos estén completos, haz clic en 'Guardar' para registrar el nuevo registro.",
        "url": f"/{controller}/Create",
        "click": "button.btn-primary[type='submit']",
    })

    # ── Editar: ir al listado, resaltar botón de edición ────────
    pasos_editar = [{
        "descripcion": (
            f"En el listado, ubica la fila del registro que deseas modificar y "
            f"haz clic en el botón de editar (lápiz). Se abrirá el mismo "
            f"formulario que en 'Crear', pero con los datos del registro "
            f"pre-cargados — sigue los mismos pasos descritos en la sección "
            f"'Crear un nuevo registro'."
        ),
        "url": f"/{controller}/Index",
        "esperar_selector": ".action-btn.btn-edit",
        "click": ".action-btn.btn-edit",
    }]

    # ── Eliminar ─────────────────────────────────────────────────
    pasos_eliminar = [{
        "descripcion": (
            f"En el listado, ubica la fila del registro que deseas eliminar y "
            f"haz clic en el botón de eliminar (papelera). El sistema te "
            f"pedirá confirmar la acción antes de borrar el registro."
        ),
        "url": f"/{controller}/Index",
        "esperar_selector": ".action-btn.btn-delete",
        "click": ".action-btn.btn-delete",
    }]

    secciones = [
        {"nombre": f"Acceder al listado de {titulo_lower}",      "pasos": pasos_listado},
        {"nombre": "Crear un nuevo registro",                     "pasos": pasos_crear},
        {"nombre": "Editar un registro existente",                "pasos": pasos_editar},
        {"nombre": "Eliminar un registro",                        "pasos": pasos_eliminar},
    ]

    # ── Acciones extra (.btn-medical, etc.) ──────────────────────
    for accion in acciones_extra:
        secciones.append({
            "nombre": f"Acción adicional: {accion['titulo']}",
            "pasos": [{
                "descripcion": (
                    f"En el listado, ubica la fila correspondiente y haz clic "
                    f"en el botón '{accion['titulo']}'. El sistema abrirá la "
                    f"pantalla asociada a esta acción."
                ),
                "url": f"/{controller}/Index",
                "esperar_selector": f".action-btn.{accion['clase']}",
                "click": f".action-btn.{accion['clase']}",
            }],
        })

    if buscador_sel:
        secciones.append({
            "nombre": "Buscar en el listado",
            "pasos": [{
                "descripcion": "Usa el buscador global para filtrar los resultados por nombre o cualquier campo visible.",
                "url": f"/{controller}/Index",
                "fill": { buscador_sel: "Ejemplo" },
                "click": buscador_sel,
            }],
        })

    return {
        "titulo":    f"Manual del Módulo de {titulo_humano}",
        "subtitulo": f"Cómo gestionar el catálogo de {titulo_lower}",
        "autor":     "PetsHome",
        "secciones": secciones,
    }


# ──────────────────────────────────────────────────────────
# MAIN
# ──────────────────────────────────────────────────────────
def main():
    parser = argparse.ArgumentParser(description="Scaffold de receta de manual")
    parser.add_argument("controller", help="Nombre del Controller (ej: Empleado)")
    parser.add_argument("--nombre", help="Slug del archivo de receta", default=None)
    parser.add_argument("--stdout", action="store_true", help="Imprime JSON en vez de escribir archivo")
    args = parser.parse_args()

    nombre = args.nombre or args.controller.lower()
    receta = generar_receta(args.controller, nombre)

    if args.stdout:
        print(json.dumps(receta, indent=2, ensure_ascii=False))
        return

    out_dir = _HERE / "manuales"
    out_dir.mkdir(exist_ok=True)
    out_path = out_dir / f"{nombre}.json"
    out_path.write_text(
        json.dumps(receta, indent=2, ensure_ascii=False),
        encoding="utf-8",
    )
    print(f"✅ Receta generada: {out_path.relative_to(PROJECT_ROOT)}")
    print(f"   Secciones: {len(receta['secciones'])}")
    for s in receta["secciones"]:
        print(f"   • {s['nombre']} ({len(s['pasos'])} pasos)")
    print(f"\nPróximo paso:")
    print(f"   python manual_gen.py {nombre}")


if __name__ == "__main__":
    main()
