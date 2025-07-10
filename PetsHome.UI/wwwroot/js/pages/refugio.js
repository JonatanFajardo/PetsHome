var Refugio = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'refg_Id',
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
                    FieldName: 'refg_Nombre',
                    DisplayName: '🏢 Nombre',
                    Width: '150px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        return `<div class="name-cell">
                                <i class="fas fa-building text-primary me-2"></i>
                                <span class="fw-bold text-dark" style="font-size: 15px;">${data || 'Sin nombre'}</span>
                            </div>`;
                    }
                },
                {
                    FieldName: 'refg_RTN',
                    DisplayName: '📋 RTN',
                    Width: '150px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (!data) {
                            return `<div class="rtn-cell">
                                            <span class="text-muted fst-italic">No especificado</span>
                                        </div>`;
                                                }
                                                return `<div class="rtn-cell">
                                        <span class="badge bg-info bg-gradient px-3 py-2">
                                            <i class="fas fa-id-card me-1"></i>
                                            ${data}
                                        </span>
                                    </div>`;
                    }
                },
                {
                    FieldName: 'refg_Ubicacion',
                    DisplayName: '📍 Ubicación',
                    Width: '200px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        if (!data) {
                            return `<div class="location-cell">
                                            <span class="text-muted fst-italic">No especificado</span>
                                        </div>`;
                                                }
                                                return `<div class="location-cell">
                                        <i class="fas fa-map-marker-alt text-danger me-2"></i>
                                        <span class="fw-medium text-dark">${data}</span>
                                    </div>`;
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());