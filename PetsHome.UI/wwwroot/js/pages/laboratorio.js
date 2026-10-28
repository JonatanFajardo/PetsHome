var Laboratorio = (function () {

    var obj = {};

    function estadoBadge(estado) {
        var e = (estado || '').toLowerCase();
        var map = {
            'pendiente':  { txt: 'Pendiente',  bg: '#fef3c7', fg: '#92400e', icon: 'fa-hourglass-half' },
            'completado': { txt: 'Completado', bg: '#dbeafe', fg: '#1e40af', icon: 'fa-vial-circle-check' },
            'revisado':   { txt: 'Revisado',   bg: '#dcfce7', fg: '#166534', icon: 'fa-check-double' }
        };
        var c = map[e] || { txt: estado || '—', bg: '#ede9fe', fg: '#6b21a8', icon: 'fa-circle' };
        return '<span style="background:' + c.bg + ';color:' + c.fg + ';padding:3px 10px;border-radius:12px;font-size:12px;font-weight:600;white-space:nowrap;">'
            + '<i class="fas ' + c.icon + ' mr-1"></i>' + c.txt + '</span>';
    }

    obj.datatable = function (Direction) {
        $(function () {
            var header = [
                { FieldName: 'lab_Id', Size: 70, Visibility: false },
                {
                    FieldName: 'lab_Examen', Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<div style="display:flex;align-items:center;gap:8px;">'
                            + '<span style="background:#ede9fe;border-radius:50%;width:30px;height:30px;display:inline-flex;align-items:center;justify-content:center;">'
                            + '<i class="fas fa-flask" style="color:#7c3aed;font-size:13px;"></i></span>'
                            + '<span style="font-weight:600;color:#111827;">' + data + '</span></div>';
                    }
                },
                {
                    FieldName: 'lab_Tipo', Size: 110, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="color:#374151;">' + data + '</span>';
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
                    FieldName: 'lab_Estado', Size: 130, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        return estadoBadge(data);
                    }
                },
                {
                    FieldName: 'lab_FechaSolicitud', Size: 150, Visibility: true,
                    Render: function (data, type) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        var d = new Date(data);
                        if (isNaN(d.getTime())) return data;
                        return '<span style="color:#374151;">' + d.toLocaleDateString('es-HN') + '</span>';
                    }
                },
                {
                    FieldName: 'ParametrosFueraRango', Size: 120, Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        var total = row['TotalParametros'] || 0;
                        if (data > 0) {
                            return '<span style="background:#fee2e2;color:#991b1b;padding:3px 10px;border-radius:12px;font-size:12px;font-weight:700;">'
                                + '<i class="fas fa-triangle-exclamation mr-1"></i>' + data + ' de ' + total + '</span>';
                        }
                        if (total > 0) {
                            return '<span style="background:#dcfce7;color:#166534;padding:3px 10px;border-radius:12px;font-size:12px;font-weight:600;">'
                                + '<i class="fas fa-check mr-1"></i>En rango</span>';
                        }
                        return '<span style="color:#9ca3af;">Sin datos</span>';
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    };

    return obj;

}());
