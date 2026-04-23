var Adopcion = (function () {

    var obj = {},
        statusFilter = "pending",
        statusFilterRegistered = false;

    function registerStatusFilter() {
        if (statusFilterRegistered) {
            return;
        }

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            if (settings.nTable.id !== "datatable") {
                return true;
            }

            var api = new $.fn.dataTable.Api(settings);
            var row = api.row(dataIndex).data();

            if (!row) {
                return true;
            }

            if (statusFilter === "adopted") {
                return row.masc_EsAdoptado;
            }

            return !row.masc_EsAdoptado;
        });

        statusFilterRegistered = true;
    }

    obj.filterByStatus = function (status) {
        statusFilter = status === "adopted" ? "adopted" : "pending";

        var tableInstance = $.fn.dataTable.isDataTable("#datatable")
            ? $("#datatable").DataTable()
            : null;

        if (tableInstance) {
            tableInstance.draw();
        }
    };

    obj.datatable = function (DirectionUrls) {
        $(function () {
            registerStatusFilter();

            var header = [];
            header = [
                { FieldName: 'masc_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'masc_Nombre',
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<div style="display:flex;align-items:center;gap:8px;">'
                                + '<span style="background:#ede9fe;border-radius:6px;padding:4px 7px;">'
                                + '<i class="fas fa-paw" style="color:#7c3aed;font-size:13px;"></i></span>'
                                + '<span style="font-weight:500;color:#111827;">' + data + '</span>'
                                + '</div>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'raza_Descripcion',
                    Size: 130,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'raza_TipoAnimal',
                    Size: 120,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var colors = {
                                'Canino':  { bg: '#dbeafe', color: '#1d4ed8' },
                                'Felino':  { bg: '#ede9fe', color: '#7c3aed' },
                                'Ave':     { bg: '#fef9c3', color: '#a16207' }
                            };
                            var c = colors[data] || { bg: '#f3f4f6', color: '#374151' };
                            return '<span style="background:' + c.bg + ';color:' + c.color + ';padding:3px 10px;border-radius:999px;font-size:12px;font-weight:600;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'masc_Edad',
                    Size: 90,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (data === null || data === undefined || data === '') return '<span style="color:#9ca3af;">—</span>';
                            return data + ' año' + (data == 1 ? '' : 's');
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'masc_Sexo',
                    Size: 90,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var esMacho = data === 'Macho' || data === 'M';
                            var bg = esMacho ? '#dbeafe' : '#fce7f3';
                            var color = esMacho ? '#1d4ed8' : '#9d174d';
                            var label = esMacho ? 'Macho' : 'Hembra';
                            return '<span style="background:' + bg + ';color:' + color + ';padding:3px 10px;border-radius:999px;font-size:12px;font-weight:600;">' + label + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'masc_EsReservado',
                    Size: 120,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            return row.masc_EsAdoptado
                                ? '<span class="status-badge status-adoptado">Adoptado</span>'
                                : '<span class="status-badge status-pendiente">Pendiente</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cantidadSolicitantes',
                    Size: 120,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            var count = data || 0;
                            var colorClass = count > 0 ? 'badge-info' : 'badge-secondary';
                            var title = count === 1 ? '1 solicitante' : (count + ' solicitantes');
                            return '<span class="badge badge-pill ' + colorClass + '" title="' + title + '"><i class="fas fa-users mx-1"></i>' + count + '</span>';
                        }
                        return data;
                    }
                }
            ];
            datatable.init(DirectionUrls, header);
        })
    }

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = [];
            datatable.init(Direction, header);
        })
    }

    return obj;

}());
