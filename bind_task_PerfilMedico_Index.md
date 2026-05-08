# TAREA: Conectar vista Razor al modelo

> Controller: `PerfilMedico` | Action: `Index`

Tu tarea es tomar la vista Razor estática y conectarla al ViewModel.
Reemplaza todos los datos hardcodeados con bindings `@Model`, `@foreach`, etc.
**Devuelve SOLO el contenido del .cshtml completo y actualizado.**

## Convenciones del proyecto PetsHome

### Estructura de la vista
```razor
@model PetsHome.Business.Models.{Entity}ViewModel
@{
    ViewData["Title"]           = "...";
    Layout                      = "~/Views/Shared/_Layout.cshtml";
    ViewData["CurrentPantalla"] = "Nombre pantalla";   // mismo valor que [PantallaAuthorize]

    // Si una sección tiene solo 1 fila (detalle/cabecera):
    var ficha = Model.NombreSeccion.FirstOrDefault();
}
```

### Listas → @foreach
```razor
@foreach (var item in Model.NombreLista)
{
    <div>@item.Campo</div>
}
```

### Sección de 1 sola fila (ej: ficha de mascota)
```razor
var ficha = Model.FichaMascota.FirstOrDefault();
// luego usar: @ficha?.Campo  (con ? por si es null)
```

### Empty state cuando no hay datos
```razor
@if (!Model.Lista.Any())
{
    <div class="empty-state">...</div>
}
```

### Fechas
```razor
@item.FechaCampo.ToString("dd MMM yyyy")
@item.FechaNullable?.ToString("dd MMM yyyy")
```

### Sección Scripts con window._urls
```razor
@section Scripts {
  <script>
    window._urls = {
      accion: '@Url.Action("Accion", "Controller")'
    };
  </script>
  <script src="~/js/pages/{slug}.js"></script>
}
```

### Regla importante
- Usar los nombres de campo EXACTAMENTE como están en las Result classes.
- No inventar propiedades que no existen en el ViewModel.
- Devolver SOLO el .cshtml completo, sin explicaciones ni bloques de código extra.


## ViewModel: PerfilMedicoViewModel.cs

```csharp
using PetsHome.Common.Entities;
using System.Collections.Generic;

        namespace PetsHome.Business.Models
        {
            public class PerfilMedicoViewModel
            {
                // ── Secciones ────────────────────────────────────
                public List<PR_Medico_PerfilMedico_FichaMascotaResult> FichaMascota { get; set; }
            = new List<PR_Medico_PerfilMedico_FichaMascotaResult>();

        public List<PR_Medico_PerfilMedico_UltimasCitasResult> UltimasCitas { get; set; }
            = new List<PR_Medico_PerfilMedico_UltimasCitasResult>();

        public List<PR_Medico_PerfilMedico_MedicamentosActivosResult> MedicamentosActivos { get; set; }
            = new List<PR_Medico_PerfilMedico_MedicamentosActivosResult>();

        public List<PR_Medico_PerfilMedico_TodasCitasResult> TodasCitas { get; set; }
            = new List<PR_Medico_PerfilMedico_TodasCitasResult>();

        public List<PR_Medico_PerfilMedico_TratamientosResult> Tratamientos { get; set; }
            = new List<PR_Medico_PerfilMedico_TratamientosResult>();

        public List<PR_Medico_PerfilMedico_VacunasResult> Vacunas { get; set; }
            = new List<PR_Medico_PerfilMedico_VacunasResult>();

                // ── Conteos calculados ────────────────────────────
                public int TotalFichaMascota => FichaMascota?.Count ?? 0;
        public int TotalUltimasCitas => UltimasCitas?.Count ?? 0;
        public int TotalMedicamentosActivos => MedicamentosActivos?.Count ?? 0;
        public int TotalTodasCitas => TodasCitas?.Count ?? 0;
        public int TotalTratamientos => Tratamientos?.Count ?? 0;
        public int TotalVacunas => Vacunas?.Count ?? 0;
                public int TotalAlertas => TotalFichaMascota + TotalUltimasCitas + TotalMedicamentosActivos + TotalTodasCitas + TotalTratamientos + TotalVacunas;
            }
        }
```


## Result Classes


## PR_Medico_PerfilMedico_FichaMascotaResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_FichaMascotaResult
    {
        public int masc_Id { get; set; }
public string masc_Nombre { get; set; }
public string Raza { get; set; }
public string Edad { get; set; }
public string Sexo { get; set; }
public bool EsEsterilizada { get; set; }
public string Microchip { get; set; }
public decimal? Peso { get; set; }
public string Adoptante { get; set; }
public string Refugio { get; set; }
public DateTime? UltimaVisita { get; set; }
public string EstadoSalud { get; set; }
public int TotalCitas { get; set; }
public int TratamientosActivos { get; set; }
public int VacunasAlDia { get; set; }
    }
}
```


## PR_Medico_PerfilMedico_UltimasCitasResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_UltimasCitasResult
    {
        public int cita_Id { get; set; }
public DateTime cita_FechaConsulta { get; set; }
public string TipoConsulta { get; set; }
public string cita_Diagnostico { get; set; }
public string Veterinario { get; set; }
    }
}
```


## PR_Medico_PerfilMedico_MedicamentosActivosResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_MedicamentosActivosResult
    {
        public int trat_Id { get; set; }
public string Medicamento { get; set; }
public string Dosis { get; set; }
public int? DiasRestantes { get; set; }
public int PorcentajeCompletado { get; set; }
    }
}
```


## PR_Medico_PerfilMedico_TodasCitasResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_TodasCitasResult
    {
        public int cita_Id { get; set; }
public DateTime cita_FechaConsulta { get; set; }
public string TipoConsulta { get; set; }
public string cita_Diagnostico { get; set; }
public string Veterinario { get; set; }
public string Hora { get; set; }
    }
}
```


## PR_Medico_PerfilMedico_TratamientosResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_TratamientosResult
    {
        public int trat_Id { get; set; }
public string NombreTratamiento { get; set; }
public string Medicamento { get; set; }
public DateTime trat_FechaInicio { get; set; }
public DateTime? trat_FechaFin { get; set; }
public int PorcentajeCompletado { get; set; }
public string EstadoTratamiento { get; set; }
    }
}
```


## PR_Medico_PerfilMedico_VacunasResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Medico_PerfilMedico_VacunasResult
    {
        public int vac_Id { get; set; }
public string VacunaNombre { get; set; }
public DateTime? FechaAplicada { get; set; }
public DateTime? FechaProxima { get; set; }
public string EstadoVacuna { get; set; }
    }
}
```


## Vista estática a transformar: PerfilMedico/Index.cshtml

```razor
@model PetsHome.Business.Models.PerfilMedicoViewModel
@{
    ViewData["Title"] = "Perfil Médico — VetCare";
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewData["CurrentPantalla"] = "PerfilMedico";

    var ficha = Model.FichaMascota.FirstOrDefault();

    // BUG CORREGIDO #1: Ambas ramas del ternario devolvían el mismo valor.
    // Ahora se distingue correctamente entre estados de salud.
    Func<string, string> tipoBadgeClass = tipo => tipo switch
    {
        "Urgencia"   => "badge-urgencia",
        "Vacunación" => "badge-vacuna",
        "Revisión"   => "badge-revision",
        _            => "badge-control"
    };

    Func<string, string> tipoCardClass = tipo => tipo switch
    {
        "Urgencia"   => "tipo-urgencia",
        "Vacunación" => "tipo-vacuna",
        "Revisión"   => "tipo-revision",
        _            => "tipo-control"
    };
}

@section Styles {
    <link rel="stylesheet" href="~/css/perfil-medico-mascota.css" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.3/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.11.3/font/bootstrap-icons.min.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Playfair+Display:wght@600;700&amp;family=DM+Sans:wght@300;400;500;600&amp;display=swap" rel="stylesheet" />
}

<div class="app-wrapper">

    <!-- ════════════ SIDEBAR ════════════ -->
    <aside class="sidebar">
        <div class="avatar-wrap">
            <div class="avatar-img">
                <span class="avatar-emoji">🐕</span>
            </div>

            @{
                // BUG CORREGIDO #1: Ternario sin efecto. Antes: condición ? "status-treatment" : "status-treatment"
                // Ahora diferencia correctamente los estados.
                var statusClass = ficha?.EstadoSalud != null && ficha.EstadoSalud.Contains("Tratamiento")
                    ? "status-treatment"
                    : "status-healthy";
            }
            <span class="status-badge @statusClass">@ficha?.EstadoSalud</span>
        </div>

        <div class="pet-name">@ficha?.masc_Nombre</div>
        <div class="pet-meta">
            @ficha?.Raza · @ficha?.Edad<br />
            @ficha?.Sexo · @(ficha?.EsEsterilizada == true ? "Esterilizada" : "No esterilizada")
        </div>

        <div class="shelter-tag">
            <i class="bi bi-house-heart-fill"></i>
            @ficha?.Refugio
        </div>

        <div class="sidebar-divider"></div>

        <div class="mini-stats">
            <div class="mini-stat" style="animation-delay:.1s">
                <div class="mini-stat-icon purple">
                    <i class="bi bi-calendar-check"></i>
                </div>
                <div>
                    <div class="mini-stat-val">@ficha?.TotalCitas</div>
                    <div class="mini-stat-lbl">Total Citas</div>
                </div>
            </div>

            <div class="mini-stat" style="animation-delay:.18s">
                <div class="mini-stat-icon orange">
                    <i class="bi bi-capsule"></i>
                </div>
                <div>
                    <div class="mini-stat-val">@ficha?.TratamientosActivos</div>
                    <div class="mini-stat-lbl">Tratamientos activos</div>
                </div>
            </div>

            <div class="mini-stat" style="animation-delay:.26s">
                <div class="mini-stat-icon green">
                    <i class="bi bi-shield-check"></i>
                </div>
                <div>
                    <div class="mini-stat-val">@ficha?.VacunasAlDia</div>
                    <div class="mini-stat-lbl">Vacunas al día</div>
                </div>
            </div>
        </div>

        <div class="sidebar-divider"></div>

        <div class="sidebar-ficha">
            <div class="section-title">Ficha</div>
            <div class="ficha-details">
                <div><b>Microchip</b> · @ficha?.Microchip</div>
                <div><b>Peso</b> · @ficha?.Peso kg</div>
                <div><b>Adoptante</b> · @ficha?.Adoptante</div>
                <div><b>Último control</b> · @ficha?.UltimaVisita?.ToString("dd MMM yyyy")</div>
            </div>
        </div>
    </aside>

    <!-- ════════════ MAIN ════════════ -->
    <div class="main-content">

        <!-- Header -->
        <header class="top-header">
            <div class="header-left">
                <div class="header-title">@ficha?.masc_Nombre — Perfil Médico</div>
                <nav class="breadcrumb-nav">
                    <span>VetCare</span>
                    <i class="bi bi-chevron-right"></i>
                    <span>Mascotas</span>
                    <i class="bi bi-chevron-right"></i>
                    <span class="breadcrumb-active">@ficha?.masc_Nombre</span>
                </nav>
            </div>
            {{/* BUG CORREGIDO #3: onclick inline reemplazado por id+JS externo */}}
            <button class="btn-nueva" id="btnNuevaConsulta">
                <i class="bi bi-plus-lg"></i> Nueva Consulta
            </button>
        </header>

        <!-- Tab Bar -->
        <nav class="tab-bar">
            <button class="tab-btn active" data-tab="resumen">
                <i class="bi bi-grid-1x2"></i> Resumen
            </button>
            <button class="tab-btn" data-tab="citas">
                <i class="bi bi-calendar3"></i> Citas
                <span class="tab-count">@Model.TotalTodasCitas</span>
            </button>
            <button class="tab-btn" data-tab="historial">
                <i class="bi bi-clock-history"></i> Historial
            </button>
            <button class="tab-btn" data-tab="recetas">
                <i class="bi bi-file-earmark-medical"></i> Recetas
            </button>
            <button class="tab-btn" data-tab="tratamientos">
                <i class="bi bi-activity"></i> Tratamientos
                <span class="tab-count">@Model.TotalTratamientos</span>
            </button>
            <button class="tab-btn" data-tab="vacunas">
                <i class="bi bi-shield-check"></i> Vacunas
            </button>
        </nav>

        <!-- Content -->
        <div class="content-area">

            <!-- ═══ RESUMEN ═══ -->
            <div class="tab-panel active" id="tab-resumen">
                <div class="section-title">Últimas Citas</div>

                <div class="citas-grid">
                    @if (!Model.UltimasCitas.Any())
                    {
                        <div class="empty-state">
                            <i class="bi bi-calendar3"></i>
                            <p>No hay citas recientes.</p>
                        </div>
                    }

                    @foreach (var item in Model.UltimasCitas)
                    {
                        <div class="cita-card @tipoCardClass(item.TipoConsulta)">
                            <div class="cita-fecha">
                                <i class="bi bi-calendar3"></i>
                                @item.cita_FechaConsulta.ToString("dd MMM yyyy")
                            </div>
                            <div class="tipo-badge @tipoBadgeClass(item.TipoConsulta)">
                                @item.TipoConsulta
                            </div>
                            <div class="cita-diag">@item.cita_Diagnostico</div>
                            <div class="cita-vet">
                                <i class="bi bi-person-badge"></i> @item.Veterinario
                            </div>
                        </div>
                    }
                </div>

                <!-- Lower section -->
                <div class="resumen-lower">

                    <!-- Medications table -->
                    <div class="meds-card">
                        <div class="meds-header">
                            <span>
                                <i class="bi bi-capsule me-2 icon-purple"></i>Medicamentos Activos
                            </span>
                            <span class="meds-count">@Model.TotalMedicamentosActivos en curso</span>
                        </div>

                        @if (!Model.MedicamentosActivos.Any())
                        {
                            <div class="empty-state">
                                <p>No hay medicamentos activos.</p>
                            </div>
                        }
                        else
                        {
                            <table class="meds-table">
                                <thead>
                                    <tr>
                                        <th>Medicamento</th>
                                        <th>Progreso</th>
                                        <th class="text-center">Días rest.</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    @foreach (var med in Model.MedicamentosActivos)
                                    {
                                        var diasClass = med.DiasRestantes <= 5  ? "dias-low" :
                                                        med.DiasRestantes <= 10 ? "dias-mid" : "dias-ok";
                                        <tr>
                                            <td>
                                                <div class="med-name">@med.Medicamento</div>
                                                <div class="med-dose">@med.Dosis</div>
                                            </td>
                                            <td>
                                                <div class="prog-bar">
                                                    <div class="prog-fill" data-pct="@med.PorcentajeCompletado"></div>
                                                </div>
                                                <div class="prog-pct">@med.PorcentajeCompletado% completado</div>
                                            </td>
                                            <td class="dias-cell @diasClass">@med.DiasRestantes</td>
                                        </tr>
                                    }
                                </tbody>
                            </table>
                        }
                    </div>

                    <!-- Timeline -->
                    <div class="timeline-card">
                        <div class="timeline-header">
                            <i class="bi bi-clock-history me-2 icon-purple"></i>Últimos Eventos
                        </div>
                        <div class="timeline-body">
                            @foreach (var item in Model.UltimasCitas)
                            {
                                var tlClass = item.TipoConsulta == "Urgencia"   ? "red"    :
                                              item.TipoConsulta == "Vacunación" ? "green"  : "purple";
                                var tlIcon  = item.TipoConsulta == "Urgencia"   ? "bi-exclamation-triangle" :
                                              item.TipoConsulta == "Vacunación" ? "bi-shield-check"         : "bi-stethoscope";
                                <div class="tl-item">
                                    <div class="tl-icon @tlClass">
                                        <i class="bi @tlIcon"></i>
                                    </div>
                                    <div class="tl-text">
                                        <div class="tl-event">@item.TipoConsulta</div>
                                        <div class="tl-date">
                                            @item.cita_FechaConsulta.ToString("dd MMM yyyy") · @item.Veterinario
                                        </div>
                                    </div>
                                </div>
                            }
                        </div>
                    </div>

                </div>
            </div><!-- /tab-resumen -->

            <!-- ═══ CITAS ═══ -->
            <div class="tab-panel" id="tab-citas">
                <div class="section-title">Todas las Citas</div>

                <div class="citas-list">
                    @if (!Model.TodasCitas.Any())
                    {
                        <div class="empty-state">
                            <i class="bi bi-calendar3"></i>
                            <p>No hay citas registradas.</p>
                        </div>
                    }

                    @foreach (var item in Model.TodasCitas)
                    {
                        <div class="cita-row">
                            <div class="cita-row-date">
                                <div class="cita-row-day">@item.cita_FechaConsulta.ToString("dd")</div>
                                <div class="cita-row-month">@item.cita_FechaConsulta.ToString("MMM")</div>
                            </div>
                            <div class="cita-row-body">
                                <div class="cita-row-title">@item.TipoConsulta</div>
                                <div class="cita-row-sub">
                                    <span><i class="bi bi-person-badge"></i> @item.Veterinario</span>
                                    <span><i class="bi bi-clock"></i> @item.Hora</span>
                                </div>
                            </div>
                            <div>
                                <span class="tipo-badge @tipoBadgeClass(item.TipoConsulta)">@item.TipoConsulta</span>
                            </div>
                        </div>
                    }
                </div>
            </div>

            <!-- ═══ HISTORIAL ═══ -->
            <div class="tab-panel" id="tab-historial">
                <div class="empty-state">
                    <i class="bi bi-archive"></i>
                    <div class="empty-title">Historial Clínico Completo</div>
                    <p>El historial detallado estará disponible próximamente.<br />Por ahora puedes consultar el Resumen general.</p>
                </div>
            </div>

            <!-- ═══ RECETAS ═══ -->
            <div class="tab-panel" id="tab-recetas">
                <div class="empty-state">
                    <i class="bi bi-file-earmark-medical"></i>
                    <div class="empty-title">Recetas Digitales</div>
                    <p>Aquí aparecerán las recetas emitidas por los veterinarios.</p>
                </div>
            </div>

            <!-- ═══ TRATAMIENTOS ═══ -->
            <div class="tab-panel" id="tab-tratamientos">
                <div class="section-title">Tratamientos en Curso</div>

                <div class="trat-list">
                    @if (!Model.Tratamientos.Any())
                    {
                        <div class="empty-state">
                            <i class="bi bi-activity"></i>
                            <p>No hay tratamientos registrados.</p>
                        </div>
                    }

                    @foreach (var trat in Model.Tratamientos)
                    {
                        var tstatClass   = trat.EstadoTratamiento == "Finalizado" ? "tstat-done" : "tstat-active";
                        var tratOpacity  = trat.EstadoTratamiento == "Finalizado" ? "opacity:.75" : "";
                        var progStyle    = trat.EstadoTratamiento == "Finalizado"
                            ? "background:linear-gradient(90deg,#6EE7B7,#059669)"
                            : "";

                        <div class="trat-card" style="@tratOpacity">
                            <div class="trat-icon-wrap">💊</div>
                            <div class="trat-info">
                                <div class="trat-name">@trat.NombreTratamiento</div>
                                <div class="trat-med">
                                    <i class="bi bi-capsule"></i> @trat.Medicamento
                                </div>
                                <div class="trat-dates">
                                    <span>
                                        <i class="bi bi-calendar-event"></i>
                                        Inicio: @trat.trat_FechaInicio.ToString("dd MMM yyyy")
                                    </span>
                                    <span>
                                        <i class="bi bi-calendar-x"></i>
                                        Fin: @trat.trat_FechaFin?.ToString("dd MMM yyyy")
                                    </span>
                                </div>
                            </div>
                            <div class="trat-progress">
                                <div class="trat-prog-label">
                                    <span class="trat-prog-name">Progreso del tratamiento</span>
                                    <span class="trat-prog-pct">@trat.PorcentajeCompletado%</span>
                                </div>
                                <div class="trat-prog-bar">
                                    <div class="trat-prog-fill"
                                         data-pct="@trat.PorcentajeCompletado"
                                         style="@progStyle">
                                    </div>
                                </div>
                            </div>
                            <div class="trat-status-col">
                                <span class="trat-status @tstatClass">@trat.EstadoTratamiento</span>
                            </div>
                        </div>
                    }
                </div>
            </div>

            <!-- ═══ VACUNAS ═══ -->
            <div class="tab-panel" id="tab-vacunas">
                <div class="section-title">Cartilla de Vacunación</div>

                <div class="vacunas-grid">
                    @if (!Model.Vacunas.Any())
                    {
                        <div class="empty-state">
                            <i class="bi bi-shield-check"></i>
                            <p>No hay vacunas registradas.</p>
                        </div>
                    }

                    @foreach (var vac in Model.Vacunas)
                    {
                        var vClass   = vac.EstadoVacuna == "Vencida"                                    ? "vacuna-venc"    :
                                      (vac.EstadoVacuna != null && vac.EstadoVacuna.Contains("róxima")) ? "vacuna-proxima" : "vacuna-ok";
                        var dotClass = vac.EstadoVacuna == "Vencida"                                    ? "dot-venc"    :
                                      (vac.EstadoVacuna != null && vac.EstadoVacuna.Contains("róxima")) ? "dot-proxima" : "dot-ok";

                        <div class="vacuna-card">
                            <div class="vacuna-icon">💉</div>
                            <div class="vacuna-name">@vac.VacunaNombre</div>
                            <div class="vacuna-date">
                                <i class="bi bi-calendar-check"></i>
                                Aplicada: @vac.FechaAplicada?.ToString("dd MMM yyyy")
                            </div>
                            <div class="vacuna-next @vClass">
                                <span class="vacuna-dot @dotClass"></span>
                                @vac.EstadoVacuna · Próxima: @vac.FechaProxima?.ToString("MMM yyyy")
                            </div>
                        </div>
                    }

                    <div class="vacuna-card vacuna-card--add" id="btnAnadirVacuna">
                        <i class="bi bi-plus-circle vacuna-add-icon"></i>
                        <div class="vacuna-add-label">Añadir vacuna</div>
                    </div>
                </div>
            </div>

        </div><!-- /content-area -->
    </div><!-- /main-content -->
</div><!-- /app-wrapper -->


<!-- ════════ MODAL NUEVA CONSULTA ════════ -->
{{/* BUG CORREGIDO #4: Fecha hardcodeada "2026-04-25" reemplazada por fecha dinámica */}}
<div id="modalConsulta" class="modal-overlay" aria-modal="true" role="dialog" hidden>
    <div class="modal-box">
        <div class="modal-header">
            <div>
                <div class="modal-title">Nueva Consulta</div>
                <div class="modal-subtitle">@ficha?.masc_Nombre · @ficha?.Raza</div>
            </div>
            <button class="modal-close" id="btnCerrarModal" aria-label="Cerrar">×</button>
        </div>

        <div class="modal-body">
            <div class="modal-grid-2">
                <div class="form-group">
                    <label class="form-label">TIPO</label>
                    <select class="form-control" id="modalTipo">
                        <option>Control General</option>
                        <option>Urgencia</option>
                        <option>Vacunación</option>
                        <option>Revisión</option>
                        <option>Cirugía</option>
                    </select>
                </div>
                <div class="form-group">
                    <label class="form-label">FECHA</label>
                    {{/* BUG CORREGIDO #4: valor dinámico, no hardcodeado */}}
                    <input class="form-control" type="date" id="modalFecha"
                           value="@DateTime.Now.ToString("yyyy-MM-dd")" />
                </div>
            </div>

            <div class="form-group">
                <label class="form-label">VETERINARIO</label>
                <select class="form-control" id="modalVeterinario">
                    <option>Dra. Ana Martínez</option>
                    <option>Dr. Carlos Herrera</option>
                    <option>Dra. Patricia Ruiz</option>
                </select>
            </div>

            <div class="form-group">
                <label class="form-label">DIAGNÓSTICO / NOTAS</label>
                <textarea class="form-control"
                          id="modalDiagnostico"
                          placeholder="Describe el motivo de consulta o diagnóstico..."
                          rows="3"></textarea>
            </div>

            <div class="modal-actions">
                <button class="btn-cancel" id="btnCancelarModal">Cancelar</button>
                <button class="btn-save"   id="btnGuardarConsulta">Guardar Consulta</button>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script>
        window._urls = {
            perfilMedico: '@Url.Action("Index", "PerfilMedico")'
        };
    </script>
    <script src="~/js/pages/perfil-medico-mascota.js"></script>
}
```


---
**Instrucción final:** Devuelve únicamente el archivo `.cshtml` con todos los bindings aplicados. Sin explicaciones, sin bloques markdown extra. Solo el contenido del archivo.
