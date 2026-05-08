"""
tema_03.py — Split diagonal oscuro + ventana principal centrada grande
Paleta: negro carbón + verde esmeralda
"""

import textwrap, os
from pathlib import Path
from PIL import Image, ImageDraw, ImageFilter, ImageFont
from datetime import datetime
from themes.base_theme import BaseTheme


def _font(size, bold=False):
    candidates = (
        ["/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
         "/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf"]
        if bold else
        ["/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
         "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf"]
    )
    for p in candidates:
        if os.path.exists(p):
            try:
                return ImageFont.truetype(p, size)
            except Exception:
                pass
    return ImageFont.load_default()


class Tema(BaseTheme):
    NOMBRE = "tema_03"

    def componer(self, screenshots, config, indice, output_path):
        W, H = 1400, 900
        CARBON  = (13, 17, 23)
        DARK2   = (22, 27, 34)
        EMERALD = (16, 185, 129)
        WHITE   = (255, 255, 255)
        SUBGRAY = (125, 133, 144)

        canvas = Image.new("RGB", (W, H), CARBON)
        draw = ImageDraw.Draw(canvas)

        # Panel diagonal derecho
        draw.polygon([(W//2, 0), (W, 0), (W, H), (W//2 + 180, H)], fill=DARK2)

        # Línea diagonal acento
        draw.polygon([(W//2 - 4, 0), (W//2 + 4, 0),
                      (W//2 + 184, H), (W//2 + 176, H)], fill=EMERALD)

        # Líneas horizontales decorativas izquierda
        for i, y in enumerate(range(40, 200, 22)):
            alpha = max(30, 120 - i * 15)
            w = max(20, 280 - i * 30)
            draw.rectangle([50, y, 50 + w, y + 1], fill=(*EMERALD, alpha))

        # Etiqueta
        f_tag = _font(16)
        etiqueta = config.get("etiqueta", "")
        if etiqueta:
            draw.text((50, 220), f"> {etiqueta}", font=f_tag, fill=EMERALD)

        # Nombre
        nombre = config.get("proyecto", "Proyecto").upper()
        f_big = _font(80, bold=True)
        words = nombre.split()
        y = 260
        for w in words:
            draw.text((50, y), w, font=f_big, fill=WHITE)
            y += 88

        # Subtítulo
        sub = config.get("subtitulo", "")
        f_sub = _font(21)
        for l in textwrap.wrap(sub, 24):
            draw.text((50, y + 12), l, font=f_sub, fill=SUBGRAY)
            y += 30

        # Código + fecha
        f_code = _font(14)
        draw.text((50, H - 52), f"#{self.NOMBRE}", font=f_code, fill=EMERALD)
        draw.text((50, H - 32), datetime.now().strftime("%d/%m/%Y"), font=f_code, fill=SUBGRAY)

        # Screenshots
        n = len(screenshots)
        if n == 0:
            canvas.save(str(output_path), "PNG")
            return output_path

        positions = [
            {"x": 380,  "y": 60,  "w": 680, "h": 425},
            {"x": 700,  "y": 430, "w": 660, "h": 412},
        ]

        idx_a = (indice * 2) % n
        idx_b = (indice * 2 + 1) % n
        shots = [screenshots[idx_a], screenshots[min(idx_b, n-1)]]

        for shot_path, pos in zip(shots, positions):
            try:
                img = Image.open(shot_path).convert("RGB")
                img = img.resize((pos["w"], pos["h"]), Image.LANCZOS)

                sh = Image.new("RGBA", (W, H), (0, 0, 0, 0))
                ImageDraw.Draw(sh).rectangle(
                    [pos["x"]+10, pos["y"]+10,
                     pos["x"]+pos["w"]+10, pos["y"]+pos["h"]+10],
                    fill=(16, 185, 129, 35)
                )
                sh = sh.filter(ImageFilter.GaussianBlur(18))
                canvas = Image.alpha_composite(canvas.convert("RGBA"), sh).convert("RGB")

                frame = Image.new("RGB", (pos["w"]+2, pos["h"]+2), (30, 40, 30))
                frame.paste(img, (1, 1))
                canvas.paste(frame, (pos["x"]-1, pos["y"]-1))
            except Exception:
                pass

        canvas.save(str(output_path), "PNG")
        return output_path
