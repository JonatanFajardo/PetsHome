
var Clientes = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'cli_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'cli_Nombres',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<div style="display:flex;align-items:center;gap:8px;">'
                                + '<span style="background:#ede9fe;border-radius:50%;width:30px;height:30px;display:inline-flex;align-items:center;justify-content:center;">'
                                + '<i class="fas fa-user" style="color:#7c3aed;font-size:13px;"></i></span>'
                                + '<span style="font-weight:600;color:#111827;">' + data + '</span>'
                                + '</div>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cli_Apellidos',
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
                    FieldName: 'cli_Identidad',
                    Size: 140,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-family:monospace;color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cli_Telefono',
                    Size: 130,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;"><i class="fas fa-phone mr-1" style="color:#9ca3af;font-size:11px;"></i>' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cli_Correo',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<a href="mailto:' + data + '" style="color:#7c3aed;"><i class="fas fa-envelope mr-1" style="font-size:11px;"></i>' + data + '</a>';
                        }
                        return data;
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    };

    return obj;

}());
