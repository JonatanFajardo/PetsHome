# PetsHome — Seguimiento de Pantallas por Rol

> Actualizado: 2026-04-28  
> Leyenda: ✅ Completo · 🔧 Mejorada (sobre CRUD base) · 🆕 Nueva · ⏳ Pendiente · 📋 CRUD base

---

## 🩺 ROL: Veterinario

| # | Pantalla | Ruta | Estado | Notas |
|---|----------|------|--------|-------|
| 1 | Alertas Médicas | `AlertaMedica/Index` | ✅🆕 | Dashboard de alertas activas. CSS/JS propio. SP: `PR_Medico_AlertaMedica_Dashboard` |
| 2 | Calendario de Citas | `CitaMedica/Calendario` | ✅🆕 | Vista calendario mensual de citas. CSS/JS propio |
| 3 | Citas Médicas (tabla) | `CitaMedica/Index` | ✅📋 | CRUD estándar |
| 4 | Perfil Médico Mascota | `PerfilMedico/Index` | ✅🆕 | Vista 360° de la mascota. CSS/JS propio. SP: `PR_Medico_PerfilMedico_Dashboard` |
| 5 | Historial Médico | `HistorialMedico/Index` | ✅📋 | CRUD estándar |
| 6 | Recetas | `Receta/Index` | ✅📋 | CRUD estándar |
| 7 | Tratamientos | `Tratamiento/Index` | ✅📋 | CRUD estándar |
| 8 | Control de Vacunación | `ControlVacunacion/Index` | ✅🆕 | Matriz mascota×vacuna. KPIs + semáforo ok/warn/red. SP: `PR_Medico_ControlVacunacion_MatrizVacunacion` |
| 9 | Dashboard Veterinario | — | ⏳ | CSS existe (`dashboard-veterinario.css`). Pantalla nueva pendiente |

---

## 📊 ROL: Supervisor

| # | Pantalla | Ruta | Estado | Notas |
|---|----------|------|--------|-------|
| 1 | Reporte Adopciones | `ReporteAdopciones/Index` | ✅🆕 | Dashboard de adopciones. CSS/JS propio. SP: `PR_Refugio_ReporteAdopciones_Dashboard` |
| 2 | Solicitudes (Kanban) | `Solicitud/Index` | ✅🔧 | Agregada 3ª vista Kanban con cambio de estado. SQL: `FIX_solicitudes_estado.sql` |
| 3 | Recepciones Mercancía | `RecepcionMercancia/Index` | ✅🔧 | Barra resumen + filtros período/tipo. SQL: `FIX_recepcion_mercancia_summary.sql` |
| 4 | Items (Stock) | `Item/Index` | ✅🔧 | Semáforo OK/Bajo/Crítico + tab "Por Vencer". SQL: `FIX_item_stock.sql` |
| 5 | Eventos (Calendario) | `Evento/Index` | ✅🔧 | Agregada 3ª vista calendario mensual con pills |
| 6 | Voluntarios por Evento | `Evento/Details` | ✅🔧 | Sección voluntarios: confirmar/ausente/asignar. SQL: `FIX_evento_voluntarios.sql` |
| 7 | Mascotas | `Mascota/Index` | ✅📋 | CRUD estándar |
| 8 | Adopciones | `Adopcion/Index` | ✅📋 | CRUD estándar |
| 9 | Refugios | `Refugio/Index` | ✅📋 | CRUD estándar |
| 10 | Dashboard Supervisor | — | ⏳ | Pantalla nueva pendiente |

---

## ⚙️ ROL: Administrador / Director

| # | Pantalla | Ruta | Estado | Notas |
|---|----------|------|--------|-------|
| 1 | Usuarios | `Usuarios/Index` | ✅📋 | Gestión de usuarios del sistema |
| 2 | Roles | `Roles/Index` | ✅📋 | Gestión de roles y permisos |
| 3 | Empleados | `Empleado/Index` | ✅📋 | CRUD estándar |
| 4 | Voluntarios | `Voluntario/Index` | ✅📋 | CRUD estándar |
| 5 | Localidades | `Localidad/Index` | ✅📋 | Departamentos y municipios |
| 6 | Home / Dashboard | `Home/Index` | ✅ | Dashboard principal rediseñado |

---

## 📦 MÓDULO: Inventario (compartido Supervisor/Admin)

| # | Pantalla | Ruta | Estado | Notas |
|---|----------|------|--------|-------|
| 1 | Items | `Item/Index` | ✅🔧 | Ver tabla Supervisor arriba |
| 2 | Recepciones | `RecepcionMercancia/Index` | ✅🔧 | Ver tabla Supervisor arriba |
| 3 | Detalle Recepción | `RecepcionMercancia/DetailRecepcion` | ✅📋 | Vista de detalles de una recepción |

---

## 📚 CATÁLOGOS (Admin)

Todos en `Catalogo/` — solo CRUD base, sin mejoras de UX pendientes:

`Categoria` · `Gravedad` · `Procedencia` · `Raza` · `TipoConsulta` · `TipoEsterilizacion` · `TipoMedicamento` · `TipoParasito` · `Vacuna` · `ViaAdministracion` · `EmpleadosCargo`

---

## ⏳ PENDIENTES

| Pantalla | Rol | Tipo | Descripción |
|----------|-----|------|-------------|
| Dashboard Supervisor | Supervisor | 🆕 Nueva | KPIs de refugio: adopciones, eventos, stock crítico, solicitudes pendientes |
| Dashboard Veterinario | Veterinario | 🆕 Nueva | CSS ya existe. Resumen del día: citas, alertas, tratamientos activos |
| ~~Control de Vacunación~~ | Veterinario | ✅ Listo | Implementado — ver tabla Veterinario |
| Consulta Médica Integrada | Veterinario | ⏳ Nueva | Pantalla unificada cita + historial + receta + tratamiento |
| Perfil de Adopción | Supervisor | ⏳ Mejora | `Adopcion/Detail` enriquecida con historial médico y fotos |

---

## 🗂️ SQL Scripts ejecutados en BD

| Archivo | Qué hace |
|---------|----------|
| `FIX_solicitudes_estado.sql` | Agrega `sol_Estado` a `tbSolicitudes`, SP `CambiarEstado` |
| `FIX_recepcion_mercancia_summary.sql` | Actualiza List SP con TotalItems, ValorTotal, ItemsPorVencer |
| `FIX_item_stock.sql` | Agrega `itm_StockMinimo`, SP List con stock calculado, SP PorVencer |
| `FIX_evento_voluntarios.sql` | Agrega estado/fecha a `tbEventos_tbVoluntarios`, 4 SPs voluntarios |
| `FIX_alertas_medicas.sql` / `FIX_ALERTAMEDICA_pantalla.sql` | SPs y pantalla AlertaMedica |
| `FIX_CONTROLVACUNACION_pantalla.sql` | Pantalla ControlVacunacion (ya ejecutado por scaffold) |
| `25_SP_MEDICO_CONTROLVACUNACION_DASHBOARD.sql` | SPs MatrizVacunacion y Dashboard |
| `FIX_PERFILMEDICO_pantalla.sql` | Pantalla PerfilMedico |
| `FIX_REPORTEADOPCIONES_pantalla.sql` | Pantalla ReporteAdopciones |
| `23_SP_PERFIL_MEDICO_MASCOTA.sql` | SP principal del perfil médico |
| `24_SP_REFUGIO_REPORTEADOPCIONES_DASHBOARD.sql` | SP del reporte de adopciones |
| `21/22_SP_MEDICO_ALERTAMEDICA_DASHBOARD.sql` | SPs del dashboard de alertas |
| `13_SP_CITA_MEDICA_CALENDARIO.sql` | SP del calendario de citas |
