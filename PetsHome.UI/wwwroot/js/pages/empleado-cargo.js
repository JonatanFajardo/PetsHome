var EmpleadoCargo = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'cag_Id', Size: 60 },
                { FieldName: 'cag_Descripcion' },
                { FieldName: 'cag_Salario', Size: 100 },
                { FieldName: 'esActivo', Size: 100 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;

}());
