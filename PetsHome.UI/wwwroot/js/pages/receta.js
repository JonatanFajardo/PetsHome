var Receta = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            header = [
                {
                    FieldName: 'receta_Id',
                    Size: 60,
                    Visibility: false
                },
                {
                    FieldName: 'masc_Nombre',
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-paw" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'receta_Medicamento',
                    Size: 150,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-weight:500;color:#111827;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'TipoMedicamento',
                    Size: 120,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                                 + 'font-size:12px;font-weight:600;background:#ede9fe;color:#7c3aed;">'
                                 + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'receta_Dosis',
                    Size: 90,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (data === null || data === undefined || data === '') return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-weight:600;color:#111827;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'receta_Frecuencia',
                    Size: 100,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'receta_Duracion',
                    Size: 100,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'receta_Estado',
                    Size: 110,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var val = data.toString().trim();
                            var lower = val.toLowerCase();
                            var bg, color;
                            if (lower === 'activa')          { bg = '#dcfce7'; color = '#15803d'; }
                            else if (lower === 'completada') { bg = '#dbeafe'; color = '#1d4ed8'; }
                            else if (lower === 'cancelada')  { bg = '#fee2e2'; color = '#b91c1c'; }
                            else                             { bg = '#f3f4f6'; color = '#374151'; }
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                                 + 'font-size:12px;font-weight:600;background:' + bg + ';color:' + color + ';">'
                                 + val + '</span>';
                        }
                        return data;
                    }
                }
            ];

            datatable.init(Direction, header);
        });
    }

    return obj;

}());

function deleteReceta(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
