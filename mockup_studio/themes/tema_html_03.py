"""
tema_html_03.py — Lee tema_html_03.html, inyecta datos y toma screenshot con Playwright.
Para editar el diseño: abre tema_html_03.html directamente en VS Code o el browser.
"""

import base64
import tempfile
from pathlib import Path
from datetime import datetime
from themes.base_theme import BaseTheme

_TEMPLATE = Path(__file__).with_suffix(".html")


def _to_b64(path: Path) -> str:
    with open(path, "rb") as f:
        return f"data:image/png;base64,{base64.b64encode(f.read()).decode()}"


class Tema(BaseTheme):
    NOMBRE = "tema_html_03"

    def componer(self, screenshots, config, indice, output_path: Path):
        from playwright.sync_api import sync_playwright

        n = len(screenshots)
        if n == 0:
            from PIL import Image
            Image.new("RGB", (1400, 900), (26, 26, 26)).save(str(output_path))
            return output_path

        imgs = [_to_b64(screenshots[(indice * 2 + i) % n]) for i in range(3)]

        html = _TEMPLATE.read_text(encoding="utf-8")
        html = html.replace("{{PROYECTO}}",    config.get("proyecto",  "Proyecto").upper())
        html = html.replace("{{SUBTITULO}}",   config.get("subtitulo", ""))
        html = html.replace("{{ETIQUETA}}",    config.get("etiqueta",  "proyecto"))
        html = html.replace("{{TEMA_NOMBRE}}", self.NOMBRE)
        html = html.replace("{{FECHA}}",       datetime.now().strftime("%d %b %Y"))
        html = html.replace("{{IMG_1}}", imgs[0])
        html = html.replace("{{IMG_2}}", imgs[1])
        html = html.replace("{{IMG_3}}", imgs[2])

        tmp = Path(tempfile.mktemp(suffix=".html"))
        tmp.write_text(html, encoding="utf-8")

        try:
            with sync_playwright() as p:
                browser = p.chromium.launch(headless=True)
                page = browser.new_page(
                    viewport={"width": 1400, "height": 900},
                    ignore_https_errors=True,
                )
                page.goto(tmp.as_uri(), wait_until="networkidle", timeout=15000)
                page.wait_for_timeout(800)
                page.screenshot(path=str(output_path), full_page=False)
                browser.close()
        finally:
            tmp.unlink(missing_ok=True)

        return output_path