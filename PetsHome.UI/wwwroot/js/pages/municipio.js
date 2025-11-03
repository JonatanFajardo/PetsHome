var Municipio = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        var header = [
            {FieldName: "mpio_Id"},
            {FieldName: "mpio_Codigo"},
            {FieldName: "mpio_Descripcion"}
        ];
        datatablePartials.init(Direction.listUrl, Direction.id, header);
    }

    return obj;

}());

