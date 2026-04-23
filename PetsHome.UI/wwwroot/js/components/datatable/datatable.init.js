//==================================================
//Recorre el arreglo de encabezados
//==================================================


var datatable = (function () {
    var obj = {},
        action,
        table,
        $deleteModal;

    //obj.NewModal

    table = $("#datatable");

    function deleteModal(params) {

        $deleteModal = $(params.deleteModalId);
        $deleteModal.on("show.bs.modal", function () {
            var modalTitle = "Eliminar ", deleteBtnText = "Aceptar";
            $(params.deleteModalId + " .modal-title").html(modalTitle + params.displayName);
            $(params.deleteModalId + " .modal-footer .btn-danger").html("<i class='mdi mdi-content-save'></i> " + deleteBtnText);

        });
        $deleteModal.on("hidden.bs.modal", function () { });
    }

    function typeModal(params) {
        switch (params.type) {

            case 'delete':
                $(function () {
                    deleteModal(params);
                    $deleteModal.modal("show");
                });
                break;
        }
    }

    obj.configure = function (params) {
        //if (params.dataTableId === undefined)
        //    params.dataTableId = "#datatable";
        if (params.editModalId === undefined)
            params.editModalId = "#edit-modal";

        if (params.deleteModalId === undefined)
            params.deleteModalId = "#delete-modal-secondary";

        $(function () {
            //createDataTable(params);
            typeModal(params);
            //createModal(params);
            //createEditModal(params);
        });
    };

    obj.begin = function (xhr, settings) {
        if (action == "edit") {
            submitBtn = Ladda.create($(".ladda-button")[0]);
        }
        else {
            submitBtn = Ladda.create($(".ladda-button")[0]);
        }
        submitBtn.start();
    };

    obj.success = function (data, status, xhr) {
        if (data.success) {
            //$editModal.modal("hide");
            $deleteModal.modal("hide");
            /*alertConfig.alert("Success", data.type);*/
            table.DataTable().ajax.reload(null, false);
        }
        else {
            //$editModal.modal("hide");
            $deleteModal.modal("hide");
            alertConfig.alert("Ocurrió  un error.", data.error);
        }
        alertConfig.alert(data.message, data.type);
    };

    obj.failure = function (xhr, status, error) {
        console.log("Ocurrió  un error.");
    }

    obj.complete = function (xhr, status) {
        submitBtn.stop();
    };
    return obj;
}());



var datatable = (function () {
    var obj = {};
    //serverSide



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
     * Inicializa y configura el DataTable
     * @param {Object} DirectionUrls Direccion al que se enviaran los datos
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (DirectionUrls, header) {
        $(function () {

            //configuraciones
            $.extend(true, $.fn.dataTable.defaults, {
                dom:
                    "<'row' <'col-md-4 'B><'col-md-6'f><'col-md-2'l>>" +
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
                    info: "Mostrando _START_ de _TOTAL_ registros",
                    infoEmpty: "Mostrando 0 al 0 de un total de 0 registros",
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


            var exportOptions = { columns: [0, 1, 2], orthogonal: "export" };
            var table = $('#datatable').DataTable({
                responsive: true,
                deferRender:true,
                //serverSide: true,
                buttons: [
                    {
                        text: '<i class="mdi mdi-refresh"></i>',
                        titleAttr: 'Recargar tabla',
                        className: "btn btn-secondary",
                        action: function (e, dt, config) {
                            dt.ajax.reload();
                        }
                    },
                    {
                        extend: "excelHtml5",
                        title: "Exportar a EXCEL",
                        text: "<i class='mdi mdi-file-excel-outline'></i> Excel",
                        className: "btn btn-secondary",
                        exportOptions: exportOptions
                    },
                    {
                        extend: "pdfHtml5",
                        title: "Exportar a PDF",
                        text: "<i class='mdi mdi-file-pdf-outline'></i> PDF",
                        className: "btn btn-secondary",
                        exportOptions: exportOptions
                    },
                    {
                        extend: "csvHtml5",
                        title: "Exportar a CSV",
                        text: "<i class='mdi mdi-file-multiple-outline'></i> CSV",
                        className: "btn btn-secondary",
                        exportOptions: exportOptions
                    },
                    {

                        //attr: 
                        //    'data-toggle': "modal",
                        //    'data-target': "#edit-modal"
                        //},
                        //text: 'Nuevo',
                        //class: "btn btn-primary",
                        //attr: {
                        //    title: "Añadir nuevo elemento",
                        //    id: "add-btn",
                        //    //onclick: "nuevo()",
                        //    'data-toggle': "modal",
                        //    'data-target': "#edit-modal"
                        //    /*onclick: "nuevo()"*/
                        //}
                        attr: {
                            title: "Añadir nuevo elemento",
                            id: "add-btn",
                            class: "btn btn-primary",
                            'data-style': "zoom-in",
                            'data-toggle': "modal",
                            'data-target': "#edit-modal"
                        },
                        text: '<i class="mdi mdi-plus-thick ladda-button"> Nuevo</i>'


                        //text: 'Nuevo <i class="mdi mdi-plus-thick"></i>',
                        //className: "btn btn-success",
                        //attr: {
                        //    title: "Añadir componente nuevo",
                        //    onclick: "Modal()"
                        //}

                    }
                ],
                ajax: function (data, callback, settings) {
                    $.ajax({
                        url: DirectionUrls.urlList,
                        type: "GET",
                        dataType: "json",
                        success: function (response) {
                            callback(response);
                        },
                        error: function (jqXHR, textStatus) {
                            console.log('error:' + textStatus);
                        }
                    });
                },
                columnDefs: obj.dataHeader(header),
                stateSave: false

            });

            // Evento click que Redirecciona.
            // Obtiene el id seleccionado en el boton, Redirecciona a la vista de editar.
            table.on("click", ".edit-btn", function (e) {
                var getIdEdit = $(this).data("id");
                console.log(getIdEdit);
                window.location = `${DirectionUrls.urlUpdate}/${getIdEdit}`;
            });

            table.on("click", ".delete-btn-btn", function (e) {
                var getIdDelete = $(this).data("id");
                $("#delete-item-id").val(getIdDelete);
                Modals.configure({
                    displayName: "empleado",
                    type: "delete"
                });
                //$(function () {
                //    Modals.configure({
                //        displayName: "empleado",
                //        type: "delete"
                //    });
                //})
                //var obj = {};

                //obj.Modals = function () {
                //    $(function () {
                //        Modals.configure({
                //            displayName: "empleado",
                //            type: "delete"
                //        });
                //    })
                //}
                //return obj;

            });

            // Evento click de clase .add-btn.
            // Redirecciona a la vista de crear.
            var addBtn = $('#add-btn');
            addBtn.click(function () {
                window.location = `${DirectionUrls.urlInsert}`;
            });

            // Ocultar botón Nuevo si no tiene permiso de insertar
            var p = window._currentPantalla || '';
            var h = window.PetsPermisosHelper;
            if (p && h && !h.insertar(p)) {
                addBtn.hide();
                $('.btn-nueva-mascota, #add-btn-header').hide();
            }



        });

        //Eliminamos la agrupaciond de los botones.
        $(function () {
            $(".dt-buttons").removeClass("btn-group");

        });

        // Aplicar personalización de controles
        this.customizeControls();

    };

    //obj.RedirectNew = function (tabla) {
    //    $(function () {
    //        window.location = '/' + tabla + '/Agregar';

    //    })
    //}


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
            if (header[i].Visibility == false) {
                head[i]['visible'] = false
            }
            //if (header[i].Visibility == false || header[i].Visibility !== undefined) {
            //    head[i]['visible'] = false
            //}
            // Entra si se desea indicar un ancho especifico
            if (_header[i].Size != undefined) {
                head[i]['width'] = _header[i].Size
            }
            // Entra si se desea personalizar el render de la columna
            if (_header[i].render != undefined) {
                head[i]['render'] = _header[i].render
            }
        }

        head.push({
            targets: i,
            className: "text-center action-buttons-cell",
            width: 130,
            render: function (data, type, row) {
                botones = "";
                var head = _header[0].FieldName;
                var p = window._currentPantalla || '';
                var h = window.PetsPermisosHelper;
                if (type == "display") {
                    botones += '<div class="action-buttons-wrapper">';
                    // Botón Ver Detalles
                    if (!p || !h || h.consultar(p))
                        botones += '<button class="action-btn btn-view" onclick="viewDetail(' + row[head] + ')" title="Ver detalles"><i class="fas fa-eye"></i></button>';
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




    return obj;
}());

// Función global para ver detalles
// Esta función debe ser sobrescrita en cada página específica
function viewDetail(id) {
    // Obtener el controlador actual desde la URL
    var pathArray = window.location.pathname.split('/');
    var controller = pathArray[1] || 'Home';

    // Redirigir a la página de detalles
    window.location.href = '/' + controller + '/Detail/' + id;
}

