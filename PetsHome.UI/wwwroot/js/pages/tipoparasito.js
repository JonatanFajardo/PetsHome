var TipoParasito = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoPar_Id', Size: 80, Visibility: false },
                {
                    FieldName: 'tipoPar_Descripcion',
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-bug" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'tipoPar_Categoria',
                    Size: 150,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var val = data.toString().trim();
                            var lower = val.toLowerCase();
                            var bg, color;
                            if (lower === 'externo')      { bg = '#fef9c3'; color = '#a16207'; }
                            else if (lower === 'interno') { bg = '#fee2e2'; color = '#b91c1c'; }
                            else                          { bg = '#f3f4f6'; color = '#374151'; }
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                                 + 'font-size:12px;font-weight:600;background:' + bg + ';color:' + color + ';">'
                                 + val + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'tipoPar_EsActivo',
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
