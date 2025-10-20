var InventariosDetalle = (function () {

    var obj = {};

    obj.datatablePartials = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {FieldName: "invdet_Id"},
                {FieldName: "inv_Id"},
                {FieldName: "itm_Id"},
                {FieldName: "invdet_Existencia"},
                {FieldName: "invdet_Stock"},
                {FieldName: "invdet_EsEliminado"},
                {FieldName: "invdet_UsuarioCrea"},
                {FieldName: "invdet_FechaCrea"},
                {FieldName: "invdet_UsuarioModifica"},
                {FieldName: "invdet_FechaModifica"}
            ];
            datatablePartials.init(Direction, header);
        })
    }
    return obj;

}());