

init();
function init() {

}

//==================================================
//variables globales
//==================================================

//var tabla;
//var deleteId = 0;
//var GlobalHeader;

//==================================================
//Actualiza el datatable
//==================================================
function reloadTable() {
    $("#datatable").DataTable().ajax.reload();
}

//==================================================
//Indicamos cual de los errorres se presento
//==================================================
//function Mensaje(msj) {
//    switch (msj) {
//        case "error":
//            appConfig.alert("error", "Ha ocurrido un error");
//            window.setTimeout(function () { }, 1000);
//            break;
//        case "I":
//            appConfig.alert(
//                "success",
//                "Registro insertado correctamente"
//            );
//            reloadTable();
//            $("#modalNuevo").modal("hide");
//            // tabla = {};
//            $("#errorMessage").empty();
//            msj = "";
//            break;
//        case "I-1":
//            appConfig.alert("error", "Error al insertar correctamente");

//            $("#errorMessage").empty();
//            msj = "";
//            break;
//        case "U":
//            appConfig.alert("success", "Registro editado correctamente");
//            reloadTable();
//            $("#modalNuevo").modal("hide");
//            $("#errorMessage").empty();
//            //tabla = {};
//            break;
//        case "U-1":
//            appConfig.alert("error", "Error editado correctamente");

//            $("#errorMessage").empty();
//            break;
//        case "D":
//            appConfig.alert(
//                "success",
//                "Registro Eliminado correctamente"
//            );
//            reloadTable();
//            $("#modalDelete").modal("hide");

//            $("#errorMessage").empty();

//            break;
//    }
//}

//==================================================
//Configuraciones y alertas
//==================================================

var appConfig = (function () {
    var obj = {};
    obj.alert = function (type, message) {
        $(function () {
            toastr.options = {
                closeButton: false,
                debug: false,
                newestOnTop: false,
                progressBar: false,
                positionClass: "toast-bottom-right",
                preventDuplicates: true,
                onclick: null,
                showDuration: "300",
                hideDuration: "1000",
                timeOut: "5000",
                extendedTimeOut: "1000",
                showEasing: "swing",
                hideEasing: "linear",
                showMethod: "fadeIn",
                hideMethod: "fadeOut",
            };

            toastr.remove();
            //success - error - warning - info
            switch (type) {
                default:
                case "info":
                case 0:
                    toastr.info(
                        '<i class="fas fa-exclamation-circle"></i><span>' +
                        message +
                        "</span>"
                    );
                    break;
                case "success":
                case 1:
                    toastr.success(
                        '<i class="fas fa-check-circle"></i><span>' +
                        message +
                        "</span>"
                    );
                    break;
                case "error":
                case 2:
                    toastr.error(
                        '<i class="fas fa-exclamation-circle"></i><span>' +
                        message +
                        "</span>"
                    );
                    break;
                case "warning":
                case 3:
                    toastr.warning(
                        '<i class="fas fa-exclamation-triangle"></i><span>' +
                        message +
                        "</span>"
                    );
                    break;
            }
        });
    };

    //function setupPlugins() {
    //    $.extend(true, $.fn.dataTable.defaults, {
    //        dom:
    //            "<'row'<'col-md-8 text-left'B><'col-md-4'f>>" +
    //            "<'row'<'col-sm-12'tr>>" +
    //            "<'row'<'col-sm-5'i><'col-sm-7'p>>",

    //        order: [],
    //        scrollCollapse: true,
    //        paging: true,
    //        stateSave: true,
    //        processing: true,
    //        lengthMenu: [
    //            [10, 25, 50, 100, -1],
    //            [10, 25, 50, 100, "Todos"],
    //        ],
    //        pageLenght: 25,
    //        displayLength: 25,
    //        language: {
    //            processing: "<span class='mdi mdi mdi mdi-ufo-outline'></span>",
    //            lengthMenu: "Mostrar _MENU_ registros",
    //            zeroRecords: "No se encontraron resultados",
    //            emptyTable: "Ningún dato disponible en esta tabla",
    //            info:
    //                "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
    //            infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
    //            infoFiltered: "(filtrado de un total de _MAX_ registros)",
    //            infoPostFix: "",
    //            search: "",
    //            url: "",
    //            infoThousands: ",",
    //            loadingRecords: " ",
    //            searchPlaceholder: "Buscar en la tabla...",
    //            paginate: {
    //                first: "Primero",
    //                last: "Último",
    //                next: "Siguiente",
    //                previous: "Anterior",
    //            },
    //            aria: {
    //                sortAscending:
    //                    ": Activar para ordenar la columna de manera ascendente",
    //                sortDescending:
    //                    ": Activar para ordenar la columna de manera descendente",
    //            },
    //        },
    //    });
    //}



    //obj.init = function () {
    //    setupPlugins();
    //};
    return obj;
})();

//$(function () {
//    appConfig.init();
//});


//==================================================
//Eliminar
//==================================================

function EliminarModal() {
    var title = ` 
        <h5  class="modal-title">Eliminar</h5>

        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
    ` ;
    var body = ` 
        <p>¿Está seguro que desea eliminar el registro?</p>
        <input type="hidden" id="id"/>
    ` ;
    var footer = ` 
        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
        <button type="button" class="btn btn-danger" id="button-modal" onclick="EliminarModalShow()"><i class="mdi mdi-delete"></i>Eliminar</button>
    ` ;
    InnerHTMLCard(title, body, footer, "eliminar-modal");
}

function getIdDelete(id) {
    $('#eliminar-modal').modal('show');
    $('#id').val(id);
}

function EliminarModalShow() {
    id = $('#id').val();
    console.log(id);
    data = {
        tabla: tabla,
        id: id
    };

    console.log('ELIMINAR');

    $.ajax({
        url: "/Appets/Eliminar",
        type: "POST",
        dataType: "json",
        data: data
    }).done(function (resultAjax) {
        if (resultAjax != -1) {
            reloadTable();
            $('#eliminar-modal').modal('hide');
        }
    });
}


function InnerHTMLCard(title, body, footer, idPadre) {
    //card-js
    var container = document.getElementById(idPadre);

    // Create card element 
    const card = document.createElement('div');
    card.classList = 'card-header';
    card.classList = 'card-body';
    card.classList = 'card-footer';

    // Construct card content
    var content = ` 
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="card">
                        <div class="card-header">
                            ${title}
                        </div>
                        <div class="card-body">
                            ${body}
                        </div>
                        <div class="modal-footer">
                            ${footer}
                        </div>
                    </div>
                </div>
            </div>
    `;

    container.innerHTML += content;

}
