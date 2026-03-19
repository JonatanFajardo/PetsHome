var EmpleadosCargo = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            header = [
                { FieldName: 'cag_Id', Size: 60, Visibility: false},
                { FieldName: 'cag_Descripcion' },
                { FieldName: 'cag_Salario', Size: 100 },
                { FieldName: 'esActivo', Size: 100 }
            ];

            // Usar datatableCatalogs para catálogos simples
            datatableCatalogs.init(Direction, header);
        });
    }

    obj.initValidation = function (validarUrl) {
        if (!$.validator.methods.noSpaceAtStart) {
            $.validator.addMethod("noSpaceAtStart", function (value) {
                return value.length === 0 || value[0] !== ' ';
            }, "No puede comenzar con espacios.");
        }
        $("#edit-modal").one("shown.bs.modal", function () {
            var $form = $(this).find("form");
            $form.removeData("validator").removeData("unobtrusiveValidation");
            $form.validate({
                rules: {
                    cag_Descripcion: {
                        required: true,
                        maxlength: 100,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                cag_Descripcion: function () { return $("#Descripcion").val(); },
                                cag_Id: function () { return $("#item-id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    cag_Descripcion: {
                        required: "La descripción es requerida.",
                        maxlength: "Máximo 100 caracteres.",
                        remote: "Ya existe un cargo con esa descripción."
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
