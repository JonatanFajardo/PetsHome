// ============================================================
// POS de cobros pendientes POR VISITA (feature/cobro-por-visita)
// Cada grupo = una visita (cita). Se cobra la visita con una DISPOSICIÓN:
//   Pagado | PorCobrar (+ vencimiento) | SinCargo (+ motivo).
// Los cargos sin cita_Id (legacy) se agrupan aparte como "Sin visita asignada".
// ============================================================
(function () {
    "use strict";
    var urls = window._urlsCobros;
    if (!urls) return;

    var body = document.getElementById("cobrosBody");
    var totalEl = document.getElementById("cobrosTotal");

    // Modal
    var modal = document.getElementById("cobModal");
    var modalSub = document.getElementById("cobModalSub");
    var errEl = document.getElementById("cobErr");
    var vencBox = document.getElementById("cobVenc");
    var vencInput = document.getElementById("cobVencInput");
    var motivoBox = document.getElementById("cobMotivo");
    var motivoInput = document.getElementById("cobMotivoInput");
    var btnConfirm = document.getElementById("cobConfirm");
    var btnCancel = document.getElementById("cobCancel");
    var currentCita = null;

    function money(n) { return "L " + (Number(n) || 0).toFixed(2); }
    function esc(s) { var d = document.createElement("div"); d.textContent = s == null ? "" : String(s); return d.innerHTML; }
    function fecha(s) { if (!s) return "—"; var d = new Date(s); return isNaN(d) ? "—" : d.toLocaleDateString("es-HN"); }

    var post = window.postForm; // token y headers los maneja el helper global (http.js)

    function cargar() {
        window.getJson(urls.list)
            .then(function (res) { render(res.data || []); })
            .catch(function () { body.innerHTML = '<div class="cobros-empty">No se pudieron cargar los cobros.</div>'; });
    }

    // Agrupa las filas por cita_Id (null => "Sin visita asignada")
    function agrupar(rows) {
        var map = {}, orden = [];
        rows.forEach(function (r) {
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

    function render(rows) {
        if (!rows.length) {
            body.innerHTML = '<div class="cobros-empty"><i class="bi bi-check-circle"></i> No hay cobros pendientes.</div>';
            totalEl.textContent = "";
            return;
        }
        var grupos = agrupar(rows);
        var total = rows.reduce(function (a, r) { return a + (r.cargo_Total || 0); }, 0);
        totalEl.textContent = "Total pendiente: " + money(total);

        var html = "";
        grupos.forEach(function (g) {
            html += '<div class="visita-card">';
            html += '<div class="visita-top">';
            if (g.sin) {
                html += '<span class="cita-badge sin"><i class="bi bi-exclamation-triangle"></i> Sin visita asignada</span>';
            } else {
                html += '<span class="cita-badge"><i class="bi bi-calendar-event"></i> Visita #' + g.citaId + '</span>';
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
                    '<i class="bi bi-cash"></i> Cobrar visita</button>';
            }
            html += '</div>';
            html += '</div>';
        });
        body.innerHTML = html;
        wire();
    }

    function wire() {
        body.querySelectorAll("[data-cobrar]").forEach(function (b) {
            b.addEventListener("click", function () {
                abrirModal(b.getAttribute("data-cobrar"), b.getAttribute("data-masc"),
                           b.getAttribute("data-cli"), b.getAttribute("data-total"));
            });
        });
    }

    // ----- Modal -----
    function seleccion() { var r = modal.querySelector("input[name='disp']:checked"); return r ? r.value : "Pagado"; }
    function pintarExtras() {
        var v = seleccion();
        vencBox.style.display = (v === "PorCobrar") ? "" : "none";
        motivoBox.style.display = (v === "SinCargo") ? "" : "none";
        modal.querySelectorAll(".cob-opt").forEach(function (o) {
            o.classList.toggle("sel", o.querySelector("input").checked);
        });
    }
    modal.querySelectorAll("input[name='disp']").forEach(function (i) { i.addEventListener("change", pintarExtras); });

    function abrirModal(citaId, masc, cli, total) {
        currentCita = citaId;
        errEl.textContent = "";
        modalSub.textContent = "Visita #" + citaId + " · " + masc + " · " + cli + " · " + money(total);
        modal.querySelector("input[value='Pagado']").checked = true;
        vencInput.value = ""; motivoInput.value = "";
        pintarExtras();
        modal.classList.add("open");
    }
    function cerrarModal() { modal.classList.remove("open"); currentCita = null; }
    btnCancel.addEventListener("click", cerrarModal);
    modal.addEventListener("click", function (e) { if (e.target === modal) cerrarModal(); });

    btnConfirm.addEventListener("click", function () {
        if (!currentCita) return;
        var disp = seleccion();
        var data = { citaId: currentCita, disposicion: disp };
        errEl.textContent = "";

        if (disp === "PorCobrar") {
            if (!vencInput.value) { errEl.textContent = "Indica la fecha de vencimiento."; return; }
            data.vencimiento = vencInput.value;
        }
        if (disp === "SinCargo") {
            var m = motivoInput.value.trim();
            if (m.length < 5) { errEl.textContent = "El motivo debe tener al menos 5 caracteres."; return; }
            data.motivo = m;
        }

        btnConfirm.disabled = true;
        // Para Pagado/PorCobrar abrimos la pestaña DENTRO del gesto del clic (evita bloqueo de popup).
        var win = (disp === "SinCargo") ? null : window.open("about:blank", "_blank");

        post(urls.facturarVisita, data).then(function (res) {
            btnConfirm.disabled = false;
            if (res.success) {
                cerrarModal();
                if (res.factId) {
                    var url = urls.comprobante + "/" + res.factId;
                    if (win) { win.location = url; } else { window.location = url; }
                } else if (win) { win.close(); }
                cargar();
            } else {
                if (win) win.close();
                errEl.textContent = res.message || "No se pudo cobrar la visita.";
            }
        }).catch(function () {
            btnConfirm.disabled = false;
            if (win) win.close();
            errEl.textContent = "Error de red.";
        });
    });

    cargar();
})();
