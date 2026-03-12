var CitaMedica = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'cita_Id',
                    Size: 60,
                    Visibility: false,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'masc_Nombre',
                    Size: 150,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name"><i class="fas fa-paw text-primary mr-2"></i>' + data + '</div>';
                    }
                },
                {
                    FieldName: 'cita_Diagnostico',
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (!data) return '<span class="text-muted">Sin diagnóstico</span>';
                        var diagnosticoCorto = data.length > 50 ? data.substring(0, 50) + '...' : data;
                        return '<span title="' + data + '">' + diagnosticoCorto + '</span>';
                    }
                },
                {
                    FieldName: 'cita_TipoConsulta',
                    Size: 120,
                    Visibility: true,
                    Render: function(data, type, row) {
                        var badgeClass = 'badge-primary';
                        if (data === 'Emergencia') badgeClass = 'badge-danger';
                        else if (data === 'Vacunación') badgeClass = 'badge-success';
                        else if (data === 'Cirugía') badgeClass = 'badge-warning';
                        else if (data === 'Control') badgeClass = 'badge-info';

                        return '<span class="badge ' + badgeClass + '">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'cita_Peso',
                    Size: 80,
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (!data) return '<span class="text-muted">N/A</span>';
                        return '<span style="font-weight: 500;">' + data + ' kg</span>';
                    }
                },
                {
                    FieldName: 'cita_Temperatura',
                    Size: 90,
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (!data) return '<span class="text-muted">N/A</span>';
                        var temp = parseFloat(data);
                        var colorClass = 'text-success';
                        if (temp < 38) colorClass = 'text-info';
                        else if (temp > 39.5) colorClass = 'text-danger';

                        return '<span class="' + colorClass + '" style="font-weight: 500;">' + temp + '°C</span>';
                    }
                },
                {
                    FieldName: 'cita_ProximaCita',
                    Size: 120,
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (!data) return '<span class="text-muted">Sin programar</span>';

                        var fechaCita = new Date(data);
                        var fechaFormateada = fechaCita.toLocaleDateString('es-ES', {
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric'
                        });

                        var hoy = new Date();
                        var diasRestantes = Math.ceil((fechaCita - hoy) / (1000 * 60 * 60 * 24));
                        var colorClass = 'text-primary';

                        if (diasRestantes < 0) colorClass = 'text-danger';
                        else if (diasRestantes <= 7) colorClass = 'text-warning';

                        return '<span class="' + colorClass + '">' + fechaFormateada + '</span>';
                    }
                }
            ];

            // Inicializar datatable
            datatable.init(Direction, header);
        });
    }

    return obj;

}());

// Función global para eliminar cita médica
function deleteCitaMedica(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}