var Mascota = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'masc_Id', Visibility: true },
                { FieldName: 'masc_Fila', Size: 200, Visibility: true },
                { FieldName: 'masc_Nombre', Visibility: true },
                { FieldName: 'raza_Descripcion', Visibility: true },
                { FieldName: 'refg_Nombre', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());