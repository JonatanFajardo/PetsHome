
var Item = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'itm_Id', Size: 60, Visibility: false },
                { FieldName: 'itm_Codigo', Size: 100, Visibility: true },
                { FieldName: 'itm_Descripcion', Visibility: true },
                { FieldName: 'cat_Descripcion', Size: 140, Visibility: true },
                { FieldName: 'itm_Precio', Size: 100, Visibility: true }
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
                    itm_Codigo: {
                        required: true,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                itm_Codigo: function () { return $("#itm_Codigo").val(); },
                                itm_Id: function () { return $("#itm_Id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    itm_Codigo: {
                        required: "El código es requerido.",
                        remote: "Ya existe un item con ese código."
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