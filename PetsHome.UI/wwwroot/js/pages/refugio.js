var Refugio = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "refg_Id", Size: 200 },
                { FieldName: "refg_Nombre" },
                { FieldName: "refg_RTN" },
                { FieldName: "refg_Ubicacion" }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());