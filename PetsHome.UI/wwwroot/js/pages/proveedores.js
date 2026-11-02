
var Proveedores = (function () {

    var obj = {};

    function dash(data) {
        if (!data) return '<span style="color:#9ca3af;">—</span>';
        return data;
    }

    obj.datatable = function (Direction) {
        $(function () {
            var header = [
                { FieldName: 'prov_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'prov_Nombre',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        if (!data) return dash(data);
                        return '<div style="display:flex;align-items:center;gap:8px;">'
                            + '<span style="background:#dbeafe;border-radius:6px;padding:4px 7px;">'
                            + '<i class="fas fa-truck" style="color:#2563eb;font-size:13px;"></i></span>'
                            + '<span style="font-weight:500;color:#111827;">' + data + '</span>'
                            + '</div>';
                    }
                },
                {
                    FieldName: 'prov_Email',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        if (!data) return dash(data);
                        return '<span style="color:#374151;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'prov_Telefono',
                    Size: 160,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        if (!data) return dash(data);
                        return '<span style="font-family:monospace;color:#374151;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'prov_EsActivo',
                    Size: 110,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        var activo = data === 'Activo';
                        var bg = activo ? '#dcfce7' : '#fee2e2';
                        var color = activo ? '#166534' : '#991b1b';
                        return '<span style="background:' + bg + ';color:' + color
                            + ';border-radius:12px;padding:3px 10px;font-size:12px;font-weight:600;">'
                            + (data || '—') + '</span>';
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    };

    return obj;

}());
