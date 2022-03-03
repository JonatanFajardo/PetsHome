var Voluntario = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "vol_Id", Size: 200 },
                { FieldName: "vol_HorasTrabajadas" },
                { FieldName: "vol_Nombres" },
                { FieldName: "per_Identidad" }
            ];
            console.log("llego");
            datatable.init(Direction, header);
        })
    }
    return obj;

}());