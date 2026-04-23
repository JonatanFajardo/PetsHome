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
                    render: function (data, type) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af;">—</span>';
                            var label = data.toString().toLowerCase().replace(/\b\w/g, function (l) { return l.toUpperCase(); });
                            return '<span style="display:inline-flex;align-items:center;gap:8px;">'
                                 +   '<span style="display:inline-flex;align-items:center;justify-content:center;'
                                 +         'width:28px;height:28px;border-radius:6px;background:#ede9fe;">'
                                 +     '<i class="fas fa-shield-alt" style="color:#7c3aed;font-size:13px;"></i>'
                                 +   '</span>'
                                 +   '<span style="font-weight:500;color:#111827;">' + label + '</span>'
                                 + '</span>';
                        }
                        return data;
                    }
                },
                {
                    data: "cantidadPantallas",
                    width: 120,
                    className: "text-center",
                    render: function (data, type) {
                        if (type === 'display') {
                            var n = data || 0;
                            return '<span style="display:inline-block;padding:2px 10px;border-radius:9999px;'
                                 + 'font-size:12px;font-weight:600;background:#ede9fe;color:#7c3aed;">'
                                 + n + (n === 1 ? ' pantalla' : ' pantallas') + '</span>';
                        }
                        return data;
                    }
                },
                {
                    data: "rol_Estado",
                    width: 100,
                    render: function (data, type) {
                        if (type === 'display') {
                            var isActive = data === true || data === 1 || data === "Activo" || data === "True" || data === "true";
                            return '<span class="status-badge ' + (isActive ? 'status-activo' : 'status-inactivo') + '">'
                                 + (isActive ? 'Activo' : 'Inactivo') + '</span>';
                        }
                        return data;
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
            $('.crud-visible').prop('checked', true);
            $('.crud-check').prop('checked', true).prop('disabled', false);
            updateAllGroupBadges();
        });

        $('#btn-deselect-all').on('click', function () {
            $('.crud-visible').prop('checked', false);
            $('.crud-check').prop('checked', false).prop('disabled', true);
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

        // Save pantallas with CRUD permissions
        $('#btn-save-pantallas').on('click', function () {
            var rolId = $('#pantallas-rol-id').val();
            var permisos = [];

            $('.crud-row').each(function () {
                var panId = $(this).data('pan-id');
                var consultar = $('.crud-check[data-pan-id="' + panId + '"][data-crud="consultar"]').is(':checked');
                var insertar = $('.crud-check[data-pan-id="' + panId + '"][data-crud="insertar"]').is(':checked');
                var editar = $('.crud-check[data-pan-id="' + panId + '"][data-crud="editar"]').is(':checked');
                var eliminar = $('.crud-check[data-pan-id="' + panId + '"][data-crud="eliminar"]').is(':checked');

                if (consultar || insertar || editar || eliminar) {
                    permisos.push({
                        pan_Id: panId,
                        ropan_Consultar: consultar,
                        ropan_Insertar: insertar,
                        ropan_Editar: editar,
                        ropan_Eliminar: eliminar
                    });
                }
            });

            var $btn = $(this);
            $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin mr-1"></i>Guardando...');

            $.ajax({
                url: urls.urlSavePantallas,
                type: "POST",
                data: { rolId: rolId, permisosJson: JSON.stringify(permisos) },
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

        $.ajax({
            url: urls.urlPantallasList,
            type: "GET",
            dataType: "json",
            success: function (allResponse) {
                $.ajax({
                    url: urls.urlPantallasByRol + "?id=" + rolId,
                    type: "GET",
                    dataType: "json",
                    success: function (assignedResponse) {
                        var permisos = {};
                        if (assignedResponse && assignedResponse.data) {
                            $.each(assignedResponse.data, function (i, p) {
                                permisos[p.pan_Id] = {
                                    consultar: p.ropan_Consultar,
                                    insertar: p.ropan_Insertar,
                                    editar: p.ropan_Editar,
                                    eliminar: p.ropan_Eliminar
                                };
                            });
                        }
                        var pantallas = allResponse.data || allResponse;
                        buildTree(container, pantallas, permisos);
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

    var crudLabels = [
        { key: 'consultar', label: 'Consultar', icon: 'fa-eye' },
        { key: 'insertar', label: 'Insertar', icon: 'fa-plus' },
        { key: 'editar', label: 'Editar', icon: 'fa-edit' },
        { key: 'eliminar', label: 'Eliminar', icon: 'fa-trash' }
    ];

    function buildTree(container, pantallas, permisos) {
        if (!pantallas || pantallas.length === 0) {
            container.html('<p class="text-muted text-center py-3">No hay pantallas disponibles</p>');
            return;
        }

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

            html += '<li class="grupo-item">';
            html += '<div class="grupo-header" data-grupo="' + grupoId + '">';
            html += '<div><i class="' + icon + ' grupo-icon"></i>' + grupoName + ' <span class="badge badge-secondary grupo-badge" data-grupo="' + grupoId + '"></span></div>';
            html += '<div class="grupo-actions">';
            html += '<button type="button" class="grupo-toggle-all" data-grupo="' + grupoId + '">marcar todo</button>';
            html += '<i class="fas fa-chevron-down chevron" data-grupo="' + grupoId + '"></i>';
            html += '</div>';
            html += '</div>';
            html += '<div class="grupo-content" data-grupo="' + grupoId + '">';

            html += '<table class="crud-table">';
            html += '<thead><tr>';
            html += '<th class="crud-th-pantalla">Pantalla</th>';
            html += '<th class="crud-th-visible"><i class="fas fa-eye" title="Visible en menu"></i> Visible</th>';
            $.each(crudLabels, function (i, c) {
                html += '<th class="crud-th"><i class="fas ' + c.icon + '" title="' + c.label + '"></i> ' + c.label + '</th>';
            });
            html += '</tr></thead><tbody>';

            $.each(items, function (i, p) {
                var perm = permisos[p.pan_Id] || {};
                var anyChecked = perm.consultar || perm.insertar || perm.editar || perm.eliminar;

                html += '<tr class="crud-row" data-pan-id="' + p.pan_Id + '" data-grupo="' + grupoId + '">';
                html += '<td class="crud-td-pantalla">' + p.pan_Descripcion + '</td>';
                html += '<td class="crud-td-check"><input type="checkbox" class="crud-visible" data-pan-id="' + p.pan_Id + '" ' + (anyChecked ? 'checked' : '') + ' title="Visible en menu"></td>';

                $.each(crudLabels, function (i, c) {
                    var checked = perm[c.key] ? 'checked' : '';
                    var disabled = !anyChecked ? 'disabled' : '';
                    html += '<td class="crud-td-check"><input type="checkbox" class="crud-check" data-pan-id="' + p.pan_Id + '" data-crud="' + c.key + '" data-grupo="' + grupoId + '" ' + checked + ' ' + disabled + '></td>';
                });

                html += '</tr>';
            });

            html += '</tbody></table>';
            html += '</div>';
            html += '</li>';
        });

        html += '</ul>';
        container.html(html);

        updateAllGroupBadges();
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

        // Toggle all in group (mark all visible + all CRUD)
        $('.grupo-toggle-all').off('click').on('click', function (e) {
            e.stopPropagation();
            var grupoId = $(this).data('grupo');
            var $visibles = $('.crud-visible').filter(function () { return $(this).closest('.crud-row').data('grupo') === grupoId; });
            var $checks = $('.crud-check[data-grupo="' + grupoId + '"]');
            var allChecked = $visibles.length === $visibles.filter(':checked').length && $checks.length === $checks.filter(':checked').length;
            var newState = !allChecked;
            $visibles.prop('checked', newState);
            $checks.prop('checked', newState).prop('disabled', !newState);
            updateGroupBadge(grupoId);
        });

        // Visible checkbox - controls sidebar visibility and enables/disables CRUD
        $('.crud-visible').off('change').on('change', function () {
            var panId = $(this).data('pan-id');
            var isChecked = $(this).is(':checked');
            var $crudChecks = $('.crud-check[data-pan-id="' + panId + '"]');

            if (isChecked) {
                // Al marcar visible, habilitar CRUDs y marcar Consultar por defecto
                $crudChecks.prop('disabled', false);
                $crudChecks.filter('[data-crud="consultar"]').prop('checked', true);
            } else {
                // Al desmarcar visible, desmarcar y deshabilitar todos los CRUD
                $crudChecks.prop('checked', false).prop('disabled', true);
            }

            var grupoId = $(this).closest('.crud-row').data('grupo');
            updateGroupBadge(grupoId);
        });

        // Individual CRUD checkbox change
        $('.crud-check').off('change').on('change', function () {
            var panId = $(this).data('pan-id');
            var grupoId = $(this).data('grupo');
            // Si se desmarcaron todos los CRUD, desmarcar visible
            var $crudChecks = $('.crud-check[data-pan-id="' + panId + '"]');
            var anyChecked = $crudChecks.filter(':checked').length > 0;
            $('.crud-visible[data-pan-id="' + panId + '"]').prop('checked', anyChecked);
            if (!anyChecked) {
                $crudChecks.prop('disabled', true);
            }
            updateGroupBadge(grupoId);
        });
    }

    function updateGroupBadge(grupoId) {
        var $rows = $('.crud-row[data-grupo="' + grupoId + '"]');
        var total = $rows.length;
        var withPerms = 0;
        $rows.each(function () {
            var panId = $(this).data('pan-id');
            if ($('.crud-check[data-pan-id="' + panId + '"]:checked').length > 0) withPerms++;
        });
        $('.grupo-badge[data-grupo="' + grupoId + '"]').text(withPerms + '/' + total);
    }

    function updateAllGroupBadges() {
        var grupos = [];
        $('.grupo-badge').each(function () {
            var g = $(this).data('grupo');
            if (grupos.indexOf(g) < 0) grupos.push(g);
        });
        $.each(grupos, function (i, g) { updateGroupBadge(g); });
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
