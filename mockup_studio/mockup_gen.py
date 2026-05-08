"""
MockupGen — Generador automático de mockups para proyectos web
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Uso:
  python mockup_gen.py                     usa config.json, tema por defecto del config
  python mockup_gen.py mi_config.json      config personalizado
  python mockup_gen.py --nuevo-tema        asigna nuevo tema aleatorio
  python mockup_gen.py --tema tema_03      fuerza un tema específico
  python mockup_gen.py --listar-temas      muestra los temas disponibles
  python mockup_gen.py --portfolio         solo captura el portfolio (sin mockup)
"""

import json
import os
import sys
import random
import importlib
import argparse
from pathlib import Path
from datetime import datetime

# Asegurar que el directorio del script esté en el path (para importar themes/)
_HERE = Path(__file__).parent.resolve()
if str(_HERE) not in sys.path:
    sys.path.insert(0, str(_HERE))

# Fix encoding en Windows para soportar emojis en la consola
if sys.stdout.encoding and sys.stdout.encoding.lower() != "utf-8":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
        sys.stderr.reconfigure(encoding="utf-8")
    except Exception:
        pass

from reportlab.lib.pagesizes import landscape, A4
from reportlab.pdfgen import canvas as pdf_canvas
from playwright.sync_api import sync_playwright


# ──────────────────────────────────────────────────────────
# DESCUBRIMIENTO DE TEMAS
# ──────────────────────────────────────────────────────────

THEMES_DIR = _HERE / "themes"


def descubrir_temas() -> dict:
    temas = {}
    if not THEMES_DIR.exists():
        return temas
    for archivo in sorted(THEMES_DIR.glob("tema_*.py")):
        nombre = archivo.stem
        try:
            modulo    = importlib.import_module(f"themes.{nombre}")
            instancia = modulo.Tema()
            temas[nombre] = instancia
        except Exception as e:
            print(f"  ⚠ No se pudo cargar {nombre}: {e}")
    return temas


# ──────────────────────────────────────────────────────────
# LOCK FILE
# ──────────────────────────────────────────────────────────

def leer_lock(config_path: Path) -> dict:
    lock_path = config_path.parent / "mockup.lock"
    if lock_path.exists():
        try:
            with open(lock_path, "r", encoding="utf-8") as f:
                return json.load(f)
        except Exception:
            pass
    return {}


def escribir_lock(config_path: Path, tema: str):
    lock_path = config_path.parent / "mockup.lock"
    datos = {
        "tema": tema,
        "asignado_el": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
    }
    with open(lock_path, "w", encoding="utf-8") as f:
        json.dump(datos, f, indent=2, ensure_ascii=False)
    print(f"  🔒 Lock actualizado → {lock_path.name}  (tema: {tema})")


def resolver_tema(temas: dict, lock: dict, config: dict, forzar, nuevo: bool) -> str:
    """
    Prioridad:
      1. --tema X          → fuerza ese tema
      2. --nuevo-tema      → aleatorio distinto al actual
      3. lock existe       → usa el del lock
      4. config["tema"]    → tema por defecto del proyecto
      5. ninguno           → aleatorio y crea lock
    """
    nombres = list(temas.keys())
    if not nombres:
        raise RuntimeError("No hay temas en themes/")

    if forzar:
        if forzar not in temas:
            raise ValueError(f"Tema '{forzar}' no existe. Disponibles: {nombres}")
        return forzar

    if nuevo:
        actual     = lock.get("tema")
        candidatos = [n for n in nombres if n != actual] or nombres
        return random.choice(candidatos)

    if lock.get("tema") in temas:
        print(f"  🔒 Tema del lock: {lock['tema']}  (usa --nuevo-tema para cambiar)")
        return lock["tema"]

    # Tema por defecto definido en config.json
    tema_config = config.get("tema")
    if tema_config and tema_config in temas:
        print(f"  ⚙  Tema por defecto del config: {tema_config}")
        return tema_config

    elegido = random.choice(nombres)
    print(f"  🎲 Primera vez — tema asignado: {elegido}")
    return elegido


# ──────────────────────────────────────────────────────────
# LOGIN + CAPTURA (función reutilizable)
# ──────────────────────────────────────────────────────────

def _login(page, url_base: str, login_cfg: dict):
    login_url = f"{url_base}{login_cfg['url']}"
    print(f"  🔑 Iniciando sesión en {login_url} ...")
    try:
        page.goto(login_url, wait_until="networkidle", timeout=15000)
        page.fill(f"[name='{login_cfg['campo_usuario']}']", login_cfg["usuario"])
        page.fill(f"[name='{login_cfg['campo_password']}']", login_cfg["password"])
        page.click(login_cfg["boton_submit"])
        page.wait_for_load_state("networkidle", timeout=15000)
        print(f"       ✓ Sesión iniciada")
    except Exception as e:
        print(f"       ✗ Error en login: {e}")


def capturar_rutas(url_base: str, rutas: list, output_dir: Path,
                   vp: dict, espera: int, login_cfg: dict | None,
                   prefijo: str = "screen") -> list:
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
                page.wait_for_timeout(espera)
                page.screenshot(path=str(dest), full_page=False)
                capturas.append(dest)
                print(f"       ✓ {dest.name}")
            except Exception as e:
                print(f"       ✗ Error: {e}")

        browser.close()
    return capturas


# ──────────────────────────────────────────────────────────
# EXPORTAR PDF
# ──────────────────────────────────────────────────────────

def exportar_pdf(mockup_paths: list, output_path: Path, proyecto: str):
    page_w, page_h = landscape(A4)
    c      = pdf_canvas.Canvas(str(output_path), pagesize=landscape(A4))
    margin = 20

    for i, mp in enumerate(mockup_paths):
        if not mp.exists():
            continue
        c.drawImage(
            str(mp), margin, margin,
            width=page_w - margin * 2,
            height=page_h - margin * 2,
            preserveAspectRatio=True,
            anchor="c",
        )
        c.setFont("Helvetica", 8)
        c.setFillColorRGB(0.5, 0.5, 0.5)
        c.drawString(margin, 8,
            f"{proyecto} — Mockup {i+1}  |  {datetime.now().strftime('%d/%m/%Y %H:%M')}")
        if i < len(mockup_paths) - 1:
            c.showPage()

    c.save()
    print(f"  📄 PDF: {output_path.name}")


# ──────────────────────────────────────────────────────────
# MAIN
# ──────────────────────────────────────────────────────────

def main():
    parser = argparse.ArgumentParser(
        description="MockupGen — genera mockups automáticos de proyectos web"
    )
    parser.add_argument(
        "config", nargs="?", default="config.json",
        help="Ruta al config.json del proyecto (default: config.json)"
    )
    parser.add_argument("--tema",        metavar="NOMBRE", help="Fuerza un tema específico")
    parser.add_argument("--nuevo-tema",  action="store_true", help="Asigna un nuevo tema aleatorio")
    parser.add_argument("--listar-temas",action="store_true", help="Muestra los temas disponibles")
    parser.add_argument("--portfolio",   action="store_true", help="Solo captura el portfolio, sin generar mockup")
    args = parser.parse_args()

    # ── Cargar config ──────────────────────────────────────
    config_path = (_HERE / args.config) if not Path(args.config).is_absolute() else Path(args.config)
    if not config_path.exists():
        config_path = Path(args.config)
    if not config_path.exists():
        print(f"❌ No se encontró {args.config}")
        sys.exit(1)

    with open(config_path, "r", encoding="utf-8") as f:
        config = json.load(f)

    proyecto  = config.get("proyecto", "Proyecto")
    url_base  = config["url_base"].rstrip("/")
    vp        = config.get("viewport", {"width": 1280, "height": 800})
    espera    = config.get("espera_ms", 2000)
    login_cfg = config.get("login")

    # Carpetas fijas de salida
    mockups_dir   = config_path.parent / "output" / "mockups"
    portfolio_dir = config_path.parent / "output" / "portfolio"
    mockups_dir.mkdir(parents=True, exist_ok=True)
    portfolio_dir.mkdir(parents=True, exist_ok=True)

    # ── Captura de portfolio ───────────────────────────────
    rutas_portfolio = config.get("rutas_portfolio", [])
    if rutas_portfolio:
        print(f"\n🖼  Capturando portfolio ({len(rutas_portfolio)} pantallas)...")
        capturar_rutas(url_base, rutas_portfolio, portfolio_dir,
                       vp, espera, login_cfg, prefijo="portfolio")
        print(f"  📁 Portfolio en: output/portfolio/")

    if args.portfolio:
        print("\n✅ Portfolio listo.")
        return

    # ── Cargar temas ───────────────────────────────────────
    temas = descubrir_temas()

    if args.listar_temas:
        print(f"\n🎨 Temas disponibles ({len(temas)}):")
        for nombre in temas:
            print(f"   • {nombre}")
        return

    if not temas:
        print("❌ No se encontraron temas en themes/")
        sys.exit(1)

    # ── Resolver tema ──────────────────────────────────────
    lock = leer_lock(config_path)
    try:
        tema_elegido = resolver_tema(temas, lock, config, args.tema, args.nuevo_tema)
    except (ValueError, RuntimeError) as e:
        print(f"❌ {e}")
        sys.exit(1)

    escribir_lock(config_path, tema_elegido)
    tema = temas[tema_elegido]
    print(f"  🎨 Tema activo: {tema_elegido}")

    # ── Capturar rutas de mockup ───────────────────────────
    rutas = config.get("rutas", ["/"])
    print(f"\n📸 Capturando pantallas del mockup...")
    screenshots = capturar_rutas(url_base, rutas, mockups_dir,
                                 vp, espera, login_cfg, prefijo="screen")

    if not screenshots:
        print("\n⚠  Sin screenshots. ¿Está corriendo el proyecto?")
        from PIL import Image
        ph = mockups_dir / "screen_00.png"
        Image.new("RGB", (vp["width"], vp["height"]), (240, 240, 245)).save(str(ph))
        screenshots = [ph]

    # ── Componer mockups ───────────────────────────────────
    num_mockups = config.get("num_mockups", 2)
    hacer_pdf   = config.get("exportar_pdf", True)

    print(f"\n🎨 Componiendo {num_mockups} mockup(s) con [{tema_elegido}]...")
    mockup_paths = []
    for i in range(num_mockups):
        out = mockups_dir / f"mockup_{proyecto}_{i+1:02d}.png"
        tema.componer(screenshots, config, i, out)
        print(f"  ✅ {out.name}")
        mockup_paths.append(out)

    # ── PDF ────────────────────────────────────────────────
    if hacer_pdf and mockup_paths:
        print("\n📦 Exportando PDF...")
        exportar_pdf(
            mockup_paths,
            mockups_dir / f"mockups_{proyecto}.pdf",
            proyecto,
        )

    print(f"\n🎉 ¡Listo!")
    print(f"   📁 Mockups:   output/mockups/")
    print(f"   📁 Portfolio: output/portfolio/")


if __name__ == "__main__":
    main()
