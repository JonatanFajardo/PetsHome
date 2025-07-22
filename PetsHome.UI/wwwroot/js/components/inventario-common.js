/**
 * Componentes comunes para el sistema de inventario actualizado
 * Funcionalidades compartidas entre Recepciones, Salidas y Existencias
 */
var InventarioCommon = (function () {

    var obj = {};

    // Configuración global
    var config = {
        dateFormat: 'dd/mm/yyyy',
        currency: 'COP',
        locale: 'es-CO'
    };

    /**
     * Inicializa los componentes comunes
     */
    obj.init = function(options) {
        if (options) {
            $.extend(config, options);
        }
        
        obj.initializeDatePickers();
        obj.initializeSelect2();
        obj.initializeValidation();
        obj.initializeTooltips();
    };

    /**
     * Inicializa los date pickers
     */
    obj.initializeDatePickers = function() {
        $('input[type="date"]').each(function() {
            $(this).attr('max', new Date().toISOString().split('T')[0]);
        });
    };

    /**
     * Inicializa Select2 para mejores dropdowns
     */
    obj.initializeSelect2 = function() {
        if (typeof $.fn.select2 !== 'undefined') {
            $('.form-control select').select2({
                theme: 'bootstrap4',
                language: 'es'
            });
        }
    };

    /**
     * Inicializa validación personalizada
     */
    obj.initializeValidation = function() {
        // Validación de números positivos
        $('input[type="number"]').on('input', function() {
            var value = parseFloat($(this).val());
            var min = parseFloat($(this).attr('min')) || 0;
            
            if (value < min) {
                $(this).val(min);
            }
        });

        // Validación de fechas futuras donde no sea permitido
        $('input[type="date"]:not(.allow-future)').on('change', function() {
            var selectedDate = new Date($(this).val());
            var today = new Date();
            today.setHours(23, 59, 59, 999);
            
            if (selectedDate > today) {
                alert('No se permite seleccionar fechas futuras');
                $(this).val('');
            }
        });
    };

    /**
     * Inicializa tooltips
     */
    obj.initializeTooltips = function() {
        $('[title]').tooltip();
        $('[data-toggle="tooltip"]').tooltip();
    };

    /**
     * Formatea números como moneda
     */
    obj.formatCurrency = function(amount) {
        return new Intl.NumberFormat(config.locale, {
            style: 'currency',
            currency: config.currency
        }).format(amount);
    };

    /**
     * Formatea fechas
     */
    obj.formatDate = function(date) {
        if (!date) return '';
        
        var d = new Date(date);
        return d.toLocaleDateString(config.locale);
    };

    /**
     * Formatea fecha y hora
     */
    obj.formatDateTime = function(date) {
        if (!date) return '';
        
        var d = new Date(date);
        return d.toLocaleString(config.locale);
    };

    /**
     * Calcula diferencia de días entre fechas
     */
    obj.daysDifference = function(date1, date2) {
        var d1 = new Date(date1);
        var d2 = new Date(date2);
        var timeDiff = Math.abs(d2.getTime() - d1.getTime());
        return Math.ceil(timeDiff / (1000 * 3600 * 24));
    };

    /**
     * Verifica si una fecha está próxima a vencer (30 días)
     */
    obj.isExpiringSoon = function(expirationDate) {
        if (!expirationDate) return false;
        
        var expDate = new Date(expirationDate);
        var today = new Date();
        var thirtyDaysFromNow = new Date();
        thirtyDaysFromNow.setDate(today.getDate() + 30);
        
        return expDate <= thirtyDaysFromNow && expDate >= today;
    };

    /**
     * Verifica si una fecha está vencida
     */
    obj.isExpired = function(expirationDate) {
        if (!expirationDate) return false;
        
        var expDate = new Date(expirationDate);
        var today = new Date();
        
        return expDate < today;
    };

    /**
     * Obtiene el color del badge según el estado del stock
     */
    obj.getStockBadgeColor = function(stock, minimo, maximo) {
        if (stock === 0) return 'danger';
        if (stock <= minimo) return 'warning';
        if (stock >= maximo) return 'info';
        return 'success';
    };

    /**
     * Obtiene el texto del estado del stock
     */
    obj.getStockStatusText = function(stock, minimo, maximo) {
        if (stock === 0) return 'Sin Stock';
        if (stock <= minimo) return 'Stock Bajo';
        if (stock >= maximo) return 'Stock Alto';
        return 'Stock Normal';
    };

    /**
     * Muestra notificaciones toast
     */
    obj.showToast = function(message, type, title) {
        type = type || 'info';
        title = title || 'Notificación';
        
        var toastHtml = '<div class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-delay="5000">' +
                       '<div class="toast-header bg-' + type + ' text-white">' +
                       '<strong class="mr-auto">' + title + '</strong>' +
                       '<button type="button" class="ml-2 mb-1 close text-white" data-dismiss="toast">&times;</button>' +
                       '</div>' +
                       '<div class="toast-body">' + message + '</div>' +
                       '</div>';
        
        // Crear contenedor si no existe
        if ($('#toast-container').length === 0) {
            $('body').append('<div id="toast-container" class="position-fixed" style="top: 20px; right: 20px; z-index: 1080;"></div>');
        }
        
        $('#toast-container').append(toastHtml);
        $('.toast:last').toast('show');
        
        // Remover después de que se oculte
        $('.toast:last').on('hidden.bs.toast', function() {
            $(this).remove();
        });
    };

    /**
     * Confirma acciones destructivas
     */
    obj.confirmAction = function(message, callback) {
        if (confirm(message)) {
            if (typeof callback === 'function') {
                callback();
            }
            return true;
        }
        return false;
    };

    /**
     * Muestra modal de confirmación personalizado
     */
    obj.showConfirmModal = function(title, message, onConfirm, onCancel) {
        var modalHtml = '<div class="modal fade" id="confirm-modal" tabindex="-1" role="dialog">' +
                       '<div class="modal-dialog" role="document">' +
                       '<div class="modal-content">' +
                       '<div class="modal-header">' +
                       '<h5 class="modal-title">' + title + '</h5>' +
                       '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
                       '</div>' +
                       '<div class="modal-body">' + message + '</div>' +
                       '<div class="modal-footer">' +
                       '<button type="button" class="btn btn-secondary" data-dismiss="modal">Cancelar</button>' +
                       '<button type="button" class="btn btn-primary" id="confirm-action">Confirmar</button>' +
                       '</div></div></div></div>';
        
        // Remover modal anterior si existe
        $('#confirm-modal').remove();
        
        $('body').append(modalHtml);
        
        $('#confirm-action').on('click', function() {
            $('#confirm-modal').modal('hide');
            if (typeof onConfirm === 'function') {
                onConfirm();
            }
        });
        
        $('#confirm-modal').on('hidden.bs.modal', function() {
            $(this).remove();
            if (typeof onCancel === 'function') {
                onCancel();
            }
        });
        
        $('#confirm-modal').modal('show');
    };

    /**
     * Valida si hay conexión a internet
     */
    obj.checkConnection = function(callback) {
        $.ajax({
            url: '/api/ping',
            type: 'GET',
            timeout: 3000,
            success: function() {
                if (callback) callback(true);
            },
            error: function() {
                if (callback) callback(false);
            }
        });
    };

    /**
     * Maneja errores de AJAX de forma centralizada
     */
    obj.handleAjaxError = function(xhr, status, error) {
        var message = 'Error desconocido';
        
        if (xhr.status === 0) {
            message = 'Sin conexión a internet';
        } else if (xhr.status === 404) {
            message = 'Recurso no encontrado';
        } else if (xhr.status === 500) {
            message = 'Error interno del servidor';
        } else if (status === 'timeout') {
            message = 'Tiempo de espera agotado';
        } else {
            message = error || 'Error en la comunicación';
        }
        
        obj.showToast(message, 'danger', 'Error');
    };

    /**
     * Configuración global de AJAX
     */
    obj.setupAjaxDefaults = function() {
        $.ajaxSetup({
            timeout: 30000,
            beforeSend: function() {
                // Mostrar indicador de carga
                if ($('#loading-indicator').length === 0) {
                    $('body').append('<div id="loading-indicator" class="position-fixed" style="top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: 9999;">' +
                                   '<div class="spinner-border text-primary" role="status">' +
                                   '<span class="sr-only">Cargando...</span></div></div>');
                }
            },
            complete: function() {
                // Ocultar indicador de carga
                $('#loading-indicator').remove();
            },
            error: function(xhr, status, error) {
                obj.handleAjaxError(xhr, status, error);
            }
        });
    };

    /**
     * Utilidades para DataTables
     */
    obj.dataTableUtils = {
        // Configuración común de DataTables
        getCommonConfig: function() {
            return {
                responsive: true,
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.10.24/i18n/Spanish.json'
                },
                pageLength: 25,
                lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'Todos']],
                dom: '<"row"<"col-sm-6"l><"col-sm-6"f>>' +
                     '<"row"<"col-sm-12"tr>>' +
                     '<"row"<"col-sm-5"i><"col-sm-7"p>>',
                order: [[0, 'desc']] // Ordenar por primera columna descendente
            };
        },
        
        // Función para exportar tabla
        exportTable: function(tableId, filename, format) {
            var table = $(tableId).DataTable();
            
            if (format === 'excel') {
                table.button('.buttons-excel').trigger();
            } else if (format === 'pdf') {
                table.button('.buttons-pdf').trigger();
            } else if (format === 'csv') {
                table.button('.buttons-csv').trigger();
            }
        }
    };

    /**
     * Utilidades para formularios
     */
    obj.formUtils = {
        // Serializa formulario a objeto JSON
        serializeToJson: function(formSelector) {
            var formData = {};
            $(formSelector).serializeArray().forEach(function(field) {
                formData[field.name] = field.value;
            });
            return formData;
        },
        
        // Limpia formulario
        clearForm: function(formSelector) {
            $(formSelector)[0].reset();
            $(formSelector + ' .is-invalid').removeClass('is-invalid');
            $(formSelector + ' .invalid-feedback').hide();
        },
        
        // Marca campo como inválido
        markFieldInvalid: function(fieldName, message) {
            var field = $('[name="' + fieldName + '"]');
            field.addClass('is-invalid');
            
            var feedback = field.siblings('.invalid-feedback');
            if (feedback.length === 0) {
                field.after('<div class="invalid-feedback"></div>');
                feedback = field.siblings('.invalid-feedback');
            }
            feedback.text(message).show();
        }
    };

    return obj;

}());

// Inicializar automáticamente cuando el documento esté listo
$(document).ready(function() {
    InventarioCommon.init();
    InventarioCommon.setupAjaxDefaults();
});