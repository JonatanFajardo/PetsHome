var Localidad = (function () {
    var obj = {};

    // Variables globales del módulo
    var departamentosData = [];
    var currentView = 'grid';
    var currentSort = 'name-asc';
    var urlDetailDepartamento = '';
    var urlFormPartialDepartamento = '';

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                { FieldName: 'depto_Id' },
                { FieldName: 'depto_Codigo' },
                { FieldName: 'depto_Descripcion' },
                { FieldName: 'depto_Capital' },
                {
                    FieldName: 'depto_Poblacion',
                    Render: function (data, type, row) {
                        if (data) {
                            return parseInt(data).toLocaleString();
                        }
                        return 'N/A';
                    }
                },
                {
                    FieldName: 'depto_AreaKm2',
                    Render: function (data, type, row) {
                        if (data) {
                            return parseFloat(data).toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + ' km²';
                        }
                        return 'N/A';
                    }
                },
                {
                    FieldName: 'municipiosCount',
                    Render: function (data, type, row) {
                        return '<span class="badge badge-info">0</span>';
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    }

    // Inicializar el módulo
    obj.init = function (config) {
        urlDetailDepartamento = config.urlDetail;
        urlFormPartialDepartamento = config.urlUpdate;

        // Inicializar DataTable
        obj.datatable({
            urlList: config.urlList,
            urlDetail: config.urlDetail,
            urlInsert: config.urlInsert,
            urlUpdate: config.urlUpdate
        });

        // Cargar datos al iniciar
        loadDepartamentosData(config.urlList);

        // Configurar eventos
        setupEventListeners();
    };

    // Cargar datos de departamentos
    function loadDepartamentosData(urlList) {
        $.ajax({
            url: urlList,
            type: "GET",
            dataType: "json",
            success: function (response) {
                if (response.data) {
                    departamentosData = response.data;
                    $('#departamentosCount').text(departamentosData.length);
                    renderCardsView();
                }
            },
            error: function (xhr, status, error) {
                console.error("Error al cargar departamentos:", error);
            }
        });
    }

    // Funciones para acciones de departamentos
    obj.viewDepartamento = function (id) {
        window.location.href = urlDetailDepartamento + '/' + id;
    };

    obj.editDepartamento = function (id) {
        window.location.href = urlFormPartialDepartamento + '/' + id;
    };

    obj.deleteDepartamento = function (id) {
        $('#delete-item-id').val(id);
        $('#delete-modal').modal('show');
    };

    // Crear card de departamento usando el template
    function createDepartmentCard(dept) {
        const template = document.getElementById('department-card-template');
        const clone = template.content.cloneNode(true);

        clone.querySelector('[data-field="name"]').textContent = dept.depto_Descripcion;
        clone.querySelector('[data-field="code"]').textContent = dept.depto_Codigo || '';
        clone.querySelector('[data-field="capital"]').textContent = dept.depto_Capital || 'N/A';

        const poblacion = dept.depto_Poblacion ? parseInt(dept.depto_Poblacion).toLocaleString() : 'N/A';
        clone.querySelector('[data-field="population"]').textContent = poblacion;

        const area = dept.depto_AreaKm2 ? parseFloat(dept.depto_AreaKm2).toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 0 }) + ' km²' : 'N/A';
        clone.querySelector('[data-field="area"]').textContent = 'Área: ' + area;

        const municipiosCount = 0;
        clone.querySelector('[data-field="municipalities"]').textContent = 'Municipios: ' + municipiosCount;

        const card = clone.querySelector('.department-card');
        card.addEventListener('click', () => obj.viewDepartamento(dept.depto_Id));

        return clone;
    }

    // Renderizar vista de tarjetas
    function renderCardsView() {
        const container = document.getElementById('departamentosGrid');
        container.innerHTML = '';

        if (departamentosData.length === 0) {
            const noResults = document.createElement('div');
            noResults.className = 'col-12 text-center py-5';
            noResults.innerHTML = '<p class="text-muted">No se encontraron departamentos</p>';
            container.appendChild(noResults);
        } else {
            departamentosData.forEach(dept => {
                const card = createDepartmentCard(dept);
                container.appendChild(card);
            });
        }
    }

    // Alternar entre vista tabla y cards
    obj.toggleView = function (view) {
        currentView = view;
        if (view === 'table') {
            $('#tableView').show();
            $('#gridView').hide();
            $('#tableViewBtn').addClass('active');
            $('#gridViewBtn').removeClass('active');
        } else {
            $('#tableView').hide();
            $('#gridView').show();
            $('#tableViewBtn').removeClass('active');
            $('#gridViewBtn').addClass('active');
            renderCardsView();
        }
    };

    // Ordenar datos
    obj.sortData = function (sortType) {
        currentSort = sortType;

        let sortLabel = '';
        switch (sortType) {
            case 'name-asc': sortLabel = 'Nombre (A-Z)'; break;
            case 'name-desc': sortLabel = 'Nombre (Z-A)'; break;
            case 'population-desc': sortLabel = 'Poblacion (Mayor a menor)'; break;
            case 'population-asc': sortLabel = 'Poblacion (Menor a mayor)'; break;
            case 'area-desc': sortLabel = 'Area (Mayor a menor)'; break;
            case 'area-asc': sortLabel = 'Area (Menor a mayor)'; break;
        }
        $('#sortLabel').text(sortLabel);

        departamentosData.sort((a, b) => {
            switch (sortType) {
                case 'name-asc':
                    return (a.depto_Descripcion || '').localeCompare(b.depto_Descripcion || '');
                case 'name-desc':
                    return (b.depto_Descripcion || '').localeCompare(a.depto_Descripcion || '');
                case 'population-desc':
                    return (b.depto_Poblacion || 0) - (a.depto_Poblacion || 0);
                case 'population-asc':
                    return (a.depto_Poblacion || 0) - (b.depto_Poblacion || 0);
                case 'area-desc':
                    return (b.depto_AreaKm2 || 0) - (a.depto_AreaKm2 || 0);
                case 'area-asc':
                    return (a.depto_AreaKm2 || 0) - (b.depto_AreaKm2 || 0);
                default:
                    return 0;
            }
        });

        renderCardsView();
        return false;
    };

    // Placeholder para filtros
    obj.toggleFilters = function () {
        alert('Funcionalidad de filtros avanzados - Por implementar');
    };

    // Configurar event listeners
    function setupEventListeners() {
        $('#globalSearch').on('keyup', function () {
            let searchTerm = this.value.toLowerCase();

            if (currentView === 'table') {
                $('#datatable').DataTable().search(searchTerm).draw();
            } else {
                if (searchTerm === '') {
                    renderCardsView();
                } else {
                    const filtered = departamentosData.filter(item => {
                        return (item.depto_Descripcion && item.depto_Descripcion.toLowerCase().includes(searchTerm)) ||
                            (item.depto_Codigo && item.depto_Codigo.toLowerCase().includes(searchTerm)) ||
                            (item.depto_Capital && item.depto_Capital.toLowerCase().includes(searchTerm));
                    });

                    const container = document.getElementById('departamentosGrid');
                    container.innerHTML = '';

                    if (filtered.length === 0) {
                        const noResults = document.createElement('div');
                        noResults.className = 'col-12 text-center py-5';
                        noResults.innerHTML = '<p class="text-muted">No se encontraron resultados</p>';
                        container.appendChild(noResults);
                    } else {
                        filtered.forEach(dept => {
                            const card = createDepartmentCard(dept);
                            container.appendChild(card);
                        });
                    }
                }
            }
        });
    }

    return obj;
}());