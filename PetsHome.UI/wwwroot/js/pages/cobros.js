// ============================================================
// POS de cobros pendientes por dispensación (Mejoras v3 - Fase 3)
// ============================================================
(function () {
    "use strict";
    var urls = window._urlsCobros;
    if (!urls) return;

    var body = document.getElementById("cobrosBody");
    var totalEl = document.getElementById("cobrosTotal");

    function money(n) { return "L " + (Number(n) || 0).toFixed(2); }

    function post(url, data) {
        return fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8", "X-Requested-With": "XMLHttpRequest" },
            body: new URLSearchParams(data).toString()
        }).then(function (r) { return r.json(); });
    }

    function cargar() {
        fetch(urls.list, { headers: { "X-Requested-With": "XMLHttpRequest" } })
            .then(function (r) { return r.json(); })
            .then(function (res) { render(res.data || []); })
            .catch(function () { body.innerHTML = '<div class="cobros-empty">No se pudieron cargar los cobros.</div>'; });
    }

    function render(rows) {
        if (!rows.length) {
            body.innerHTML = '<div class="cobros-empty"><i class="bi bi-check-circle"></i> No hay cobros pendientes.</div>';
            totalEl.textContent = "";
            return;
        }
        var total = rows.reduce(function (a, r) { return a + (r.cargo_Total || 0); }, 0);
        totalEl.textContent = "Total pendiente: " + money(total);

        var html = '<table class="cobros-table"><thead><tr>' +
            '<th>Cliente</th><th>Mascota</th><th>Medicamento</th>' +
            '<th class="txt-right">Cant</th><th class="txt-right">Precio</th><th class="txt-right">Total</th>' +
            '<th class="txt-right">Acciones</th></tr></thead><tbody>';

        rows.forEach(function (r) {
            html += '<tr>' +
                '<td><span class="cli-badge">#' + r.cli_Id + '</span></td>' +
                '<td>' + (r.mascotaNombre || r.MascotaNombre || '—') + '</td>' +
                '<td>' + (r.cargo_DescripcionSnap || '') + '</td>' +
                '<td class="txt-right">' + r.cargo_Cantidad + '</td>' +
                '<td class="txt-right">' + money(r.cargo_PrecioUnitSnap) + '</td>' +
                '<td class="txt-right"><strong>' + money(r.cargo_Total) + '</strong></td>' +
                '<td class="txt-right">' +
                    '<button class="btn-cobrar" data-cobrar="' + r.cli_Id + '">Cobrar cliente</button> ' +
                    '<button class="btn-condonar" data-condonar="' + r.cargo_Id + '">Condonar</button>' +
                '</td></tr>';
        });
        html += '</tbody></table>';
        body.innerHTML = html;
        wire();
    }

    function wire() {
        body.querySelectorAll("[data-cobrar]").forEach(function (b) {
            b.addEventListener("click", function () {
                var cli = b.getAttribute("data-cobrar");
                if (!confirm("¿Generar factura con TODOS los cargos pendientes del cliente #" + cli + "?")) return;
                b.disabled = true;
                post(urls.facturar, { cliId: cli }).then(function (res) {
                    alert(res.message || (res.success ? "Facturado." : "Error."));
                    cargar();
                }).catch(function () { b.disabled = false; alert("Error de red."); });
            });
        });
        body.querySelectorAll("[data-condonar]").forEach(function (b) {
            b.addEventListener("click", function () {
                var cargo = b.getAttribute("data-condonar");
                var motivo = prompt("Motivo de condonación (mínimo 5 caracteres):");
                if (motivo === null) return;
                if (motivo.trim().length < 5) { alert("El motivo debe tener al menos 5 caracteres."); return; }
                post(urls.condonar, { cargoId: cargo, motivo: motivo.trim() }).then(function (res) {
                    if (res.success) cargar();
                    else alert(res.message || "No se pudo condonar.");
                });
            });
        });
    }

    cargar();
})();
