var Entrada = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'ent_Id', Visibility: false },
                { FieldName: 'ent_Descripcion', Size: 200, Visibility: true },
                { FieldName: 'refg_Nombre', Visibility: true },
                { FieldName: 'ent_Fecha', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());