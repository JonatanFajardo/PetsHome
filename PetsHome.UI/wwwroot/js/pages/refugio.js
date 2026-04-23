var Refugio = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: "refg_Id", Size: 60, Visibility: false },
                {
                    FieldName: "refg_Nombre",
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-home" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "refg_RTN",
                    Size: 130,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var digits = data.toString().replace(/\D/g, '');
                            var formatted = digits.length === 14
                                ? digits.slice(0,4) + '-' + digits.slice(4,8) + '-' + digits.slice(8)
                                : data;
                            return '<span style="font-family:monospace;letter-spacing:0.5px;">' + formatted + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "refg_Ubicacion",
                    Size: 200,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "esActivo",
                    Size: 140,
                    render: function (data, type) {
                        if (type === 'display') {
                            var isActive = data === true || data === 1 || data === "Activo" || data === "true";
                            return '<span class="status-badge ' + (isActive ? 'status-activo' : 'status-inactivo') + '">'
                                 + (isActive ? 'Activo' : 'Inactivo') + '</span>';
                        }
                        return data;
                    }
                }
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
