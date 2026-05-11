#!/usr/bin/env python3
"""
normalize_commits.py  —  Dos fases:
  Fase 1: Dividir commits que mezclan cambios (rebase -i + scripts bash)
  Fase 2: Renombrar commits al formato Conventional Commits (filter-branch)

Uso: python normalize_commits.py
"""

import subprocess, sys, os, textwrap, re, shutil

REPO_ROOT   = os.path.dirname(os.path.abspath(__file__))
HELPERS_DIR = os.path.join(REPO_ROOT, ".rebase_helpers")
FILTER_SH   = os.path.join(HELPERS_DIR, "msg_filter_normalize.sh")
BATCH_SIZE  = 25
TIPOS       = "feat, fix, style, refactor, docs, chore, perf, test, build, ci, revert"
CONV_PAT    = re.compile(r"^[0-9a-f]{40} (feat|fix|style|refactor|docs|chore|perf|test|build|ci|revert)[\(:]")

# ══════════════════════════════════════════════════════════════════════════════
#  GIT HELPERS
# ══════════════════════════════════════════════════════════════════════════════

def run(cmd, cwd=REPO_ROOT, check=True):
    r = subprocess.run(cmd, cwd=cwd, capture_output=True, text=True)
    if check and r.returncode != 0:
        raise RuntimeError(r.stderr or r.stdout)
    return r.stdout.strip()

def git_log_full():
    """Retorna lista [(hash_completo, mensaje)] del más nuevo al más viejo."""
    out = run(["git", "log", "--format=%H %s"])
    result = []
    for line in out.splitlines():
        if line:
            h, _, msg = line.partition(" ")
            result.append((h, msg))
    return result

def non_conventional(log=None):
    if log is None:
        log = git_log_full()
    return [(h, m) for h, m in log if not CONV_PAT.match(h + " " + m)]

def commit_stat(hash_):
    """git show --stat de un commit."""
    return run(["git", "show", "--stat", "--format=", hash_])

def commit_diff(hash_):
    """git show completo (diff) de un commit — para el prompt de la IA."""
    return run(["git", "show", "--format=", hash_])

def files_in_commit(hash_):
    """Lista de archivos modificados en un commit."""
    out = run(["git", "diff-tree", "--no-commit-id", "-r", "--name-only", hash_])
    return [l for l in out.splitlines() if l]

def stash_tracked():
    dirty = [l for l in run(["git", "status", "--short"]).splitlines()
             if l and l[1] in ("M", "A", "D")]
    if dirty:
        print("\n⚠  Cambios sin commit detectados — haciendo stash temporal...")
        run(["git", "stash", "push", "-m", "normalize_commits: stash"])
        return True
    return False

def pop_stash():
    run(["git", "stash", "pop"])
    print("✔  Stash restaurado.")

def delete_original_refs():
    for ref in run(["git", "for-each-ref", "--format=%(refname)",
                    "refs/original/"], check=False).splitlines():
        subprocess.run(["git", "update-ref", "-d", ref],
                       cwd=REPO_ROOT, capture_output=True)

# ══════════════════════════════════════════════════════════════════════════════
#  UI HELPERS
# ══════════════════════════════════════════════════════════════════════════════

def hr(char="─", width=62): print(char * width)

def box(title):
    hr("═")
    print(f"  {title}")
    hr("═")

def read_multiline(prompt=""):
    if prompt:
        print(prompt)
    print("  (Pega el texto y termina con una línea vacía + Enter)\n")
    lines = []
    while True:
        try:
            line = input()
        except EOFError:
            break
        if line == "" and lines:
            break
        lines.append(line)
    return "\n".join(lines)

def ask(prompt, valid=("s", "n")):
    while True:
        r = input(f"\n{prompt} ({'/'.join(valid)}): ").strip().lower()
        if r in valid:
            return r

# ══════════════════════════════════════════════════════════════════════════════
#  FASE 1 — SPLITS
# ══════════════════════════════════════════════════════════════════════════════

SPLIT_PROMPT_TMPL = """
Analiza este commit de Git. Contiene cambios de {n_files} archivos.
Necesito saber si CONVIENE dividirlo en sub-commits más atómicos.

STAT:
{stat}

Si SÍ conviene dividirlo, responde con este formato exacto (sin explicaciones):

SPLIT: SI
SUB1_FILES: archivo1.cs, archivo2.js
SUB1_MSG: tipo(scope): descripción en minúscula
SUB2_FILES: *
SUB2_MSG: tipo(scope): descripción en minúscula

(usa * en el último sub-commit para indicar "el resto de archivos")
(puedes agregar SUB3, SUB4, etc. si necesitas más de 2 partes)

Si NO conviene dividirlo (cambios cohesivos), responde:
SPLIT: NO
MSG: tipo(scope): descripción en minúscula

Tipos permitidos: {tipos}
""".strip()

def build_split_prompt(hash_, stat, files):
    return SPLIT_PROMPT_TMPL.format(
        n_files=len(files),
        stat=stat,
        tipos=TIPOS,
    )

def parse_split_response(text, all_files):
    """
    Parsea la respuesta de la IA para un split.
    Retorna:
      {"split": False, "msg": "..."}
      {"split": True, "subs": [{"files": [...], "msg": "..."}, ...]}
    """
    lines = [l.strip() for l in text.strip().splitlines() if l.strip()]
    result = {}

    for l in lines:
        if l.upper().startswith("SPLIT:"):
            val = l.split(":", 1)[1].strip().upper()
            result["split"] = val == "SI"
        elif l.upper().startswith("MSG:"):
            result["msg"] = l.split(":", 1)[1].strip()

    if not result.get("split", False):
        return {"split": False, "msg": result.get("msg", "")}

    # parsear SUBn_FILES y SUBn_MSG
    subs_raw = {}
    for l in lines:
        m = re.match(r"SUB(\d+)_(FILES|MSG):\s*(.+)", l, re.IGNORECASE)
        if m:
            idx, key, val = int(m.group(1)), m.group(2).upper(), m.group(3).strip()
            subs_raw.setdefault(idx, {})[key] = val

    subs = []
    used_files = set()
    for idx in sorted(subs_raw):
        sub = subs_raw[idx]
        msg = sub.get("MSG", "")
        raw_files = sub.get("FILES", "*")
        if raw_files.strip() == "*":
            files = [f for f in all_files if f not in used_files]
        else:
            files = [f.strip() for f in raw_files.split(",") if f.strip()]
        used_files.update(files)
        subs.append({"files": files, "msg": msg})

    return {"split": True, "subs": subs}

def write_split_script(hash_short, subs, orig_date_cmd):
    """
    Genera el script bash para dividir un commit.
    subs = [{"files": [...], "msg": "..."}]
    """
    os.makedirs(HELPERS_DIR, exist_ok=True)
    path = os.path.join(HELPERS_DIR, f"split_{hash_short}.sh")

    lines = ["#!/bin/bash", "set -e",
             'ORIG_AUTHOR_DATE=$(git log --format="%ai" -n 1 HEAD)',
             'ORIG_COMMITTER_DATE="$ORIG_AUTHOR_DATE"',
             "git reset HEAD~1",
             ""]

    for i, sub in enumerate(subs):
        lines.append(f"# Sub-commit {i+1}: {sub['msg']}")
        if i < len(subs) - 1:
            for f in sub["files"]:
                lines.append(f'git add "{f}"')
        else:
            lines.append("git add -u")  # el último toma el resto
        lines.append(
            f'GIT_AUTHOR_DATE="$ORIG_AUTHOR_DATE" '
            f'GIT_COMMITTER_DATE="$ORIG_COMMITTER_DATE" '
            f'git commit -m "{sub["msg"]}"'
        )
        lines.append("")

    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(lines))
    return path

SEQ_EDITOR_TMPL = """#!/usr/bin/env python3
import sys, re
HELPERS = r"{helpers}"
todo = sys.argv[1]
with open(todo, encoding='utf-8') as f:
    lines = f.readlines()
out = []
for line in lines:
    s = line.rstrip()
    if re.match(r'^pick\\s+{short}', s):
        out.append(line)
        out.append('exec bash "' + HELPERS + '/split_{short}.sh"\\n')
        continue
    out.append(line)
with open(todo, 'w', encoding='utf-8') as f:
    f.writelines(out)
""".strip()

def run_split_rebase(hash_full, split_script_path):
    """Lanza git rebase -i HASH^ con el seq_editor para inyectar el exec."""
    short = hash_full[:7]
    seq_path = os.path.join(HELPERS_DIR, f"seq_{short}.py")

    seq_code = SEQ_EDITOR_TMPL.format(
        helpers=HELPERS_DIR.replace("\\", "/"),
        short=short,
    )
    with open(seq_path, "w", encoding="utf-8", newline="\n") as f:
        f.write(seq_code)

    env = {
        **os.environ,
        "GIT_SEQUENCE_EDITOR": f"python {seq_path}",
        "GIT_EDITOR": "true",
    }
    print(f"\n⏳ Ejecutando rebase para {short}...")
    r = subprocess.run(
        ["git", "rebase", "-i", f"{hash_full}^"],
        cwd=REPO_ROOT, env=env,
        capture_output=True, text=True
    )
    if r.returncode != 0:
        print(f"❌ Rebase falló:\n{r.stdout}\n{r.stderr}")
        print("Abortando rebase...")
        subprocess.run(["git", "rebase", "--abort"], cwd=REPO_ROOT)
        return False
    print(f"✔  Split de {short} aplicado.")
    return True

def phase1_splits():
    box("FASE 1 — Identificar y dividir commits mixtos")
    print("""
Se revisará cada commit que no sigue Conventional Commits.
Para cada uno, el script genera un prompt para una IA externa
que decide si conviene dividirlo y cómo.
""")

    if ask("¿Ejecutar Fase 1 (splits)?") == "n":
        return

    total_splits = 0

    while True:
        log = git_log_full()
        candidates = non_conventional(log)
        if not candidates:
            print("\n✔  No quedan commits sin normalizar.")
            break

        # tomar el más viejo que aún no es convencional
        # procesamos de más viejo a más nuevo
        oldest = candidates[-1]
        hash_, msg = oldest

        hr()
        print(f"\n  Commit: {hash_[:8]}  {msg[:60]}")

        stat   = commit_stat(hash_)
        files  = files_in_commit(hash_)

        print(f"\n  Archivos modificados ({len(files)}):")
        for f in files:
            print(f"    {f}")

        print("\n📋 COPIA Y PEGA ESTE PROMPT EN LA IA:\n")
        prompt = build_split_prompt(hash_, stat, files)
        hr("·")
        print(prompt)
        hr("·")

        raw = read_multiline("\n⬇  Respuesta de la IA:")
        if not raw.strip():
            print("Sin respuesta — saltando este commit.")
            break

        parsed = parse_split_response(raw, files)

        if not parsed["split"]:
            msg_new = parsed.get("msg", "")
            print(f"\n  IA dice: NO dividir → renombrar a: {msg_new}")
            if msg_new and ask("¿Aceptar?") == "s":
                pass  # se procesará en Fase 2
            continue

        # mostrar plan de split
        print(f"\n  IA dice: DIVIDIR en {len(parsed['subs'])} sub-commits:")
        for i, sub in enumerate(parsed["subs"], 1):
            print(f"    {i}. {sub['msg']}")
            for f in sub["files"]:
                print(f"       • {f}")

        if ask("¿Aplicar este split?") == "n":
            print("Split cancelado — se renombrará en Fase 2.")
            continue

        split_path = write_split_script(hash_[:7], parsed["subs"], None)
        ok = run_split_rebase(hash_, split_path)
        if ok:
            total_splits += 1
        else:
            if ask("¿Continuar con el siguiente commit?") == "n":
                break

    print(f"\n✔  Fase 1 completada. Splits aplicados: {total_splits}")

# ══════════════════════════════════════════════════════════════════════════════
#  FASE 2 — RENOMBRADO
# ══════════════════════════════════════════════════════════════════════════════

def build_rename_prompt(batch):
    lines = "\n".join(f"{h}  {msg}" for h, msg in batch)
    return textwrap.dedent(f"""
        Convierte estos mensajes de commits al formato Conventional Commits.

        Formato requerido:
            tipo(scope): descripción en minúscula

        Reglas:
        - Tipos permitidos: {TIPOS}
        - scope: una palabra en minúscula (omitir si aplica a todo el proyecto)
        - descripción: en minúscula, sin punto al final, en español
        - Si el mensaje es muy largo, resume la idea principal

        Devuelve ÚNICAMENTE las líneas en este formato exacto (sin explicaciones):
            HASH|nuevo mensaje

        Commits:
        {lines}
    """).strip()

def parse_rename_response(text, expected):
    mapping = {}
    for line in text.strip().splitlines():
        line = line.strip()
        if "|" not in line:
            continue
        h, _, msg = line.partition("|")
        h, msg = h.strip(), msg.strip()
        full = next((eh for eh in expected if eh.startswith(h)), None)
        if full and msg:
            mapping[full] = msg
    return mapping

def write_filter_script(mapping):
    os.makedirs(HELPERS_DIR, exist_ok=True)
    lines = ["#!/bin/bash", 'case "$GIT_COMMIT" in']
    for h, msg in mapping.items():
        safe = msg.replace('"', '\\"')
        lines.append(f'  {h}) printf "%s" "{safe}";;')
    lines += ["  *) cat;;", "esac"]
    with open(FILTER_SH, "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(lines) + "\n")

def run_filter_branch():
    script_path = FILTER_SH.replace("\\", "/")
    env = {**os.environ, "FILTER_BRANCH_SQUELCH_WARNING": "1"}
    print("\n⏳ Ejecutando git filter-branch...")
    r = subprocess.run(
        ["git", "filter-branch", "-f",
         "--msg-filter", f'bash "{script_path}"',
         "--env-filter", 'export GIT_COMMITTER_DATE="$GIT_AUTHOR_DATE"',
         "--", "--all"],
        cwd=REPO_ROOT, env=env,
        stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True
    )
    lines = r.stdout.splitlines()
    rewrites = sum(1 for l in lines if "Rewrite" in l)
    if rewrites:
        print(f"   Commits procesados: {rewrites}")
    for l in lines:
        if "Ref '" in l:
            print(f"   {l.strip()}")
    if r.returncode != 0:
        print("❌ filter-branch falló:")
        print(r.stdout[-2000:])
        sys.exit(1)
    print("✔  Reescritura completada.")

def phase2_renames():
    box("FASE 2 — Renombrar commits al formato Conventional Commits")

    commits = non_conventional()
    if not commits:
        print("\n✔  No hay commits que renombrar.")
        return

    print(f"\n{len(commits)} commits para renombrar en tandas de {BATCH_SIZE}.")

    if ask("¿Ejecutar Fase 2 (renombrado)?") == "n":
        return

    all_mapping = {}
    expected_hashes = [h for h, _ in commits]
    total_batches = (len(commits) + BATCH_SIZE - 1) // BATCH_SIZE

    idx = 0
    while idx < len(commits):
        batch = commits[idx:idx + BATCH_SIZE]
        batch_num = idx // BATCH_SIZE + 1

        hr()
        print(f"\n  Tanda {batch_num}/{total_batches}  ({len(batch)} commits)")

        print("\n📋 COPIA Y PEGA ESTE PROMPT EN LA IA:\n")
        hr("·")
        print(build_rename_prompt(batch))
        hr("·")

        raw = read_multiline("\n⬇  Respuesta de la IA:")
        mapping = parse_rename_response(raw, expected_hashes)

        if not mapping:
            print("⚠  No se parseó ninguna línea. ¿Reintentar tanda?")
            if ask("¿Reintentar?") == "s":
                continue

        missing = [h for h, _ in batch if h not in mapping]
        if missing:
            print(f"⚠  Faltan {len(missing)} hash(es):")
            for h in missing:
                orig = next(m for hh, m in batch if hh == h)
                print(f"   {h[:8]}  {orig[:60]}")
            if ask("¿Continuar de todas formas?") == "n":
                continue

        all_mapping.update(mapping)
        print(f"✔  Tanda {batch_num} lista: {len(mapping)} mensajes.")
        idx += BATCH_SIZE

    if not all_mapping:
        print("❌ Sin mapeo generado. Abortando.")
        return

    # resumen
    hr()
    print(f"  {len(all_mapping)} commits listos para reescribir:")
    for h, msg in list(all_mapping.items())[:5]:
        print(f"  {h[:8]}  →  {msg[:60]}")
    if len(all_mapping) > 5:
        print(f"  ... y {len(all_mapping)-5} más")

    if ask("¿Ejecutar reescritura del historial?") == "n":
        write_filter_script(all_mapping)
        print(f"Script guardado en {FILTER_SH} — ejecútalo manualmente.")
        return

    stashed = stash_tracked()
    delete_original_refs()
    write_filter_script(all_mapping)
    run_filter_branch()
    if stashed:
        pop_stash()
    delete_original_refs()

    remaining = non_conventional()
    if remaining:
        print(f"\n⚠  Quedan {len(remaining)} sin normalizar.")
    else:
        print("\n✔  Todos los commits siguen Conventional Commits.")

# ══════════════════════════════════════════════════════════════════════════════
#  MAIN
# ══════════════════════════════════════════════════════════════════════════════

def main():
    box("normalize_commits.py — Normalización completa del historial Git")
    print("""
  Fase 1: Dividir commits que mezclan cambios distintos
  Fase 2: Renombrar mensajes al formato Conventional Commits
""")

    try:
        run(["git", "rev-parse", "--git-dir"])
    except Exception:
        print("❌ No estás en un repositorio Git.")
        sys.exit(1)

    total = len(non_conventional())
    if total == 0:
        print("✔  El historial ya sigue Conventional Commits completamente.")
        sys.exit(0)

    print(f"  Commits a normalizar: {total}")
    input("\n  Presiona Enter para comenzar...\n")

    # ── Fase 1
    phase1_splits()

    # ── Fase 2
    phase2_renames()

    # ── Force push
    hr("═")
    remaining = non_conventional()
    if not remaining:
        if ask("¿Hacer force push a origin/master?") == "s":
            r = subprocess.run(
                ["git", "push", "--force", "origin", "master"],
                cwd=REPO_ROOT, capture_output=True, text=True
            )
            print(r.stdout or r.stderr)
    else:
        print(f"⚠  Quedan {len(remaining)} commits sin normalizar — no se hizo push.")

    print("\n✅ Proceso completado.")

if __name__ == "__main__":
    main()
