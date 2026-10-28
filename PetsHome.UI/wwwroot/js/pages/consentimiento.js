var Consentimiento = (function () {

    var obj = {};

    function estadoBadge(estado) {
        var e = (estado || '').toLowerCase();
        var map = {
            'pendiente': { txt: 'Pendiente', bg: '#fef3c7', fg: '#92400e', icon: 'fa-hourglass-half' },
            'firmado':   { txt: 'Firmado',   bg: '#dcfce7', fg: '#166534', icon: 'fa-signature' },
            'rechazado': { txt: 'Rechazado', bg: '#fee2e2', fg: '#991b1b', icon: 'fa-ban' }
        };
        var c = map[e] || { txt: estado || '—', bg: '#ede9fe', fg: '#6b21a8', icon: 'fa-circle' };
        return '<span style="background:' + c.bg + ';color:' + c.fg + ';padding:3px 10px;border-radius:12px;font-size:12px;font-weight:600;white-space:nowrap;">'
            + '<i class="fas ' + c.icon + ' mr-1"></i>' + c.txt + '</span>';
    }

    function tipoLabel(tipo) {
        var map = { 'Cirugia': 'Cirugía', 'Anestesia': 'Anestesia', 'Eutanasia': 'Eutanasia', 'General': 'General' };
        return map[tipo] || tipo || '—';
    }

    obj.datatable = function (Direction) {
        $(function () {
            var header = [
                { FieldName: 'cons_Id', Size: 70, Visibility: false },
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
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="color:#374151;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'cons_Tipo', Size: 120, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        return '<span style="color:#374151;">' + tipoLabel(data) + '</span>';
                    }
                },
                {
                    FieldName: 'cons_Estado', Size: 130, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        return estadoBadge(data);
                    }
                },
                {
                    FieldName: 'cons_FirmanteNombre', Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="color:#374151;"><i class="fas fa-user-pen mr-1" style="color:#9ca3af;font-size:11px;"></i>' + data + '</span>';
                    }
                },
                {
                    FieldName: 'cons_FechaCrea', Size: 150, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        var d = new Date(data);
                        if (isNaN(d.getTime())) return data;
                        return '<span style="color:#374151;">' + d.toLocaleDateString('es-HN') + '</span>';
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    };

    return obj;

}());
