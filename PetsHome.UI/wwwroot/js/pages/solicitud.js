var Solicitud = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            var header = [
                {
                    FieldName: "sol_Id",
                    DisplayName: "🆔 ID Solicitud",
                    Width: "120px",
                    Align: "center",
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        if (!data) return '<span class="text-muted">N/A</span>';
                        return `<div class="id-cell">
                        <i class="fas fa-hashtag text-primary me-2"></i>
                        <span class="badge bg-primary-subtle text-primary fw-bold">#${data}</span>
                    </div>`;
                    }
                },
                {
                    FieldName: "sol_Identidad",
                    DisplayName: "📄 Identidad",
                    Width: "140px",
                    Align: "center",
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        if (!data) return '<span class="text-muted">Sin identidad</span>';
                        // Formatear identidad con guiones si tiene 13 dígitos
                        const identidadFormateada = data.length === 13 ?
                            data.replace(/(\d{4})(\d{4})(\d{5})/, '$1-$2-$3') : data;
                        return `<div class="identity-cell">
                        <i class="fas fa-id-card text-info me-2"></i>
                        <span class="fw-medium text-dark">${identidadFormateada}</span>
                    </div>`;
                    }
                },
                {
                    FieldName: "sol_Nombres",
                    DisplayName: "👤 Nombre del Solicitante",
                    Width: "200px",
                    Align: "left",
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        if (!data) return '<span class="text-muted fst-italic">Sin nombre</span>';
                        return `<div class="applicant-name-cell">
                        <i class="fas fa-user text-success me-2"></i>
                        <strong class="text-dark">${data}</strong>
                    </div>`;
                    }
                },
                {
                    FieldName: "masc_Nombre",
                    DisplayName: "🐾 Nombre de la Mascota",
                    Width: "180px",
                    Align: "left",
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        if (!data) return '<span class="text-muted fst-italic">Sin mascota</span>';
                        return `<div class="pet-name-cell">
                        <i class="fas fa-paw text-warning me-2"></i>
                        <strong class="text-primary">${data}</strong>
                    </div>`;
                    }
                },
                {
                    FieldName: "sol_Correo",
                    DisplayName: "📧 Correo Electrónico",
                    Width: "220px",
                    Align: "left",
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        if (!data) return '<span class="text-muted">Sin correo</span>';
                        // Validar formato de email básico
                        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                        const isValid = emailRegex.test(data);
                        const colorClass = isValid ? 'text-primary' : 'text-danger';
                        const icon = isValid ? 'fas fa-envelope' : 'fas fa-envelope-open-text';

                        return `<div class="email-cell">
                        <i class="${icon} ${colorClass} me-2"></i>
                        <span class="fw-medium ${colorClass}">${data}</span>
                        ${!isValid ? '<br><small class="text-danger">⚠️ Formato inválido</small>' : ''}
                    </div>`;
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());