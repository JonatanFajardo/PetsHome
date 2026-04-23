var TipoEsterilizacion = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoEst_Id', Size: 80, Visibility: false },
                {
                    FieldName: 'tipoEst_Descripcion',
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-venus-mars" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'tipoEst_Sexo',
                    Size: 150,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var val = data.toString().trim();
                            var lower = val.toLowerCase();
                            var bg, color;
                            if (lower === 'macho')       { bg = '#dbeafe'; color = '#1d4ed8'; }
                            else if (lower === 'hembra') { bg = '#fce7f3'; color = '#be185d'; }
                            else if (lower === 'ambos')  { bg = '#ede9fe'; color = '#7c3aed'; }
                            else                         { bg = '#f3f4f6'; color = '#374151'; }
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                                 + 'font-size:12px;font-weight:600;background:' + bg + ';color:' + color + ';">'
                                 + val + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'tipoEst_EsActivo',
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
