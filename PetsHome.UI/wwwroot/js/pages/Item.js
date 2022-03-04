
var Item = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'itm_Id', Visibility: true },
                { FieldName: 'itm_Codigo', Visibility: true },
                { FieldName: 'itm_Descripcion', Visibility: true },
                { FieldName: 'cat_Descripcion', Visibility: true },
                { FieldName: 'itm_Precio', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());