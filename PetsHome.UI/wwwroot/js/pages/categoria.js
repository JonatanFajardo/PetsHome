
var Categoria = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'cat_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'cat_Descripcion',
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-weight:500;color:#111827;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cat_EsActivo',
                    Size: 140,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            var activo = data === true || data === 1 || data === 'Activo' || data === 'true';
                            var label = activo ? 'Activo' : 'Inactivo';
                            var cls = activo ? 'status-activo' : 'status-inactivo';
                            return '<span class="status-badge ' + cls + '">' + label + '</span>';
                        }
                        return data;
                    }
                }
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
                    cat_Descripcion: {
                        required: true,
                        maxlength: 100,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                cat_Descripcion: function () { return $("#Descripcion").val(); },
                                cat_Id: function () { return $("#item-id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    cat_Descripcion: {
                        required: "La descripción es requerida.",
                        maxlength: "Máximo 100 caracteres.",
                        remote: "Ya existe una categoría con esa descripción."
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
