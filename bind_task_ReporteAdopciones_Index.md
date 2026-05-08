# TAREA: Conectar vista Razor al modelo

> Controller: `ReporteAdopciones` | Action: `Index`

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


## ViewModel: ReporteAdopcionesViewModel.cs

```csharp
using PetsHome.Common.Entities;
using System.Collections.Generic;

        namespace PetsHome.Business.Models
        {
            public class ReporteAdopcionesViewModel
            {
                // ── Secciones ────────────────────────────────────
                public List<PR_Refugio_ReporteAdopciones_ResumenResult> Resumen { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_ResumenResult>();

        public List<PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult> AdopcionesPorMes { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult>();

        public List<PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult> EstadoSolicitudes { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult>();

        public List<PR_Refugio_ReporteAdopciones_TopRazasResult> TopRazas { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_TopRazasResult>();

        public List<PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult> AdopcionesRecientes { get; set; }
            = new List<PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult>();

                // ── Conteos calculados ────────────────────────────
                public int TotalResumen => Resumen?.Count ?? 0;
        public int TotalAdopcionesPorMes => AdopcionesPorMes?.Count ?? 0;
        public int TotalEstadoSolicitudes => EstadoSolicitudes?.Count ?? 0;
        public int TotalTopRazas => TopRazas?.Count ?? 0;
        public int TotalAdopcionesRecientes => AdopcionesRecientes?.Count ?? 0;
                public int TotalAlertas => TotalResumen + TotalAdopcionesPorMes + TotalEstadoSolicitudes + TotalTopRazas + TotalAdopcionesRecientes;
            }
        }
```


## Result Classes


## PR_Refugio_ReporteAdopciones_ResumenResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Refugio_ReporteAdopciones_ResumenResult
    {
        public int TotalAdopciones { get; set; }
public int SolicitudesPendientes { get; set; }
public int TasaAprobacion { get; set; }
public int TiempoPromedio { get; set; }
    }
}
```


## PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Refugio_ReporteAdopciones_AdopcionesPorMesResult
    {
        public string Mes { get; set; }
public int Cantidad { get; set; }
    }
}
```


## PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Refugio_ReporteAdopciones_EstadoSolicitudesResult
    {
        public string Estado { get; set; }
public int Cantidad { get; set; }
    }
}
```


## PR_Refugio_ReporteAdopciones_TopRazasResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Refugio_ReporteAdopciones_TopRazasResult
    {
        public string Raza { get; set; }
public int CantidadAdopciones { get; set; }
    }
}
```


## PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult

```csharp
using System;

namespace PetsHome.Common.Entities
{
    public class PR_Refugio_ReporteAdopciones_AdopcionesRecientesResult
    {
        public string MascotaNombre { get; set; }
public string Raza { get; set; }
public string Adoptante { get; set; }
public DateTime FechaAdopcion { get; set; }
public string Estado { get; set; }
public int DiasTranscurridos { get; set; }
    }
}
```


## Vista estática a transformar: ReporteAdopciones/Index.cshtml

```razor
@{
    ViewData["Title"] = "Reporte de Adopciones — PetsHome";
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewData["CurrentPantalla"] = "";
}

@section Styles {
  <link rel="stylesheet" href="~/css/reporte-adopciones.css" />
  <link href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.2/css/bootstrap.min.css" rel="stylesheet"/>
  <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet"/>
  <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&amp;family=DM+Mono:wght@400;500&amp;display=swap" rel="stylesheet"/>
}

<div class="custom-tooltip" id="tooltip"></div>
<!-- ═══════════════════ HEADER ═══════════════════ -->
<div class="report-header">
<div class="header-title">
<div class="header-icon">🐾</div>
<div>
<h1>Reporte de Adopciones</h1>
<span>PetsHome · Actualizado hace 5 min</span>
</div>
</div>
<div class="header-filters">
<select class="filter-select" id="periodoSelect" onchange="updatePeriod()">
<option value="hoy">Hoy</option>
<option value="semana">Esta semana</option>
<option selected="" value="mes">Este mes</option>
<option value="anio">Este año</option>
<option value="custom">Rango personalizado</option>
</select>
<select class="filter-select" id="refugioSelect" onchange="updateRefugio()">
<option value="todos">Todos los refugios</option>
<option value="las-lomas">Refugio Las Lomas</option>
<option value="la-esperanza">Refugio La Esperanza</option>
<option value="vida-animal">Refugio Vida Animal</option>
</select>
<button class="btn-export" onclick="exportPDF()">
<i class="fas fa-file-pdf"></i> Exportar PDF
    </button>
</div>
</div>
<!-- ═══════════════════ PAGE CONTENT ═══════════════════ -->
<div class="page-wrap">
<!-- KPI ROW -->
<div class="kpi-grid" id="kpiGrid">
<div class="kpi-card green" data-delay="0">
<div class="kpi-header">
<div class="kpi-label">Total adopciones</div>
<div class="kpi-icon"><i class="fas fa-paw"></i></div>
</div>
<div class="kpi-value" data-target="48" id="kpi-total">0</div>
<div class="kpi-sub">
        Del período seleccionado  
        <span class="kpi-trend trend-up"><i class="fas fa-arrow-trend-up"></i> +12%</span>
</div>
</div>
<div class="kpi-card amber" data-delay="80">
<div class="kpi-header">
<div class="kpi-label">Solicitudes pendientes</div>
<div class="kpi-icon"><i class="fas fa-hourglass-half"></i></div>
</div>
<div class="kpi-value" data-target="14" id="kpi-pending">0</div>
<div class="kpi-sub">
        Esperando revisión  
        <span class="kpi-trend trend-neu"><i class="fas fa-minus"></i> Sin cambio</span>
</div>
</div>
<div class="kpi-card blue" data-delay="160">
<div class="kpi-header">
<div class="kpi-label">Tasa de aprobación</div>
<div class="kpi-icon"><i class="fas fa-chart-pie"></i></div>
</div>
<div class="kpi-value" data-suffix="%" data-target="76" id="kpi-rate">0</div>
<div class="kpi-sub">
        Solicitudes aprobadas  
        <span class="kpi-trend trend-up"><i class="fas fa-arrow-trend-up"></i> +5%</span>
</div>
</div>
<div class="kpi-card purple" data-delay="240">
<div class="kpi-header">
<div class="kpi-label">Tiempo promedio</div>
<div class="kpi-icon"><i class="fas fa-clock"></i></div>
</div>
<div class="kpi-value" data-suffix=" d" data-target="8" id="kpi-time">0</div>
<div class="kpi-sub">
        Promedio del proceso  
        <span class="kpi-trend trend-down"><i class="fas fa-arrow-trend-down"></i> -2d</span>
</div>
</div>
</div><!-- /kpi-grid -->
<!-- CHARTS ROW -->
<div class="charts-row">
<!-- Bar chart -->
<div class="chart-card" id="barCard">
<div class="card-title">Adopciones por mes</div>
<div class="card-subtitle">Últimos 6 meses · comparativa</div>
<div class="chart-wrap">
<canvas id="barChart"></canvas>
</div>
</div>
<!-- Donut chart -->
<div class="chart-card" id="donutCard">
<div class="card-title">Estado de solicitudes</div>
<div class="card-subtitle">Distribución actual del período</div>
<div class="donut-wrap">
<div class="donut-canvas-row">
<canvas id="donutChart"></canvas>
<div class="dona-legend" id="donaLegend"></div>
</div>
</div>
</div>
</div><!-- /charts-row -->
<!-- BOTTOM ROW -->
<div class="bottom-row">
<!-- Top breeds -->
<div class="list-card" id="breedsCard">
<div class="card-title">🏆 Top 5 razas más adoptadas</div>
<div class="card-subtitle">Período seleccionado · cantidad de adopciones</div>
<div id="breedsList"></div>
</div>
<!-- Recent adoptions -->
<div class="list-card" id="recentCard">
<div class="card-title">🕐 Adopciones recientes</div>
<div class="card-subtitle">Últimas 6 solicitudes registradas</div>
<div style="overflow-x:auto">
<table class="adopt-table">
<thead>
<tr>
<th>Mascota</th>
<th>Adoptante</th>
<th>Fecha</th>
<th>Estado</th>
<th style="text-align:center">Días</th>
</tr>
</thead>
<tbody id="recentTableBody"></tbody>
</table>
</div>
</div>
</div><!-- /bottom-row -->
</div><!-- /page-wrap -->

@section Scripts {
  <script src="~/js/pages/reporte-adopciones.js"></script>
}
```


---
## Reglas de lógica — OBLIGATORIAS

### 1. Cero ternarios sin efecto
Nunca escribas un ternario donde ambas ramas devuelven el mismo valor.
```csharp
// ❌ PROHIBIDO
var cls = condicion ? "mismo-valor" : "mismo-valor";
// ✅ CORRECTO
var cls = condicion ? "valor-a" : "valor-b";
```

### 2. Cero valores hardcodeados que cambien con el tiempo
Fechas, años y horas deben generarse en tiempo de ejecución.
```html
<!-- ❌ PROHIBIDO -->
<input type="date" value="2026-04-25" />
<!-- ✅ CORRECTO -->
<input type="date" value="@DateTime.Now.ToString("yyyy-MM-dd")" />
```

### 3. Sin lógica duplicada en la vista
Si el mismo bloque if/switch para calcular una clase CSS aparece más de una vez, extráelo en una Func<> al inicio del bloque @{ }.
```csharp
// ✅ Definir una vez, usar en todos los foreach
Func<string, string> badgeClass = tipo => tipo switch
{
    "Urgencia"   => "badge-urgencia",
    "Vacunación" => "badge-vacuna",
    _            => "badge-control"
};
```

---
## Reglas de HTML — OBLIGATORIAS

### 4. Sin event handlers inline
Nunca uses onclick, onchange, etc. en el HTML. Asigna id al elemento; el handler va en el .js externo.
```html
<!-- ❌ PROHIBIDO -->
<button onclick="document.getElementById('modal').style.display='flex'">
<!-- ✅ CORRECTO -->
<button id="btnAbrirModal">
```

### 5. Sin estilos inline de presentación
Los valores de style="" de diseño fijo van en el .css. Solo se permiten inline para valores dinámicos del modelo (p. ej. style="width:@pct%").
```html
<!-- ❌ PROHIBIDO -->
<div style="font-size:12px;color:#888">
<!-- ✅ CORRECTO -->
<div class="ficha-details">
```

### 6. Visibilidad de modales con hidden, no con style="display:none"
```html
<!-- ❌ PROHIBIDO -->
<div id="modal" style="display:none">
<!-- ✅ CORRECTO -->
<div id="modal" hidden aria-modal="true" role="dialog">
```

---
## Reglas de formato — OBLIGATORIAS

### 7. Indentación consistente de 4 espacios
Cada nivel de anidación agrega 4 espacios. Aplica a HTML, Razor y C# dentro de @{ }.

### 8. Comentario de cierre en divs contenedores principales
Todo </div> que cierre un bloque con más de ~20 líneas debe llevar un comentario identificador.
```html
    </div><!-- /tab-resumen -->
</div><!-- /content-area -->
```

### 9. Alinear operadores ternarios multilínea
```csharp
var cls = estado == "Vencida" ? "dot-venc"    :
          estado == "Próxima" ? "dot-proxima" : "dot-ok";
```

---
## Checklist — verifica esto antes de generar el archivo

- [ ] ¿Algún ternario tiene ambas ramas iguales? → corregir
- [ ] ¿Hay alguna fecha, año o valor temporal hardcodeado? → usar expresión C#
- [ ] ¿Hay bloques if/switch para clases CSS repetidos más de una vez? → extraer a Func<>
- [ ] ¿Hay algún onclick / onchange inline? → mover a JS externo con id
- [ ] ¿Hay style="" con valores de diseño fijos? → mover a CSS con clase
- [ ] ¿Algún modal usa style="display:none"? → usar hidden
- [ ] ¿Toda la indentación es de 4 espacios y consistente? → revisar anidación
- [ ] ¿Los divs principales tienen comentario de cierre? → agregar

---
**Instrucción final:** Devuelve únicamente el archivo `.cshtml` con todos los bindings aplicados y con todas las reglas anteriores cumplidas. Sin explicaciones, sin bloques markdown extra. Solo el contenido del archivo.
