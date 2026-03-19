var Empleado = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'emp_Id',
                    Size: 60,
                    Visibility: false,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'emp_Codigo',
                    Size: 100,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="font-weight: 500;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'emp_Nombres',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + data + '</div>';
                    }
                },
                {
                    FieldName: 'cag_Descripcion',
                    Size: 140,
                    Visibility: true
                },
                {
                    FieldName: 'refg_Nombre',
                    Size: 150,
                    Visibility: true
                },
                {
                    FieldName: 'esActivo',
                    Size: 140,
                    Visibility: true,
                    Render: function(data, type, row) {
                        var estado = (data && data.toLowerCase() === 'activo') ? 'Activo' : 'Inactivo';
                        var badgeClass = estado === 'Activo' ? 'status-disponible' : 'status-adoptado';
                        return '<span class="status-badge ' + badgeClass + '">' + estado + '</span>';
                    }
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
                                emp_Id: function () { return $("#emp_Id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    "per.per_Identidad": {
                        required: "La identidad es requerida.",
                        maxlength: "Máximo 20 caracteres.",
                        remote: "Ya existe un empleado con esa identidad."
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

// Función global para eliminar empleado
function deleteEmpleado(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}