var Voluntario = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "vol_Id", Size: 200 },
                { FieldName: "vol_HorasTrabajadas" },
                { FieldName: "Nombres" },
                { FieldName: "per_Identidad" },
                { FieldName: "vol_Recurrente" }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());