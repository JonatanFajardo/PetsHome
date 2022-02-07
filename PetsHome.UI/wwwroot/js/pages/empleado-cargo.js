var EmpleadoCargo = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: 'cag_Id', Size: 200 },
                { FieldName: 'cag_Descripcion' },
                { FieldName: 'cag_Salario' },
                { FieldName: 'esActivo' }
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;

}());
