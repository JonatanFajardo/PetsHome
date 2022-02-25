var EntradasDetalle = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'entdet_Id', Visibility: false },
                { FieldName: 'ent_Id', Size: 200, Visibility: true },
                { FieldName: 'itm_Id', Visibility: true },
                { FieldName: 'entdet_Cantidad', Visibility: true },
                { FieldName: 'UsuarioCreacion', Visibility: true },
                { FieldName: 'ent_FechaCrea', Visibility: true },
                { FieldName: 'UsuarioModificacion', Visibility: true },
                { FieldName: 'ent_FechaModifica', Visibility: true }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());