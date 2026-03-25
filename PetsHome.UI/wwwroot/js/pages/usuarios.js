var Usuarios = (function () {
    var obj = {};
    var urls = {};
    var dt;

    obj.init = function (config) {
        urls = config;
        initDataTable();
        initForm();
        initDelete();
        initSearch();
        loadRolesDropdown();
    };

    function initDataTable() {
        dt = $('#datatable').DataTable({
            responsive: true,
            processing: true,
            dom:
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            order: [],
            lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todos"]],
            pageLength: 10,
            language: {
                processing: "Procesando...",
                zeroRecords: "No se encontraron resultados",
                emptyTable: "No hay usuarios registrados",
                info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
                infoEmpty: "Sin registros",
                infoFiltered: "(filtrado de _MAX_ total)",
                paginate: { first: "Primero", last: "Ultimo", next: "Siguiente", previous: "Anterior" }
            },
            ajax: {
                url: urls.urlList,
                type: "GET",
                dataType: "json",
                error: function (xhr, status, error) {
                    console.log("Error al cargar usuarios: " + error);
                }
            },
            columns: [
                {
                    data: "usu_Id",
                    width: 60,
                    render: function (data) {
                        return '<span style="color:#6B7280;font-weight:600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    data: "usu_Nombre",
                    render: function (data) {
                        return '<div class="pet-name"><i class="fas fa-user-circle mr-2" style="color:#6366f1;"></i>' + data + '</div>';
                    }
                },
                {
                    data: "rol_Descripcion",
                    width: 160,
                    render: function (data) {
                        return '<span class="badge" style="background:#e0e7ff;color:#4338ca;padding:4px 12px;border-radius:12px;font-size:13px;">' + (data || 'Sin rol') + '</span>';
                    }
                },
                {
                    data: "usu_EsActivo",
                    width: 100,
                    render: function (data) {
                        var activo = data === true || data === "True" || data === "true";
                        var badge = activo ? 'status-disponible' : 'status-adoptado';
                        var text = activo ? 'Activo' : 'Inactivo';
                        return '<span class="status-badge ' + badge + '">' + text + '</span>';
                    }
                },
                {
                    data: "usu_Id",
                    width: 130,
                    orderable: false,
                    searchable: false,
                    className: "text-center action-buttons-cell",
                    render: function (data) {
                        return '<div class="action-buttons-wrapper">' +
                            '<button class="action-btn btn-edit" onclick="Usuarios.edit(' + data + ')" title="Editar"><i class="fas fa-edit"></i></button>' +
                            '<button class="action-btn btn-delete" onclick="Usuarios.confirmDelete(' + data + ')" title="Eliminar"><i class="fas fa-trash"></i></button>' +
                            '</div>';
                    }
                }
            ]
        });
    }

    function initForm() {
        $('#edit-modal').on('hidden.bs.modal', function () {
            $('#form-usuario')[0].reset();
            $('#item-id').val('0');
            $('#Usu_EsActivo').prop('checked', true);
        });

        $('#add-btn-header').on('click', function () {
            $('#item-id').val('0');
            $('#form-usuario')[0].reset();
            $('#Usu_EsActivo').prop('checked', true);
            $('.modal-title').html('<i class="fas fa-user-plus mr-2"></i>Nuevo Usuario');
        });

        $('#form-usuario').on('submit', function (e) {
            e.preventDefault();
            var formData = $(this).serialize();

            // Manejar checkbox (si no esta checked, no se envia)
            if (!$('#Usu_EsActivo').is(':checked')) {
                formData += '&Usu_EsActivo=false';
            }

            $.ajax({
                url: urls.urlCreate,
                type: "POST",
                data: formData,
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        $('#edit-modal').modal('hide');
                        dt.ajax.reload(null, false);
                        toastr.success(response.message || 'Operacion exitosa');
                    } else {
                        toastr.error(response.message || 'Error al guardar');
                    }
                },
                error: function () {
                    toastr.error('Error de conexion');
                }
            });
        });
    }

    function initDelete() {
        $('#btn-confirm-delete').on('click', function () {
            var id = $('#delete-item-id').val();
            $.ajax({
                url: urls.urlDelete,
                type: "POST",
                data: { usu_Id: id },
                dataType: "json",
                success: function (response) {
                    $('#delete-modal').modal('hide');
                    if (response.success) {
                        dt.ajax.reload(null, false);
                        toastr.success(response.message || 'Usuario eliminado');
                    } else {
                        toastr.error(response.message || 'Error al eliminar');
                    }
                },
                error: function () {
                    $('#delete-modal').modal('hide');
                    toastr.error('Error de conexion');
                }
            });
        });
    }

    function initSearch() {
        $('#globalSearch').on('keyup', function () {
            dt.search(this.value).draw();
        });
    }

    function loadRolesDropdown() {
        $.ajax({
            url: urls.urlRolesDropdown,
            type: "GET",
            dataType: "json",
            success: function (response) {
                var $select = $('#Rol_Id');
                $select.find('option:not(:first)').remove();
                if (response.data) {
                    $.each(response.data, function (i, item) {
                        $select.append('<option value="' + item.rol_Id + '">' + item.rol_Descripcion + '</option>');
                    });
                }
            }
        });
    }

    obj.edit = function (id) {
        $.ajax({
            url: urls.urlFind,
            type: "GET",
            data: { id: id },
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    var item = response.item;
                    $('#item-id').val(item.usu_Id);
                    $('#Usu_Nombre').val(item.usu_Nombre);
                    $('#Emp_Id').val(item.emp_Id || 0);
                    $('#Rol_Id').val(item.rol_Id);
                    $('#Usu_EsActivo').prop('checked', item.usu_EsActivo === true || item.usu_EsActivo === "True" || item.usu_EsActivo === "true");
                    $('.modal-title').html('<i class="fas fa-user-edit mr-2"></i>Editar Usuario');
                    $('#edit-modal').modal('show');
                } else {
                    toastr.error(response.message || 'No se encontro el usuario');
                }
            },
            error: function () {
                toastr.error('Error de conexion');
            }
        });
    };

    obj.confirmDelete = function (id) {
        // Buscar el nombre del usuario en la tabla
        var name = '';
        dt.rows().every(function () {
            var d = this.data();
            if (d.usu_Id === id) {
                name = d.usu_Nombre;
            }
        });
        $('#delete-item-id').val(id);
        $('#delete-item-name').text(name);
        $('#delete-modal').modal('show');
    };

    return obj;
}());
