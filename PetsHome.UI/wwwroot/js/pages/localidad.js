var Localidad = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'depto_Id'},
                { FieldName: 'depto_Codigo'},
                { FieldName: 'depto_Descripcion'}
            ];
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
        })
    }
    return obj;

}());
