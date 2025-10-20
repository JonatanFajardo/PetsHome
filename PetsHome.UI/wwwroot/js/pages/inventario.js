var Inventario = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {FieldName: "inv_Id"},
                {FieldName: "inv_Fecha"},
                {FieldName: "refg_Nombre"}
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
