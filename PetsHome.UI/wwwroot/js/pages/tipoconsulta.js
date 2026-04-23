var TipoConsulta = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoCon_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'tipoCon_Descripcion',
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-stethoscope" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'tipoCon_EsActivo',
                    Size: 140,
                    render: function (data, type) {
                        if (type === 'display') {
                            var isActive = data === true || data === 1 || data === "Activo" || data === "true";
                            return '<span class="status-badge ' + (isActive ? 'status-activo' : 'status-inactivo') + '">'
                                 + (isActive ? 'Activo' : 'Inactivo') + '</span>';
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
