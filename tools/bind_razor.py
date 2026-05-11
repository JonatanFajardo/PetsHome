#!/usr/bin/env python3
"""
bind_razor.py
═══════════════════════════════════════════════════════════════
Empaqueta todo lo que una IA externa necesita para conectar
una vista Razor estática a su ViewModel + Result classes.

Qué hace:
  1. Encuentra la vista .cshtml generada por html_to_razor.py
  2. Encuentra el ViewModel del controller
  3. Extrae los nombres de las Result classes desde el ViewModel
  4. Encuentra cada archivo Result class en Common/Entities
  5. Genera un archivo bind_task_{controller}_{action}.md
     listo para pegar en Claude.ai, ChatGPT, etc.

Uso:
  python bind_razor.py <Controller> <Action>

Ejemplos:
  python bind_razor.py PerfilMedico Index
  python bind_razor.py Mascota Index
  python bind_razor.py CitaMedica Create
═══════════════════════════════════════════════════════════════
"""

import argparse
import re
import sys
from pathlib import Path

BASE = Path(__file__).parent

# ─────────────────────────────────────────────────────────────
#  RUTAS DEL PROYECTO
# ─────────────────────────────────────────────────────────────
VIEWS_DIR   = BASE / "PetsHome.UI"    / "Views"
MODELS_DIR  = BASE / "PetsHome.Business" / "Models"
ENTITIES_DIR = BASE / "PetsHome.Common"  / "Entities"

# ─────────────────────────────────────────────────────────────
#  CONVENCIONES DE BINDING (incluidas en el prompt)
# ─────────────────────────────────────────────────────────────
CONVENTIONS = """\
## Convenciones del proyecto PetsHome

### Estructura de la vista
```razor
@model PetsHome.Business.Models.{Entity}ViewModel
@{
    ViewData["Title"]           = "...";
    Layout                      = "~/Views/Shared/_Layout.cshtml";
    ViewData["CurrentPantalla"] = "Nombre pantalla";   // mismo valor que [PantallaAuthorize]

    // Si una sección tiene solo 1 fila (detalle/cabecera):
    var ficha = Model.NombreSeccion.FirstOrDefault();
}
```

### Listas → @foreach
```razor
@foreach (var item in Model.NombreLista)
{
    <div>@item.Campo</div>
}
```

### Sección de 1 sola fila (ej: ficha de mascota)
```razor
var ficha = Model.FichaMascota.FirstOrDefault();
// luego usar: @ficha?.Campo  (con ? por si es null)
```

### Empty state cuando no hay datos
```razor
@if (!Model.Lista.Any())
{
    <div class="empty-state">...</div>
}
```

### Fechas
```razor
@item.FechaCampo.ToString("dd MMM yyyy")
@item.FechaNullable?.ToString("dd MMM yyyy")
```

### Sección Scripts con window._urls
```razor
@section Scripts {
  <script>
    window._urls = {
      accion: '@Url.Action("Accion", "Controller")'
    };
  </script>
  <script src="~/js/pages/{slug}.js"></script>
}
```

### Regla importante
- Usar los nombres de campo EXACTAMENTE como están en las Result classes.
- No inventar propiedades que no existen en el ViewModel.
- Devolver SOLO el .cshtml completo, sin explicaciones ni bloques de código extra.
"""

# ─────────────────────────────────────────────────────────────
#  HELPERS
# ─────────────────────────────────────────────────────────────

def find_view(controller: str, action: str) -> Path | None:
    path = VIEWS_DIR / controller / f"{action}.cshtml"
    return path if path.exists() else None


def find_viewmodel(controller: str) -> Path | None:
    # Busca {Controller}ViewModel.cs en Business/Models (recursivo)
    pattern = f"{controller}ViewModel.cs"
    for f in MODELS_DIR.rglob(pattern):
        return f
    return None


def extract_result_class_names(viewmodel_code: str) -> list[str]:
    """Extrae nombres de clases PR_..._Result del ViewModel."""
    return list(dict.fromkeys(
        re.findall(r'\bPR_[A-Za-z0-9_]+Result\b', viewmodel_code)
    ))


def find_result_class(class_name: str) -> Path | None:
    filename = f"{class_name}.cs"
    for f in ENTITIES_DIR.rglob(filename):
        return f
    return None


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def section(title: str, content: str, lang: str = "") -> str:
    fence = f"```{lang}" if lang else "```"
    return f"\n## {title}\n\n{fence}\n{content.strip()}\n```\n"

# ─────────────────────────────────────────────────────────────
#  MAIN
# ─────────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(
        description="Genera un archivo de tarea para que una IA externa conecte la vista Razor al modelo."
    )
    parser.add_argument("controller", help="Nombre del controller (ej: PerfilMedico)")
    parser.add_argument("action",     help="Nombre de la acción/vista (ej: Index)")
    args = parser.parse_args()

    controller = args.controller
    action     = args.action

    print("=" * 60)
    print("  bind_razor.py")
    print("=" * 60)

    # 1. Vista
    view_path = find_view(controller, action)
    if not view_path:
        print(f"  [ERROR] No se encontró la vista:")
        print(f"          {VIEWS_DIR / controller / (action + '.cshtml')}")
        print("  Ejecuta primero html_to_razor.py para generarla.")
        sys.exit(1)
    print(f"  [View]      {view_path.relative_to(BASE)}")

    # 2. ViewModel
    vm_path = find_viewmodel(controller)
    if not vm_path:
        print(f"  [ERROR] No se encontró el ViewModel: {controller}ViewModel.cs")
        print("  Ejecuta primero scaffold_backend.py o scaffold.py.")
        sys.exit(1)
    print(f"  [ViewModel] {vm_path.relative_to(BASE)}")
    vm_code = read(vm_path)

    # 3. Result classes
    result_names = extract_result_class_names(vm_code)
    result_classes = {}
    for name in result_names:
        path = find_result_class(name)
        if path:
            result_classes[name] = read(path)
            print(f"  [Result]    {path.relative_to(BASE)}")
        else:
            print(f"  [WARN]      No encontrada: {name}.cs")

    # 4. Armar el documento
    lines = []
    lines.append(f"# TAREA: Conectar vista Razor al modelo\n")
    lines.append(f"> Controller: `{controller}` | Action: `{action}`\n")
    lines.append(
        "Tu tarea es tomar la vista Razor estática y conectarla al ViewModel.\n"
        "Reemplaza todos los datos hardcodeados con bindings `@Model`, `@foreach`, etc.\n"
        "**Devuelve SOLO el contenido del .cshtml completo y actualizado.**\n"
    )

    lines.append(CONVENTIONS)

    lines.append(section(f"ViewModel: {controller}ViewModel.cs", vm_code, "csharp"))

    if result_classes:
        lines.append("\n## Result Classes\n")
        for name, code in result_classes.items():
            lines.append(section(name, code, "csharp"))

    lines.append(section(
        f"Vista estática a transformar: {controller}/{action}.cshtml",
        read(view_path),
        "razor"
    ))
    lines.append(
        "\n---\n"
        "## Reglas de lógica — OBLIGATORIAS\n\n"

        "### 1. Cero ternarios sin efecto\n"
        "Nunca escribas un ternario donde ambas ramas devuelven el mismo valor.\n"
        "```csharp\n"
        "// ❌ PROHIBIDO\n"
        "var cls = condicion ? \"mismo-valor\" : \"mismo-valor\";\n"
        "// ✅ CORRECTO\n"
        "var cls = condicion ? \"valor-a\" : \"valor-b\";\n"
        "```\n\n"

        "### 2. Cero valores hardcodeados que cambien con el tiempo\n"
        "Fechas, años y horas deben generarse en tiempo de ejecución.\n"
        "```html\n"
        "<!-- ❌ PROHIBIDO -->\n"
        "<input type=\"date\" value=\"2026-04-25\" />\n"
        "<!-- ✅ CORRECTO -->\n"
        "<input type=\"date\" value=\"@DateTime.Now.ToString(\"yyyy-MM-dd\")\" />\n"
        "```\n\n"

        "### 3. Sin lógica duplicada en la vista\n"
        "Si el mismo bloque if/switch para calcular una clase CSS aparece más de una vez, "
        "extráelo en una Func<> al inicio del bloque @{ }.\n"
        "```csharp\n"
        "// ✅ Definir una vez, usar en todos los foreach\n"
        "Func<string, string> badgeClass = tipo => tipo switch\n"
        "{\n"
        "    \"Urgencia\"   => \"badge-urgencia\",\n"
        "    \"Vacunación\" => \"badge-vacuna\",\n"
        "    _            => \"badge-control\"\n"
        "};\n"
        "```\n\n"

        "---\n"
        "## Reglas de HTML — OBLIGATORIAS\n\n"

        "### 4. Sin event handlers inline\n"
        "Nunca uses onclick, onchange, etc. en el HTML. Asigna id al elemento; el handler va en el .js externo.\n"
        "```html\n"
        "<!-- ❌ PROHIBIDO -->\n"
        "<button onclick=\"document.getElementById('modal').style.display='flex'\">\n"
        "<!-- ✅ CORRECTO -->\n"
        "<button id=\"btnAbrirModal\">\n"
        "```\n\n"

        "### 5. Sin estilos inline de presentación\n"
        "Los valores de style=\"\" de diseño fijo van en el .css. "
        "Solo se permiten inline para valores dinámicos del modelo (p. ej. style=\"width:@pct%\").\n"
        "```html\n"
        "<!-- ❌ PROHIBIDO -->\n"
        "<div style=\"font-size:12px;color:#888\">\n"
        "<!-- ✅ CORRECTO -->\n"
        "<div class=\"ficha-details\">\n"
        "```\n\n"

        "### 6. Visibilidad de modales con hidden, no con style=\"display:none\"\n"
        "```html\n"
        "<!-- ❌ PROHIBIDO -->\n"
        "<div id=\"modal\" style=\"display:none\">\n"
        "<!-- ✅ CORRECTO -->\n"
        "<div id=\"modal\" hidden aria-modal=\"true\" role=\"dialog\">\n"
        "```\n\n"

        "---\n"
        "## Reglas de formato — OBLIGATORIAS\n\n"

        "### 7. Indentación consistente de 4 espacios\n"
        "Cada nivel de anidación agrega 4 espacios. Aplica a HTML, Razor y C# dentro de @{ }.\n\n"

        "### 8. Comentario de cierre en divs contenedores principales\n"
        "Todo </div> que cierre un bloque con más de ~20 líneas debe llevar un comentario identificador.\n"
        "```html\n"
        "    </div><!-- /tab-resumen -->\n"
        "</div><!-- /content-area -->\n"
        "```\n\n"

        "### 9. Alinear operadores ternarios multilínea\n"
        "```csharp\n"
        "var cls = estado == \"Vencida\" ? \"dot-venc\"    :\n"
        "          estado == \"Próxima\" ? \"dot-proxima\" : \"dot-ok\";\n"
        "```\n\n"

        "---\n"
        "## Checklist — verifica esto antes de generar el archivo\n\n"
        "- [ ] ¿Algún ternario tiene ambas ramas iguales? → corregir\n"
        "- [ ] ¿Hay alguna fecha, año o valor temporal hardcodeado? → usar expresión C#\n"
        "- [ ] ¿Hay bloques if/switch para clases CSS repetidos más de una vez? → extraer a Func<>\n"
        "- [ ] ¿Hay algún onclick / onchange inline? → mover a JS externo con id\n"
        "- [ ] ¿Hay style=\"\" con valores de diseño fijos? → mover a CSS con clase\n"
        "- [ ] ¿Algún modal usa style=\"display:none\"? → usar hidden\n"
        "- [ ] ¿Toda la indentación es de 4 espacios y consistente? → revisar anidación\n"
        "- [ ] ¿Los divs principales tienen comentario de cierre? → agregar\n\n"

        "---\n"
        "**Instrucción final:** Devuelve únicamente el archivo `.cshtml` "
        "con todos los bindings aplicados y con todas las reglas anteriores cumplidas. "
        "Sin explicaciones, sin bloques markdown extra. "
        "Solo el contenido del archivo.\n"
    )

    output_text = "\n".join(lines)

    # 5. Guardar
    out_file = BASE / f"bind_task_{controller}_{action}.md"
    out_file.write_text(output_text, encoding="utf-8")

    kb = len(output_text.encode("utf-8")) / 1024
    print()
    print(f"  Archivo generado: {out_file.name}  ({kb:.1f} KB)")
    print()
    print("  Próximos pasos:")
    print(f"  1. Abre bind_task_{controller}_{action}.md")
    print("  2. Copia el contenido completo")
    print("  3. Pégalo en Claude.ai, ChatGPT, etc.")
    print(f"  4. Reemplaza Views/{controller}/{action}.cshtml con la respuesta")
    print("=" * 60)


if __name__ == "__main__":
    main()
