/* Presupuestos / Estimados — listado, creación y detalle. */
var Estimados = (function () {
    'use strict';

    function money(n) {
        n = Number(n) || 0;
        return 'L. ' + n.toLocaleString('es-HN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }
    function fecha(s) {
        if (!s) return '—';
        var d = new Date(s);
        if (isNaN(d)) return '—';
        return d.toLocaleDateString('es-HN');
    }
    function badge(estado) {
        var cls = 'est-badge est-' + (estado || '').toLowerCase();
        return '<span class="' + cls + '">' + (estado || '') + '</span>';
    }

    // ───────────────────────── INDEX ─────────────────────────
    function initIndex(urls) {
        var estado = '';

        function cargar() {
            $.getJSON(urls.list, { estado: estado }, function (resp) {
                var rows = (resp && resp.data) || [];
                var tb = $('#est-tbody').empty();
                if (!rows.length) {
                    tb.append('<tr><td colspan="9" class="text-center" style="padding:28px;color:#888;">Sin presupuestos.</td></tr>');
                    return;
                }
                rows.forEach(function (r) {
                    var detUrl = urls.detail + '/' + r.est_Id;
                    tb.append(
                        '<tr>' +
                        '<td>' + r.est_Id + '</td>' +
                        '<td>' + (r.cliente || '<span style="color:#999;">Consumidor final</span>') + '</td>' +
                        '<td>' + (r.mascota || '—') + '</td>' +
                        '<td>' + fecha(r.est_Fecha) + '</td>' +
                        '<td>' + fecha(r.est_Vigencia) + '</td>' +
                        '<td class="text-center">' + r.lineas + '</td>' +
                        '<td class="text-right">' + money(r.est_Total) + '</td>' +
                        '<td class="text-center">' + badge(r.est_Estado) + '</td>' +
                        '<td class="text-center"><a class="est-link-btn" href="' + detUrl + '"><i class="fas fa-eye"></i> Ver</a></td>' +
                        '</tr>');
                });
            });
        }

        $('#est-tabs').on('click', '.est-tab', function () {
            $('#est-tabs .est-tab').removeClass('active');
            $(this).addClass('active');
            estado = $(this).data('estado') || '';
            cargar();
        });

        cargar();
    }

    // ──────────────────────── CREATE ─────────────────────────
    function initCreate(urls) {
        var tipo = 'Servicio';
        var lineas = [];
        var servicios = [], productos = [];

        function fillItemSelect() {
            var src = (tipo === 'Servicio') ? servicios : productos;
            var sel = $('#est-item').empty();
            sel.append('<option value="">— Seleccionar —</option>');
            src.forEach(function (x) {
                sel.append('<option value="' + x.id + '" data-precio="' + x.precio + '" data-nombre="' +
                    $('<div>').text(x.nombre).html() + '">' + x.nombre + '</option>');
            });
        }

        function recalc() {
            var total = lineas.reduce(function (a, l) { return a + (l.cantidad * l.precioUnit); }, 0);
            $('#est-total').text(money(total));
            $('#est-guardar-btn').prop('disabled', lineas.length === 0);
        }

        function renderLines() {
            var tb = $('#est-lines-body').empty();
            if (!lineas.length) {
                tb.append('<tr><td colspan="6" class="text-center" style="padding:20px;color:#999;">Aún no hay líneas.</td></tr>');
                recalc(); return;
            }
            lineas.forEach(function (l, i) {
                tb.append(
                    '<tr>' +
                    '<td>' + l.tipo + '</td>' +
                    '<td>' + $('<div>').text(l.descripcion).html() + '</td>' +
                    '<td class="text-center">' + l.cantidad + '</td>' +
                    '<td class="text-right">' + money(l.precioUnit) + '</td>' +
                    '<td class="text-right">' + money(l.cantidad * l.precioUnit) + '</td>' +
                    '<td class="text-center"><button type="button" class="est-del" data-i="' + i + '"><i class="fas fa-trash"></i></button></td>' +
                    '</tr>');
            });
            recalc();
        }

        // Cargar dropdowns
        $.getJSON(urls.clientesDropdown, function (data) {
            var sel = $('#est-cliente').empty();
            sel.append('<option value="">— Consumidor final —</option>');
            (data || []).forEach(function (c) {
                sel.append('<option value="' + c.id + '">' + $('<div>').text(c.nombre).html() + '</option>');
            });
        });
        $.getJSON(urls.serviciosDropdown, function (data) { servicios = data || []; if (tipo === 'Servicio') fillItemSelect(); });
        $.getJSON(urls.productosDropdown, function (data) { productos = data || []; });

        $('#est-type-toggle').on('click', 'button', function () {
            $('#est-type-toggle button').removeClass('active');
            $(this).addClass('active');
            tipo = $(this).data('tipo');
            fillItemSelect();
            $('#est-desc').val(''); $('#est-precio').val('0.00');
        });

        $('#est-item').on('change', function () {
            var opt = $(this).find('option:selected');
            if (opt.val()) {
                $('#est-desc').val(opt.data('nombre'));
                $('#est-precio').val(Number(opt.data('precio') || 0).toFixed(2));
            }
        });

        $('#est-add-btn').on('click', function () {
            var opt = $('#est-item').find('option:selected');
            var desc = ($('#est-desc').val() || '').trim();
            var cant = parseFloat($('#est-cant').val());
            var precio = parseFloat($('#est-precio').val());
            if (!desc) { alert('Ingrese una descripción.'); return; }
            if (!(cant > 0)) { alert('La cantidad debe ser mayor que cero.'); return; }
            if (!(precio >= 0)) { alert('Precio inválido.'); return; }
            lineas.push({
                tipo: tipo,
                servId: tipo === 'Servicio' && opt.val() ? parseInt(opt.val()) : null,
                itmId: tipo === 'Producto' && opt.val() ? parseInt(opt.val()) : null,
                descripcion: desc,
                cantidad: cant,
                precioUnit: precio
            });
            renderLines();
            $('#est-item').val(''); $('#est-desc').val(''); $('#est-cant').val('1'); $('#est-precio').val('0.00');
        });

        $('#est-lines-body').on('click', '.est-del', function () {
            lineas.splice(parseInt($(this).data('i')), 1);
            renderLines();
        });

        $('#est-limpiar-btn').on('click', function () {
            lineas = []; renderLines();
            $('#est-cliente').val(''); $('#est-mascota').val(''); $('#est-vigencia').val(''); $('#est-notas').val('');
        });

        $('#est-guardar-btn').on('click', function () {
            if (!lineas.length) return;
            var btn = $(this).prop('disabled', true);
            var payload = {
                estId: null,
                cliId: $('#est-cliente').val() ? parseInt($('#est-cliente').val()) : null,
                mascId: $('#est-mascota').val() ? parseInt($('#est-mascota').val()) : null,
                vigencia: $('#est-vigencia').val() || null,
                notas: ($('#est-notas').val() || '').trim() || null,
                lineas: lineas
            };
            $.ajax({
                url: urls.guardar, method: 'POST', contentType: 'application/json',
                data: JSON.stringify(payload)
            }).done(function (r) {
                if (r && r.success) {
                    window.location.href = urls.detailUrl + '/' + r.estId;
                } else {
                    alert((r && r.message) || 'No se pudo guardar.');
                    btn.prop('disabled', false);
                }
            }).fail(function () {
                alert('Error al guardar el presupuesto.');
                btn.prop('disabled', false);
            });
        });

        renderLines();
    }

    // ──────────────────────── DETAIL ─────────────────────────
    function initDetail(urls, estId) {
        $('#est-id-lbl').text(estId);

        function postJson(url, body, okFn) {
            $.ajax({ url: url, method: 'POST', contentType: 'application/json', data: JSON.stringify(body) })
                .done(okFn)
                .fail(function () { alert('Error al procesar la solicitud.'); cargar(); });
        }

        function cambiar(estado) {
            postJson(urls.cambiarEstado, { estId: estId, estado: estado }, function (r) {
                if (!r || !r.success) alert((r && r.message) || 'No se pudo cambiar el estado.');
                cargar();
            });
        }

        function convertir() {
            if (!confirm('¿Convertir este presupuesto en factura? Esta acción no se puede deshacer.')) return;
            postJson(urls.convertir, { estId: estId }, function (r) {
                if (r && r.success) {
                    alert('Factura creada (#' + r.factId + ').');
                } else {
                    alert((r && r.message) || 'No se pudo convertir.');
                }
                cargar();
            });
        }

        function renderActions(estado, facturaId) {
            var box = $('#est-actions').empty();
            if (estado === 'Borrador') {
                box.append(btn('info', 'paper-plane', 'Marcar como Enviado', function () { cambiar('Enviado'); }));
            }
            if (estado === 'Borrador' || estado === 'Enviado') {
                box.append(btn('success', 'check', 'Aceptar', function () { cambiar('Aceptado'); }));
                box.append(btn('danger', 'times', 'Rechazar', function () { cambiar('Rechazado'); }));
            }
            if (estado === 'Aceptado') {
                box.append(btn('success', 'file-invoice-dollar', 'Convertir a factura', convertir));
            }
            if (!box.children().length && estado !== 'Convertido') {
                box.append('<p style="color:#888;">Sin acciones disponibles.</p>');
            }
            var link = $('#est-factura-link').empty();
            if (estado === 'Convertido' && facturaId) {
                link.html('<div class="est-conv-note"><i class="fas fa-check-circle"></i> Convertido en la factura #' +
                    facturaId + '. <a href="' + urls.facturaUrl + '">Ir a facturación</a></div>');
            }
        }

        function btn(variant, icon, text, fn) {
            var b = $('<button type="button" class="est-btn est-btn-' + variant + '"><i class="fas fa-' + icon + '"></i>' + text + '</button>');
            b.on('click', fn);
            return b;
        }

        function cargar() {
            $.getJSON(urls.detalle, { id: estId }, function (resp) {
                if (!resp || !resp.header) {
                    $('#est-header-body').html('<div class="est-empty">No se encontró el presupuesto.</div>');
                    return;
                }
                var h = resp.header;
                $('#est-estado-badge').attr('class', 'est-badge est-' + (h.est_Estado || '').toLowerCase()).text(h.est_Estado);
                $('#est-header-body').html(
                    row('Cliente', h.cliente || 'Consumidor final') +
                    row('Identidad', h.cli_Identidad || '—') +
                    row('Mascota', h.mascota || '—') +
                    row('Fecha', fecha(h.est_Fecha)) +
                    row('Vigencia', fecha(h.est_Vigencia)) +
                    row('Notas', h.est_Notas || '—')
                );
                $('#est-detail-total').text(money(h.est_Total));

                var tb = $('#est-detail-lines').empty();
                (resp.lineas || []).forEach(function (l) {
                    tb.append(
                        '<tr>' +
                        '<td>' + l.estd_Tipo + '</td>' +
                        '<td>' + $('<div>').text(l.estd_Descripcion).html() + '</td>' +
                        '<td class="text-center">' + l.estd_Cantidad + '</td>' +
                        '<td class="text-right">' + money(l.estd_PrecioUnit) + '</td>' +
                        '<td class="text-right">' + money(l.estd_Subtotal) + '</td>' +
                        '</tr>');
                });
                renderActions(h.est_Estado, h.est_FacturaId);
            });
        }

        function row(k, v) {
            return '<div class="est-kv"><span class="k">' + k + '</span><span class="v">' + $('<div>').text(v).html() + '</span></div>';
        }

        cargar();
    }

    return { initIndex: initIndex, initCreate: initCreate, initDetail: initDetail };
})();
