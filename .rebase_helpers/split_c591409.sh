#!/bin/bash
set -e

ORIG_AUTHOR_DATE=$(git log --format="%ai" -n 1 HEAD)
ORIG_COMMITTER_DATE=$(git log --format="%ci" -n 1 HEAD)

git reset HEAD~1

# 1/2: docs(implementacion): actualizar documentación del módulo
git add "IMPLEMENTACION_MODULO.md"
GIT_AUTHOR_DATE="$ORIG_AUTHOR_DATE" GIT_COMMITTER_DATE="$ORIG_COMMITTER_DATE" git commit -m "docs(implementacion): actualizar documentación del módulo"

# 2/2: fix(controllers): corregir condición invertida en Insert y Update
git add -u
GIT_AUTHOR_DATE="$ORIG_AUTHOR_DATE" GIT_COMMITTER_DATE="$ORIG_COMMITTER_DATE" git commit -m "fix(controllers): corregir condición invertida en Insert y Update"
