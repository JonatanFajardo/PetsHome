var Vacuna = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "vac_Id", Size: 60, Visibility: false},
                { FieldName: "vac_Descripcion" },
                { FieldName: "vacu_Especie", Size: 120 },
                { FieldName: "vacu_DosisRecomendada", Size: 130 },
                { FieldName: "vacu_PeriodoRefuerzo", Size: 130 },
                { FieldName: "esActivo", Size: 140 }
            ];
            datatableCatalogs.init(Direction, header);
        })
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
                    vac_Descripcion: {
                        required: true,
                        maxlength: 100,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                vac_Descripcion: function () { return $("#Descripcion").val(); },
                                vac_Id: function () { return $("#item-id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    vac_Descripcion: {
                        required: "La descripción es requerida.",
                        maxlength: "Máximo 100 caracteres.",
                        remote: "Ya existe una vacuna con esa descripción."
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