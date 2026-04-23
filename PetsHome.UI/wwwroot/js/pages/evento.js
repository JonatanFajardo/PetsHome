var Evento = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            console.log("ws");
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                { FieldName: 'eve_Id', Visibility: false },
                {
                    FieldName: 'eve_Descripcion',
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return `
                                <div style="display:flex;align-items:center;gap:10px;">
                                    <div style="background:#ede9fe;border-radius:50%;width:32px;height:32px;
                                                display:flex;align-items:center;justify-content:center;flex-shrink:0;">
                                        <i class="fas fa-paw" style="color:#7c3aed;font-size:14px;"></i>
                                    </div>
                                    <span>${data ?? ''}</span>
                                </div>`;
                        }
                        return data;
                    }
                },
                { FieldName: 'refg_Id', Size: 60, Visibility: true },
                {
                    FieldName: 'refg_Nombre',
                    Size: 150,
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return `
                                <div style="display:flex;align-items:center;gap:10px;">
                                    <div style="background:#ede9fe;border-radius:50%;width:32px;height:32px;
                                                display:flex;align-items:center;justify-content:center;flex-shrink:0;">
                                        <i class="fas fa-house-user" style="color:#7c3aed;font-size:14px;"></i>
                                    </div>
                                    <span>${data ?? ''}</span>
                                </div>`;
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'eve_Fecha',
                    Size: 110,
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af">—</span>';
                            var fecha = new Date(data);
                            return isNaN(fecha.getTime())
                                ? '<span style="color:#9ca3af">—</span>'
                                : fecha.toLocaleDateString('es-HN', {
                                    day: '2-digit',
                                    month: '2-digit',
                                    year: 'numeric'
                                });
                        }
                        return data;
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    }
    return obj;

}());