var TipoMedicamento = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoMed_Id', Size: 60, Visibility: false},
                { FieldName: 'tipoMed_Descripcion' },
                { FieldName: 'tipoMed_EsActivo', Size: 140 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
