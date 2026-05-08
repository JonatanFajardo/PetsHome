#!/usr/bin/env python3
import sys, re
HELPERS = "c:/Users/nayel/OneDrive/Documentos/GitHub/001 Proyectos Estables/PetsHome/.rebase_helpers"
todo = sys.argv[1]
with open(todo, encoding='utf-8') as f: lines = f.readlines()
out = []
for line in lines:
    s = line.rstrip()
    if re.match(r'^pick\s+c591409', s):
        out.append(line)
        out.append('exec bash "' + HELPERS + '/split_c591409.sh"\n')
        continue
    out.append(line)
with open(todo, 'w', encoding='utf-8') as f: f.writelines(out)
