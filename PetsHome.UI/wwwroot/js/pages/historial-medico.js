
var HistorialMedico = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'cita_Id', Visibility: false },
                { FieldName: 'mascota', Visibility: true },
                { FieldName: 'esterilizacion', Size: 120, Visibility: true },
                { FieldName: 'comportamiento', Size: 120, Visibility: true },
                { FieldName: 'saludcuidado', Size: 130, Visibility: true },
                { FieldName: 'informacionadicional', Size: 150, Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
