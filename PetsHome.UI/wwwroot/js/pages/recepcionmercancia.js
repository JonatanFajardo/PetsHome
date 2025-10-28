var RecepcionMercancia = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                { FieldName: 'recep_Id' },
                { FieldName: 'recep_Descripcion' },
                { FieldName: 'recep_Fecha', render: function(data) {
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('es-HN');
                    }
                    return '';
                }},
                { FieldName: 'refg_Nombre' },
                { FieldName: 'recep_TipoRecepcion' },
                { FieldName: 'recep_NumeroDocumento' }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());
