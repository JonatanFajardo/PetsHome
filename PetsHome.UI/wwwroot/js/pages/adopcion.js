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
                { FieldName: 'CantidadSolicitantes' }
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

