/* ==========================================================
   Reportes — pestañas Ingresos · Citas · Servicios · Inventario
   - Rango de fechas (por defecto últimos 30 días); al cambiar recarga la pestaña activa.
   - Consumo de datos vía window.getJson (http.js, token y headers automáticos).
   - Gráficas ApexCharts con render perezoso (se dibujan al abrir la pestaña, ya visible).
   - Exportar CSV (archivo generado en el servidor, descarga por enlace a /Reportes/*Csv).
   - Imprimir / PDF (encabezado de impresión + window.print; el usuario guarda como PDF).
   ========================================================== */
(function () {
    "use strict";

    var urls = window._urls || {};

    /* ── Paleta del proyecto ── */
    var CLR = {
        accent: "#6c5ce7",
        blue:   "#0984e3",
        green:  "#00b894",
        amber:  "#e17055",
        red:    "#d63031",
        muted:  "#8a92a6",
        border: "#e8eaf2",
        text:   "#2d3436"
    };

    /* ── Formateadores ── */
    function money(n) {
        return "L. " + (Number(n) || 0).toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }
    function entero(n) { return (Number(n) || 0).toLocaleString("es-HN"); }
    function fechaCorta(s) {
        var d = new Date(s);
        return isNaN(d) ? (s || "") : d.toLocaleDateString("es-HN", { day: "2-digit", month: "short" });
    }
    function esc(s) { var d = document.createElement("div"); d.textContent = (s == null ? "" : String(s)); return d.innerHTML; }
    function iso(d) {
        // yyyy-mm-dd en hora local (evita el corrimiento de toISOString por UTC)
        var y = d.getFullYear(), m = ("0" + (d.getMonth() + 1)).slice(-2), day = ("0" + d.getDate()).slice(-2);
        return y + "-" + m + "-" + day;
    }

    /* ── Estado ── */
    var cache  = {};   // datos por pestaña (para el CSV y evitar refetch)
    var charts = {};   // instancias ApexCharts por pestaña
    var activeTab = "ingresos";

    var desdeEl = document.getElementById("repDesde");
    var hastaEl = document.getElementById("repHasta");

    /* ── Fechas por defecto: últimos 30 días ── */
    (function initFechas() {
        var hoy = new Date();
        var hace = new Date(); hace.setDate(hoy.getDate() - 29);
        desdeEl.value = iso(hace);
        hastaEl.value = iso(hoy);
    })();

    function rango() { return { desde: desdeEl.value, hasta: hastaEl.value }; }
    function qs(url) {
        var r = rango();
        return url + "?desde=" + encodeURIComponent(r.desde) + "&hasta=" + encodeURIComponent(r.hasta);
    }

    /* ── Base ApexCharts ── */
    function baseChart(type, height) {
        return {
            background: "transparent",
            toolbar: { show: false },
            fontFamily: "inherit",
            foreColor: CLR.text,
            type: type,
            height: height || 320,
            animations: { enabled: true, easing: "easeinout", speed: 700 }
        };
    }
    var GRID = { borderColor: CLR.border, strokeDashArray: 4, xaxis: { lines: { show: false } } };
    var LBL  = { style: { colors: CLR.muted, fontSize: "11px" } };

    function destroyChart(key) {
        if (charts[key]) { try { charts[key].destroy(); } catch (e) {} charts[key] = null; }
    }

    /* ══════════════════════ PESTAÑAS ══════════════════════ */
    function panel(tab) { return document.getElementById("panel-" + tab); }

    function setActive(tab) {
        activeTab = tab;
        document.querySelectorAll(".rep-tab").forEach(function (b) {
            b.classList.toggle("active", b.getAttribute("data-tab") === tab);
        });
        document.querySelectorAll(".rep-panel").forEach(function (p) {
            p.classList.toggle("active", p.id === "panel-" + tab);
        });
    }

    function loadTab(tab, force) {
        if (cache[tab] && !force) { renderTab(tab, cache[tab]); return; }
        var url = (tab === "inventario") ? urls.inventario : qs(urls[tab]);
        if (!url) return;
        window.getJson(url).then(function (res) {
            cache[tab] = res || {};
            renderTab(tab, cache[tab]);
        }).catch(function () {
            renderError(tab);
        });
    }

    function renderTab(tab, data) {
        if (tab === "ingresos")   renderIngresos(data);
        else if (tab === "citas") renderCitas(data);
        else if (tab === "servicios") renderServicios(data);
        else if (tab === "inventario") renderInventario(data);
    }

    function renderError(tab) {
        var p = panel(tab);
        if (!p) return;
        var card = p.querySelector(".rep-card");
        if (card) {
            var holder = card.querySelector(".rep-chart") || card.querySelector("tbody") || card;
            if (holder.tagName === "TBODY") holder.innerHTML = '<tr><td colspan="9"><div class="rep-empty"><i class="fas fa-triangle-exclamation"></i>No se pudieron cargar los datos.</div></td></tr>';
            else holder.innerHTML = '<div class="rep-empty"><i class="fas fa-triangle-exclamation"></i>No se pudieron cargar los datos.</div>';
        }
    }

    /* ══════════════════════ INGRESOS ══════════════════════ */
    function renderIngresos(d) {
        var total = Number(d.total) || 0;
        var anterior = Number(d.totalAnterior) || 0;
        var serie = d.serie || [];

        document.getElementById("ing-total").textContent = money(total);
        document.getElementById("ing-anterior").textContent = money(anterior);

        var cmp = document.getElementById("ing-compare");
        if (anterior > 0) {
            var pct = ((total - anterior) / anterior) * 100;
            var cls = pct > 0 ? "up" : (pct < 0 ? "down" : "neu");
            var ico = pct > 0 ? "fa-arrow-trend-up" : (pct < 0 ? "fa-arrow-trend-down" : "fa-minus");
            cmp.innerHTML = '<span class="rep-trend ' + cls + '"><i class="fas ' + ico + '"></i> ' +
                (pct > 0 ? "+" : "") + pct.toFixed(1) + "%</span> vs período anterior";
        } else {
            cmp.innerHTML = '<span class="rep-trend neu"><i class="fas fa-minus"></i> s/d</span> sin período anterior';
        }

        destroyChart("ingresos");
        if (typeof ApexCharts === "undefined") return;
        charts.ingresos = new ApexCharts(document.getElementById("ing-chart"), {
            chart: baseChart("area", 320),
            series: [{ name: "Ingresos", data: serie.map(function (x) { return Number(x.monto) || 0; }) }],
            xaxis: {
                categories: serie.map(function (x) { return fechaCorta(x.fecha); }),
                labels: LBL, axisBorder: { show: false }, axisTicks: { show: false }
            },
            yaxis: { labels: { style: LBL.style, formatter: function (v) { return money(v); } } },
            stroke: { curve: "smooth", width: 2.5 },
            fill: { type: "gradient", gradient: { shadeIntensity: 1, opacityFrom: 0.35, opacityTo: 0.02, stops: [0, 90] } },
            colors: [CLR.accent],
            markers: { size: 3, strokeWidth: 2, strokeColors: "#fff", hover: { size: 6 } },
            dataLabels: { enabled: false },
            grid: GRID,
            tooltip: { theme: "light", y: { formatter: function (v) { return money(v); } } }
        });
        charts.ingresos.render();
    }

    /* ══════════════════════ CITAS ══════════════════════ */
    function renderCitas(d) {
        document.getElementById("cit-total").textContent      = entero(d.total);
        document.getElementById("cit-atendidas").textContent  = entero(d.atendidas);
        document.getElementById("cit-noshow").textContent     = entero(d.noShow);
        document.getElementById("cit-canceladas").textContent = entero(d.canceladas);

        var rows = d.porDoctor || [];
        var tb = document.getElementById("cit-doctores");
        if (!rows.length) {
            tb.innerHTML = '<tr><td colspan="2"><div class="rep-empty"><i class="fas fa-user-doctor"></i>Sin citas en el período.</div></td></tr>';
            return;
        }
        tb.innerHTML = rows.map(function (r) {
            return '<tr><td class="strong">' + esc(r.doctor) + '</td><td class="num">' + entero(r.cantidad) + '</td></tr>';
        }).join("");
    }

    /* ══════════════════════ SERVICIOS ══════════════════════ */
    function renderServicios(d) {
        var items = d.items || [];

        var tb = document.getElementById("serv-tabla");
        if (!items.length) {
            tb.innerHTML = '<tr><td colspan="3"><div class="rep-empty"><i class="fas fa-briefcase-medical"></i>Sin servicios en el período.</div></td></tr>';
        } else {
            tb.innerHTML = items.map(function (r) {
                return '<tr><td class="strong">' + esc(r.servicio) + '</td>' +
                    '<td class="num">' + entero(r.cantidad) + '</td>' +
                    '<td class="num">' + money(r.ingreso) + '</td></tr>';
            }).join("");
        }

        destroyChart("servicios");
        if (typeof ApexCharts === "undefined") return;
        charts.servicios = new ApexCharts(document.getElementById("serv-chart"), {
            chart: baseChart("bar", 340),
            plotOptions: { bar: { borderRadius: 6, columnWidth: "55%", distributed: items.length <= 12 } },
            series: [{ name: "Cantidad", data: items.map(function (x) { return Number(x.cantidad) || 0; }) }],
            xaxis: {
                categories: items.map(function (x) { return x.servicio; }),
                labels: { style: LBL.style, rotate: -35, trim: true, hideOverlappingLabels: true },
                axisBorder: { show: false }, axisTicks: { show: false }
            },
            yaxis: { labels: LBL, min: 0 },
            colors: ["#6c5ce7", "#7c6fff", "#8b7bff", "#a78bfa", "#5a4bd1", "#0984e3", "#00b894"],
            dataLabels: { enabled: true, style: { colors: ["#fff"], fontSize: "11px", fontWeight: 700 } },
            legend: { show: false },
            grid: GRID,
            tooltip: { theme: "light" }
        });
        charts.servicios.render();
    }

    /* ══════════════════════ INVENTARIO ══════════════════════ */
    function renderInventario(d) {
        var rows = d.stockBajo || [];
        var tb = document.getElementById("inv-tabla");
        if (!rows.length) {
            tb.innerHTML = '<tr><td colspan="3"><div class="rep-empty"><i class="fas fa-check"></i>Ningún producto con stock bajo.</div></td></tr>';
            return;
        }
        tb.innerHTML = rows.map(function (r) {
            var stock = Number(r.stock) || 0, min = Number(r.minimo) || 0;
            var chip = stock <= min ? "low" : "ok";
            return '<tr><td class="strong">' + esc(r.item) + '</td>' +
                '<td class="num"><span class="rep-stock-chip ' + chip + '">' + entero(stock) + '</span></td>' +
                '<td class="num">' + entero(min) + '</td></tr>';
        }).join("");
    }

    /* ══════════════════════ EXPORTAR CSV ══════════════════════ */
    /* El archivo lo genera el servidor (con BOM UTF-8); aquí solo armamos la
       URL con el rango de fechas y disparamos la descarga con un enlace. */
    function exportCsv(tab) {
        var base = urls[tab + "Csv"];
        if (!base) return;
        var url = base;
        if (tab !== "inventario") {   // inventario no filtra por fecha
            var r = rango();
            url += "?desde=" + encodeURIComponent(r.desde) + "&hasta=" + encodeURIComponent(r.hasta);
        }
        var a = document.createElement("a");
        a.href = url;
        document.body.appendChild(a); a.click(); document.body.removeChild(a);
    }

    /* ══════════════════════ IMPRIMIR / PDF ══════════════════════ */
    /* Rellena el encabezado de impresión (título del reporte, rango y fecha de
       generación) según la pestaña activa y lanza el diálogo del navegador
       (el usuario elige "Guardar como PDF"). */
    var TITULOS = {
        ingresos:   "Reporte de Ingresos",
        citas:      "Reporte de Citas",
        servicios:  "Reporte de Servicios",
        inventario: "Reporte de Inventario — Stock bajo"
    };
    function printTab() {
        var titleEl = document.getElementById("repPrintTitle");
        var metaEl  = document.getElementById("repPrintMeta");
        if (titleEl) titleEl.textContent = TITULOS[activeTab] || "Reporte";
        if (metaEl) {
            var partes = [];
            if (activeTab !== "inventario") {
                var r = rango();
                partes.push("Período: " + (r.desde || "—") + " a " + (r.hasta || "—"));
            }
            partes.push("Generado: " + new Date().toLocaleString("es-HN"));
            metaEl.textContent = partes.join("   ·   ");
        }
        window.print();
    }

    /* ══════════════════════ EVENTOS ══════════════════════ */
    document.querySelectorAll(".rep-tab").forEach(function (b) {
        b.addEventListener("click", function () {
            var tab = b.getAttribute("data-tab");
            setActive(tab);
            loadTab(tab);   // usa cache si ya se cargó
        });
    });

    document.querySelectorAll("[data-export]").forEach(function (b) {
        b.addEventListener("click", function () { exportCsv(b.getAttribute("data-export")); });
    });
    document.querySelectorAll("[data-print]").forEach(function (b) {
        b.addEventListener("click", function () { printTab(); });
    });

    function onFechaChange() {
        cache = {};   // los datos dependen del rango → invalidar todo
        loadTab(activeTab, true);   // recargar solo la pestaña activa
    }
    desdeEl.addEventListener("change", onFechaChange);
    hastaEl.addEventListener("change", onFechaChange);

    /* ── Arranque: cargar la pestaña activa (Ingresos) ── */
    loadTab(activeTab);
})();
