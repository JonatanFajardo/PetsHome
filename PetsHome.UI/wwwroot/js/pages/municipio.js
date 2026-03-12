var Municipio = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        var header = [
            {FieldName: "mpio_Id", Size: 60, Visibility: false},
            {FieldName: "mpio_Codigo", Size: 100},
            {FieldName: "mpio_Descripcion"}
        ];
        datatablePartials.init(Direction, header);
    }

    return obj;

}());

