"""Diagnostico: imprime anchos del DOM tras inyectar CSS."""
import json, sys
from pathlib import Path
from playwright.sync_api import sync_playwright

if sys.stdout.encoding and sys.stdout.encoding.lower() != "utf-8":
    sys.stdout.reconfigure(encoding="utf-8")

HERE   = Path(__file__).parent
CFG    = json.loads((HERE / "config.json").read_text(encoding="utf-8"))
URL    = CFG["url_base"].rstrip("/") + "/Mascota/Index"
LOGIN  = CFG["login"]
VP     = CFG["viewport"]

CSS = """
    #sidebar, .sidebar-wrapper, .overlay,
    .header.navbar, .sub-header-container,
    .navbar.expand-md.navbar-expand-md { display: none !important; }
    .main-container, #container { padding-left:0!important; margin-left:0!important; }
    .main-content, #content {
        margin-left:0!important; padding-left:24px!important; padding-right:24px!important;
        width:100%!important; max-width:100%!important; flex: 1 1 100% !important;
    }
"""

with sync_playwright() as p:
    b = p.chromium.launch(headless=True)
    ctx = b.new_context(viewport=VP, ignore_https_errors=True)
    pg = ctx.new_page()

    # login
    pg.goto(CFG["url_base"].rstrip("/") + LOGIN["url"], wait_until="networkidle")
    pg.fill(f"[name='{LOGIN['campo_usuario']}']", LOGIN["usuario"])
    pg.fill(f"[name='{LOGIN['campo_password']}']", LOGIN["password"])
    pg.click(LOGIN["boton_submit"])
    pg.wait_for_load_state("networkidle")

    pg.goto(URL, wait_until="networkidle")

    def medir(rotulo):
        data = pg.evaluate("""
            () => {
                const sel = ['#sidebar', '.sidebar-wrapper', '#content', '.main-container',
                             '.pets-container', '#datatable', '#datatable_wrapper',
                             '.dataTables_wrapper', 'table.dataTable'];
                const out = {};
                for (const s of sel) {
                    const el = document.querySelector(s);
                    if (!el) { out[s] = null; continue; }
                    const r = el.getBoundingClientRect();
                    const cs = getComputedStyle(el);
                    out[s] = {w: Math.round(r.width), display: cs.display,
                              ml: cs.marginLeft, pl: cs.paddingLeft};
                }
                out._viewport = window.innerWidth;
                return out;
            }
        """)
        print(f"\n=== {rotulo} ===")
        for k, v in data.items():
            print(f"  {k}: {v}")

    medir("ANTES de inyectar CSS")
    pg.add_style_tag(content=CSS)
    pg.wait_for_timeout(500)
    medir("DESPUES de inyectar CSS")

    pg.set_viewport_size({"width": VP["width"]+1, "height": VP["height"]})
    pg.set_viewport_size(VP)
    pg.evaluate("""() => {
        if (window.jQuery && jQuery.fn.dataTable) {
            try { jQuery.fn.dataTable.tables({visible:true,api:true}).columns.adjust().draw(false); } catch(e){}
        }
    }""")
    pg.wait_for_timeout(1000)
    medir("DESPUES de resize + columns.adjust")

    pg.screenshot(path=str(HERE / "debug_mascota.png"))
    print("\nScreenshot: debug_mascota.png")
    b.close()
