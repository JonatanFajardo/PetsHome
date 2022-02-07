
var Categoria = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'cat_Id', Size: 200},
                { FieldName: 'cat_Descripcion'}
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;

}());