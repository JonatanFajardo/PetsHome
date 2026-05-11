#!/usr/bin/env python3
"""
html_to_razor.py
════════════════════════════════════════��═══════════════════════
Convierte un diseño HTML estático a una vista Razor (.cshtml)
siguiendo las convenciones del proyecto PetsHome (.NET Core MVC).

Qué hace:
  1. Extrae <style>   → wwwroot/css/{slug}.css
  2. Extrae <script>  → wwwroot/js/pages/{slug}.js
  3. Detecta URLs en el JS y genera un bloque de inyección
     con @Url.Action para que el .js no tenga rutas hardcodeadas
  4. Conserva los CDN <link> en @section Styles
  5. Elimina el topbar del HTML (el _Layout ya lo provee)
  6. Genera el .cshtml con header Razor + secciones correctas

Uso:
  python html_to_razor.py <archivo.html> [opciones]

Ejemplos:
  python html_to_razor.py calendario-citas.html
  python html_to_razor.py mi-pantalla.html \\
      --controller Mascota \\
      --action Galeria \\
      --pantalla "Listado de mascotas" \\
      --views-dir PetsHome.UI/Views \\
      --wwwroot    PetsHome.UI/wwwroot

Dependencias:
  pip install beautifulsoup4
═══════════════════════════════���════════════════════════════��═══
"""

import argparse
import os
import re
import sys
import textwrap
from pathlib import Path

# ────────────────────���────────────────────────────────────────
#  BeautifulSoup  (requerido)
# ─────────────────────────────────────────────────────────────
try:
    from bs4 import BeautifulSoup
except ImportError:
    print("ERROR: Instala beautifulsoup4 primero:")
    print("       pip install beautifulsoup4")
    sys.exit(1)


# ═════════════════��════════════════════════════════���═══════════
#  DETECCIÓN DE URLs EN JS
# ══════════════════════════════════════════════════════════════
# Patrones que indican una ruta MVC en el JS.
# Captura strings como '/Controller/Action' o '/Controller/Action/param'
_URL_PATTERN = re.compile(
    r"""(['"`])(/[A-Za-z][A-Za-z0-9]*(?:/[A-Za-z][A-Za-z0-9]*)(?:/[^'"`\s]*)?)['"`]"""
)

# Rutas que no son rutas MVC (CDNs, paths absolutos comunes)
_URL_IGNORE = re.compile(r'^/(cdn|assets|plugins|bootstrap|fonts|images|css|js|img)', re.I)


def _camel(s: str) -> str:
    """'CalendarioData' → 'calendarioData'"""
    return s[0].lower() + s[1:] if s else s


def detectar_urls_js(js_code: str) -> dict[str, tuple[str, str]]:
    """
    Detecta strings que parecen rutas /Controller/Action en el código JS.
    Devuelve dict: { 'varName': ('Controller', 'Action') }
    """
    urls = {}
    for m in _URL_PATTERN.finditer(js_code):
        ruta = m.group(2)
        if _URL_IGNORE.match(ruta):
            continue
        partes = [p for p in ruta.strip('/').split('/') if p]
        if len(partes) >= 2:
            controller, action = partes[0], partes[1]
            var_name = _camel(action) + _camel(controller)
            if var_name not in urls:
                urls[var_name] = (controller, action)
    return urls


def reemplazar_urls_js(js_code: str, url_map: dict[str, tuple[str, str]]) -> str:
    """
    Reemplaza las rutas hardcodeadas en el JS por referencias a window._urls.xxx
    """
    for var_name, (controller, action) in url_map.items():
        # Construye el patrón de búsqueda para esta ruta específica
        ruta_base = f'/{controller}/{action}'
        # Reemplaza ocurrencias del string de ruta
        js_code = re.sub(
            r"""(['"`])""" + re.escape(ruta_base) + r"""(['"`/][^'"`]*)(['"`])""",
            lambda m: f'window._urls.{var_name}',
            js_code
        )
        # También reemplaza la forma simple sin trailing slash
        js_code = js_code.replace(f"'{ruta_base}'", f'window._urls.{var_name}')
        js_code = js_code.replace(f'"{ruta_base}"', f'window._urls.{var_name}')
        js_code = js_code.replace(f'`{ruta_base}`', f'window._urls.{var_name}')
    return js_code


# ═══════════════════════════════════════════════════════��══════
#  GENERACIÓN DE CONTENIDO
# ══════════════════════════════════════════════════════════════

def generar_bloque_urls(url_map: dict[str, tuple[str, str]], indent: str = '  ') -> str:
    """Genera el bloque <script> que inyecta las URLs desde Razor."""
    if not url_map:
        return ''
    lineas = [f'{indent}<script>',
              f'{indent}  // URLs generadas por html_to_razor.py — no editar manualmente',
              f'{indent}  window._urls = {{']
    for var_name, (controller, action) in url_map.items():
        lineas.append(f"{indent}    {var_name}: '@Url.Action(\"{action}\", \"{controller}\")',")
    # quitar la coma del último elemento
    lineas[-1] = lineas[-1].rstrip(',')
    lineas.append(f'{indent}  }};')
    lineas.append(f'{indent}</script>')
    return '\n'.join(lineas)


def generar_cshtml(
    *,
    controller: str,
    action: str,
    pantalla: str,
    titulo: str,
    cdn_links: list[str],
    css_slug: str,
    js_slug: str,
    body_html: str,
    url_map: dict[str, tuple[str, str]],
    tiene_css: bool,
    tiene_js: bool,
) -> str:
    """Genera el contenido completo del archivo .cshtml."""

    # ── Header Razor ────────────────────────────────────────
    header = textwrap.dedent(f"""\
        @{{
            ViewData["Title"] = "{titulo}";
            Layout = "~/Views/Shared/_Layout.cshtml";
            ViewData["CurrentPantalla"] = "{pantalla}";
        }}
    """)

    # ── @section Styles ──────────────────────────────────���───
    styles_lines = []
    if tiene_css:
        styles_lines.append(f'  <link rel="stylesheet" href="~/css/{css_slug}.css" />')
    for cdn in cdn_links:
        styles_lines.append(f'  {cdn}')

    section_styles = ''
    if styles_lines:
        section_styles = '@section Styles {\n' + '\n'.join(styles_lines) + '\n}\n'

    # ── @section Scripts ─────────────────────────────────────
    scripts_parts = []
    if url_map:
        scripts_parts.append(generar_bloque_urls(url_map, indent='  '))
    if tiene_js:
        scripts_parts.append(f'  <script src="~/js/pages/{js_slug}.js"></script>')

    section_scripts = ''
    if scripts_parts:
        section_scripts = '\n@section Scripts {\n' + '\n'.join(scripts_parts) + '\n}\n'

    return '\n'.join(filter(None, [header, section_styles, body_html.strip(), section_scripts]))


# ══════════════════════════════════════════════════════════════
#  LIMPIEZA DE HTML
# ══════════════════════════════════════════════════════════════

_TOPBAR_SELECTORS = [
    {'class': 'topbar'},
    {'id': 'topbar'},
    {'class': 'cal-page-header'},   # si se integra con layout completo
    {'id': 'header'},
]


def limpiar_body(soup: BeautifulSoup) -> str:
    """
    Extrae el contenido relevante del <body>:
    - Elimina el topbar
    - Elimina overlays de tooltip/toast que se regeneran en el template
    - Devuelve el HTML limpio como string
    """
    body = soup.find('body')
    if not body:
        body = soup

    # Eliminar topbar
    for sel in _TOPBAR_SELECTORS:
        el = body.find(attrs=sel)
        if el:
            el.decompose()

    # Eliminar <script src="...bootstrap..."> (ya cargado en _Layout)
    for tag in body.find_all('script', src=True):
        src = tag.get('src', '')
        if 'bootstrap' in src.lower() or 'cdn.jsdelivr.net' in src.lower():
            tag.decompose()

    # Retornar innerHTML
    return body.decode_contents().strip()


# ══════════════════════════════════════════════════════════════
#  PIPELINE PRINCIPAL
# ══════════════════════════════════════════════════════════════

def convertir(
    html_path: Path,
    controller: str,
    action: str,
    pantalla: str,
    views_dir: Path,
    wwwroot_dir: Path,
    slug: str,
) -> None:
    sep  = '=' * 60
    sep2 = '-' * 60
    print(f'\n{sep}')
    print(f'  html_to_razor.py')
    print(sep)
    print(f'  Entrada   : {html_path}')
    print(f'  Controller: {controller}  |  Action: {action}')
    print(f'  Pantalla  : {pantalla}')
    print(f'{sep2}\n')

    html_raw = html_path.read_text(encoding='utf-8')
    soup = BeautifulSoup(html_raw, 'html.parser')

    # ── 1. Extraer <style> ───────────────────���────────────────
    style_blocks = soup.find_all('style')
    css_content = '\n\n'.join(tag.string or '' for tag in style_blocks if tag.string)
    for tag in style_blocks:
        tag.decompose()

    # ── 2. Extraer <script> inline ───────────────────────────
    script_blocks = soup.find_all('script', src=False)
    js_content = '\n\n'.join(tag.string or '' for tag in script_blocks if tag.string)
    for tag in script_blocks:
        tag.decompose()

    # ── 3. Detectar URLs en el JS ─────────────────────────────
    url_map = detectar_urls_js(js_content)
    if url_map:
        js_content = reemplazar_urls_js(js_content, url_map)
        print('  URLs detectadas en JS:')
        for var, (c, a) in url_map.items():
            print(f'    window._urls.{var:30s} → @Url.Action("{a}", "{c}")')

    # ── 4. CDN links del <head> ───────────────────────────────
    cdn_links = []
    head = soup.find('head')
    if head:
        for tag in head.find_all('link', rel=lambda r: r and 'stylesheet' in r):
            href = tag.get('href', '')
            if href.startswith('http') or href.startswith('//'):
                cdn_links.append(str(tag))
                tag.decompose()

    # ── 5. Título de la página ────────────────────────────────
    title_tag = soup.find('title')
    titulo = title_tag.string.strip() if title_tag and title_tag.string else action

    # ── 6. Limpiar body ───────────────────────────────────────
    body_html = limpiar_body(soup)

    # ── 7. Determinar paths de salida ─────────────────────────
    css_out  = wwwroot_dir / 'css' / f'{slug}.css'
    js_out   = wwwroot_dir / 'js' / 'pages' / f'{slug}.js'
    view_out = views_dir / controller / f'{action}.cshtml'

    tiene_css = bool(css_content.strip())
    tiene_js  = bool(js_content.strip())

    # ── 8. Generar .cshtml ────────────────────────────────────
    cshtml = generar_cshtml(
        controller=controller,
        action=action,
        pantalla=pantalla,
        titulo=titulo,
        cdn_links=cdn_links,
        css_slug=slug,
        js_slug=slug,
        body_html=body_html,
        url_map=url_map,
        tiene_css=tiene_css,
        tiene_js=tiene_js,
    )

    # ── 9. Escribir archivos ──────────────────────────────────
    archivos_creados = []

    if tiene_css:
        css_out.parent.mkdir(parents=True, exist_ok=True)
        css_out.write_text(css_content, encoding='utf-8')
        archivos_creados.append(('CSS', css_out))

    if tiene_js:
        js_out.parent.mkdir(parents=True, exist_ok=True)
        js_out.write_text(js_content, encoding='utf-8')
        archivos_creados.append(('JS', js_out))

    view_out.parent.mkdir(parents=True, exist_ok=True)
    view_out.write_text(cshtml, encoding='utf-8')
    archivos_creados.append(('View', view_out))

    # ── 10. Resumen ───────────────────────────────────────────
    print('\n  Archivos generados:')
    for tipo, ruta in archivos_creados:
        size_kb = ruta.stat().st_size / 1024
        print(f'    [{tipo:5s}]  {ruta}  ({size_kb:.1f} KB)')

    print('\n  Proximos pasos manuales:')
    print('    1. Revisar el .cshtml generado y ajustar el contenido del body')
    if url_map:
        print('    2. Verificar que window._urls tenga las rutas correctas')
        print('    3. El JS ya usa window._urls.xxx -- no hardcodear rutas en el .js')
    print('    4. Crear el backend: SP -> Result -> ViewModel -> Repository -> Service -> Controller')
    print(f'    5. Asegurarse de que "{pantalla}" este asignada al rol del usuario')
    print(f'\n{sep}\n')


# ══════════════════════════════════════════════════════════════
#  MODO RAZOR: extrae CSS/JS de un .cshtml existente
# ══════════════════════════════════════════════════════════════

# Detecta líneas con sintaxis Razor (@ que no sea @@)
_RAZOR_LINE = re.compile(r'@(?!@)')


def extraer_de_razor(cshtml_path: Path, slug: str, wwwroot_dir: Path) -> None:
    """
    Extrae el CSS y JS inline de un .cshtml ya existente:

      <style>...</style>         → wwwroot/css/{slug}.css
                                   reemplaza con <link rel="stylesheet" .../>

      <script> sin src </script> → wwwroot/js/pages/{slug}.js
                                   Las líneas con @ (Razor) se conservan
                                   como bloque <script> inline.

    Modifica el .cshtml in-place.
    """
    sep  = '=' * 60
    sep2 = '-' * 60
    print(f'\n{sep}')
    print(f'  html_to_razor.py  [modo Razor]')
    print(sep)
    print(f'  Entrada : {cshtml_path}')
    print(f'  Slug    : {slug}')
    print(f'{sep2}\n')

    content = cshtml_path.read_text(encoding='utf-8')
    archivos = []
    changed  = False

    # ── 1. Extraer <style> ───────────────────────────────────
    style_m = re.search(r'<style[^>]*>(.*?)</style>', content, re.DOTALL)
    if style_m:
        css_content = style_m.group(1).strip()
        css_out = wwwroot_dir / 'css' / f'{slug}.css'
        css_out.parent.mkdir(parents=True, exist_ok=True)
        css_out.write_text(css_content, encoding='utf-8')
        archivos.append(('CSS', css_out))
        print(f'  [OK]  CSS  {css_out.relative_to(wwwroot_dir.parent)}  ({css_out.stat().st_size/1024:.1f} KB)')

        link = f'<link rel="stylesheet" href="~/css/{slug}.css" />'
        content = content[:style_m.start()] + link + content[style_m.end():]
        changed = True
    else:
        print('  [SKIP] No se encontro <style> inline')

    # ── 2. Extraer <script> inline de @section Scripts ──────
    # Trabajamos dentro del bloque @section Scripts para evitar
    # tocar otros <script src="..."> que ya existen en la vista.
    # Greedy .*</script> para capturar hasta el ULTIMO </script>
    # antes del cierre del section (evita que \n} matchee dentro del JS)
    sec_m = re.search(
        r'(@section\s+Scripts\s*\{)(.*</script>)(\s*\n\})',
        content, re.DOTALL
    )
    if sec_m:
        sec_body  = sec_m.group(2)
        script_m  = re.search(
            r'<script(?![^>]*\bsrc\b)[^>]*>(.*?)</script>',
            sec_body, re.DOTALL
        )
        if script_m:
            raw_js = script_m.group(1)
            lines  = raw_js.splitlines()

            razor_lines = [l for l in lines if _RAZOR_LINE.search(l)]
            js_lines    = [l for l in lines if not _RAZOR_LINE.search(l)]

            js_content = '\n'.join(js_lines).strip()
            js_out = wwwroot_dir / 'js' / 'pages' / f'{slug}.js'
            js_out.parent.mkdir(parents=True, exist_ok=True)
            js_out.write_text(js_content, encoding='utf-8')
            archivos.append(('JS', js_out))
            print(f'  [OK]  JS   {js_out.relative_to(wwwroot_dir.parent)}  ({js_out.stat().st_size/1024:.1f} KB)')

            if razor_lines:
                non_empty = [l for l in razor_lines if l.strip()]
                print(f'  [INFO] {len(non_empty)} linea(s) Razor conservadas como script inline:')
                for l in non_empty:
                    print(f'         {l.strip()}')
                url_block = '\n<script>\n' + '\n'.join(non_empty) + '\n</script>\n'
            else:
                url_block = ''

            js_ref = f'<script src="~/js/pages/{slug}.js"></script>'
            replacement = url_block + js_ref

            new_body    = sec_body[:script_m.start()] + replacement + sec_body[script_m.end():]
            new_section = sec_m.group(1) + new_body + sec_m.group(3)
            content = content[:sec_m.start()] + new_section + content[sec_m.end():]
            changed = True
        else:
            print('  [SKIP] No se encontro <script> inline en @section Scripts')
    else:
        print('  [SKIP] No se encontro @section Scripts en el archivo')

    # ── 3. Guardar .cshtml ───────────────────────────────────
    if changed:
        cshtml_path.write_text(content, encoding='utf-8')
        print(f'  [OK]  .cshtml actualizado: {cshtml_path.name}')

    print('\n  Archivos generados:')
    for tipo, ruta in archivos:
        print(f'    [{tipo:5s}]  {ruta}  ({ruta.stat().st_size/1024:.1f} KB)')
    print(f'\n{sep}\n')


# ══════════════════════════════════════════════════════════════
#  CLI
# ══════════════════════════════════════════════════════════════

def main():
    parser = argparse.ArgumentParser(
        description='Convierte HTML estático a Razor, o extrae CSS/JS de un .cshtml existente.',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=textwrap.dedent("""\
            Modo HTML (entrada .html):
              python html_to_razor.py Downloads/calendario-citas.html \\
                  --controller CitaMedica --action Calendario \\
                  --pantalla "Listado de citas medicas"

            Modo Razor (entrada .cshtml):
              python html_to_razor.py PetsHome.UI/Views/CitaMedica/Calendario.cshtml \\
                  --slug calendario-citas
        """)
    )
    parser.add_argument('html', help='Ruta al archivo .html o .cshtml')
    parser.add_argument('--controller', '-c', default=None,
                        help='Nombre del Controller MVC (default: derivado del nombre del archivo)')
    parser.add_argument('--action', '-a', default=None,
                        help='Nombre de la Action/View (default: derivado del nombre del archivo)')
    parser.add_argument('--pantalla', '-p', default='',
                        help='Nombre de la pantalla en el sistema de seguridad')
    parser.add_argument('--slug', default=None,
                        help='Nombre base para los archivos CSS/JS (default: nombre del archivo)')
    parser.add_argument('--views-dir', default='PetsHome.UI/Views',
                        help='Directorio raíz de vistas (default: PetsHome.UI/Views)')
    parser.add_argument('--wwwroot', default='PetsHome.UI/wwwroot',
                        help='Directorio wwwroot (default: PetsHome.UI/wwwroot)')

    args = parser.parse_args()

    html_path = Path(args.html)
    if not html_path.exists():
        print(f'ERROR: No se encontró el archivo: {html_path}')
        sys.exit(1)

    # Derivar nombres desde el archivo si no se especifican
    stem = html_path.stem                         # "calendario-citas"
    parts = stem.replace('-', ' ').title().split()  # ["Calendario", "Citas"]

    slug       = args.slug       or stem
    controller = args.controller or parts[0]
    action     = args.action     or (''.join(parts) if len(parts) > 1 else parts[0])

    # Resolver paths relativos al directorio del script
    base = Path(__file__).parent
    views_dir  = base / args.views_dir
    wwwroot_dir = base / args.wwwroot

    # ── Despacho según extensión ─────────────────────────────
    if html_path.suffix.lower() == '.cshtml':
        extraer_de_razor(
            cshtml_path=html_path,
            slug=slug,
            wwwroot_dir=wwwroot_dir,
        )
    else:
        convertir(
            html_path=html_path,
            controller=controller,
            action=action,
            pantalla=args.pantalla,
            views_dir=views_dir,
            wwwroot_dir=wwwroot_dir,
            slug=slug,
        )


if __name__ == '__main__':
    main()
