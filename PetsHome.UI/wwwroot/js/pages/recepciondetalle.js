var RecepcionDetalle = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        var header = [
            { FieldName: "recdet_Id", Visibility: false},
            { FieldName: "itm_Descripcion" },
            { FieldName: "recdet_Cantidad" },
            { FieldName: "recdet_PrecioUnitario" },
            { FieldName: "recdet_FechaVencimiento" },
            { FieldName: "recdet_NumeroLote" }
        ];
        datatablePartials.init(Direction, header);
    }

    return obj;

}());
