/* ==========================================================
   Dashboard Admin — ApexCharts (tema claro, paleta del proyecto)
   Colores: accent #6c63ff · green #10b981 · amber #f59e0b
            red #ef4444 · blue #3b82f6 · muted #6b7280
   ========================================================== */
(function () {
    'use strict';

    var d = window._adminDash || {};

    /* ── Paleta del proyecto ── */
    var CLR = {
        accent: '#6c63ff',
        blue:   '#3b82f6',
        green:  '#10b981',
        amber:  '#f59e0b',
        red:    '#ef4444',
        muted:  '#6b7280',
        border: '#e5e7eb',
        text:   '#1a1a2e'
    };

    /* ── Base compartida para todas las charts ── */
    var BASE = {
        chart: {
            background:  'transparent',
            toolbar:     { show: false },
            fontFamily:  'inherit',
            foreColor:   CLR.text,
            animations:  { enabled: true, easing: 'easeinout', speed: 750 }
        },
        grid: {
            borderColor:     CLR.border,
            strokeDashArray: 4,
            xaxis: { lines: { show: false } }
        },
        tooltip: { theme: 'light' }
    };

    var LABEL_STYLE = { style: { colors: CLR.muted, fontSize: '11px' } };

    /* ── 1. TENDENCIAS — área con gradiente (2 series) ── */
    function initTendencias() {
        var el = document.getElementById('chart-tendencias');
        if (!el || !d.tendencias || !d.tendencias.length) return;

        var cats       = d.tendencias.map(function(r) { return r.etiquetaMes; });
        var ingresos   = d.tendencias.map(function(r) { return r.ingresos;   });
        var adopciones = d.tendencias.map(function(r) { return r.adopciones; });

        new ApexCharts(el, {
            chart:  Object.assign({}, BASE.chart, { type: 'area', height: 280 }),
            series: [
                { name: 'Ingresos al refugio', data: ingresos   },
                { name: 'Adopciones',           data: adopciones }
            ],
            xaxis: {
                categories:  cats,
                labels:      LABEL_STYLE,
                axisBorder:  { show: false },
                axisTicks:   { show: false }
            },
            yaxis:  { labels: LABEL_STYLE, min: 0 },
            stroke: { curve: 'smooth', width: 2.5 },
            fill: {
                type: 'gradient',
                gradient: { shadeIntensity: 1, opacityFrom: 0.35, opacityTo: 0.02, stops: [0, 90] }
            },
            colors:  [CLR.accent, CLR.green],
            markers: { size: 4, strokeWidth: 2, strokeColors: '#fff', hover: { size: 6 } },
            legend:  {
                position: 'top', horizontalAlign: 'right',
                labels:   { colors: CLR.muted },
                fontSize: '12px',
                markers:  { width: 10, height: 10, radius: 10 }
            },
            grid:    BASE.grid,
            tooltip: BASE.tooltip
        }).render();
    }

    /* ── 2. MASCOTAS POR ESTADO — donut ── */
    function initEstados() {
        var el = document.getElementById('chart-estados');
        if (!el || !d.estados || !d.estados.length) return;

        var COLORS = [CLR.accent, CLR.amber, CLR.green, CLR.blue];

        new ApexCharts(el, {
            chart:  Object.assign({}, BASE.chart, { type: 'donut', height: 280 }),
            series: d.estados.map(function(e) { return e.cantidad; }),
            labels: d.estados.map(function(e) { return e.estado;   }),
            colors: COLORS,
            plotOptions: {
                pie: {
                    donut: {
                        size: '68%',
                        labels: {
                            show: true,
                            total: {
                                show:      true,
                                label:     'Total',
                                color:     CLR.muted,
                                fontSize:  '12px',
                                fontWeight: 600,
                                formatter: function(w) {
                                    return w.globals.seriesTotals.reduce(function(a, b) { return a + b; }, 0);
                                }
                            },
                            value: { color: CLR.text, fontSize: '1.4rem', fontWeight: 700 }
                        }
                    }
                }
            },
            stroke:     { show: true, width: 2, colors: ['#fff'] },
            dataLabels: { enabled: false },
            legend: {
                position: 'bottom',
                labels:   { colors: CLR.muted },
                fontSize: '12px',
                markers:  { width: 10, height: 10, radius: 10 }
            },
            tooltip: BASE.tooltip
        }).render();
    }

    /* ── 3. TOP RAZAS — barra horizontal ── */
    function initRazas() {
        var el = document.getElementById('chart-razas');
        if (!el || !d.razas || !d.razas.length) return;

        /* Gradiente de la misma familia que el accent */
        var COLORS = ['#6c63ff','#7c6fff','#8b7bff','#6357e8','#5849c9','#4f46e5','#4338ca'];

        new ApexCharts(el, {
            chart:       Object.assign({}, BASE.chart, { type: 'bar', height: 260 }),
            plotOptions: { bar: { horizontal: true, borderRadius: 5, distributed: true, barHeight: '60%' } },
            series:      [{ name: 'Adoptadas', data: d.razas.map(function(r) { return r.total; }) }],
            xaxis: {
                categories: d.razas.map(function(r) { return r.raza_Descripcion; }),
                labels:     LABEL_STYLE,
                axisBorder: { show: false },
                axisTicks:  { show: false }
            },
            yaxis:       { labels: { style: { colors: CLR.muted, fontSize: '11px' } } },
            colors:      COLORS,
            dataLabels:  {
                enabled: true,
                style:   { colors: ['#fff'], fontSize: '11px', fontWeight: 600 },
                formatter: function(v) { return v.toLocaleString(); }
            },
            legend:  { show: false },
            grid:    BASE.grid,
            tooltip: BASE.tooltip
        }).render();
    }

    /* ── 4. HEATMAP — citas por día y hora ── */
    function initHeatmap() {
        var el = document.getElementById('chart-heatmap');
        if (!el) return;

        var flat = d.heatmap || [];
        if (!flat.length) {
            el.innerHTML = '<p class="da-empty">Sin datos de citas recientes</p>';
            return;
        }

        var dias = ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'];

        var horasSet = {};
        flat.forEach(function(f) { horasSet[f.hora] = true; });
        var horas = Object.keys(horasSet).map(Number).sort(function(a, b) { return a - b; });

        var series = horas.map(function(hora) {
            return {
                name: (hora < 10 ? '0' : '') + hora + ':00',
                data: dias.map(function(dia, idx) {
                    var found = flat.find(function(f) { return f.hora === hora && f.diaSemana === idx + 1; });
                    return { x: dia, y: found ? found.cantidad : 0 };
                })
            };
        });

        new ApexCharts(el, {
            chart:  Object.assign({}, BASE.chart, { type: 'heatmap', height: 260 }),
            series: series,
            colors: [CLR.accent],
            plotOptions: {
                heatmap: {
                    shadeIntensity: .65,
                    radius: 3,
                    colorScale: {
                        ranges: [
                            { from: 0,   to: 0,   color: '#f3f2ff', name: 'Sin citas' },
                            { from: 1,   to: 2,   color: '#c4b5fd', name: '1 – 2'    },
                            { from: 3,   to: 5,   color: '#8b5cf6', name: '3 – 5'    },
                            { from: 6,   to: 999, color: '#6c63ff', name: '6+'       }
                        ]
                    }
                }
            },
            dataLabels: { enabled: false },
            xaxis:  { labels: LABEL_STYLE },
            yaxis:  { labels: { style: { colors: CLR.muted, fontSize: '10px' } } },
            tooltip: BASE.tooltip,
            legend: {
                show: true,
                labels: { colors: CLR.muted },
                fontSize: '11px'
            }
        }).render();
    }

    /* ── 5. EMBUDO DE ADOPCIÓN — barras horizontales ── */
    function initEmbudo() {
        var el = document.getElementById('chart-embudo');
        if (!el || !d.embudo || !d.embudo.length) return;

        /* Colores degradados de mayor a menor */
        var COLORS = [CLR.accent, '#7c6fff', '#8b7bff', '#a89cff'];

        new ApexCharts(el, {
            chart:       Object.assign({}, BASE.chart, { type: 'bar', height: 260 }),
            plotOptions: { bar: { horizontal: true, borderRadius: 5, distributed: true, barHeight: '55%' } },
            series:      [{ name: 'Solicitudes', data: d.embudo.map(function(e) { return e.cantidad; }) }],
            xaxis: {
                categories: d.embudo.map(function(e) { return e.etapa; }),
                labels:     LABEL_STYLE,
                axisBorder: { show: false },
                axisTicks:  { show: false }
            },
            yaxis:      { labels: { style: { colors: CLR.muted, fontSize: '12px' } } },
            colors:     COLORS,
            dataLabels: {
                enabled:  true,
                style:    { colors: ['#fff'], fontWeight: 700 },
                formatter: function(v) { return v.toLocaleString(); }
            },
            legend:  { show: false },
            grid:    BASE.grid,
            tooltip: BASE.tooltip
        }).render();
    }

    /* ── CountUp en KPIs ── */
    function initCountUp() {
        if (typeof countUp === 'undefined') return;
        document.querySelectorAll('.stat-value').forEach(function(el) {
            var n = parseInt(el.textContent.replace(/[^0-9]/g, ''), 10);
            if (isNaN(n) || n <= 0) return;
            el.textContent = '0';
            new countUp.CountUp(el, n, { duration: 2, useEasing: true }).start();
        });
    }

    /* ── Arranque ── */
    document.addEventListener('DOMContentLoaded', function () {
        initTendencias();
        initEstados();
        initRazas();
        initHeatmap();
        initEmbudo();
        initCountUp();
    });
})();
