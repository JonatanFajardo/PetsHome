var Mascota = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'masc_Id',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'masc_Imagen',
                    Visibility: true,
                    Render: function(data, type, row) {
                        if (data) {
                            return '<img src="data:image/png;base64,' + data + '" class="pet-photo" alt="' + row.masc_Nombre + '">';
                        } else {
                            return '<div class="pet-photo-placeholder"><i class="fas fa-image"></i></div>';
                        }
                    }
                },
                {
                    FieldName: 'masc_Nombre',
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<div><div class="pet-name">' + data + '</div><div class="pet-type">Perro</div></div>';
                    }
                },
                {
                    FieldName: 'raza_Descripcion',
                    Visibility: true
                },
                {
                    FieldName: 'refg_Nombre',
                    Visibility: true
                },
                {
                    FieldName: 'masc_EsAdoptado',
                    Visibility: true,
                    Render: function(data, type, row) {
                        var estado = 'Disponible';
                        var badgeClass = 'status-disponible';

                        if (row.masc_EsAdoptado) {
                            estado = 'Adoptado';
                            badgeClass = 'status-adoptado';
                        } else if (row.masc_EsReservado) {
                            estado = 'En Tratamiento';
                            badgeClass = 'status-tratamiento';
                        }

                        return '<span class="status-badge ' + badgeClass + '">' + estado + '</span>';
                    }
                }
            ];

            // Inicializar datatable
            datatable.init(Direction, header);
        });
    }

    return obj;

}());

// Función global para eliminar mascota
function deletePet(id) {
    $('#delete-item-id').val(id);
    $('#delete-modal').modal('show');
}
