var TipoParasito = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoPar_Id', Size: 80, Visibility: false},
                { FieldName: 'tipoPar_Descripcion' },
                { FieldName: 'tipoPar_Categoria', Size: 150 },
                { FieldName: 'tipoPar_EsActivo', Size: 140 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
