var Evento = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            console.log("ws");
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'eve_Id', Visibility: false },
                { FieldName: 'eve_Descripcion', Size: 200, Visibility: true },
                { FieldName: 'refg_Id', Visibility: true },
                { FieldName: 'refg_Nombre', Visibility: true },
                { FieldName: 'eve_Fecha', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());