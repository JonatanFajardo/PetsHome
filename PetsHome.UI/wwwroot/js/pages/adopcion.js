var Adopcion = (function () {

    var obj = {};

    // Inicializa DataTable con columnas de la lista de adopciones
    obj.datatable = function (DirectionUrls) {
        $(function () {
            var header = [];
            header = [
                { FieldName: 'masc_Id', Size: 80 },
                { FieldName: 'masc_Nombre' },
                { FieldName: 'raza_Descripcion' },
                { FieldName: 'raza_TipoAnimal' },
                { FieldName: 'masc_Edad', Render: function (data, type, row) { return data + ' años'; } },
                { FieldName: 'masc_Sexo' },
                { FieldName: 'masc_EsReservado', Render: function (data, type, row) {
                    if (row.masc_EsAdoptado) return '<span class="status-badge status-adoptado">Adoptado</span>';
                    return data ? '<span class="status-badge status-pendiente">Reservado</span>' : '<span class="status-badge status-disponible">Disponible</span>';
                } },
                { FieldName: 'cantidadSolicitantes', Render: function (data, type, row) {
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
