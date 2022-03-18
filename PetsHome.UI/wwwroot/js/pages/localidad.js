var Localidad = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'depto_Id'},
                { FieldName: 'depto_Codigo'},
                { FieldName: 'depto_Descripcion'}
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
