var Mascota = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();

            // Definir headers con configuración personalizada
            header = [
                {
                    FieldName: 'masc_Id',
                    Size: 60,
                    Visibility: true,
                    Render: function(data, type, row) {
                        return '<span style="color: #6B7280; font-weight: 600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    FieldName: 'masc_Imagen',
                    Size: 80,
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
                    Size: 130,
                    Visibility: true
                },
                {
                    FieldName: 'refg_Nombre',
                    Size: 150,
                    Visibility: true
                },
                {
                    FieldName: 'masc_EsAdoptado',
                    Size: 120,
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
