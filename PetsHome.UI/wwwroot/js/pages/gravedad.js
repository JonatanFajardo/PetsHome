var Gravedad = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'grav_Id', Size: 60 },
                { FieldName: 'grav_Descripcion' },
                { FieldName: 'grav_EsActivo', Size: 140 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
