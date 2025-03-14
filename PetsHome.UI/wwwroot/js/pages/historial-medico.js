
var HistorialMedico = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'medic_Id', Visibility: false },
                { FieldName: 'mascota', Size: 200, Visibility: true },
                { FieldName: 'esterilizacion', Visibility: true },
                { FieldName: 'comportamiento', Visibility: true },
                { FieldName: 'saludcuidado', Visibility: true },
                { FieldName: 'informacionadicional', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
