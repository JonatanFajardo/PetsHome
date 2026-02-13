var RecepcionMercancia = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                { FieldName: 'recep_Id', Size: 60 },
                { FieldName: 'recep_Descripcion' },
                { FieldName: 'recep_Fecha', Size: 110, render: function(data) {
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('es-HN');
                    }
                    return '';
                }},
                { FieldName: 'refg_Nombre', Size: 150 },
                { FieldName: 'recep_TipoRecepcion', Size: 130 },
                { FieldName: 'recep_NumeroDocumento', Size: 130 }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
