/* =========================================================
   pos.js  —  Punto de Venta / Facturación
   Construido contra el contrato de FacturacionController.
   ========================================================= */

var POS = (function () {

    var obj = {};

    var urls = {};
    var lineas = [];                 // líneas de la factura en curso
    var tipoActual = 'Servicio';     // 'Servicio' | 'Producto'
    var catServicios = [];           // [{id, nombre, precio}]
    var catProductos = [];           // [{id, nombre, precio}]
    var historialCargado = false;

    var ISV_RATE = 0.15;

    // -------------------- Utilidades --------------------
    function money(v) {
        var n = parseFloat(v);
        if (isNaN(n)) n = 0;
        return 'L. ' + n.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function toast(msg, type) {
        if (typeof toastr !== 'undefined') {
            (toastr[type] || toastr.info)(msg);
        } else {
            alert(msg);
        }
    }

    function escapeHtml(s) {
        return String(s == null ? '' : s)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;')
            .replace(/>/g, '&gt;').replace(/"/g, '&quot;');
    }

    // -------------------- Carga de catálogos --------------------
    function cargarClientes() {
        $.getJSON(urls.clientesDropdown)
            .done(function (data) {
                var $sel = $('#pos-cliente').empty();
                (data || []).forEach(function (c) {
                    $sel.append($('<option>').val(c.id).text(c.nombre));
                });
                // Preseleccionar "Consumidor Final" si existe
                var $cf = $sel.find('option').filter(function () {
                    return /consumidor\s*final/i.test($(this).text());
                }).first();
                if ($cf.length) $sel.val($cf.val());
            })
            .fail(function () { toast('No se pudieron cargar los clientes.', 'error'); });
    }

    function cargarServicios() {
        $.getJSON(urls.serviciosDropdown)
            .done(function (data) { catServicios = data || []; if (tipoActual === 'Servicio') poblarItems(); })
            .fail(function () { toast('No se pudieron cargar los servicios.', 'error'); });
    }

    function cargarProductos() {
        $.getJSON(urls.productosDropdown)
            .done(function (data) { catProductos = data || []; if (tipoActual === 'Producto') poblarItems(); })
            .fail(function () { toast('No se pudieron cargar los productos.', 'error'); });
    }

    // -------------------- Selector de ítems --------------------
    function catalogoActual() {
        return tipoActual === 'Servicio' ? catServicios : catProductos;
    }

    function poblarItems() {
        var $sel = $('#pos-item').empty();
        $sel.append($('<option>').val('').text('— Seleccione —'));
        catalogoActual().forEach(function (it) {
            $sel.append($('<option>').val(it.id).text(it.nombre)
                .attr('data-precio', it.precio));
        });
        // limpiar campos dependientes
        $('#pos-desc').val('');
        $('#pos-precio').val('0.00');
    }

    function onItemSeleccionado() {
        var id = $('#pos-item').val();
        if (!id) { $('#pos-desc').val(''); $('#pos-precio').val('0.00'); $('#pos-exento').prop('checked', false); return; }
        var item = catalogoActual().find(function (x) { return String(x.id) === String(id); });
        if (item) {
            $('#pos-desc').val(item.nombre);
            $('#pos-precio').val(parseFloat(item.precio || 0).toFixed(2));
            // El exento lo decide el catalogo; el usuario aun puede ajustarlo.
            $('#pos-exento').prop('checked', !!item.exento);
        }
    }

    function setTipo(tipo) {
        tipoActual = tipo;
        $('#pos-type-toggle button').each(function () {
            $(this).toggleClass('active', $(this).data('tipo') === tipo);
        });
        poblarItems();
    }

    // -------------------- Líneas --------------------
    function agregarLinea() {
        var id = $('#pos-item').val();
        var desc = ($('#pos-desc').val() || '').trim();
        var cant = parseFloat($('#pos-cant').val());
        var precio = parseFloat($('#pos-precio').val());
        var descuento = parseFloat($('#pos-desc-monto').val());
        var exento = $('#pos-exento').is(':checked');

        if (!id) { toast('Seleccione un ' + tipoActual.toLowerCase() + '.', 'warning'); return; }
        if (!desc) { toast('Ingrese una descripción.', 'warning'); return; }
        if (isNaN(cant) || cant <= 0) { toast('La cantidad debe ser mayor a cero.', 'warning'); return; }
        if (isNaN(precio) || precio < 0) { toast('El precio no es válido.', 'warning'); return; }
        if (isNaN(descuento) || descuento < 0) descuento = 0;
        var bruto = cant * precio;
        if (descuento > bruto) { toast('El descuento no puede ser mayor al importe de la línea.', 'warning'); return; }

        lineas.push({
            tipo: tipoActual,
            servId: tipoActual === 'Servicio' ? parseInt(id, 10) : null,
            itmId: tipoActual === 'Producto' ? parseInt(id, 10) : null,
            descripcion: desc,
            cantidad: cant,
            precioUnit: precio,
            descuento: descuento,
            exento: exento
        });

        renderLineas();
        recalcular();

        // reset del formulario de línea (mantiene el tipo elegido)
        $('#pos-item').val('');
        $('#pos-desc').val('');
        $('#pos-cant').val('1');
        $('#pos-precio').val('0.00');
        $('#pos-desc-monto').val('0.00');
        $('#pos-exento').prop('checked', false);
        $('#pos-item').focus();
    }

    function quitarLinea(idx) {
        lineas.splice(idx, 1);
        renderLineas();
        recalcular();
    }

    function renderLineas() {
        var $body = $('#pos-lines-body').empty();

        if (!lineas.length) {
            $body.append(
                '<tr class="pos-lines-empty"><td colspan="7">' +
                '<i class="fas fa-inbox mr-2"></i>Aún no hay ítems en la factura.</td></tr>'
            );
            return;
        }

        lineas.forEach(function (l, i) {
            var descuento = parseFloat(l.descuento) || 0;
            var subtotal = (l.cantidad * l.precioUnit) - descuento;
            if (subtotal < 0) subtotal = 0;
            var badgeClass = l.tipo === 'Servicio' ? 'pos-tipo-servicio' : 'pos-tipo-producto';
            var exentoTag = l.exento ? ' <span class="pos-exento-tag" title="No causa ISV">Exento</span>' : '';
            var $tr = $(
                '<tr>' +
                    '<td><span class="pos-tipo-badge ' + badgeClass + '">' + l.tipo + '</span></td>' +
                    '<td>' + escapeHtml(l.descripcion) + exentoTag + '</td>' +
                    '<td class="text-center">' + l.cantidad + '</td>' +
                    '<td class="text-right pos-dinero">' + money(l.precioUnit) + '</td>' +
                    '<td class="text-right pos-dinero">' + (descuento > 0 ? '-' + money(descuento) : '—') + '</td>' +
                    '<td class="text-right pos-dinero">' + money(subtotal) + '</td>' +
                    '<td class="text-center">' +
                        '<button type="button" class="pos-remove-btn" data-idx="' + i + '" title="Quitar">' +
                        '<i class="fas fa-trash"></i></button>' +
                    '</td>' +
                '</tr>'
            );
            $body.append($tr);
        });
    }

    // Subtotal de una linea ya con su descuento aplicado (nunca negativo).
    function subtotalLinea(l) {
        var s = (l.cantidad * l.precioUnit) - (parseFloat(l.descuento) || 0);
        return s < 0 ? 0 : s;
    }

    function calcularSubtotal() {
        return lineas.reduce(function (acc, l) { return acc + subtotalLinea(l); }, 0);
    }

    function recalcular() {
        var subtotal = calcularSubtotal();
        // Base gravable = subtotales de las lineas NO exentas.
        var base = lineas.reduce(function (acc, l) { return acc + (l.exento ? 0 : subtotalLinea(l)); }, 0);
        var isv = Math.round(base * ISV_RATE * 100) / 100;

        // Descuento a nivel factura, acotado a [0, subtotal].
        var factDesc = parseFloat($('#pos-fact-descuento').val());
        if (isNaN(factDesc) || factDesc < 0) factDesc = 0;
        if (factDesc > subtotal) factDesc = subtotal;

        var total = subtotal - factDesc + isv;
        if (total < 0) total = 0;

        $('#pos-subtotal').text(money(subtotal));
        $('#pos-base').text(money(base));
        $('#pos-isv').text(money(isv));
        $('#pos-total').text(money(total));

        $('#pos-facturar-btn').prop('disabled', lineas.length === 0);
    }

    // -------------------- Facturar --------------------
    function facturar() {
        if (!lineas.length) { toast('Agregue al menos un ítem.', 'warning'); return; }

        var cliVal = $('#pos-cliente').val();
        var mascVal = $('#pos-mascota').val();

        var factDesc = parseFloat($('#pos-fact-descuento').val());
        if (isNaN(factDesc) || factDesc < 0) factDesc = 0;

        var payload = {
            cliId: cliVal ? parseInt(cliVal, 10) : null,
            mascId: mascVal ? parseInt(mascVal, 10) : null,
            descuento: factDesc,
            lineas: lineas.map(function (l) {
                return {
                    tipo: l.tipo,
                    servId: l.servId,
                    itmId: l.itmId,
                    descripcion: l.descripcion,
                    cantidad: l.cantidad,
                    precioUnit: l.precioUnit,
                    descuento: parseFloat(l.descuento) || 0,
                    exento: !!l.exento
                };
            })
        };

        var totalActual = $('#pos-total').text();
        var $btn = $('#pos-facturar-btn').prop('disabled', true)
            .html('<i class="fas fa-spinner fa-spin"></i>Procesando...');

        $.ajax({
            url: urls.crear,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            dataType: 'json'
        })
        .done(function (resp) {
            if (resp && resp.success) {
                toast((resp.message || 'Factura emitida') + ' — Factura #' + resp.factId +
                      ' · Total ' + totalActual, 'success');
                limpiar();
                historialCargado = false; // forzar recarga del historial la próxima vez
                // Aviso opcional: la pantalla de facturación vuelve a la lista y refresca.
                if (typeof window.onFacturaCreada === 'function') window.onFacturaCreada(resp);
            } else {
                toast((resp && resp.message) ? resp.message : 'No se pudo emitir la factura.', 'error');
            }
        })
        .fail(function () {
            toast('Error de comunicación al emitir la factura.', 'error');
        })
        .always(function () {
            $btn.html('<i class="fas fa-check-circle"></i>Facturar');
            recalcular();
        });
    }

    function limpiar() {
        lineas = [];
        $('#pos-mascota').val('');
        $('#pos-item').val('');
        $('#pos-desc').val('');
        $('#pos-cant').val('1');
        $('#pos-precio').val('0.00');
        $('#pos-desc-monto').val('0.00');
        $('#pos-exento').prop('checked', false);
        $('#pos-fact-descuento').val('0.00');
        // Reset cliente a Consumidor Final
        var $cf = $('#pos-cliente option').filter(function () {
            return /consumidor\s*final/i.test($(this).text());
        }).first();
        if ($cf.length) $('#pos-cliente').val($cf.val());
        setTipo('Servicio');
        renderLineas();
        recalcular();
    }

    // -------------------- Historial --------------------
    function cargarHistorial() {
        if (historialCargado) return;

        if ($.fn.DataTable.isDataTable('#pos-historial')) {
            $('#pos-historial').DataTable().ajax.reload();
            historialCargado = true;
            return;
        }

        $('#pos-historial').DataTable({
            ajax: { url: urls.list, dataSrc: 'data' },
            columns: [
                { data: 'fact_Id', width: '70px' },
                {
                    data: 'fact_Numero',
                    render: function (d) {
                        return '<span style="font-family:monospace;font-weight:600;">' +
                               escapeHtml(d || '—') + '</span>';
                    }
                },
                { data: 'cliente', render: function (d) { return escapeHtml(d || 'Consumidor Final'); } },
                {
                    data: 'fact_Fecha',
                    render: function (d, t) {
                        if (t !== 'display') return d;
                        return d ? new Date(d).toLocaleDateString('es-HN') : '—';
                    }
                },
                {
                    data: 'fact_Total', className: 'text-right',
                    render: function (d) { return '<span class="pos-dinero">' + money(d) + '</span>'; }
                },
                {
                    data: 'saldo', className: 'text-right',
                    render: function (d) {
                        var v = parseFloat(d) || 0;
                        if (v <= 0) return '<span style="color:#15803d;">' + money(0) + '</span>';
                        return '<span style="color:#b45309;font-weight:600;">' + money(v) + '</span>';
                    }
                },
                {
                    data: 'fact_Vencimiento', className: 'text-center',
                    render: function (d, t, row) {
                        if (!d) return '—';
                        var f = new Date(d);
                        if (t !== 'display') return d;
                        var txt = f.toLocaleDateString('es-HN');
                        var saldo = parseFloat(row.saldo) || 0;
                        var hoy = new Date(new Date().toDateString());
                        if (saldo > 0 && f < hoy)
                            return '<span style="color:#b91c1c;font-weight:600;" title="Vencida">' + txt + ' ⚠</span>';
                        return txt;
                    }
                },
                {
                    data: 'fact_Estado', className: 'text-center',
                    render: function (d) {
                        var e = (d || '').toLowerCase();
                        var bg = '#dcfce7', co = '#15803d';
                        if (e.indexOf('anul') > -1 || e.indexOf('cancel') > -1) { bg = '#fee2e2'; co = '#b91c1c'; }
                        else if (e.indexOf('cobrar') > -1 || e.indexOf('pend') > -1 || e.indexOf('emit') > -1) { bg = '#fef3c7'; co = '#b45309'; }
                        return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;' +
                               'font-size:12px;font-weight:600;background:' + bg + ';color:' + co + ';">' +
                               escapeHtml(d || '—') + '</span>';
                    }
                },
                {
                    data: 'fact_Id', className: 'text-center', orderable: false,
                    render: function (d, t, row) {
                        var saldo = parseFloat(row.saldo) || 0;
                        var txt = saldo > 0 ? 'Cobrar' : 'Ver';
                        return '<a href="' + urls.comprobante + '/' + d + '" ' +
                               'style="display:inline-block;padding:4px 12px;border-radius:6px;font-size:12px;' +
                               'font-weight:600;text-decoration:none;background:#6d28d9;color:#fff;">' + txt + '</a>';
                    }
                }
            ],
            order: [[0, 'desc']],
            responsive: true,
            language: {
                processing: 'Procesando...',
                zeroRecords: 'No hay facturas registradas',
                emptyTable: 'No hay facturas registradas',
                info: 'Mostrando _START_ de _TOTAL_ registros',
                infoEmpty: 'Mostrando 0 de 0 registros',
                lengthMenu: 'Mostrar _MENU_',
                search: 'Buscar:',
                paginate: { next: 'Siguiente', previous: 'Anterior', first: 'Primero', last: 'Último' }
            }
        });
        historialCargado = true;
    }

    // -------------------- Tabs --------------------
    obj.switchTab = function (tab) {
        var isPos = tab === 'pos';
        $('#view-pos').toggle(isPos);
        $('#view-historial').toggle(!isPos);
        $('#tab-pos').toggleClass('active', isPos);
        $('#tab-historial').toggleClass('active', !isPos);
        if (!isPos) cargarHistorial();
    };

    // -------------------- Init --------------------
    obj.init = function (urlsCfg) {
        urls = urlsCfg || {};

        cargarClientes();
        cargarServicios();
        cargarProductos();

        setTipo('Servicio');
        renderLineas();
        recalcular();

        // eventos
        $('#pos-type-toggle').on('click', 'button', function () {
            setTipo($(this).data('tipo'));
        });
        $('#pos-item').on('change', onItemSeleccionado);
        $('#pos-add-btn').on('click', agregarLinea);
        $('#pos-lines-body').on('click', '.pos-remove-btn', function () {
            quitarLinea(parseInt($(this).data('idx'), 10));
        });
        $('#pos-facturar-btn').on('click', facturar);
        $('#pos-limpiar-btn').on('click', limpiar);

        // Recalcular totales cuando cambia el descuento de factura
        $('#pos-fact-descuento').on('input', recalcular);

        // Enter en cantidad/precio/descuento agrega la línea
        $('#pos-cant, #pos-precio, #pos-desc-monto').on('keydown', function (e) {
            if (e.key === 'Enter') { e.preventDefault(); agregarLinea(); }
        });
    };

    return obj;

}());
