// ─────────────────────────────────────────────────────────────
// Recordatorios / recalls (care-reminders). Cola de seguimiento
// clinico. Lista con filtros + acciones (Hecho / Descartar / Enviado)
// + boton "Generar" que crea recordatorios desde la data real.
// Dependencias: jQuery (ya cargado) y SweetAlert2 (Swal, opcional).
// ─────────────────────────────────────────────────────────────
(function () {
    var U = window._urls || {};
    var filas = [];

    function esc(s) {
        if (s === null || s === undefined) return '';
        return String(s).replace(/[&<>"']/g, function (c) {
            return { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c];
        });
    }

    function fechaCorta(data) {
        if (!data) return '—';
        var d = new Date(data);
        if (isNaN(d.getTime())) return esc(data);
        return d.toLocaleDateString('es-HN');
    }

    function tipoBadge(tipo) {
        var map = {
            'VacunaRefuerzo': { txt: 'Refuerzo de vacuna', bg: '#dbeafe', fg: '#1e40af', icon: 'fa-syringe' },
            'Seguimiento':    { txt: 'Seguimiento',        bg: '#ede9fe', fg: '#6b21a8', icon: 'fa-stethoscope' },
            'Control':        { txt: 'Control',            bg: '#dcfce7', fg: '#166534', icon: 'fa-clipboard-check' }
        };
        var c = map[tipo] || { txt: tipo || '—', bg: '#f3f4f6', fg: '#374151', icon: 'fa-circle' };
        return '<span style="background:' + c.bg + ';color:' + c.fg + ';padding:3px 10px;border-radius:12px;font-size:12px;font-weight:600;white-space:nowrap;">'
            + '<i class="fas ' + c.icon + ' mr-1"></i>' + c.txt + '</span>';
    }

    function estadoBadge(estado) {
        var map = {
            'Pendiente':  { bg: '#fef3c7', fg: '#92400e', icon: 'fa-hourglass-half' },
            'Enviado':    { bg: '#dbeafe', fg: '#1e40af', icon: 'fa-paper-plane' },
            'Hecho':      { bg: '#dcfce7', fg: '#166534', icon: 'fa-check' },
            'Descartado': { bg: '#fee2e2', fg: '#991b1b', icon: 'fa-ban' }
        };
        var c = map[estado] || { bg: '#f3f4f6', fg: '#374151', icon: 'fa-circle' };
        return '<span style="background:' + c.bg + ';color:' + c.fg + ';padding:3px 10px;border-radius:12px;font-size:12px;font-weight:600;white-space:nowrap;">'
            + '<i class="fas ' + c.icon + ' mr-1"></i>' + esc(estado || '—') + '</span>';
    }

    function faltan(row) {
        var n = parseInt(row.DiasRestantes, 10);
        if (isNaN(n)) return '—';
        if (row.EsVencido) {
            return '<span style="color:#991b1b;font-weight:700;"><i class="fas fa-triangle-exclamation mr-1"></i>Vencido ' + Math.abs(n) + ' d</span>';
        }
        if (n === 0) return '<span style="color:#b45309;font-weight:700;">Hoy</span>';
        if (n < 0) return '<span style="color:#991b1b;font-weight:700;">Hace ' + Math.abs(n) + ' d</span>';
        return '<span style="color:#374151;">En ' + n + ' d</span>';
    }

    function acciones(row) {
        var terminal = row.rec_Estado === 'Hecho' || row.rec_Estado === 'Descartado';
        var html = '<div style="display:flex;gap:6px;justify-content:center;flex-wrap:wrap;">';
        if (!terminal) {
            html += '<button class="btn btn-sm btn-success js-hecho" data-id="' + row.rec_Id + '" title="Marcar como hecho"><i class="fas fa-check"></i></button>';
            html += '<button class="btn btn-sm btn-outline-danger js-descartar" data-id="' + row.rec_Id + '" title="Descartar"><i class="fas fa-ban"></i></button>';
        } else {
            html += '<span style="color:#9ca3af;font-size:12px;">' + esc(row.rec_Estado) + '</span>';
        }
        html += '</div>';
        return html;
    }

    function pintar(rows) {
        var $b = $('#cuerpoTabla');
        if (!rows || rows.length === 0) {
            $b.html('<tr><td colspan="8" style="text-align:center;padding:24px;color:#9ca3af;">No hay recordatorios para este filtro.</td></tr>');
            return;
        }
        var html = '';
        rows.forEach(function (r) {
            html += '<tr>'
                + '<td><span style="font-weight:600;color:#111827;"><i class="fas fa-paw mr-2" style="color:#7c3aed;"></i>' + esc(r.Mascota) + '</span></td>'
                + '<td>' + esc(r.Responsable) + (r.Telefono ? '<br><span style="color:#9ca3af;font-size:11px;"><i class="fas fa-phone mr-1"></i>' + esc(r.Telefono) + '</span>' : '') + '</td>'
                + '<td>' + tipoBadge(r.rec_Tipo) + '</td>'
                + '<td>' + fechaCorta(r.rec_FechaObjetivo) + '</td>'
                + '<td>' + faltan(r) + '</td>'
                + '<td>' + estadoBadge(r.rec_Estado) + '</td>'
                + '<td><span style="color:#6b7280;font-size:13px;">' + esc(r.rec_Nota) + '</span></td>'
                + '<td class="text-center">' + acciones(r) + '</td>'
                + '</tr>';
        });
        $b.html(html);
    }

    function cargar() {
        var params = {
            estado: $('#fEstado').val(),
            desde: $('#fDesde').val() || null,
            hasta: $('#fHasta').val() || null
        };
        $('#cuerpoTabla').html('<tr><td colspan="8" style="text-align:center;padding:24px;color:#9ca3af;">Cargando…</td></tr>');
        $.get(U.list, params, function (resp) {
            filas = (resp && resp.data) || [];
            aplicarBusqueda();
        }).fail(function () {
            $('#cuerpoTabla').html('<tr><td colspan="8" style="text-align:center;padding:24px;color:#991b1b;">No se pudo cargar.</td></tr>');
        });
    }

    function aplicarBusqueda() {
        var q = ($('#globalSearch').val() || '').toLowerCase();
        if (!q) { pintar(filas); return; }
        pintar(filas.filter(function (r) {
            return (esc(r.Mascota) + ' ' + esc(r.Responsable) + ' ' + esc(r.rec_Tipo) + ' ' + esc(r.rec_Nota)).toLowerCase().indexOf(q) >= 0;
        }));
    }

    function toast(ok, msg) {
        if (window.Swal) {
            Swal.fire({ icon: ok ? 'success' : 'error', title: ok ? 'Listo' : 'Atención', text: msg, timer: ok ? 2200 : undefined, showConfirmButton: !ok });
        } else {
            alert(msg);
        }
    }

    function postAccion(url, data, cb) {
        $.post(url, data, function (resp) {
            toast(resp && resp.ok, (resp && resp.mensaje) || '');
            if (cb) cb(resp);
        }).fail(function () { toast(false, 'No se pudo completar la acción.'); });
    }

    $(function () {
        $('#btnFiltrar').on('click', cargar);
        $('#fEstado').on('change', cargar);
        $('#globalSearch').on('keyup', aplicarBusqueda);

        $('#btnGenerar').on('click', function () {
            var $btn = $(this).prop('disabled', true);
            postAccion(U.generar, {}, function () { cargar(); });
            setTimeout(function () { $btn.prop('disabled', false); }, 1200);
        });

        $('#cuerpoTabla').on('click', '.js-hecho', function () {
            postAccion(U.marcarHecho, { rec_Id: $(this).data('id') }, function () { cargar(); });
        });

        $('#cuerpoTabla').on('click', '.js-descartar', function () {
            var id = $(this).data('id');
            if (window.Swal) {
                Swal.fire({
                    title: 'Descartar recordatorio',
                    input: 'text',
                    inputLabel: '¿Por qué ya no aplica? (opcional)',
                    showCancelButton: true,
                    confirmButtonText: 'Descartar',
                    cancelButtonText: 'Cancelar'
                }).then(function (res) {
                    if (res.isConfirmed) postAccion(U.descartar, { rec_Id: id, nota: res.value || '' }, function () { cargar(); });
                });
            } else if (confirm('¿Descartar este recordatorio?')) {
                postAccion(U.descartar, { rec_Id: id, nota: '' }, function () { cargar(); });
            }
        });

        cargar();
    });
})();
