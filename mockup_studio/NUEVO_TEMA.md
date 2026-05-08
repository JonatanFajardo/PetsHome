# Prompt para crear un nuevo tema HTML

Copia y pega esto en la IA de tu elección. Rellena las secciones marcadas con `[ ]`.

---

## PROMPT

Necesito que me generes dos archivos para un generador de mockups automático:
- `tema_html_XX.html` — el diseño en HTML/CSS puro
- `tema_html_XX.py`  — la lógica en Python que lee el HTML e inyecta los datos

---

### Contexto del sistema

El generador (`mockup_gen.py`) captura screenshots de un proyecto web con Playwright,
luego le pasa esas imágenes al tema para que las componga en un mockup de presentación.

El método `componer()` del tema recibe:
- `screenshots` — lista de rutas a archivos `.png` (capturas reales de la app)
- `config` — dict con los datos del proyecto (ver keys abajo)
- `indice` — número de mockup (0, 1, 2…) para variar qué screenshots se muestran
- `output_path` — Path donde guardar el PNG final

El canvas de salida es siempre **1400 × 900 px**.

---

### Keys del config disponibles

```python
config["proyecto"]   # Nombre del proyecto, ej: "PetsHome"
config["subtitulo"]  # Descripción corta, ej: "Sistema de gestión de refugio"
config["etiqueta"]   # Categoría, ej: "administrativo"
```

---

### Placeholders del HTML

El `.py` reemplaza estos textos en el HTML antes de tomar el screenshot:

| Placeholder       | Valor inyectado                        |
|-------------------|----------------------------------------|
| `{{PROYECTO}}`    | Nombre del proyecto en MAYÚSCULAS      |
| `{{SUBTITULO}}`   | Descripción del proyecto               |
| `{{ETIQUETA}}`    | Etiqueta/categoría                     |
| `{{TEMA_NOMBRE}}` | Nombre del tema, ej: `tema_html_02`    |
| `{{FECHA}}`       | Fecha actual, ej: `20 Apr 2026`        |
| `{{IMG_1}}`       | `data:image/png;base64,...` (screenshot 1) |
| `{{IMG_2}}`       | `data:image/png;base64,...` (screenshot 2) |
| `{{IMG_3}}`       | `data:image/png;base64,...` (screenshot 3) |

> Las imágenes van embebidas en base64 directamente en el `src` del `<img>`.

---

### Archivo `.py` de referencia (cópialo tal cual, solo cambia el nombre)

```python
"""
tema_html_XX.py — Lee tema_html_XX.html, inyecta datos y toma screenshot con Playwright.
Para editar el diseño: abre tema_html_XX.html directamente en VS Code o el browser.
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
    NOMBRE = "tema_html_XX"

    def componer(self, screenshots, config, indice, output_path: Path):
        from playwright.sync_api import sync_playwright

        n = len(screenshots)
        if n == 0:
            from PIL import Image
            Image.new("RGB", (1400, 900), (244, 243, 251)).save(str(output_path))
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
```

> El `.py` es siempre el mismo. Solo necesitas crear el `.html`.

---

### Lo que necesito que generes

**Nombre del tema:** `tema_html_[XX]`

**Estilo visual que quiero:**
[ Describe el diseño: colores, tipografía, layout, estilo general.
  Ejemplo: "fondo oscuro azul marino, título grande en blanco, tres screenshots
  apilados con rotación leve, estilo glassmorphism, acento en cyan" ]

**Proyecto al que va dirigido:**
[ Nombre y tipo de proyecto. Ejemplo: "PetsHome, sistema de gestión de refugio de animales" ]

**Paleta de colores:**
[ Ejemplo: primario #1e293b, acento #06b6d4, fondo #0f172a ]

---

### Reglas para el HTML

1. Canvas fijo: `width: 1400px; height: 900px; overflow: hidden` en el `body`
2. No usar Google Fonts con `@import url(...)` — usar fuentes del sistema:
   `font-family: 'Segoe UI', system-ui, sans-serif`
3. Las imágenes van en `<img src="{{IMG_1}}" />` — no uses `background-image`
4. Los placeholders `{{PROYECTO}}`, `{{IMG_1}}`, etc. deben aparecer exactamente así en el HTML
5. No incluir JavaScript
6. El resultado debe verse bien como imagen estática de 1400×900

---

### Entregables esperados

1. `tema_html_XX.html` — archivo HTML/CSS completo y listo para usar
2. `tema_html_XX.py`   — el `.py` de arriba con `XX` reemplazado por el número del tema

---
