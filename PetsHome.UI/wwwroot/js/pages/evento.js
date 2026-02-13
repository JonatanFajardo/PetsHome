var Evento = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            console.log("ws");
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'eve_Id', Visibility: false },
                { FieldName: 'eve_Descripcion', Visibility: true },
                { FieldName: 'refg_Id', Size: 60, Visibility: true },
                { FieldName: 'refg_Nombre', Size: 150, Visibility: true },
                { FieldName: 'eve_Fecha', Size: 110, Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());