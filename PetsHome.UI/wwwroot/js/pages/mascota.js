var Mascota = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            header = [
                { FieldName: 'masc_Id', Size: 60, Visibility: false },
                {
                    FieldName: 'masc_Imagen',
                    Size: 80,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (data) {
                                return '<img src="data:image/png;base64,' + data + '" style="width:40px;height:40px;border-radius:50%;object-fit:cover;" alt="Foto">';
                            }
                            return '<span style="color:#9ca3af;font-size:28px;"><i class="fas fa-user-circle"></i></span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'masc_Nombre',
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<div style="display:flex;align-items:center;gap:8px;">'
                                + '<span style="background:#ede9fe;border-radius:6px;padding:4px 7px;">'
                                + '<i class="fas fa-paw" style="color:#7c3aed;font-size:13px;"></i></span>'
                                + '<span style="font-weight:500;color:#111827;">' + data + '</span>'
                                + '</div>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'raza_Descripcion',
                    Size: 130,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'refg_Nombre',
                    Size: 150,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            return '<span style="color:#374151;">' + data + '</span>';
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'masc_EsAdoptado',
                    Size: 120,
                    Visibility: true,
                    Render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.masc_EsAdoptado) {
                                return '<span class="status-badge status-adoptado">Adoptado</span>';
                            } else if (row.masc_EsReservado) {
                                return '<span class="status-badge status-tratamiento">En Tratamiento</span>';
                            }
                            return '<span class="status-badge status-disponible">Disponible</span>';
                        }
                        return data;
                    }
                }
            ];
            datatable.init(Direction, header);
        });
    }

    return obj;

}());

function deletePet(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
