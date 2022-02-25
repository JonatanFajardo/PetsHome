var Refugio = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "raza_Id", Size: 200 },
                { FieldName: "raza_Descripcion" }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());