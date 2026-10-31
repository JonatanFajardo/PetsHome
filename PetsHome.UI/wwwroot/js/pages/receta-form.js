/*
 * receta-form.js — Conecta la Calculadora de dosis con el formulario de Receta
 * (Crear / Editar). Usa el modulo reutilizable window.DosisCalculator.
 *
 * El contexto del paciente llega desde la vista en window._dosisCtx:
 *   { especie, peso }   (peso puede venir null si la mascota no lo tiene)
 * Las URLs llegan en window._urls.vademecum (patron del repo, sin rutas fijas en JS).
 */
(function () {
    "use strict";

    var DC = window.DosisCalculator;
    var ctx = window._dosisCtx || {};
    var urls = window._urls || {};

    var $ = function (id) { return document.getElementById(id); };

    var selFarmaco = $("dcFarmaco");
    var selConcent = $("dcConcentracion");
    var inpPesoManual = $("dcPesoManual");
    var boxResultado = $("dcResultado");
    var btnUsar = $("dcUsarDosis");
    var inpDosis = $("receta_Dosis");

    if (!selFarmaco || !DC) return; // la pantalla no tiene calculadora

    var listaFarmacos = [];
    var ultimoResultado = null;

    // Peso efectivo: el de la mascota, o el capturado a mano si no hay.
    function pesoEfectivo() {
        var p = (ctx.peso !== null && ctx.peso !== undefined && ctx.peso !== "")
            ? parseFloat(ctx.peso) : NaN;
        if (!isNaN(p) && p > 0) return p;
        if (inpPesoManual) {
            var pm = parseFloat(inpPesoManual.value);
            if (!isNaN(pm) && pm > 0) return pm;
        }
        return NaN;
    }

    function escapeHtml(s) {
        return String(s == null ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    }

    function dosisSeleccionada() {
        var opt = selFarmaco.options[selFarmaco.selectedIndex];
        return opt ? opt._dosis : null;
    }

    // Rellena el select de concentraciones a partir del farmaco elegido.
    function refrescarConcentraciones(dosis) {
        if (!selConcent) return;
        var concs = DC.parseCsvNumeros(dosis ? dosis.vade_ConcentracionesMgMl : null);
        selConcent.innerHTML = "";
        if (!concs.length) {
            var o = document.createElement("option");
            o.value = ""; o.textContent = "(sin concentracion)";
            selConcent.appendChild(o);
            selConcent.disabled = true;
            return;
        }
        selConcent.disabled = false;
        concs.forEach(function (c) {
            var o = document.createElement("option");
            o.value = String(c);
            o.textContent = c + " mg/mL";
            selConcent.appendChild(o);
        });
    }

    function render(html) { if (boxResultado) boxResultado.innerHTML = html; }

    function aviso(texto, tipo) {
        render('<div class="dc-aviso dc-aviso-' + (tipo || "info") + '">' + escapeHtml(texto) + '</div>');
        ultimoResultado = null;
        if (btnUsar) btnUsar.disabled = true;
    }

    function recalcular() {
        var dosis = dosisSeleccionada();
        if (!dosis) {
            render("");
            ultimoResultado = null;
            if (btnUsar) btnUsar.disabled = true;
            return;
        }

        var peso = pesoEfectivo();
        if (isNaN(peso)) {
            aviso("Registra el peso de la mascota para calcular la dosis.", "warn");
            return;
        }

        var concentracion = (selConcent && selConcent.value) ? selConcent.value : undefined;

        var res;
        try {
            res = DC.calcular({ farmacoDosis: dosis, pesoKg: peso, concentracionMgMl: concentracion });
        } catch (e) {
            aviso(e.message || "No se pudo calcular la dosis.", "error");
            return;
        }

        ultimoResultado = res;
        if (btnUsar) btnUsar.disabled = false;

        var h = "";
        h += '<div class="dc-linea dc-dosis"><span class="dc-k">Rango de dosis</span>'
           + '<span class="dc-v">' + res.rangoMgBajo + ' - ' + res.rangoMgAlto + ' mg'
           + (res.capado ? ' <span class="dc-cap">(limitado al maximo)</span>' : '') + '</span></div>';

        if (res.tabletas && res.tabletas.length) {
            var tabs = res.tabletas.map(function (t) {
                return '<li>' + escapeHtml(fraccion(t.tabletasPorDosis)) + ' tableta(s) de ' + t.fuerzaMg + ' mg por toma</li>';
            }).join("");
            h += '<div class="dc-linea"><span class="dc-k">Tabletas sugeridas</span>'
               + '<span class="dc-v"><ul class="dc-tabs">' + tabs + '</ul></span></div>';
        }

        if (res.volBajoMl !== undefined) {
            h += '<div class="dc-linea"><span class="dc-k">Volumen</span>'
               + '<span class="dc-v">' + res.volBajoMl + ' - ' + res.volAltoMl + ' mL por toma</span></div>';
        }

        if (res.via || res.frecuencia) {
            h += '<div class="dc-linea"><span class="dc-k">Via / Frecuencia</span>'
               + '<span class="dc-v">' + escapeHtml([res.via, res.frecuencia].filter(Boolean).join(" · ")) + '</span></div>';
        }

        if (res.advertencias && res.advertencias.length) {
            var adv = res.advertencias.map(function (a) { return '<li>' + escapeHtml(a) + '</li>'; }).join("");
            h += '<div class="dc-advertencias"><strong>Advertencias</strong><ul>' + adv + '</ul></div>';
        }

        h += '<div class="dc-descargo">' + escapeHtml(res.descargo) + '</div>';
        render(h);
    }

    // Muestra 0.25 -> "1/4", 0.5 -> "1/2", 1.5 -> "1 1/2", etc.
    function fraccion(n) {
        var entero = Math.floor(n);
        var resto = n - entero;
        var frac = "";
        if (Math.abs(resto - 0.25) < 0.001) frac = "1/4";
        else if (Math.abs(resto - 0.5) < 0.001) frac = "1/2";
        else if (Math.abs(resto - 0.75) < 0.001) frac = "3/4";
        if (entero && frac) return entero + " " + frac;
        if (frac) return frac;
        return String(entero);
    }

    // --- Eventos ---
    selFarmaco.addEventListener("change", function () {
        refrescarConcentraciones(dosisSeleccionada());
        recalcular();
    });
    if (selConcent) selConcent.addEventListener("change", recalcular);
    if (inpPesoManual) inpPesoManual.addEventListener("input", recalcular);

    if (btnUsar) {
        btnUsar.addEventListener("click", function () {
            if (!ultimoResultado || !inpDosis) return;
            inpDosis.value = DC.construirTextoDosis(ultimoResultado);
            inpDosis.dispatchEvent(new Event("change", { bubbles: true }));
            if (window.Swal) {
                Swal.fire({ toast: true, position: "bottom-end", icon: "success",
                    title: "Dosis copiada al campo", showConfirmButton: false, timer: 1500 });
            }
        });
    }

    // --- Carga inicial del vademecum ---
    if (urls.vademecum) {
        DC.cargarVademecum(urls.vademecum, ctx.especie)
            .then(function (lista) {
                listaFarmacos = lista;
                DC.poblarSelectFarmacos(selFarmaco, listaFarmacos);
                if (!listaFarmacos.length) {
                    aviso("No hay farmacos en el vademecum para esta especie.", "info");
                }
            })
            .catch(function () {
                aviso("No se pudo cargar el vademecum.", "error");
            });
    }
})();
