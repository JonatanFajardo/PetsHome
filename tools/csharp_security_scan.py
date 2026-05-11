#!/usr/bin/env python3
"""
Scanner de vulnerabilidades para proyectos C# (.NET / ASP.NET)
Sin API, 100% local. Detecta CSRF, XSS, SQLi, secrets y más.
Uso: python csharp_security_scan.py ./mi-proyecto
"""

import re
import sys
import json
from pathlib import Path
from datetime import datetime
from dataclasses import dataclass, field, asdict

# ─── Estructura de hallazgo ────────────────────────────────────────────────────

@dataclass
class Hallazgo:
    tipo: str
    severidad: str          # CRITICA | ALTA | MEDIA | BAJA
    archivo: str
    linea: int
    codigo: str
    descripcion: str
    recomendacion: str

# ─── Reglas por categoría ──────────────────────────────────────────────────────

REGLAS = [

    # ── CSRF ────────────────────────────────────────────────────────────────────
    {
        "tipo": "CSRF",
        "severidad": "ALTA",
        "descripcion": "Acción POST sin [ValidateAntiForgeryToken]",
        "recomendacion": "Agrega [ValidateAntiForgeryToken] sobre el método y @Html.AntiForgeryToken() en el form",
        "patron": re.compile(
            r'\[HttpPost\](?![\s\S]{0,200}?\[ValidateAntiForgeryToken\])',
            re.MULTILINE
        ),
        "extensiones": {".cs"},
    },

    # ── XSS ─────────────────────────────────────────────────────────────────────
    {
        "tipo": "XSS",
        "severidad": "ALTA",
        "descripcion": "Html.Raw() puede inyectar HTML/JS sin sanitizar",
        "recomendacion": "Evita Html.Raw() con input del usuario. Usa Html.Encode() o @variable directamente en Razor",
        "patron": re.compile(r'Html\.Raw\s*\(', re.IGNORECASE),
        "extensiones": {".cshtml", ".cs"},
    },
    {
        "tipo": "XSS",
        "severidad": "ALTA",
        "descripcion": "Response.Write() sin encoding puede producir XSS",
        "recomendacion": "Usa HttpUtility.HtmlEncode() antes de escribir datos de usuario",
        "patron": re.compile(r'Response\.Write\s*\(', re.IGNORECASE),
        "extensiones": {".cs", ".aspx"},
    },
    {
        "tipo": "XSS",
        "severidad": "MEDIA",
        "descripcion": "InnerHtml asignado directamente puede ser XSS",
        "recomendacion": "Usa InnerText en lugar de InnerHtml, o sanitiza el valor antes",
        "patron": re.compile(r'\.InnerHtml\s*=', re.IGNORECASE),
        "extensiones": {".cs"},
    },

    # ── SQL Injection ────────────────────────────────────────────────────────────
    {
        "tipo": "SQL_INJECTION",
        "severidad": "CRITICA",
        "descripcion": "Concatenación directa de string en query SQL",
        "recomendacion": "Usa parámetros: cmd.Parameters.AddWithValue('@param', valor) o Entity Framework con LINQ",
        "patron": re.compile(
            r'(ExecuteReader|ExecuteNonQuery|ExecuteScalar|SqlCommand)\s*\([^)]*\+',
            re.IGNORECASE
        ),
        "extensiones": {".cs"},
    },
    {
        "tipo": "SQL_INJECTION",
        "severidad": "CRITICA",
        "descripcion": "String interpolada dentro de query SQL",
        "recomendacion": "Nunca uses $\"...{variable}...\" en queries. Usa parámetros SqlParameter",
        "patron": re.compile(
            r'(SELECT|INSERT|UPDATE|DELETE).{0,100}\$"',
            re.IGNORECASE
        ),
        "extensiones": {".cs"},
    },
    {
        "tipo": "SQL_INJECTION",
        "severidad": "ALTA",
        "descripcion": "FromSqlRaw() con interpolación puede ser SQLi en EF Core",
        "recomendacion": "Usa FromSqlInterpolated() o parámetros explícitos con FromSqlRaw()",
        "patron": re.compile(r'FromSqlRaw\s*\(\s*\$"', re.IGNORECASE),
        "extensiones": {".cs"},
    },

    # ── Secretos hardcodeados ────────────────────────────────────────────────────
    {
        "tipo": "HARDCODED_SECRET",
        "severidad": "CRITICA",
        "descripcion": "Posible contraseña o secret hardcodeado en el código",
        "recomendacion": "Mueve los secretos a appsettings.json, variables de entorno o Azure Key Vault",
        "patron": re.compile(
            r'(password|pwd|secret|apikey|api_key|connectionstring)\s*=\s*"[^"]{4,}"',
            re.IGNORECASE
        ),
        "extensiones": {".cs", ".config", ".json"},
    },

    # ── Open Redirect ────────────────────────────────────────────────────────────
    {
        "tipo": "OPEN_REDIRECT",
        "severidad": "MEDIA",
        "descripcion": "Redirect() con valor no validado puede redirigir a sitios externos",
        "recomendacion": "Usa Url.IsLocalUrl(returnUrl) antes de redirigir, o usa RedirectToAction()",
        "patron": re.compile(
            r'return\s+Redirect\s*\(\s*[a-zA-Z]',
            re.IGNORECASE
        ),
        "extensiones": {".cs"},
    },

    # ── Deserialización insegura ─────────────────────────────────────────────────
    {
        "tipo": "INSECURE_DESERIALIZATION",
        "severidad": "ALTA",
        "descripcion": "BinaryFormatter está marcado como inseguro por Microsoft desde .NET 5",
        "recomendacion": "Usa System.Text.Json o XmlSerializer en su lugar",
        "patron": re.compile(r'BinaryFormatter', re.IGNORECASE),
        "extensiones": {".cs"},
    },

    # ── Criptografía débil ───────────────────────────────────────────────────────
    {
        "tipo": "WEAK_CRYPTO",
        "severidad": "ALTA",
        "descripcion": "MD5 o SHA1 son algoritmos de hash inseguros para contraseñas",
        "recomendacion": "Usa BCrypt, Argon2 o PBKDF2 para contraseñas. Para integridad de datos usa SHA256+",
        "patron": re.compile(r'(MD5|SHA1)\.Create\(\)', re.IGNORECASE),
        "extensiones": {".cs"},
    },
    {
        "tipo": "WEAK_CRYPTO",
        "severidad": "MEDIA",
        "descripcion": "DES/3DES son cifrados obsoletos y vulnerables",
        "recomendacion": "Usa AES-256 en lugar de DES o TripleDES",
        "patron": re.compile(r'(DESCryptoServiceProvider|TripleDES)', re.IGNORECASE),
        "extensiones": {".cs"},
    },

    # ── Debug / info expuesta ────────────────────────────────────────────────────
    {
        "tipo": "INFO_EXPOSURE",
        "severidad": "BAJA",
        "descripcion": "customErrors mode='Off' expone stack traces en producción",
        "recomendacion": "Cambia a customErrors mode='On' o 'RemoteOnly' en Web.config",
        "patron": re.compile(r'customErrors\s+mode\s*=\s*"Off"', re.IGNORECASE),
        "extensiones": {".config"},
    },
]

# ─── Lógica de escaneo ─────────────────────────────────────────────────────────

EXTENSIONES_SOPORTADAS = {".cs", ".cshtml", ".aspx", ".config"}
IGNORAR_DIRS = {"bin", "obj", ".git", "node_modules", "packages", "Migrations"}

def obtener_archivos(directorio: str) -> list[Path]:
    base = Path(directorio)
    archivos = []
    for f in base.rglob("*"):
        if any(p in IGNORAR_DIRS for p in f.parts):
            continue
        if f.suffix in EXTENSIONES_SOPORTADAS and f.is_file():
            archivos.append(f)
    return sorted(archivos)


def escanear_archivo(path: Path) -> list[Hallazgo]:
    hallazgos = []
    try:
        lineas = path.read_text(encoding="utf-8", errors="ignore").splitlines()
    except Exception:
        return []

    contenido_completo = "\n".join(lineas)

    for regla in REGLAS:
        if path.suffix not in regla["extensiones"]:
            continue

        for match in regla["patron"].finditer(contenido_completo):
            # Calcular número de línea
            num_linea = contenido_completo[:match.start()].count("\n") + 1
            codigo = lineas[num_linea - 1].strip() if num_linea <= len(lineas) else ""

            hallazgos.append(Hallazgo(
                tipo=regla["tipo"],
                severidad=regla["severidad"],
                archivo=str(path),
                linea=num_linea,
                codigo=codigo[:120],
                descripcion=regla["descripcion"],
                recomendacion=regla["recomendacion"],
            ))

    return hallazgos


# ─── Salida en consola ─────────────────────────────────────────────────────────

COLORES = {
    "CRITICA": "\033[91m",
    "ALTA":    "\033[93m",
    "MEDIA":   "\033[94m",
    "BAJA":    "\033[92m",
    "BOLD":    "\033[1m",
    "CYAN":    "\033[96m",
    "RESET":   "\033[0m",
}

ICONOS = {
    "CSRF":                   "🔒",
    "XSS":                    "💉",
    "SQL_INJECTION":           "🗄️",
    "HARDCODED_SECRET":        "🔐",
    "OPEN_REDIRECT":           "↪️",
    "INSECURE_DESERIALIZATION":"📦",
    "WEAK_CRYPTO":             "🔑",
    "INFO_EXPOSURE":           "👁️",
}

def imprimir_hallazgos(hallazgos: list[Hallazgo]):
    por_archivo: dict[str, list[Hallazgo]] = {}
    for h in hallazgos:
        por_archivo.setdefault(h.archivo, []).append(h)

    for archivo, items in por_archivo.items():
        print(f"\n{COLORES['BOLD']}📄 {archivo}{COLORES['RESET']}")
        for h in items:
            color = COLORES.get(h.severidad, "")
            icono = ICONOS.get(h.tipo, "⚠️")
            print(f"  {icono} {color}{COLORES['BOLD']}[{h.severidad}] {h.tipo}{COLORES['RESET']} — Línea {h.linea}")
            print(f"     📌 {h.descripcion}")
            print(f"     💡 {COLORES['CYAN']}{h.recomendacion}{COLORES['RESET']}")
            if h.codigo:
                print(f"     ▶  {h.codigo}")

# ─── Main ──────────────────────────────────────────────────────────────────────

def main():
    if len(sys.argv) < 2:
        print("Uso: python csharp_security_scan.py <directorio>")
        sys.exit(1)

    directorio = sys.argv[1]
    if not Path(directorio).is_dir():
        print(f"❌ No existe el directorio: {directorio}")
        sys.exit(1)

    archivos = obtener_archivos(directorio)
    if not archivos:
        print("⚠️  No se encontraron archivos .cs/.cshtml/.config")
        sys.exit(0)

    print(f"\n{COLORES['BOLD']}🔍 C# Security Scanner — Local{COLORES['RESET']}")
    print(f"📁 Proyecto : {directorio}")
    print(f"📄 Archivos : {len(archivos)}")
    print("─" * 60)

    todos: list[Hallazgo] = []
    for archivo in archivos:
        todos.extend(escanear_archivo(archivo))

    if not todos:
        print("\n✅  Sin vulnerabilidades detectadas.")
    else:
        imprimir_hallazgos(todos)

    # Conteo por severidad
    conteo = {"CRITICA": 0, "ALTA": 0, "MEDIA": 0, "BAJA": 0}
    for h in todos:
        conteo[h.severidad] = conteo.get(h.severidad, 0) + 1

    print("\n" + "═" * 60)
    print(f"{COLORES['BOLD']}📊 RESUMEN{COLORES['RESET']}")
    print(f"  🔴 Críticas : {conteo['CRITICA']}")
    print(f"  🟡 Altas    : {conteo['ALTA']}")
    print(f"  🔵 Medias   : {conteo['MEDIA']}")
    print(f"  🟢 Bajas    : {conteo['BAJA']}")
    print(f"  ──────────────")
    print(f"  Total       : {sum(conteo.values())}")

    # Guardar JSON
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    reporte = Path(f"reporte_seguridad_{timestamp}.json")
    reporte.write_text(
        json.dumps([asdict(h) for h in todos], indent=2, ensure_ascii=False),
        encoding="utf-8"
    )
    print(f"\n💾 Reporte guardado: {reporte}")
    print("═" * 60)

if __name__ == "__main__":
    main()
