var Donacion = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            // Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                    {
                        FieldName: 'dona_Id',
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
                        FieldName: 'dona_TipoDonacion',
                        DisplayName: '🎁 Tipo',
                        Width: '120px',
                        Align: 'center',
                        Visibility: true,
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">N/A</span>';

                            let icon = 'fas fa-gift';
                            let colorClass = 'bg-primary';

                            switch (data.toLowerCase()) {
                                case 'monetaria':
                                    icon = 'fas fa-dollar-sign';
                                    colorClass = 'bg-success';
                                    break;
                                case 'artículos':
                                case 'articulos':
                                    icon = 'fas fa-boxes';
                                    colorClass = 'bg-info';
                                    break;
                                case 'mixta':
                                    icon = 'fas fa-handshake';
                                    colorClass = 'bg-warning';
                                    break;
                                case 'servicios':
                                    icon = 'fas fa-tools';
                                    colorClass = 'bg-secondary';
                                    break;
                            }

                            return `<div class="tipo-cell">
                        <span class="badge ${colorClass} bg-gradient px-3 py-2">
                            <i class="${icon} me-1"></i>
                            ${data}
                        </span>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'dona_NombreDonante',
                        DisplayName: '👤 Donante',
                        Width: '180px',
                        Align: 'left',
                        Visibility: true,
                        Sortable: true,
                        Searchable: true,
                        Render: function (data) {
                            return `<div class="donante-cell">
                        <i class="fas fa-user text-primary me-2"></i>
                        <span class="fw-bold text-dark">${data || 'Sin nombre'}</span>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'dona_MontoMonetario',
                        DisplayName: '💰 Monto',
                        Width: '120px',
                        Align: 'right',
                        Visibility: true,
                        Sortable: true,
                        Render: function (data, type, row) {
                            if (!data || data == 0) {
                                // Si es donación de artículos, mostrar valor estimado
                                if (row.dona_ValorEstimado && row.dona_ValorEstimado > 0) {
                                    return `<div class="monto-cell">
                                <i class="fas fa-calculator text-info me-1"></i>
                                <span class="fw-bold text-info">$${parseFloat(row.dona_ValorEstimado).toLocaleString('es-ES', { minimumFractionDigits: 2 })}</span>
                                <br><small class="text-muted">Estimado</small>
                            </div>`;
                                }
                                return '<span class="text-muted">$0.00</span>';
                            }
                            
                            return `<div class="monto-cell">
                        <i class="fas fa-dollar-sign text-success me-1"></i>
                        <span class="fw-bold text-success">$${parseFloat(data).toLocaleString('es-ES', { minimumFractionDigits: 2 })}</span>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'dona_FechaDonacion',
                        DisplayName: '📅 Fecha',
                        Width: '120px',
                        Align: 'center',
                        Visibility: true,
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">N/A</span>';
                            
                            const fecha = new Date(data);
                            const fechaFormateada = fecha.toLocaleDateString('es-ES', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric'
                            });
                            
                            return `<div class="fecha-cell">
                        <i class="fas fa-calendar text-info me-1"></i>
                        <span class="text-dark">${fechaFormateada}</span>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'dona_Estado',
                        DisplayName: '🏷️ Estado',
                        Width: '130px',
                        Align: 'center',
                        Visibility: true,
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">N/A</span>';

                            let icon = 'fas fa-flag';
                            let colorClass = 'bg-secondary';

                            switch (data.toLowerCase()) {
                                case 'recibida':
                                    icon = 'fas fa-check';
                                    colorClass = 'bg-success';
                                    break;
                                case 'en proceso':
                                    icon = 'fas fa-clock';
                                    colorClass = 'bg-warning';
                                    break;
                                case 'procesada':
                                    icon = 'fas fa-check-circle';
                                    colorClass = 'bg-info';
                                    break;
                                case 'rechazada':
                                    icon = 'fas fa-times';
                                    colorClass = 'bg-danger';
                                    break;
                            }

                            return `<div class="estado-cell">
                        <span class="badge ${colorClass} bg-gradient px-3 py-2">
                            <i class="${icon} me-1"></i>
                            ${data}
                        </span>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'refg_Nombre',
                        DisplayName: '🏠 Refugio',
                        Width: '150px',
                        Align: 'left',
                        Visibility: true,
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">N/A</span>';
                            
                            return `<div class="refugio-cell">
                        <i class="fas fa-home text-warning me-1"></i>
                        <span class="text-dark">${data}</span>
                    </div>`;
                        }
                    }
            ];
            datatable.init(Direction, header);
        });
    }

    return obj;

}());