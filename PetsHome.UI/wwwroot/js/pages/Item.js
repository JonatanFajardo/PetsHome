
var Item = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'itm_Id', Size: 60, Visibility: true },
                { FieldName: 'itm_Codigo', Size: 100, Visibility: true },
                { FieldName: 'itm_Descripcion', Visibility: true },
                { FieldName: 'cat_Descripcion', Size: 140, Visibility: true },
                { FieldName: 'itm_Precio', Size: 100, Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());