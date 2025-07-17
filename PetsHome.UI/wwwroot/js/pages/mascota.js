var Mascota = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            // Nombre | Tamaño/AutoWidth | Visibilidad
            console.log(Direction.listUrl);
            header = [
                    {
                        FieldName: 'masc_Id',
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
                        FieldName: 'masc_Fila',
                        DisplayName: '📋 Código',
                        Width: '120px',
                        Align: 'center',
                        Visibility: true,
                        Sortable: true,
                        ExportVisible: false,
                        Render: function (data) {
                            return `<div class="code-cell">
                        <i class="fas fa-barcode text-info me-2"></i>
                        <span class="fw-bold text-dark">${data || 'N/A'}</span>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'masc_Imagen',
                        DisplayName: '📸 Imagen',
                        Width: '100px',
                        Align: 'center',
                        Visibility: true,
                        Sortable: false,
                        ExportVisible: false,
                        Render: function (data) {
                            if (!data) {
                                return `<div class="image-cell">
                            <div class="placeholder-image">
                                <i class="fas fa-paw text-muted" style="font-size: 24px;"></i>
                            </div>
                        </div>`;
                            }
                            return `<div class="image-cell">
                        <img src="${data}" 
                             alt="Mascota" 
                             class="pet-image rounded-circle"
                             style="width: 50px; height: 50px; object-fit: cover; border: 2px solid #e9ecef;"
                             onerror="this.style.display='none'; this.nextElementSibling.style.display='flex';">
                        <div class="placeholder-image" style="display: none;">
                            <i class="fas fa-paw text-muted" style="font-size: 24px;"></i>
                        </div>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'masc_Nombre',
                        DisplayName: '🐾 Nombre',
                        Width: '180px',
                        Align: 'left',
                        Visibility: true,
                        Sortable: true,
                        Searchable: true,
                        Render: function (data) {
                            return `<div class="pet-name-cell">
                        <i class="fas fa-heart text-danger me-2"></i>
                        <span class="fw-bold text-primary" style="font-size: 16px;">${data || 'Sin nombre'}</span>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'raza_Descripcion',
                        DisplayName: '🏷️ Raza',
                        Align: 'center',
                        Visibility: true,
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted fst-italic">No especificado</span>';

                            
                            return `<div class="breed-cell">
                        <span class="badge bg-primary bg-gradient px-3 py-2">
                            <i class="fas fa-dog me-1"></i>
                            ${data}
                        </span>
                    </div>`;
                        }
                    },
                    {
                        FieldName: 'masc_Edad',
                        DisplayName: '⏰ Edad',
                        Width: '200px',
                        Align: 'center',
                        Visibility: true,
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">N/A</span>';

                            const edad = parseInt(data);
                            let icon = 'fas fa-birthday-cake';
                            let colorClass = 'text-primary';
                            let stage = '';

                            if (edad < 1) {
                                icon = 'fas fa-baby';
                                colorClass = 'text-success';
                                stage = 'Cachorro';
                            } else if (edad < 3) {
                                icon = 'fas fa-child';
                                colorClass = 'text-info';
                                stage = 'Joven';
                            } else if (edad < 7) {
                                icon = 'fas fa-user';
                                colorClass = 'text-warning';
                                stage = 'Adulto';
                            } else {
                                icon = 'fas fa-user-clock';
                                colorClass = 'text-secondary';
                                stage = 'Senior';
                            }

                            return `<div class="age-cell">
                        <i class="${icon} ${colorClass} me-1"></i>
                        <span class="fw-bold ${colorClass}">${edad} años</span>
                        <br>
                        <small class="text-muted">${stage}</small>
                    </div>`;
                        }
                    },
                {
                    FieldName: 'masc_Sexo',
                    DisplayName: '♂️♀️ Sexo',
                    Width: '100px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (!data) return '<span class="text-muted">N/A</span>';

                        const sexo = data.toLowerCase();
                        let icon = 'fas fa-question';
                        let colorClass = 'text-secondary';
                        let bgColor = 'bg-secondary';
                        let displayText = data;

                        if (sexo === 'macho' || sexo === 'm' || sexo === 'male') {
                            icon = 'fas fa-mars';
                            bgColor = 'bg-secondary';
                            displayText = 'Macho';
                        } else if (sexo === 'hembra' || sexo === 'h' || sexo === 'female') {
                            icon = 'fas fa-venus';
                            bgColor = 'bg-rosado'; // Usa la clase personalizada
                            displayText = 'Hembra';
                        }

                        return `<div class="gender-cell">
            <span class="badge ${bgColor} px-3 py-2">
                <i class="${icon} me-1"></i>
                ${displayText}
            </span>
        </div>`;
                    }
                },

                {
                    FieldName: 'masc_EsAdoptado',
                    DisplayName: '🏠 Estado',
                    Width: '130px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        // Manejar diferentes valores posibles
                        const esAdoptado = data === true || data === 'true' || data === 1 || data === '1' ||
                            (typeof data === 'string' && data.toLowerCase() === 'adoptado');

                        if (esAdoptado) {
                            return `<div class="adoption-cell">
                            <span class="badge bg-success bg-gradient px-3 py-2">
                                <i class="fas fa-home me-1"></i>
                                Adoptado
                            </span>
                        </div>`;
                        } else {
                            return `<div class="adoption-cell">
                            <span class="badge bg-warning bg-gradient px-3 py-2 text-white">
                                <i class="fas fa-heart me-1"></i>
                                Disponible
                            </span>
                        </div>`;
                        }
                    }
                }

            ];
            datatable.init(Direction, header);
        });
    }

    return obj;

}());