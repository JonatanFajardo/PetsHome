var Tratamiento = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'trat_Id',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'Mascota',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + (data || 'N/A') + '</div>';
                    }
                },
                {
                    FieldName: 'trat_Medicamento',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || 'N/A';
                    }
                },
                {
                    FieldName: 'TipoMedicamento',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || 'N/A';
                    }
                },
                {
                    FieldName: 'ViaAdministracion',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || 'N/A';
                    }
                },
                {
                    FieldName: 'trat_FechaAplicacion',
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (data) {
                            var date = new Date(data);
                            return date.toLocaleDateString('es-HN');
                        }
                        return 'N/A';
                    }
                },
                {
                    FieldName: 'trat_ProximaDosis',
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (data) {
                            var date = new Date(data);
                            return date.toLocaleDateString('es-HN');
                        }
                        return 'N/A';
                    }
                },
                {
                    FieldName: 'trat_Estado',
                    Visibility: true,
                    Render: function(data, type, row) {
                        var badgeClass = 'status-disponible';
                        var estado = data || 'Activo';

                        switch(estado.toLowerCase()) {
                            case 'completado':
                                badgeClass = 'status-adoptado';
                                break;
                            case 'pendiente':
                                badgeClass = 'status-tratamiento';
                                break;
                            case 'en proceso':
                                badgeClass = 'status-tratamiento';
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

// Función global para eliminar tratamiento
function deleteTratamiento(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
