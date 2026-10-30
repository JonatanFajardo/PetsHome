// ============================================================
// Whiteboard / tablero Kanban de recepción (Mejoras v3 - Fase 4)
// Carga por AJAX, auto-refresca, y permite arrastrar tarjetas
// entre columnas → cambia el estado de la visita (CambiarEstado).
// ============================================================
(function () {
    "use strict";
    var urls = window._urlsWb;
    if (!urls) return;

    var board = document.getElementById("wbBoard");
    var bodies = {
        "En espera": board.querySelector('[data-drop="En espera"]'),
        "En atencion": board.querySelector('[data-drop="En atencion"]'),
        "Finalizada": board.querySelector('[data-drop="Finalizada"]')
    };
    var dragId = null;
    var refreshTimer = null;

    function esc(s) { return (s || "").replace(/[&<>"]/g, function (c) { return { "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;" }[c]; }); }
    function tiempo(m) { m = Number(m) || 0; if (m < 60) return "hace " + m + "m"; return "hace " + Math.floor(m / 60) + "h " + (m % 60) + "m"; }

    function cargar() {
        fetch(urls.data, { headers: { "X-Requested-With": "XMLHttpRequest" } })
            .then(function (r) { return r.json(); })
            .then(render)
            .catch(function () {});
    }

    function render(rows) {
        var groups = { "En espera": [], "En atencion": [], "Finalizada": [] };
        (rows || []).forEach(function (r) { if (groups[r.estado]) groups[r.estado].push(r); });

        Object.keys(bodies).forEach(function (estado) {
            var body = bodies[estado];
            var list = groups[estado];
            body.parentElement.querySelector("[data-count]").textContent = list.length;
            if (!list.length) { body.innerHTML = '<div class="wb-empty">Sin pacientes</div>'; return; }
            body.innerHTML = list.map(card).join("");
        });
        wireCards();
    }

    function card(r) {
        var prio = r.prioridad || "Normal";
        var prioCls = (prio === "Urgente" || prio === "Alta") ? " prio-" + prio : "";
        var quien = r.esInterno ? '<span class="wb-tag interno">Interno</span>'
                                : (r.duenoNombre ? '<span class="wb-sub">' + esc(r.duenoNombre) + '</span>' : '');
        var vet = r.vetAsignado ? '<span class="wb-tag">Dr(a). ' + esc(r.vetAsignado) + '</span>' : '';
        var prioTag = (prio === "Urgente" || prio === "Alta") ? '<span class="wb-tag prio-' + prio + '">' + prio + '</span>' : '';
        var tipo = r.tipoConsulta ? '<span class="wb-tag">' + esc(r.tipoConsulta) + '</span>' : '';
        // Finalizada es terminal → sin acciones. En las columnas activas se puede
        // sacar la tarjeta del tablero marcándola "No asistió" o "Cancelar".
        var esFinal = /finaliz/i.test(r.estado || "");
        var acciones = esFinal ? "" :
            '<div class="wb-card-actions">' +
                '<button type="button" class="wb-act wb-act-noshow" data-cita="' + r.citaId + '" data-estado="No asistio" title="Marcar que no asistió">' +
                    '<i class="bi bi-person-x"></i> No asistió</button>' +
                '<button type="button" class="wb-act wb-act-cancel" data-cita="' + r.citaId + '" data-estado="Cancelada" title="Cancelar la visita">' +
                    '<i class="bi bi-x-circle"></i> Cancelar</button>' +
            '</div>';
        return '<div class="wb-card' + prioCls + '" draggable="true" data-cita="' + r.citaId + '">' +
            '<div class="wb-card-top"><span class="wb-pet">' + esc(r.mascotaNombre || "Mascota") + '</span>' +
            '<span class="wb-min">' + tiempo(r.minutosEspera) + '</span></div>' +
            (r.esInterno ? '' : (r.duenoNombre ? '<div class="wb-sub">' + esc(r.duenoNombre) + '</div>' : '')) +
            (r.motivo ? '<div class="wb-sub">' + esc(r.motivo) + '</div>' : '') +
            '<div class="wb-tags">' + (r.esInterno ? '<span class="wb-tag interno">Interno</span>' : '') + prioTag + tipo + vet + '</div>' +
            acciones +
            '</div>';
    }

    function wireCards() {
        board.querySelectorAll(".wb-card").forEach(function (c) {
            c.addEventListener("dragstart", function () { dragId = c.getAttribute("data-cita"); c.style.opacity = ".5"; });
            c.addEventListener("dragend", function () { c.style.opacity = "1"; });
        });
    }

    Object.keys(bodies).forEach(function (estado) {
        var body = bodies[estado];
        body.addEventListener("dragover", function (e) { e.preventDefault(); body.classList.add("wb-over"); });
        body.addEventListener("dragleave", function () { body.classList.remove("wb-over"); });
        body.addEventListener("drop", function (e) {
            e.preventDefault();
            body.classList.remove("wb-over");
            if (!dragId) return;
            var cita = dragId; dragId = null;
            var nuevo = body.getAttribute("data-drop");
            window.postForm(urls.cambiarEstado, { citaId: cita, estado: nuevo })
              .then(function (res) { if (!res.ok) alert(res.mensaje || "No se pudo cambiar el estado."); cargar(); })
              .catch(function () { cargar(); });
        });
    });

    // Acciones por tarjeta: "No asistió" / "Cancelar" → sacan la tarjeta del tablero.
    // No tocan el drag&drop entre las 3 columnas (son botones, no zonas de drop).
    board.addEventListener("click", function (e) {
        var btn = e.target.closest(".wb-act");
        if (!btn) return;
        e.stopPropagation();
        var cita = btn.getAttribute("data-cita");
        var estado = btn.getAttribute("data-estado");
        var label = (estado === "Cancelada") ? "Cancelar la visita" : "Marcar como \"No asistió\"";
        function doIt() {
            window.postForm(urls.cambiarEstado, { citaId: cita, estado: estado })
              .then(function (res) { if (!res.ok) alert(res.mensaje || "No se pudo cambiar el estado."); cargar(); })
              .catch(function () { cargar(); });
        }
        if (window.Swal) {
            Swal.fire({
                icon: "warning", title: "¿" + label + "?",
                text: "La tarjeta saldrá del tablero.",
                showCancelButton: true, confirmButtonText: "Sí", cancelButtonText: "No"
            }).then(function (r) { if (r.isConfirmed) doIt(); });
        } else if (confirm("¿" + label + "?")) { doIt(); }
    });
    // Evitar que al presionar un botón de acción se inicie el arrastre de la tarjeta.
    board.addEventListener("mousedown", function (e) { if (e.target.closest(".wb-act")) e.stopPropagation(); });

    cargar();
    refreshTimer = setInterval(cargar, 30000); // auto-refresh cada 30s
    window.addEventListener("beforeunload", function () { if (refreshTimer) clearInterval(refreshTimer); });
})();
