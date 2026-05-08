"""
tema_05.py — Cyberpunk: fondo morado profundo + grid neón + glitch offset
Paleta: violeta oscuro + magenta / rosa neón
"""

import textwrap, os, math
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
    NOMBRE = "tema_05"

    def componer(self, screenshots, config, indice, output_path):
        W, H = 1400, 900
        BG      = (18, 8, 38)
        PURPLE  = (88, 28, 135)
        MAGENTA = (236, 72, 153)
        PINK    = (249, 168, 212)
        WHITE   = (255, 255, 255)
        DGRAY   = (100, 80, 120)

        canvas = Image.new("RGB", (W, H), BG)
        draw = ImageDraw.Draw(canvas)

        # Grid perspectiva
        vanish_x, vanish_y = W // 2, H // 2
        for i in range(-12, 13):
            x_near = W // 2 + i * 70
            draw.line([(x_near, H), (vanish_x, vanish_y)],
                      fill=(*PURPLE, 80), width=1)
        for j in range(0, 8):
            t = j / 7
            y = int(vanish_y + (H - vanish_y) * (t ** 1.5))
            draw.line([(0, y), (W, y)], fill=(*PURPLE, 50), width=1)

        # Rectángulo acento izquierda
        draw.rectangle([0, 0, 6, H], fill=MAGENTA)

        # Glitch: copia offset del nombre
        nombre = config.get("proyecto", "Proyecto").upper()
        f_big = _font(80, bold=True)
        draw.text((58, 122), nombre, font=f_big, fill=(*MAGENTA, 60))  # sombra offset

        # Etiqueta
        f_tag = _font(17)
        etiqueta = config.get("etiqueta", "")
        if etiqueta:
            draw.text((56, 74), f"// {etiqueta}", font=f_tag, fill=MAGENTA)

        # Nombre principal
        draw.text((56, 118), nombre, font=f_big, fill=WHITE)

        # Subtítulo
        sub = config.get("subtitulo", "")
        f_sub = _font(21)
        y_s = 220
        for l in textwrap.wrap(sub, 25):
            draw.text((58, y_s), l, font=f_sub, fill=PINK)
            y_s += 30

        # Líneas decorativas cortas
        for i in range(5):
            draw.rectangle([56, y_s + 30 + i*14, 56 + 60 - i*10, y_s + 32 + i*14],
                           fill=MAGENTA)

        # Código + fecha
        f_code = _font(14)
        draw.text((56, H - 52), f"[ {self.NOMBRE} ]", font=f_code, fill=MAGENTA)
        draw.text((56, H - 32), datetime.now().strftime("%d/%m/%Y"), font=f_code, fill=DGRAY)

        # Screenshots
        n = len(screenshots)
        if n == 0:
            canvas.save(str(output_path), "PNG")
            return output_path

        positions = [
            {"x": 400, "y": 50,  "w": 660, "h": 412},
            {"x": 680, "y": 420, "w": 660, "h": 412},
        ]

        idx_a = (indice * 2) % n
        idx_b = (indice * 2 + 1) % n
        shots = [screenshots[idx_a], screenshots[min(idx_b, n-1)]]

        for shot_path, pos in zip(shots, positions):
            try:
                img = Image.open(shot_path).convert("RGB")
                img = img.resize((pos["w"], pos["h"]), Image.LANCZOS)

                # Glow magenta
                sh = Image.new("RGBA", (W, H), (0, 0, 0, 0))
                ImageDraw.Draw(sh).rectangle(
                    [pos["x"]-4, pos["y"]-4,
                     pos["x"]+pos["w"]+4, pos["y"]+pos["h"]+4],
                    fill=(*MAGENTA, 60)
                )
                sh = sh.filter(ImageFilter.GaussianBlur(22))
                canvas = Image.alpha_composite(canvas.convert("RGBA"), sh).convert("RGB")

                frame = Image.new("RGB", (pos["w"]+2, pos["h"]+2), MAGENTA)
                frame.paste(img, (1, 1))
                canvas.paste(frame, (pos["x"]-1, pos["y"]-1))
            except Exception:
                pass

        canvas.save(str(output_path), "PNG")
        return output_path
