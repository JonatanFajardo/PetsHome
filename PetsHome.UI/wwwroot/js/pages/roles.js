var Roles = (function () {
    var obj = {};
    var urls = {};
    var dt;

    obj.init = function (config) {
        urls = config;
        initDataTable();
        initForm();
        initDelete();
        initSearch();
        initPantallas();
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
                emptyTable: "No hay roles registrados",
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
                    console.log("Error al cargar roles: " + error);
                }
            },
            columns: [
                {
                    data: "rol_Id",
                    width: 60,
                    render: function (data) {
                        return '<span style="color:#6B7280;font-weight:600;">#' + String(data).padStart(3, '0') + '</span>';
                    }
                },
                {
                    data: "rol_Descripcion",
                    render: function (data) {
                        return '<div class="pet-name"><i class="fas fa-shield-alt mr-2" style="color:#6366f1;"></i>' + data + '</div>';
                    }
                },
                {
                    data: "cantidadPantallas",
                    width: 120,
                    className: "text-center",
                    render: function (data) {
                        return '<span class="badge" style="background:#e0e7ff;color:#4338ca;padding:4px 12px;border-radius:12px;font-size:13px;">' + (data || 0) + ' pantallas</span>';
                    }
                },
                {
                    data: "rol_Estado",
                    width: 100,
                    render: function (data) {
                        var activo = data === true || data === "True" || data === "true";
                        var badge = activo ? 'status-disponible' : 'status-adoptado';
                        var text = activo ? 'Activo' : 'Inactivo';
                        return '<span class="status-badge ' + badge + '">' + text + '</span>';
                    }
                },
                {
                    data: "rol_Id",
                    width: 160,
                    orderable: false,
                    searchable: false,
                    className: "text-center action-buttons-cell",
                    render: function (data) {
                        return '<div class="action-buttons-wrapper">' +
                            '<button class="action-btn btn-edit" onclick="Roles.edit(' + data + ')" title="Editar"><i class="fas fa-edit"></i></button>' +
                            '<button class="action-btn" onclick="Roles.openPantallas(' + data + ')" title="Asignar pantallas" style="color:#2196F3;"><i class="fas fa-shield-alt"></i></button>' +
                            '<button class="action-btn btn-delete" onclick="Roles.confirmDelete(' + data + ')" title="Eliminar"><i class="fas fa-trash"></i></button>' +
                            '</div>';
                    }
                }
            ]
        });
    }

    function initForm() {
        $('#edit-modal').on('hidden.bs.modal', function () {
            $('#form-rol')[0].reset();
            $('#item-id').val('0');
            $('#rol_Estado').prop('checked', true);
        });

        $('#add-btn-header').on('click', function () {
            $('#item-id').val('0');
            $('#form-rol')[0].reset();
            $('#rol_Estado').prop('checked', true);
            $('.modal-title').first().html('<i class="fas fa-shield-alt mr-2"></i>Nuevo Rol');
        });

        $('#form-rol').on('submit', function (e) {
            e.preventDefault();
            var formData = $(this).serialize();
            if (!$('#rol_Estado').is(':checked')) {
                formData += '&rol_Estado=false';
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
                data: { rol_Id: id },
                dataType: "json",
                success: function (response) {
                    $('#delete-modal').modal('hide');
                    if (response.success) {
                        dt.ajax.reload(null, false);
                        toastr.success(response.message || 'Rol eliminado');
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

    // ==========================================
    // Pantallas assignment
    // ==========================================

    function initPantallas() {
        // Select/deselect all
        $('#btn-select-all').on('click', function () {
            $('.pantalla-check').prop('checked', true);
            updateAllGroupBadges();
        });

        $('#btn-deselect-all').on('click', function () {
            $('.pantalla-check').prop('checked', false);
            updateAllGroupBadges();
        });

        // Expand/collapse all
        $('#btn-expand-all').on('click', function () {
            $('.grupo-content').slideDown(200);
            $('.chevron').removeClass('collapsed');
        });

        $('#btn-collapse-all').on('click', function () {
            $('.grupo-content').slideUp(200);
            $('.chevron').addClass('collapsed');
        });

        // Save pantallas
        $('#btn-save-pantallas').on('click', function () {
            var rolId = $('#pantallas-rol-id').val();
            var selectedIds = [];
            $('.pantalla-check:checked').each(function () {
                selectedIds.push($(this).val());
            });

            var $btn = $(this);
            $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin mr-1"></i>Guardando...');

            $.ajax({
                url: urls.urlSavePantallas,
                type: "POST",
                data: { rolId: rolId, pantallaIds: selectedIds.join(',') },
                dataType: "json",
                success: function (response) {
                    $('#pantallas-modal').modal('hide');
                    if (response.success) {
                        dt.ajax.reload(null, false);
                        toastr.success(response.message || 'Pantallas asignadas');
                    } else {
                        toastr.error(response.message || 'Error al asignar pantallas');
                    }
                },
                error: function () {
                    $('#pantallas-modal').modal('hide');
                    toastr.error('Error de conexion');
                },
                complete: function () {
                    $btn.prop('disabled', false).html('<i class="fas fa-save mr-1"></i>Guardar Pantallas');
                }
            });
        });
    }

    function loadPantallasTree(rolId) {
        var container = $('#pantallas-tree-container');
        container.html('<div class="text-center py-4"><i class="fas fa-spinner fa-spin fa-2x" style="color:#6366f1;"></i><p class="mt-2 text-muted">Cargando pantallas...</p></div>');

        // Load all pantallas
        $.ajax({
            url: urls.urlPantallasList,
            type: "GET",
            dataType: "json",
            success: function (allResponse) {
                // Load assigned pantallas
                $.ajax({
                    url: urls.urlPantallasByRol + "?id=" + rolId,
                    type: "GET",
                    dataType: "json",
                    success: function (assignedResponse) {
                        var assignedIds = [];
                        if (assignedResponse && assignedResponse.data) {
                            assignedIds = assignedResponse.data.map(function (x) { return x.pan_Id; });
                        }
                        var pantallas = allResponse.data || allResponse;
                        buildTree(container, pantallas, assignedIds);
                    },
                    error: function () {
                        container.html('<p class="text-danger text-center py-3">Error al cargar pantallas asignadas</p>');
                    }
                });
            },
            error: function () {
                container.html('<p class="text-danger text-center py-3">Error al cargar pantallas</p>');
            }
        });
    }

    function buildTree(container, pantallas, assignedIds) {
        if (!pantallas || pantallas.length === 0) {
            container.html('<p class="text-muted text-center py-3">No hay pantallas disponibles</p>');
            return;
        }

        // Group by pan_Grupo
        var grupos = {};
        $.each(pantallas, function (i, p) {
            var grupo = p.pan_Grupo || 'General';
            if (!grupos[grupo]) grupos[grupo] = [];
            grupos[grupo].push(p);
        });

        var grupoIcons = {
            'Home': 'fas fa-home',
            'Cuenta': 'fas fa-user',
            'Inventario': 'fas fa-box-open',
            'Administracion': 'fas fa-cogs',
            'Adopcion': 'fas fa-dog',
            'Medicamento': 'fas fa-suitcase-medical',
            'Seguridad': 'fas fa-shield-alt'
        };

        var html = '<ul class="pantallas-tree">';

        $.each(grupos, function (grupoName, items) {
            var grupoId = grupoName.replace(/\s+/g, '_').toLowerCase();
            var icon = grupoIcons[grupoName] || 'fas fa-folder';

            var totalItems = items.length;
            var checkedItems = 0;
            $.each(items, function (i, p) {
                if (assignedIds.indexOf(p.pan_Id) >= 0) checkedItems++;
            });

            html += '<li class="grupo-item">';
            html += '<div class="grupo-header" data-grupo="' + grupoId + '">';
            html += '<div><i class="' + icon + ' grupo-icon"></i>' + grupoName + ' <span class="badge badge-secondary grupo-badge" data-grupo="' + grupoId + '">' + checkedItems + '/' + totalItems + '</span></div>';
            html += '<div class="grupo-actions">';
            html += '<button type="button" class="grupo-toggle-all" data-grupo="' + grupoId + '">marcar todo</button>';
            html += '<i class="fas fa-chevron-down chevron" data-grupo="' + grupoId + '"></i>';
            html += '</div>';
            html += '</div>';
            html += '<div class="grupo-content" data-grupo="' + grupoId + '">';

            $.each(items, function (i, p) {
                var checked = assignedIds.indexOf(p.pan_Id) >= 0 ? 'checked' : '';
                html += '<div class="pantalla-item">';
                html += '<input type="checkbox" class="pantalla-check" id="pan_' + p.pan_Id + '" value="' + p.pan_Id + '" data-grupo="' + grupoId + '" ' + checked + '>';
                html += '<label for="pan_' + p.pan_Id + '">' + p.pan_Descripcion + '</label>';
                html += '</div>';
            });

            html += '</div>';
            html += '</li>';
        });

        html += '</ul>';
        container.html(html);

        // Bind tree events
        bindTreeEvents();
    }

    function bindTreeEvents() {
        // Toggle group expand/collapse
        $('.grupo-header').off('click').on('click', function (e) {
            if ($(e.target).hasClass('grupo-toggle-all') || $(e.target).closest('.grupo-toggle-all').length) return;
            var grupoId = $(this).data('grupo');
            $('.grupo-content[data-grupo="' + grupoId + '"]').slideToggle(200);
            $(this).find('.chevron').toggleClass('collapsed');
        });

        // Toggle all in group
        $('.grupo-toggle-all').off('click').on('click', function (e) {
            e.stopPropagation();
            var grupoId = $(this).data('grupo');
            var $checks = $('.pantalla-check[data-grupo="' + grupoId + '"]');
            var allChecked = $checks.length === $checks.filter(':checked').length;
            $checks.prop('checked', !allChecked);
            updateGroupBadge(grupoId);
        });

        // Update badge on checkbox change
        $('.pantalla-check').off('change').on('change', function () {
            var grupoId = $(this).data('grupo');
            updateGroupBadge(grupoId);
        });
    }

    function updateGroupBadge(grupoId) {
        var $checks = $('.pantalla-check[data-grupo="' + grupoId + '"]');
        var total = $checks.length;
        var checked = $checks.filter(':checked').length;
        $('.grupo-badge[data-grupo="' + grupoId + '"]').text(checked + '/' + total);
    }

    function updateAllGroupBadges() {
        $('.grupo-badge').each(function () {
            updateGroupBadge($(this).data('grupo'));
        });
    }

    // ==========================================
    // Public methods
    // ==========================================

    obj.edit = function (id) {
        $.ajax({
            url: urls.urlFind,
            type: "GET",
            data: { id: id },
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    var item = response.item;
                    $('#item-id').val(item.rol_Id);
                    $('#rol_Descripcion').val(item.rol_Descripcion);
                    $('#rol_Estado').prop('checked', item.rol_Estado === true || item.rol_Estado === "True" || item.rol_Estado === "true");
                    $('#edit-modal .modal-title').html('<i class="fas fa-shield-alt mr-2"></i>Editar Rol');
                    $('#edit-modal').modal('show');
                } else {
                    toastr.error(response.message || 'No se encontro el rol');
                }
            },
            error: function () {
                toastr.error('Error de conexion');
            }
        });
    };

    obj.confirmDelete = function (id) {
        var name = '';
        dt.rows().every(function () {
            var d = this.data();
            if (d.rol_Id === id) {
                name = d.rol_Descripcion;
            }
        });
        $('#delete-item-id').val(id);
        $('#delete-item-name').text(name);
        $('#delete-modal').modal('show');
    };

    obj.openPantallas = function (id) {
        $('#pantallas-rol-id').val(id);
        loadPantallasTree(id);
        $('#pantallas-modal').modal('show');
    };

    return obj;
}());
