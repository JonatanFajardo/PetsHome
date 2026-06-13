"""
ManualMaster — Combina varios manuales en un único HTML
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Toma todos los manuales generados en output/manuales/{slug}/ y compone un
único index.html con TOC anidado: Módulo → Sección → Pasos.

Uso:
  python manual_master.py                    todos los manuales existentes
  python manual_master.py mascotas empleado  solo los listados (en orden)
"""

import sys
import json
import html
import argparse
from pathlib import Path
from datetime import datetime

if sys.stdout.encoding and sys.stdout.encoding.lower() != "utf-8":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
        sys.stderr.reconfigure(encoding="utf-8")
    except Exception:
        pass

_HERE = Path(__file__).parent.resolve()
MANUALES_DIR = _HERE / "manuales"
OUT_DIR = _HERE / "output" / "manuales"


def slug(s):
    import re
    return re.sub(r"[^a-z0-9]+", "-", s.lower()).strip("-")


HTML_TEMPLATE = """<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="UTF-8">
<title>{titulo}</title>
<style>
  :root {{
    --bg: #f1f4f8; --card: #ffffff; --text: #1f2937; --muted: #6b7280;
    --accent: #6c5ce7; --border: #e5e7eb;
    --shadow: 0 2px 8px rgba(15, 23, 42, .06);
  }}
  * {{ box-sizing: border-box; }}
  body {{ margin: 0; font-family: -apple-system, "Segoe UI", Roboto, sans-serif;
         background: var(--bg); color: var(--text); }}
  header.cover {{ text-align: center; padding: 64px 24px 32px; }}
  header.cover .icon {{ width: 96px; height: 96px; border-radius: 50%;
         background: linear-gradient(135deg,#a29bfe,#6c5ce7); margin: 0 auto 16px;
         display: flex; align-items: center; justify-content: center;
         color: white; font-size: 42px; box-shadow: var(--shadow); }}
  header.cover h1 {{ font-size: 36px; margin: 8px 0 4px; }}
  header.cover .meta {{ color: var(--muted); font-size: 14px; }}

  .layout {{ display: grid; grid-template-columns: 320px 1fr;
            gap: 32px; max-width: 1280px; margin: 24px auto 80px; padding: 0 24px; }}
  nav.toc {{ position: sticky; top: 24px; align-self: start;
            background: var(--card); border-radius: 12px; padding: 16px;
            box-shadow: var(--shadow); max-height: calc(100vh - 48px); overflow-y: auto; }}
  nav.toc h3 {{ font-size: 13px; text-transform: uppercase;
                color: var(--muted); margin: 4px 8px 12px; letter-spacing: .05em; }}
  nav.toc .modulo {{ margin-bottom: 8px; }}
  nav.toc .modulo > a {{ display: block; padding: 10px 12px; border-radius: 8px;
              color: var(--text); text-decoration: none; font-size: 14px;
              font-weight: 600; line-height: 1.4; }}
  nav.toc .modulo > a:hover {{ background: #f3f4f6; }}
  nav.toc .modulo > a.active {{ background: #ede9fe; color: var(--accent); }}
  nav.toc ul.subsecciones {{ list-style: none; padding: 0 0 0 16px; margin: 0; }}
  nav.toc ul.subsecciones a {{ display: block; padding: 6px 12px; border-radius: 6px;
              color: var(--muted); text-decoration: none; font-size: 13px; }}
  nav.toc ul.subsecciones a:hover {{ background: #f3f4f6; color: var(--text); }}
  nav.toc ul.subsecciones a.active {{ color: var(--accent); font-weight: 600; }}

  main {{ min-width: 0; }}
  article.modulo {{ margin-bottom: 80px; }}
  article.modulo > h1 {{ font-size: 28px; padding-bottom: 12px;
                         border-bottom: 2px solid var(--accent); margin-bottom: 24px; }}
  article.modulo > .modulo-sub {{ color: var(--muted); margin-top: -16px;
                                  margin-bottom: 32px; }}
  section.seccion {{ margin-bottom: 48px; }}
  section.seccion > h2 {{ text-align: center; font-size: 18px;
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
    <div class="meta">{subtitulo} · {fecha}</div>
  </header>

  <div class="layout">
    <nav class="toc">
      <h3>{n_modulos} módulos</h3>
      {toc}
    </nav>
    <main>
      {body}
    </main>
  </div>

<script>
  const modLinks = document.querySelectorAll('nav.toc .modulo > a');
  const secLinks = document.querySelectorAll('nav.toc ul.subsecciones a');
  const modules  = document.querySelectorAll('article.modulo');
  const sections = document.querySelectorAll('section.seccion');

  const obsMod = new IntersectionObserver((entries) => {{
    entries.forEach(e => {{
      if (e.isIntersecting) {{
        modLinks.forEach(l => l.classList.toggle('active',
          l.getAttribute('href') === '#' + e.target.id));
      }}
    }});
  }}, {{ rootMargin: '-10% 0px -75% 0px' }});
  modules.forEach(m => obsMod.observe(m));

  const obsSec = new IntersectionObserver((entries) => {{
    entries.forEach(e => {{
      if (e.isIntersecting) {{
        secLinks.forEach(l => l.classList.toggle('active',
          l.getAttribute('href') === '#' + e.target.id));
      }}
    }});
  }}, {{ rootMargin: '-30% 0px -60% 0px' }});
  sections.forEach(s => obsSec.observe(s));
</script>
</body>
</html>
"""


def cargar_manual(slug_manual: str):
    """Lee la receta + las capturas cacheadas (_pasos.json) de un manual."""
    receta_path = MANUALES_DIR / f"{slug_manual}.json"
    cache_path  = OUT_DIR / slug_manual / "_pasos.json"
    if not receta_path.exists() or not cache_path.exists():
        return None
    with open(receta_path,  "r", encoding="utf-8") as f: receta = json.load(f)
    with open(cache_path,   "r", encoding="utf-8") as f: pasos  = json.load(f)
    return receta, pasos


def render(manuales: list, output_path: Path):
    toc_items = []
    body_items = []

    for slug_manual, receta, pasos in manuales:
        mod_id = slug(receta["titulo"])

        # TOC: módulo + subsecciones
        sub_links = []
        for sec in pasos:
            sec_id = f"{mod_id}--{slug(sec['nombre'])}"
            sub_links.append(f'<li><a href="#{sec_id}">{html.escape(sec["nombre"])}</a></li>')
        toc_items.append(f"""
          <div class="modulo">
            <a href="#{mod_id}">{html.escape(receta['titulo'])}</a>
            <ul class="subsecciones">{''.join(sub_links)}</ul>
          </div>
        """)

        # Body: módulo
        secciones_html = []
        for sec in pasos:
            sec_id = f"{mod_id}--{slug(sec['nombre'])}"
            pasos_html = []
            for p in sec["pasos"]:
                # Las imágenes están en output/manuales/{slug}/img/paso_NN.png
                img_src = f"{slug_manual}/{p['img']}"
                pasos_html.append(f"""
                  <div class="paso">
                    <div class="paso-header">
                      <div class="paso-num">{p['n']}</div>
                      <div class="paso-desc">{html.escape(p['descripcion'])}</div>
                    </div>
                    <img src="{img_src}" alt="Paso {p['n']}">
                  </div>
                """)
            secciones_html.append(f"""
              <section class="seccion" id="{sec_id}">
                <h2>{html.escape(sec['nombre'])}</h2>
                {''.join(pasos_html)}
              </section>
            """)

        body_items.append(f"""
          <article class="modulo" id="{mod_id}">
            <h1>{html.escape(receta['titulo'])}</h1>
            <p class="modulo-sub">{html.escape(receta.get('subtitulo', ''))}</p>
            {''.join(secciones_html)}
          </article>
        """)

    final = HTML_TEMPLATE.format(
        titulo    = "Manual de Usuario — PetsHome",
        subtitulo = f"{len(manuales)} módulos documentados",
        fecha     = datetime.now().strftime("%d/%m/%Y"),
        n_modulos = len(manuales),
        toc       = "\n".join(toc_items),
        body      = "\n".join(body_items),
    )
    output_path.write_text(final, encoding="utf-8")


def main():
    parser = argparse.ArgumentParser(description="Combina manuales en un solo HTML")
    parser.add_argument("slugs", nargs="*", help="slugs en orden (vacío = todos)")
    args = parser.parse_args()

    if args.slugs:
        slugs = args.slugs
    else:
        slugs = sorted(p.stem for p in MANUALES_DIR.glob("*.json"))

    manuales = []
    for s in slugs:
        data = cargar_manual(s)
        if data is None:
            print(f"  ⚠ Saltando '{s}' (falta receta o capturas)")
            continue
        manuales.append((s, *data))

    if not manuales:
        print("❌ No hay manuales para combinar.")
        sys.exit(1)

    output_path = OUT_DIR / "index.html"
    render(manuales, output_path)
    print(f"✅ Manual maestro generado: {output_path}")
    print(f"   {len(manuales)} módulos:")
    for s, r, _ in manuales:
        print(f"   • {r['titulo']}")


if __name__ == "__main__":
    main()
