var TipoConsulta = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoCon_Id', Size: 200 },
                { FieldName: 'tipoCon_Descripcion' }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
