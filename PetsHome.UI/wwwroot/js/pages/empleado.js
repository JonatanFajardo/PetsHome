var Empleado = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'emp_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'emp_Codigo',
                    Size: 100,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-family:monospace;font-weight:600;letter-spacing:0.5px;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'emp_Nombres',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var nombre = data.toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<div style="display:flex;align-items:center;gap:8px;">'
                                + '<span style="background:#ede9fe;border-radius:6px;padding:4px 7px;">'
                                + '<i class="fas fa-user" style="color:#7c3aed;font-size:13px;"></i></span>'
                                + '<span style="font-weight:500;color:#111827;">' + nombre + '</span>'
                                + '</div>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cag_Descripcion',
                    Size: 140,
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
                    FieldName: 'refg_Nombre',
                    Size: 150,
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
                    FieldName: 'esActivo',
                    Size: 140,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            var activo = data === true || data === 1 || (typeof data === 'string' && data.toLowerCase() === 'activo') || data === 'true';
                            var label = activo ? 'Activo' : 'Inactivo';
                            var cls = activo ? 'status-activo' : 'status-inactivo';
                            return '<span class="status-badge ' + cls + '">' + label + '</span>';
                        }
                        return data;
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    }

    obj.initValidation = function (validarUrl) {
        $(function () {
            var $identidad = $("#per_per_Identidad");
            if ($identidad.length) {
                $identidad.rules("add", {
                    remote: {
                        url: validarUrl,
                        type: "GET",
                        data: {
                            per_Identidad: function () { return $identidad.val(); },
                            emp_Id: function () { return $("#emp_Id").val(); }
                        }
                    },
                    messages: {
                        remote: "Ya existe un empleado con esa identidad."
                    }
                });
            }
        });
    };

    return obj;

}());

function deleteEmpleado(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
