var Vacuna = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                { FieldName: "vac_Id", Size: 60, Visibility: false },

                {
                    FieldName: "vac_Descripcion",
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return '<span style="font-weight:500;color:#111827;">' + (data ?? '—') + '</span>';
                        }
                        return data;
                    }
                },

                {
                    FieldName: "vacu_Especie",
                    Size: 120,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var estilos = {
                                'Canino':  { bg: '#dbeafe', color: '#1d4ed8' },
                                'Felino':  { bg: '#ede9fe', color: '#7c3aed' },
                                'Ave':     { bg: '#fef9c3', color: '#a16207' },
                                'Reptil':  { bg: '#dcfce7', color: '#15803d' },
                                'Roedor':  { bg: '#fce7f3', color: '#be185d' }
                            };
                            var s = estilos[data] || { bg: '#f3f4f6', color: '#374151' };
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;font-size:12px;font-weight:600;background:' + s.bg + ';color:' + s.color + ';">' + data + '</span>';
                        }
                        return data;
                    }
                },

                {
                    FieldName: "vacu_DosisRecomendada",
                    Size: 130,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },

                {
                    FieldName: "vacu_PeriodoRefuerzo",
                    Size: 130,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (data === null || data === undefined || data === '') {
                                return '<span style="color:#9ca3af;">—</span>';
                            }
                            var num = parseInt(data, 10);
                            if (!isNaN(num)) {
                                var label = num === 1 ? 'día' : 'días';
                                return '<span style="color:#374151;"><strong style="color:#111827;">' + num + '</strong> ' + label + '</span>';
                            }
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },

                {
                    FieldName: "esActivo",
                    Size: 140,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            var activo = data === true || data === 1 || data === "Activo" || data === "true";
                            if (activo) {
                                return '<span class="status-badge status-activo">Activo</span>';
                            } else {
                                return '<span class="status-badge status-inactivo">Inactivo</span>';
                            }
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
