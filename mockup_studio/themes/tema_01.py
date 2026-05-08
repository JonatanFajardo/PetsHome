"""
tema_01.py — Franja lateral bold + ventanas flotantes escalonadas
Paleta: azul marino oscuro + acento cyan
"""

import textwrap
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
    import os
    for p in candidates:
        if os.path.exists(p):
            try:
                return ImageFont.truetype(p, size)
            except Exception:
                pass
    return ImageFont.load_default()


class Tema(BaseTheme):
    NOMBRE = "tema_01"

    def componer(self, screenshots, config, indice, output_path):
        W, H = 1400, 900
        NAVY   = (15, 23, 42)
        STRIPE = (22, 35, 65)
        CYAN   = (6, 182, 212)
        WHITE  = (255, 255, 255)
        GRAY   = (148, 163, 184)

        canvas = Image.new("RGB", (W, H), NAVY)
        draw = ImageDraw.Draw(canvas)

        # Franja lateral izquierda
        draw.rectangle([0, 0, 340, H], fill=STRIPE)

        # Línea acento vertical
        draw.rectangle([340, 0, 346, H], fill=CYAN)

        # Puntos decorativos
        for i in range(6):
            draw.ellipse([22 + i*52, H-50, 38 + i*52, H-34], fill=CYAN if i == 0 else STRIPE)

        # Etiqueta
        etiqueta = config.get("etiqueta", "")
        if etiqueta:
            f = _font(17)
            draw.text((36, 80), etiqueta.upper(), font=f, fill=CYAN)

        # Nombre
        nombre = config.get("proyecto", "Proyecto").upper()
        f_big = _font(70, bold=True)
        # Dividir en líneas de ~8 chars
        words = nombre.split()
        lines = []
        line = ""
        for w in words:
            if len(line) + len(w) > 9:
                if line:
                    lines.append(line.strip())
                line = w + " "
            else:
                line += w + " "
        if line:
            lines.append(line.strip())

        y = 140
        for l in lines:
            draw.text((36, y), l, font=f_big, fill=WHITE)
            y += 80

        # Subtítulo
        sub = config.get("subtitulo", "")
        if sub:
            f_sub = _font(22)
            for l in textwrap.wrap(sub, 20):
                draw.text((36, y + 20), l, font=f_sub, fill=GRAY)
                y += 32

        # Código del tema
        f_code = _font(14)
        draw.text((36, H - 80), f"// {self.NOMBRE}", font=f_code, fill=CYAN)
        draw.text((36, H - 58), datetime.now().strftime("%d/%m/%Y"), font=f_code, fill=GRAY)

        # Screenshots escalonados
        n = len(screenshots)
        if n == 0:
            canvas.save(str(output_path), "PNG")
            return output_path

        positions = [
            {"x": 390, "y": 40,  "w": 700, "h": 437},
            {"x": 630, "y": 390, "w": 700, "h": 437},
        ]

        idx_a = (indice * 2) % n
        idx_b = (indice * 2 + 1) % n
        shots = [screenshots[idx_a], screenshots[min(idx_b, n-1)]]

        for shot_path, pos in zip(shots, positions):
            try:
                img = Image.open(shot_path).convert("RGB")
                img = img.resize((pos["w"], pos["h"]), Image.LANCZOS)

                # Sombra cyan sutil
                sh = Image.new("RGBA", (W, H), (0, 0, 0, 0))
                ImageDraw.Draw(sh).rectangle(
                    [pos["x"]+8, pos["y"]+8, pos["x"]+pos["w"]+8, pos["y"]+pos["h"]+8],
                    fill=(6, 182, 212, 40)
                )
                sh = sh.filter(ImageFilter.GaussianBlur(20))
                canvas_rgba = canvas.convert("RGBA")
                canvas_rgba = Image.alpha_composite(canvas_rgba, sh)
                canvas = canvas_rgba.convert("RGB")

                # Borde
                frame = Image.new("RGB", (pos["w"]+4, pos["h"]+4), (30, 45, 80))
                frame.paste(img, (2, 2))
                canvas.paste(frame, (pos["x"]-2, pos["y"]-2))
            except Exception:
                pass

        canvas.save(str(output_path), "PNG")
        return output_path
