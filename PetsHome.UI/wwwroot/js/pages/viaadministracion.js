var ViaAdministracion = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'viaAdmin_Id', Size: 60 },
                { FieldName: 'viaAdmin_Descripcion' },
                { FieldName: 'viaAdmin_EsActivo', Size: 140 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
