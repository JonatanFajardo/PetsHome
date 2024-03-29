﻿//==================================================
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
                    buttons: [{
                        extend: "excelHtml5",
                        text: '<i class="mdi mdi-file-excel"> Excel</i>',
                        exportOptions: exportOptions
                    },
                    {
                        extend: "csvHtml5",
                        text: '<i class="mdi mdi-file-excel-outline"> CSV</i>',
                        exportOptions: exportOptions
                    },
                    {
                        extend: "pdfHtml5",
                        text: '<i class="mdi mdi-file-pdf"> CSV</i>',
                        exportOptions: exportOptions
                    }]
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
                    console.log("Error en el envio de la peticion de listado " + jqXHR.responseJSON);
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
     * Inicializa el datatable.
     * @param {Object} listUrl Direccion al que se enviaran los datos
     * @param {Array} header Listado de nombres y configuraciones en las columnas.
     */
    obj.init = function (listUrl, header) {
        this.config();
        this.createDatatable(listUrl, header);
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
            render: function (data, type, row) {
                botones = "";
                var head = _header[0].FieldName;
                if (type == "display") {
                    //botones += '<button class="btn btn-soft-secondary btn-sm edit-btn ladda-button" data-style="zoom-in" data-id="' + row[head] + '"><span class"ladda-label"><i class="fa-thin fa-pen-to-square"></i></span></button>';
                    //botones += '<button class="btn btn-soft-danger btn-sm ml-1 delete-btn ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '"><span class"ladda-label"><i class="fa-thin fa-trash"></i></span></button>';
                    //botones += '<button class="btn btn-secondary btn-sm" href="/Usuarios/Editar/1"><i class="mdi mdi-square-edit-outline"></i></button>';
                    botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-id="' + row[head] + '" class="bs-tooltip edit-btn" data-toggle="tooltip" data-placement="top" title="" data-original-title="Edit"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a>';
                    botones += '<a href="javascript:void(0);" ladda-button" data-style="zoom-in" data-toggle="modal" data-target="#delete-modal" data-id="' + row[head] + '" class="bs-tooltip delete-btn" data-toggle="tooltip" data-placement="top" title="" data-original-title="Delete"><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg></a>';
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


