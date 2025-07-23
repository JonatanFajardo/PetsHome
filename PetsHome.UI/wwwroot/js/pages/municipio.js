var Municipio = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {FieldName: "mpio_Id"},
                {FieldName: "mpio_Codigo"},
                {FieldName: "mpio_Descripcion"}
            ];
            datatablePartials.initPartials(Direction.listUrl, Direction.id, header);
        })
    }
    return obj;

}());

