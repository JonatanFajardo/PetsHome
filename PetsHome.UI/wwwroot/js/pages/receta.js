var Receta = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'receta_Id',
                    Size: 60,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'masc_Nombre',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + (data || 'Sin mascota') + '</div>';
                    }
                },
                {
                    FieldName: 'receta_Medicamento',
                    Size: 150,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="font-weight: 500;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'TipoMedicamento',
                    Size: 120,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || '<span style="color: #9CA3AF;">Sin tipo</span>';
                    }
                },
                {
                    FieldName: 'receta_Dosis',
                    Size: 90,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || '<span style="color: #9CA3AF;">N/A</span>';
                    }
                },
                {
                    FieldName: 'receta_Frecuencia',
                    Size: 100,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || '<span style="color: #9CA3AF;">N/A</span>';
                    }
                },
                {
                    FieldName: 'receta_Duracion',
                    Size: 100,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || '<span style="color: #9CA3AF;">N/A</span>';
                    }
                },
                {
                    FieldName: 'receta_Estado',
                    Size: 110,
                    Visibility: true,
                    Render: function(data, type, row) {
                        var estado = data || 'Activa';
                        var badgeClass = '';

                        switch(estado) {
                            case 'Activa':
                                badgeClass = 'status-disponible';
                                break;
                            case 'Completada':
                                badgeClass = 'status-adoptado';
                                break;
                            case 'Cancelada':
                                badgeClass = 'status-inactivo';
                                break;
                            default:
                                badgeClass = 'status-disponible';
                        }

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

// Función global para eliminar receta
function deleteReceta(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
