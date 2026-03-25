//==================================================
//Recorre el arreglo de encabezados
//==================================================

var datatablePartials = (function () {
    var obj = {};
    //serverSide

    /**
     * @param {Object} directions Objeto con todas las URLs del controlador
     * @param {any} header
     */
    obj.createDatatable = function (directions, header) {
        var exportOptions = { columns: [0, 1, 2], orthogonal: "export" };
        var table = $('#datatable').DataTable({
            //serverSide: true,
            //responsive: true,
            buttons: [
                //{
                //text: '<i class="mdi mdi-refresh"> Recargar</i>',
                //titleAttr: 'Recargar tabla',
                //action: function (e, dt, config) {
                //    dt.ajax.reload();
                //}
                //},
                {
                    extend: "collection",
                    text: '<i class="mdi mdi-export"> Exportar</i>',
                    titleAttr: 'Exportar esta tabla',
                    buttons: [
                        {
                            extend: "excelHtml5",
                            title: "Exportar a EXCEL",
                            text: "<i class='mdi mdi-file-excel-outline'></i> Excel",
                            class: "btn btn-secondary",
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
                            title: "Exportar a CSV",
                            extend: "csvHtml5",
                            text: "<i class='mdi mdi-file-multiple-outline'></i> CSV",
                            className: "btn-secondary",
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
                var url = directions.listUrl;
                // Si viene un id, agregarlo a la URL
                if (directions.id) {
                    url = url + "/" + directions.id;
                }
                $.ajax({
                    url: url,
                    type: "GET",
                    dataType: "json",
                    success: function (response) {
                        console.log('response' + response);
                        console.log('reponse.data' + response.data);
                        callback(response);
                    },
                }).fail(function (jqXHR, textStatus, error) {
                    console.log("Error en el envio de la peticion de listado " + jqXHR.responseJSON);
                });
            },
            columnDefs: obj.dataHeader(header),
            stateSave: false

        });
    }

    /**
     * Configura el datatable.
     * */
    obj.config = function () {

        //$("input").removeClass(".form-control-sm")
        //configuraciones
        $.extend(true, $.fn.dataTable.defaults, {
            dom:
                "<'row mb-3' <'col-md-3 'B><'col-md-7'f><'col-md-2'l>>" +
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
     * Inicializa el datatable.
     * @param {Object} directions Objeto con todas las URLs del controlador (listUrl, detailsUrl, id, etc.)
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (directions, header) {
        // Guardar las direcciones para uso global
        obj.directions = directions || {};
        this.config();
        this.createDatatable(directions, header);
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
            if (_header[i].Visibility === false) {
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
            className: "text-center action-buttons-cell",
            width: 130,
            render: function (data, type, row) {
                botones = "";
                var head = _header[0].FieldName;
                if (type == "display") {
                    var p = window._currentPantalla || '';
                    var h = window.PetsPermisosHelper;
                    botones += '<div class="action-buttons-wrapper">';
                    // Botón Ver Detalles
                    if (!p || !h || h.consultar(p))
                        botones += '<button class="action-btn btn-view" onclick="viewDetailPartial(' + row[head] + ')" title="Ver detalles"><i class="fas fa-eye"></i></button>';
                    // Botón Editar
                    if (!p || !h || h.editar(p))
                        botones += '<button class="action-btn btn-edit edit-btn" data-id="' + row[head] + '" title="Editar"><i class="fas fa-edit"></i></button>';
                    // Botón Eliminar
                    if (!p || !h || h.eliminar(p))
                        botones += '<button class="action-btn btn-delete delete-btn" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '" title="Eliminar"><i class="fas fa-trash"></i></button>';
                    botones += '</div>';
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

// Note: datatablePartials is initializesd manually from specific pages
// Do not auto-initialize here as it requires specific parameters (listUrl, id, header)



function RedirectEdit(params) {
    window.location = '/' + tabla + '/Edit/' + params + '';
}

// Función global para ver detalles en módulos parciales
function viewDetailPartial(id) {
    // Verificar si datatablePartials.directions tiene detailsUrl configurado
    if (datatablePartials.directions && datatablePartials.directions.detailsUrl) {
        // Si hay detailsUrl, redirigir a la página de detalles
        window.location.href = datatablePartials.directions.detailsUrl + '/' + id;
        return;
    }

    // Fallback: Usar Catalogs para modal (comportamiento anterior)
    if (typeof Catalogs === 'undefined' || (!Catalogs.detailGetUrl && !Catalogs.getUrl)) {
        console.error('datatablePartials.directions.detailsUrl no está configurado y Catalogs tampoco. Configura uno de los dos.');
        console.log('ID solicitado:', id);
        return;
    }

    var detailModalId = Catalogs.detailModalId || '#detail-modal';

    // Verificar si existe el modal de detalles
    if ($(detailModalId).length === 0) {
        console.error('Modal ' + detailModalId + ' no encontrado');
        return;
    }

    // Usar Catalogs.detailGetUrl (preferido) o Catalogs.getUrl como fallback
    var getUrl = Catalogs.detailGetUrl || Catalogs.getUrl;

    console.log('viewDetailPartial llamado con ID:', id);
    console.log('URL a usar:', getUrl);

    $.ajax({
        url: getUrl,
        type: 'GET',
        data: { id: id },
        success: function(response) {
            console.log('Respuesta recibida:', response);
            if (response && response.data) {
                // Llenar dinámicamente el modal con los datos
                fillDetailModal(response.data);

                // Mostrar el modal
                $(detailModalId).modal('show');
            } else {
                console.error('Respuesta sin datos:', response);
            }
        },
        error: function(xhr, status, error) {
            console.error('Error al cargar los detalles:', error);
            console.error('URL llamada:', getUrl + '?id=' + id);
            console.error('Response:', xhr.responseText);
        }
    });
}

// Función auxiliar para llenar el modal con los datos
function fillDetailModal(data) {
    // Recorrer todas las propiedades del objeto data
    for (var key in data) {
        if (data.hasOwnProperty(key)) {
            // Buscar un elemento con el ID correspondiente
            var elementId = '#detail-' + key.toLowerCase().replace(/_/g, '-');
            var $element = $(elementId);

            if ($element.length > 0) {
                var value = data[key];

                // Formatear valores especiales
                if (value === null || value === undefined) {
                    value = 'N/A';
                } else if (typeof value === 'number' && key.toLowerCase().includes('precio')) {
                    value = 'L. ' + parseFloat(value).toFixed(2);
                } else if (key.toLowerCase().includes('fecha') && value) {
                    value = new Date(value).toLocaleDateString('es-HN');
                }

                $element.text(value);
            }
        }
    }
}

