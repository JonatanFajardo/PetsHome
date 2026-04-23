
var Item = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'itm_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'itm_Codigo',
                    Size: 100,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-family:monospace;font-weight:600;letter-spacing:0.5px;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'itm_Descripcion',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<div style="display:flex;align-items:center;gap:8px;">'
                                + '<span style="background:#ede9fe;border-radius:6px;padding:4px 7px;">'
                                + '<i class="fas fa-box" style="color:#7c3aed;font-size:13px;"></i></span>'
                                + '<span style="font-weight:500;color:#111827;">' + data + '</span>'
                                + '</div>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cat_Descripcion',
                    Size: 140,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'itm_Precio',
                    Size: 100,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (data === null || data === undefined || data === '') return '<span style="color:#9ca3af;">—</span>';
                            var val = parseFloat(data);
                            if (isNaN(val)) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-weight:500;color:#111827;">$' + val.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</span>';
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
