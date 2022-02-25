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
                { FieldName: "sol_Apellidos" },
                { FieldName: "sol_Telefono" },
                { FieldName: "sol_Correo" },
                { FieldName: "sol_Fecha" },
                { FieldName: "masc_Nombre" }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());