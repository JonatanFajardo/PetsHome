var ReportantesTipo = (function () {

    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function() {
            var header = new Array();

            header = [
                { FieldName: "reptip_Id", Size: 80 },
                { FieldName: "reptip_Descripcion" },
                {
                    FieldName: "reptip_EsActivo",
                    Size: 120,
                    Render: function(data, type, row) {
                        var estado = data ? 'Activo' : 'Inactivo';
                        var badgeClass = data ? 'status-disponible' : 'status-adoptado';
                        return '<span class="status-badge ' + badgeClass + '">' + estado + '</span>';
                    }
                }
            ]

            datatableCatalogs.init(Direction.listUrl, header);
        })
    }
    return obj;

}());
