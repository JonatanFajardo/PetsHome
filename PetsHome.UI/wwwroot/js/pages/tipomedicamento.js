var TipoMedicamento = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoMed_Id', Size: 60 },
                { FieldName: 'tipoMed_Descripcion' },
                { FieldName: 'esActivo', Size: 140 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
