var HistorialMedico = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // TODO: Definir headers según estructura de datos
            header = [];

            // Usar datatableCatalogs para catálogos simples
            datatableCatalogs.init(Direction.listUrl, header);
        });
    }

    return obj;

}());
