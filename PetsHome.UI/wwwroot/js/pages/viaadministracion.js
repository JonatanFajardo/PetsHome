var ViaAdministracion = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'viaAdmin_Id', Size: 200 },
                { FieldName: 'viaAdmin_Descripcion' }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
