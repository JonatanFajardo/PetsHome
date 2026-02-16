var TipoParasito = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'tipoPar_Id', Size: 80 },
                { FieldName: 'tipoPar_Descripcion' },
                { FieldName: 'tipoPar_Categoria', Size: 150 },
                { FieldName: 'esActivo', Size: 140 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
