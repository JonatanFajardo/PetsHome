# Mejoras v3 — Paquete clínico-operativo

> **Rama:** `feature/mejoras-v3`
> **Origen:** ideas extraídas de **OpenVPM** (PIMS veterinario open-source, clonado al lado del repo) + principios de `CLAUDE.md`.
> **Fecha inicio:** 2026-10-21

OpenVPM es el **referente** (modelo de datos + UI). NO se copia su código (es Next.js/PostgreSQL); se copian **patrones y conceptos** adaptados a nuestro stack (ASP.NET MVC + SQL Server + Dapper + SPs).

---

## Alcance de esta etapa (el paquete)

Cuatro áreas, no solo clínica:

```
  Clínica    → alergias/crónicos (banderas rojas), ficha con tabs
  Inventario → marca de origen anti doble-descuento, snapshots
  Ventas/POS → cola dispensar→cobrar, factura de venta 'V'
  Recepción  → whiteboard/sala de espera tipo Kanban
```

Orden sugerido de implementación:

```
1. Clínica     (banderas rojas reales — desbloquea seguridad clínica, bajo riesgo)
2. Inventario  (anti doble-descuento + snapshots — endurece lo que ya existe)
3. Ventas/POS  (cola dispensar→cobrar — cierra el hueco de la venta 'V')
4. Recepción   (whiteboard kanban — mejora visual sobre sala de espera actual)
```

---

## Principios que gobiernan estas mejoras (de CLAUDE.md)

- **Mínima interferencia** — extender lo que ya existe (ficha PerfilMedico ya usa tabs; sala de espera ya existe), no reconstruir.
- **Aditivo, no reemplazo** — columnas y tablas nuevas que se suman; lo viejo sigue vivo.
- **Opcional y degradable** — toda mejora apagable vía `tbModulosConfig`; sistema funciona sin ella.
- **Feed, no gate** — recepción/cobro rellenan colas; nunca bloquean al veterinario.
- **Separar dominios** — interno (refugio) vs cliente externo vs adoptante; no conflar.
- **Un dato, varias lentes** — una fuente de verdad (ej. cola del día) que cada rol ve distinto.
- **Analizar rol × función ANTES de construir → filtros + permisos** — para TODA mejora/pantalla, primero armar la matriz rol × qué-hace × nivel (★). De ahí se derivan: (a) los **filtros** que necesita cada rol (p. ej. el vet ve solo su columna; recepción ve todo), y (b) los **permisos en la BD** (`tbPantallas` + `tbRolesPantallas` con ver/insertar/editar/eliminar) para que cada rol opere como más le convenga. No construir "para todos igual"; construir "cada rol su lente", y reflejarlo en datos (filtros en SP/consulta) y en seguridad (permisos). Aplicarlo cuando aplique (si una mejora es transversal a roles).
- **Adaptarnos a la DB de OpenVPM** — cuando modelemos algo que OpenVPM ya resolvió, replicar SU esquema (nombres de campos, enums, relaciones) traducido a nuestra convención (`tb{Entidad}`, prefijos, `_EsEliminado` + auditoría). No inventar un modelo paralelo. Verificar siempre el schema real en `openvpm/packages/db/schema/*.ts` antes de diseñar.

---

## ⚙️ Protocolo de trabajo — AUTO-REGISTRO (importante)

> **Regla permanente para Claude:** este `.md` es la memoria viva del proyecto. Las cosas que se deciden en el chat se olvidan; las que se escriben aquí, no.

Claude DEBE, sin que se lo pidan:
1. **Registrar en la Bitácora** (abajo) toda decisión tomada en el chat: modelo elegido, cambio de alcance, nombre de rama, convención acordada, hallazgo de OpenVPM que vamos a copiar, y cualquier corrección de un supuesto previo.
2. **Guardar hallazgos de verificación** — cuando se revisa el esquema/código real (de OpenVPM o de PetsHome), anotar el dato concreto (archivo + qué se encontró) para no re-investigarlo después.
3. **Actualizar las secciones de fase** a medida que avanzan (marcar checklists, detallar lo que estaba "pendiente").
4. Hacerlo **en el momento**, no al final de la sesión. Cada vez que el usuario confirme algo o se descubra un dato clave → escribirlo aquí antes de seguir.

> Esto se complementa con la memoria persistente de Claude Code (`MEMORY.md`), que guarda el hábito entre sesiones.

---

## 🔎 Estándar de verificación (OBLIGATORIO antes de marcar [VERIFICADO])

> Lección 2026-10-22: las clases C# (`*.cs`) están **desactualizadas** respecto a la DB real. `tbMascotas.cs` tenía 18 props; la DB viva tiene 23 columnas (faltaban `raza_Id, proc_Id, masc_IngresoId, tall_Id, masc_Origen, dueno_Id, cli_Id`).

Regla: **ninguna tabla se diseña ni se marca [VERIFICADO] leyendo el `.cs`.** Siempre volcar columnas REALES desde LocalDB:
```
sqlcmd -S "(localdb)\MSSQLLocalDB" -d PETSHOMEDB  → sys.columns de la tabla
```
Esto aplica a toda tabla que toquemos, en cada fase, antes de escribir SQL.

## ⚠️ Deuda técnica

- ✅ **RESUELTO (2026-10-22) — receta_Estado normalizado.** Set canónico femenino: `Activa | Completada | Cancelada | Vencida`. Script `68_RECETA_ESTADO_NORMALIZAR.sql` (migración datos + CHECK `CK_tbRecetas_Estado`). Código unificado: default del insert → 'Activa' (scripts 13 **y 52** re-desplegados), lectores que filtraban 'Activo' corregidos y re-desplegados (19 AlertaMedica_RecetasSinRevision, 23 PerfilMedico_MedicamentosActivos). Verificado en DB viva: 0 SPs desplegados filtran por 'Activo'. (Nota: el agente dejó el script 52 sin corregir por límite de sesión; se completó manualmente.)
- ✅ **RESUELTO (2026-10-22) — saldet_RecetaId wiring.** `ConsultaService.GuardarAsync` construye un mapa línea→recetaId (paso 3) y lo pasa a `InsertSalidaDetalleAsync` (paso 4, repo ampliado con `saldetRecetaId` opcional) para TODAS las líneas entregadas. El encolado de cargo (cliente externo) reusa el mismo receta_Id. Probado en transacción: la salida queda con `saldet_RecetaId` + snapshot poblados.
- ✅ **RESUELTO (2026-10-22) — ISV 15% en factura.** SP `FacturarCliente` aplica `impuesto = ROUND(subtotal*15/100,2)`, `total = subtotal+impuesto` (misma convención que script 43 / FacturacionService). Probado: 1700 → 255 → 1955.
- ◐ Pendiente menor: prueba visual autenticada (login) de las 4 pantallas.

## 📒 Bitácora de decisiones

| Fecha | Decisión / hallazgo | Detalle |
|-------|---------------------|---------|
| 2026-10-22 | **Regla: rol × función → filtros + permisos** | Antes de construir CUALQUIER cosa, analizar la matriz rol×función×nivel. De ahí salen los filtros por rol (lentes) y los permisos en BD (`tbPantallas`/`tbRolesPantallas`). Construir "cada rol su lente", reflejado en datos (filtros SP) y seguridad. Ver Principios |
| 2026-10-22 | Manual de cliente creado | `docs/MANUAL_USUARIO_V3.md` — guía en lenguaje humano (sin columnas/tecnicismos) de las 4 novedades + agenda por rol, con "qué hace / para qué sirve a cada rol". |
| 2026-10-22 | **Agenda: lente por rol (1ra aplicación de la regla)** | El calendario YA existía (`CitaMedica/Calendario`, FullCalendar custom). NO son pantallas nuevas. Se agregó LENTE por rol: SP `69_CALENDARIO_LENTE_VETERINARIO.sql` (+`@vet_Id` + devuelve veterinario). Vet (rol 4) ve SOLO sus citas + sin asignar (forzado, no puede espiar otras); admin/recepción ven todo o filtran por doctor (dropdown en sidebar que reusa `/Recepcion/Veterinarios`). Capas: Result+VM+repo+service+controller (`ResolverVetLente`) + JS (`window.calFiltrarDoctor`). Permisos: reusa pantalla "Listado de citas medicas" (sin pantalla nueva). Compila 0 errores. PENDIENTE (pieza grande, aparte): columnas-por-doctor VISUALES + drag-to-reschedule (rework del render custom). |
| 2026-10-21 | Rama de trabajo | `feature/mejoras-v3` desde `master`, abarca TODO el paquete (no solo clínica) |
| 2026-10-21 | OpenVPM = referente | Clonado en `../openvpm` (hermano del repo). Copiamos patrones/modelo, NO código (es Next.js/PostgreSQL) |
| 2026-10-22 | Las 4 áreas son MEJORAS, no módulos nuevos | Inventario (=Items), Recepción, Ventas/POS ya existen. Solo Clínica/banderas rojas es parte nueva de verdad |
| 2026-10-22 | Alergias/crónicos = modelo estructurado (Opción B) | Adaptado al esquema real de OpenVPM, no texto libre |
| 2026-10-22 | Corrección de supuesto | OpenVPM NO usa ledger append-only para alergias: usa **soft-delete** (`deletedAt`). Equivale a nuestro `_EsEliminado`. Fuente: `openvpm/packages/db/schema/patients.ts:138` y `clinical.ts:598` |
| 2026-10-22 | Adaptarnos a la DB de OpenVPM | Nuevo principio permanente (ver arriba) |
| 2026-10-22 | Nivel de adaptación = **B (fuerte)** | Alinear a la forma de OpenVPM todo el DOMINIO CLÍNICO, fase por fase. Ignorar su capa SaaS (telemetría/stripe/SMS) |
| 2026-10-22 | **Por qué B** (contexto del usuario) | PetsHome se diseñó según cómo el usuario *imaginaba* que funcionan estos sistemas, sin certeza del dominio real. OpenVPM lo construyó gente con experiencia veterinaria → se toma su modelo como la referencia "correcta" del negocio. Guía: preferir SU estructura sobre suposiciones propias cuando haya duda de dominio |
| 2026-10-22 | Tamaño DBs | OpenVPM ~124 tablas / 106 enums (pero ~60-70 son ruido SaaS); núcleo clínico ~45-55. PetsHome 38 tablas + 209 SPs. Su fuerte = normalización clínica (datos que PetsHome guarda como texto) |
| 2026-10-22 | Enums → CHECK | SQL Server no tiene enums nativos: replicar los `pgEnum` de OpenVPM como `nvarchar` + `CHECK` constraint (ej. severidad, estado de condición) para ser fieles a su modelo |
| 2026-10-22 | **Convención de naming: entidad primero** | Tablas hijas lideran con la entidad padre: `tbMascotaAlergia`, `tbMascotaPadecimiento` (NO `tbAlergiaMascota`). Agrupa visualmente/alfabéticamente todas las `tbMascota*` → fácil de encontrar. Regla general a seguir en todo el proyecto |
| 2026-10-22 | Vocabulario propio (anti-calco) | Conservar el diseño de OpenVPM pero con palabras PetsHome: "padecimiento", "sustancia", "gravedad". No es plagio (las ideas no tienen copyright; su código AGPL no se toca), pero usamos léxico propio |
| 2026-10-22 | Gravedad de alergia = **Leve/Media/Alta** | Confirmado por el usuario |
| 2026-10-22 | Hallazgo verificado: recetas de OpenVPM | `prescriptions.ts`: mantiene `medicationName` (texto) JUNTO a `productId` (FK) = texto-snapshot + FK, NO reemplaza el texto. Usa `startDate/endDate` (fechas), NO "duración". Tiene `dosage, frequency, quantity, refillsRemaining, instructions, operationId`. → Normalizar recetas es ADITIVO sobre el texto existente, no sustituirlo |
| 2026-10-22 | **Verificación 3 agentes: Fases 2-4 al 100%** | Cruce esquema real OpenVPM ↔ BD viva PetsHome. Resultados abajo |
| 2026-10-22 | Fase 2 corregido | Prefijo real `saldet_` (no sdet_). PetsHome YA evita doble-descuento (triggers fuente única, scripts 51/58); la receta no descuenta. Solo adoptar: snapshot precio/descr + `saldet_RecetaId` con uq filtrado (idempotencia). Lote/caducidad/costo NO aplican (no modelados). Stock es INT (sin fracciones) |
| 2026-10-22 | Fase 3 corregido | `tbRecetas` YA ~90% normalizada (dosis/frecuencia/fechas/instrucciones/estado/itm_Id/medicamento existen). Solo falta `receta_Cantidad` INT + `receta_OperacionId` + CHECK estado. `tbCargoDispensacion` = tabla nueva (corazón). `quantity` es INT en el SQL real (no numeric). OMITIR prescription_events (sobre-ingeniería), refills (sin flujo), capa SaaS. tbFacturas no tiene cita_Id |
| 2026-10-22 | Fase 4 CONFIRMADO: cero esquema | Whiteboard OpenVPM = 100% derivado de appointments. `tbCitaMedica` ya tiene estado/prioridad/HoraLlegada/recepcionista/veterinario. Estados mapean 1:1 (En espera/En atencion/Finalizada → 3 columnas). Solo 1 SP nuevo (ampliar WHERE para incluir 'Finalizada'). UI drag→CambiarEstado (ya existe). CERO ALTER TABLE |
| 2026-10-22 | **Estándar: verificar contra DB VIVA, no el `.cs`** | Las clases C# están desactualizadas (ver sección "Estándar de verificación"). Toda tabla se vuelca con sqlcmd antes de diseñar |
| 2026-10-22 | Fase 1 verificada en vivo | `tbMascotas` real: PK `masc_Id` int; auditoría `masc_EsEliminado/UsuarioCrea/FechaCrea(def getdate)/UsuarioModifica/FechaModifica`. Hallazgo: tiene `dueno_Id` Y `cli_Id` a la vez (matiz de dominio a revisar). Las tablas hijas nuevas usan `masc_Id` como FK y replican ese patrón de auditoría |
| 2026-10-22 | Fases 2/3 re-verificadas en vivo (confirmadas) | `tbRecetas` vivo: NO tiene `receta_Cantidad` ni `operacion_Id` → confirma los 2 ADD + CHECK estado (default ya es 'Activo'). SORPRESA POSITIVA: ya tiene `tipoMed_Id` + `viaAdmin_Id` (tipo de medicamento + vía de administración) → más normalizada que lo asumido. Dosis/Frecuencia son nvarchar(200), Duracion nvarchar(100). `tbSalidasDetalles`: prefijo `saldet_`, cantidad INT, sin snapshot (confirma Fase 2). `tbFacturas`: SIN `cita_Id` (confirmado), estado def 'Emitida'. `tbFacturaDetalle`: `fdet_Descripcion` varchar(250), tiene `fdet_Tipo`/`serv_Id`/`itm_Id`, SIN marca de origen → Fase 3 añade el vínculo vía tbCargoDispensacion |
| 2026-10-22 | **ISV 15% aplicado en FacturarCliente (script 66)** | ✅ RESUELTA la deuda de "factura con impuesto=0". Reusada la convención existente (script 43 + `FacturacionService.cs const ISV=15.00m`): `DECLARE @impuestoPorc DECIMAL(5,2)=15.00` → `fact_Impuesto=ROUND(subtotal*15/100.0,2)`, `fact_Total=subtotal+impuesto`. Columnas verificadas en vivo: las 3 son DECIMAL(18,2). Probado end-to-end en TRAN (encolar 2×850 → facturar → ROLLBACK): subtotal 1700.00 · impuesto 255.00 · total 1955.00 = CUADRA. Solo SP; C# intacto. Comentario deja la puerta a parametrizar la tasa a futuro |

---

# 1. CLÍNICA — banderas rojas + ficha con tabs

### Qué
- **Banderas rojas reales**: alergias y enfermedades crónicas visibles en la cabecera de la ficha y de "En Consulta".
- **Ficha con tabs**: extender la ficha de `PerfilMedico` (que ya usa tabs) con la estructura tipo OpenVPM.

### Por qué
Hoy las "banderas rojas" son placeholder (meds activos + vacunas vencidas). `tbMascotas` **no tiene** columnas de alergia ni crónicos → riesgo clínico real (recetar algo a lo que el animal es alérgico).

### Referente OpenVPM
- Ficha de paciente con banner de alergias arriba ("No known allergies recorded · + Add allergy").
- Tabs: `Overview · Medical Records · Appointments · Weight History · Vitals · Vaccinations · Invoices`.

### Modelo — adaptado al esquema REAL de OpenVPM

Fuente verificada: `openvpm/packages/db/schema/patients.ts:138` (patient_allergies) y `clinical.ts:598` (problem_list).

Naming: **entidad primero** (`tbMascota*`) + **vocabulario propio** (no calco del inglés).

```
OpenVPM patient_allergies          →  PetsHome tbMascotaAlergia   (prefijo alrg_)
──────────────────────────────────────────────────────────────────
allergen   varchar(255) NOT NULL   →  alrg_Sustancia   nvarchar(255) NOT NULL
reaction   text         NULL       →  alrg_Reaccion    nvarchar(max) NULL
severity   enum = moderate         →  alrg_Gravedad    nvarchar(20)  CHECK (Leve|Media|Alta) def 'Media'
notedBy / notedAt                  →  alrg_UsuarioCrea / alrg_FechaCrea   (auditoría estándar)
deletedAt (soft-delete)            →  alrg_EsEliminado  (corregir = marcar, NO borrar)
                                      + masc_Id, auditoría estándar

OpenVPM problem_list               →  PetsHome tbMascotaPadecimiento  (prefijo pade_)
──────────────────────────────────────────────────────────────────
description varchar(500) NOT NULL   →  pade_Nombre      nvarchar(500) NOT NULL
status enum (active|resolved|chronic)→ pade_Estado  nvarchar(20) CHECK (Vigente|Superado|Crónico) def 'Vigente'
onsetDate / resolvedDate            →  pade_Desde / pade_Hasta   date NULL
                                      + masc_Id, auditoría estándar
```

Valores de `alrg_Gravedad` = **Leve / Media / Alta** (confirmado 2026-10-22).

Decisión: **replicar el esquema de OpenVPM tal cual**, usando nuestro `_EsEliminado` como el soft-delete que "conserva sin borrar". NO texto libre, NO ledger de eventos nuevo.

Fidelidad a su modelo (adaptación B):
- Sus `pgEnum` se replican como `nvarchar(20)` + **`CHECK`** constraint:
  `CHECK (alrg_Severidad IN ('Leve','Moderada','Severa'))` y `CHECK (cond_Estado IN ('Activa','Resuelta','Crónica'))`.
- Se conserva su semántica "quién/cuándo lo registró clínicamente" (notedBy/notedAt) → mapeado a `alrg_UsuarioCrea/alrg_FechaCrea` (en PetsHome la auditoría ya cumple ese rol).
- `severity` arranca con default **Moderada** (igual que su default `moderate`).

### UI
```
┌─ Ficha Mochi ───────────────────────────────────────┐
│ 🔴 ALERGIAS: Penicilina   ⚠ CRÓNICO: Insuf. renal   │ ← banner rojo
├──────────────────────────────────────────────────────┤
│ Resumen · Historial · Citas · Peso · Vacunas · Recetas│ ← tabs (ya existen)
└──────────────────────────────────────────────────────┘
```
Mismo banner embebido en "En Consulta" como bandera roja real.

### Estado
- [ ] Script SQL: columnas/ts en `tbMascotas` (o tablas normalizadas)
- [ ] SP: incluir alergias/crónicos en el detalle de mascota
- [ ] Entidad C# `tbMascotas.cs` + ViewModels
- [ ] Banner en ficha `PerfilMedico/Index`
- [ ] Banner en `Consulta/Index` (banderas rojas reales)
- [ ] Re-login si toca pantalla nueva

---

# 2. INVENTARIO — marca de origen + snapshots *(pendiente de detallar)*

### Qué
- **Marca de origen** en la línea de salida para no descontar stock dos veces.
- **Snapshots** de precio/descripción al dispensar (congelar el valor histórico).

### Referente OpenVPM
`invoice_items.sourcePrescriptionId` / `sourceDispenseChargeId` → filtra líneas ya descontadas. Snapshots inmutables de precio en `dispense_charge_queue`.

> Detalle de modelo y pasos: **pendiente** (se desarrolla al llegar a esta área).

---

# 3. VENTAS/POS — cola dispensar→cobrar + factura 'V' *(pendiente de detallar)*

### Qué
Desacoplar **descontar stock** (lo hace el vet al dispensar) de **cobrar** (lo hace recepción/POS después), vía una cola de cargos pendientes. Cierra el hueco actual: la venta tipo 'V' no genera factura automática.

### Referente OpenVPM
`dispense_charge_queue` (pending → invoiced | waived), reapertura automática al anular factura, idempotencia por `operation_id`.

> Detalle de modelo y pasos: **pendiente**.

---

# 4. RECEPCIÓN — whiteboard/sala de espera Kanban *(pendiente de detallar)*

### Qué
Convertir la sala de espera (hoy lista) en un **tablero Kanban de 3 columnas** con tiempo transcurrido por paciente.

### Referente OpenVPM
Whiteboard: `Waiting · In Progress · Completed`, tarjeta = mascota + doctor + "in 8h 6m". Es la misma fuente de verdad (citas de hoy) vista como tablero.

> Detalle de modelo y pasos: **pendiente**.

---

# Resumen de cambios en la Base de Datos (todo el paquete)

Leyenda: 🆕 tabla nueva · ➕ columnas nuevas (ALTER) · ⚙️ SP nuevo · 🔁 normalización (cambia la forma de algo existente).
Estado: **[FIJO]** = confirmado y especificado · **[TENT.]** = tentativo, se detalla al llegar a esa fase (verificar contra OpenVPM antes).

### Fase 1 — Clínica  **[FIJO · tareas 1-3 HECHAS 2026-10-22]**
> Secuencia: Opción B (fase por fase, DB→app).
> - **Tarea 1 (SQL tablas):** `Database/61_MASCOTA_ALERGIA_PADECIMIENTO.sql` — `Medico.tbMascotaAlergia` + `Medico.tbMascotaPadecimiento` creadas y verificadas en DB viva (columnas, CHECK, FKs, índices filtrados).
> - **Tarea 2 (SPs):** `Database/62_SP_MASCOTA_ALERGIA_PADECIMIENTO.sql` — 6 SPs (List/Insert/Corregir × 2), patrón CodeStatus/MessageStatus, probados end-to-end (insert→list→corregir).
> - **Tarea 3 (backend):** entidades + Result + ViewModels + repos + services + registros (ServiceConfiguration + MappingProfileExtensions). **Compila OK (0 errores).**
> - **DECISIÓN encoding:** estado crónico se guarda SIN tilde (`'Cronico'`) para evitar mojibake (sqlcmd/codepage corrompió `'Crónico'` en el CHECK). La UI muestra "Crónico" vía `MascotaPadecimientoViewModel.EstadoDisplay`. Regla: evitar tildes en valores de enum almacenados.
> - **Tareas 4-5 (UI componente):** alert bar reutilizable cargado por AJAX. Archivos: `BanderaClinicaController` (Bar + Agregar/Corregir × 2), `Views/BanderaClinica/_AlertBarClinica.cshtml` (chips + form inline estilo OpenVPM), `_AlertBarHost.cshtml` (host con `window._urlsBandera`), `wwwroot/css/bandera-clinica.css` (morado PetsHome + rojo/ámbar), `wwwroot/js/pages/bandera-clinica.js`. **Compila OK.** Mínima interferencia: un solo componente, se incluye con `@await Html.PartialAsync("~/Views/BanderaClinica/_AlertBarHost.cshtml", (int)mascId)`.
> - **Tareas 6-8 (montaje):** host incluido en `PerfilMedico/Index` (bajo el header) y en `Consulta/Index` (reemplaza el placeholder "Sin alergias registradas" que tenía un TODO reconociendo la falta). Compila OK. Verificado en app real: arranque sin errores DI, HOME 200, ruta `/BanderaClinica/Bar` responde 302→login ([Authorize] activo). **FASE 1 funcional de punta a punta.**
> - Nota: los botones decorativos de header (📷 Fotos / ✍ Firma / ⬇ Resumen) del mockup NO se implementaron (no son funcionales aún); solo el alert bar real. Pendiente: prueba visual autenticada (login) y decidir si se agregan esos botones.
```
🆕 tbMascotaAlergia        (alrg_Sustancia, alrg_Reaccion, alrg_Gravedad CHECK[Leve|Media|Alta],
                            masc_Id, auditoría, alrg_EsEliminado)
🆕 tbMascotaPadecimiento   (pade_Nombre, pade_Estado CHECK[Vigente|Superado|Crónico],
                            pade_Desde, pade_Hasta, masc_Id, auditoría, pade_EsEliminado)
⚙️ SPs: Alergia  List/Insert/Corregir   ·   Padecimiento  List/Insert/Corregir
   (Corregir = marcar _EsEliminado, no borrar)
```

### Fase 2 — Inventario (snapshot + trazabilidad receta→salida)  **[HECHA 2026-10-22]**
> `Database/63_SALIDA_SNAPSHOT_TRAZABILIDAD.sql` ejecutado. `tbSalidasDetalles` + `saldet_PrecioUnitario` (CHECK≥0), `saldet_DescripcionItem`, `saldet_RecetaId` (FK + uq filtrado). SP `PR_Inventario_Consulta_SalidaDetalle_Insert` captura el snapshot AUTOMÁTICAMENTE desde `tbItems` (COALESCE) → **cero cambios en C#, el flujo de consulta ya lo usa**. Probado transaccional (ROLLBACK): snapshot precio 95.00 + descripción capturados; trigger de descuento intacto.
> - Snapshot de precio/descripción: **funcionando ya**.
> - `saldet_RecetaId` (traza receta→salida + idempotencia): columna lista, **wiring diferido a Fase 3** (donde vive la liga receta↔cargo; requiere que RecetaService.AddAsync devuelva el id).

Detalle original del diseño:
```
IMPORTANTE: PetsHome YA evita el doble-descuento con triggers de FUENTE ÚNICA (scripts 51/58).
La receta NO descuenta stock; lo hace el trigger de tbSalidasDetalles. Entonces de OpenVPM
solo vale adoptar: snapshot de precio/descripción + marca de origen con idempotencia.

➕ Inventario.tbSalidasDetalles (prefijo REAL = saldet_, NO sdet_):
   + saldet_RecetaId        INT NULL  FK→Medico.tbRecetas(receta_Id)   [trazabilidad + idempotencia]
   + saldet_PrecioUnitario  DECIMAL(18,2) NULL  CHECK (>=0)            [snapshot precio]  ★★★
   + saldet_DescripcionItem NVARCHAR(500) NULL                         [snapshot descripción] ★★
   + UNIQUE INDEX filtrado sobre saldet_RecetaId (no eliminado)        [1 salida por receta]
   El SP de inserción se EXTIENDE para setear los snapshots (el trigger NO los rellena).
   El trigger de descuento NO se toca (solo lee cantidad/itm_Id) → ALTER aditivo es seguro.

NO aplica a PetsHome (su inventario no lo modela): lote, caducidad, costo unitario.
Stock es INT por item+refugio (sin fracciones).
```

### Fase 3 — Ventas/POS (cola dispensar→cobrar)  **[HECHA 2026-10-22]**
> - **DB probada end-to-end:** `64` (tbRecetas +receta_Cantidad +receta_OperacionId +uq) · `65` (tbCargoDispensacion + CHECK 3 ramas + 5 SPs, Encolar auto-snapshot desde tbItems) · `66` (FacturarCliente: 1 factura con todos los cargos pendientes del cliente, transaccional). Probado: encolar→idempotencia→list→condonar→reabrir→facturar (factura 2550.00 = 3×850, cargo→Facturado). CHECK de receta_Estado DIFERIDO (deuda técnica Activa/Activo).
> - **Backend + wiring (compila):** entidad/Result/VM/repo/service/registros. `RecetaRepository.AddReturningIdAsync` (devuelve id). `ConsultaService.GuardarAsync`: al dispensar a CLIENTE EXTERNO (cli_Id set) captura receta_Id y encola el cargo (best-effort, nunca bloquea; stock se descuenta igual en paso 4).
> - **Pantalla POS:** `CobrosController` ([Authorize]) + `Views/Cobros/Index.cshtml` + `cobros.js` (lista pendientes, "Cobrar cliente" = FacturarCliente, "Condonar" con motivo). Link en sidebar (grupo Clínica, "💵 Cobros pendientes"). Verificado: app bootea, rutas 302→login.
> - **Notas v1:** ~~factura con impuesto=0~~ → ✅ ISV 15% aplicado (script 66, probado: 1700/255/1955); POS sin PantallaAuthorize (como el alert bar) → sin re-login; `saldet_RecetaId` (Fase 2) sigue sin wiring (la traza receta↔salida se puede cerrar luego ligándola al cargo).
> - Decisión scaffold: NO se usó `scaffold_backend.py` (genera CRUD estándar; la cola es custom). Hand-written.

Detalle original del diseño:
```
HALLAZGO: tbRecetas YA está ~90% normalizada. Ya tiene receta_Medicamento (texto) + itm_Id (FK) +
receta_Dosis + receta_Frecuencia + receta_FechaInicio/Fin + receta_Instrucciones + receta_Estado +
receta_Duracion. Confirma la premisa: OpenVPM conserva texto + FK juntos; PetsHome ya lo hace.

➕ Medico.tbRecetas (solo faltan 2-3):
   + receta_Cantidad     INT NULL                           [cantidad dispensada]  ★★★
   + receta_OperacionId  UNIQUEIDENTIFIER NULL + uq         [idempotencia dispensar] ★★★
   + CHECK receta_Estado IN ('Activo','Completado','Cancelado','Vencido')  [normaliza enum] ★★
   (quantity es INT en el SQL real de OpenVPM, NO numeric 13,3 → usar INT)

🆕 Veterinaria.tbCargoDispensacion  (el CORAZÓN de la fase — cola de cobro, NO toca stock):
   cargo_Id PK · receta_Id FK · masc_Id · cli_Id · cita_Id NULL · itm_Id · cargo_Cantidad
   cargo_DescripcionSnap NVARCHAR(500) · cargo_PrecioUnitSnap DECIMAL(18,2)   [snapshots]
   cargo_Estado NVARCHAR(20) CHECK(Pendiente|Facturado|Condonado)
   fact_Id NULL · fdet_Id NULL · cargo_ResueltoPor NULL · cargo_FechaResuelto NULL · cargo_MotivoResolucion NULL
   + UNIQUE INDEX filtrado sobre receta_Id    [1 cargo por receta — idempotencia]
   + CHECK de 3 ramas (coherencia estado↔campos resueltos, igual que OpenVPM)
⚙️ 4 SPs: Encolar (solo si cli_Id set) · Facturar (reusa Factura_Crear) · Condonar · Reabrir

❌ OMITIR (sobre-ingeniería / no aplica):
   • tbRecetaEvento (ledger inmutable) — auditoría estándar + estados + cola ya cubren el valor
   • refills_remaining — PetsHome no tiene flujo de resurtido hoy
   • legacy_review, practice_id/RLS, triggers de rechazo UPDATE/DELETE (son multi-tenant/SaaS)
Nota: tbFacturas NO tiene cita_Id (ligar cobro↔consulta usaría masc_Id+fecha, o agregar cita_Id).
```

### Fase 4 — Recepción (whiteboard Kanban)  **[HECHA 2026-10-22 — CERO cambios de esquema]**
> - **DB:** `67_WHITEBOARD_LIST.sql` → `PR_Medico_Whiteboard_List` (mismo shape que SalaEspera_List pero 3 estados: En espera/En atencion/Finalizada del día). SalaEspera_List intacto.
> - **Backend:** `RecepcionRepository.WhiteboardAsync` + `RecepcionService.WhiteboardAsync` (reusa el Result y el Map existentes). Controller: `Tablero()` (vista) + `WhiteboardData()` (JSON, reusa el lente por rol `ResolverVetLente`).
> - **UI Kanban:** `Views/Recepcion/Tablero.cshtml` + `whiteboard.css` (morado) + `whiteboard.js` (carga AJAX, auto-refresh 30s, drag-drop nativo entre columnas → `CambiarEstado` existente). Link en sidebar ("🗂️ Tablero del día"), gated por permiso "Recepcion" (sin pantalla nueva → sin re-login). Verificado: compila, app bootea, rutas 302→login.
> - Cero ALTER TABLE: tbCitaMedica ya tenía todo (estado/prioridad/HoraLlegada/vet).

Detalle original:
```
CONFIRMADO: el whiteboard de OpenVPM es 100% derivado de appointments (sin tablas propias).
tbCitaMedica YA tiene TODO lo necesario (verificado en LocalDB):
   cita_Estado · cita_Prioridad · cita_HoraLlegada (check-in real) · cita_Recepcionista · cita_Veterinario

Mapa de estados (match 1:1 con las 3 columnas del Kanban):
   "En espera"   → col 1 (Waiting)      "En atencion" → col 2 (In Progress)
   "Finalizada"  → col 3 (Completed)
Tiempo en sala: ya se calcula en SP con DATEDIFF(MINUTE, cita_HoraLlegada, GETDATE()).

➕ NINGÚN ALTER TABLE.
⚙️ 1 SP de lectura: variante de PR_Medico_SalaEspera_List con el WHERE ampliado para incluir
   'Finalizada' del día (hoy el SP solo trae 'En espera'/'En atencion').
UI: arrastrar tarjeta → PR_Medico_Visita_CambiarEstado (ya existe). Kanban reemplaza solo la vista.
```

### Cross-cutting (todo el paquete)
```
• Idempotencia: columna operacion_Id (única) en inserts críticos (dispensar, encolar, facturar)
• Enums de OpenVPM → nvarchar + CHECK en SQL Server
• Soft-delete _EsEliminado como "conservar sin borrar" en todas las tablas nuevas
```

---

# Universo completo — todo lo que tiene OpenVPM vs PetsHome

Inventario exhaustivo de módulos de OpenVPM y si PetsHome lo tiene. Útil para ver el techo de lo implementable (no todo entra en v3).

| # | Módulo OpenVPM | PetsHome hoy | Gap / oportunidad |
|---|----------------|--------------|-------------------|
| 1 | **Pacientes/Clientes** (ficha, microchip, fotos, **alergias**, multi-mascota, buscador global) | Parcial — tbMascotas + tbClientes, sin alergias/crónicos | **v3 #1** · microchip/buscador global = extra |
| 2 | **Agenda visual** (calendario día/semana, columna por doctor, detección de conflictos, recurrentes) | Parcial — tbCitaMedica, sin calendario visual por doctor | Alta: calendario tipo OpenVPM sería gran salto |
| 3 | **EMR / SOAP** (notas SOAP rich-text, problem list activo/crónico, adjuntos) | Débil — diagnóstico = texto en la cita; sin SOAP estructurado ni problem list | Media-alta: estructurar la consulta |
| 4 | **Vacunas + certificados** | Parcial — vac_Id en cita, sin certificado imprimible | Baja: generar certificado PDF |
| 5 | **Laboratorio** (resultados con rangos de referencia + gráficas de tendencia) | No existe | Media: módulo nuevo |
| 6 | **Recetas** (lifecycle, refills, calculadora de dosis) | Débil — receta_Medicamento texto libre + itm_Id opcional | **v3 #3** (dispensación) · dosing calc = extra |
| 7 | **Inventario** (catálogo, stock, lote, caducidad, reorder, proveedores) | **Fuerte** — items + existencias por refugio + salidas | PetsHome gana (multi-refugio). Solo endurecer → **v3 #2** |
| 8 | **Sustancias controladas** (log, balances, testigo de desecho, auditoría, DEA schedule) | No existe | Media: requisito regulatorio si aplica |
| 9 | **Facturación** (closeout, estimados→factura, pagos, saldos, plantillas) | Parcial — POS/ventas, factura 'V' manual | **v3 #3** · estimados/saldos = extra |
| 10 | **Whiteboard / sala de espera** | Parcial — sala de espera como lista | **v3 #4** |
| 11 | **Lista de espera por demanda** (waitlist: "avísenme si se abre hueco") | No existe | Baja-media: feed, no gate |
| 12 | **Hub de comunicación** (historial llamadas/SMS/email, recordatorios) | No existe | Media: requiere proveedor SMS/email |
| 13 | **Reportes/Analytics** (KPIs, ingresos, utilización, producción por doctor, export CSV/PDF) | Parcial — dashboards por rol | Media: reportes gerenciales |
| 14 | **Portal del cliente** (ver historial, pedir cita, certificados, pagar online, recetas) | No existe | Alta en esfuerzo: app/área pública nueva |
| 15 | **Planes de bienestar / membresías** | No existe | Baja prioridad |
| 16 | **Seguros** | No existe | Baja prioridad |
| 17 | **Planes de tratamiento / plantillas** | Parcial — tbTratamientos texto | Media |
| 18 | **Signos vitales** (vitals estructurados) | Parcial — peso/temp en la cita | Baja: estructurar |
| 19 | **Consentimientos** (consent forms + firma) | Parcial — ficha responsable (script 56) | Baja |
| 20 | **API abierta** (REST v1 + webhooks + API keys) | No existe | Alta en esfuerzo: solo si hay integradores |
| 21 | **Agente IA** (tool-use sobre datos de la práctica) | No existe | Futuro |

### Lectura rápida del universo
```
PetsHome YA gana   →  Inventario (multi-refugio), dominio interno/refugio, roles/pantallas
Paquete v3 cubre   →  #1 clínica · #2 inventario · #3 ventas · #4 recepción
Siguiente ola útil →  Agenda visual (#2), SOAP estructurado (#3), reportes (#13)
Grandes proyectos  →  Portal cliente (#14), API abierta (#20), Comunicación (#12)
Nicho/regulatorio  →  Sustancias controladas (#8), seguros (#16), membresías (#15)
```

---

## Qué NO copiar de OpenVPM

```
✗ Su almacén de inventario (1 lote / 1 caducidad / sin ubicación) — el nuestro es mejor
✗ Quitar el dominio interno/refugio — OpenVPM no lo tiene; nosotros lo necesitamos
✗ Reescribir con triggers — al revés: movernos hacia lógica explícita, no esconderla en la BD
✗ Su stack (Next.js/PostgreSQL/tRPC) — solo tomamos patrones, no código
```

---

## SOAP + signos vitales en "En Consulta" (script 70)

**Qué se hizo:** la pantalla `Views/Consulta/Index.cshtml` se reorganizó en los 4 bloques clínicos SOAP (Subjetivo / Objetivo / Evaluación / Plan) y se enriqueció el bloque Objetivo con más signos vitales. No es pantalla nueva: se reagruparon los campos que ya existían y se agregaron inputs de vitales. Todo lo que ya funcionaba sigue intacto (alert bar de banderas rojas, SearchSelect de medicamentos, check "Entregar", flujo de guardar recetas + salida de inventario + cola de cobro).

**Mapeo SOAP de la pantalla:**

```
S · Subjetivo   → tipo de consulta + motivo
O · Objetivo    → peso, temperatura, FC, FR + mucosas, llenado capilar,
                  escala de dolor, condición corporal
A · Evaluación  → diagnóstico (+ sugerencias por texto)
P · Plan        → medicamentos, vacuna, próxima cita
```

**Columnas de `Medico.tbCitaMedica`:**

| Vital | Columna | Estado previo |
|-------|---------|---------------|
| Frecuencia cardíaca | `cita_FrecuenciaCardiaca` (int) | Ya existía; el SP y el repo ya la recibían, pero el flujo de consulta NO la guardaba |
| Frecuencia respiratoria | `cita_FrecuenciaRespiratoria` (int) | Igual que FC |
| Mucosas | `cita_Mucosas` (nvarchar 50) | **Agregada** — enum sin tildes (Rosadas/Palidas/Ictericas/Cianoticas/Congestivas) |
| Llenado capilar | `cita_LlenadoCapilar` (decimal 3,1, seg) | **Agregada** |
| Escala de dolor | `cita_EscalaDolor` (int 0–10) | **Agregada** |
| Condición corporal | `cita_CondicionCorporal` (decimal 3,1, 1–9) | **Agregada** |

Todas opcionales (NULL). Enums almacenados sin tildes, mostrados con tilde en la UI.

**Rangos de referencia (aviso suave en el front, NO bloquea):**

```
Temperatura         37.5–39.2 °C
Frecuencia cardíaca 60–180 lpm
Frec. respiratoria  10–40 rpm
Llenado capilar     1–2 seg
Escala de dolor     0–3 esperado (>3 = dolor moderado/severo)
Condición corporal  4–5 ideal (escala 1–9)
Mucosas             normal = Rosadas (otro valor = hallazgo a valorar)
```

Fuera de rango → borde ámbar + nota breve. No impide finalizar la consulta.

**Archivos tocados:**
- `Database/70_SOAP_VITALS.sql` — columnas nuevas + `CREATE OR ALTER` de los SP Insert/Update de cita.
- `PetsHome.Common/Entities/Medico/tbCitaMedica.cs` — 4 propiedades nuevas.
- `PetsHome.Logic/Repositories/CitaMedicaRepository.cs` — 4 parámetros en Add/Edit.
- `PetsHome.Business/Models/ConsultaViewModels.cs` — DTO `GuardarConsultaDto` con FC/FR + 4 vitales.
- `PetsHome.Business/Services/ConsultaService.cs` — `GuardarAsync` ahora persiste FC/FR + vitales (best-effort).
- `PetsHome.UI/Views/Consulta/Index.cshtml` — reorganización SOAP + inputs de vitales.
- `PetsHome.UI/wwwroot/css/en-consulta.css` — estilos SOAP (letra S/O/A/P morada) + campo vital + aviso de rango.
- `PetsHome.UI/wwwroot/js/pages/en-consulta.js` — envío de vitales en el payload + validación de rango suave.

**Rol × función:** SOAP y signos vitales los llena el **veterinario** durante la consulta. No hay variación relevante por rol, no se agregaron permisos ni filtros nuevos.

**Verificación:** `dotnet build` 0 errores. Insert probado en transacción (BEGIN TRAN / ROLLBACK) contra la BD viva: FC/FR + los 4 vitales persisten correctamente y Mucosas se guarda sin tilde.

---

## Sustancias controladas (bitácora regulatoria)

**Qué es:** módulo nuevo (pantalla "Sustancias controladas", menú Clínica) inspirado en el `controlled-substances` de OpenVPM. Bitácora de movimientos de fármacos controlados con saldo calculado y auditoría completa.

**Modelo:** `Medico.tbSustanciaMovimiento` (prefijo `scm_`). Referencia `Inventario.tbItems(itm_Id)` para reusar el catálogo de productos (no se altera tbItems ni se agrega flag "controlado"; el fármaco se elige del catálogo existente). Tipos sin tildes: `Recibido | Administrado | Desechado | Devuelto`. FKs a `tbItems`, `Refugio.tbMascotas` (paciente), `Seguridad.tbUsuarios` (testigo y usuario que registra). `scm_DeaSchedule` (II–V) y `scm_Lote` opcionales.

**Saldo (calculado, no columna):** `SUM(Recibido + Devuelto) − SUM(Administrado + Desechado)` por fármaco. Lo entrega `PR_Medico_SustanciaSaldo_List`.

**Validaciones (en `PR_Medico_SustanciaMovimiento_RegistrarMovimiento`):** cantidad > 0; `Administrado` exige paciente; `Desechado` exige testigo; en `Administrado`/`Desechado` el saldo no puede quedar negativo (se rechaza con el disponible en el mensaje). Devuelve `CodeStatus/MessageStatus/scm_Id`; el controller muestra el motivo del rechazo. Anular = soft-delete (deja rastro).

**Rol × función (pantalla + permisos):** Administrador = todo (recibir + anular); Veterinario = consultar + registrar (administración/desecho); Supervisor = solo lectura (auditoría). Sin acceso para el resto. Permisos en `Database/74_SUSTANCIAS_CONTROLADAS_PANTALLA.sql`; `[PantallaAuthorize("Sustancias controladas")]` en el controller.

**Archivos:**
- `Database/73_SUSTANCIAS_CONTROLADAS.sql` — tabla + 5 SPs (List, Detail, SaldoList, RegistrarMovimiento, Delete).
- `Database/74_SUSTANCIAS_CONTROLADAS_PANTALLA.sql` — pantalla + matriz de permisos.
- `PetsHome.Common/Entities/Medico/` — `tbSustanciaMovimiento` + 3 Result.
- `PetsHome.Business/Models/SustanciaControladaViewModels.cs`, `Services/SustanciaControladaService.cs`.
- `PetsHome.Logic/Repositories/SustanciaControladaRepository.cs`.
- `PetsHome.UI/Controllers/SustanciaControladaController.cs`; `Views/SustanciaControlada/{Index,Create,Detail}.cshtml`; `wwwroot/js/pages/sustancias-controladas.js`.
- Registros en `ServiceConfiguration.cs` + `MappingProfileExtensions.cs`; link en `_sidebar.cshtml` (grupo Clínica).

**Verificación:** `dotnet build` 0 errores. SPs probados en transacción (BEGIN TRAN/ROLLBACK) contra la BD viva: casos OK (Recibido/Administrado/Desechado/Devuelto, saldo final 80) y los que deben fallar — administrar sin paciente (−4), desechar sin testigo (−5), saldo insuficiente (−8), cantidad cero (−2). Rutas verificadas: 302 → Login (auth-gated).

---

## Consentimientos (autorizaciones firmadas por el dueño)

**Qué es:** módulo nuevo (pantalla "Listado de consentimientos", menú Clínica) inspirado en `consents.ts` de OpenVPM (consent forms / consent requests). Formularios que el dueño firma autorizando un procedimiento, ligados a mascota/cliente y opcionalmente a la consulta, con estado y bitácora de firma poco mutable.

**Modelo:** `Medico.tbConsentimiento` (prefijo `cons_`). FKs a `Refugio.tbMascotas(masc_Id)`, `Veterinaria.tbClientes(cli_Id)` (dueño, NULL si interno), `Medico.tbCitaMedica(cita_Id)` (opcional), `Seguridad.tbUsuarios(usu_Id)`. Enums sin tildes con CHECK: `cons_Tipo` (`General|Cirugia|Anestesia|Eutanasia`), `cons_Estado` (`Pendiente|Firmado|Rechazado`). `cons_Texto` (nvarchar max) es la declaración; `cons_FirmanteNombre` + `cons_FechaFirma` registran la firma. El `cli_Id` se **resuelve de la mascota** en el Insert (concepto responsable binario del script 56: cliente o interno). La UI muestra los enums con tilde.

**Firma (fuera de alcance v1):** no hay captura de firma gráfica (trazo/imagen). Se registra nombre del firmante + fecha + estado "Firmado". Documentado como mejora futura.

**Reglas (en SPs):** Insert valida mascota/tipo/texto. Update solo mientras `Pendiente` (−2 si ya firmado/rechazado → texto inmutable). CambiarEstado solo desde `Pendiente` a `Firmado`/`Rechazado` (estados terminales, −3 si no está pendiente); `Firmado` exige firmante (−4). Delete = soft-delete (anular, deja rastro). Todos devuelven `CodeStatus/MessageStatus`.

**Rol × función (pantalla + permisos):** Administrador = todo (incluye anular); Veterinario = consultar + insertar + editar (crea y marca firmado/rechazado; no anula); Recepcionista = consultar + insertar + editar (crea y registra la firma del dueño; no anula). Sin acceso para el resto. Permisos en `Database/76_CONSENTIMIENTOS_PANTALLA.sql`; `[PantallaAuthorize("Listado de consentimientos")]` en el controller. Firmar/rechazar requiere permiso "editar"; anular requiere "eliminar".

**Archivos:**
- `Database/75_CONSENTIMIENTOS.sql` — tabla + 6 SPs (List, Find, Detail, Insert, Update, CambiarEstado, Delete).
- `Database/76_CONSENTIMIENTOS_PANTALLA.sql` — pantalla + matriz de permisos (roles 1/4/7).
- `PetsHome.Common/Entities/Medico/` — `tbConsentimiento` + 3 Result (List/Find/Detail).
- `PetsHome.Business/Models/ConsentimientoViewModels.cs`, `Services/ConsentimientoService.cs`.
- `PetsHome.Logic/Repositories/ConsentimientoRepository.cs`.
- `PetsHome.UI/Controllers/ConsentimientoController.cs`; `Views/Consentimiento/{Index,Create,Detail}.cshtml`; `wwwroot/js/pages/consentimiento.js`.
- Registros en `ServiceConfiguration.cs` + `MappingProfileExtensions.cs`; link en `_sidebar.cshtml` (grupo Clínica). La vista Create pre-llena `cons_Texto` con plantillas simples por tipo (JS).

**Verificación:** `dotnet build` 0 errores. SPs probados en transacción (BEGIN TRAN/ROLLBACK) contra la BD viva: Insert OK (1) + inválidos (tipo −2, texto vacío −3); Update pendiente (1) y bloqueado tras firmar (−2); firmar sin nombre (−4) y con nombre (1); re-cambiar estado terminal (−3); Detail/List/Delete OK. Rutas verificadas: `/Consentimiento/{Index,Create,Detail,List}` → 302 Login (auth-gated); Index sigue a `/Account/Login` (200).

---

## Lista de espera por demanda (waitlist)

**Qué es:** módulo nuevo (pantalla "Lista de espera", menú Recepción) inspirado en `waitlist.ts` de OpenVPM (status waiting→scheduled|cancelled; `matchesForSlot(date,typeId)` FIFO al liberarse una cita). Registra **demanda SIN cupo todavía** ("avíseme si se libera un espacio"). Es DISTINTO de la sala de espera del día (`Medico.tbCitaMedica` estado 'En espera' + Tablero Kanban de Recepción), que es la cola de HOY de pacientes que ya llegaron. Son tablas y pantallas distintas.

**Modelo:** `Medico.tbListaEspera` (prefijo `lesp_`). FKs a `Refugio.tbMascotas(masc_Id)`, `Veterinaria.tbClientes(cli_Id)` (dueño, NULL si interno/sin cliente), `Medico.tbTiposConsulta(tipoCon_Id)` (tipo deseado, NULL = cualquiera), `Seguridad.tbUsuarios(usu_Id)`. `lesp_FechaDeseada` DATE NULL (NULL = lo antes posible). Enums sin tildes con CHECK: `lesp_Prioridad` (`Normal|Alta|Urgente`), `lesp_Estado` (`Esperando|Agendada|Cancelada`). El `cli_Id` se **resuelve de la mascota** en el Insert (responsable binario). Índice filtrado `IX_tbListaEspera_Esperando` (SET ANSI_NULLS/QUOTED_IDENTIFIER ON) para acelerar la cola FIFO.

**Sugerencia FIFO (feed, no gate):** `PR_Medico_ListaEspera_SugerenciasParaHueco @fecha @tipoCon_Id` devuelve los `Esperando` que encajan con el hueco, **más antiguo primero** respetando prioridad. Reglas de encaje (análogas a `matchWaitlist` de OpenVPM): tipo → el registro no pide tipo (NULL) o coincide con el del hueco (o el hueco no especifica); fecha → el registro no pide fecha (NULL) o el hueco cae en/antes de su fecha deseada (`@fecha <= lesp_FechaDeseada`, interpretada como "para tal fecha a más tardar"). Orden: `Urgente > Alta > Normal`, luego `lesp_FechaCrea ASC`. La waitlist nunca bloquea el agendado normal; solo sugiere a quién llamar.

**Reglas (en SPs):** Insert valida mascota/prioridad/tipo y **evita duplicado activo** (misma mascota ya 'Esperando' → −4). Update solo mientras `Esperando` (−2 si ya agendada/cancelada → histórico inmutable). CambiarEstado solo desde `Esperando` a `Agendada`/`Cancelada` (estados terminales, −3 si no sigue esperando). Delete = soft-delete. Todos devuelven `CodeStatus/MessageStatus`. List ordena Esperando primero, luego prioridad y FIFO.

**Rol × función (pantalla + permisos):** Administrador = todo; Recepcionista = consultar + insertar + editar + eliminar (gestiona la lista: anota, agenda, cancela, quita); Veterinario = solo consultar (ve a quién llamar). Sin acceso para el resto. Permisos en `Database/78_LISTA_ESPERA_PANTALLA.sql` (pantalla "Lista de espera", grupo Veterinaria, modu_Id 13 = módulo Recepción, degradable con él); `[PantallaAuthorize("Lista de espera")]` en el controller. Agendar/cancelar requiere "editar"; quitar requiere "eliminar".

**Archivos:**
- `Database/77_LISTA_ESPERA.sql` — tabla + índice filtrado + 7 SPs (List, SugerenciasParaHueco, Find, Detail, Insert, Update, CambiarEstado, Delete).
- `Database/78_LISTA_ESPERA_PANTALLA.sql` — pantalla + matriz de permisos (roles 1/4/7).
- `PetsHome.Common/Entities/Medico/` — `tbListaEspera` + 4 Result (List/Find/Detail/Sugerencia).
- `PetsHome.Business/Models/ListaEsperaViewModels.cs`, `Services/ListaEsperaService.cs` (reusa `TipoConsultaService` para el dropdown de tipos).
- `PetsHome.Logic/Repositories/ListaEsperaRepository.cs` (reusa `PR_Refugio_Mascotas_Dropdown`).
- `PetsHome.UI/Controllers/ListaEsperaController.cs`; `Views/ListaEspera/{Index,Create,Detail}.cshtml`; `wwwroot/js/pages/lista-espera.js`.
- Registros en `ServiceConfiguration.cs` + `MappingProfileExtensions.cs`; link en `_sidebar.cshtml` (grupo Recepción). Index tiene botón "¿A quién llamar?" (modal fecha+tipo → Sugerencias); Detail tiene Agendar/Cancelar.

**Verificación:** `dotnet build` 0 errores. SPs probados en transacción (BEGIN TRAN/ROLLBACK) contra la BD viva: Insert x3 OK + duplicado (−4); List con orden prioridad+FIFO; SugerenciasParaHueco (fecha 2026-10-25 tipo 1 → solo el de tipo 1/sin fecha; excluye tipo 2 y el de fecha deseada anterior) y sin filtros (todos los esperando); CambiarEstado Agendada (1) y re-cambio terminal (−3); Detail/Find OK. Rutas verificadas: `/ListaEspera/{Index,Create,Sugerencias}` → 302 → `/Account/Login` (auth-gated); `/` → 200.
