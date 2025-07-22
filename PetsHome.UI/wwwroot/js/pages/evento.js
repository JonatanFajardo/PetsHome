var Evento = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'eve_Id',
                    DisplayName: '#️⃣ ID',
                    Width: '80px',
                    Align: 'center',
                    Visibility: false,
                    Sortable: true,
                    Render: function (data) {
                        return `<div class="id-cell">
                                    <span class="badge bg-secondary rounded-pill">#${data}</span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'eve_Descripcion',
                    DisplayName: '📝 Descripción',
                    Width: '300px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        return `<div class="description-cell">
                                    <i class="fas fa-file-alt text-primary me-2"></i>
                                    <span class="fw-bold text-dark" style="font-size: 15px;">${data || 'Sin descripción'}</span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'refg_Id',
                    DisplayName: '🏢 ID Refugio',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        return `<div class="refuge-id-cell">
                                    <span class="badge bg-info bg-gradient px-3 py-2">
                                        <i class="fas fa-building me-1"></i>
                                        #${data}
                                    </span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'refg_Nombre',
                    DisplayName: '🏠 Refugio',
                    Width: '200px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        return `<div class="refuge-name-cell">
                                    <i class="fas fa-home text-success me-2"></i>
                                    <span class="fw-medium text-dark">${data || 'Sin refugio'}</span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'eve_Fecha',
                    DisplayName: '📅 Fecha',
                    Width: '150px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (!data) {
                            return `<div class="date-cell">
                                        <span class="text-white fst-italic">Sin fecha</span>
                                    </div>`;
                        }

                        // Formatear la fecha si es necesario
                        const fecha = new Date(data);
                        const fechaFormateada = fecha.toLocaleDateString('es-ES', {
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric'
                        });

                        return `<div class="date-cell">
                                    <span class="badge bg-warning bg-gradient px-3 py-2 text-white">
                                        <i class="fas fa-calendar me-1"></i>
                                        ${fechaFormateada}
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