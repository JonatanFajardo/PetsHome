/*
 * http.js — capa comun de peticiones AJAX para PetsHome.
 *
 * Objetivo: que al escribir una pantalla NO tengas que acordarte del token
 * antiforgery, de los headers ni del .json(). Igual que antes con datatable.init,
 * aqui solo dices "manda estos datos a esta URL" y ya.
 *
 *   window.postJson(url, obj)  -> POST con cuerpo JSON      (actions [FromBody])
 *   window.postForm(url, obj)  -> POST form-urlencoded      (actions [FromForm])
 *   window.getJson(url)        -> GET y devuelve el JSON
 *
 * Las tres devuelven una Promesa que resuelve al JSON de la respuesta.
 * El token antiforgery se lee solo del <meta name="csrf-token"> del _Layout.
 */
(function () {
    function csrf() {
        var m = document.querySelector("meta[name='csrf-token']");
        return m ? m.getAttribute("content") : "";
    }

    function parse(r) {
        // 204/vacio -> objeto vacio; si no, intenta JSON.
        return r.text().then(function (t) {
            if (!t) return {};
            try { return JSON.parse(t); } catch (e) { return { ok: false, mensaje: t }; }
        });
    }

    window.postJson = function (url, data) {
        return fetch(url, {
            method: "POST",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest",
                "RequestVerificationToken": csrf()
            },
            body: JSON.stringify(data || {})
        }).then(parse);
    };

    window.postForm = function (url, data) {
        return fetch(url, {
            method: "POST",
            credentials: "same-origin",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
                "X-Requested-With": "XMLHttpRequest",
                "RequestVerificationToken": csrf()
            },
            body: new URLSearchParams(data || {}).toString()
        }).then(parse);
    };

    window.getJson = function (url) {
        return fetch(url, {
            credentials: "same-origin",
            headers: { "X-Requested-With": "XMLHttpRequest" }
        }).then(parse);
    };
})();
