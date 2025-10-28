var RecepcionDetalle = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                { FieldName: "recdet_Id" },
                { FieldName: "itm_Descripcion" },
                { FieldName: "recdet_Cantidad" },
                { FieldName: "recdet_PrecioUnitario", render: function(data) {
                    if (data) {
                        return 'L ' + parseFloat(data).toFixed(2);
                    }
                    return '';
                }},
                { FieldName: "recdet_FechaVencimiento", render: function(data) {
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('es-HN');
                    }
                    return '';
                }},
                { FieldName: "recdet_NumeroLote" }
            ];
            datatablePartials.init(Direction.listUrl, Direction.id, header);
        })
    }
    return obj;

}());
