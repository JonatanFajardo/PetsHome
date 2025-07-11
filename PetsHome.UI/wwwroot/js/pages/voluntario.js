var Voluntario = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'vol_Id',
                    DisplayName: '#️⃣ ID',
                    Width: '80px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        return `<div class="id-cell">
                                    <span class="badge bg-secondary rounded-pill">#${data}</span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'vol_HorasTrabajadas',
                    DisplayName: '⏰ Horas Trabajadas',
                    Width: '160px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (!data || data === 0) {
                            return `<div class="hours-cell">
                                        <span class="text-muted fst-italic">Sin horas</span>
                                    </div>`;
                        }

                        const horas = parseInt(data);
                        let colorClass = 'bg-secondary';
                        let textClass = 'text-white';

                        if (horas >= 100) {
                            colorClass = 'bg-success';
                        } else if (horas >= 50) {
                            colorClass = 'bg-primary';
                        } else if (horas >= 20) {
                            colorClass = 'bg-warning';
                            textClass = 'text-dark';
                        }

                        return `<div class="hours-cell">
                                    <span class="badge ${colorClass} bg-gradient px-3 py-2 ${textClass}">
                                        <i class="fas fa-clock me-1"></i>
                                        ${horas}h
                                    </span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'vol_Nombres',
                    DisplayName: '👤 Nombres',
                    Width: '250px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        return `<div class="names-cell">
                                    <i class="fas fa-user text-primary me-2"></i>
                                    <span class="fw-bold text-dark" style="font-size: 15px;">${data || 'Sin nombre'}</span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'per_Identidad',
                    DisplayName: '🆔 Identidad',
                    Width: '180px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (!data) {
                            return `<div class="identity-cell">
                                        <span class="text-muted fst-italic">No especificado</span>
                                    </div>`;
                        }

                        // Formatear el número de identidad (asumiendo formato hondureño)
                        const identidad = data.toString().replace(/\D/g, ''); // Solo números
                        let identidadFormateada = identidad;

                        if (identidad.length === 13) {
                            identidadFormateada = identidad.replace(/(\d{4})(\d{4})(\d{5})/, '$1-$2-$3');
                        }

                        return `<div class="identity-cell">
                                    <span class="badge bg-info bg-gradient px-3 py-2">
                                        <i class="fas fa-id-card me-1"></i>
                                        ${identidadFormateada}
                                    </span>
                                </div>`;
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());