
var HistorialMedico = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'cita_Id', Visibility: false },
                {
                    FieldName: 'mascota',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<div style="display:flex;align-items:center;gap:8px;">'
                                + '<span style="background:#ede9fe;border-radius:6px;padding:4px 7px;">'
                                + '<i class="fas fa-paw" style="color:#7c3aed;font-size:13px;"></i></span>'
                                + '<span style="font-weight:500;color:#111827;">' + data + '</span>'
                                + '</div>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'esterilizacion',
                    Size: 120,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (data === null || data === undefined || data === '') return '<span style="color:#9ca3af;">—</span>';
                            var activo = data === true || data === 1 || data === 'Sí' || data === 'Si' || data === 'true';
                            var label = activo ? 'Sí' : 'No';
                            var cls = activo ? 'status-activo' : 'status-inactivo';
                            return '<span class="status-badge ' + cls + '">' + label + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'comportamiento',
                    Size: 120,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'saludcuidado',
                    Size: 130,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'informacionadicional',
                    Size: 150,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var corto = data.length > 60 ? data.substring(0, 60) + '…' : data;
                            return '<span title="' + data + '" style="color:#374151;">' + corto + '</span>';
                        }
                        return data;
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    }

    return obj;

}());
