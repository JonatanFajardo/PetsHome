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

    // Inicializa DataTable con columnas de la lista de adopciones
    obj.datatable = function (DirectionUrls) {
        $(function () {
            registerStatusFilter();

            var header = [];
            header = [
                { FieldName: 'masc_Id', Size: 60 },
                { FieldName: 'masc_Nombre' },
                { FieldName: 'raza_Descripcion', Size: 130 },
                { FieldName: 'raza_TipoAnimal', Size: 120 },
                { FieldName: 'masc_Edad', Size: 90, Render: function (data, type, row) { return data + ' años'; } },
                { FieldName: 'masc_Sexo', Size: 90 },
                { FieldName: 'masc_EsReservado', Size: 120, Render: function (data, type, row) {
                    return row.masc_EsAdoptado
                        ? '<span class="status-badge status-adoptado">Adoptado</span>'
                        : '<span class="status-badge status-pendiente">Pendiente</span>';
                } },
                { FieldName: 'cantidadSolicitantes', Size: 120, Render: function (data, type, row) {
                    var count = data || 0;
                    var colorClass = count > 0 ? 'badge-info' : 'badge-secondary';
                    var title = count === 1 ? '1 solicitante' : (count + ' solicitantes');
                    return `<span class="badge badge-pill  ${colorClass}" title="${title}"><i class="fas fa-users mx-1"></i>${count}</span>`;
                } }
            ];
            datatable.init(DirectionUrls, header);
        })
    }

    // Helper (compatibilidad si fuese llamado)
    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = [];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
