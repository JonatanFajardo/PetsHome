
## Git commits
Cuando vayas a hacer un commit :

1. Analiza todos los cambios staged
2. Identifica si los cambios pertenecen a más de un tipo (feat, fix, refactor, etc.)
3. Si mezclan tipos → divídelos en commits separados, uno por tipo
4. Aplica el formato Conventional Commits a cada uno

**Formato:** `<tipo>(<scope>): <descripción en minúscula>`
**Tipos:** feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert
**Scope:** módulo afectado (auth, pets, api…)
**Reglas:**
- Descripción corta, imperativa, sin punto final
- Un commit = un solo tipo
- Si el cambio rompe la API, agrega `BREAKING CHANGE:` en el footer

**Ejemplo — cambios mezclados:**
En vez de:
  feat(ViewModels): agregar validaciones y eliminar código redundante

Hacer dos commits:
  feat(ViewModels): agregar validaciones de rango, formato y longitud
  refactor(HistorialMedicoViewModel): eliminar validación redundante en X

  NO QUIERO NINGUNA MENCION O AUTORIA TUYA O DE TU PARTE.