var SalidaDetalle = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'itm_Codigo',
                    DisplayName: 'Código',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'itm_Descripcion',
                    DisplayName: 'Ítem',
                    Width: '200px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'saldet_Cantidad',
                    DisplayName: 'Cantidad',
                    Width: '100px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'saldet_PrecioUnitario',
                    DisplayName: 'Precio Unitario',
                    Width: '120px',
                    Align: 'right',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        return '₡' + parseFloat(data || 0).toLocaleString('es-CR', {minimumFractionDigits: 2});
                    }
                },
                {
                    FieldName: 'ValorTotal',
                    DisplayName: 'Valor Total',
                    Width: '120px',
                    Align: 'right',
                    Sortable: true,
                    Render: function (data, type, row) {
                        var total = (row.saldet_Cantidad || 0) * (row.saldet_PrecioUnitario || 0);
                        return '₡' + total.toLocaleString('es-CR', {minimumFractionDigits: 2});
                    }
                },
                {
                    FieldName: 'saldet_Motivo',
                    DisplayName: 'Motivo',
                    Width: '150px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        return data || 'N/A';
                    }
                },
                {
                    FieldName: 'StockDisponible',
                    DisplayName: 'Stock Disponible',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        var stock = data || 0;
                        var color = stock > 10 ? 'success' : stock > 0 ? 'warning' : 'danger';
                        return '<span class="badge badge-' + color + '">' + stock + '</span>';
                    }
                }
            ];
            datatablePartials.initPartials(Direction.listUrl, Direction.id, header);
        })
    }

    // Función para calcular total automáticamente
    obj.calculateTotal = function() {
        var cantidad = parseFloat($('#detalle-cantidad').val()) || 0;
        var precio = parseFloat($('#detalle-precio').val()) || 0;
        var total = cantidad * precio;
        $('#detalle-total').val(total.toFixed(2));
        
        // Mostrar el total formateado en el campo de solo lectura
        $('#detalle-total-display').text('₡' + total.toLocaleString('es-CR', {minimumFractionDigits: 2}));
    }

    // Función para cargar ítems en dropdown
    obj.loadItems = function(callback) {
        $.get('/SalidaDetalle/GetItems', function(data) {
            var select = $('#select-item');
            select.empty().append('<option value="">Seleccione un ítem</option>');
            $.each(data, function(index, item) {
                select.append($('<option>').val(item.value).text(item.text).data('codigo', item.codigo));
            });
            if (callback && typeof callback === 'function') {
                callback();
            }
        }).fail(function() {
            console.error('Error al cargar ítems');
        });
    }

    // Función para obtener stock disponible de un ítem
    obj.getItemStock = function(itemId, refugioId) {
        if (!itemId || !refugioId) return;
        
        $.get('/SalidaDetalle/GetStockDisponible', {
            itemId: itemId,
            refugioId: refugioId
        }, function(data) {
            $('#detalle-stock').val(data.stock || 0);
            $('#stock-disponible-display').text(data.stock || 0);
            obj.validateStock();
        }).fail(function() {
            $('#detalle-stock').val(0);
            $('#stock-disponible-display').text('0');
            console.error('Error al obtener stock del ítem');
        });
    }

    // Función para validar stock disponible
    obj.validateStock = function() {
        var cantidad = parseInt($('#detalle-cantidad').val()) || 0;
        var stock = parseInt($('#detalle-stock').val()) || 0;
        var warning = $('#stock-warning');
        var submitBtn = $('#submit-detalle-btn');
        
        if (cantidad > stock) {
            warning.removeClass('d-none');
            warning.find('.stock-cantidad').text(cantidad);
            warning.find('.stock-disponible').text(stock);
            submitBtn.prop('disabled', true);
        } else {
            warning.addClass('d-none');
            submitBtn.prop('disabled', false);
        }
    }

    // Función para validar formulario antes de enviar
    obj.validateForm = function() {
        var itemId = $('#select-item').val();
        var cantidad = parseInt($('#detalle-cantidad').val()) || 0;
        var precio = parseFloat($('#detalle-precio').val()) || 0;
        
        if (!itemId) {
            alert('Debe seleccionar un ítem');
            return false;
        }
        
        if (cantidad <= 0) {
            alert('La cantidad debe ser mayor a 0');
            return false;
        }
        
        if (precio <= 0) {
            alert('El precio unitario debe ser mayor a 0');
            return false;
        }
        
        var stock = parseInt($('#detalle-stock').val()) || 0;
        if (cantidad > stock) {
            alert('La cantidad no puede ser mayor al stock disponible (' + stock + ')');
            return false;
        }
        
        return true;
    }

    // Función para limpiar formulario
    obj.clearForm = function() {
        $('#detalle-id').val(0);
        $('#select-item').val('');
        $('#detalle-cantidad').val('');
        $('#detalle-precio').val('');
        $('#detalle-motivo').val('');
        $('#detalle-stock').val('');
        $('#detalle-total').val('');
        $('#detalle-total-display').text('₡0.00');
        $('#stock-disponible-display').text('0');
        $('#is-edit').val(false);
        $('#stock-warning').addClass('d-none');
        $('#submit-detalle-btn').prop('disabled', false);
    }

    // Función para cargar datos del detalle en modo edición
    obj.loadDetalleData = function(detalleId) {
        if (!detalleId) return;
        
        $.get('/SalidaDetalle/FindDetalle', { id: detalleId }, function(response) {
            if (response.success && response.item) {
                var detalle = response.item;
                $('#detalle-id').val(detalle.saldet_Id);
                $('#select-item').val(detalle.itm_Id);
                $('#detalle-cantidad').val(detalle.saldet_Cantidad);
                $('#detalle-precio').val(detalle.saldet_PrecioUnitario);
                $('#detalle-motivo').val(detalle.saldet_Motivo);
                $('#detalle-stock').val(detalle.StockDisponible);
                $('#stock-disponible-display').text(detalle.StockDisponible);
                $('#is-edit').val(true);
                
                obj.calculateTotal();
                obj.validateStock();
            } else {
                alert('Error al cargar los datos del detalle');
            }
        }).fail(function() {
            alert('Error al comunicarse con el servidor');
        });
    }

    // Event handlers para formulario de detalles
    obj.initEventHandlers = function() {
        // Calcular total automáticamente cuando cambian cantidad o precio
        $(document).on('input', '#detalle-cantidad, #detalle-precio', function() {
            obj.calculateTotal();
        });

        // Validar stock cuando cambia la cantidad
        $(document).on('input', '#detalle-cantidad', function() {
            obj.validateStock();
        });

        // Cargar stock cuando se selecciona un ítem
        $(document).on('change', '#select-item', function() {
            var itemId = $(this).val();
            var refugioId = $('#refugio-id').val(); // Debe estar disponible en la vista
            
            if (itemId && refugioId) {
                obj.getItemStock(itemId, refugioId);
            } else {
                $('#detalle-stock').val(0);
                $('#stock-disponible-display').text('0');
                obj.validateStock();
            }
        });

        // Cargar ítems al mostrar modal
        $(document).on('show.bs.modal', '#edit-detalle-modal', function() {
            obj.loadItems();
        });

        // Limpiar formulario al cerrar modal
        $(document).on('hidden.bs.modal', '#edit-detalle-modal', function() {
            obj.clearForm();
        });

        // Validar formulario antes de enviar
        $(document).on('submit', '#detalle-form', function(e) {
            if (!obj.validateForm()) {
                e.preventDefault();
                return false;
            }
        });

        // Botón para agregar nuevo detalle
        $(document).on('click', '#add-detalle-btn', function() {
            $('#edit-detalle-modal .modal-title').text('Agregar Detalle de Salida');
            $('#submit-detalle-btn').text('Agregar');
            obj.clearForm();
            $('#edit-detalle-modal').modal('show');
        });

        // Botones de editar en la tabla
        $(document).on('click', '.edit-detalle-btn', function() {
            var detalleId = $(this).data('id');
            $('#edit-detalle-modal .modal-title').text('Editar Detalle de Salida');
            $('#submit-detalle-btn').text('Actualizar');
            obj.loadDetalleData(detalleId);
            $('#edit-detalle-modal').modal('show');
        });

        // Botones de eliminar en la tabla
        $(document).on('click', '.delete-detalle-btn', function() {
            var detalleId = $(this).data('id');
            if (confirm('¿Está seguro de eliminar este detalle de salida?')) {
                obj.deleteDetalle(detalleId);
            }
        });
    }

    // Función para eliminar detalle
    obj.deleteDetalle = function(detalleId) {
        var salidaId = $('#salida-id').val(); // Debe estar disponible en la vista
        
        $.post('/SalidaDetalle/RemoveDetalle', {
            saldet_Id: detalleId,
            sal_Id: salidaId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        }, function() {
            // Recargar la tabla de detalles
            if (typeof datatablePartials !== 'undefined' && datatablePartials.table) {
                datatablePartials.table.ajax.reload();
            }
            
            // Mostrar mensaje de éxito
            console.log('Detalle eliminado correctamente');
        }).fail(function() {
            alert('Error al eliminar el detalle');
        });
    }

    // Función para inicializar todos los event handlers
    obj.init = function() {
        obj.initEventHandlers();
    }

    return obj;

}());