var RecepcionMercancia = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'recep_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'recep_Descripcion',
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-truck" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'recep_Fecha',
                    Size: 110,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var date = new Date(data);
                            if (isNaN(date.getTime())) return '<span style="color:#9ca3af;">—</span>';
                            return date.toLocaleDateString('es-HN', { day: '2-digit', month: '2-digit', year: 'numeric' });
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'refg_Nombre',
                    Size: 150,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="font-weight:500;color:#111827;">' + label + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'recep_TipoRecepcion',
                    Size: 130,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var val = data.toString().trim();
                            var lower = val.toLowerCase();
                            var bg, color;
                            if (lower === 'donacion' || lower === 'donación') { bg = '#dcfce7'; color = '#15803d'; }
                            else if (lower === 'compra')                      { bg = '#dbeafe'; color = '#1d4ed8'; }
                            else if (lower === 'traslado')                    { bg = '#fef9c3'; color = '#a16207'; }
                            else                                              { bg = '#f3f4f6'; color = '#374151'; }
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                                 + 'font-size:12px;font-weight:600;background:' + bg + ';color:' + color + ';">'
                                 + val + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'recep_NumeroDocumento',
                    Size: 130,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-family:monospace;letter-spacing:0.5px;color:#374151;">' + data + '</span>';
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
