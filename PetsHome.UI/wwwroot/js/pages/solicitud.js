var Solicitud = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: "sol_Id", Size: 60, Visibility: false },
                {
                    FieldName: "sol_Identidad",
                    Size: 130,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var digits = data.toString().replace(/\D/g, '');
                            if (digits.length === 13) {
                                data = digits.slice(0,4) + '-' + digits.slice(4,8) + '-' + digits.slice(8);
                            }
                            return '<span style="font-family:monospace;letter-spacing:0.5px;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "sol_Nombres",
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-user" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "masc_Nombre",
                    Size: 150,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:6px;">'
                                 +   '<i class="fa fa-paw" style="color:#7c3aed;font-size:12px;"></i>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "sol_Correo",
                    Size: 180,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var email = data.toString().toLowerCase();
                            return '<a href="mailto:' + email + '" style="color:#7c3aed;text-decoration:none;">' + email + '</a>';
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
