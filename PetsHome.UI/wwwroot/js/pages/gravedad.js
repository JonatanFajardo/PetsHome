var Gravedad = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'grav_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'grav_Descripcion',
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-weight:500;color:#111827;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'grav_EsActivo',
                    Size: 140,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            var activo = data === true || data === 1 || data === 'Activo' || data === 'true';
                            var label = activo ? 'Activo' : 'Inactivo';
                            var cls = activo ? 'status-activo' : 'status-inactivo';
                            return '<span class="status-badge ' + cls + '">' + label + '</span>';
                        }
                        return data;
                    }
                }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }

    return obj;
}());
