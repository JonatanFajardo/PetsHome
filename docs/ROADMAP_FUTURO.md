# Roadmap futuro — PetsHome

Ideas y módulos pendientes, para retomar después. Referente: OpenVPM (clonado en `../openvpm`).
Estado actual del producto: **v1.3.0** (ver `docs/VERSIONADO.md`).

---

## 🔶 v2.0.0 (candidata) — Refactor CONTEXTUAL

**Qué es:** mover la gestión de datos clínicos de **pantallas CRUD globales** (menús tipo "Tratamientos", "Recetas" que listan TODO) hacia lo **contextual**: verlos/agregarlos **dentro de la ficha de la mascota (Perfil Médico)** y de la consulta, como hace OpenVPM.

**Por qué es MAYOR (v2.0):** cambia la **navegación** del sistema (se quitan o se vacían menús globales; el flujo pasa a "abrir la mascota → pestañas"). Eso rompe el contrato de navegación → por SemVer es un cambio MAYOR.

**Ejemplo concreto:** hoy `/Tratamiento` es un CRUD global. En la versión contextual, los tratamientos se verían/agregarían como una **pestaña dentro del Perfil Médico** de cada mascota (que YA tiene pestañas: Resumen · Historial · Citas · Peso · Vacunas · Recetas → agregar "Tratamientos").

**Cómo lo hace OpenVPM:** la ficha del paciente ES el centro. Pestañas: Resumen, Historial clínico, Citas, Peso, Vitales, Vacunas, Facturas. Los tratamientos/procedimientos/recetas viven ahí y en el "encounter" (consulta). NO hay menú global que liste todo. Los CRUD globales se reservan para **catálogos** (tipos de cita, productos, plantillas), que van en "Configuración".

**Alcance tentativo:**
- Mover a pestañas del Perfil Médico: Tratamientos, Recetas, (quizá) Laboratorio, Consentimientos, Vitales.
- Dejar como CRUD global SOLO los catálogos (en Configuración).
- Mantener compatibilidad: no perder datos; es reubicación de UI + navegación.

**Riesgo:** medio-alto (toca pantallas que ya funcionan). Hacerlo como su propia etapa/rama, bien probado. NO urgente: hoy la info ya se puede ver (ficha) y gestionar (CRUD) — esto es modernización de UX, no funcionalidad faltante.

---

## Módulos grandes pendientes (de OpenVPM)

Cada uno sería una o más versiones MINOR (aditivas), su propia rama/etapa.

| Módulo | Qué hace | Dónde viviría | Esfuerzo |
|--------|----------|----------------|----------|
| **Planes de bienestar / membresías** | Paquetes de pago mensual que cubren servicios (vacunas, chequeos) por una cuota | Área de cliente / facturación | [mucho] |
| **Seguros** | Registrar la póliza de seguro de la mascota y vincularla a los cobros | Ficha del cliente / facturación | [medio-mucho] |
| **Agente IA** | Asistente que consulta datos de la clínica (buscar pacientes, resumen clínico, calcular dosis) | Pantalla propia | [mucho] |

---

## Excluidos por decisión (no implementar por ahora)

- **Portal del cliente** — ya existe en PetsHome; se mejorará después si hace falta.
- **API abierta + webhooks** — no se implementará por ahora.
- **Hub de comunicación (SMS/email)** — no se implementará por ahora.

---

## Ya cerrado (referencia rápida)

Clínica (banderas rojas), Inventario (snapshot), Ventas/POS (cobros), Recepción (tablero Kanban),
Agenda (lente por rol), SOAP+Vitales, Laboratorio, Sustancias controladas, Consentimientos,
Lista de espera (waitlist). Todo en `master`, hasta **v1.3.0**. Detalle en `docs/MEJORAS_V3.md`.
