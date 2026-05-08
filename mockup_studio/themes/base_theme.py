"""
base_theme.py — Clase base para todos los temas de MockupGen
"""

from pathlib import Path


class BaseTheme:
    NOMBRE = "base"

    def componer(self, screenshots: list, config: dict, indice: int, output_path: Path):
        """
        Genera un mockup y lo guarda en output_path.
        Cada tema sobreescribe este método.
        """
        raise NotImplementedError(f"El tema '{self.NOMBRE}' debe implementar componer()")
