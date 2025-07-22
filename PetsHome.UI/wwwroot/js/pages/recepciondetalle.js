var RecepcionDetalle = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'itm_Descripcion',
                    DisplayName: 'Ítem',
                    Width: '200px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'recdet_Cantidad',
                    DisplayName: 'Cantidad',
                    Width: '100px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'recdet_PrecioUnitario',
                    DisplayName: 'Precio Unitario',
                    Width: '120px',
                    Align: 'right',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        return '$' + parseFloat(data || 0).toFixed(2);
                    }
                },
                {
                    FieldName: 'valorTotal',
                    DisplayName: 'Valor Total',
                    Width: '120px',
                    Align: 'right',
                    Sortable: true,
                    Render: function (data) {
                        return '$' + parseFloat(data || 0).toFixed(2);
                    }
                },
                {
                    FieldName: 'recdet_FechaVencimiento',
                    DisplayName: 'Fecha Vencimiento',
                    Width: '140px',
                    Align: 'center',
                    
                    Sortable: true,
                    Render: function (data) {
                        return data ? new Date(data).toLocaleDateString('es-ES') : 'N/A';
                    }
                },
                {
                    FieldName: 'recdet_NumeroLote',
                    DisplayName: 'No. Lote',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        return data || 'N/A';
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
    }

    // Función para cargar ítems en dropdown
    obj.loadItems = function(callback) {
        $.get('/RecepcionDetalle/GetItems', function(data) {
            var select = $('#select-item');
            select.empty().append('<option value="">Seleccione un ítem</option>');
            $.each(data, function(index, item) {
                select.append($('<option>').val(item.value).text(item.text));
            });
            if (callback && typeof callback === 'function') {
                callback();
            }
        });
    }

    // Event handlers para formulario de detalles
    obj.initEventHandlers = function() {
        // Calcular total automáticamente cuando cambian cantidad o precio
        $(document).on('input', '#detalle-cantidad, #detalle-precio', function() {
            obj.calculateTotal();
        });

        // Cargar ítems al mostrar modal
        $(document).on('show.bs.modal', '#edit-detalle-modal', function() {
            obj.loadItems();
        });
    }

    return obj;

}());