var Catalogs = (function () {
    var obj = {},
        table,
        formId,
        $editModal,
        $deleteModal,
        action,
        submitBtn;

    /**
     * Evento que le envia al controlador el id para editar un registro, levanta el modal,
     * @param {any} params
     */
    function createDataTable(params) {
        table = $("#datatable");

        // Eventos que se levantan al dar clic en el boton de editar en datatable.
        table.on("click", ".edit-btn", function (e) {
            var id = $(this).data("id");
            var btn = Ladda.create($(this)[0]); //button spin animation
            action = "edit";
            $.ajax({
                type: "GET",
                url: `${params.getUrl}/${id}`,
                dataType: "json",
                beforeSend: function () {
                    btn.start();
                },
                success: function (response) {
                    if (response.success) {
                        assignSettings(response.item);
                        $editModal.modal("show");
                    }
                    else {
                        alertConfig.alert(response.message, 'error');
                    }
                },
                complete: function () {
                    btn.stop();
                }
            });
        });

        // Eventos que se levantan al dar clic en el boton de detalle en datatable.
        table.on("click", ".details-btn", function (e) {
            var id = $(this).data("id");
            var btn = Ladda.create($(this)[0]); //button spin animation
            action = "details";
            $.ajax({
                type: "GET",
                url: `${params.getUrl}/${id}`,
                dataType: "json",
                beforeSend: function () {
                    btn.start();
                },
                success: function (response) {
                    if (!response.success) {
                        assignSettings(response.item);
                        $editModal.modal("show");
                    }
                    else {
                        alertConfig.alert(response.message, response.type);
                    }
                },
                complete: function () {
                    btn.stop();
                }
            });
        });

        //Resetea los valores.
        $("#add-btn").click(function () {
            console.log("click");
            $(formId).trigger("reset");
            $("#item-id").val("0");
            action = "edit";
        });

        table.on("click", ".delete-btn", function (e) {
            var id = $(this).data("id"); 
            console.log(id);
            var btn = Ladda.create($(this)[0]);
            console.log('btn delete: ' + params.getUrl);
            action = "delete";
            $.ajax({
                type: "GET",
                url: `${params.getUrl}/${id}`, 
                dataType: "json",
                beforeSend: function () {
                    btn.start();
                },
                success: function (response) {
                    if (response.success) {
                        $("#delete-item-id").val(id);
                        //alertConfig.alert('Eliminado correctamente.', 'success');
                        $deleteModal.modal("show");
                    }
                    else {
                        alertConfig.alert('Parece que ocurrio un error inesperado.', 'error');
                    }
                },
                complete: function () {
                    btn.stop();
                }
            });
        });
    }

    // Assing ajax values to inputs in edit form
    var assignValue = function (n, v) {

        // obtiene el input con el name de la propiedad enviada
        var $input = $('#edit-modal form input[name="' + n + '" i]');
        if ($input.length > 0) {
            if ($input.attr("type") == "text" || $input.attr("type") == "hidden") {
                $input.val(v);
            } else if ($input.attr("type") == "checkbox") {
                $input.prop("checked", v);
            }
        }
        else {
            $input = $('#edit-modal form select[name="' + n + '" i]');
            if ($input.length > 0) {
                $input.val(v);
                if ($input.hasClass("selectpicker")) {
                    $input.selectpicker("refresh");
                }
            }
            else {
                $input = $('#edit-modal form textarea[name="' + n + '" i]');
                if ($input.length > 0) {
                    $input.val(v);
                }
            }
        }
    }


    var assignSettings = function (list, prefix) {
        //viene indefinido cuando se ejecuta desde el edit entonces se instancia el prefix como un string
        if (typeof prefix != "undefined") {
            prefix += ".";
        } else {
            prefix = "";
        }

        for (var item in list) {
            if ((typeof list[item] == "string") || (typeof list[item] == "boolean") || typeof list[item] == "number") {
                assignValue(prefix + item, list[item]);
            } else if (typeof list[item] == "object") {
                var n1 = item;
                assignSettings(list[n1], prefix + n1);
            }
        }
    }

    /**
     * Configura y establece los valores del modal.
     * @param {any} params
     */
    function createEditModal(params) {
        $editModal = $(params.editModalId);
        formId = params.editModalId + " form:first";

        $editModal.on("show.bs.modal", function () {
            var modalTitle = "Agregar ", saveBtnText = "Guardar";

            //si el id es igual a 0 que le asigne otros valores al titulo y al boton de guardar
            if ($("#item-id").val() && $("#item-id").val() != "0") {
                modalTitle = "Editar ";
                saveBtnText = "Guardar cambios";
            }

            $(params.editModalId + " .modal-title").html(modalTitle + params.displayName);
            $(params.editModalId + " .modal-footer .btn-primary").html("<i class='mdi mdi-content-save'></i> " + saveBtnText);

            //le asigna el focus al primer input del primer formulario despues de pasados los 500s
            setTimeout(function () {
                $(formId + " *:input[type!=hidden]:first").focus();
            }, 500);
        });


        $editModal.on("hidden.bs.modal", function () {
            $(":input", formId).not(':button, :submit, :reset, input[name="__RequestVerificationToken"]').val("").removeAttr("readonly");

            //$(".selectpicker").val("").selectpicker("refresh");

            var validator = $(formId).validate();
            $('[name]', formId).each(function () {
                validator.successList.push(this);
                validator.showErrors();
            });
            validator.resetForm();
            validator.reset();
            $(".input-validation-error").removeClass("input-validation-error");
        });


    }

    /**
     * Configura y establece los valores del modal.
     * @param {any} params
     */
    function createDeleteModal(params) {

        $deleteModal = $(params.deleteModalId);
        $deleteModal.on("show.bs.modal", function () {
            var modalTitle = "Eliminar ", saveBtnText = "Aceptar";
            $(params.deleteModalId + " .modal-title").html(modalTitle + params.displayName);
            $(params.deleteModalId + " .modal-footer .btn-danger").html("<i class='mdi mdi-content-save'></i> " + saveBtnText);

        });


        $deleteModal.on("hidden.bs.modal", function () {});
    }






    obj.configure = function (params) {
        //if (params.dataTableId === undefined)
        //    params.dataTableId = "#datatable";
        if (params.editModalId === undefined)
            params.editModalId = "#edit-modal";

        if (params.deleteModalId === undefined)
            params.deleteModalId = "#delete-modal";

        $(function () {
            createDataTable(params);
            createDeleteModal(params);
            createEditModal(params);
        });
    };

    obj.begin = function (xhr, settings) {
        if (action == "edit") {
            submitBtn = Ladda.create($(".ladda-button")[0]);
        }
        else {
            submitBtn = Ladda.create($(".ladda-button")[0]);
        }
        submitBtn.start();
    };

    obj.success = function (data, status, xhr) {
        
        if (data.success) {
            $editModal.modal("hide");
            $deleteModal.modal("hide");
            table.DataTable().ajax.reload(null, false);
        }
        else {
            $editModal.modal("hide");
            $deleteModal.modal("hide");
            alertConfig.alert(data.message, 'error');
        }
        alertConfig.alert(data.message, data.type);
    };

    obj.failure = function (xhr, status, error) {
        console.log("Ocurrió  un error.");
    }

    obj.complete = function (xhr, status) {
        submitBtn.stop();
    };

    return obj;
}());



