/*
 * consulta-dosis.js — Pega la Calculadora de dosis por peso a la pantalla de
 * Consulta (cabina del veterinario). Reusa el modulo autocontenido
 * window.DosisCalculator, igual que la pantalla de Recetas.
 *
 * NO reescribe en-consulta.js: solo LEE el peso en vivo (#ecPeso) y la especie
 * (window._dosisCtx.especie), y al "Usar esta dosis" rellena el campo "dosis" de
 * la LINEA de medicamento objetivo disparando su evento 'input' para que
 * en-consulta.js persista el cambio en su estado interno.
 *
 * Objetivo: cada linea de medicamento tiene un boton de calculadora que abre
 * este bloque apuntando a ESA linea. Si se abre sin elegir, aplica a la ultima.
 */
(function () {
    "use strict";

    var DC = window.DosisCalculator;
    var ctx = window._dosisCtx || {};
    var urls = window._urls || {};

    var $ = function (id) { return document.getElementById(id); };

    var selFarmaco = $("dcFarmaco");
    var selConcent = $("dcConcentracion");
    var boxResultado = $("dcResultado");
    var btnUsar = $("dcUsarDosis");
    var detalle = $("ecDosisCalc");
    var inpPeso = $("ecPeso");
    var pesoVal = $("dcPesoVal");
    var targetLbl = $("dcTarget");

    if (!selFarmaco || !DC) return; // la pantalla no tiene calculadora

    var listaFarmacos = [];
    var ultimoResultado = null;
    var targetIndex = -1; // indice (data-i) de la linea de medicamento objetivo

    function escapeHtml(s) {
        return String(s == null ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    }

    // Peso en vivo del formulario: el vet puede actualizarlo durante la visita.
    function pesoEfectivo() {
        var p = inpPeso ? parseFloat(inpPeso.value) : NaN;
        return (!isNaN(p) && p > 0) ? p : NaN;
    }

    function refrescarPesoChip() {
        if (!pesoVal) return;
        var p = pesoEfectivo();
        pesoVal.textContent = isNaN(p) ? "—" : p;
    }

    // Input de dosis de la linea objetivo (o la ultima linea si no hay elegida).
    function inputDosisObjetivo() {
        var box = $("ecMedLines");
        if (!box) return null;
        var sel = box.querySelector('[data-field="dosis"][data-i="' + targetIndex + '"]');
        if (sel) return sel;
        var inputs = box.querySelectorAll('[data-field="dosis"]');
        return inputs.length ? inputs[inputs.length - 1] : null;
    }

    function refrescarTargetLabel() {
        if (!targetLbl) return;
        var inp = inputDosisObjetivo();
        if (!inp) { targetLbl.style.display = "none"; return; }
        var line = inp.closest(".ec-line-med");
        var nm = line ? line.querySelector(".ec-line-name") : null;
        var nombre = nm ? nm.textContent.trim() : "medicamento";
        targetLbl.style.display = "";
        targetLbl.innerHTML = 'Aplicar a: <strong>' + escapeHtml(nombre) + '</strong>';
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

    function recalcular() {
        refrescarPesoChip();
        var dosis = dosisSeleccionada();
        if (!dosis) {
            render("");
            ultimoResultado = null;
            if (btnUsar) btnUsar.disabled = true;
            return;
        }

        var peso = pesoEfectivo();
        if (isNaN(peso)) {
            aviso("Captura el peso para calcular.", "warn");
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
        if (btnUsar) btnUsar.disabled = !inputDosisObjetivo();

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

    // --- Eventos de la calculadora ---
    selFarmaco.addEventListener("change", function () {
        refrescarConcentraciones(dosisSeleccionada());
        recalcular();
    });
    if (selConcent) selConcent.addEventListener("change", recalcular);
    if (inpPeso) inpPeso.addEventListener("input", recalcular);

    if (btnUsar) {
        btnUsar.addEventListener("click", function () {
            if (!ultimoResultado) return;
            var inp = inputDosisObjetivo();
            if (!inp) {
                aviso("Agrega un medicamento para aplicar la dosis.", "warn");
                return;
            }
            inp.value = DC.construirTextoDosis(ultimoResultado);
            // Dispara 'input' para que en-consulta.js persista el valor en su estado.
            inp.dispatchEvent(new Event("input", { bubbles: true }));
            if (window.Swal) {
                Swal.fire({
                    toast: true, position: "bottom-end", icon: "success",
                    title: "Dosis aplicada al medicamento", showConfirmButton: false, timer: 1500
                });
            }
        });
    }

    // Boton de calculadora por linea (delegado; las lineas se re-renderizan).
    var medBox = $("ecMedLines");
    if (medBox) {
        medBox.addEventListener("click", function (e) {
            var btn = e.target.closest(".ec-med-calc");
            if (!btn) return;
            targetIndex = parseInt(btn.dataset.i, 10);
            if (detalle) detalle.open = true;
            refrescarTargetLabel();
            recalcular();
            if (detalle && detalle.scrollIntoView) {
                detalle.scrollIntoView({ behavior: "smooth", block: "nearest" });
            }
        });
    }

    // Al abrir la calculadora a mano, apuntar a una linea valida.
    if (detalle) {
        detalle.addEventListener("toggle", function () {
            if (detalle.open) { refrescarTargetLabel(); refrescarPesoChip(); }
        });
    }

    refrescarPesoChip();

    // --- Carga inicial del vademecum (por especie del paciente) ---
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
