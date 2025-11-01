var Ingreso = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'ingr_Id',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'ingr_FechaIngreso',
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
                    FieldName: 'ingr_LugarRescate',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + data + '</div>';
                    }
                },
                {
                    FieldName: 'ingr_PersonaRescatista',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return data || '<span class="text-muted">Sin especificar</span>';
                    }
                },
                {
                    FieldName: 'refg_Nombre',
                    Visibility: true
                },
                {
                    FieldName: 'ingr_EsEmergencia',
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (data) {
                            return '<span class="status-badge status-urgente">EMERGENCIA</span>';
                        } else {
                            return '<span class="status-badge status-disponible">Normal</span>';
                        }
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
    window.location.href = '/Ingreso/Detail/' + id;
}

// Función global para eliminar ingreso
function deleteIngreso(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
