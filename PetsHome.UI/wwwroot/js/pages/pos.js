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
        if (!id) { $('#pos-desc').val(''); $('#pos-precio').val('0.00'); return; }
        var item = catalogoActual().find(function (x) { return String(x.id) === String(id); });
        if (item) {
            $('#pos-desc').val(item.nombre);
            $('#pos-precio').val(parseFloat(item.precio || 0).toFixed(2));
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

        if (!id) { toast('Seleccione un ' + tipoActual.toLowerCase() + '.', 'warning'); return; }
        if (!desc) { toast('Ingrese una descripción.', 'warning'); return; }
        if (isNaN(cant) || cant <= 0) { toast('La cantidad debe ser mayor a cero.', 'warning'); return; }
        if (isNaN(precio) || precio < 0) { toast('El precio no es válido.', 'warning'); return; }

        lineas.push({
            tipo: tipoActual,
            servId: tipoActual === 'Servicio' ? parseInt(id, 10) : null,
            itmId: tipoActual === 'Producto' ? parseInt(id, 10) : null,
            descripcion: desc,
            cantidad: cant,
            precioUnit: precio
        });

        renderLineas();
        recalcular();

        // reset del formulario de línea (mantiene el tipo elegido)
        $('#pos-item').val('');
        $('#pos-desc').val('');
        $('#pos-cant').val('1');
        $('#pos-precio').val('0.00');
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
                '<tr class="pos-lines-empty"><td colspan="6">' +
                '<i class="fas fa-inbox mr-2"></i>Aún no hay ítems en la factura.</td></tr>'
            );
            return;
        }

        lineas.forEach(function (l, i) {
            var subtotal = l.cantidad * l.precioUnit;
            var badgeClass = l.tipo === 'Servicio' ? 'pos-tipo-servicio' : 'pos-tipo-producto';
            var $tr = $(
                '<tr>' +
                    '<td><span class="pos-tipo-badge ' + badgeClass + '">' + l.tipo + '</span></td>' +
                    '<td>' + escapeHtml(l.descripcion) + '</td>' +
                    '<td class="text-center">' + l.cantidad + '</td>' +
                    '<td class="text-right pos-dinero">' + money(l.precioUnit) + '</td>' +
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

    function calcularSubtotal() {
        return lineas.reduce(function (acc, l) { return acc + (l.cantidad * l.precioUnit); }, 0);
    }

    function recalcular() {
        var subtotal = calcularSubtotal();
        var isv = subtotal * ISV_RATE;
        var total = subtotal + isv;

        $('#pos-subtotal').text(money(subtotal));
        $('#pos-isv').text(money(isv));
        $('#pos-total').text(money(total));

        $('#pos-facturar-btn').prop('disabled', lineas.length === 0);
    }

    // -------------------- Facturar --------------------
    function facturar() {
        if (!lineas.length) { toast('Agregue al menos un ítem.', 'warning'); return; }

        var cliVal = $('#pos-cliente').val();
        var mascVal = $('#pos-mascota').val();

        var payload = {
            cliId: cliVal ? parseInt(cliVal, 10) : null,
            mascId: mascVal ? parseInt(mascVal, 10) : null,
            lineas: lineas.map(function (l) {
                return {
                    tipo: l.tipo,
                    servId: l.servId,
                    itmId: l.itmId,
                    descripcion: l.descripcion,
                    cantidad: l.cantidad,
                    precioUnit: l.precioUnit
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
                    data: 'fact_Estado', className: 'text-center',
                    render: function (d) {
                        var e = (d || '').toLowerCase();
                        var bg = '#dcfce7', co = '#15803d';
                        if (e.indexOf('anul') > -1 || e.indexOf('cancel') > -1) { bg = '#fee2e2'; co = '#b91c1c'; }
                        else if (e.indexOf('pend') > -1) { bg = '#fef3c7'; co = '#b45309'; }
                        return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;' +
                               'font-size:12px;font-weight:600;background:' + bg + ';color:' + co + ';">' +
                               escapeHtml(d || '—') + '</span>';
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

        // Enter en cantidad agrega la línea
        $('#pos-cant, #pos-precio').on('keydown', function (e) {
            if (e.key === 'Enter') { e.preventDefault(); agregarLinea(); }
        });
    };

    return obj;

}());
