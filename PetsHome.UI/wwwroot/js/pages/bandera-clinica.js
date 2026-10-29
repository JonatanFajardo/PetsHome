// ============================================================
// Banderas rojas clínicas (alergias + padecimientos)
// Carga el alert bar por AJAX y gestiona alta/corrección.
// Requiere window._urlsBandera (definido en _AlertBarHost.cshtml).
// ============================================================
(function () {
    "use strict";

    var host = document.getElementById("banderaClinicaHost");
    if (!host || !window._urlsBandera) return;

    var urls = window._urlsBandera;
    var mascId = parseInt(host.getAttribute("data-mascid"), 10) || 0;
    if (mascId <= 0) return;

    function cargarBar() {
        fetch(urls.bar + "?mascId=" + mascId, { headers: { "X-Requested-With": "XMLHttpRequest" } })
            .then(function (r) { return r.text(); })
            .then(function (html) { host.innerHTML = html; wire(); })
            .catch(function () { host.innerHTML = '<div style="color:#8b80b8;font-size:.85rem">No se pudieron cargar las banderas clínicas.</div>'; });
    }

    var post = window.postForm; // token y headers los maneja el helper global (http.js)

    function wire() {
        var root = host.querySelector(".bclin");
        if (!root) return;

        // abrir form inline
        root.querySelectorAll("[data-add]").forEach(function (btn) {
            btn.addEventListener("click", function () {
                var tipo = btn.getAttribute("data-add");
                var form = root.querySelector('.bclin-form[data-form="' + tipo + '"]');
                if (form) { form.hidden = false; var inp = form.querySelector("input"); if (inp) inp.focus(); }
            });
        });

        // cancelar form
        root.querySelectorAll(".f-cancel").forEach(function (btn) {
            btn.addEventListener("click", function () {
                var form = btn.closest(".bclin-form");
                if (form) { form.hidden = true; form.querySelectorAll("input").forEach(function (i) { i.value = ""; }); }
            });
        });

        // guardar alergia
        var saveAlerg = root.querySelector('[data-save="alergia"]');
        if (saveAlerg) {
            saveAlerg.addEventListener("click", function () {
                var form = saveAlerg.closest(".bclin-form");
                var sustancia = form.querySelector(".f-sustancia").value.trim();
                if (!sustancia) { form.querySelector(".f-sustancia").focus(); return; }
                saveAlerg.disabled = true;
                post(urls.agregarAlergia, {
                    masc_Id: mascId,
                    alrg_Sustancia: sustancia,
                    alrg_Reaccion: form.querySelector(".f-reaccion").value.trim(),
                    alrg_Gravedad: form.querySelector(".f-gravedad").value
                }).then(function (res) {
                    if (res && res.success) cargarBar();
                    else { saveAlerg.disabled = false; alert((res && res.message) || "No se pudo guardar la alergia."); }
                }).catch(function () { saveAlerg.disabled = false; alert("Error de red al guardar."); });
            });
        }

        // guardar padecimiento
        var savePade = root.querySelector('[data-save="padecimiento"]');
        if (savePade) {
            savePade.addEventListener("click", function () {
                var form = savePade.closest(".bclin-form");
                var nombre = form.querySelector(".f-nombre").value.trim();
                if (!nombre) { form.querySelector(".f-nombre").focus(); return; }
                savePade.disabled = true;
                post(urls.agregarPadecimiento, {
                    masc_Id: mascId,
                    pade_Nombre: nombre,
                    pade_Estado: form.querySelector(".f-estado").value
                }).then(function (res) {
                    if (res && res.success) cargarBar();
                    else { savePade.disabled = false; alert((res && res.message) || "No se pudo guardar el padecimiento."); }
                }).catch(function () { savePade.disabled = false; alert("Error de red al guardar."); });
            });
        }

        // corregir (marcar como error) alergia / padecimiento
        root.querySelectorAll("[data-corregir]").forEach(function (btn) {
            btn.addEventListener("click", function () {
                var tipo = btn.getAttribute("data-corregir");
                var id = parseInt(btn.getAttribute("data-id"), 10);
                if (!confirm("¿Marcar como error y quitar de la lista?")) return;
                var url = tipo === "alergia" ? urls.corregirAlergia : urls.corregirPadecimiento;
                var data = tipo === "alergia" ? { alrgId: id } : { padeId: id };
                post(url, data).then(function (res) {
                    if (res && res.success) cargarBar();
                    else alert((res && res.message) || "No se pudo corregir.");
                });
            });
        });
    }

    cargarBar();
})();
