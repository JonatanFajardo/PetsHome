var Inventario = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {FieldName: "inv_Id"},
                {FieldName: "inv_Fecha"},
                {FieldName: "refg_Id"},
                {FieldName: "inv_EsEliminado"},
                {FieldName: "inv_UsuarioCrea"},
                {FieldName: "inv_FechaCrea"},
                {FieldName: "inv_UsuarioModifica"},
                {FieldName: "inv_FechaModifica"}
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());