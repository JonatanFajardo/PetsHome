var Voluntario = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'vol_Id',
                    Size: 60,
                    Visibility: false,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'vol_HorasTrabajadas',
                    Size: 100,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="font-weight: 500;">' + data + ' hrs</span>';
                    }
                },
                {
                    FieldName: 'vol_Nombres',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + data + '</div>';
                    }
                },
                {
                    FieldName: 'per_Identidad',
                    Size: 150,
                    Visibility: true
                }
            ];

            // Inicializar datatable
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