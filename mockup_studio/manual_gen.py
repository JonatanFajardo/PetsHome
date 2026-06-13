"""
ManualGen — Genera manuales paso a paso al estilo Scribe
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Lee una receta JSON con secciones y pasos, ejecuta cada paso con Playwright,
toma screenshots con un círculo naranja sobre el elemento clicado y genera
una página HTML autocontenida con sidebar de TOC y pasos numerados.

Uso:
  python manual_gen.py mascotas              usa manuales/mascotas.json
  python manual_gen.py mascotas --no-capturar  re-genera HTML sin re-capturar
"""

import json
import sys
import argparse
import html
from pathlib import Path
from datetime import datetime

_HERE = Path(__file__).parent.resolve()

if sys.stdout.encoding and sys.stdout.encoding.lower() != "utf-8":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
        sys.stderr.reconfigure(encoding="utf-8")
    except Exception:
        pass

from PIL import Image, ImageDraw, ImageFilter
from playwright.sync_api import sync_playwright


# ──────────────────────────────────────────────────────────
# CSS para ocultar chrome de la app durante el manual
# ──────────────────────────────────────────────────────────
HIDE_CHROME_CSS = """
    .header.navbar, .sub-header-container { display: none !important; }
    .main-content, #content {
        padding-top: 16px !important;
    }
"""

# Color y forma del highlight
HIGHLIGHT_COLOR   = (239, 68, 68, 235)    # rojo vibrante
HIGHLIGHT_BORDER  = 4
HIGHLIGHT_PADDING = 10
SHADOW_OFFSET     = 3                     # desplazamiento de la sombra (px)
SHADOW_BLUR       = 6                     # radio de blur
SHADOW_COLOR      = (0, 0, 0, 110)        # negro semi-transparente


# ──────────────────────────────────────────────────────────
# LOGIN
# ──────────────────────────────────────────────────────────
def _login(page, url_base, login_cfg):
    login_url = f"{url_base}{login_cfg['url']}"
    print(f"  🔑 Login en {login_url}")
    page.goto(login_url, wait_until="domcontentloaded", timeout=15000)
    page.fill(f"[name='{login_cfg['campo_usuario']}']", login_cfg["usuario"])
    page.fill(f"[name='{login_cfg['campo_password']}']", login_cfg["password"])
    page.click(login_cfg["boton_submit"])
    page.wait_for_load_state("domcontentloaded", timeout=15000)


# ──────────────────────────────────────────────────────────
# DIBUJAR RECTÁNGULO MORADO SOBRE BBOX
# ──────────────────────────────────────────────────────────
def dibujar_circulo(img_path, bbox):
    base = Image.open(img_path).convert("RGBA")

    x0 = bbox["x"]                   - HIGHLIGHT_PADDING
    y0 = bbox["y"]                   - HIGHLIGHT_PADDING
    x1 = bbox["x"] + bbox["width"]   + HIGHLIGHT_PADDING
    y1 = bbox["y"] + bbox["height"]  + HIGHLIGHT_PADDING

    # Sombra: dibujar rectángulo, blurearlo, desplazar
    shadow = Image.new("RGBA", base.size, (0, 0, 0, 0))
    sdraw = ImageDraw.Draw(shadow)
    sdraw.rectangle(
        [x0 + SHADOW_OFFSET, y0 + SHADOW_OFFSET,
         x1 + SHADOW_OFFSET, y1 + SHADOW_OFFSET],
        outline=SHADOW_COLOR, width=HIGHLIGHT_BORDER,
    )
    shadow = shadow.filter(ImageFilter.GaussianBlur(SHADOW_BLUR))

    # Rectángulo principal encima
    overlay = Image.new("RGBA", base.size, (0, 0, 0, 0))
    draw = ImageDraw.Draw(overlay)
    draw.rectangle([x0, y0, x1, y1], outline=HIGHLIGHT_COLOR, width=HIGHLIGHT_BORDER)

    combinado = Image.alpha_composite(base, shadow)
    combinado = Image.alpha_composite(combinado, overlay).convert("RGB")
    combinado.save(img_path, "PNG", optimize=True)


# ──────────────────────────────────────────────────────────
# EJECUTAR LA RECETA
# ──────────────────────────────────────────────────────────
def ejecutar_receta(receta, url_base, vp, espera_default, login_cfg, output_dir):
    output_dir.mkdir(parents=True, exist_ok=True)
    img_dir = output_dir / "img"
    img_dir.mkdir(exist_ok=True)

    pasos_render = []   # lista plana de pasos para el HTML
    paso_global  = 0

    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        ctx = browser.new_context(
            viewport={"width": vp["width"], "height": vp["height"]},
            device_scale_factor=1,
            ignore_https_errors=True,
        )
        page = ctx.new_page()

        if login_cfg:
            _login(page, url_base, login_cfg)

        url_actual = None  # para evitar re-navegar a la misma URL

        for sec_idx, sec in enumerate(receta["secciones"]):
            print(f"\n📂 [{sec_idx+1}] {sec['nombre']}")
            seccion_pasos = []

            for paso in sec["pasos"]:
                paso_global += 1
                desc = paso.get("descripcion", "")
                print(f"   ↳ {paso_global:02d}  {desc[:60]}")

                # 1. Navegar (solo si la URL cambió respecto al paso anterior)
                if "url" in paso and paso["url"] != url_actual:
                    try:
                        page.goto(f"{url_base}{paso['url']}",
                                  wait_until="domcontentloaded", timeout=15000)
                        url_actual = paso["url"]
                    except Exception as e:
                        print(f"        ⚠ navigate: {e}")

                # 1b. Forzar 'show' en submenús colapsados (sidebar dropdowns)
                for sel in (paso.get("expandir_menu") or []):
                    try:
                        page.evaluate(
                            "(s) => document.querySelectorAll(s).forEach(el => el.classList.add('show'))",
                            sel,
                        )
                    except Exception:
                        pass

                # 1c. Esperar a que un selector específico aparezca (DataTables, AJAX)
                if paso.get("esperar_selector"):
                    try:
                        page.wait_for_selector(paso["esperar_selector"],
                                               timeout=8000, state="visible")
                    except Exception as e:
                        print(f"        ⚠ esperar_selector: {str(e).splitlines()[0]}")

                # 2. Llenar campos (si hay) — timeout corto para fallar rápido
                for sel, val in (paso.get("fill") or {}).items():
                    try:
                        page.fill(sel, val, timeout=5000)
                    except Exception as e:
                        print(f"        ⚠ fill {sel}: {str(e).splitlines()[0]}")

                # 2b. Selects: int → index, str → label
                for sel, val in (paso.get("select") or {}).items():
                    try:
                        if isinstance(val, int):
                            page.locator(sel).select_option(index=val, timeout=5000)
                        else:
                            page.locator(sel).select_option(label=str(val), timeout=5000)
                    except Exception as e:
                        print(f"        ⚠ select {sel}: {str(e).splitlines()[0]}")

                # 3. Inyectar CSS de chrome oculto
                try:
                    page.add_style_tag(content=HIDE_CHROME_CSS)
                except Exception:
                    pass

                # 4. Scroll al target del clic para que sea visible en el viewport
                target_sel = paso.get("click")
                if target_sel:
                    try:
                        page.locator(f"{target_sel}:visible").first.scroll_into_view_if_needed(timeout=3000)
                    except Exception:
                        try:
                            page.locator(target_sel).first.scroll_into_view_if_needed(timeout=3000)
                        except Exception:
                            pass

                # 5. Esperar
                page.wait_for_timeout(paso.get("esperar", espera_default))

                # 6. Screenshot
                img_name = f"paso_{paso_global:02d}.png"
                img_path = img_dir / img_name
                page.screenshot(path=str(img_path), full_page=False)

                # 7. Dibujar círculo sobre el target del clic (si hay)
                if target_sel:
                    try:
                        # Preferir el primer match VISIBLE
                        loc = page.locator(f"{target_sel}:visible").first
                        if loc.count() == 0:
                            loc = page.locator(target_sel).first
                        bbox = loc.bounding_box(timeout=3000)
                        if bbox:
                            dibujar_circulo(img_path, bbox)
                    except Exception as e:
                        print(f"        ⚠ bbox {target_sel}: {e}")

                # 7. Hacer el clic (solo si la receta lo pide explícitamente con avanzar=true)
                if target_sel and paso.get("avanzar"):
                    try:
                        page.click(f"{target_sel}:visible", timeout=5000)
                        page.wait_for_load_state("networkidle", timeout=8000)
                    except Exception as e:
                        print(f"        ⚠ click {target_sel}: {e}")

                seccion_pasos.append({
                    "n": paso_global,
                    "descripcion": desc,
                    "img": f"img/{img_name}",
                })

            pasos_render.append({
                "nombre": sec["nombre"],
                "pasos":  seccion_pasos,
            })

        browser.close()

    return pasos_render


# ──────────────────────────────────────────────────────────
# RENDER HTML
# ──────────────────────────────────────────────────────────
HTML_TEMPLATE = """<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="UTF-8">
<title>{titulo}</title>
<style>
  :root {{
    --bg: #f1f4f8;
    --card: #ffffff;
    --text: #1f2937;
    --muted: #6b7280;
    --accent: #6c5ce7;
    --border: #e5e7eb;
    --shadow: 0 2px 8px rgba(15, 23, 42, .06);
  }}
  * {{ box-sizing: border-box; }}
  body {{ margin: 0; font-family: -apple-system, "Segoe UI", Roboto, sans-serif;
         background: var(--bg); color: var(--text); }}
  header.cover {{ text-align: center; padding: 56px 24px 24px; }}
  header.cover .icon {{ width: 96px; height: 96px; border-radius: 50%;
         background: linear-gradient(135deg,#a29bfe,#6c5ce7); margin: 0 auto 16px;
         display: flex; align-items: center; justify-content: center;
         color: white; font-size: 42px; box-shadow: var(--shadow); }}
  header.cover h1 {{ font-size: 32px; margin: 8px 0 4px; }}
  header.cover .meta {{ color: var(--muted); font-size: 14px; }}
  .layout {{ display: grid; grid-template-columns: 280px 1fr;
            gap: 32px; max-width: 1200px; margin: 24px auto 80px; padding: 0 24px; }}
  nav.toc {{ position: sticky; top: 24px; align-self: start;
            background: var(--card); border-radius: 12px; padding: 16px;
            box-shadow: var(--shadow); max-height: calc(100vh - 48px); overflow-y: auto; }}
  nav.toc h3 {{ font-size: 13px; text-transform: uppercase;
                color: var(--muted); margin: 4px 8px 12px; letter-spacing: .05em; }}
  nav.toc a {{ display: block; padding: 10px 12px; border-radius: 8px;
              color: var(--text); text-decoration: none; font-size: 14px; line-height: 1.4; }}
  nav.toc a:hover {{ background: #f3f4f6; }}
  nav.toc a.active {{ background: #ede9fe; color: var(--accent); font-weight: 600; }}
  main {{ min-width: 0; }}
  section.seccion {{ margin-bottom: 48px; }}
  section.seccion > h2 {{ text-align: center; font-size: 20px;
                          color: var(--text); margin: 24px 0 24px;
                          position: relative; }}
  section.seccion > h2::before, section.seccion > h2::after {{
      content: ""; display: inline-block; width: 80px; height: 1px;
      background: var(--border); vertical-align: middle; margin: 0 16px; }}
  .paso {{ background: var(--card); border-radius: 12px; padding: 20px 24px;
           margin-bottom: 20px; box-shadow: var(--shadow); }}
  .paso-header {{ display: flex; align-items: center; gap: 16px;
                  margin-bottom: 16px; }}
  .paso-num {{ width: 40px; height: 40px; border-radius: 50%;
               background: #ede9fe; color: var(--accent);
               display: flex; align-items: center; justify-content: center;
               font-weight: 700; font-size: 18px; flex-shrink: 0; }}
  .paso-desc {{ font-size: 16px; line-height: 1.5; }}
  .paso img {{ display: block; max-width: 100%; height: auto;
               border-radius: 8px; border: 1px solid var(--border); }}
</style>
</head>
<body>
  <header class="cover">
    <div class="icon">📘</div>
    <h1>{titulo}</h1>
    <div class="meta">{subtitulo} · {autor} · {fecha}</div>
  </header>

  <div class="layout">
    <nav class="toc">
      <h3>{toc_label}</h3>
      {toc}
    </nav>
    <main>
      {body}
    </main>
  </div>

<script>
  const links = document.querySelectorAll('nav.toc a');
  const sections = document.querySelectorAll('section.seccion');
  const obs = new IntersectionObserver((entries) => {{
    entries.forEach(e => {{
      if (e.isIntersecting) {{
        links.forEach(l => l.classList.toggle('active',
          l.getAttribute('href') === '#' + e.target.id));
      }}
    }});
  }}, {{ rootMargin: '-30% 0px -60% 0px' }});
  sections.forEach(s => obs.observe(s));
</script>
</body>
</html>
"""


def slug(s):
    import re
    return re.sub(r"[^a-z0-9]+", "-", s.lower()).strip("-")


def render_html(receta, pasos_render, output_path):
    toc_items = []
    body_items = []

    for sec in pasos_render:
        sec_id = slug(sec["nombre"])
        toc_items.append(f'<a href="#{sec_id}">{html.escape(sec["nombre"])}</a>')

        pasos_html = []
        for p in sec["pasos"]:
            pasos_html.append(f"""
              <div class="paso">
                <div class="paso-header">
                  <div class="paso-num">{p['n']}</div>
                  <div class="paso-desc">{html.escape(p['descripcion'])}</div>
                </div>
                <img src="{p['img']}" alt="Paso {p['n']}">
              </div>
            """)

        body_items.append(f"""
          <section class="seccion" id="{sec_id}">
            <h2>{html.escape(sec['nombre'])}</h2>
            {''.join(pasos_html)}
          </section>
        """)

    final = HTML_TEMPLATE.format(
        titulo    = html.escape(receta.get("titulo", "Manual")),
        subtitulo = html.escape(receta.get("subtitulo", "")),
        autor     = html.escape(receta.get("autor", "")),
        fecha     = datetime.now().strftime("%d/%m/%Y"),
        toc_label = f"{len(pasos_render)} secciones",
        toc       = "\n".join(toc_items),
        body      = "\n".join(body_items),
    )

    output_path.write_text(final, encoding="utf-8")


# ──────────────────────────────────────────────────────────
# MAIN
# ──────────────────────────────────────────────────────────
def main():
    parser = argparse.ArgumentParser(description="ManualGen — manuales paso a paso")
    parser.add_argument("nombre", help="Nombre de la receta (sin .json) en manuales/")
    parser.add_argument("--config", default="config.json")
    parser.add_argument("--no-capturar", action="store_true",
                        help="Solo regenera el HTML usando capturas y datos previos")
    args = parser.parse_args()

    # Receta
    receta_path = _HERE / "manuales" / f"{args.nombre}.json"
    if not receta_path.exists():
        print(f"❌ No se encontró {receta_path}")
        sys.exit(1)
    with open(receta_path, "r", encoding="utf-8") as f:
        receta = json.load(f)

    # Config (login + url_base)
    config_path = _HERE / args.config
    with open(config_path, "r", encoding="utf-8") as f:
        config = json.load(f)

    url_base  = config["url_base"].rstrip("/")
    vp        = config.get("viewport", {"width": 1280, "height": 800})
    espera    = config.get("espera_ms", 1500)
    login_cfg = config.get("login")

    output_dir = _HERE / "output" / "manuales" / args.nombre
    output_dir.mkdir(parents=True, exist_ok=True)
    cache_path = output_dir / "_pasos.json"

    if args.no_capturar and cache_path.exists():
        with open(cache_path, "r", encoding="utf-8") as f:
            pasos_render = json.load(f)
        print(f"  ↺ Reutilizando capturas previas")
    else:
        pasos_render = ejecutar_receta(
            receta, url_base, vp, espera, login_cfg, output_dir,
        )
        with open(cache_path, "w", encoding="utf-8") as f:
            json.dump(pasos_render, f, indent=2, ensure_ascii=False)

    html_path = output_dir / "index.html"
    render_html(receta, pasos_render, html_path)
    print(f"\n🎉 Manual generado: {html_path}")


if __name__ == "__main__":
    main()
