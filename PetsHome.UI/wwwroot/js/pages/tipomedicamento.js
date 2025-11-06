var TipoMedicamento = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoMed_Id', Size: 200 },
                { FieldName: 'tipoMed_Descripcion' }
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;
}());
