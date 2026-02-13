var EmpleadosCargo = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            header = [
                { FieldName: 'cag_Id', Size: 60 },
                { FieldName: 'cag_Descripcion' },
                { FieldName: 'cag_Salario', Size: 100 },
                { FieldName: 'esActivo', Size: 100 }
            ];

            // Usar datatableCatalogs para catálogos simples
            datatableCatalogs.init(Direction, header);
        });
    }

    return obj;

}());
