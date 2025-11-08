var Municipio = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        var header = [
            {FieldName: "mpio_Id"},
            {FieldName: "mpio_Codigo"},
            {FieldName: "mpio_Descripcion"}
        ];
        datatablePartials.init(Direction, header);
    }

    return obj;

}());

