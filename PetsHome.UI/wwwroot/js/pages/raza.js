var Raza = (function () {
   
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function() {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            //header = [
            //    //["Fila"],
            //    "raza_Id",
            //    "raza_Descripcion"
            //];

            header = [
                { FieldName: "raza_Id", Size: 80 },
                { FieldName: "raza_Descripcion" },
                { FieldName: "raza_TipoAnimal", Size: 150 },
                { FieldName: "raza_Tamano", Size: 120 },
                { FieldName: "raza_TipoPelaje", Size: 150 }
            ]

            //header = [
            //    ["raza_Id", 80, false],
            //    ["raza_Descripcion", 0, true]
            //];
            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;

}());



           