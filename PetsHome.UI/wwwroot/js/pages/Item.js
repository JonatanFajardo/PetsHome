
var Item = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'itm_Id',
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
                    FieldName: 'itm_Codigo',
                    DisplayName: '📋 Código',
                    Width: '140px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        if (!data) {
                            return `<div class="code-cell">
                                            <span class="text-muted fst-italic">Sin código</span>
                                        </div>`;
                                                }
                                                return `<div class="code-cell">
                                        <span class="badge bg-dark bg-gradient px-3 py-2">
                                            <i class="fas fa-barcode me-1"></i>
                                            ${data}
                                        </span>
                                    </div>`;
                    }
                },
                {
                    FieldName: 'itm_Descripcion',
                    DisplayName: '📦 Descripción',
                    Width: '300px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        return `<div class="description-cell">
                                    <i class="fas fa-box text-primary me-2"></i>
                                    <span class="fw-bold text-dark" style="font-size: 15px;">${data || 'Sin descripción'}</span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'cat_Descripcion',
                    DisplayName: '🏷️ Categoría',
                    Width: '180px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Searchable: true,
                    Render: function (data) {
                        if (!data) {
                            return `<div class="category-cell">
                                        <span class="text-muted fst-italic">Sin categoría</span>
                                    </div>`;
                        }

                        // Colores por categoría (puedes personalizar según tus categorías)
                        const categoryColors = {
                            'alimento': 'bg-success',
                            'juguete': 'bg-warning',
                            'medicina': 'bg-danger',
                            'accesorio': 'bg-info',
                            'limpieza': 'bg-primary'
                        };

                        const colorClass = categoryColors[data.toLowerCase()] || 'bg-secondary';
                        const textClass = colorClass === 'bg-warning' ? 'text-dark' : 'text-white';

                        return `<div class="category-cell">
                                    <span class="badge ${colorClass} bg-gradient px-3 py-2 ${textClass}">
                                        <i class="fas fa-tag me-1"></i>
                                        ${data}
                                    </span>
                                </div>`;
                    }
                },
                {
                    FieldName: 'itm_Precio',
                    DisplayName: '💰 Precio',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (!data || data === 0) {
                            return `<div class="price-cell">
                                        <span class="text-muted fst-italic">Gratis</span>
                                    </div>`;
                                            }

                                            const precio = parseFloat(data);
                                            let colorClass = 'bg-success';
                                            let textClass = 'text-white';

                                            if (precio >= 1000) {
                                                colorClass = 'bg-danger';
                                            } else if (precio >= 500) {
                                                colorClass = 'bg-warning';
                                                textClass = 'text-white';
                                            } else if (precio >= 100) {
                                                colorClass = 'bg-primary';
                                            }

                                            return `<div class="price-cell">
                                    <span class="badge ${colorClass} bg-gradient px-3 py-2 ${textClass}">
                                        L. ${precio.toLocaleString('es-HN', { minimumFractionDigits: 2 })}
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