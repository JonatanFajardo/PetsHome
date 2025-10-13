var Solicitud = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "sol_Id", Size: 200 },
                { FieldName: "sol_Identidad" },
                { FieldName: "sol_Nombres" },
                { FieldName: "masc_Nombre" },
                { FieldName: "sol_Correo" }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());