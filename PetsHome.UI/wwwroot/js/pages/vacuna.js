var Vacuna = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tama�o/AutoWidth | Visibilidad
            header = [
                { FieldName: "vac_Id", Size: 60 },
                { FieldName: "vac_Descripcion" },
                { FieldName: "vacu_Especie", Size: 120 },
                { FieldName: "vacu_DosisRecomendada", Size: 130 },
                { FieldName: "vacu_PeriodoRefuerzo", Size: 130 }
            ];
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;

}());