//==================================================
//Recorre el arreglo de encabezados
//==================================================

var datatableCatalogs2 = (function () {
    var obj = {};
    //serverSide

    /**
     * @param {any} listUrl
     * @param {any} header
     */
    obj.createDatatable = function (listUrl, header) {
        var exportOptions = { columns: [0, 1, 2], orthogonal: "export" };
        $('#datatable').DataTable({
            responsive: true,
            serverSide: true,
            //deferLoading: true,
            deferRender: true,
            buttons: [
                {
                    text: '<i class="mdi mdi-refresh"> Recargar</i>',
                    titleAttr: 'Recargar tabla',
                    className: 'btn btn-secondary',
                    action: function (e, dt, config) {
                        dt.ajax.reload();
                    }
                },
                //{
                //    title: "Exportar a CSV",
                //    extend: "csvHtml5",
                //    text: "<i class='mdi mdi-file-multiple-outline'></i> CSV",
                //    className: "btn-secondary",
                //    exportOptions: exportOptions
                //},
                {
                    extend: "pdfHtml5",
                    title: "Exportar a PDF",
                    text: "<i class='mdi mdi-file-pdf-outline'></i> PDF",
                    className: "btn btn-secondary",
                    exportOptions: exportOptions
                },
                {
                    extend: "excelHtml5",
                    title: "Exportar a EXCEL",
                    text: "<i class='mdi mdi-file-excel-outline'></i> Excel",
                    className: "btn btn-secondary",
                    exportOptions: exportOptions
                },
                {

                    //attr: 
                    //    'data-toggle': "modal",
                    //    'data-target': "#edit-modal"
                    //},
                    //text: 'Nuevo',
                    //class: "btn btn-primary",
                    //attr: {
                    //    title: "Añadir nuevo elemento",
                    //    id: "add-btn",
                    //    //onclick: "nuevo()",
                    //    'data-toggle': "modal",
                    //    'data-target': "#edit-modal"
                    //    /*onclick: "nuevo()"*/
                    //}
                    attr: {
                        title: "Añadir nuevo elemento",
                        id: "add-btn",
                        class: "btn btn-primary ",
                        'data-style': "zoom-in",
                        'data-toggle': "modal",
                        'data-target': "#edit-modal"
                    },
                    text: '<i class="mdi mdi-plus-thick ladda-button"> Nuevo</i>'


                    //text: 'Nuevo <i class="mdi mdi-plus-thick"></i>',
                    //className: "btn btn-success",
                    //attr: {
                    //    title: "Añadir componente nuevo",
                    //    onclick: "Modal()"
                    //}

                }
            ],
            ajax: function (data, callback, settings) {
                var tableId = settings.nTable.id;
                console.log("mostrar")
                // Mostrar progreso inicial
                DataTableProgress.show(tableId);
                DataTableProgress.update('init', 'DataTable inicializado correctamente');

                // Validaciones previas
                if (!DirectionUrls || !listUrl) {
                    console.error('listUrl no está definido');

                    DataTableProgress.error('Error de configuración', 'La URL para cargar datos no está configurada');

                    callback({
                        data: [],
                        recordsTotal: 0,
                        recordsFiltered: 0,
                        error: 'URL no configurada'
                    });
                    return;
                }

                if (typeof callback !== 'function') {
                    console.error('Callback no es una función válida');
                    DataTableProgress.error('Error interno', 'Callback no válido');
                    return;
                }

                // Actualizar progreso
                DataTableProgress.update('ajax_start', 'Iniciando petición AJAX a: ' + listUrl, {
                    url: listUrl,
                    data: data
                });

                var ajaxTimeout = 30000;

                $.ajax({
                    url: listUrl,
                    type: "GET",
                    dataType: "json",
                    timeout: ajaxTimeout,
                    data: data,
                    cache: false,
                    processData: true,

                    beforeSend: function (xhr) {
                        DataTableProgress.show('datatable', '#myTableContainer');
                        DataTableProgress.update('ajax_send', 'Iniciando petición AJAX...');
                        console.log('Enviando petición AJAX...');
                    },

                    dataSrc: function (json) {
                        // AJAX response received
                        DataTableProgress.update('ajax_response', 'Datos recibidos del servidor.');
                        return json.data;
                    },

                    success: function (response, textStatus, jqXHR) {
                        try {
                            DataTableProgress.update('ajax_response', 'Respuesta recibida del servidor', {
                                status: textStatus,
                                recordCount: Array.isArray(response) ? response.length : (response.data ? response.data.length : 'desconocido')
                            });

                            console.log('Respuesta recibida:', response);
                            console.log('Status:', textStatus);

                            DataTableProgress.update('processing', 'Procesando datos recibidos...');

                            if (!response) {
                                throw new Error('Respuesta vacía del servidor');
                            }

                            var processedResponse = validateAndProcessResponse(response);

                            DataTableProgress.update('complete', `Datos procesados correctamente (${processedResponse.data.length} registros)`, {
                                recordCount: processedResponse.data.length,
                                totalRecords: processedResponse.recordsTotal
                            });

                            callback(processedResponse);

                            console.log('Datos procesados correctamente');

                        } catch (processError) {
                            console.error('Error procesando respuesta:', processError);
                            console.error('Stack trace:', processError.stack); // Línea agregada para ver el stack trace

                            DataTableProgress.error('Error al procesar datos', processError.message);

                            callback({
                                data: [],
                                recordsTotal: 0,
                                recordsFiltered: 0,
                                error: 'Error procesando datos: ' + processError.message
                            });
                        }
                    },

                    error: function (jqXHR, textStatus, errorThrown) {
                        console.error('=== ERROR AJAX ===');
                        console.error('Status:', textStatus);
                        console.error('Error:', errorThrown);
                        console.error('Status Code:', jqXHR.status);
                        console.error('Response Text:', jqXHR.responseText); // Línea agregada para ver la respuesta completa
                        console.error('Response Headers:', jqXHR.getAllResponseHeaders()); // Línea agregada para ver headers

                        var errorMessage = 'Error desconocido';
                        var errorDetails = `Status: ${textStatus}\nCódigo: ${jqXHR.status}\nError: ${errorThrown}`;


                        // Determinar mensaje de error
                        switch (textStatus) {
                            case 'timeout':
                                errorMessage = 'La petición tardó demasiado tiempo';
                                break;
                            case 'error':
                                switch (jqXHR.status) {
                                    case 0:
                                        errorMessage = 'Sin conexión al servidor';
                                        break;
                                    case 401:
                                        errorMessage = 'Sesión expirada';
                                        break;
                                    case 403:
                                        errorMessage = 'Acceso denegado';
                                        break;
                                    case 404:
                                        errorMessage = 'Recurso no encontrado';
                                        break;
                                    case 500:
                                        errorMessage = 'Error interno del servidor';
                                        break;
                                    default:
                                        errorMessage = `Error del servidor (${jqXHR.status})`;
                                }
                                break;
                            case 'parsererror':
                                errorMessage = 'Error en formato de respuesta';
                                break;
                            default:
                                errorMessage = `Error: ${textStatus}`;
                        }

                        // Mostrar error más detallado
                        DataTableProgress.error(errorMessage, errorDetails);

                        callback({
                            data: [],
                            recordsTotal: 0,
                            recordsFiltered: 0,
                            error: errorMessage
                        });
                    },

                    complete: function (jqXHR, textStatus) {
                        console.log('Petición AJAX completada:', textStatus);
                        console.log('Status final:', jqXHR.status);
                    }
                });
            },
            columnDefs: obj.dataHeader(header)

        });
    }

    
        /**
          * DataTableProgress - Sistema completo de progreso para DataTables
          * Maneja estados: init, ajax_start, ajax_send, ajax_response, processing, complete, error
          */

        var DataTableProgress = (function () {
            'use strict';

            // Configuración por defecto
            var config = {
                showConsoleLog: true,
                autoHideSuccess: true,
                autoHideDelay: 3000,
                useProgressBar: true,
                animationDuration: 300,
                notificationPosition: 'top-right' // top-right, top-left, bottom-right, bottom-left
            };

            // Estados activos por tabla
            var activeStates = {};

            // Contenedores de UI
            var containers = {
                notifications: null,
                // Eliminamos el modal
                progressBars: {} // Ahora contendrá referencias a las barras de progreso por tabla
            };

            /**
             * Inicializar el sistema de progreso
             */
            function init() {
                createNotificationContainer();
                setupStyles(); // Mantener solo los estilos generales y de notificación

                if (config.showConsoleLog) {
                    console.log('DataTableProgress inicializado');
                }
            }

            /**
             * Crear contenedor de notificaciones
             */
            function createNotificationContainer() {
                if (containers.notifications) return;

                var positionClass = getPositionClass(config.notificationPosition);

                containers.notifications = $(`
            <div id="datatable-notifications" class="datatable-notifications ${positionClass}">
            </div>
        `);

                $('body').append(containers.notifications);
            }

            /**
             * Configurar estilos CSS
             */
            function setupStyles() {
                if ($('#datatable-progress-styles').length) return;

                var styles = `
            <style id="datatable-progress-styles">
                /* Estilos de notificaciones */
                .datatable-notifications {
                    position: fixed;
                    z-index: 9999;
                    width: 350px;
                    max-height: 100vh;
                    overflow-y: auto;
                    pointer-events: none;
                }
                
                .datatable-notifications.top-right {
                    top: 20px;
                    right: 20px;
                }
                
                .datatable-notifications.top-left {
                    top: 20px;
                    left: 20px;
                }
                
                .datatable-notifications.bottom-right {
                    bottom: 20px;
                    right: 20px;
                }
                
                .datatable-notifications.bottom-left {
                    bottom: 20px;
                    left: 20px;
                }
                
                .progress-notification {
                    background: white;
                    border-radius: 12px;
                    padding: 16px;
                    margin-bottom: 12px;
                    box-shadow: 0 8px 32px rgba(0,0,0,0.12);
                    border-left: 4px solid #007bff;
                    pointer-events: auto;
                    transform: translateX(100%);
                    animation: slideInRight 0.3s ease-out forwards;
                    max-width: 100%;
                }
                
                .progress-notification.success {
                    border-left-color: #28a745;
                }
                
                .progress-notification.error {
                    border-left-color: #dc3545;
                }
                
                .progress-notification.warning {
                    border-left-color: #ffc107;
                }
                
                @keyframes slideInRight {
                    to { transform: translateX(0); }
                }
                
                @keyframes slideOutRight {
                    to { transform: translateX(100%); }
                }
                
                .notification-header {
                    display: flex;
                    align-items: center;
                    margin-bottom: 8px;
                }
                
                .notification-icon {
                    margin-right: 10px;
                    font-size: 1.2em;
                }
                
                .notification-title {
                    font-size: 0.95em;
                    font-weight: 600;
                    margin: 0;
                    flex-grow: 1;
                }
                
                .notification-close {
                    background: none;
                    border: none;
                    font-size: 1.2em;
                    cursor: pointer;
                    opacity: 0.6;
                    padding: 0;
                    width: 20px;
                    height: 20px;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                }
                
                .notification-close:hover {
                    opacity: 1;
                }
                
                .notification-description {
                    font-size: 0.85em;
                    color: #666;
                    margin: 0;
                    line-height: 1.4;
                }
                
                .notification-progress {
                    margin-top: 8px;
                    height: 3px;
                    background: rgba(0,0,0,0.1);
                    border-radius: 2px;
                    overflow: hidden;
                }
                
                .notification-progress-bar {
                    height: 100%;
                    background: #007bff;
                    width: 0%;
                    transition: width 0.3s ease;
                }
                
                /* Estilos de la barra de progreso horizontal para la tabla */
                //#datatable-progress-datatable {
                //    transition: opacity 0.5s ease-out; 
                //}

                .datatable-progress-container {
                    background: #f8f9fa;
                    border: 1px solid #e9ecef;
                    border-radius: 8px;
                    padding: 15px;
                    margin-bottom: 20px; /* Espacio entre la barra y la tabla */
                    box-shadow: 0 4px 12px rgba(0,0,0,0.05);
                    overflow: hidden; /* Asegura que no se desborde el contenido */

                }

                .datatable-progress-container.hidden {
                    display: none;
                }
                
                .datatable-progress-timeline {
                    display: flex;
                    justify-content: space-between;
                    align-items: flex-start;
                    position: relative;
                    padding: 20px 0;
                }
                
                .datatable-timeline-step {
                    flex: 1;
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    text-align: center;
                    position: relative;
                    padding: 0 10px;
                }
                
                .datatable-timeline-step:not(:last-child)::after {
                    content: '';
                    position: absolute;
                    left: 50%;
                    transform: translateX(-50%);
                    top: 36px; /* Ajustado para que la línea quede un poco más abajo del icono */
                    width: calc(100% - 60px);
                    height: 2px;
                    background: #e9ecef;
                    z-index: -1;
                }
                
                .datatable-timeline-step.active::after,
                .datatable-timeline-step.completed::after {
                    background: #007bff;
                }
                
                .datatable-timeline-step.completed::after {
                    background: #28a745;
                }
                
                .datatable-step-icon {
                    width: 36px;
                    height: 36px;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin-bottom: 8px;
                    font-size: 1.2em;
                    flex-shrink: 0;
                    z-index: 1;
                }
                
                .datatable-timeline-step.pending .datatable-step-icon {
                    background: #e9ecef;
                    color: #6c757d;
                }
                
                .datatable-timeline-step.active .datatable-step-icon {
                    background: #007bff;
                    color: white;
                }
                
                .datatable-timeline-step.completed .datatable-step-icon {
                    background: #28a745;
                    color: white;
                }
                
                .datatable-timeline-step.error .datatable-step-icon {
                    background: #dc3545;
                    color: white;
                }
                
                .datatable-step-info h6 {
                    margin: 0 0 4px 0;
                    font-size: 1em;
                    white-space: nowrap;
                    overflow: hidden;
                    text-overflow: ellipsis;
                }
                
                .datatable-step-info p {
                    margin: 0;
                    font-size: 0.85em;
                    color: #6c757d;
                    white-space: nowrap;
                    overflow: hidden;
                    text-overflow: ellipsis;
                }

                .datatable-current-status {
                    display: flex;
                    align-items: center;
                    background: #eaf3ff; /* Fondo más suave */
                    padding: 10px 15px;
                    border-radius: 6px;
                    margin-top: 15px;
                    border: 1px solid #cce5ff;
                }

                .datatable-status-icon {
                    width: 30px;
                    height: 30px;
                    background: rgba(0,123,255,0.1);
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    margin-right: 15px;
                }
                
                .datatable-status-title {
                    color: #2c3e50;
                    margin-bottom: 2px;
                    font-size: 0.9em;
                    font-weight: 600;
                }
                
                .datatable-status-description {
                    color: #6c757d;
                    font-size: 0.8em;
                }
                
                /* Responsive */

 

        /* Teléfonos grandes (≤ 768px) */
        @media (max-width: 768px) {
            .datatable-notifications {
                width: calc(100vw - 40px);
                left: 20px !important;
                right: 20px !important;
            }

            .datatable-timeline-step {
                flex-basis: 100%; /* 1 por fila */
                margin-bottom: 20px;
            }
        }

        /* Teléfonos pequeños (≤ 480px) */
        @media (max-width: 480px) {
            .datatable-notifications {
                width: calc(100vw - 20px);
                left: 10px !important;
                right: 10px !important;
            }
            </style>
        `;

                $('head').append(styles);
            }

            /**
             * Obtener clase CSS de posición
             */
            function getPositionClass(position) {
                var validPositions = ['top-right', 'top-left', 'bottom-right', 'bottom-left'];
                return validPositions.includes(position) ? position : 'top-right';
            }

            /**
             * Mostrar progreso inicial
             * @param {string} tableId - El ID de la tabla a la que se asocia el progreso.
             * @param {string} targetElementSelector - Selector del elemento donde se insertará la barra de progreso.
             */
            function show(tableId, targetElementSelector) {
                if (!tableId) {
                    console.warn('DataTableProgress.show: tableId es requerido');
                    return;
                }
                if (!targetElementSelector) {
                    console.warn('DataTableProgress.show: targetElementSelector es requerido para mostrar el progreso en la tabla.');
                    return;
                }

                // Inicializar estado
                activeStates[tableId] = {
                    currentStep: 'init',
                    steps: {},
                    startTime: Date.now(),
                    details: {}
                };

                // Crear y añadir el contenedor de la barra de progreso a la tabla
                createTableProgressBar(tableId, targetElementSelector);

                if (config.showConsoleLog) {
                    console.log(`DataTableProgress.show: ${tableId}`);
                }
            }

            /**
             * Actualizar estado del progreso
             * @param {string} step - Clave del paso actual.
             * @param {string} message - Mensaje descriptivo del paso.
             * @param {object} details - Detalles técnicos opcionales.
             */
            function update(step, message, details) {
                var tableId = getCurrentTableId();
                if (!tableId || !activeStates[tableId]) {
                    console.warn('DataTableProgress.update: No hay estado activo para actualizar.');
                    return;
                }

                var state = activeStates[tableId];
                state.currentStep = step;
                state.steps[step] = {
                    message: message,
                    details: details || {},
                    timestamp: Date.now(),
                    status: 'completed'
                };

                // Actualizar UI de la barra de progreso de la tabla
                updateTableProgressBar(tableId, step, message, details);

                if (config.showConsoleLog) {
                    console.log(`DataTableProgress.update [${step}]:`, message, details);
                }
            }

            /**
             * Mostrar error
             * @param {string} title - Título del error.
             * @param {string} message - Mensaje del error.
             * @param {object} details - Detalles técnicos opcionales del error.
             */
            function error(title, message, details) {
                var tableId = getCurrentTableId();
                if (tableId && activeStates[tableId]) {
                    var state = activeStates[tableId];
                    state.steps[state.currentStep] = {
                        message: message,
                        details: details || {},
                        timestamp: Date.now(),
                        status: 'error'
                    };
                    updateTableProgressBarError(tableId, title, message);
                }

                showNotification('error', title, message);

                if (config.showConsoleLog) {
                    console.error(`DataTableProgress.error: ${title}`, message, details);
                }
            }

            /**
             * Ocultar progreso
             * @param {string} tableId - ID de la tabla para ocultar su progreso.
             */
            function hide(tableId) {
                if (tableId && activeStates[tableId]) {
                    // Ocultar la barra de progreso de la tabla
                    if (containers.progressBars[tableId]) {
                        containers.progressBars[tableId].addClass('hidden');
                    }
                    delete activeStates[tableId];
                    delete containers.progressBars[tableId]; // Eliminar referencia
                }

                if (config.showConsoleLog) {
                    console.log(`DataTableProgress.hide: ${tableId}`);
                }
            }

            /**
             * Mostrar notificación
             */
            function showNotification(type, title, message, duration) {
                if (!containers.notifications) {
                    createNotificationContainer();
                }

                duration = duration || (type === 'error' ? 8000 : 4000);

                var notificationId = 'notification-' + Date.now();
                var iconClass = getIconClass(type);

                var notification = $(`
            <div id="${notificationId}" class="progress-notification ${type}">
                <div class="notification-header">
                    <div class="notification-icon">
                        <i class="${iconClass}"></i>
                    </div>
                    <h6 class="notification-title">${title}</h6>
                    <button class="notification-close" onclick="DataTableProgress.removeNotification('${notificationId}')">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
                <p class="notification-description">${message}</p>
                ${config.useProgressBar && type === 'info' ? '<div class="notification-progress"><div class="notification-progress-bar"></div></div>' : ''}
            </div>
        `);

                containers.notifications.append(notification);

                // Auto-hide para tipos que no son errores
                if (config.autoHideSuccess && type !== 'error') {
                    setTimeout(function () {
                        removeNotification(notificationId);
                    }, duration);
                }

                return notificationId;
            }

            /**
             * Remover notificación
             */
            function removeNotification(notificationId) {
                var notification = $('#' + notificationId);
                if (notification.length) {
                    notification.css('animation', 'slideOutRight 0.3s ease-in forwards');
                    setTimeout(function () {
                        notification.remove();
                    }, 300);
                }
            }

            /**
             * Crear y adjuntar la barra de progreso a la tabla
             * @param {string} tableId - El ID de la tabla.
             * @param {string} targetElementSelector - Selector del elemento donde se insertará la barra de progreso.
             */
            function createTableProgressBar(tableId, targetElementSelector) {
                // Si ya existe, solo la mostramos
                if (containers.progressBars[tableId]) {
                    containers.progressBars[tableId].removeClass('hidden');
                    initializeTableProgressBar(tableId); // Reiniciar el estado visual
                    return;
                }

                var progressHtml = `
            <div id="datatable-progress-${tableId}" class="datatable-progress-container">
                <div class="datatable-progress-timeline">
                    </div>
                <div class="datatable-current-status mt-3">
                    <div class="d-flex align-items-center">
                        <div class="datatable-status-icon me-3">
                            <i class="fas fa-circle-notch fa-spin text-primary"></i>
                        </div>
                        <div class="status-info flex-grow-1">
                            <h6 class="datatable-status-title mb-1">Iniciando...</h6>
                            <p class="datatable-status-description mb-0 text-muted">Preparando la carga de datos</p>
                        </div>
                    </div>
                </div>
            </div>
        `;

                var targetElement = $(targetElementSelector);
                if (targetElement.length) {
                    var progressBar = $(progressHtml);
                    targetElement.prepend(progressBar); // Inserta antes del contenido existente del target
                    containers.progressBars[tableId] = progressBar;
                    initializeTableProgressBar(tableId);
                } else {
                    console.error(`DataTableProgress: No se encontró el elemento objetivo para la tabla ${tableId} con el selector ${targetElementSelector}.`);
                }
            }

            /**
             * Inicializar la línea de tiempo de la barra de progreso de la tabla
             * @param {string} tableId - El ID de la tabla.
             */
            function initializeTableProgressBar(tableId) {
                var progressBarContainer = containers.progressBars[tableId];
                if (!progressBarContainer) return;

                var timeline = progressBarContainer.find('.datatable-progress-timeline');
                var steps = [
                    { key: 'init', title: 'Inicialización', description: 'Preparando tabla' },
                    { key: 'ajax_start', title: 'Configurando Petición', description: 'Validando parámetros' },
                    { key: 'ajax_send', title: 'Enviando Datos', description: 'Comunicando con servidor' },
                    { key: 'ajax_response', title: 'Recibiendo Respuesta', description: 'Descargando datos' },
                    { key: 'processing', title: 'Procesando Datos', description: 'Validando y normalizando' },
                    { key: 'complete', title: 'Completado', description: 'Datos listos para mostrar' }
                ];

                timeline.empty();

                steps.forEach(function (step) {
                    var stepHtml = `
                <div class="datatable-timeline-step pending" data-step="${step.key}">
                    <div class="datatable-step-icon">
                        <i class="fas fa-circle"></i>
                    </div>
                    <div class="datatable-step-info">
                        <h6>${step.title}</h6>
                        <p>${step.description}</p>
                    </div>
                </div>
            `;
                    timeline.append(stepHtml);
                });

                // Reiniciar el estado actual
                updateCurrentStatusTable(progressBarContainer, 'init', 'Preparando la carga de datos');
            }

            /**
             * Actualizar la barra de progreso de la tabla
             * @param {string} tableId - El ID de la tabla.
             * @param {string} step - El paso actual.
             * @param {string} message - El mensaje del paso.
             */
            function updateTableProgressBar(tableId, step, message) {
                var progressBarContainer = containers.progressBars[tableId];
                if (!progressBarContainer) return;

                updateTimelineStepTable(progressBarContainer, step, 'active');
                updateCurrentStatusTable(progressBarContainer, step, message);

                // Marcar pasos anteriores como completados
                var stepOrder = ['init', 'ajax_start', 'ajax_send', 'ajax_response', 'processing', 'complete'];
                var currentIndex = stepOrder.indexOf(step);
                for (var i = 0; i < currentIndex; i++) {
                    updateTimelineStepTable(progressBarContainer, stepOrder[i], 'completed');
                }

                // Si es el paso final, ocultar después de un tiempo
                if (step === 'complete') {
                    updateCurrentStatusTable(progressBarContainer, 'complete', 'La carga de datos se completó exitosamente');
                    showNotification('success', 'Carga Completada', 'Los datos se han cargado correctamente.');

                    if (config.autoHideSuccess) {
                        setTimeout(function () {
                            hide(tableId);
                        }, config.autoHideDelay);
                    }
                }
            }

            /**
             * Actualizar la barra de progreso de la tabla con un error
             * @param {string} tableId - El ID de la tabla.
             * @param {string} title - Título del error.
             * @param {string} message - Mensaje del error.
             */
            function updateTableProgressBarError(tableId, title, message) {
                var progressBarContainer = containers.progressBars[tableId];
                if (!progressBarContainer) return;

                var currentStep = activeStates[tableId]?.currentStep || 'error';
                updateTimelineStepTable(progressBarContainer, currentStep, 'error');
                updateCurrentStatusTable(progressBarContainer, 'error', message);

                showNotification('error', title, message);
            }

            /**
             * Actualizar un paso individual en la línea de tiempo de la tabla
             * @param {jQuery} container - El contenedor de la barra de progreso de la tabla.
             * @param {string} step - La clave del paso a actualizar.
             * @param {string} status - El nuevo estado ('pending', 'active', 'completed', 'error').
             */
            function updateTimelineStepTable(container, step, status) {
                var stepElement = container.find(`.datatable-timeline-step[data-step="${step}"]`);
                if (stepElement.length) {
                    stepElement.removeClass('pending active completed error').addClass(status);

                    var icon = stepElement.find('.datatable-step-icon i');
                    if (status === 'completed') {
                        icon.removeClass().addClass('fas fa-check');
                    } else if (status === 'active') {
                        icon.removeClass().addClass('fas fa-circle-notch fa-spin');
                    } else if (status === 'error') {
                        icon.removeClass().addClass('fas fa-times');
                    }
                }
            }

            /**
             * Actualizar el estado actual en la barra de progreso de la tabla
             * @param {jQuery} container - El contenedor de la barra de progreso de la tabla.
             * @param {string} step - La clave del paso actual.
             * @param {string} message - El mensaje descriptivo.
             */
            function updateCurrentStatusTable(container, step, message) {
                var statusTitle = container.find('.datatable-status-title');
                var statusDescription = container.find('.datatable-status-description');
                var statusIcon = container.find('.datatable-status-icon i');

                statusTitle.text(getStepTitle(step));
                statusDescription.text(message);

                if (step === 'complete') {
                    statusIcon.removeClass().addClass('fas fa-check-circle text-success');
                } else if (step === 'error') {
                    statusIcon.removeClass().addClass('fas fa-exclamation-circle text-danger');
                } else {
                    statusIcon.removeClass().addClass('fas fa-circle-notch fa-spin text-primary');
                }
            }

            /**
             * Obtener ID de tabla actual
             */
            function getCurrentTableId() {
                var tableIds = Object.keys(activeStates);
                return tableIds.length > 0 ? tableIds[tableIds.length - 1] : null;
            }

            /**
             * Obtener título del paso
             */
            function getStepTitle(step) {
                var titles = {
                    'init': 'Inicializando',
                    'ajax_start': 'Preparando Petición',
                    'ajax_send': 'Enviando Datos',
                    'ajax_response': 'Recibiendo Respuesta',
                    'processing': 'Procesando Datos',
                    'complete': 'Completado',
                    'error': 'Error'
                };
                return titles[step] || 'Procesando';
            }

            /**
             * Obtener clase de icono
             */
            function getIconClass(type) {
                var icons = {
                    'info': 'fas fa-info-circle',
                    'success': 'fas fa-check-circle',
                    'error': 'fas fa-exclamation-circle',
                    'warning': 'fas fa-exclamation-triangle'
                };
                return icons[type] || 'fas fa-info-circle';
            }

            /**
             * Configurar opciones
             */
            function configure(options) {
                config = Object.assign(config, options);
            }

            // Auto-inicializar cuando el DOM esté listo
            $(document).ready(function () {
                init();
            });

            // API pública
            return {
                show: show,
                update: update,
                error: error,
                hide: hide,
                showNotification: showNotification,
                removeNotification: removeNotification,
                configure: configure
            };

        })();


      

    /**
     * Configura el datatable.
     * */
    obj.config = function () {

        //configuraciones
        $.extend(true, $.fn.dataTable.defaults, {
            dom:
                "<'row mb-3' <'col-md-4 'B><'col-md-6'f><'col-md-2'l>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            order: [],
            scrollCollapse: true,
            paging: true,
            stateSave: true,
            //bLengthChange: false,
            //bInfo: false,
            processing: true,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
            pageLenght: 10,
            displayLength: 10,
            language: {
                processing: "Procesando...",
                lengthMenu: " _MENU_ ",
                zeroRecords: "No se encontraron resultados",
                emptyTable: "Ningún dato disponible en esta tabla",
                info: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                infoFiltered: "(filtrado de un total de _MAX_ registros)",
                infoPostFix: "",
                search: "",
                url: "",
                infoThousands: ",",
                loadingRecords: " ",
                searchPlaceholder: "Buscar en la tabla...",
                paginate: {
                    first: "Primero",
                    last: "Último",
                    next: "Siguiente",
                    previous: "Anterior"
                },
                aria: {
                    sortAscending: ": Activar para ordenar la columna de manera ascendente",
                    sortDescending: ": Activar para ordenar la columna de manera descendente"
                }
            }
        });
    }

    /**
     * Inicializa el datatable.
     * @param {Object} listUrl Direccion al que se enviaran los datos
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (listUrl, header) {
        console.log(listUrl)
        this.config();
        this.createDatatable(listUrl, header);
    };

    $('#datatable').on('init.dt', function () {
        $('#add-btn')
            .attr('data-toggle', 'modal')
            .attr('data-target', '#edit-modal');
    });

    /**
     * Configura el header del DataTable
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     * @returns 
     */
    obj.dataHeader = function (header) {
        var _header = header;
        head = [];


        var i = 0;
        for (i; i < _header.length; i++) {


            head.push({
                targets: i,
                data: _header[i].FieldName

            })

            // Entra si se desea deshabilitar la columna
            if (_header[i].Visibility == false || _header[i].Visibility != undefined) {
                head[i]['visible'] = false
            }

            // Entra si se desea indicar un ancho especifico
            if (_header[i].Size != undefined) {
                head[i]['width'] = _header[i].Size
            }
            //console.log();
        }
        console.log(head);

        head.push({
            targets: i,
            className: "text-center",
            render: function (data, type, row) {
                botones = "";
                var head = _header[0].FieldName;
                if (type == "display") {
                    //botones += '<button class="btn btn-soft-secondary btn-sm edit-btn ladda-button" data-style="zoom-in" data-id="' + row[head] + '"><span class"ladda-label"><i class="fa-thin fa-pen-to-square"></i></span></button>';
                    //botones += '<button class="btn btn-soft-danger btn-sm ml-1 delete-btn ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '"><span class"ladda-label"><i class="fa-thin fa-trash"></i></span></button>';
                    //botones += '<button class="btn btn-secondary btn-sm" href="/Usuarios/Editar/1"><i class="mdi mdi-square-edit-outline"></i></button>';
                    botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-id="' + row[head] + '" class="bs-tooltip edit-btn" data-toggle="tooltip" data-placement="top" title="" data-original-title="Edit"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';
                    botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '" class="bs-tooltip delete-btn" data-toggle="tooltip" data-placement="top" title="" data-original-title="Delete"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';
                }
                return botones;
            }
        })
        return head;
    };

    //obj.RedirectNew = function (tabla) {
    //    $(function () {
    //        window.location = '/' + tabla + '/Agregar';
    //    })
    //}

    return obj;
}());




$(function () {
    datatable.init();
});



function RedirectEdit(params) {
    window.location = '/' + tabla + '/Edit/' + params + '';
}


