var EntradasDetalle = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {FieldName: "entdet_Id"},
                {FieldName: "ent_Id"},
                {FieldName: "itm_Id"},
                {FieldName: "entdet_Cantidad"},
                {FieldName: "entdet_EsEliminado"},
                {FieldName: "entdet_UsuarioCrea"},
                {FieldName: "entdet_FechaCrea"},
                {FieldName: "entdet_UsuarioModifica"},
                {FieldName: "entdet_FechaModifica"}
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());