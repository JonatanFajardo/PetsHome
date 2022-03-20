var EntradasDetalle = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'entdet_Id', Visibility: false },
                { FieldName: 'itm_Descripcion', Visibility: true },
                { FieldName: 'entdet_Cantidad', Visibility: true }
            ];
            datatablePartials.init(Direction.listUrl, Direction.id, header);
        })
    }
    return obj;

}());