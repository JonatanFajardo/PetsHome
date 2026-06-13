"""
GridsGen — Genera 3 grids 3x3 de pantallas para portfolio
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Captura 27 pantallas (9 listados + 9 formularios + 9 dashboards/reportes)
y compone 3 imágenes PNG con título narrativo, listas para README/portfolio.

Uso:
  python grids_gen.py                  usa config.json para login/url_base
  python grids_gen.py --solo grid1     captura/compone solo un grid
  python grids_gen.py --no-capturar    re-compone usando capturas existentes
"""

import json
import sys
import argparse
from pathlib import Path
from datetime import datetime

_HERE = Path(__file__).parent.resolve()

if sys.stdout.encoding and sys.stdout.encoding.lower() != "utf-8":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
        sys.stderr.reconfigure(encoding="utf-8")
    except Exception:
        pass

from PIL import Image, ImageDraw, ImageFont
from playwright.sync_api import sync_playwright


# ──────────────────────────────────────────────────────────
# DEFINICIÓN DE LOS 3 GRIDS
# ──────────────────────────────────────────────────────────

GRIDS = {
    "grid1_listados": {
        "titulo":    "Cobertura del dominio",
        "subtitulo": "9 listados que cubren todo el ciclo del albergue",
        "rutas": [
            "/Mascota/Index",
            "/CitaMedica/Index",
            "/ControlVacunacion/Index",
            "/Item/Index",
            "/Empleado/Index",
            "/Adopcion/Index",
            "/RecepcionMercancia/Index",
            "/Usuarios/Index",
            "/AlertaMedica/Index",
        ],
    },
    "grid2_formularios": {
        "titulo":    "Complejidad de formularios",
        "subtitulo": "Validación, dropdowns dependientes, detalles dinámicos y permisos",
        "rutas": [
            "/Mascota/Create",
            "/CitaMedica/Create",
            "/ControlVacunacion/Create",
            "/Item/Create",
            "/Empleado/Create",
            "/Adopcion/Create",
            "/RecepcionMercancia/Index",
            "/Roles/Index",
            "/Account/Login",
        ],
    },
    "grid3_dashboards": {
        "titulo":    "Insight y valor de negocio",
        "subtitulo": "Dashboards por rol, reportes y vistas agregadas",
        "rutas": [
            "/Home/Index",
            "/DashboardVeterinario/Index",
            "/DashboardCuidador/Index",
            "/ReporteAdopciones/Index",
            "/PerfilMedico/Index",
            "/HistorialMedico/Index",
            "/Evento/Index",
            "/Landing/Index",
            "/Solicitud/Index",
        ],
    },
}


# Layout del grid (PNG final)
CELL_W       = 640      # ancho de cada celda
CELL_H       = 400      # alto de cada celda
GAP          = 16       # espacio entre celdas
PAD          = 40       # padding alrededor
HEADER_H     = 140      # alto del encabezado (título + subtítulo)
BG_COLOR     = (24, 24, 32)
HEADER_COLOR = (255, 255, 255)
SUB_COLOR    = (180, 180, 195)
CELL_BORDER  = (60, 60, 75)


# CSS que se inyecta antes de cada screenshot — oculta sidebar/navbar y
# expande el contenido principal a ancho completo. Pensado para mockups.
HIDE_CHROME_CSS = """
    #sidebar, .sidebar-wrapper, .overlay,
    .header.navbar, .sub-header-container,
    .navbar.expand-md.navbar-expand-md {
        display: none !important;
    }
    .main-container, #container {
        padding-left: 0 !important;
        margin-left: 0 !important;
    }
    .main-content, #content {
        margin-left: 0 !important;
        padding-left: 24px !important;
        padding-right: 24px !important;
        padding-top: 24px !important;
        width: 100% !important;
        max-width: 100% !important;
        flex: 1 1 100% !important;
    }
    /* DataTables guarda el ancho inline al inicializar; forzamos 100% */
    table.dataTable, table.dataTable.no-footer,
    .pets-table, table#datatable {
        width: 100% !important;
    }
    body { overflow-x: hidden !important; }
"""


# ──────────────────────────────────────────────────────────
# LOGIN + CAPTURA
# ──────────────────────────────────────────────────────────

def _login(page, url_base, login_cfg):
    login_url = f"{url_base}{login_cfg['url']}"
    print(f"  🔑 Login en {login_url}")
    page.goto(login_url, wait_until="networkidle", timeout=15000)
    page.fill(f"[name='{login_cfg['campo_usuario']}']", login_cfg["usuario"])
    page.fill(f"[name='{login_cfg['campo_password']}']", login_cfg["password"])
    page.click(login_cfg["boton_submit"])
    page.wait_for_load_state("networkidle", timeout=15000)
    print(f"       ✓ Sesión iniciada")


def capturar_grid(url_base, rutas, output_dir, vp, espera, login_cfg, prefijo):
    output_dir.mkdir(parents=True, exist_ok=True)
    capturas = []
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

        for i, ruta in enumerate(rutas):
            url  = f"{url_base}{ruta}"
            dest = output_dir / f"{prefijo}_{i:02d}.png"
            try:
                print(f"  ↳ [{i+1}/{len(rutas)}] {url}")
                page.goto(url, wait_until="networkidle", timeout=15000)
                page.add_style_tag(content=HIDE_CHROME_CSS)
                # Resize real del viewport → DataTables y otros componentes
                # miden de nuevo. Cambiar y volver fuerza el ciclo.
                page.set_viewport_size({"width": vp["width"] + 1, "height": vp["height"]})
                page.set_viewport_size({"width": vp["width"],     "height": vp["height"]})
                page.evaluate("""
                    () => {
                        if (window.jQuery && jQuery.fn && jQuery.fn.dataTable) {
                            try {
                                const api = jQuery.fn.dataTable.tables({ visible: true, api: true });
                                api.columns.adjust().draw(false);
                                if (api.responsive) { try { api.responsive.recalc(); } catch(e){} }
                            } catch (e) {}
                        }
                    }
                """)
                page.wait_for_timeout(espera)
                page.screenshot(path=str(dest), full_page=False)
                capturas.append(dest)
                print(f"       ✓ {dest.name}")
            except Exception as e:
                print(f"       ✗ {e}")
                capturas.append(None)

        browser.close()
    return capturas


# ──────────────────────────────────────────────────────────
# COMPOSICIÓN DEL GRID 3x3
# ──────────────────────────────────────────────────────────

def _font(size, bold=False):
    candidates = [
        "C:/Windows/Fonts/segoeuib.ttf" if bold else "C:/Windows/Fonts/segoeui.ttf",
        "C:/Windows/Fonts/arialbd.ttf"  if bold else "C:/Windows/Fonts/arial.ttf",
    ]
    for c in candidates:
        if Path(c).exists():
            try:
                return ImageFont.truetype(c, size)
            except Exception:
                pass
    return ImageFont.load_default()


def componer_grid(capturas, titulo, subtitulo, output_path):
    cols, rows = 3, 3
    canvas_w = PAD * 2 + CELL_W * cols + GAP * (cols - 1)
    canvas_h = PAD * 2 + HEADER_H + CELL_H * rows + GAP * (rows - 1)

    canvas = Image.new("RGB", (canvas_w, canvas_h), BG_COLOR)
    draw   = ImageDraw.Draw(canvas)

    # Encabezado
    draw.text((PAD, PAD),       titulo,    fill=HEADER_COLOR, font=_font(42, bold=True))
    draw.text((PAD, PAD + 60),  subtitulo, fill=SUB_COLOR,    font=_font(22))

    # Celdas
    y0 = PAD + HEADER_H
    for idx in range(9):
        col = idx % cols
        row = idx // cols
        x   = PAD + col * (CELL_W + GAP)
        y   = y0  + row * (CELL_H + GAP)

        # Borde / placeholder
        draw.rectangle([x, y, x + CELL_W, y + CELL_H], fill=(40, 40, 52), outline=CELL_BORDER, width=1)

        cap = capturas[idx] if idx < len(capturas) else None
        if cap and Path(cap).exists():
            try:
                img = Image.open(cap).convert("RGB")
                # Encajar manteniendo aspect ratio
                img.thumbnail((CELL_W, CELL_H), Image.LANCZOS)
                ox = x + (CELL_W - img.width)  // 2
                oy = y + (CELL_H - img.height) // 2
                canvas.paste(img, (ox, oy))
            except Exception as e:
                draw.text((x + 10, y + 10), f"err: {e}", fill=(220, 80, 80), font=_font(14))
        else:
            draw.text((x + CELL_W // 2 - 30, y + CELL_H // 2 - 10),
                      "—", fill=SUB_COLOR, font=_font(28, bold=True))

    canvas.save(output_path, "PNG", optimize=True)
    print(f"  🖼  {output_path.name}  ({canvas_w}x{canvas_h})")


# ──────────────────────────────────────────────────────────
# MAIN
# ──────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(description="GridsGen — 3 grids 3x3 para portfolio")
    parser.add_argument("--config", default="config.json", help="config.json del proyecto")
    parser.add_argument("--solo", choices=list(GRIDS.keys()), help="Genera solo un grid")
    parser.add_argument("--no-capturar", action="store_true", help="Re-compone usando capturas existentes")
    args = parser.parse_args()

    config_path = _HERE / args.config
    if not config_path.exists():
        print(f"❌ No se encontró {config_path}")
        sys.exit(1)

    with open(config_path, "r", encoding="utf-8") as f:
        config = json.load(f)

    url_base  = config["url_base"].rstrip("/")
    vp        = config.get("viewport", {"width": 1280, "height": 800})
    espera    = config.get("espera_ms", 2000)
    login_cfg = config.get("login")

    out_root = config_path.parent / "output" / "grids"
    out_root.mkdir(parents=True, exist_ok=True)

    grids_a_correr = {args.solo: GRIDS[args.solo]} if args.solo else GRIDS

    for slug, spec in grids_a_correr.items():
        print(f"\n📸 {slug}: {spec['titulo']}")
        capturas_dir = out_root / slug
        capturas_dir.mkdir(parents=True, exist_ok=True)

        if args.no_capturar:
            capturas = sorted(capturas_dir.glob(f"{slug}_*.png"))
            print(f"  ↺ Reutilizando {len(capturas)} capturas existentes")
        else:
            capturas = capturar_grid(
                url_base, spec["rutas"], capturas_dir,
                vp, espera, login_cfg, prefijo=slug,
            )

        out_png = out_root / f"{slug}.png"
        componer_grid(capturas, spec["titulo"], spec["subtitulo"], out_png)

    print(f"\n🎉 Listo. Grids en: {out_root}")
    print(f"   {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")


if __name__ == "__main__":
    main()
