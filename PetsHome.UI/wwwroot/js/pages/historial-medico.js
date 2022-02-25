
var HistorialMedico = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'medic_Id', Visibility: false },
                { FieldName: 'masc_Id', Size: 200, Visibility: true },
                { FieldName: 'medic_Esterilizacion', Visibility: true },
                { FieldName: 'com_Id', Visibility: true },
                { FieldName: 'medic_SaludCuidado', Visibility: true },
                { FieldName: 'medic_InformacionAdicional', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
