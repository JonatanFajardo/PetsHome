var RecepcionDetalle = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        var header = [
            { FieldName: "recdet_Id", Visibility: false },
            {
                FieldName: "itm_Descripcion",
                render: function (data, type) {
                    if (type === 'display') {
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                        return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                             +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                             +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                             +     '<i class="fa fa-box" style="color:#7c3aed;font-size:13px;"></i>'
                             +   '</span>'
                             +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                             + '</span>';
                    }
                    return data;
                }
            },
            {
                FieldName: "recdet_Cantidad",
                render: function (data, type) {
                    if (type === 'display') {
                        if (data === null || data === undefined || data === '') return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="font-weight:600;color:#111827;">' + data + '</span>';
                    }
                    return data;
                }
            },
            {
                FieldName: "recdet_PrecioUnitario",
                render: function (data, type) {
                    if (type === 'display') {
                        if (data === null || data === undefined || data === '') return '<span style="color:#9ca3af;">—</span>';
                        var num = parseFloat(data);
                        if (isNaN(num)) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="font-weight:500;color:#111827;">$' + num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</span>';
                    }
                    return data;
                }
            },
            {
                FieldName: "recdet_FechaVencimiento",
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
                FieldName: "recdet_NumeroLote",
                render: function (data, type) {
                    if (type === 'display') {
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="font-family:monospace;letter-spacing:0.5px;color:#374151;">' + data + '</span>';
                    }
                    return data;
                }
            }
        ];
        datatablePartials.init(Direction, header);
    }

    return obj;

}());
