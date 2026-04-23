var CitaMedica = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'cita_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'masc_Nombre',
                    Size: 150,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<div style="display:flex;align-items:center;gap:8px;">'
                                + '<span style="background:#ede9fe;border-radius:6px;padding:4px 7px;">'
                                + '<i class="fas fa-paw" style="color:#7c3aed;font-size:13px;"></i></span>'
                                + '<span style="font-weight:500;color:#111827;">' + data + '</span>'
                                + '</div>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cita_Diagnostico',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var corto = data.length > 50 ? data.substring(0, 50) + '…' : data;
                            return '<span title="' + data + '">' + corto + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cita_TipoConsulta',
                    Size: 120,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var badgeClass = 'badge-primary';
                            if (data === 'Emergencia')  badgeClass = 'badge-danger';
                            else if (data === 'Vacunación') badgeClass = 'badge-success';
                            else if (data === 'Cirugía')    badgeClass = 'badge-warning';
                            else if (data === 'Control')    badgeClass = 'badge-info';
                            return '<span class="badge ' + badgeClass + '">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cita_Peso',
                    Size: 80,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="font-weight:500;">' + data + ' kg</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cita_Temperatura',
                    Size: 90,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var temp = parseFloat(data);
                            var colorClass = 'text-success';
                            if (temp < 38) colorClass = 'text-info';
                            else if (temp > 39.5) colorClass = 'text-danger';
                            return '<span class="' + colorClass + '" style="font-weight:500;">' + temp + '°C</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'cita_ProximaCita',
                    Size: 120,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var fechaCita = new Date(data);
                            if (isNaN(fechaCita)) return '<span style="color:#9ca3af;">—</span>';
                            var fechaFormateada = fechaCita.toLocaleDateString('es-HN', { day: '2-digit', month: '2-digit', year: 'numeric' });
                            var diasRestantes = Math.ceil((fechaCita - new Date()) / (1000 * 60 * 60 * 24));
                            var colorClass = 'text-primary';
                            if (diasRestantes < 0) colorClass = 'text-danger';
                            else if (diasRestantes <= 7) colorClass = 'text-warning';
                            return '<span class="' + colorClass + '">' + fechaFormateada + '</span>';
                        }
                        return data;
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    }

    return obj;

}());

function deleteCitaMedica(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
