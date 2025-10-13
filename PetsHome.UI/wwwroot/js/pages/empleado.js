var Empleado = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'emp_Id',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'emp_Codigo',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="font-weight: 500;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'emp_Nombres',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + data + '</div>';
                    }
                },
                {
                    FieldName: 'cag_Descripcion',
                    Visibility: true
                },
                {
                    FieldName: 'refg_Nombre',
                    Visibility: true
                },
                {
                    FieldName: 'emp_EsActivo',
                    Visibility: true,
                    Render: function(data, type, row) {
                        var estado = data ? 'Activo' : 'Inactivo';
                        var badgeClass = data ? 'status-disponible' : 'status-adoptado';

                        return '<span class="status-badge ' + badgeClass + '">' + estado + '</span>';
                    }
                }
            ];

            // Inicializar datatable
            datatable.init(Direction, header);
        });
    }

    return obj;

}());

// Función global para eliminar empleado
function deleteEmpleado(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}