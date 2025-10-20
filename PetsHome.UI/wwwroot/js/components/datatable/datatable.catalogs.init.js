//==================================================
//Recorre el arreglo de encabezados
//==================================================

var datatableCatalogs = (function () {
    var obj = {};
    //serverSide

    /**
     * @param {any} listUrl
     * @param {any} header
     */
    obj.createDatatable = function (listUrl, header) {
        var exportOptions = { columns: [0, 1, 2], orthogonal: "export" };
        $('#datatable').DataTable({
            //serverSide: true,
            //responsive: true,
            buttons: [
                {
                text: '<i class="mdi mdi-refresh"> Recargar</i>',
                titleAttr: 'Recargar tabla',
                action: function (e, dt, config) {
                    dt.ajax.reload();
                }
                },
                {
                    extend: "collection",
                    text: '<i class="mdi mdi-export"> Exportar</i>',
                    titleAttr: 'Exportar esta tabla',
                    buttons: [
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
                        }
                    ]
                },
                {
                    attr: {
                        title: "Añadir nuevo elemento",
                        id: "add-btn",
                        class: "btn btn-primary",
                        'data-style': "zoom-in",
                        'data-toggle': "modal",
                        'data-target': "#edit-modal"
                    },
                    text: '<i class="mdi mdi-plus-thick ladda-button"> Nuevo</i>'
                }
            ],
            ajax: function (data, callback, settings) {
                $.ajax({
                    url: listUrl,
                    type: "GET",
                    dataType: "json",
                    success: function (response) {
                        callback(response);
                    },
                }).fail(function (jqXHR, textStatus, error) {
                    var errorMsg = jqXHR.responseJSON ? JSON.stringify(jqXHR.responseJSON) : (textStatus + ": " + error);
                    console.log("Error en el envio de la peticion de listado: " + errorMsg);
                });
            },
            columnDefs: obj.dataHeader(header)

        });
    }

    /**
     * Configura el datatable.
     * */
    obj.config = function () {

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
    }

    /**
     * Personaliza y mueve los controles del DataTable después de inicializar
     */
    obj.customizeControls = function () {
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
    };

    /**
     * Inicializa el datatable.
     * @param {Object} listUrl Direccion al que se enviaran los datos
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (listUrl, header) {
        this.config();
        this.createDatatable(listUrl, header);
        this.customizeControls();
    };

    $('#datatable').on('init.dt', function () {
        $('#add-btn')
            .attr('data-toggle', 'modal')
            .attr('data-target', '#edit-modal');
    });

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
            if (_header[i].Visibility == false || _header[i].Visibility != undefined) {
                head[i]['visible'] = false
            }

            // Entra si se desea indicar un ancho especifico
            if (_header[i].Size != undefined) {
                head[i]['width'] = _header[i].Size
            }
            //console.log();
        }
        console.log(head);

        head.push({
            targets: i,
            className: "text-center",
            width: 110,
            render: function (data, type, row) {
                botones = "";
                var head = _header[0].FieldName;
                if (type == "display") {
                    // Botón Ver Detalles
                    botones += '<button class="action-btn btn-view" onclick="viewDetailCatalog(' + row[head] + ')" title="Ver detalles"><i class="fas fa-eye"></i></button>';
                    // Botón Editar
                    botones += '<button class="action-btn btn-edit edit-btn" data-id="' + row[head] + '" title="Editar"><i class="fas fa-edit"></i></button>';
                    // Botón Eliminar
                    botones += '<button class="action-btn btn-delete delete-btn" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '" title="Eliminar"><i class="fas fa-trash"></i></button>';
                }
                return botones;
            }
        })
        return head;
    };

    //obj.RedirectNew = function (tabla) {
    //    $(function () {
    //        window.location = '/' + tabla + '/Agregar';
    //    })
    //}

    return obj;
}());




$(function () {
    datatable.init();
});



function RedirectEdit(params) {
    window.location = '/' + tabla + '/Edit/' + params + '';
}

// Función global para ver detalles en catálogos
function viewDetailCatalog(id) {
    // Obtener el controlador actual desde la URL
    var pathArray = window.location.pathname.split('/');
    var controller = pathArray[2] || pathArray[1]; // En catálogos es /Catalogo/NombreCatalogo

    // Redirigir a la página de detalles
    window.location.href = '/Catalogo/' + controller + '/Detail/' + id;
}

