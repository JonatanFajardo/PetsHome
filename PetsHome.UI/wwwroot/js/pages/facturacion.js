/* =========================================================
   facturacion.js  —  Pantalla única de Facturación
   Pestañas por estado (Todas / Por facturar / Por cobrar /
   Vencidas / Pagadas / Anuladas) sobre UNA sola fuente:
     - Facturas:      GET /Facturacion/List      (filtradas en el front)
     - Por facturar:  GET /Cobros/ListPorVisita   (visitas con cargos sin factura)
   El cobro de visita reusa el modal de Cobros + POST /Cobros/FacturarVisita.
   Peticiones vía window.getJson / window.postForm (http.js) — sin tokens a mano.
   ========================================================= */

var Facturacion = (function () {
    "use strict";

    var obj = {};
    var urls = {};

    var facturas = [];      // todas las facturas (GET /Facturacion/List)
    var porVisita = [];     // cargos pendientes por visita (GET /Cobros/ListPorVisita)
    var activeTab = "todas";

    // -------------------- Utilidades --------------------
    function money(n) {
        var v = parseFloat(n);
        if (isNaN(v)) v = 0;
        return "L. " + v.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }
    function esc(s) {
        var d = document.createElement("div");
        d.textContent = s == null ? "" : String(s);
        return d.innerHTML;
    }
    function fecha(s) {
        if (!s) return "—";
        var d = new Date(s);
        return isNaN(d) ? "—" : d.toLocaleDateString("es-HN");
    }
    function hoy0() { return new Date(new Date().toDateString()); }

    function esVencida(f) {
        if (f.fact_Estado !== "PorCobrar") return false;
        if ((parseFloat(f.saldo) || 0) <= 0) return false;
        if (!f.fact_Vencimiento) return false;
        return new Date(f.fact_Vencimiento) < hoy0();
    }

    // -------------------- Filtro por pestaña --------------------
    function filtrar(tab) {
        return facturas.filter(function (f) {
            var estado = f.fact_Estado || "";
            var saldo = parseFloat(f.saldo) || 0;
            switch (tab) {
                case "todas":     return true;
                case "porCobrar": return estado === "PorCobrar" && saldo > 0 && !esVencida(f);
                case "vencidas":  return esVencida(f);
                case "pagadas":   return estado === "Pagada";
                case "anuladas":  return estado === "Anulada" || estado === "SinCargo";
                default:          return true;
            }
        });
    }

    // -------------------- Contadores de pestañas --------------------
    function pintarContadores() {
        var counts = {
            todas: facturas.length,
            porFacturar: agrupar(porVisita).filter(function (g) { return !g.sin; }).length,
            porCobrar: filtrar("porCobrar").length,
            vencidas: filtrar("vencidas").length,
            pagadas: filtrar("pagadas").length,
            anuladas: filtrar("anuladas").length
        };
        Object.keys(counts).forEach(function (k) {
            var el = document.querySelector('.fact-count[data-count="' + k + '"]');
            if (!el) return;
            var n = counts[k];
            el.textContent = n;
            el.classList.toggle("has", n > 0);
        });
    }

    // -------------------- Badge de estado --------------------
    function badgeEstado(f) {
        if (esVencida(f)) return '<span class="fact-badge vencida">Vencida</span>';
        var e = f.fact_Estado || "";
        if (e === "Pagada")    return '<span class="fact-badge pagada">Pagada</span>';
        if (e === "PorCobrar") return '<span class="fact-badge porcobrar">Por cobrar</span>';
        if (e === "Anulada")   return '<span class="fact-badge anulada">Anulada</span>';
        if (e === "SinCargo")  return '<span class="fact-badge anulada">Sin cargo</span>';
        return '<span class="fact-badge anulada">' + esc(e || "—") + '</span>';
    }

    // -------------------- Render tabla de facturas --------------------
    function renderTabla() {
        var tbody = document.getElementById("fact-tbody");
        var rows = filtrar(activeTab);

        var q = (document.getElementById("fact-buscar").value || "").trim().toLowerCase();
        if (q) {
            rows = rows.filter(function (f) {
                return [(f.fact_Numero || ""), (f.cliente || ""), (f.mascota || "")]
                    .join(" ").toLowerCase().indexOf(q) > -1;
            });
        }

        // Total pendiente (saldo) de las filas mostradas
        var totalSaldo = rows.reduce(function (a, f) { return a + (parseFloat(f.saldo) || 0); }, 0);
        var pill = document.getElementById("fact-total-pill");
        pill.textContent = totalSaldo > 0
            ? rows.length + " factura(s) · Saldo " + money(totalSaldo)
            : rows.length + " factura(s)";

        if (!rows.length) {
            tbody.innerHTML = '<tr class="fact-empty-row"><td colspan="9">' +
                '<i class="fas fa-inbox mr-2"></i>No hay facturas en esta pestaña.</td></tr>';
            return;
        }

        rows.sort(function (a, b) { return (b.fact_Id || 0) - (a.fact_Id || 0); });

        var html = "";
        rows.forEach(function (f) {
            var saldo = parseFloat(f.saldo) || 0;
            var venc = esVencida(f);
            var saldoHtml = saldo > 0
                ? '<span class="fact-saldo-pos">' + money(saldo) + '</span>'
                : '<span class="fact-saldo-cero">' + money(0) + '</span>';
            var vencHtml = f.fact_Vencimiento
                ? (fecha(f.fact_Vencimiento) + (venc ? ' <i class="fas fa-exclamation-triangle"></i>' : ''))
                : '—';
            var accionTxt = saldo > 0 ? "Cobrar" : "Ver";

            html += '<tr' + (venc ? ' class="fact-row-vencida"' : '') + '>' +
                '<td><span style="font-family:monospace;font-weight:600;">' + esc(f.fact_Numero || "—") + '</span></td>' +
                '<td>' + esc(f.cliente || "Consumidor Final") + '</td>' +
                '<td>' + esc(f.mascota || "—") + '</td>' +
                '<td>' + fecha(f.fact_Fecha) + '</td>' +
                '<td class="text-right fact-money">' + money(f.fact_Total) + '</td>' +
                '<td class="text-right fact-money fact-saldo">' + saldoHtml + '</td>' +
                '<td class="text-center fact-vence">' + vencHtml + '</td>' +
                '<td class="text-center">' + badgeEstado(f) + '</td>' +
                '<td class="text-center">' +
                    '<a class="fact-link" href="' + urls.comprobante + '/' + f.fact_Id + '">' + accionTxt + '</a>' +
                '</td>' +
                '</tr>';
        });
        tbody.innerHTML = html;
    }

    // -------------------- "Por facturar": agrupar por visita --------------------
    // (misma lógica que cobros.js: un grupo = una cita; null => "Sin visita asignada")
    function agrupar(filas) {
        var map = {}, orden = [];
        filas.forEach(function (r) {
            var key = (r.cita_Id == null) ? "sin" : String(r.cita_Id);
            if (!map[key]) {
                map[key] = {
                    citaId: r.cita_Id, sin: r.cita_Id == null,
                    masc: r.mascotaNombre || r.MascotaNombre || "—",
                    cli: r.clienteNombre || r.ClienteNombre || ("Cliente #" + r.cli_Id),
                    fecha: r.citaFecha || r.CitaFecha,
                    motivo: r.citaMotivo || r.CitaMotivo,
                    items: [], total: 0
                };
                orden.push(key);
            }
            map[key].items.push(r);
            map[key].total += (r.cargo_Total || 0);
        });
        return orden.map(function (k) { return map[k]; });
    }

    function renderPorFacturar() {
        var body = document.getElementById("pf-body");
        var totalEl = document.getElementById("pf-total");

        if (!porVisita.length) {
            body.innerHTML = '<div class="cobros-empty"><i class="fas fa-check-circle"></i> No hay visitas pendientes de facturar.</div>';
            totalEl.textContent = "";
            return;
        }

        var grupos = agrupar(porVisita);
        var total = porVisita.reduce(function (a, r) { return a + (r.cargo_Total || 0); }, 0);
        totalEl.textContent = "Total pendiente: " + money(total);

        var html = "";
        grupos.forEach(function (g) {
            html += '<div class="visita-card"><div class="visita-top">';
            if (g.sin) {
                html += '<span class="cita-badge sin"><i class="fas fa-exclamation-triangle"></i> Sin visita asignada</span>';
            } else {
                html += '<span class="cita-badge"><i class="fas fa-calendar-day"></i> Visita #' + g.citaId + '</span>';
            }
            html += '<span class="masc">' + esc(g.masc) + '</span>';
            html += '<span class="cli">' + esc(g.cli) + '</span>';
            html += '<span class="meta">' + (g.sin ? "" : fecha(g.fecha) + (g.motivo ? " · " + esc(g.motivo) : "")) + '</span>';
            html += '<span class="visita-sum"><b>' + money(g.total) + '</b><small>' + g.items.length + ' cargo(s)</small></span>';
            html += '</div>';

            html += '<table class="cobros-table"><thead><tr>' +
                '<th>Medicamento</th><th class="txt-right">Cant</th><th class="txt-right">Precio</th><th class="txt-right">Total</th>' +
                '</tr></thead><tbody>';
            g.items.forEach(function (r) {
                html += '<tr>' +
                    '<td>' + esc(r.cargo_DescripcionSnap) + '</td>' +
                    '<td class="txt-right">' + r.cargo_Cantidad + '</td>' +
                    '<td class="txt-right">' + money(r.cargo_PrecioUnitSnap) + '</td>' +
                    '<td class="txt-right"><strong>' + money(r.cargo_Total) + '</strong></td>' +
                    '</tr>';
            });
            html += '</tbody></table>';

            html += '<div class="visita-acts">';
            if (g.sin) {
                html += '<span class="meta" style="align-self:center;color:#92650a">Cargos sin cita: asignar una visita para poder cobrarlos.</span>';
            } else {
                html += '<button class="btn-visita" data-cobrar="' + g.citaId + '" ' +
                    'data-masc="' + esc(g.masc) + '" data-cli="' + esc(g.cli) + '" data-total="' + g.total + '">' +
                    '<i class="fas fa-cash-register"></i> Cobrar visita</button>';
            }
            html += '</div></div>';
        });
        body.innerHTML = html;

        body.querySelectorAll("[data-cobrar]").forEach(function (b) {
            b.addEventListener("click", function () {
                abrirModal(b.getAttribute("data-cobrar"), b.getAttribute("data-masc"),
                           b.getAttribute("data-cli"), b.getAttribute("data-total"));
            });
        });
    }

    // -------------------- Pestañas --------------------
    function setTab(tab) {
        activeTab = tab;
        document.querySelectorAll("#fact-tabs .pos-tab").forEach(function (t) {
            t.classList.toggle("active", t.getAttribute("data-tab") === tab);
        });
        var esPF = tab === "porFacturar";
        document.getElementById("fact-table-wrap").style.display = esPF ? "none" : "";
        document.getElementById("fact-toolbar").style.display = esPF ? "none" : "flex";
        document.getElementById("fact-porfacturar").style.display = esPF ? "" : "none";
        if (esPF) renderPorFacturar();
        else renderTabla();
    }

    // -------------------- Modal "Cobrar visita" (reusado de cobros.js) --------------------
    var modal, modalSub, errEl, metodoBox, metodoInput, vencBox, vencInput,
        abonoBox, abonoInput, abonoMetodoInput, motivoBox, motivoInput,
        btnConfirm, btnCancel, currentCita = null;

    function seleccion() {
        var r = modal.querySelector("input[name='disp']:checked");
        return r ? r.value : "Pagado";
    }
    function pintarExtras() {
        var v = seleccion();
        metodoBox.style.display = (v === "Pagado") ? "" : "none";
        vencBox.style.display = (v === "PorCobrar") ? "" : "none";
        abonoBox.style.display = (v === "PorCobrar") ? "" : "none";
        motivoBox.style.display = (v === "SinCargo") ? "" : "none";
        modal.querySelectorAll(".cob-opt").forEach(function (o) {
            o.classList.toggle("sel", o.querySelector("input").checked);
        });
    }
    function abrirModal(citaId, masc, cli, total) {
        currentCita = citaId;
        errEl.textContent = "";
        modalSub.textContent = "Visita #" + citaId + " · " + masc + " · " + cli + " · " + money(total);
        modal.querySelector("input[value='Pagado']").checked = true;
        vencInput.value = ""; motivoInput.value = "";
        metodoInput.value = "Efectivo";
        abonoInput.value = ""; abonoMetodoInput.value = "Efectivo";
        pintarExtras();
        modal.classList.add("open");
    }
    function cerrarModal() { modal.classList.remove("open"); currentCita = null; }

    function confirmar() {
        if (!currentCita) return;
        var disp = seleccion();
        // metodo y abonoInicial siempre en el form (nunca null): por defecto Efectivo / 0.
        var data = { citaId: currentCita, disposicion: disp, metodo: "Efectivo", abonoInicial: 0 };
        errEl.textContent = "";

        if (disp === "Pagado") {
            data.metodo = metodoInput.value || "Efectivo";
        }
        if (disp === "PorCobrar") {
            if (!vencInput.value) { errEl.textContent = "Indica la fecha de vencimiento."; return; }
            data.vencimiento = vencInput.value;
            var abono = parseFloat(abonoInput.value);
            if (isNaN(abono) || abono < 0) abono = 0;
            if (abono > 0) {
                data.abonoInicial = abono;
                data.metodo = abonoMetodoInput.value || "Efectivo";
            }
        }
        if (disp === "SinCargo") {
            var m = motivoInput.value.trim();
            if (m.length < 5) { errEl.textContent = "El motivo debe tener al menos 5 caracteres."; return; }
            data.motivo = m;
        }

        btnConfirm.disabled = true;
        // Para Pagado/PorCobrar abrimos la pestaña DENTRO del gesto del clic (evita bloqueo de popup).
        var win = (disp === "SinCargo") ? null : window.open("about:blank", "_blank");

        window.postForm(urls.facturarVisita, data).then(function (res) {
            btnConfirm.disabled = false;
            if (res.success) {
                cerrarModal();
                if (res.factId) {
                    var url = urls.comprobante + "/" + res.factId;
                    if (win) { win.location = url; } else { window.location = url; }
                } else if (win) { win.close(); }
                recargar();
            } else {
                if (win) win.close();
                errEl.textContent = res.message || "No se pudo cobrar la visita.";
            }
        }).catch(function () {
            btnConfirm.disabled = false;
            if (win) win.close();
            errEl.textContent = "Error de red.";
        });
    }

    function wireModal() {
        modal = document.getElementById("cobModal");
        modalSub = document.getElementById("cobModalSub");
        errEl = document.getElementById("cobErr");
        metodoBox = document.getElementById("cobMetodo");
        metodoInput = document.getElementById("cobMetodoInput");
        vencBox = document.getElementById("cobVenc");
        vencInput = document.getElementById("cobVencInput");
        abonoBox = document.getElementById("cobAbono");
        abonoInput = document.getElementById("cobAbonoInput");
        abonoMetodoInput = document.getElementById("cobAbonoMetodoInput");
        motivoBox = document.getElementById("cobMotivo");
        motivoInput = document.getElementById("cobMotivoInput");
        btnConfirm = document.getElementById("cobConfirm");
        btnCancel = document.getElementById("cobCancel");

        modal.querySelectorAll("input[name='disp']").forEach(function (i) {
            i.addEventListener("change", pintarExtras);
        });
        btnCancel.addEventListener("click", cerrarModal);
        modal.addEventListener("click", function (e) { if (e.target === modal) cerrarModal(); });
        btnConfirm.addEventListener("click", confirmar);
    }

    // -------------------- Tarjetas de totales (cuentas por cobrar) --------------------
    function setMonto(id, val) {
        var el = document.getElementById(id);
        if (el) el.textContent = money(val);
    }
    function cargarResumenAR() {
        if (!urls.resumenAR) return Promise.resolve();
        return window.getJson(urls.resumenAR).then(function (r) {
            r = r || {};
            setMonto("ar-porcobrar", r.porCobrar);
            setMonto("ar-vencido", r.vencido);
            setMonto("ar-cobradomes", r.cobradoMes);
        }).catch(function () { /* deja L. 0.00 */ });
    }

    // -------------------- Carga de datos --------------------
    function recargar() {
        return Promise.all([
            window.getJson(urls.list).then(function (r) { facturas = (r && r.data) || []; }).catch(function () { facturas = []; }),
            window.getJson(urls.listPorVisita).then(function (r) { porVisita = (r && r.data) || []; }).catch(function () { porVisita = []; }),
            cargarResumenAR()
        ]).then(function () {
            pintarContadores();
            setTab(activeTab);
        });
    }

    // -------------------- Alternar vista Lista / Nueva factura --------------------
    function mostrarComposer() {
        document.getElementById("view-lista").style.display = "none";
        document.getElementById("view-pos-composer").style.display = "";
    }
    function mostrarLista() {
        document.getElementById("view-pos-composer").style.display = "none";
        document.getElementById("view-lista").style.display = "";
    }

    // -------------------- Init --------------------
    obj.init = function (cfg) {
        urls = cfg || {};
        wireModal();

        document.querySelectorAll("#fact-tabs .pos-tab").forEach(function (t) {
            t.addEventListener("click", function () { setTab(t.getAttribute("data-tab")); });
        });

        var buscar = document.getElementById("fact-buscar");
        if (buscar) buscar.addEventListener("input", function () {
            if (activeTab !== "porFacturar") renderTabla();
        });

        document.getElementById("btn-nueva-factura").addEventListener("click", mostrarComposer);
        document.getElementById("btn-volver-facturas").addEventListener("click", function () {
            mostrarLista();
            recargar();
        });

        // Cuando el POS emite una factura: volver a la lista y refrescar.
        window.onFacturaCreada = function () {
            mostrarLista();
            activeTab = "todas";
            recargar();
        };

        recargar();
    };

    return obj;
}());
