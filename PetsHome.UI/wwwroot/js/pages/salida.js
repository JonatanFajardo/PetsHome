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
                    FieldName: 'refg_Id',
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
                        return data ? new Date(data).toLocaleDateString('es-ES') : '';
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

    return obj;

}());