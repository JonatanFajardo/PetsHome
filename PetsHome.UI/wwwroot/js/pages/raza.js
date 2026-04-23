var Raza = (function () {
   
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function() {
            var header = new Array();

            header = [
                { FieldName: "raza_Id", Size: 80, Visibility: false },
                {
                    FieldName: "raza_Descripcion",
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fa fa-paw" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "raza_TipoAnimal",
                    Size: 150,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var val = data.toString().trim();
                            var lower = val.toLowerCase();
                            var bg, color;
                            if (lower === 'canino')      { bg = '#dbeafe'; color = '#1d4ed8'; }
                            else if (lower === 'felino') { bg = '#ede9fe'; color = '#7c3aed'; }
                            else if (lower === 'ave')    { bg = '#fef9c3'; color = '#a16207'; }
                            else                         { bg = '#f3f4f6'; color = '#374151'; }
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                                 + 'font-size:12px;font-weight:600;background:' + bg + ';color:' + color + ';">'
                                 + val + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "raza_Tamano",
                    Size: 120,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var val = data.toString().trim();
                            var lower = val.toLowerCase();
                            var bg, color;
                            if (lower === 'pequeño' || lower === 'pequeno') { bg = '#dcfce7'; color = '#15803d'; }
                            else if (lower === 'mediano')                   { bg = '#fef9c3'; color = '#a16207'; }
                            else if (lower === 'grande')                    { bg = '#fee2e2'; color = '#b91c1c'; }
                            else                                            { bg = '#f3f4f6'; color = '#374151'; }
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                                 + 'font-size:12px;font-weight:600;background:' + bg + ';color:' + color + ';">'
                                 + val + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "raza_TipoPelaje",
                    Size: 150,
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var val = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="font-weight:500;color:#111827;">' + val + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: "raza_EsActivo",
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
