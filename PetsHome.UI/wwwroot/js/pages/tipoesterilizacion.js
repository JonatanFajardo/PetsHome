var TipoEsterilizacion = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoEst_Id', Size: 80 },
                { FieldName: 'tipoEst_Descripcion' },
                { FieldName: 'tipoEst_Sexo', Size: 150 },
                { FieldName: 'tipoEst_EsActivo', Size: 140 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
