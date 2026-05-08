#!/bin/bash
set -e

ORIG_AUTHOR_DATE=$(git log --format="%ai" -n 1 HEAD)
ORIG_COMMITTER_DATE=$(git log --format="%ci" -n 1 HEAD)

# Remove accidentally committed temp files
git rm --cached --ignore-unmatch \
    ".rebase_helpers/seq_editor.py" \
    ".rebase_helpers/split_126cede.sh" \
    ".rebase_helpers/split_2e87d2d.sh" \
    ".rebase_helpers/split_68f7c1a.sh" \
    ".rebase_helpers/split_7d0efc6.sh" \
    ".rebase_helpers/split_8cb5aea.sh" \
    ".rebase_helpers/split_935e116.sh" \
    ".rebase_helpers/split_a64690b.sh" \
    ".rebase_helpers/split_ae61dc6.sh" \
    ".rebase_helpers/split_b8fd368.sh" \
    ".rebase_helpers/split_bd87e10.sh" \
    ".rebase_helpers/split_c591409.sh" \
    ".rebase_helpers/split_fd8c195.sh" \
    "SEGURIDAD_ESTADO.md" \
    "commits.txt" \
    "commits_full.txt" \
    "commits_last10.txt" \
    "commits_nodiff.txt" \
    "commits_stat.txt" \
    "commits_worddiff.txt" \
    "rewrite_history.py" || true

GIT_AUTHOR_DATE="$ORIG_AUTHOR_DATE" GIT_COMMITTER_DATE="$ORIG_COMMITTER_DATE" git commit --amend --no-edit
