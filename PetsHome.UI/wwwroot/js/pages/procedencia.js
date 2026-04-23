var Procedencia = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();

            header = [
                { FieldName: 'proc_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'proc_Descripcion',
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-map-marker-alt" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'proc_EsActivo',
                    Size: 140,
                    render: function (data, type) {
                        if (type === 'display') {
                            var isActive = data === true || data === 1 || data === "Activo" || data === "true";
                            var cssClass = isActive ? 'status-badge status-activo' : 'status-badge status-inactivo';
                            var label    = isActive ? 'Activo' : 'Inactivo';
                            return '<span class="' + cssClass + '">' + label + '</span>';
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
                    proc_Descripcion: {
                        required: true,
                        maxlength: 100,
                        noSpaceAtStart: true,
                        remote: {
                            url: validarUrl,
                            type: "GET",
                            data: {
                                proc_Descripcion: function () { return $("#Descripcion").val(); },
                                proc_Id: function () { return $("#item-id").val(); }
                            }
                        }
                    }
                },
                messages: {
                    proc_Descripcion: {
                        required: "La descripción es requerida.",
                        maxlength: "Máximo 100 caracteres.",
                        remote: "Ya existe una procedencia con esa descripción."
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
