/*
 * select-enhance.js — Mejora GLOBAL y masiva de dropdowns.
 *
 * Convierte automáticamente todos los <select> de cualquier pantalla en
 * buscadores (select2) con búsqueda que ignora acentos y mayúsculas.
 * Si el campo pertenece a un catálogo con alta rápida, al no haber
 * coincidencia muestra "Agregar" que abre el modal "Nuevo" de la pantalla
 * de ese catálogo (vía ?openCreate=1), en otra pestaña.
 *
 * Se auto-ejecuta en cada página. No hay que tocar las vistas.
 *
 * Excluir un <select> puntual:  clase "no-select2"  o  atributo data-no-select2.
 *
 * Agregar "Agregar -> modal" a un catálogo nuevo:  una línea en CATALOG_SCREENS
 * { prefijo_del_campo : "/Controller" }.  La pantalla destino debe soportar
 * ?openCreate=1 (abrir su modal de "Nuevo").
 */
(function ($) {
    'use strict';
    if (!$ || !$.fn || !$.fn.select2) return;

    // Prefijo del id/name del <select>  ->  controlador de su pantalla de catálogo.
    // Solo estos muestran el enlace "Agregar". La pantalla debe soportar ?openCreate=1.
    var CATALOG_SCREENS = {
        'raza_': '/Raza',
        'proc_': '/Procedencia'
    };

    function sinAcentos(s) {
        return (s || '').toString().normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase();
    }

    // Búsqueda sin acentos ni mayúsculas ("jose" encuentra "José").
    function matcherSinAcentos(params, data) {
        var term = (params.term || '').trim();
        if (term === '') return data;
        if (typeof data.text === 'undefined') return null;
        return sinAcentos(data.text).indexOf(sinAcentos(term)) > -1 ? data : null;
    }

    // URL del modal "Nuevo" para un <select>, o null si el catálogo no aplica.
    function addUrlFor(el) {
        var key = el.id || el.name || '';
        for (var prefix in CATALOG_SCREENS) {
            if (key.indexOf(prefix) === 0) {
                return CATALOG_SCREENS[prefix] + '/Index?openCreate=1';
            }
        }
        return null;
    }

    function enhance(el) {
        var $el = $(el);

        // Guardas: no re-inicializar ni pisar otros plugins / casos especiales.
        if ($el.data('select2-done')) return;
        if ($el.is('[multiple]')) return;                      // multiselect: otro plugin
        if ($el.hasClass('no-select2') || $el.attr('data-no-select2') != null) return;
        if ($el.hasClass('selectpicker')) return;              // bootstrap-select
        if ($el.closest('.dataTables_length').length) return;  // paginador de datatables
        if ($el.closest('.modal').length) return;              // selects dentro de modales
        if ($el.closest('.select2-container').length) return;

        var url = addUrlFor(el);
        var opts = {
            width: '100%',
            placeholder: $el.find('option[value=""]').first().text() || '-- Seleccionar --',
            matcher: matcherSinAcentos
        };
        if (url) {
            opts.language = {
                noResults: function () {
                    return '<a href="#" class="s2-add-link" data-url="' + url + '">'
                         + '<i class="fas fa-plus mr-1"></i>Agregar nuevo</a>';
                }
            };
            opts.escapeMarkup = function (m) { return m; }; // permite el HTML del enlace
        }

        $el.select2(opts);
        $el.data('select2-done', true);
    }

    // Aplica a todos los <select> del contexto (documento por defecto).
    function enhanceAll(ctx) {
        $(ctx || document).find('select').each(function () { enhance(this); });
    }

    $(function () {
        enhanceAll(document);

        // Clic en "Agregar" -> abre el modal de la pantalla del catálogo (otra pestaña).
        $(document).on('click', '.s2-add-link', function (e) {
            e.preventDefault();
            window.open($(this).data('url'), '_blank');
        });
    });

    // Para pantallas que cargan <select> por AJAX y quieran re-aplicar:  enhanceDropdowns(contenedor)
    window.enhanceDropdowns = enhanceAll;
})(window.jQuery);
