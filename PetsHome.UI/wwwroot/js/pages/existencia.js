var Existencia = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'exist_Id',
                    DisplayName: 'ID',
                    Width: '80px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'itm_Codigo',
                    DisplayName: 'Código',
                    Width: '120px',
                    Align: 'left',
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
                    FieldName: 'cat_Descripcion',
                    DisplayName: 'Categoría',
                    Width: '150px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'refg_Nombre',
                    DisplayName: 'Refugio',
                    Width: '180px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'exist_Stock',
                    DisplayName: 'Stock Actual',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data, type, row) {
                        var color = getStockColor(data, row.exist_StockMinimo);
                        return '<span class="badge badge-' + color + ' badge-lg">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'exist_StockMinimo',
                    DisplayName: 'Mínimo',
                    Width: '100px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'exist_StockMaximo',
                    DisplayName: 'Máximo',
                    Width: '100px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: null,
                    DisplayName: 'Estado',
                    Width: '130px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: false,
                    Render: function (data, type, row) {
                        var estado = getEstadoStock(row.exist_Stock, row.exist_StockMinimo, row.exist_StockMaximo);
                        var color = getEstadoColor(estado);
                        return '<span class="badge badge-' + color + '">' + estado + '</span>';
                    }
                },
                {
                    FieldName: 'exist_FechaActualizacion',
                    DisplayName: 'Última Actualización',
                    Width: '160px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (data) {
                            return new Date(data).toLocaleDateString('es-ES');
                        }
                        return '';
                    }
                }
            ];
            
            // Agregar columna de acciones
            header.push({
                FieldName: null,
                DisplayName: 'Acciones',
                Width: '120px',
                Align: 'center',
                Visibility: true,
                Sortable: false,
                Render: function (data, type, row) {
                    var actionsHtml = '<div class="btn-group btn-group-sm" role="group">';
                    
                    // Botón ver detalle
                    actionsHtml += '<a href="' + Direction.urlDetail + '/' + row.exist_Id + '" class="btn btn-info btn-sm" title="Ver Detalle">';
                    actionsHtml += '<i class="fas fa-eye"></i></a>';
                    
                    // Botón actualizar stock
                    actionsHtml += '<button type="button" class="btn btn-warning btn-sm" title="Actualizar Stock" ';
                    actionsHtml += 'onclick="openUpdateStockModal(' + row.itm_Id + ',' + row.refg_Id + ',\'' + row.itm_Descripcion + '\',\'' + row.refg_Nombre + '\',' + row.exist_Stock + ')">';
                    actionsHtml += '<i class="fas fa-edit"></i></button>';
                    
                    actionsHtml += '</div>';
                    return actionsHtml;
                }
            });

            datatable.init(Direction, header);
        })
    }

    function getStockColor(stock, minimo) {
        if (stock === 0) return 'danger';
        if (stock <= minimo) return 'warning';
        return 'success';
    }

    function getEstadoStock(stock, minimo, maximo) {
        if (stock === 0) return 'Sin Stock';
        if (stock <= minimo) return 'Stock Bajo';
        if (stock >= maximo) return 'Stock Alto';
        return 'Stock Normal';
    }

    function getEstadoColor(estado) {
        switch(estado) {
            case 'Sin Stock': return 'danger';
            case 'Stock Bajo': return 'warning';
            case 'Stock Alto': return 'info';
            default: return 'success';
        }
    }

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {FieldName: "exist_Id"},
                {FieldName: "itm_Id"},
                {FieldName: "refg_Id"},
                {FieldName: "exist_Stock"}
            ];
            datatable.init(Direction, header);
        })
    }

    // Función para filtrar por estado de stock
    obj.aplicarFiltroEstado = function(estado) {
        var table = $('#datatable').DataTable();
        
        if (estado === 'stockbajo') {
            table.column(8).search('Stock Bajo').draw();
        } else if (estado === 'sinstock') {
            table.column(8).search('Sin Stock').draw();
        } else {
            table.column(8).search('').draw();
        }
    }

    // Función para resaltar filas críticas
    obj.resaltarFilasCriticas = function() {
        $('#datatable tbody tr').each(function() {
            var $row = $(this);
            var estadoBadge = $row.find('td:eq(8) .badge');
            
            if (estadoBadge.hasClass('badge-danger')) {
                $row.addClass('table-danger');
            } else if (estadoBadge.hasClass('badge-warning')) {
                $row.addClass('table-warning');
            }
        });
    }

    // Función para actualizar stock en tiempo real
    obj.actualizarStockTiempoReal = function(itemId, refugioId, nuevoStock) {
        return new Promise(function(resolve, reject) {
            $.ajax({
                url: '/Existencia/UpdateStock',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    ItemId: itemId,
                    RefugioId: refugioId,
                    NuevoStock: nuevoStock
                }),
                success: function(response) {
                    if (response.success) {
                        resolve(response);
                    } else {
                        reject(response.message || 'Error desconocido');
                    }
                },
                error: function(xhr, status, error) {
                    reject('Error de conexión: ' + error);
                }
            });
        });
    }

    // Función para generar alertas de stock
    obj.generarAlertasStock = function() {
        var table = $('#datatable').DataTable();
        var data = table.rows().data();
        
        var sinStock = [];
        var stockBajo = [];
        
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            if (row.exist_Stock === 0) {
                sinStock.push(row);
            } else if (row.exist_Stock <= row.exist_StockMinimo) {
                stockBajo.push(row);
            }
        }
        
        // Mostrar notificaciones
        if (sinStock.length > 0) {
            obj.mostrarNotificacion('Sin Stock', sinStock.length + ' ítems sin stock', 'danger');
        }
        
        if (stockBajo.length > 0) {
            obj.mostrarNotificacion('Stock Bajo', stockBajo.length + ' ítems con stock bajo', 'warning');
        }
    }

    // Función para mostrar notificaciones
    obj.mostrarNotificacion = function(titulo, mensaje, tipo) {
        var alertHtml = '<div class="alert alert-' + tipo + ' alert-dismissible fade show" role="alert">' +
                       '<strong>' + titulo + ':</strong> ' + mensaje +
                       '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
                       '<span aria-hidden="true">&times;</span></button></div>';
        
        $('.container').prepend(alertHtml);
        
        // Auto-ocultar después de 8 segundos
        setTimeout(function() {
            $('.alert').fadeOut();
        }, 8000);
    }

    // Función para exportar datos
    obj.exportarDatos = function(formato) {
        var table = $('#datatable').DataTable();
        
        if (formato === 'excel') {
            // Usar extensión de DataTables para Excel
            table.button('.buttons-excel').trigger();
        } else if (formato === 'pdf') {
            // Usar extensión de DataTables para PDF
            table.button('.buttons-pdf').trigger();
        }
    }

    // Función para calcular métricas de stock
    obj.calcularMetricas = function() {
        var table = $('#datatable').DataTable();
        var data = table.rows().data();
        
        var metricas = {
            totalItems: data.length,
            sinStock: 0,
            stockBajo: 0,
            stockNormal: 0,
            stockAlto: 0,
            valorTotal: 0
        };
        
        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            
            if (row.exist_Stock === 0) {
                metricas.sinStock++;
            } else if (row.exist_Stock <= row.exist_StockMinimo) {
                metricas.stockBajo++;
            } else if (row.exist_Stock >= row.exist_StockMaximo) {
                metricas.stockAlto++;
            } else {
                metricas.stockNormal++;
            }
            
            // Calcular valor total si existe precio
            if (row.itm_Precio) {
                metricas.valorTotal += row.exist_Stock * row.itm_Precio;
            }
        }
        
        return metricas;
    }

    // Función para actualizar dashboard
    obj.actualizarDashboard = function() {
        var metricas = obj.calcularMetricas();
        
        $('#total-items').text(metricas.totalItems);
        $('#items-stock-bajo').text(metricas.stockBajo);
        $('#items-sin-stock').text(metricas.sinStock);
        
        if (metricas.valorTotal > 0) {
            $('#valor-total').text('$' + metricas.valorTotal.toLocaleString('es-CO'));
        }
    }

    // Inicialización automática después de cargar datos
    obj.inicializar = function() {
        // Ejecutar después de que DataTable haya cargado
        $('#datatable').on('draw.dt', function() {
            obj.resaltarFilasCriticas();
            obj.generarAlertasStock();
            obj.actualizarDashboard();
        });
    }

    return obj;

}());