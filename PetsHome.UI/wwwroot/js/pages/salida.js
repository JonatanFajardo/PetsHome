var Salida = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'sal_Id',
                    DisplayName: 'ID',
                    Width: '80px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'sal_Descripcion',
                    DisplayName: 'Descripción',
                    Width: '250px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'sal_TipoSalida',
                    DisplayName: 'Tipo Salida',
                    Width: '150px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        var color = getTipoSalidaColor(data);
                        return '<span class="badge badge-' + color + '">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'refg_Nombre',
                    DisplayName: 'ID Refugio',
                    Width: '100px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'sal_Fecha',
                    DisplayName: 'Fecha',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (!data) return '';
                        const fecha = new Date(data).toLocaleDateString('es-ES');
                        return `<span class="badge badge-light">${fecha}</span>`;
                    }
                }
            ];

            datatable.init(Direction, header);
        })
    }

    function getTipoSalidaColor(tipo) {
        switch(tipo) {
            case 'Consumo': return 'success';
            case 'Donación': return 'info';
            case 'Transferencia': return 'primary';
            case 'Pérdida': return 'danger';
            case 'Vencimiento': return 'warning';
            case 'Rotura': return 'dark';
            default: return 'secondary';
        }
    }

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {FieldName: "sal_Id"},
                {FieldName: "sal_Fecha"},
                {FieldName: "sal_TipoSalida"},
                {FieldName: "refg_Id"}
            ];
            datatable.init(Direction, header);
        })
    }

    // Función para validar formulario de salida
    obj.validarFormulario = function() {
        var fecha = $('input[name="sal_Fecha"]').val();
        var tipo = $('select[name="sal_TipoSalida"]').val();
        var refugio = $('select[name="refg_Id"]').val();
        var descripcion = $('textarea[name="sal_Descripcion"]').val();

        if (!fecha) {
            alert('La fecha es requerida');
            return false;
        }

        if (!tipo) {
            alert('El tipo de salida es requerido');
            return false;
        }

        if (!refugio) {
            alert('El refugio es requerido');
            return false;
        }

        if (!descripcion.trim()) {
            alert('La descripción es requerida');
            return false;
        }

        return true;
    }

    // Función para verificar stock antes de procesar salida
    obj.verificarStock = function(refugioId, items, callback) {
        if (!refugioId) {
            alert('Seleccione un refugio primero');
            return;
        }

        $.ajax({
            url: '/Salida/VerificarStock',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                RefugioId: refugioId,
                ItemsConCantidades: items
            }),
            success: function(response) {
                if (callback) callback(response.disponible);
            },
            error: function() {
                alert('Error al verificar stock');
                if (callback) callback(false);
            }
        });
    }

    // Función para mostrar alertas según el tipo de salida
    obj.mostrarAlertaTipo = function(tipo) {
        var mensaje = '';
        var alertClass = 'alert-info';

        switch(tipo) {
            case 'Pérdida':
                mensaje = 'Las salidas por pérdida deben ser documentadas apropiadamente para auditoría.';
                alertClass = 'alert-warning';
                break;
            case 'Vencimiento':
                mensaje = 'Asegúrese de verificar las fechas de vencimiento antes de procesar la salida.';
                alertClass = 'alert-warning';
                break;
            case 'Rotura':
                mensaje = 'Documente el motivo de la rotura para análisis de calidad.';
                alertClass = 'alert-danger';
                break;
            case 'Consumo':
                mensaje = 'Verifique que el consumo corresponda a las necesidades reales del refugio.';
                alertClass = 'alert-success';
                break;
            default:
                return;
        }

        // Mostrar alerta temporal
        var alertHtml = '<div class="alert ' + alertClass + ' alert-dismissible fade show mt-2" role="alert">' +
                       mensaje +
                       '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
                       '<span aria-hidden="true">&times;</span></button></div>';
        
        $('select[name="sal_TipoSalida"]').after(alertHtml);
        
        // Auto-ocultar después de 5 segundos
        setTimeout(function() {
            $('.alert').fadeOut();
        }, 5000);
    }

    // Función para configurar comportamiento según tipo de salida
    obj.configurarTipoSalida = function() {
        $('select[name="sal_TipoSalida"]').on('change', function() {
            var tipo = $(this).val();
            
            // Limpiar alertas anteriores
            $('.alert').remove();
            
            // Mostrar alerta específica
            obj.mostrarAlertaTipo(tipo);
            
            // Configurar etiquetas dinámicas
            var numeroDocLabel = $('label[for="sal_NumeroDocumento"]');
            var destinoLabel = $('label[for="sal_DestinoId"]');

            switch(tipo) {
                case 'Consumo':
                    numeroDocLabel.text('Vale de Consumo');
                    destinoLabel.text('ID Área de Consumo');
                    break;
                case 'Donación':
                    numeroDocLabel.text('Recibo de Donación');
                    destinoLabel.text('ID Beneficiario');
                    break;
                case 'Transferencia':
                    numeroDocLabel.text('Orden de Transferencia');
                    destinoLabel.text('ID Refugio Destino');
                    break;
                case 'Pérdida':
                    numeroDocLabel.text('Reporte de Pérdida');
                    destinoLabel.text('ID Área');
                    break;
                case 'Vencimiento':
                    numeroDocLabel.text('Acta de Vencimiento');
                    destinoLabel.text('ID Área Disposición');
                    break;
                case 'Rotura':
                    numeroDocLabel.text('Reporte de Rotura');
                    destinoLabel.text('ID Área');
                    break;
                default:
                    numeroDocLabel.text('Número de Documento');
                    destinoLabel.text('ID Destino');
                    break;
            }
        });
    }

    // Función para confirmar salidas críticas
    obj.confirmarSalidaCritica = function(tipo) {
        var tiposCriticos = ['Pérdida', 'Vencimiento', 'Rotura'];
        
        if (tiposCriticos.includes(tipo)) {
            return confirm('¿Está seguro de procesar esta salida por ' + tipo.toLowerCase() + '? Esta acción afectará el inventario permanentemente.');
        }
        
        return true;
    }

    // Función para limpiar formulario
    obj.limpiarFormulario = function() {
        $('input[name="sal_Fecha"]').val('');
        $('select[name="sal_TipoSalida"]').val('');
        $('select[name="refg_Id"]').val('');
        $('input[name="sal_NumeroDocumento"]').val('');
        $('input[name="sal_DestinoId"]').val('');
        $('textarea[name="sal_Descripcion"]').val('');
        $('.alert').remove();
    }

    // Función para inicializar la tabla de detalles
    obj.initDetallesTable = function(config) {
        if (!config || !config.urlList) {
            console.error('Salida.initDetallesTable: config.urlList es requerido');
            return;
        }

        // Configurar la tabla de detalles
        var tableConfig = {
            urlList: config.urlList,
            urlDetail: '',
            urlInsert: '',
            urlUpdate: ''
        };

        var header = [
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
                Align: 'right',
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
                    return '₡' + parseFloat(data).toLocaleString('es-CR', {minimumFractionDigits: 2});
                }
            },
            {
                FieldName: 'ValorTotal',
                DisplayName: 'Valor Total',
                Width: '120px',
                Align: 'right',
                Visibility: true,
                Sortable: true,
                Render: function (data, type, row) {
                    var total = row.saldet_Cantidad * row.saldet_PrecioUnitario;
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
                Align: 'right',
                Visibility: true,
                Sortable: true,
                Render: function (data, type, row) {
                    var stock = row.StockDisponible || 0;
                    var color = stock > 10 ? 'success' : stock > 0 ? 'warning' : 'danger';
                    return '<span class="badge badge-' + color + '">' + stock + '</span>';
                }
            }
        ];

        // Inicializar DataTable para detalles
        $('#datatable-detalles').DataTable({
            ajax: {
                url: config.urlList,
                type: 'GET',
                dataSrc: 'data'
            },
            columns: header.map(function(col) {
                return {
                    data: col.FieldName,
                    name: col.FieldName,
                    title: col.DisplayName,
                    width: col.Width,
                    className: 'text-' + col.Align,
                    orderable: col.Sortable,
                    render: col.Render
                };
            }).concat([{
                data: null,
                title: 'Acciones',
                width: '120px',
                className: 'text-center',
                orderable: false,
                render: function (data, type, row) {
                    return '<button class="btn btn-sm btn-primary edit-detalle-btn" data-id="' + row.saldet_Id + '">' +
                           '<i class="mdi mdi-pencil"></i></button> ' +
                           '<button class="btn btn-sm btn-danger delete-detalle-btn" data-id="' + row.saldet_Id + '">' +
                           '<i class="mdi mdi-delete"></i></button>';
                }
            }]),
            responsive: true,
            language: {
                url: '//cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json'
            }
        });

        // Event handlers para botones de acción
        $(document).on('click', '#add-detalle-btn', function() {
            obj.showDetalleModal();
        });

        $(document).on('click', '.edit-detalle-btn', function() {
            var id = $(this).data('id');
            obj.showDetalleModal(id);
        });

        $(document).on('click', '.delete-detalle-btn', function() {
            var id = $(this).data('id');
            obj.showDeleteDetalleModal(id);
        });

        // Cargar ítems para el dropdown
        if (config.urlGetItems) {
            obj.loadItems(config.urlGetItems);
        }

        // Event handlers para el modal de detalles
        obj.setupDetalleModalEvents();
    };

    // Función para mostrar modal de detalle
    obj.showDetalleModal = function(detalleId) {
        detalleId = detalleId || 0;
        
        if (detalleId === 0) {
            // Nuevo detalle
            $('#edit-detalle-modal .modal-title').text('Agregar Detalle de Salida');
            $('#edit-detalle-modal button[type="submit"]').text('Agregar');
            obj.clearDetalleForm();
        } else {
            // Editar detalle
            $('#edit-detalle-modal .modal-title').text('Editar Detalle de Salida');
            $('#edit-detalle-modal button[type="submit"]').text('Actualizar');
            obj.loadDetalleData(detalleId);
        }
        
        $('#edit-detalle-modal').modal('show');
    };

    // Función para mostrar modal de eliminar detalle
    obj.showDeleteDetalleModal = function(detalleId) {
        $('#delete-detalle-id').val(detalleId);
        $('#delete-detalle-modal .modal-title').text('Eliminar Detalle de Salida');
        $('#delete-detalle-modal').modal('show');
    };

    // Función para limpiar formulario de detalle
    obj.clearDetalleForm = function() {
        $('#detalle-id').val(0);
        $('#select-item').val('');
        $('#detalle-cantidad').val('');
        $('#detalle-precio').val('');
        $('#detalle-motivo').val('');
        $('#detalle-stock').val('');
        $('#detalle-total').val('');
        $('#is-edit').val(false);
        $('#stock-warning').addClass('d-none');
    };

    // Función para cargar datos del detalle
    obj.loadDetalleData = function(detalleId) {
        // Implementar carga de datos del detalle
        console.log('Cargando detalle:', detalleId);
    };

    // Función para cargar ítems
    obj.loadItems = function(url) {
        $.get(url).done(function(data) {
            var select = $('#select-item');
            select.empty();
            select.append('<option value="">Seleccione un ítem</option>');
            
            $.each(data, function(index, item) {
                select.append('<option value="' + item.itm_Id + '">' + item.itm_Descripcion + '</option>');
            });
        });
    };

    // Función para configurar eventos del modal de detalles
    obj.setupDetalleModalEvents = function() {
        // Calcular total automáticamente
        $(document).on('input', '#detalle-cantidad, #detalle-precio', function() {
            obj.calculateTotal();
        });

        // Validar stock al seleccionar ítem o cambiar cantidad
        $(document).on('change', '#select-item', function() {
            obj.checkItemStock();
        });

        $(document).on('input', '#detalle-cantidad', function() {
            obj.validateStock();
        });
    };

    // Función para calcular total
    obj.calculateTotal = function() {
        var cantidad = parseFloat($('#detalle-cantidad').val()) || 0;
        var precio = parseFloat($('#detalle-precio').val()) || 0;
        var total = cantidad * precio;
        
        $('#detalle-total').val(total.toFixed(2));
    };

    // Función para verificar stock del ítem
    obj.checkItemStock = function() {
        var itemId = $('#select-item').val();
        if (itemId) {
            // Implementar verificación de stock
            console.log('Verificando stock para ítem:', itemId);
        }
    };

    // Función para validar stock
    obj.validateStock = function() {
        var cantidad = parseInt($('#detalle-cantidad').val()) || 0;
        var stock = parseInt($('#detalle-stock').val()) || 0;
        
        if (cantidad > stock) {
            $('#stock-warning').removeClass('d-none');
        } else {
            $('#stock-warning').addClass('d-none');
        }
    };

    return obj;

}());