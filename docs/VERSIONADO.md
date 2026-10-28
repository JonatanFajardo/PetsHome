# Versionado de PetsHome (SemVer + tags de Git)

Guía de cómo se versiona PetsHome en GitHub. **Léela antes de crear un tag nuevo.**

---

## Política: Versionado Semántico (SemVer)

Formato: **`vMAYOR.MINOR.PATCH`** (ej. `v1.3.0`).

| Parte | Cuándo subirla | En ESTE proyecto significa… |
|-------|----------------|------------------------------|
| **MAYOR** (`v2.0.0`) | Algo se **rompe** / cambia el contrato | Cambia la navegación (menús/URLs), se quitan pantallas, cambia la forma de datos/permisos de modo incompatible. Ej: refactor contextual (mover CRUD global a pestañas) |
| **MINOR** (`v1.1.0`) | Funciones **nuevas** que conviven con lo viejo | Un módulo/pantalla nuevo que no rompe lo existente. **La mayoría de nuestro trabajo cae aquí** (clínica, laboratorio, cobros, etc.) |
| **PATCH** (`v1.0.1`) | Correcciones | Bug fixes, ajustes menores sin funciones nuevas |

**Regla de oro:** aditivo = MINOR · romper algo = MAYOR · arreglar = PATCH.

### Fase beta / desarrollo (`0.x`)
Todo lo anterior a la primera versión estable es `0.x` (desarrollo, "nada garantizado estable"). La `1.0.0` marca el primer release estable.

### Pre-releases (opcional)
Para avances no finales: sufijo `-beta.N` o `-rc.N`. Ej: `v1.4.0-beta.1`, `v2.0.0-rc.1`.

---

## Historia de versiones (tags)

```
BETA / DESARROLLO (0.x)
  v0.1.0  2022-03  Esqueleto: CRUD de refugio (mascotas, empleados, catálogos)   → 0177dca
  v0.5.0  2025-11  Módulo médico BETA (citas, recetas, tratamientos)              → 9364f89
  v0.9.0  2026-06  Candidata: permisos por pantalla, perfil médico, dashboards   → d83aa8d

ESTABLES (1.x)  ← todo aditivo
  v1.0.0  2026-10-04  Primera estable: refugio + roles + médico + dashboards      → 18b552d
  v1.1.0  2026-10-21  Clínica veterinaria (clientes, POS, En Consulta, recepción) → 5fc52f7
  v1.2.0  2026-10-27  Mejoras v3 (banderas rojas, snapshot, cobros, tablero, agenda)→ df3efd9
  v1.3.0  2026-10-27  Módulos clínicos (SOAP, Lab, Controladas, Consentimientos,
                      Lista de espera)   ← VERSIÓN ACTUAL                          → 58b932f

FUTURO
  v2.0.0  (pendiente)  Refactor CONTEXTUAL → cambia navegación = MAYOR. Ver docs/ROADMAP_FUTURO.md
```

**Versión actual: `v1.3.0`.**

---

## Cómo crear un tag nuevo (a futuro)

Flujo recomendado: **se taggea sobre `master` ya estable** (compila, probado), normalmente justo **después de mergear** una tanda de features.

```bash
# 1. Asegúrate de estar en master actualizado y que compila
git checkout master

# 2. Decide el salto según SemVer:
#    - ¿solo funciones nuevas que no rompen? -> MINOR  (1.3.0 -> 1.4.0)
#    - ¿rompiste navegación/URLs/datos?       -> MAYOR  (1.x   -> 2.0.0)
#    - ¿solo correcciones?                    -> PATCH  (1.3.0 -> 1.3.1)

# 3. Crea el tag ANOTADO (con mensaje descriptivo)
git tag -a v1.4.0 -m "v1.4.0: <qué trae esta versión, en 1-2 líneas>"

# 4. Publícalo en GitHub (aparece en la pestaña Releases)
git push origin v1.4.0
# (o todos a la vez)  git push origin --tags
```

Tips:
- Un tag apunta a un commit concreto. Para taggear un commit pasado: `git tag -a vX.Y.Z <hash> -m "..."`.
- Ver tags: `git tag -l`. Ver detalle: `git show v1.4.0`.
- Borrar un tag mal puesto: `git tag -d vX.Y.Z` (local) y `git push origin :refs/tags/vX.Y.Z` (remoto).

### Automatización (opcional, a futuro)
Si se quiere automatizar: usar **GitHub Actions** con una herramienta como `release-please` o `semantic-release`, que leen los mensajes de Conventional Commits (`feat:` → minor, `fix:` → patch, `feat!:`/`BREAKING CHANGE` → major) y crean el tag + release notes solos. Requiere configurar un workflow; por ahora el flujo manual de arriba es suficiente.

> Nota de commits: usamos **Conventional Commits** (`feat`, `fix`, `chore`, `docs`…) separados por tipo y **sin autoría de Claude**. Eso hace que la automatización futura sea directa.
