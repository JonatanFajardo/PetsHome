var Gravedad = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'grav_Id', Size: 200 },
                { FieldName: 'grav_Descripcion' }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
