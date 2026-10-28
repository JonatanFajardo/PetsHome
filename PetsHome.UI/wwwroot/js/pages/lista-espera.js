var ListaEspera = (function () {

    var obj = {};

    function estadoBadge(estado) {
        var e = (estado || '').toLowerCase();
        var map = {
            'esperando': { txt: 'Esperando', bg: '#fef3c7', fg: '#92400e', icon: 'fa-hourglass-half' },
            'agendada':  { txt: 'Agendada',  bg: '#dcfce7', fg: '#166534', icon: 'fa-calendar-check' },
            'cancelada': { txt: 'Cancelada', bg: '#fee2e2', fg: '#991b1b', icon: 'fa-ban' }
        };
        var c = map[e] || { txt: estado || '—', bg: '#ede9fe', fg: '#6b21a8', icon: 'fa-circle' };
        return '<span style="background:' + c.bg + ';color:' + c.fg + ';padding:3px 10px;border-radius:12px;font-size:12px;font-weight:600;white-space:nowrap;">'
            + '<i class="fas ' + c.icon + ' mr-1"></i>' + c.txt + '</span>';
    }

    function prioridadBadge(prioridad) {
        var p = (prioridad || '').toLowerCase();
        var map = {
            'urgente': { bg: '#fee2e2', fg: '#991b1b', icon: 'fa-triangle-exclamation' },
            'alta':    { bg: '#ffedd5', fg: '#9a3412', icon: 'fa-arrow-up' },
            'normal':  { bg: '#f3f4f6', fg: '#374151', icon: 'fa-minus' }
        };
        var c = map[p] || { bg: '#f3f4f6', fg: '#374151', icon: 'fa-minus' };
        return '<span style="background:' + c.bg + ';color:' + c.fg + ';padding:3px 10px;border-radius:12px;font-size:12px;font-weight:600;white-space:nowrap;">'
            + '<i class="fas ' + c.icon + ' mr-1"></i>' + (prioridad || 'Normal') + '</span>';
    }

    function fechaCorta(data) {
        if (!data) return '<span style="color:#9ca3af;">Lo antes posible</span>';
        var d = new Date(data);
        if (isNaN(d.getTime())) return data;
        return '<span style="color:#374151;">' + d.toLocaleDateString('es-HN') + '</span>';
    }

    obj.datatable = function (Direction) {
        $(function () {
            var header = [
                { FieldName: 'lesp_Id', Size: 70, Visibility: false },
                {
                    FieldName: 'Mascota', Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<div style="display:flex;align-items:center;gap:8px;">'
                            + '<span style="background:#ede9fe;border-radius:50%;width:30px;height:30px;display:inline-flex;align-items:center;justify-content:center;">'
                            + '<i class="fas fa-paw" style="color:#7c3aed;font-size:13px;"></i></span>'
                            + '<span style="font-weight:600;color:#111827;">' + data + '</span></div>';
                    }
                },
                {
                    FieldName: 'Responsable', Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        var html = '<span style="color:#374151;">' + (data || '—') + '</span>';
                        if (row && row.Telefono) {
                            html += '<br><span style="color:#9ca3af;font-size:11px;"><i class="fas fa-phone mr-1"></i>' + row.Telefono + '</span>';
                        }
                        return html;
                    }
                },
                {
                    FieldName: 'TipoConsulta', Size: 130, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        return '<span style="color:#374151;">' + (data || 'Cualquiera') + '</span>';
                    }
                },
                {
                    FieldName: 'lesp_FechaDeseada', Size: 130, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        return fechaCorta(data);
                    }
                },
                {
                    FieldName: 'lesp_Prioridad', Size: 110, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        return prioridadBadge(data);
                    }
                },
                {
                    FieldName: 'lesp_Estado', Size: 120, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        return estadoBadge(data);
                    }
                },
                {
                    FieldName: 'DiasEsperando', Size: 110, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        var n = parseInt(data, 10) || 0;
                        var txt = n === 0 ? 'Hoy' : (n === 1 ? '1 día' : n + ' días');
                        return '<span style="color:#6b21a8;font-weight:600;">' + txt + '</span>';
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    };

    return obj;

}());
