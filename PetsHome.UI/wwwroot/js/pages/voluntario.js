var Voluntario = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            header = [
                { FieldName: 'vol_Id', Size: 60, Visibility: false },

                {
                    FieldName: 'vol_Nombres',
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var nombre = data.toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="font-weight:500;color:#111827;">' + nombre + '</span>';
                        }
                        return data;
                    }
                },

                {
                    FieldName: 'per_Identidad',
                    Size: 150,
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var limpio = String(data).replace(/\D/g, '');
                            var formateado = limpio.length === 13
                                ? limpio.slice(0, 4) + '-' + limpio.slice(4, 8) + '-' + limpio.slice(8)
                                : data;
                            return '<span style="font-family:monospace;font-size:13px;letter-spacing:0.5px;color:#374151;">' + formateado + '</span>';
                        }
                        return data;
                    }
                },

                {
                    FieldName: 'vol_HorasTrabajadas',
                    Size: 100,
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (data === null || data === undefined || data === '') {
                                return '<span style="color:#9ca3af;">—</span>';
                            }
                            var num = parseFloat(data);
                            var color = num >= 100 ? '#15803d' : num >= 50 ? '#1d4ed8' : '#374151';
                            return '<span style="font-weight:600;color:' + color + ';">' + num + '</span>'
                                 + '<span style="color:#9ca3af;font-size:12px;"> hrs</span>';
                        }
                        return data;
                    }
                }
            ];

            datatable.init(Direction, header);
        });
    }

    obj.initValidation = function (validarUrl) {
        if (!$.validator.methods.noSpaceAtStart) {
            $.validator.addMethod("noSpaceAtStart", function (value) {
                return value.length === 0 || value[0] !== ' ';
            }, "No puede comenzar con espacios.");
        }
        $(function () {
            var $form = $("form");
            $form.validate({
                rules: {
                    "per.per_Identidad": {
                        required: true,
                        maxlength: 20,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                per_Identidad: function () { return $("#per_per_Identidad").val(); },
                                vol_Id: function () { return $("#vol_Id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    "per.per_Identidad": {
                        required: "La identidad es requerida.",
                        maxlength: "Máximo 20 caracteres.",
                        remote: "Ya existe un voluntario con esa identidad."
                    }
                },
                errorElement: "span",
                errorClass: "text-danger",
                highlight: function (el) { $(el).addClass("is-invalid"); },
                unhighlight: function (el) { $(el).removeClass("is-invalid"); }
            });
        });
    };

    return obj;

}());

// Función global para eliminar voluntario
function deleteVoluntario(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}