var Localidad = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'depto_Id', Visibility: true },
                { FieldName: 'depto_Codigo', Visibility: true },
                { FieldName: 'depto_Descripcion', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
