var Empleado = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'emp_Id',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'emp_Codigo',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="font-weight: 500;">' + data + '</span>';
                    }
                },
                {
                    FieldName: 'emp_Nombres',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div class="pet-name">' + data + '</div>';
                    }
                },
                {
                    FieldName: 'cag_Descripcion',
                    Visibility: true
                },
                {
                    FieldName: 'refg_Nombre',
                    Visibility: true
                },
                {
                    FieldName: 'emp_EsActivo',
                    Visibility: true,
                    Render: function(data, type, row) {
                        var estado = data ? 'Activo' : 'Inactivo';
                        var badgeClass = data ? 'status-disponible' : 'status-adoptado';

                        return '<span class="status-badge ' + badgeClass + '">' + estado + '</span>';
                    }
                }
            ];

            // Inicializar datatable
            datatable.init(Direction, header);

            // Mover y personalizar controles después de inicializar
            setTimeout(function () {
                // Ocultar controles por defecto
                $('#datatable_filter').hide();
                $('#datatable_length label').addClass('d-none');

                // Procesar botones de DataTable
                var $dtButtons = $('.dt-buttons');
                if ($dtButtons.length > 0) {
                    // Filtrar solo los botones de exportación (CSV, PDF, Excel)
                    $dtButtons.find('a, button').each(function() {
                        var $btn = $(this);
                        var btnText = $btn.text().trim().toLowerCase();

                        // Si es botón de exportación (CSV, PDF, Excel)
                        if (btnText.includes('csv') || btnText.includes('pdf') || btnText.includes('excel')) {
                            // Remover todas las clases de Bootstrap
                            $btn.removeClass('btn-secondary btn btn-primary');

                            // Agregar clases personalizadas
                            $btn.addClass('btn-export-datatable');

                            // Agregar clase específica según tipo
                            if (btnText.includes('pdf')) {
                                $btn.addClass('btn-export-pdf');
                            } else if (btnText.includes('excel')) {
                                $btn.addClass('btn-export-excel');
                            } else if (btnText.includes('csv')) {
                                $btn.addClass('btn-export-csv');
                            }

                            // Mover al contenedor de exportación
                            $('#export-buttons-container').append($btn);
                        } else {
                            // Ocultar botones que no son de exportación (Recargar, Nuevo)
                            $btn.hide();
                        }
                    });

                    // Ocultar el contenedor original de botones de DataTable
                    $dtButtons.hide();
                }
            }, 100);
        });
    }

    return obj;

}());

// Función global para eliminar empleado
function deleteEmpleado(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}