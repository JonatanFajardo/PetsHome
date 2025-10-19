var Vacuna = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "vac_Id", Size: 200 },
                { FieldName: "vac_Descripcion" }
            ];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;

}());