var Inventario = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: 'inv_Id',
                    DisplayName: 'ID',
                    Width: '80px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'inv_Fecha',
                    DisplayName: 'Fecha',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (data) {
                            return new Date(data).toLocaleDateString('es-ES');
                        }
                        return '';
                    }
                },
                {
                    FieldName: 'inv_NombreUsuarioCrea',
                    DisplayName: 'Usuario Creación',
                    Width: '200px',
                    Align: 'left',
                    Visibility: true,
                    Sortable: true
                },
                {
                    FieldName: 'inv_FechaCrea',
                    DisplayName: 'Fecha Creación',
                    Width: '120px',
                    Align: 'center',
                    Visibility: true,
                    Sortable: true,
                    Render: function (data) {
                        if (data) {
                            return new Date(data).toLocaleDateString('es-ES');
                        }
                        return '';
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    }

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {FieldName: "inv_Id"},
                {FieldName: "inv_Fecha"},
                {FieldName: "refg_Id"}
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());