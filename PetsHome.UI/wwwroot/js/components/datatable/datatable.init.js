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
        //console.log(data);
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
     * Inicializa y configura el DataTable 
     * @param {Object} DirectionUrls Direccion al que se enviaran los datos
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (DirectionUrls, header) {
        $(function () {


            //configuraciones
            $.extend(true, $.fn.dataTable.defaults, {
                //dom: "<'row mb-3'<'col-md-7'f> <'col-md-5 d-flex justify-content-end custom-buttons'B>>" +
                //    "<'row'<'col-sm-12'tr>>" +
                //    "<'row'<'col-sm-5'i><'col-sm-7'p>>",
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


            var exportOptions = { columns: [0, 1, 2], orthogonal: "export" };
            var table = $('#datatable').DataTable({
                responsive: true,
                //serverSide: true,
                buttons: [
                    //{
                    //text: '<i class="mdi mdi-refresh"> Recargar</i>',
                    //titleAttr: 'Recargar tabla',
                    //action: function (e, dt, config) {
                    //    dt.ajax.reload();
                    //}
                    //},
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
                columnDefs: obj.dataHeader(header)

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



        });

        //Eliminamos la agrupaciond de los botones.
        $(function () {
            $(".dt-buttons").removeClass("btn-group");

        });

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
                head[i]['visible'] = "false"
            }
            //if (header[i].Visibility == false || header[i].Visibility !== undefined) {
            //    head[i]['visible'] = false
            //}
            // Entra si se desea indicar un ancho especifico
            if (_header[i].Size != undefined) {
                head[i]['width'] = _header[i].Size
            }
        }

        head.push({
            targets: i,
            className: "text-center",
            width: 80,
            render: function (data, type, row) {
                botones = "";
                var head = _header[0].FieldName;
                if (type == "display") {
                    //botones += '<button class="btn btn-soft-secondary btn-sm edit-btn ladda-button" data-style="zoom-in" data-id="' + row[head] + '"><span class"ladda-label"><i class="fa-thin fa-pen-to-square"></i></span></button>';
                    //botones += '<button class="btn btn-soft-danger btn-sm ml-1 delete-btn-btn ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '"><span class"ladda-label"><i class="fa-thin fa-trash"></i></span></button>';
                    botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-id="' + row[head] + '" class="bs-tooltip edit-btn" data-toggle="tooltip" data-placement="top" title="" data-original-title="Edit"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';
                    botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '" class="bs-tooltip delete-btn" data-toggle="tooltip" data-placement="top" title="" data-original-title="Delete"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';
                }
                return botones;
            }
        })
        return head;
    };




    return obj;
}());


