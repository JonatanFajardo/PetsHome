#!/usr/bin/env python3
import sys, re
HELPERS = "c:/Users/nayel/OneDrive/Documentos/GitHub/001 Proyectos Estables/PetsHome/.rebase_helpers"
todo = sys.argv[1]
with open(todo, encoding='utf-8') as f: lines = f.readlines()
out = []
for line in lines:
    s = line.rstrip()
    if re.match(r'^pick\s+c0b0faf', s):
        out.append(line)
        out.append('exec bash "' + HELPERS + '/split_c0b0faf.sh"\n')
        continue
    out.append(line)
with open(todo, 'w', encoding='utf-8') as f: f.writelines(out)
