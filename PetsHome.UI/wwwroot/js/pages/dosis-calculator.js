/*
 * dosis-calculator.js — Modulo reutilizable de calculo de dosis por peso.
 *
 * Portado de la logica de dosing de OpenVPM (lib/dosing/calculator.ts).
 * Es AUTOCONTENIDO: no depende de ninguna pantalla concreta, para poder
 * reusarlo tanto en Recetas como en la Consulta.
 *
 *   window.DosisCalculator.calcular({ farmacoDosis, pesoKg, concentracionMgMl })
 *       -> funcion PURA: recibe un objeto de dosis del vademecum y el peso,
 *          devuelve el rango de dosis, tabletas sugeridas, volumen, etc.
 *
 *   window.DosisCalculator.cargarVademecum(url, especie, categoria)
 *       -> fetch del vademecum, devuelve una Promesa con el arreglo de farmacos.
 *
 *   window.DosisCalculator.poblarSelectFarmacos(selectEl, lista)
 *       -> rellena un <select> con los farmacos (guarda el objeto en cada option).
 *
 *   window.DosisCalculator.construirTextoDosis(resultado)
 *       -> arma un texto claro para el campo Dosis (sin acentos).
 */
(function () {
    "use strict";

    var PESO_MAX_KG = 200;

    // Descargo que SIEMPRE se muestra junto al calculo.
    var DESCARGO =
        "Solo rangos de referencia. Verifique cada dosis contra una referencia " +
        "veterinaria vigente y el cuadro clinico del paciente antes de recetar.";

    // Redondeo a 'dp' decimales (como el round() de calculator.ts).
    function round(valor, dp) {
        var f = Math.pow(10, dp);
        return Math.round(valor * f) / f;
    }

    // Al cuarto mas cercano, para que partir la tableta sea practico (1/4, 1/2...).
    function roundToQuarter(valor) {
        return Math.round(valor * 4) / 4;
    }

    function esNumeroFinito(n) {
        return typeof n === "number" && isFinite(n);
    }

    // Convierte un texto CSV ("25,75,100") a un arreglo de numeros [25,75,100].
    function parseCsvNumeros(csv) {
        if (!csv) return [];
        return String(csv)
            .split(",")
            .map(function (x) { return parseFloat(String(x).trim()); })
            .filter(function (x) { return esNumeroFinito(x) && x > 0; });
    }

    /**
     * Calculo PURO de la dosis por peso.
     * @param {Object} opts
     * @param {Object} opts.farmacoDosis  objeto de dosis del vademecum
     * @param {number} opts.pesoKg         peso del paciente en kg
     * @param {number} [opts.concentracionMgMl] concentracion opcional (mg/mL)
     * @returns {Object} resultado del calculo
     */
    function calcular(opts) {
        opts = opts || {};
        var d = opts.farmacoDosis || {};
        var pesoKg = parseFloat(opts.pesoKg);

        if (!esNumeroFinito(pesoKg) || pesoKg <= 0) {
            throw new Error("El peso debe ser un numero positivo en kilogramos.");
        }
        if (pesoKg > PESO_MAX_KG) {
            throw new Error("El peso supera un rango razonable (mas de 200 kg). Revise las unidades.");
        }

        var mgKgMin = parseFloat(d.vdos_MgKgMin);
        var mgKgMax = parseFloat(d.vdos_MgKgMax);
        if (!esNumeroFinito(mgKgMin) || !esNumeroFinito(mgKgMax)) {
            throw new Error("Este farmaco no tiene una dosis de referencia (mg/kg) para esta especie.");
        }

        var rangoMgBajo = pesoKg * mgKgMin;
        var rangoMgAlto = pesoKg * mgKgMax;

        // Tope por dosis maxima unica, si el vademecum lo define.
        var capado = false;
        var maxUnica = (d.vdos_MaxDosisUnicaMg === null || d.vdos_MaxDosisUnicaMg === undefined || d.vdos_MaxDosisUnicaMg === "")
            ? undefined
            : parseFloat(d.vdos_MaxDosisUnicaMg);
        if (maxUnica !== undefined && esNumeroFinito(maxUnica)) {
            if (rangoMgAlto > maxUnica) { rangoMgAlto = maxUnica; capado = true; }
            if (rangoMgBajo > maxUnica) { rangoMgBajo = maxUnica; capado = true; }
        }

        rangoMgBajo = round(rangoMgBajo, 2);
        rangoMgAlto = round(rangoMgAlto, 2);

        // Advertencias: las del farmaco + notas de la dosis + aviso de tope.
        var advertencias = [];
        if (d.vade_Advertencias) advertencias.push(String(d.vade_Advertencias));
        if (d.vdos_Notas) advertencias.push(String(d.vdos_Notas));
        if (capado) {
            advertencias.push("Dosis limitada al maximo por toma de " + maxUnica + " mg.");
        }

        // Volumen (mL): la concentracion del parametro manda; si no, la del vademecum.
        var concentracion = opts.concentracionMgMl;
        if (concentracion === undefined || concentracion === null || concentracion === "") {
            var csvConc = parseCsvNumeros(d.vade_ConcentracionesMgMl);
            concentracion = csvConc.length ? csvConc[0] : undefined;
        } else {
            concentracion = parseFloat(concentracion);
        }

        var volBajoMl, volAltoMl;
        if (concentracion !== undefined) {
            if (!esNumeroFinito(concentracion) || concentracion <= 0) {
                throw new Error("La concentracion debe ser un numero positivo (mg/mL).");
            }
            volBajoMl = round(rangoMgBajo / concentracion, 2);
            volAltoMl = round(rangoMgAlto / concentracion, 2);
        }

        // Tabletas sugeridas: punto medio del rango / fuerza, al 1/4 mas cercano,
        // mostrando solo fracciones practicas (entre 1/4 y 4 tabletas).
        var tabletas = [];
        var fuerzas = parseCsvNumeros(d.vade_PresentacionesTabletaMg);
        if (fuerzas.length) {
            var medio = (rangoMgBajo + rangoMgAlto) / 2;
            tabletas = fuerzas
                .map(function (fuerzaMg) {
                    return { fuerzaMg: fuerzaMg, tabletasPorDosis: roundToQuarter(medio / fuerzaMg) };
                })
                .filter(function (t) {
                    return t.tabletasPorDosis >= 0.25 && t.tabletasPorDosis <= 4;
                });
        }

        return {
            farmaco: d.vade_Farmaco || "",
            rangoMgBajo: rangoMgBajo,
            rangoMgAlto: rangoMgAlto,
            capado: capado,
            volBajoMl: volBajoMl,
            volAltoMl: volAltoMl,
            tabletas: tabletas,
            via: d.vdos_Via || "",
            frecuencia: d.vdos_Frecuencia || "",
            advertencias: advertencias,
            descargo: DESCARGO
        };
    }

    // Carga el vademecum desde la URL del contrato. Devuelve una Promesa con el arreglo.
    function cargarVademecum(url, especie, categoria) {
        var qs = [];
        if (especie) qs.push("especie=" + encodeURIComponent(especie));
        if (categoria) qs.push("categoria=" + encodeURIComponent(categoria));
        var full = url + (qs.length ? ("?" + qs.join("&")) : "");
        return fetch(full, {
            credentials: "same-origin",
            headers: { "X-Requested-With": "XMLHttpRequest" }
        }).then(function (r) {
            return r.json();
        }).then(function (data) {
            return Array.isArray(data) ? data : [];
        });
    }

    // Rellena un <select> de farmacos. Guarda el objeto completo en cada <option>.
    function poblarSelectFarmacos(selectEl, lista) {
        if (!selectEl) return;
        selectEl.innerHTML = "";
        var opt0 = document.createElement("option");
        opt0.value = "";
        opt0.textContent = "-- Seleccionar farmaco --";
        selectEl.appendChild(opt0);

        (lista || []).forEach(function (f, i) {
            var opt = document.createElement("option");
            opt.value = String(i);
            var etiqueta = f.vade_Farmaco || "Farmaco";
            if (f.vade_Categoria) etiqueta += " (" + f.vade_Categoria + ")";
            opt.textContent = etiqueta;
            opt._dosis = f; // guardamos el objeto para el calculo
            selectEl.appendChild(opt);
        });
    }

    // Texto claro y sin acentos para el campo Dosis. Ej: "22-44 mg PO c/12-24h".
    function construirTextoDosis(res) {
        if (!res) return "";
        var partes = [];
        if (res.rangoMgBajo === res.rangoMgAlto) {
            partes.push(res.rangoMgBajo + " mg");
        } else {
            partes.push(res.rangoMgBajo + "-" + res.rangoMgAlto + " mg");
        }
        if (res.via) partes.push(res.via);
        var texto = partes.join(" ");
        if (res.frecuencia) texto += " " + res.frecuencia;
        return texto.trim();
    }

    window.DosisCalculator = {
        PESO_MAX_KG: PESO_MAX_KG,
        DESCARGO: DESCARGO,
        calcular: calcular,
        cargarVademecum: cargarVademecum,
        poblarSelectFarmacos: poblarSelectFarmacos,
        construirTextoDosis: construirTextoDosis,
        parseCsvNumeros: parseCsvNumeros
    };
})();
