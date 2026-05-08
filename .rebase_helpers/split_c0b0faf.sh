#!/bin/bash
set -e

ORIG_AUTHOR_DATE=$(git log --format="%ai" -n 1 HEAD)
ORIG_COMMITTER_DATE=$(git log --format="%ci" -n 1 HEAD)

git reset HEAD~1

# 1/2: chore(config): actualizar connection string en appsettings
git add "PetsHome.UI/appsettings.json"
GIT_AUTHOR_DATE="$ORIG_AUTHOR_DATE" GIT_COMMITTER_DATE="$ORIG_COMMITTER_DATE" git commit -m "chore(config): actualizar connection string en appsettings"

# 2/2: style(home): redisenar layout, estilos y scripts del dashboard principal
git add -u
GIT_AUTHOR_DATE="$ORIG_AUTHOR_DATE" GIT_COMMITTER_DATE="$ORIG_COMMITTER_DATE" git commit -m "style(home): redisenar layout, estilos y scripts del dashboard principal"
