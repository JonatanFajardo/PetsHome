var ReportesAbandono = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'repa_Id',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'repa_FechaReporte',
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (data) {
                            var date = new Date(data);
                            return '<span style="font-weight: 500;">' + date.toLocaleDateString('es-HN') + '</span>';
                        }
                        return '';
                    }
                },
                {
                    FieldName: 'repa_UbicacionIncidente',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + data + '</div>';
                    }
                },
                {
                    FieldName: 'TipoReportante',
                    Visibility: true
                },
                {
                    FieldName: 'NombreRefugio',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || '<span class="text-muted">Sin asignar</span>';
                    }
                },
                {
                    FieldName: 'repa_EstadoAtencion',
                    Visibility: true,
                    Render: function(data, type, row) {
                        var badgeClass = '';
                        switch(data) {
                            case 'Pendiente':
                                badgeClass = 'status-pendiente';
                                break;
                            case 'En Proceso':
                                badgeClass = 'status-en-proceso';
                                break;
                            case 'Atendido':
                                badgeClass = 'status-disponible';
                                break;
                            case 'Rechazado':
                                badgeClass = 'status-adoptado';
                                break;
                            default:
                                badgeClass = 'status-pendiente';
                        }
                        return '<span class="status-badge ' + badgeClass + '">' + data + '</span>';
                    }
                }
            ];

            // Inicializar datatable
            datatable.init(Direction, header);
        });
    }

    return obj;

}());

// Función global para ver detalle
function viewDetail(id) {
    window.location.href = '/ReportesAbandono/Detail/' + id;
}

// Función global para eliminar reporte
function deleteReporte(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
