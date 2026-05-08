"""
tema_04.py — Layout profesional con banda superior limpia + grid de screenshots
Paleta: Azul/Púrpura (Petshome) + Fondo neutro
Diseño: Banda superior prominente + 2 screenshots en grid ordenado
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


def _rounded_rectangle(draw, xy, radius=15, fill=None, outline=None, width=1):
    """Dibuja rectángulo con esquinas redondeadas."""
    x0, y0, x1, y1 = xy
    points = [
        (x0+radius, y0), (x1-radius, y0),
        (x1, y0), (x1, y0+radius),
        (x1, y1-radius), (x1, y1),
        (x1-radius, y1), (x0+radius, y1),
        (x0, y1), (x0, y1-radius),
        (x0, y0+radius), (x0, y0)
    ]
    draw.polygon(points, fill=fill, outline=outline)
    # Esquinas redondeadas
    draw.arc((x0, y0, x0+radius*2, y0+radius*2), 180, 270, outline, width)
    draw.arc((x1-radius*2, y0, x1, y0+radius*2), 270, 360, outline, width)
    draw.arc((x1-radius*2, y1-radius*2, x1, y1), 0, 90, outline, width)
    draw.arc((x0, y1-radius*2, x0+radius*2, y1), 90, 180, outline, width)


def _rounded_paste(canvas, img, x, y, radius=15):
    """Pega imagen con esquinas redondeadas y sombra."""
    # Crear máscara redondeada
    mask = Image.new("L", img.size, 0)
    draw_mask = ImageDraw.Draw(mask)
    draw_mask.rounded_rectangle([0, 0, img.width-1, img.height-1], radius=radius, fill=255)
    
    # Crear sombra
    shadow = Image.new("RGBA", (canvas.width, canvas.height), (0, 0, 0, 0))
    draw_shadow = ImageDraw.Draw(shadow)
    draw_shadow.rounded_rectangle(
        [x+8, y+8, x+img.width+8, y+img.height+8],
        radius=radius,
        fill=(0, 0, 0, 35)
    )
    shadow = shadow.filter(ImageFilter.GaussianBlur(16))
    canvas = Image.alpha_composite(canvas.convert("RGBA"), shadow).convert("RGB")
    
    # Pegar imagen
    canvas.paste(img, (x, y), mask)
    return canvas


class Tema(BaseTheme):
    NOMBRE = "tema_04"

    def componer(self, screenshots, config, indice, output_path):
        W, H = 1400, 900
        
        # 🎨 PALETA PETSHOME - Azul/Púrpura profesional
        BG_MAIN      = (248, 249, 250)      # Fondo principal
        BAND_DARK    = (79, 70, 176)        # #4F46B0 - Púrpura oscuro
        BAND_LIGHT   = (99, 102, 241)       # #6366F1 - Azul índigo
        
        WHITE_TEXT   = (255, 255, 255)
        DARK_TEXT    = (17, 24, 39)         # #111827
        GRAY_TEXT    = (107, 114, 128)      # #6B7280
        LIGHT_GRAY   = (156, 163, 175)      # #9CA3AF
        
        canvas = Image.new("RGB", (W, H), BG_MAIN)
        draw = ImageDraw.Draw(canvas)

        # ═══════════════════════════════════════════════════════
        # BANDA SUPERIOR - Degradado limpio y profesional
        # ═══════════════════════════════════════════════════════
        BAND_HEIGHT = 240
        
        # Degradado vertical suave
        for i in range(BAND_HEIGHT):
            t = i / BAND_HEIGHT
            r = int(BAND_DARK[0] + (BAND_LIGHT[0] - BAND_DARK[0]) * t)
            g = int(BAND_DARK[1] + (BAND_LIGHT[1] - BAND_DARK[1]) * t)
            b = int(BAND_DARK[2] + (BAND_LIGHT[2] - BAND_DARK[2]) * t)
            draw.rectangle([0, i, W, i+1], fill=(r, g, b))

        # ═══════════════════════════════════════════════════════
        # CONTENIDO BANDA - Textos centrados y legibles
        # ═══════════════════════════════════════════════════════
        
        # Etiqueta pequeña
        f_tag = _font(13)
        etiqueta = config.get("etiqueta", "").upper()
        if etiqueta:
            draw.text((50, 35), etiqueta, font=f_tag, fill=WHITE_TEXT)

        # Título principal - Grande y legible
        nombre = config.get("proyecto", "Proyecto").upper()
        f_title = _font(100, bold=True)
        # Centrar horizontalmente el título
        bbox = draw.textbbox((0, 0), nombre, font=f_title)
        title_width = bbox[2] - bbox[0]
        title_x = max(50, (W - title_width) // 2)
        draw.text((title_x, 60), nombre, font=f_title, fill=WHITE_TEXT)

        # Subtítulo
        sub = config.get("subtitulo", "")
        f_sub = _font(20)
        if sub:
            # Envolver subtítulo si es muy largo
            wrapped_sub = textwrap.wrap(sub, 50)
            sub_y = BAND_HEIGHT - 70
            for line in wrapped_sub:
                draw.text((50, sub_y), line, font=f_sub, fill=WHITE_TEXT)
                sub_y += 28

        # ═══════════════════════════════════════════════════════
        # ÁREA DE SCREENSHOTS - Grid ordenado
        # ═══════════════════════════════════════════════════════
        
        SCREENSHOT_SPACING = 30
        SCREENSHOT_MARGIN = 50
        
        # Calcular dimensiones de screenshots
        # 2 screenshots: uno arriba a la izquierda, otro abajo a la derecha
        available_width = W - (SCREENSHOT_MARGIN * 2) - SCREENSHOT_SPACING
        available_height = H - BAND_HEIGHT - (SCREENSHOT_MARGIN * 2) - SCREENSHOT_SPACING
        
        # Cada screenshot ocupa una columna
        screenshot_width = (available_width - SCREENSHOT_SPACING) // 2
        screenshot_height = (available_height - SCREENSHOT_SPACING) // 2
        
        # Ajustar altura manteniendo aspecto 16:9 aproximado
        screenshot_height = int(screenshot_width * 0.62)
        
        # Posiciones en grid 2x2
        positions = [
            {"x": SCREENSHOT_MARGIN, "y": BAND_HEIGHT + SCREENSHOT_MARGIN, 
             "w": screenshot_width, "h": screenshot_height},  # Arriba izq
            {"x": SCREENSHOT_MARGIN + screenshot_width + SCREENSHOT_SPACING, 
             "y": BAND_HEIGHT + SCREENSHOT_MARGIN,
             "w": screenshot_width, "h": screenshot_height},  # Arriba der
            {"x": SCREENSHOT_MARGIN, 
             "y": BAND_HEIGHT + SCREENSHOT_MARGIN + screenshot_height + SCREENSHOT_SPACING,
             "w": screenshot_width, "h": screenshot_height},  # Abajo izq
            {"x": SCREENSHOT_MARGIN + screenshot_width + SCREENSHOT_SPACING,
             "y": BAND_HEIGHT + SCREENSHOT_MARGIN + screenshot_height + SCREENSHOT_SPACING,
             "w": screenshot_width, "h": screenshot_height},  # Abajo der
        ]

        n = len(screenshots)
        if n > 0:
            # Cargar screenshots según índice
            for shot_idx, pos in enumerate(positions[:min(n, 4)]):
                img_idx = (indice * 4 + shot_idx) % n
                
                try:
                    img = Image.open(screenshots[img_idx]).convert("RGB")
                    # Redimensionar manteniendo aspecto
                    img.thumbnail((pos["w"], pos["h"]), Image.LANCZOS)
                    
                    # Crear imagen con fondo si no coincide aspecto
                    img_final = Image.new("RGB", (pos["w"], pos["h"]), BG_MAIN)
                    offset = ((pos["w"] - img.width) // 2, (pos["h"] - img.height) // 2)
                    img_final.paste(img, offset)
                    
                    # Pegar con sombra y esquinas redondeadas
                    canvas = _rounded_paste(canvas, img_final, pos["x"], pos["y"], radius=12)
                    
                except Exception as e:
                    print(f"Error procesando screenshot {img_idx}: {e}")

        # ═══════════════════════════════════════════════════════
        # FOOTER - Metadata compacta
        # ═══════════════════════════════════════════════════════
        f_code = _font(11)
        footer_y = H - 28
        draw.text((50, footer_y), self.NOMBRE, font=f_code, fill=BAND_LIGHT)
        
        date_text = datetime.now().strftime("%d %b %Y")
        draw.text((50, footer_y + 18), date_text, font=f_code, fill=LIGHT_GRAY)

        # Decoración esquina inferior derecha (sutil)
        radius_circle = 80
        draw.ellipse(
            [W - radius_circle - 20, H - radius_circle - 20, W + 20, H + 20],
            outline=(BAND_LIGHT + (150,)),  # Azul transparente
            width=2
        )

        canvas.save(str(output_path), "PNG")
        return output_path