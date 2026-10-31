# Brechas PetsHome vs OpenVPM

> Mapa completo de lo que a PetsHome le falta/difiere respecto al sistema guía (OpenVPM),
> para que el dueño decida qué aplicar. Hecho a partir de lo verificado durante el trabajo.
> Fecha: 2026-10-28.

Leyenda estado: **✅ Tiene** · **◐ Parcial** · **❌ No** · Esfuerzo [poco/medio/mucho] · Prioridad [ALTA/MEDIA/BAJA].

> ⚠️ Nota: algunos puntos conviene re-verificar contra la BD viva antes de construir (fuente de verdad = LocalDB, no los `.sql`/`.cs`).

---

## 1. FACTURACIÓN y COBRO

| Feature OpenVPM | PetsHome | Hueco | Recomendación | Esf. | Prio |
|---|---|---|---|---|---|
| **Qué es facturable**: medicamentos, **vacunas, laboratorio, procedimientos**, servicios, productos | ✅ Medicamentos + **vacunas/laboratorio/procedimientos** vía servicios facturables (script 82: `tbServicios` por categoría + `PR_..._EncolarServicio`; ConsultaService encola el cargo al guardar) | — (cerrado) | — | — | — |
| **Cobro por visita / checkout** (al cerrar, elegir Pagado/Por cobrar/Sin cargo) | ◐ **En rediseño** (rama `feature/cobro-por-visita`, sin terminar el checkout contextual) | Falta mover el cobro al cierre de consulta y quitar /Cobros | Terminar el checkout contextual | medio | **ALTA** |
| **Registrar PAGOS** (pagos, método, recibo) | ❌ La factura queda 'Emitida/Pagada' pero no hay tabla de pagos | No se registran pagos ni métodos | Tabla de pagos + registrar pago | medio | **ALTA** |
| **Pagos parciales / saldo** | ❌ | No hay abonos ni saldo pendiente real | Saldo = total − pagos | medio | MEDIA |
| **A/R (cuentas por cobrar) con vencimiento** | ◐ (el rediseño agregó `fact_Vencimiento` + estado 'PorCobrar') | Falta estado de cuenta del cliente y mora | Vista de facturas por cobrar/vencidas en /Facturacion | medio | MEDIA |
| **Estado de cuenta del cliente** (outstanding, overdue) | ❌ | No se ve cuánto debe un cliente | Resumen por cliente (suma de facturas por cobrar) | medio | MEDIA |
| **Estimados / presupuestos** (que se convierten en factura) | ❌ | No existe | Entidad estimado → factura | medio | BAJA |
| **Ajustes / descuentos / reembolsos / anulación** | ◐ Anular factura; sin descuentos/reembolsos | Faltan descuentos y ajustes | Descuento por línea / factura + nota de crédito | medio | MEDIA |
| **Impuestos configurables por ítem (taxable)** | ◐ ISV 15% fijo global | No hay ítems exentos | Flag `taxable` por producto/servicio | poco | BAJA |
| **Snapshot de precio al dispensar** | ✅ | — | — | — | — |

---

## 2. CLÍNICO (consulta, paciente, registros)

| Feature OpenVPM | PetsHome | Hueco | Recomendación | Esf. | Prio |
|---|---|---|---|---|---|
| **SOAP** (4 bloques) | ✅ (bloques S/O/A/P) | Falta firma/finalización y **addenda/correcciones** (notas inmutables) | Addenda append-only + firmar nota | medio | MEDIA |
| **Signos vitales** (rangos + tendencia) | ✅ Varios (FC/FR/mucosas/llenado/dolor/condición) | Falta **gráfica de tendencia** | Charts de vitales/peso | poco | BAJA |
| **Problem list / padecimientos** | ✅ (padecimientos) | — | — | poco | BAJA |
| **Alergias** (banderas rojas) | ✅ | — | — | — | — |
| **Vacunas: certificado imprimible + recordatorios** | ◐ Se registra la vacuna | Falta **certificado** y **recordatorio** de refuerzo | Certificado PDF + recall | medio | MEDIA |
| **Recetas: refills/resurtidos + calculadora de dosis** | ◐ texto+FK+dosis/frecuencia | Sin refills ni dosing por peso | Refills + calculadora de dosis | medio | MEDIA |
| **Laboratorio: rangos + tendencia + follow-up** | ◐ Módulo básico (exámenes+resultados+rangos) | Falta tendencia entre visitas y estado de seguimiento | Trend + follow_up status | medio | BAJA |
| **Procedimientos estructurados** | ◐ `cita_ProcedimientosRealizados` (texto) | No estructurado ni facturable | Entidad procedimiento (+billable) | medio | MEDIA |
| **Ficha paciente con pestañas** (Overview/Records/Citas/Peso/Vitales/Vacunas/Facturas) | ◐ PerfilMedico con tabs (Resumen/Historial/Citas/Peso/Vacunas/Recetas) | Faltan pestañas **Facturas**, **Vitales**, **Laboratorio** | Agregar pestañas | poco-medio | MEDIA |
| **Historial de peso con gráfica** | ◐ Peso sí; gráfica no | Falta chart de tendencia | Chart | poco | BAJA |
| **Microchip** | ❌ (no hay campo en la mascota) | Falta campo microchip | +columna + UI | poco | BAJA |
| **Fotos del paciente** | ✅ (masc_Imagen) | — | — | — | — |

---

## 3. OPERATIVO (agenda, recepción, inventario, etc.)

| Feature OpenVPM | PetsHome | Hueco | Recomendación | Esf. | Prio |
|---|---|---|---|---|---|
| **Agenda: columna por doctor** | ◐ Calendario + lente por rol | No hay vista multi-doctor en columnas | Vista por recurso/doctor | mucho | **ALTA** |
| **Agenda: arrastrar para reagendar** | ❌ | Reagendar es manual | Drag-to-reschedule | medio | MEDIA |
| **Agenda: detección de conflictos** (doctor/sala/hora) | ❌ | Permite choques | Validar al agendar | medio | MEDIA |
| **Citas recurrentes / bloques** | ❌ | No existe | Recurrencia | medio | BAJA |
| **Whiteboard / tablero del día** | ✅ (Kanban) | — | — | — | — |
| **Lista de espera (waitlist)** | ✅ (básico) | Falta "a quién llamar" automático al liberar hueco | Sugerencia FIFO ya hecha; afinarla | poco | BAJA |
| **Inventario: lotes + caducidad** | ❌ (stock plano por refugio) | No hay lote ni vencimiento | Lotes/caducidad (si aplica) | mucho | MEDIA |
| **Inventario: reorden + proveedores** | ◐ stock mínimo; recepción de mercancía | Punto de reorden/alertas y proveedores parciales | Alertas de reorden + proveedores | medio | MEDIA |
| **Sustancias controladas** | ✅ (básico: saldo, testigo, DEA) | — | — | poco | BAJA |
| **Consentimientos** | ◐ (básico) | Falta **firma digital** y plantillas | Firma + plantillas | medio | BAJA |
| **Recordatorios / recalls** (vacunas, follow-ups) | ◐ proximaCita + alertas médicas | Sin recordatorios automáticos | Motor de recalls | medio | MEDIA |
| **Reportes / analytics** (ingresos, producción por doctor, utilización, export) | ◐ Dashboards por rol | Faltan reportes financieros/gerenciales + export | Reportes + export CSV/PDF | medio | MEDIA |
| **Roles / permisos** | ✅ (granular por pantalla) | — | — | — | — |
| **Multi-sede / ubicaciones** | ◐ (refugios) | Parcial | Evaluar si hace falta | — | BAJA |

---

## 4. Excluidos por decisión del dueño (no implementar por ahora)

Portal del cliente (ya existe básico) · API abierta + webhooks · Hub SMS/email · Agente IA.
Planes de bienestar/membresías · Seguros (baja prioridad).

---

## 5. Recomendación — orden sugerido (por impacto/esfuerzo)

```
YA CERRADO (sesiones oct 2026):
  ✓ Cobro POR VISITA / checkout contextual
  ✓ Facturables vacunas / labs / procedimientos (script 82 + ConsultaService)
  ✓ PAGOS (+ saldo) y cuentas por cobrar / estado de cuenta
  ✓ Certificados de vacuna + recordatorios
  ✓ Pestañas de la ficha (Facturas/Vitales/Peso/Lab)
  ✓ Reportes financieros (feature/reportes) · 7 estados de cita

AHORA (lo de más impacto que sigue abierto):
  1. Agenda por doctor (columnas) + detección de conflictos de horario
  2. Recetas: resurtidos (refills) + calculadora de dosis por peso
  3. Motor de recordatorios/recalls automáticos

CUANDO HAGA FALTA:
  4. Inventario con lotes/caducidad · procedimientos estructurados
  5. Agenda arrastrar-para-reagendar · descuentos/ajustes/reembolsos
  6. SOAP firma/addenda · export de reportes (CSV/PDF) · estimados · firma de consentimientos
```

---

## Ficha del paciente: OpenVPM vs PetsHome (2026-10-29)

**Regla:** la ficha (expediente) y la consulta (visita activa) son DOS pantallas distintas, como en OpenVPM. Ficha = historia completa del animal (varias visitas, read-mostly, muchas pestañas). Consulta = herramienta de trabajo de UNA visita (SOAP, vitales, cargos, recetas). Si la ficha se "parece" a la consulta es porque está incompleta.

**OpenVPM — ficha del paciente** (`patients/[id]/page.tsx`): pestañas Overview · Medical Records · Documents (Photos/Consents/Documents/Lab reports) · Appointments · Weight History · Vitals · Vaccinations (+certificado) · Invoices.
**OpenVPM — consulta** (`encounters/[appointmentId]/page.tsx`): SOAP · Vitals · Charges/Payment · Prescriptions · Diagnosis/Discharge · Treatment plan.

**Comparación de pestañas de la FICHA:**

| Pestaña (OpenVPM)        | PetsHome (al 2026-10-29) |
|--------------------------|--------------------------|
| Overview / Resumen       | ✅ |
| Registros médicos        | ◐ (Historial/Citas) |
| Citas                    | ✅ |
| Peso (con gráfica)       | ❌ falta |
| Signos vitales           | ❌ falta |
| Vacunas (+certificado)   | ✅ (certificado agregado) |
| Facturas                 | ❌ falta |
| Documentos / Laboratorio | ❌ falta |

**Pendiente:** enriquecer la ficha de PetsHome con Facturas · Signos vitales · Peso (gráfica) · Laboratorio · Documentos, leyendo datos que ya existen, para que sea el expediente completo como OpenVPM. (Documentos requiere almacenamiento de archivos → puede diferirse.)

> **Actualización:** Facturas · Signos vitales · Peso (gráfica) · Laboratorio ya se implementaron (quedan en ✅). Falta solo Documentos (diferido).
