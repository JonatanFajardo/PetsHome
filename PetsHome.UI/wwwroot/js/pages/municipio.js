var Municipio = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        var header = [
            { FieldName: "mpio_Id", Size: 60, Visibility: false },
            {
                FieldName: "mpio_Codigo",
                Size: 100,
                Render: function (data, type, row) {
                    if (type === 'display') {
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="font-family:monospace;font-weight:600;letter-spacing:0.5px;">' + data + '</span>';
                    }
                    return data;
                }
            },
            {
                FieldName: "mpio_Descripcion",
                Render: function (data, type, row) {
                    if (type === 'display') {
                        if (!data) return '<span style="color:#9ca3af;">—</span>';
                        return '<span style="font-weight:500;color:#111827;">' + data + '</span>';
                    }
                    return data;
                }
            }
        ];
        datatablePartials.init(Direction, header);
    }

    return obj;

}());
