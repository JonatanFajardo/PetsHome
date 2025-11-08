var EmpleadosCargo = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            header = [
                { FieldName: 'cag_Id', Size: 200 },
                { FieldName: 'cag_Descripcion' },
                { FieldName: 'cag_Salario' },
                { FieldName: 'esActivo' }
            ];

            // Usar datatableCatalogs para catálogos simples
            datatableCatalogs.init(Direction, header);
        });
    }

    return obj;

}());
