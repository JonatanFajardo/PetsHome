var Procedencia = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'proc_Id', Size: 200 },
                { FieldName: 'proc_Descripcion' }
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;

}());