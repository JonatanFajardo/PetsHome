#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
commit_por_modulo.py — Agrupa los cambios pendientes en commits por módulo
siguiendo la convención del repo (Conventional Commits en español, sin autoría).

Flujo de DOS pasos (nunca commitea a ciegas):

  1) PLAN (dry-run, por defecto):
         python tools/commit_por_modulo.py
     - Detecta los archivos cambiados, los agrupa por módulo (entidad),
       ordena los grupos por dependencias, excluye .sql/.md y marca hacks TEMP.
     - Escribe tools/commit_plan.json e imprime el plan.
     - REVISÁS y EDITÁS el JSON: mensajes, a qué grupo va cada archivo,
       qué excluir, el orden de los commits.

  2) COMMIT (ejecuta el plan que aprobaste):
         python tools/commit_por_modulo.py --commit
     - Lee commit_plan.json, parte los archivos compartidos
       (ServiceConfiguration.cs, MappingProfileExtensions.cs) por módulo
       para que cada commit compile, y hace los git add + git commit en orden.

Qué automatiza y qué NO:
  - Automatiza: agrupar por nombre de entidad, ordenar por dependencias,
    excluir SQL/MD, partir los archivos compartidos, los git add/commit.
  - NO adivina: el mensaje semántico fino y las dudas de agrupación quedan
    para que las ajustes en el JSON. Por eso el paso 1 es editable.

Opciones:
  --commit            Ejecuta tools/commit_plan.json (en vez de regenerarlo).
  --plan RUTA         Usa otra ruta de plan (default: tools/commit_plan.json).
  --include-sql       No excluir los .sql.
  --include-md        No excluir los .md (incluye CLAUDE.md).
  --force-replan      Regenera el plan aunque ya exista (sobreescribe).
"""

import argparse
import json
import os
import re
import subprocess
import sys
from collections import Counter, defaultdict

# ── Archivos compartidos que se parten por módulo (aditivos: solo se agregan líneas) ──
SHARED_FILES = [
    "PetsHome.Business/ServiceConfiguration.cs",
    "PetsHome.Business/Extensions/MappingProfileExtensions.cs",
]

# Marcadores que delatan código de prueba temporal → se excluye y se avisa.
TEMP_MARKERS = ["TEMP", "NoExiste", "DELETEME", "QUITAR", "DEBUG BORRAR"]

PLAN_DEFAULT = "tools/commit_plan.json"


# ============================================================
#  Helpers de git
# ============================================================
def git(*args, check=True, capture=True):
    """Corre git y devuelve stdout (texto)."""
    res = subprocess.run(
        ["git", *args],
        cwd=REPO,
        check=check,
        text=True,
        encoding="utf-8",
        stdout=subprocess.PIPE if capture else None,
        stderr=subprocess.PIPE,
    )
    if check and res.returncode != 0:
        sys.exit(f"git {' '.join(args)} falló:\n{res.stderr}")
    return (res.stdout or "").rstrip("\n")


def repo_root():
    try:
        root = subprocess.check_output(
            ["git", "rev-parse", "--show-toplevel"], text=True, encoding="utf-8"
        ).strip()
        return root
    except Exception:
        sys.exit("No estás dentro de un repo git.")


def changed_files():
    """Lista (status, ruta) de todo lo cambiado/nuevo. status: 'M','A','??',..."""
    out = git("status", "--porcelain")
    files = []
    for line in out.splitlines():
        if not line.strip():
            continue
        status = line[:2].strip()
        path = line[3:].strip()
        # renombrados "a -> b": nos quedamos con el destino.
        if " -> " in path:
            path = path.split(" -> ", 1)[1]
        path = path.strip('"')
        files.append((status, path))
    return files


# ============================================================
#  Detección de módulo (entidad) a partir de la ruta
# ============================================================
def kebab(name):
    """MascotaAlergia -> mascota-alergia"""
    s = re.sub(r"(?<!^)(?=[A-Z])", "-", name)
    return s.lower()


def pascal_from_kebab(slug):
    return "".join(p.capitalize() for p in re.split(r"[-_]", slug))


def detect_entity(path):
    """
    Devuelve el nombre de entidad (PascalCase) de un archivo, o None si no aplica
    (archivo compartido / sin módulo claro).
    """
    p = path.replace("\\", "/")

    if p in SHARED_FILES:
        return None

    base = os.path.basename(p)

    # Services / Repositories / Controllers
    m = re.match(r"^(?P<e>[A-Za-z]+?)(Service|Repository|Controller)\.cs$", base)
    if m:
        return m.group("e")

    # ViewModels (uno o varios): {Entity}ViewModel.cs / {Entity}ViewModels.cs
    m = re.match(r"^(?P<e>[A-Za-z]+?)ViewModels?\.cs$", base)
    if m:
        return m.group("e")

    # Entidades Common:  tb{Entity}.cs
    m = re.match(r"^tb(?P<e>[A-Za-z]+)\.cs$", base)
    if m:
        return m.group("e")

    # Result classes:  PR_{Schema}_{Entity}_{Op}Result.cs
    m = re.match(r"^PR_[A-Za-z]+_(?P<e>[A-Za-z]+)_[A-Za-z]+Result\.cs$", base)
    if m:
        return m.group("e")

    # Vistas:  Views/{Entity}/archivo.cshtml   (ignora Shared)
    m = re.search(r"/Views/(?P<e>[A-Za-z]+)/", p)
    if m and m.group("e") != "Shared":
        return m.group("e")

    # wwwroot JS/CSS por slug de página o componente
    m = re.search(r"/wwwroot/(?:css|js/pages)/(?P<slug>[a-z0-9\-]+)\.(?:css|js)$", p)
    if m:
        return pascal_from_kebab(m.group("slug"))
    m = re.search(r"/wwwroot/js/components/(?P<slug>[a-z0-9\-]+)\.js$", p)
    if m:
        return pascal_from_kebab(m.group("slug"))

    return None


def has_temp_marker(path):
    """True si el diff del archivo contiene un marcador de código temporal."""
    try:
        diff = git("diff", "HEAD", "--", path, check=False)
    except SystemExit:
        return False
    for line in diff.splitlines():
        if line.startswith("+") and not line.startswith("+++"):
            if any(mk.lower() in line.lower() for mk in TEMP_MARKERS):
                return True
    return False


# ============================================================
#  Dependencias entre módulos (para ordenar los commits)
# ============================================================
def module_dependencies(groups):
    """
    Para cada grupo, busca referencias 'readonly XService/XRepository' en sus .cs
    y arma aristas groupo -> grupo del que depende.
    """
    entity_to_group = {}
    for gname, info in groups.items():
        entity_to_group[info["entity"]] = gname

    deps = defaultdict(set)
    for gname, info in groups.items():
        for f in info["files"]:
            if not f.endswith(".cs"):
                continue
            full = os.path.join(REPO, f)
            if not os.path.isfile(full):
                continue
            try:
                txt = open(full, encoding="utf-8", errors="ignore").read()
            except Exception:
                continue
            for ent in re.findall(r"readonly\s+([A-Za-z]+?)(?:Service|Repository)\b", txt):
                tgt = entity_to_group.get(ent)
                if tgt and tgt != gname:
                    deps[gname].add(tgt)
    return deps


def topo_order(groups, deps):
    """Orden topológico estable: primero las dependencias."""
    order = []
    visited = set()
    temp = set()

    def visit(n):
        if n in visited:
            return
        if n in temp:   # ciclo: lo dejamos pasar en orden de aparición
            return
        temp.add(n)
        for d in sorted(deps.get(n, ())):
            if d in groups:
                visit(d)
        temp.discard(n)
        visited.add(n)
        order.append(n)

    for n in groups:  # dict preserva orden de inserción (detección)
        visit(n)
    return order


# ============================================================
#  PASO 1 — Generar el plan
# ============================================================
def build_plan(include_sql, include_md):
    files = changed_files()

    excluded = []
    warnings = []
    groups = {}          # entity -> {entity, files[]}
    sin_modulo = []      # archivos sin módulo claro

    for status, path in files:
        low = path.lower()

        if path.replace("\\", "/") in SHARED_FILES:
            continue  # se reparten aparte

        if not include_sql and low.endswith(".sql"):
            excluded.append(path)
            continue
        if not include_md and low.endswith(".md"):
            excluded.append(path)
            continue

        if has_temp_marker(path):
            excluded.append(path)
            warnings.append(f"EXCLUIDO por marcador TEMP: {path}")
            continue

        ent = detect_entity(path)
        if ent is None:
            sin_modulo.append(path)
            continue

        groups.setdefault(ent, {"entity": ent, "files": []})
        groups[ent]["files"].append(path)

    # Ordenar por dependencias
    deps = module_dependencies(groups)
    order = topo_order(groups, deps)

    # Repartir las líneas de los archivos compartidos por entidad
    shared_split = split_shared(order)

    commits = []
    for ent in order:
        info = groups[ent]
        scope = kebab(ent)
        is_new = any(not file_tracked(f) for f in info["files"])
        ctype = "feat" if is_new else "fix"
        shared_lines = {sf: shared_split[sf].get(ent, []) for sf in SHARED_FILES
                        if shared_split[sf].get(ent)}
        commit = {
            "type": ctype,
            "scope": scope,
            "message": f"{ctype}({scope}): <describe el módulo {ent}>",
            "files": sorted(info["files"]),
        }
        if shared_lines:
            commit["shared_lines"] = shared_lines
        commits.append(commit)

    # Líneas compartidas que no matchearon ninguna entidad → avisar
    for sf in SHARED_FILES:
        huérfanas = shared_split[sf].get("_sin_duenio", [])
        if huérfanas:
            warnings.append(
                f"{sf}: {len(huérfanas)} línea(s) agregada(s) sin módulo detectado "
                f"(asignalas a mano con 'shared_lines' en algún commit): "
                + " | ".join(huérfanas[:3]) + ("…" if len(huérfanas) > 3 else "")
            )

    plan = {
        "_ayuda": "Editá 'message', mové archivos entre 'files', y el orden de 'commits'. "
                  "Luego: python tools/commit_por_modulo.py --commit",
        "shared_files": SHARED_FILES,
        "excluded": sorted(excluded),
        "sin_modulo": sorted(sin_modulo),
        "warnings": warnings,
        "commits": commits,
    }
    return plan


def file_tracked(path):
    r = subprocess.run(["git", "ls-files", "--error-unmatch", path],
                        cwd=REPO, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    return r.returncode == 0


def added_lines_of(shared_file):
    """Lista de líneas agregadas (texto, sin el '+') en el diff del archivo compartido."""
    diff = git("diff", "HEAD", "--", shared_file, check=False)
    out = []
    for line in diff.splitlines():
        if line.startswith("+") and not line.startswith("+++"):
            out.append(line[1:])
    return out


def split_shared(order):
    """
    Para cada archivo compartido, asigna cada línea AGREGADA a la entidad cuyo
    nombre aparece en la línea. Devuelve {shared_file: {entidad: [líneas...]}}.
    """
    result = {}
    for sf in SHARED_FILES:
        buckets = defaultdict(list)
        for line in added_lines_of(sf):
            owner = None
            # la entidad cuyo nombre aparezca en la línea (la más larga gana)
            for ent in sorted(order, key=len, reverse=True):
                if ent in line:
                    owner = ent
                    break
            buckets[owner if owner else "_sin_duenio"].append(line)
        result[sf] = buckets
    return result


# ============================================================
#  PASO 2 — Ejecutar el plan (commitear)
# ============================================================
def run_plan(plan):
    # Seguridad: partimos de un index limpio para controlar qué entra a cada commit.
    git("reset", "-q")

    shared_files = plan.get("shared_files", SHARED_FILES)

    # Estado "final" (working actual) y estado HEAD de cada archivo compartido.
    work_content = {sf: read_file(sf) for sf in shared_files}
    head_content = {sf: git("show", f"HEAD:{sf}", check=False) for sf in shared_files}

    # Para cada compartido: líneas agregadas por commit (según el plan).
    # Reconstruimos el estado intermedio quitando del working las líneas que
    # pertenecen a commits POSTERIORES al actual.
    per_commit_lines = {sf: [] for sf in shared_files}  # lista paralela a commits
    for c in plan["commits"]:
        sl = c.get("shared_lines", {})
        for sf in shared_files:
            per_commit_lines[sf].append(sl.get(sf, []))

    n = len(plan["commits"])
    for i, c in enumerate(plan["commits"]):
        msg = c["message"]
        if "<describe" in msg or "<" in msg and ">" in msg:
            sys.exit(f"El commit #{i+1} todavía tiene un mensaje placeholder:\n  {msg}\n"
                     f"Editá tools/commit_plan.json antes de --commit.")

        # 1) archivos normales del commit
        for f in c.get("files", []):
            git("add", "--", f)

        # 2) archivos compartidos: estado intermedio (quita líneas de commits > i)
        for sf in shared_files:
            later = []
            for j in range(i + 1, n):
                later.extend(per_commit_lines[sf][j])
            if not file_has_changes(sf) and not later and i < n - 1:
                continue
            intermediate = remove_surplus_lines(
                work_content[sf], head_content[sf], later
            )
            write_file(sf, intermediate)
            git("add", "--", sf)

        git("commit", "-q", "-m", msg)
        print(f"  ✓ [{i+1}/{n}] {msg}")

    # Al terminar, los compartidos ya quedaron en su estado final (working completo).
    print("\nListo. Revisá con: git log --oneline -%d" % n)
    pend = git("status", "--porcelain")
    if pend.strip():
        print("\nPendiente sin commitear (esperado: exclusiones):")
        print(pend)


def remove_surplus_lines(work_text, head_text, lines_to_remove):
    """
    Quita de 'work_text' las ocurrencias SOBRANTES (vs HEAD) de cada línea en
    lines_to_remove. Así reconstruye el estado HEAD + adiciones previas, sin
    tocar líneas que ya existían en HEAD.
    """
    work = work_text.split("\n")
    head_counts = Counter(head_text.split("\n"))
    # cuántas quitar por texto = cuántas aparecen en lines_to_remove
    want_remove = Counter(lines_to_remove)
    result = []
    removed = Counter()
    # recorremos de abajo hacia arriba para quitar las adiciones (van al final de su bloque)
    for ln in reversed(work):
        if want_remove.get(ln, 0) - removed.get(ln, 0) > 0:
            surplus_allowed = work.count(ln) - head_counts.get(ln, 0)
            if removed.get(ln, 0) < surplus_allowed:
                removed[ln] += 1
                continue
        result.append(ln)
    result.reverse()
    return "\n".join(result)


def file_has_changes(path):
    d = git("diff", "HEAD", "--", path, check=False)
    return bool(d.strip())


def read_file(rel):
    with open(os.path.join(REPO, rel), encoding="utf-8") as fh:
        return fh.read()


def write_file(rel, content):
    with open(os.path.join(REPO, rel), "w", encoding="utf-8", newline="\n") as fh:
        fh.write(content)


# ============================================================
#  Impresión del plan
# ============================================================
def print_plan(plan):
    print("\n" + "=" * 60)
    print(" PLAN DE COMMITS (revisá y editá el JSON antes de --commit)")
    print("=" * 60)
    for i, c in enumerate(plan["commits"], 1):
        print(f"\n[{i}] {c['message']}")
        for f in c["files"]:
            print(f"      {f}")
        for sf, lines in c.get("shared_lines", {}).items():
            print(f"      ~ {sf}: +{len(lines)} línea(s)")
    if plan["sin_modulo"]:
        print("\n⚠ SIN MÓDULO (asignalos a mano a algún commit en el JSON):")
        for f in plan["sin_modulo"]:
            print(f"      {f}")
    if plan["excluded"]:
        print("\n• EXCLUIDOS (no se commitean):")
        for f in plan["excluded"]:
            print(f"      {f}")
    if plan["warnings"]:
        print("\n⚠ AVISOS:")
        for w in plan["warnings"]:
            print(f"      {w}")
    print()


# ============================================================
#  Main
# ============================================================
def main():
    global REPO
    # La consola de Windows suele ser cp1252 y revienta con ⚠/cajas: forzamos UTF-8.
    for stream in (sys.stdout, sys.stderr):
        try:
            stream.reconfigure(encoding="utf-8", errors="replace")
        except Exception:
            pass
    REPO = repo_root()

    ap = argparse.ArgumentParser(description=__doc__,
                                 formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--commit", action="store_true", help="Ejecuta el plan (commitea).")
    ap.add_argument("--plan", default=PLAN_DEFAULT, help="Ruta del plan JSON.")
    ap.add_argument("--include-sql", action="store_true", help="No excluir .sql.")
    ap.add_argument("--include-md", action="store_true", help="No excluir .md.")
    ap.add_argument("--force-replan", action="store_true", help="Regenera el plan.")
    args = ap.parse_args()

    plan_path = os.path.join(REPO, args.plan)

    if args.commit:
        if not os.path.isfile(plan_path):
            sys.exit(f"No existe {args.plan}. Corré primero sin --commit para generarlo.")
        with open(plan_path, encoding="utf-8") as fh:
            plan = json.load(fh)
        if not plan.get("commits"):
            sys.exit("El plan no tiene commits.")
        print(f"Ejecutando {args.plan} ({len(plan['commits'])} commits)…")
        run_plan(plan)
        return

    # PASO 1 — generar plan
    if os.path.isfile(plan_path) and not args.force_replan:
        sys.exit(f"Ya existe {args.plan}. Editalo y corré --commit, "
                 f"o usá --force-replan para regenerarlo.")

    plan = build_plan(include_sql=args.include_sql, include_md=args.include_md)
    if not plan["commits"]:
        print("No hay cambios agrupables (¿solo exclusiones?).")
    os.makedirs(os.path.dirname(plan_path), exist_ok=True)
    with open(plan_path, "w", encoding="utf-8") as fh:
        json.dump(plan, fh, ensure_ascii=False, indent=2)
    print_plan(plan)
    print(f"Plan escrito en {args.plan}")
    print("Editá los mensajes y corré:  python tools/commit_por_modulo.py --commit")


if __name__ == "__main__":
    main()
