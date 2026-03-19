var Refugio = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "refg_Id", Size: 60, Visibility: false},
                { FieldName: "refg_Nombre" },
                { FieldName: "refg_RTN", Size: 130 },
                { FieldName: "refg_Ubicacion", Size: 200 },
                { FieldName: "esActivo", Size: 140 }
            ];
            datatable.init(Direction, header);
        })
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
                    refg_Nombre: {
                        required: true,
                        maxlength: 100,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                refg_Nombre: function () { return $("#refg_Nombre").val(); },
                                refg_Id: function () { return $("#refg_Id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    refg_Nombre: {
                        required: "El nombre es requerido.",
                        maxlength: "Máximo 100 caracteres.",
                        remote: "Ya existe un refugio con ese nombre."
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