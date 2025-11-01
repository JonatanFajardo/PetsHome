$(function () {
    var $table = $("#municipio-detalle");

    if ($table.length === 0) {
        return;
    }

    var dataTable = $table.DataTable({
        responsive: true,
        paging: false,
        searching: false,
        info: false,
        order: [],
        language: {
            zeroRecords: "No se encontraron resultados",
            emptyTable: "Ningún dato disponible en esta tabla",
            paginate: {
            first: "Primero",
            last: "Ultimo",
            next: "Siguiente",
            previous: "Anterior"
        }
        }
    });

    // Oculta contenedores de botones si el layout los agrega automáticamente.
    var $wrapper = $(dataTable.table().container());
    $wrapper.find(".dt-buttons, .dataTables_length").addClass("d-none");
});
