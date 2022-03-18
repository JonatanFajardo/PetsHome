var Municipio = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {FieldName: "mpio_Id"},
                {FieldName: "mpio_Codigo"},
                {FieldName: "mpio_Descripcion"}
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;

}());

