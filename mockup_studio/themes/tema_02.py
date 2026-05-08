"""
tema_02.py — Minimal blanco, grid sutil, sombras limpias
Paleta: blanco puro + negro suave + acento violeta
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
    NOMBRE = "tema_02"

    def componer(self, screenshots, config, indice, output_path):
        W, H = 1400, 900
        WHITE   = (252, 252, 252)
        NEAR_BK = (20, 20, 30)
        VIOLET  = (124, 58, 237)
        LGRAY   = (226, 232, 240)
        MGRAY   = (148, 163, 184)

        canvas = Image.new("RGB", (W, H), WHITE)
        draw = ImageDraw.Draw(canvas)

        # Grid puntillado de fondo
        for x in range(0, W, 32):
            for y in range(0, H, 32):
                draw.ellipse([x, y, x+2, y+2], fill=LGRAY)

        # Borde superior grueso
        draw.rectangle([0, 0, W, 5], fill=VIOLET)

        # Línea divisora vertical sutil
        draw.rectangle([420, 60, 422, H-60], fill=LGRAY)

        # Etiqueta
        f_tag = _font(15)
        etiqueta = config.get("etiqueta", "proyecto")
        draw.text((50, 70), f"[ {etiqueta.upper()} ]", font=f_tag, fill=VIOLET)

        # Nombre
        nombre = config.get("proyecto", "Proyecto")
        f_big = _font(78, bold=True)
        words = nombre.upper().split()
        y = 110
        for w in words:
            draw.text((50, y), w, font=f_big, fill=NEAR_BK)
            y += 88

        # Subtítulo
        sub = config.get("subtitulo", "")
        f_sub = _font(20)
        for l in textwrap.wrap(sub, 26):
            draw.text((50, y + 14), l, font=f_sub, fill=MGRAY)
            y += 28

        # Código + fecha
        f_code = _font(13)
        draw.text((50, H - 55), self.NOMBRE, font=f_code, fill=VIOLET)
        draw.text((50, H - 36), datetime.now().strftime("%d %b %Y"), font=f_code, fill=MGRAY)

        # Círculo decorativo esquina
        draw.ellipse([W-180, H-180, W+100, H+100], outline=LGRAY, width=3)
        draw.ellipse([W-120, H-120, W+40, H+40], outline=LGRAY, width=2)

        # Screenshots
        n = len(screenshots)
        if n == 0:
            canvas.save(str(output_path), "PNG")
            return output_path

        positions = [
            {"x": 460, "y": 50,  "w": 560, "h": 350},
            {"x": 820, "y": 440, "w": 540, "h": 338},
        ]

        idx_a = (indice * 2) % n
        idx_b = (indice * 2 + 1) % n
        shots = [screenshots[idx_a], screenshots[min(idx_b, n-1)]]

        for shot_path, pos in zip(shots, positions):
            try:
                img = Image.open(shot_path).convert("RGB")
                img = img.resize((pos["w"], pos["h"]), Image.LANCZOS)

                # Sombra suave
                sh = Image.new("RGBA", (W, H), (0, 0, 0, 0))
                ImageDraw.Draw(sh).rectangle(
                    [pos["x"]+12, pos["y"]+12,
                     pos["x"]+pos["w"]+12, pos["y"]+pos["h"]+12],
                    fill=(0, 0, 0, 55)
                )
                sh = sh.filter(ImageFilter.GaussianBlur(22))
                canvas = Image.alpha_composite(canvas.convert("RGBA"), sh).convert("RGB")

                frame = Image.new("RGB", (pos["w"]+2, pos["h"]+2), LGRAY)
                frame.paste(img, (1, 1))
                canvas.paste(frame, (pos["x"]-1, pos["y"]-1))
            except Exception:
                pass

        canvas.save(str(output_path), "PNG")
        return output_path
