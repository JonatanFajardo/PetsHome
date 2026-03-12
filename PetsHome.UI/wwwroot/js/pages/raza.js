var Raza = (function () {
   
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function() {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            //header = [
            //    //["Fila"],
            //    "raza_Id",
            //    "raza_Descripcion"
            //];

            header = [
                { FieldName: "raza_Id", Size: 80, Visibility: false },
                { FieldName: "raza_Descripcion" },
                { FieldName: "raza_TipoAnimal", Size: 150 },
                { FieldName: "raza_Tamano", Size: 120 },
                { FieldName: "raza_TipoPelaje", Size: 150 },
                { FieldName: "raza_EsActivo", Size: 140 }
            ]

            //header = [
            //    ["raza_Id", 80, false],
            //    ["raza_Descripcion", 0, true]
            //];
            datatableCatalogs.init(Direction, header);
        })
    }
    obj.initValidation = function (validarUrl) {
        $.validator.addMethod("noSpaceAtStart", function (value) {
            return value.length === 0 || value[0] !== ' ';
        }, "No puede comenzar con espacios.");

        $("#edit-modal").on("shown.bs.modal", function () {
            var $form = $(this).find("form");
            $form.removeData("validator").removeData("unobtrusiveValidation");
            $form.validate({
                rules: {
                    raza_Descripcion: {
                        required: true,
                        maxlength: 50,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                raza_Descripcion: function () { return $("#Descripcion").val(); },
                                raza_Id: function () { return $("#item-id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    raza_Descripcion: {
                        required: "La descripción es requerida.",
                        maxlength: "Máximo 50 caracteres.",
                        remote: "Ya existe una raza con esa descripción."
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



           