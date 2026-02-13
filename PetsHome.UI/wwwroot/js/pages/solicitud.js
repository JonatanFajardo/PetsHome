var Solicitud = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "sol_Id", Size: 60 },
                { FieldName: "sol_Identidad", Size: 130 },
                { FieldName: "sol_Nombres" },
                { FieldName: "masc_Nombre", Size: 150 },
                { FieldName: "sol_Correo", Size: 180 }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());