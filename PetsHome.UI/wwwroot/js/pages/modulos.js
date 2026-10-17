/* ============================================================
   Ajustes · Módulos del sistema
   - POST AJAX del toggle -> Configuracion/SetModulo
   - Grafo de dependencias en cliente (data-attributes):
       coma (,) = AND  (requiere todos)
       pipe (|) = OR   (requiere al menos uno)
   ============================================================ */
(function () {
    "use strict";

    var urls = window._urls || {};

    // Lee el grafo desde el DOM
    function leerModulos() {
        var map = {};
        document.querySelectorAll(".modulo-card").forEach(function (card) {
            map[card.dataset.codigo] = {
                codigo: card.dataset.codigo,
                nombre: card.dataset.nombre,
                nucleo: card.dataset.nucleo === "true",
                activo: card.dataset.activo === "true",
                requiere: (card.dataset.requiere || "").trim(),
                el: card
            };
        });
        return map;
    }

    var modulos = leerModulos();

    function codigosActivos() {
        return Object.keys(modulos).filter(function (c) { return modulos[c].activo; });
    }

    // ¿Se cumple la expresión de requisitos con los módulos activos dados?
    function cumple(req, activos) {
        if (!req) return true;
        var partes;
        if (req.indexOf("|") !== -1) {
            partes = req.split("|").map(trim).filter(Boolean);
            return partes.some(function (p) { return activos.indexOf(p) !== -1; });
        }
        partes = req.split(",").map(trim).filter(Boolean);
        return partes.every(function (p) { return activos.indexOf(p) !== -1; });
    }

    function trim(s) { return (s || "").trim(); }

    function nombre(codigo) {
        return modulos[codigo] ? modulos[codigo].nombre : codigo;
    }

    function etiquetaReq(req) {
        if (!req) return "";
        var or = req.indexOf("|") !== -1;
        var sep = or ? " o " : " y ";
        return req.split(or ? "|" : ",").map(trim).filter(Boolean).map(nombre).join(sep);
    }

    // Módulos activos que dependen directamente de `codigo` y quedarían rotos si se apaga
    function dependientesRotos(codigoApagar) {
        var activos = codigosActivos().filter(function (c) { return c !== codigoApagar; });
        var rotos = [];
        Object.keys(modulos).forEach(function (c) {
            var m = modulos[c];
            if (!m.activo || m.codigo === codigoApagar) return;
            if (!m.requiere) return;
            // mencionado en la expresión y ya no se cumple sin el módulo apagado
            var mencionado = m.requiere.split(/[,|]/).map(trim).indexOf(codigoApagar) !== -1;
            if (mencionado && !cumple(m.requiere, activos)) {
                rotos.push(m.nombre);
            }
        });
        return rotos;
    }

    // Recalcula el estado habilitado/atenuado de todos los toggles tras un cambio
    function refrescarEstados() {
        var activos = codigosActivos();
        Object.keys(modulos).forEach(function (c) {
            var m = modulos[c];
            var input = m.el.querySelector(".modulo-toggle");
            var label = m.el.querySelector(".switch");
            var notaReq = m.el.querySelector(".modulo-nota-req");

            m.el.classList.toggle("is-on", m.activo);
            m.el.classList.toggle("is-off", !m.activo);
            input.checked = m.activo;
            m.el.dataset.activo = m.activo ? "true" : "false";

            if (m.nucleo) {
                input.disabled = true;
                label.classList.add("switch-disabled");
                return;
            }

            var ok = cumple(m.requiere, activos);
            // Bloquear encender si no se cumple el requisito (pero siempre se puede apagar)
            var bloquear = !m.activo && !ok;
            input.disabled = bloquear;
            label.classList.toggle("switch-disabled", bloquear);

            if (notaReq) {
                var lbl = etiquetaReq(m.requiere);
                if (lbl) {
                    notaReq.style.display = "";
                    notaReq.querySelector("span").textContent = lbl;
                    notaReq.classList.toggle("req-no-cumplido", !ok && !m.activo);
                } else {
                    notaReq.style.display = "none";
                }
            }
        });
    }

    function enviar(codigo, activo, input) {
        var tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
        var token = tokenEl ? tokenEl.value : "";

        var body = new URLSearchParams();
        body.append("codigo", codigo);
        body.append("activo", activo);

        input.closest(".modulo-card").classList.add("saving");

        fetch(urls.setModulo, {
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
                "RequestVerificationToken": token,
                "X-Requested-With": "XMLHttpRequest"
            },
            body: body.toString()
        })
            .then(function (r) { return r.json(); })
            .then(function (res) {
                input.closest(".modulo-card").classList.remove("saving");
                if (res && res.success) {
                    modulos[codigo].activo = activo;
                    refrescarEstados();
                    toast(res.message || "Cambio guardado.", true);
                } else {
                    // revertir
                    input.checked = !activo;
                    toast((res && res.message) || "No se pudo guardar el cambio.", false);
                }
            })
            .catch(function () {
                input.closest(".modulo-card").classList.remove("saving");
                input.checked = !activo;
                toast("Error de red al guardar el cambio.", false);
            });
    }

    function onToggle(e) {
        var input = e.target;
        var card = input.closest(".modulo-card");
        var codigo = card.dataset.codigo;
        var activo = input.checked; // estado deseado

        if (modulos[codigo].nucleo) {
            input.checked = true;
            return;
        }

        if (activo) {
            // Encender: validar requisito (defensa extra; el toggle ya estaría disabled)
            var activosSin = codigosActivos();
            if (!cumple(modulos[codigo].requiere, activosSin)) {
                input.checked = false;
                toast("Requiere activar primero: " + etiquetaReq(modulos[codigo].requiere), false);
                return;
            }
            enviar(codigo, true, input);
        } else {
            // Apagar: advertir si rompe dependientes activos
            var rotos = dependientesRotos(codigo);
            if (rotos.length) {
                var msg = "Al apagar “" + modulos[codigo].nombre + "” estos módulos quedarán sin su requisito:\n\n• "
                    + rotos.join("\n• ") + "\n\n¿Deseas continuar?";
                if (!window.confirm(msg)) {
                    input.checked = true;
                    return;
                }
            }
            enviar(codigo, false, input);
        }
    }

    // Toast minimal (sin dependencias)
    function toast(text, ok) {
        var t = document.createElement("div");
        t.className = "modulo-toast " + (ok ? "toast-ok" : "toast-err");
        t.innerHTML = '<i class="fas ' + (ok ? "fa-check-circle" : "fa-exclamation-circle") + '"></i>' + text;
        document.body.appendChild(t);
        requestAnimationFrame(function () { t.classList.add("show"); });
        setTimeout(function () {
            t.classList.remove("show");
            setTimeout(function () { t.remove(); }, 300);
        }, 3200);
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll(".modulo-toggle").forEach(function (input) {
            input.addEventListener("change", onToggle);
        });
        refrescarEstados();
    });
})();
