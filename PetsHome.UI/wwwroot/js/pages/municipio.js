var Municipio = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                {FieldName: "mpio_Id"},
                {FieldName: "mpio_Codigo"},
                {FieldName: "mpio_Descripcion"},
                {FieldName: "depto_Id"},
                {FieldName: "mpio_EsEliminado"},
                {FieldName: "mpio_UsuarioCrea"},
                {FieldName: "mpio_FechaCrea"},
                {FieldName: "mpio_UsuarioModifica"},
                {FieldName: "mpio_FechaModifica"}
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());

