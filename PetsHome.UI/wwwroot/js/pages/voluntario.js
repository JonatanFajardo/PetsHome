var Voluntario = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'vol_Id',
                    Size: 60,
                    Visibility: false,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'vol_HorasTrabajadas',
                    Size: 100,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="font-weight: 500;">' + data + ' hrs</span>';
                    }
                },
                {
                    FieldName: 'vol_Nombres',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + data + '</div>';
                    }
                },
                {
                    FieldName: 'per_Identidad',
                    Size: 150,
                    Visibility: true
                }
            ];

            // Inicializar datatable
            datatable.init(Direction, header);
        });
    }

    return obj;

}());

// Función global para eliminar voluntario
function deleteVoluntario(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}