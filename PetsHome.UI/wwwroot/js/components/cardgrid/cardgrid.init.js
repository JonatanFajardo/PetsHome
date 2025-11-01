/**
 * CardGrid - Sistema de inicialización de grids con cards
 * Similar al patrón de DataTable pero para visualización en cards
 */

var CardGrid = (function () {
    var obj = {};
    var config = {};
    var data = [];
    var filteredData = [];
    var currentSort = 'name-asc';

    /**
     * Inicializa el grid de cards
     * @param {Object} options - Configuración del card grid
     * @param {string} options.containerId - ID del contenedor donde se renderizarán las cards
     * @param {string} options.urlList - URL para obtener los datos
     * @param {Function} options.renderCard - Función que renderiza una card individual
     * @param {Function} options.onCardClick - Callback cuando se hace click en una card (opcional)
     * @param {Object} options.searchConfig - Configuración de búsqueda (opcional)
     */
    obj.init = function (options) {
        config = $.extend({
            containerId: '#cardGrid',
            urlList: '',
            renderCard: null,
            onCardClick: null,
            searchConfig: {
                enabled: true,
                inputId: '#globalSearch',
                fields: [] // Campos en los que buscar
            },
            sortConfig: {
                enabled: true,
                defaultSort: 'name-asc'
            },
            counterConfig: {
                enabled: true,
                counterId: '#itemCount'
            },
            responsive: {
                cols: {
                    xs: 12,  // col-12 en extra small
                    sm: 6,   // col-sm-6 en small
                    md: 4,   // col-md-4 en medium
                    lg: 3    // col-lg-3 en large
                }
            }
        }, options);

        currentSort = config.sortConfig.defaultSort;

        // Cargar datos iniciales
        loadData();

        // Configurar búsqueda si está habilitada
        if (config.searchConfig.enabled && config.searchConfig.inputId) {
            setupSearch();
        }
    };

    /**
     * Carga los datos desde el servidor
     */
    function loadData() {
        if (!config.urlList) {
            console.error('CardGrid: No se especificó urlList');
            return;
        }

        $.ajax({
            url: config.urlList,
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                if (response.data) {
                    data = response.data;
                    filteredData = data;

                    // Actualizar contador si está habilitado
                    if (config.counterConfig.enabled && config.counterConfig.counterId) {
                        $(config.counterConfig.counterId).text(data.length);
                    }

                    // Aplicar ordenamiento por defecto
                    sortData(currentSort);

                    // Renderizar cards
                    render();
                } else {
                    console.error('CardGrid: Respuesta sin datos');
                }
            },
            error: function (xhr, status, error) {
                console.error('CardGrid: Error al cargar datos:', error);
                showError('Error al cargar los datos');
            }
        });
    }

    /**
     * Renderiza todas las cards en el contenedor
     */
    function render() {
        var $container = $(config.containerId);
        $container.empty();

        if (filteredData.length === 0) {
            $container.html(
                '<div class="col-12 text-center py-5">' +
                '<p class="text-muted">No se encontraron resultados</p>' +
                '</div>'
            );
            return;
        }

        // Generar clase de columna Bootstrap basada en la configuración responsive
        var colClass = 'col-' + config.responsive.cols.xs +
                      ' col-sm-' + config.responsive.cols.sm +
                      ' col-md-' + config.responsive.cols.md +
                      ' col-lg-' + config.responsive.cols.lg;

        filteredData.forEach(function (item) {
            if (config.renderCard && typeof config.renderCard === 'function') {
                var $card = config.renderCard(item, colClass);
                $container.append($card);
            } else {
                console.warn('CardGrid: No se especificó función renderCard');
            }
        });
    }

    /**
     * Configura la funcionalidad de búsqueda
     */
    function setupSearch() {
        var $searchInput = $(config.searchConfig.inputId);

        if ($searchInput.length === 0) {
            console.warn('CardGrid: No se encontró el input de búsqueda:', config.searchConfig.inputId);
            return;
        }

        $searchInput.on('keyup', function () {
            var searchTerm = $(this).val().toLowerCase().trim();

            if (searchTerm === '') {
                filteredData = data;
            } else {
                filteredData = data.filter(function (item) {
                    // Buscar en los campos especificados
                    return config.searchConfig.fields.some(function (field) {
                        var value = getNestedProperty(item, field);
                        return value && value.toString().toLowerCase().includes(searchTerm);
                    });
                });
            }

            render();
        });
    }

    /**
     * Ordena los datos según el tipo especificado
     * @param {string} sortType - Tipo de ordenamiento
     */
    function sortData(sortType) {
        currentSort = sortType;

        filteredData.sort(function (a, b) {
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
    }

    /**
     * Método público para ordenar y re-renderizar
     * @param {string} sortType - Tipo de ordenamiento
     */
    obj.sort = function (sortType) {
        sortData(sortType);
        render();
    };

    /**
     * Método público para recargar los datos
     */
    obj.reload = function () {
        loadData();
    };

    /**
     * Método público para obtener los datos actuales
     */
    obj.getData = function () {
        return data;
    };

    /**
     * Método público para obtener los datos filtrados
     */
    obj.getFilteredData = function () {
        return filteredData;
    };

    /**
     * Muestra un mensaje de error
     */
    function showError(message) {
        var $container = $(config.containerId);
        $container.html(
            '<div class="col-12 text-center py-5">' +
            '<p class="text-danger">' + message + '</p>' +
            '</div>'
        );
    }

    /**
     * Obtiene una propiedad anidada de un objeto usando notación de punto
     * @param {Object} obj - Objeto del cual obtener la propiedad
     * @param {string} path - Ruta de la propiedad (ej: 'user.name')
     */
    function getNestedProperty(obj, path) {
        return path.split('.').reduce(function (current, prop) {
            return current ? current[prop] : undefined;
        }, obj);
    }

    /**
     * Formatea un número con separadores de miles
     */
    obj.formatNumber = function (num) {
        if (!num) return 'N/A';
        return parseInt(num).toLocaleString();
    };

    /**
     * Formatea un número decimal
     */
    obj.formatDecimal = function (num, decimals) {
        if (!num) return 'N/A';
        decimals = decimals || 2;
        return parseFloat(num).toLocaleString(undefined, {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        });
    };

    return obj;
}());
