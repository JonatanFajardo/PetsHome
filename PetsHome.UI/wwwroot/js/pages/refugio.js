var Refugio = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "refg_Id", Size: 60 },
                { FieldName: "refg_Nombre" },
                { FieldName: "refg_RTN", Size: 130 },
                { FieldName: "refg_Ubicacion", Size: 200 },
                { FieldName: "esActivo", Size: 140 }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());