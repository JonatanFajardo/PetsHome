
var Servicios = (function () {

    var obj = {};

    // Formatea un numero como moneda hondurena: L. 400.00
    function formatMoney(data) {
        if (data === null || data === undefined || data === '') return '<span style="color:#9ca3af;">—</span>';
        var val = parseFloat(data);
        if (isNaN(val)) return '<span style="color:#9ca3af;">—</span>';
        return '<span style="font-weight:600;color:#111827;">L. '
            + val.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
            + '</span>';
    }

    function dash(data) {
        if (!data) return '<span style="color:#9ca3af;">—</span>';
        return data;
    }

    obj.datatable = function (Direction) {
        $(function () {
            var header = [
                { FieldName: 'serv_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'serv_Codigo',
                    Size: 120,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="font-family:monospace;font-weight:600;letter-spacing:0.5px;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'serv_Nombre',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<div style="display:flex;align-items:center;gap:8px;">'
                            + '<span style="background:#ede9fe;border-radius:6px;padding:4px 7px;">'
                            + '<i class="fas fa-concierge-bell" style="color:#7c3aed;font-size:13px;"></i></span>'
                            + '<span style="font-weight:500;color:#111827;">' + data + '</span>'
                            + '</div>';
                    }
                },
                {
                    FieldName: 'serv_Descripcion',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="color:#374151;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'serv_Precio',
                    Size: 130,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type !== 'display') return data;
                        return formatMoney(data);
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    };

    return obj;

}());
