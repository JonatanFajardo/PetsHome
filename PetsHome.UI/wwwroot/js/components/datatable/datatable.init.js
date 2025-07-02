//==================================================
//Recorre el arreglo de encabezados
//==================================================


var datatable = (function () {
    var obj = {},
        action,
        table,
        $deleteModal;

    //obj.NewModal

    table = $("#datatable");

    function deleteModal(params) {

        $deleteModal = $(params.deleteModalId);
        $deleteModal.on("show.bs.modal", function () {
            var modalTitle = "Eliminar ", deleteBtnText = "Aceptar";
            $(params.deleteModalId + " .modal-title").html(modalTitle + params.displayName);
            $(params.deleteModalId + " .modal-footer .btn-danger").html("<i class='mdi mdi-content-save'></i> " + deleteBtnText);

        });
        $deleteModal.on("hidden.bs.modal", function () { });
    }

    function typeModal(params) {
        switch (params.type) {

            case 'delete':
                $(function () {
                    deleteModal(params);
                    $deleteModal.modal("show");
                });
                break;
        }
    }

    obj.configure = function (params) {
        //if (params.dataTableId === undefined)
        //    params.dataTableId = "#datatable";
        if (params.editModalId === undefined)
            params.editModalId = "#edit-modal";

        if (params.deleteModalId === undefined)
            params.deleteModalId = "#delete-modal-secondary";

        $(function () {
            //createDataTable(params);
            typeModal(params);
            //createModal(params);
            //createEditModal(params);
        });
    };

    obj.begin = function (xhr, settings) {
        if (action == "edit") {
            submitBtn = Ladda.create($(".ladda-button")[0]);
        }
        else {
            submitBtn = Ladda.create($(".ladda-button")[0]);
        }
        submitBtn.start();
    };

    obj.success = function (data, status, xhr) {
        if (data.success) {
            //$editModal.modal("hide");
            $deleteModal.modal("hide");
            /*alertConfig.alert("Success", data.type);*/
            table.DataTable().ajax.reload(null, false);
        }
        else {
            //$editModal.modal("hide");
            $deleteModal.modal("hide");
            alertConfig.alert("Ocurrió  un error.", data.error);
        }
        alertConfig.alert(data.message, data.type);
    };

    obj.failure = function (xhr, status, error) {
        console.log("Ocurrió  un error.");
    }

    obj.complete = function (xhr, status) {
        submitBtn.stop();
    };
    return obj;
}());



var datatable = (function () {
    var obj = {};
    //serverSide



    /**
     * Inicializa y configura el DataTable 
     * @param {Object} DirectionUrls Direccion al que se enviaran los datos
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (DirectionUrls, header) {
        $(function () {

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

            var exportOptions = { columns: [0, 1, 2], orthogonal: "export" };
            var table = $('#datatable').DataTable({
                responsive: true,
                deferRender: true,
                //serverSide: true,
                buttons: [
                    {
                        text: '<i class="mdi mdi-refresh"> Recargar</i>',
                        titleAttr: 'Recargar tabla',
                        action: function (e, dt, config) {
                            dt.ajax.reload();
                        }
                    },
                    {
                        title: "Exportar a CSV",
                        extend: "csvHtml5",
                        text: "<i class='mdi mdi-file-multiple-outline'></i> CSV",
                        className: "btn-secondary",
                        exportOptions: exportOptions
                    },
                    {
                        extend: "pdfHtml5",
                        title: "Exportar a PDF",
                        text: "<i class='mdi mdi-file-pdf-outline'></i> PDF",
                        class: "btn btn-secondary",
                        exportOptions: exportOptions
                    },
                    {
                        extend: "excelHtml5",
                        title: "Exportar a EXCEL",
                        text: "<i class='mdi mdi-file-excel-outline'></i> Excel",
                        class: "btn btn-secondary",
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
                            class: "btn btn-primary",
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
                    // Validaciones previas
                    if (!DirectionUrls || !DirectionUrls.urlList) {
                        console.error('DirectionUrls.urlList no está definido');
                        // Retornar estructura vacía para que DataTable no falle
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
                        return;
                    }

                    console.log('Iniciando petición AJAX a:', DirectionUrls.urlList);
                    console.log('Datos enviados:', data);

                    // Configuración del timeout
                    var ajaxTimeout = 30000; // 30 segundos

                    $.ajax({
                        url: DirectionUrls.urlList,
                        type: "GET",
                        dataType: "json",
                        timeout: ajaxTimeout,
                        data: data, // Enviar los datos de DataTable (paginación, búsqueda, etc.)

                        // Configuraciones adicionales de seguridad
                        cache: false,
                        processData: true,

                        // Headers adicionales si es necesario
                        beforeSend: function (xhr) {
                            // Agregar token CSRF si lo usas
                            // xhr.setRequestHeader('X-CSRF-TOKEN', $('meta[name="csrf-token"]').attr('content'));

                            // Mostrar indicador de carga
                            if (typeof showLoading === 'function') {
                                showLoading();
                            }

                            console.log('Enviando petición AJAX...');
                        },

                        success: function (response, textStatus, jqXHR) {
                            try {
                                console.log('Respuesta recibida:', response);
                                console.log('Status:', textStatus);

                                // Validar estructura de respuesta
                                if (!response) {
                                    throw new Error('Respuesta vacía del servidor');
                                }

                                // Validar que la respuesta tenga la estructura esperada para DataTable
                                var processedResponse = validateAndProcessResponse(response);

                                // Si todo está bien, ejecutar callback
                                callback(processedResponse);

                                console.log('Datos procesados correctamente');

                            } catch (processError) {
                                console.error('Error procesando respuesta:', processError);

                                // Retornar estructura de error para DataTable
                                callback({
                                    data: [],
                                    recordsTotal: 0,
                                    recordsFiltered: 0,
                                    error: 'Error procesando datos: ' + processError.message
                                });

                                // Mostrar error al usuario
                                showUserError('Error al procesar los datos recibidos');

                                // Notificación específica para problemas de procesamiento
                                createCustomNotification('error',
                                    'Error de Procesamiento',
                                    'Los datos recibidos no tienen el formato esperado',
                                    6000
                                );
                            }
                        },

                        error: function (jqXHR, textStatus, errorThrown) {
                            console.error('=== ERROR AJAX ===');
                            console.error('Status:', textStatus);
                            console.error('Error:', errorThrown);
                            console.error('Status Code:', jqXHR.status);
                            console.error('Response Text:', jqXHR.responseText);
                            console.error('URL:', DirectionUrls.urlList);

                            var errorMessage = 'Error desconocido';
                            var userMessage = 'Hubo un problema al cargar los datos';

                            // Manejar diferentes tipos de errores
                            switch (textStatus) {
                                case 'timeout':
                                    errorMessage = 'Timeout - La petición tardó demasiado';
                                    userMessage = 'La carga de datos está tardando más de lo esperado. Intenta nuevamente.';
                                    break;

                                case 'error':
                                    switch (jqXHR.status) {
                                        case 0:
                                            errorMessage = 'Sin conexión - Verifica tu conexión a internet';
                                            userMessage = 'No se pudo conectar al servidor. Verifica tu conexión a internet.';
                                            break;
                                        case 400:
                                            errorMessage = 'Bad Request - Petición inválida';
                                            userMessage = 'Error en la petición. Contacta al administrador.';
                                            break;
                                        case 401:
                                            errorMessage = 'No autorizado - Sesión expirada';
                                            userMessage = 'Tu sesión ha expirado. Por favor, inicia sesión nuevamente.';
                                            // Opcional: redirigir al login
                                            // window.location.href = '/login';
                                            break;
                                        case 403:
                                            errorMessage = 'Acceso denegado - Permisos insuficientes';
                                            userMessage = 'No tienes permisos para ver esta información.';
                                            break;
                                        case 404:
                                            errorMessage = 'No encontrado - URL no existe';
                                            userMessage = 'El recurso solicitado no fue encontrado.';
                                            break;
                                        case 500:
                                            errorMessage = 'Error interno del servidor';
                                            userMessage = 'Error interno del servidor. Contacta al administrador.';
                                            break;
                                        case 502:
                                        case 503:
                                        case 504:
                                            errorMessage = 'Servidor no disponible';
                                            userMessage = 'El servidor no está disponible temporalmente. Intenta más tarde.';
                                            break;
                                        default:
                                            errorMessage = `Error HTTP ${jqXHR.status}: ${errorThrown}`;
                                            userMessage = `Error del servidor (${jqXHR.status}). Contacta al administrador.`;
                                    }
                                    break;

                                case 'abort':
                                    errorMessage = 'Petición cancelada';
                                    userMessage = 'La carga de datos fue cancelada.';
                                    break;

                                case 'parsererror':
                                    errorMessage = 'Error de parsing - Respuesta no es JSON válido';
                                    userMessage = 'Error en el formato de datos recibidos.';
                                    console.error('Response text:', jqXHR.responseText);
                                    break;

                                default:
                                    errorMessage = `Error: ${textStatus} - ${errorThrown}`;
                                    userMessage = 'Error inesperado al cargar los datos.';
                            }

                            console.error('Error final:', errorMessage);

                            // Retornar estructura de error para DataTable
                            callback({
                                data: [],
                                recordsTotal: 0,
                                recordsFiltered: 0,
                                error: errorMessage
                            });

                            // Mostrar error al usuario (múltiples notificaciones)
                            showUserError(userMessage, jqXHR.status);

                            // Mostrar también error en la tabla
                            showTableError(userMessage);

                            // Notificación adicional específica para carga de datos
                            showDataLoadError(errorMessage, jqXHR.status);

                            // Log para análisis (opcional - enviar a servidor de logs)
                            logErrorToServer({
                                url: DirectionUrls.urlList,
                                error: errorMessage,
                                status: jqXHR.status,
                                response: jqXHR.responseText,
                                timestamp: new Date().toISOString()
                            });
                        },

                        complete: function (jqXHR, textStatus) {
                            // Ocultar indicador de carga
                            if (typeof hideLoading === 'function') {
                                hideLoading();
                            }

                            console.log('Petición AJAX completada:', textStatus);
                        }
                    });
                },
                columnDefs: obj.dataHeader(header)

            });

            // Evento click que Redirecciona.
            // Obtiene el id seleccionado en el boton, Redirecciona a la vista de editar.
            table.on("click", ".edit-btn", function (e) {
                var getIdEdit = $(this).data("id");
                console.log(getIdEdit);
                window.location = `${DirectionUrls.urlUpdate}/${getIdEdit}`;
            });

            table.on("click", ".delete-btn-btn", function (e) {
                var getIdDelete = $(this).data("id");
                $("#delete-item-id").val(getIdDelete);
                Modals.configure({
                    displayName: "empleado",
                    type: "delete"
                });
                //$(function () {
                //    Modals.configure({
                //        displayName: "empleado",
                //        type: "delete"
                //    });
                //})
                //var obj = {};

                //obj.Modals = function () {
                //    $(function () {
                //        Modals.configure({
                //            displayName: "empleado",
                //            type: "delete"
                //        });
                //    })
                //}
                //return obj;

            });

            // Evento click de clase .add-btn.
            // Redirecciona a la vista de crear.
            var addBtn = $('#add-btn');
            addBtn.click(function () {
                window.location = `${DirectionUrls.urlInsert}`;
            });



        });


        // Funciones auxiliares para el manejo de errores
        function validateAndProcessResponse(response) {
            // Validar estructura básica
            if (typeof response !== 'object') {
                throw new Error('La respuesta no es un objeto válido');
            }

            // Diferentes formatos de respuesta que puede manejar
            var processedResponse = {
                data: [],
                recordsTotal: 0,
                recordsFiltered: 0
            };

            // Caso 1: Respuesta directa con array de datos
            if (Array.isArray(response)) {
                processedResponse.data = response;
                processedResponse.recordsTotal = response.length;
                processedResponse.recordsFiltered = response.length;
            }
            // Caso 2: Respuesta con estructura DataTable estándar
            else if (response.data && Array.isArray(response.data)) {
                processedResponse.data = response.data;
                processedResponse.recordsTotal = response.recordsTotal || response.data.length;
                processedResponse.recordsFiltered = response.recordsFiltered || response.data.length;
            }
            // Caso 3: Respuesta con diferentes nombres de propiedades
            else if (response.items && Array.isArray(response.items)) {
                processedResponse.data = response.items;
                processedResponse.recordsTotal = response.total || response.items.length;
                processedResponse.recordsFiltered = response.filtered || response.items.length;
            }
            // Caso 4: Error en la respuesta del servidor
            else if (response.error) {
                throw new Error('Error del servidor: ' + response.error);
            }
            // Caso 5: Respuesta inesperada
            else {
                console.warn('Estructura de respuesta inesperada:', response);
                // Intentar extraer datos de cualquier propiedad que sea un array
                var arrayProp = Object.keys(response).find(key => Array.isArray(response[key]));
                if (arrayProp) {
                    processedResponse.data = response[arrayProp];
                    processedResponse.recordsTotal = response[arrayProp].length;
                    processedResponse.recordsFiltered = response[arrayProp].length;
                } else {
                    throw new Error('No se encontraron datos válidos en la respuesta');
                }
            }

            // Validar que cada elemento tenga las propiedades esperadas
            if (processedResponse.data.length > 0) {
                var firstItem = processedResponse.data[0];
                if (typeof firstItem !== 'object') {
                    throw new Error('Los elementos de datos no son objetos válidos');
                }
            }

            return processedResponse;
        }

        function showUserError(message, statusCode) {
            // Prioridad de notificaciones: SweetAlert2 > Toastr > Bootstrap Toast > Alert nativo
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'error',
                    title: 'Error al cargar datos',
                    html: `
                <div class="text-left">
                    <p><strong>Problema:</strong> Ocurrió un problema al cargar los datos</p>
                    <p><strong>Detalle:</strong> ${message}</p>
                    ${statusCode ? `<p><small class="text-muted">Código de error: ${statusCode}</small></p>` : ''}
                </div>
            `,
                    confirmButtonText: 'Reintentar',
                    confirmButtonColor: '#3085d6',
                    showCancelButton: true,
                    cancelButtonText: 'Cerrar',
                    cancelButtonColor: '#d33',
                    width: '500px',
                    customClass: {
                        popup: 'swal-error-popup'
                    }
                }).then((result) => {
                    if (result.isConfirmed) {
                        // Recargar la tabla
                        if (window.dataTable && typeof window.dataTable.ajax.reload === 'function') {
                            window.dataTable.ajax.reload();
                        } else {
                            location.reload();
                        }
                    }
                });
            } else if (typeof toastr !== 'undefined') {
                toastr.error(
                    `Ocurrió un problema al cargar los datos: ${message}`,
                    'Error de Carga',
                    {
                        timeOut: 0,
                        extendedTimeOut: 0,
                        closeButton: true,
                        progressBar: true,
                        positionClass: 'toast-top-right',
                        iconClass: 'toast-error-custom'
                    }
                );
            } else if (typeof bootstrap !== 'undefined' && bootstrap.Toast) {
                // Bootstrap 5 Toast
                var toastId = 'toast-' + Date.now();
                var toastHtml = `
            <div id="${toastId}" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="false">
                <div class="toast-header bg-danger text-white">
                    <svg class="bd-placeholder-img rounded me-2" width="20" height="20" xmlns="http://www.w3.org/2000/svg" aria-hidden="true" preserveAspectRatio="xMidYMid slice" focusable="false">
                        <rect width="100%" height="100%" fill="#dc3545"></rect>
                        <text x="50%" y="50%" fill="white" text-anchor="middle" dy=".3em">⚠</text>
                    </svg>
                    <strong class="me-auto">Error al cargar datos</strong>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body">
                    <strong>Problema:</strong> Ocurrió un problema al cargar los datos<br>
                    <small>${message}</small>
                    ${statusCode ? `<br><small class="text-muted">Código: ${statusCode}</small>` : ''}
                </div>
            </div>
        `;

                // Crear contenedor de toasts si no existe
                if (!$('#toast-container').length) {
                    $('body').append('<div id="toast-container" class="toast-container position-fixed top-0 end-0 p-3"></div>');
                }

                $('#toast-container').append(toastHtml);
                var toast = new bootstrap.Toast($('#' + toastId));
                toast.show();

                // Limpiar toast después de cerrarse
                $('#' + toastId).on('hidden.bs.toast', function () {
                    $(this).remove();
                });

            } else {
                // Fallback a alert nativo con más información
                var alertMessage = `❌ ERROR AL CARGAR DATOS\n\n`;
                alertMessage += `Problema: Ocurrió un problema al cargar los datos\n`;
                alertMessage += `Detalle: ${message}\n`;
                if (statusCode) {
                    alertMessage += `Código de error: ${statusCode}\n`;
                }
                alertMessage += `\n¿Deseas reintentar?`;

                if (confirm(alertMessage)) {
                    // Recargar la tabla
                    if (window.dataTable && typeof window.dataTable.ajax.reload === 'function') {
                        window.dataTable.ajax.reload();
                    } else {
                        location.reload();
                    }
                }
            }
        }

        // Función adicional para mostrar notificación en la tabla misma
        function showTableError(message) {
            // Buscar el contenedor de la tabla
            var tableContainer = $('.dataTables_wrapper');
            if (tableContainer.length) {
                // Remover alertas anteriores
                tableContainer.find('.alert-danger').remove();

                // Crear alerta dentro del contenedor
                var alertHtml = `
            <div class="alert alert-danger alert-dismissible fade show mb-3" role="alert">
                <i class="fas fa-exclamation-triangle me-2"></i>
                <strong>Error al cargar datos:</strong> ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;

                tableContainer.prepend(alertHtml);

                // Auto-remover después de 10 segundos
                setTimeout(function () {
                    tableContainer.find('.alert-danger').fadeOut(500, function () {
                        $(this).remove();
                    });
                }, 10000);
            }
        }

        // Función para mostrar estado de "Sin datos" personalizado
        function showNoDataMessage(tableId, message) {
            var table = $('#' + tableId);
            if (table.length) {
                var noDataHtml = `
            <div class="text-center p-4">
                <div class="mb-3">
                    <i class="fas fa-database fa-3x text-muted"></i>
                </div>
                <h5 class="text-muted">Sin datos disponibles</h5>
                <p class="text-muted">${message || 'No se encontraron registros para mostrar'}</p>
                <button class="btn btn-primary btn-sm" onclick="location.reload()">
                    <i class="fas fa-sync-alt me-1"></i> Recargar
                </button>
            </div>
        `;

                table.find('tbody').html(`<tr><td colspan="100%" class="text-center">${noDataHtml}</td></tr>`);
            }
        }

        function logErrorToServer(errorData) {
            // Opcional: Enviar errores a servidor para análisis
            try {
                $.ajax({
                    url: '/api/log-error', // Ajusta la URL según tu backend
                    type: 'POST',
                    data: JSON.stringify(errorData),
                    contentType: 'application/json',
                    timeout: 5000,
                    success: function () {
                        console.log('Error logged to server');
                    },
                    error: function () {
                        console.warn('Could not log error to server');
                    }
                });
            } catch (e) {
                console.warn('Error logging failed:', e);
            }
        }

        // Funciones de loading (implementa según tu UI)
        function showLoading() {
            // Mostrar spinner o indicador de carga
            $('.dataTables_processing').show();
            // O usar tu sistema de loading personalizado
        }

        function hideLoading() {
            // Ocultar spinner o indicador de carga
            $('.dataTables_processing').hide();
        }

        // Función específica para notificar problemas de carga de datos
        function showDataLoadError(errorMessage, statusCode) {
            // Notificación flotante específica para errores de carga
            if (typeof Swal !== 'undefined') {
                // Toast de SweetAlert2 (menos intrusivo)
                const Toast = Swal.mixin({
                    toast: true,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 8000,
                    timerProgressBar: true,
                    background: '#f8d7da',
                    color: '#721c24',
                    iconColor: '#721c24',
                    didOpen: (toast) => {
                        toast.addEventListener('mouseenter', Swal.stopTimer)
                        toast.addEventListener('mouseleave', Swal.resumeTimer)
                    }
                });

                Toast.fire({
                    icon: 'error',
                    title: '⚠️ Problema al cargar datos',
                    html: `<small>${errorMessage}</small>`
                });

            } else if (typeof toastr !== 'undefined') {
                toastr.warning(
                    `Ocurrió un problema al cargar los datos desde el servidor`,
                    '⚠️ Error de Carga de Datos',
                    {
                        timeOut: 8000,
                        closeButton: true,
                        progressBar: true,
                        positionClass: 'toast-bottom-right',
                        iconClass: 'toast-warning'
                    }
                );
            }
        }

        // Función para crear notificaciones personalizadas en la página
        function createCustomNotification(type, title, message, duration = 5000) {
            // Crear contenedor de notificaciones si no existe
            if (!$('#custom-notifications').length) {
                $('body').append(`
            <div id="custom-notifications" 
                 style="position: fixed; top: 20px; right: 20px; z-index: 9999; width: 350px;">
            </div>
        `);
            }

            var notificationId = 'notification-' + Date.now();
            var iconClass = '';
            var bgClass = '';

            switch (type) {
                case 'error':
                    iconClass = 'fas fa-exclamation-circle';
                    bgClass = 'bg-danger';
                    break;
                case 'warning':
                    iconClass = 'fas fa-exclamation-triangle';
                    bgClass = 'bg-warning';
                    break;
                case 'info':
                    iconClass = 'fas fa-info-circle';
                    bgClass = 'bg-info';
                    break;
                case 'success':
                    iconClass = 'fas fa-check-circle';
                    bgClass = 'bg-success';
                    break;
            }

            var notificationHtml = `
        <div id="${notificationId}" class="alert alert-dismissible fade show ${bgClass} text-white mb-2" 
             style="border-radius: 8px; box-shadow: 0 4px 12px rgba(0,0,0,0.15); animation: slideInRight 0.3s ease-out;">
            <div class="d-flex align-items-start">
                <i class="${iconClass} me-2 mt-1"></i>
                <div class="flex-grow-1">
                    <strong class="d-block">${title}</strong>
                    <small>${message}</small>
                </div>
                <button type="button" class="btn-close btn-close-white ms-2" 
                        onclick="$('#${notificationId}').fadeOut(300, function(){ $(this).remove(); })">
                </button>
            </div>
            <div class="progress mt-2" style="height: 3px;">
                <div class="progress-bar bg-white" style="width: 100%; animation: shrinkWidth ${duration}ms linear;"></div>
            </div>
        </div>
    `;

            $('#custom-notifications').append(notificationHtml);

            // Auto-remover
            setTimeout(function () {
                $('#' + notificationId).fadeOut(300, function () {
                    $(this).remove();
                });
            }, duration);

            return notificationId;
        }


        //Eliminamos la agrupaciond de los botones.
        $(function () {
            $(".dt-buttons").removeClass("btn-group");

        });

    };

    //obj.RedirectNew = function (tabla) {
    //    $(function () {
    //        window.location = '/' + tabla + '/Agregar';

    //    })
    //}


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
            if (header[i].Visibility == false) {
                head[i]['visible'] = "false"
            }
            //if (header[i].Visibility == false || header[i].Visibility !== undefined) {
            //    head[i]['visible'] = false
            //}
            // Entra si se desea indicar un ancho especifico
            if (_header[i].Size != undefined) {
                head[i]['width'] = _header[i].Size
            }
        }

        head.push({
            targets: i,
            className: "text-center",
            width: 80,
            render: function (data, type, row) {
                botones = "";
                var head = _header[0].FieldName;
                if (type == "display") {
                    //botones += '<button class="btn btn-soft-secondary btn-sm edit-btn ladda-button" data-style="zoom-in" data-id="' + row[head] + '"><span class"ladda-label"><i class="fa-thin fa-pen-to-square"></i></span></button>';
                    //botones += '<button class="btn btn-soft-danger btn-sm ml-1 delete-btn-btn ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '"><span class"ladda-label"><i class="fa-thin fa-trash"></i></span></button>';
                    botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-id="' + row[head] + '" class="bs-tooltip edit-btn" data-toggle="tooltip" data-placement="top" title="" data-original-title="Edit"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';
                    botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '" class="bs-tooltip delete-btn" data-toggle="tooltip" data-placement="top" title="" data-original-title="Delete"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';
                }
                return botones;
            }
        })
        return head;
    };




    return obj;
}());


