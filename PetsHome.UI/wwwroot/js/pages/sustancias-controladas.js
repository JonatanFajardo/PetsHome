var SustanciasControladas = (function () {

    var obj = {};

    function tipoBadge(tipo) {
        var t = (tipo || '').toLowerCase();
        var map = {
            'recibido':     { txt: 'Recibido',     bg: '#dcfce7', fg: '#166534', icon: 'fa-arrow-down' },
            'administrado': { txt: 'Administrado',  bg: '#dbeafe', fg: '#1e40af', icon: 'fa-syringe' },
            'desechado':    { txt: 'Desechado',     bg: '#fee2e2', fg: '#991b1b', icon: 'fa-trash-can' },
            'devuelto':     { txt: 'Devuelto',      bg: '#fef3c7', fg: '#92400e', icon: 'fa-rotate-left' }
        };
        var c = map[t] || { txt: tipo || '—', bg: '#ede9fe', fg: '#6b21a8', icon: 'fa-circle' };
        return '<span style="background:' + c.bg + ';color:' + c.fg + ';padding:3px 10px;border-radius:12px;font-size:12px;font-weight:600;white-space:nowrap;">'
            + '<i class="fas ' + c.icon + ' mr-1"></i>' + c.txt + '</span>';
    }

    function texto(data) {
        if (!data) return '<span style="color:#9ca3af;">—</span>';
        return '<span style="color:#374151;">' + data + '</span>';
    }

    obj.datatable = function (Direction) {
        $(function () {
            var header = [
                { FieldName: 'scm_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'Farmaco', Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        var dea = row['scm_DeaSchedule'] ? ' <span style="background:#ede9fe;color:#6b21a8;border-radius:8px;padding:1px 6px;font-size:11px;font-weight:700;">DEA ' + row['scm_DeaSchedule'] + '</span>' : '';
                        return '<div style="display:flex;align-items:center;gap:8px;">'
                            + '<span style="background:#ede9fe;border-radius:50%;width:30px;height:30px;display:inline-flex;align-items:center;justify-content:center;">'
                            + '<i class="fas fa-pills" style="color:#7c3aed;font-size:13px;"></i></span>'
                            + '<span style="font-weight:600;color:#111827;">' + data + dea + '</span></div>';
                    }
                },
                {
                    FieldName: 'scm_Tipo', Size: 130, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        return tipoBadge(data);
                    }
                },
                {
                    FieldName: 'scm_Cantidad', Size: 100, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        var n = (data === null || data === undefined) ? 0 : parseFloat(data);
                        return '<span style="font-weight:600;color:#374151;">' + n.toLocaleString('es-HN', { maximumFractionDigits: 3 }) + '</span>';
                    }
                },
                {
                    FieldName: 'Mascota', Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="color:#374151;"><i class="fas fa-paw mr-1" style="color:#9ca3af;font-size:11px;"></i>' + data + '</span>';
                    }
                },
                {
                    FieldName: 'Testigo', Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="color:#374151;"><i class="fas fa-user-shield mr-1" style="color:#9ca3af;font-size:11px;"></i>' + data + '</span>';
                    }
                },
                {
                    FieldName: 'scm_Fecha', Size: 150, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        var d = new Date(data);
                        if (isNaN(d.getTime())) return data;
                        return '<span style="color:#374151;">' + d.toLocaleDateString('es-HN') + ' ' + d.toLocaleTimeString('es-HN', { hour: '2-digit', minute: '2-digit' }) + '</span>';
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    };

    // Rellena el id del movimiento a anular en el modal de confirmacion.
    $(document).on('click', '.delete-btn', function () {
        $('#delete-item-id').val($(this).data('id'));
    });

    return obj;

}());
