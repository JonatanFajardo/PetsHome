
var Empleado = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'emp_Id', Visibility: true },
                { FieldName: 'emp_Codigo', Size: 200, Visibility: true },
                { FieldName: 'emp_Nombres', Visibility: true },
                { FieldName: 'cag_Descripcion', Visibility: true },
                { FieldName: 'refg_Nombre', Visibility: true },
                { FieldName: 'emp_EsActivo', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());