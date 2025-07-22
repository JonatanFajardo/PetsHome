var RecepcionMercancia = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        // Validar que Direction tenga las URLs necesarias
        if (!Direction || !Direction.urlList) {
            console.error('RecepcionMercancia.datatable: Direction.urlList es requerido', Direction);
            return;
        }
        
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'recep_Id',
                    DisplayName: 'ID',
                    Width: '80px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'recep_Fecha',
                    DisplayName: 'Fecha',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (data) {
                            return new Date(data).toLocaleDateString('es-ES');
                        }
                        return '';
                    }
                },
                {
                    FieldName: 'recep_TipoRecepcion',
                    DisplayName: 'Tipo Recepción',
                    Width: '150px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        var color = getTipoRecepcionColor(data);
                        return '<span class="badge badge-' + color + '">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'recep_NumeroDocumento',
                    DisplayName: 'Número Documento',
                    Width: '150px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        return data || 'N/A';
                    }
                },
                {
                    FieldName: 'refg_Nombre',
                    DisplayName: 'Refugio',
                    Width: '200px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'recep_NombreUsuarioCrea',
                    DisplayName: 'Usuario Creación',
                    Width: '180px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'recep_FechaCrea',
                    DisplayName: 'Fecha Creación',
                    Width: '140px',
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
            datatable.init(Direction, header);
        })
    }

    function getTipoRecepcionColor(tipo) {
        switch(tipo) {
            case 'Compra': return 'primary';
            case 'Donación': return 'success';
            case 'Transferencia': return 'info';
            case 'Devolución': return 'warning';
            default: return 'secondary';
        }
    }

    obj.datatablePartials = function (Direction) {
        // Validar que Direction tenga las URLs necesarias
        if (!Direction || !Direction.listUrl) {
            console.error('RecepcionMercancia.datatablePartials: Direction.listUrl es requerido', Direction);
            return;
        }

        // Asegurar que la URL incluya el controlador
        var fullUrl = Direction.listUrl;
        if (!fullUrl.startsWith('/RecepcionDetalle/')) {
            fullUrl = '/RecepcionDetalle' + (fullUrl.startsWith('/') ? fullUrl : '/' + fullUrl);
        }

        console.log('Inicializando datatablePartials con:', Direction);

        // Esperar a que el DOM esté completamente listo
        $(document).ready(function() {
            // Usar setTimeout para asegurar que todos los elementos estén renderizados
            setTimeout(function() {
                // Verificar que el elemento tabla existe y está visible
                var tableElement = $('#datatable');
                if (tableElement.length === 0) {
                    console.error('Elemento #datatable no encontrado en el DOM');
                    return;
                }

                // Verificar que datatablePartials está disponible
                if (typeof datatablePartials === 'undefined') {
                    console.error('datatablePartials no está disponible. Verifique que datatable.partials.init.js esté cargado.');
                    return;
                }

                // Destruir DataTable existente si existe para evitar conflictos
                if ($.fn.DataTable.isDataTable('#datatable')) {
                    tableElement.DataTable().destroy();
                    tableElement.empty();
                }

                var header = [
                    { FieldName: 'itm_Codigo', DisplayName: 'Código' },
                    { FieldName: 'itm_Descripcion', DisplayName: 'Ítem' },
                    { FieldName: 'recdet_Cantidad', DisplayName: 'Cantidad' },
                    { FieldName: 'recdet_PrecioUnitario', DisplayName: 'Precio Unit.' },
                    { FieldName: 'valorTotal', DisplayName: 'Valor Total' },
                    { FieldName: 'recdet_FechaVencimiento', DisplayName: 'Vencimiento' },
                    { FieldName: 'recdet_NumeroLote', DisplayName: 'No. Lote' }
                ];

                try {
                    datatablePartials.initPartials(fullUrl, Direction.id, header);
                    console.log('DataTable inicializado correctamente con URL:', fullUrl);
                } catch (error) {
                    console.error('Error al inicializar DataTable:', error);
                }
            }, 100); // Delay de 100ms para asegurar renderizado completo
        });
    }


    // Función para validar formulario de recepción
    obj.validarFormulario = function() {
        var fecha = $('input[name="recep_Fecha"]').val();
        var tipo = $('select[name="recep_TipoRecepcion"]').val();
        var refugio = $('select[name="refg_Id"]').val();
        var descripcion = $('textarea[name="recep_Descripcion"]').val();

        if (!fecha) {
            alert('La fecha es requerida');
            return false;
        }

        if (!tipo) {
            alert('El tipo de recepción es requerido');
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

    // Función para limpiar formulario
    obj.limpiarFormulario = function() {
        $('input[name="recep_Fecha"]').val('');
        $('select[name="recep_TipoRecepcion"]').val('');
        $('select[name="refg_Id"]').val('');
        $('input[name="recep_NumeroDocumento"]').val('');
        $('input[name="recep_OrigenId"]').val('');
        $('textarea[name="recep_Descripcion"]').val('');
    }

    // Función para autocompletar campos según el tipo de recepción
    obj.configurarTipoRecepcion = function() {
        $('select[name="recep_TipoRecepcion"]').on('change', function() {
            var tipo = $(this).val();
            var numeroDocLabel = $('label[for="recep_NumeroDocumento"]');
            var origenLabel = $('label[for="recep_OrigenId"]');

            switch(tipo) {
                case 'Compra':
                    numeroDocLabel.text('Número de Factura');
                    origenLabel.text('ID Proveedor');
                    break;
                case 'Donación':
                    numeroDocLabel.text('Número de Recibo');
                    origenLabel.text('ID Donante');
                    break;
                case 'Transferencia':
                    numeroDocLabel.text('Número de Transferencia');
                    origenLabel.text('ID Refugio Origen');
                    break;
                default:
                    numeroDocLabel.text('Número de Documento');
                    origenLabel.text('ID Origen');
                    break;
            }
        });
    }

    return obj;

}());