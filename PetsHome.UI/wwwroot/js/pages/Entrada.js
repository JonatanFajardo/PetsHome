var Entrada = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [

            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());