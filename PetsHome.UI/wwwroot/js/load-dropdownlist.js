var dropdown = (function () {
    var obj = {};

    function addOptions(domElement, array) {
        var select = document.getElementsByName(domElement)[0];

        for (value in array) {
            var option = document.createElement("option");
            option.text = array[value];
            select.add(option);
        }
    }

    function sexo() {
        var array = ["Masculino", "Femenino"];
        array.sort();
        addOptions("load-dropdown-sexo", array);
    }

    // Función para cargar mascotas via AJAX
    function cargarMascotas() {
        $.ajax({
            url: '/CitaMedica/GetMascotasDropdown', // Cambia por tu URL del controlador
            type: 'GET',
            dataType: 'json',
            beforeSend: function () {
                // Mostrar loading
                $('#mascota-dropdown .searchable-input').attr('placeholder', 'Cargando mascotas...');
                $('#mascota-dropdown .dropdown-toggle-btn').prop('disabled', true);
            },
            success: function (data) {
                // Limpiar opciones existentes
                $('#mascota-dropdown .original-select').empty();
                $('#mascota-dropdown .dropdown-menu').empty();

                // Agregar opción por defecto
                $('#mascota-dropdown .original-select').append('<option value="">-- Seleccionar Mascota --</option>');

                // Agregar mascotas al dropdown
                $.each(data, function (index, mascota) {

                    // Agregar al select original (para el form) - CORREGIDO
                    $('#mascota-dropdown .original-select').append(
                        '<option value="' + mascota.masc_Id + '">' + mascota.masc_Nombre + '</option>'
                    );

                    // Agregar al dropdown menu personalizado - CORREGIDO
                    $('#mascota-dropdown .dropdown-menu').append(
                        '<div class="dropdown-item" data-value="' + mascota.masc_Id + '">' +
                        '<i class="fas fa-paw text-primary me-2"></i>' +
                        '<span class="mascota-nombre">' + mascota.masc_Nombre + '</span>' +
                        '</div>'
                    );
                });

                // Restaurar placeholder
                $('#mascota-dropdown .searchable-input').attr('placeholder', 'Buscar mascota...');
                $('#mascota-dropdown .dropdown-toggle-btn').prop('disabled', false);
            },
            error: function (xhr, status, error) {
                console.error('Error al cargar mascotas:', error);
                $('#mascota-dropdown .searchable-input').attr('placeholder', 'Error al cargar mascotas');

                // Mostrar mensaje de error
                $('#mascota-dropdown .dropdown-menu').html(
                    '<div class="dropdown-item-text text-danger">' +
                    '<i class="fas fa-exclamation-triangle me-2"></i>' +
                    'Error al cargar las mascotas' +
                    '</div>'
                );
            }
        });
    }

    // Función para configurar el dropdown searchable
    function configurarDropdownSearchable() {
        const $dropdown = $('#mascota-dropdown');
        const $input = $dropdown.find('.searchable-input');
        const $menu = $dropdown.find('.dropdown-menu');
        const $select = $dropdown.find('.original-select');
        const $toggleBtn = $dropdown.find('.dropdown-toggle-btn');
        const $clearBtn = $dropdown.find('.clear-selection');

        // Remover el readonly inicial para permitir clicks
        $input.prop('readonly', false);

        // Mostrar/ocultar dropdown
        $input.on('click', function () {
            console.log('Input clicked');
            if (!$(this).prop('disabled')) {
                $menu.toggleClass('show');
            }
        });

        $toggleBtn.on('click', function (e) {
            e.preventDefault();
            console.log('Toggle button clicked');
            $menu.toggleClass('show');
            $input.focus();
        });

        // Habilitar búsqueda
        $input.on('input', function () {
            const searchTerm = $(this).val().toLowerCase();
            console.log('Searching for:', searchTerm);

            $menu.find('.dropdown-item').each(function () {
                const text = $(this).text().toLowerCase();
                if (text.includes(searchTerm)) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });

            $menu.addClass('show');
        });

        // Seleccionar item
        $menu.on('click', '.dropdown-item', function (e) {
            e.preventDefault();
            const value = $(this).data('value');
            const text = $(this).find('.mascota-nombre').text();

            console.log('Selected:', value, text);

            $select.val(value);
            $input.val(text);
            $input.prop('readonly', true);
            $menu.removeClass('show');
            $clearBtn.show();

            // Trigger change event para validaciones
            $select.trigger('change');
        });

        // Limpiar selección
        $clearBtn.on('click', function (e) {
            e.preventDefault();
            $select.val('');
            $input.val('').prop('readonly', false);
            $clearBtn.hide();
            $menu.removeClass('show');
            $input.focus();
        });

        // Cerrar dropdown al hacer click fuera
        $(document).on('click', function (e) {
            if (!$dropdown.is(e.target) && $dropdown.has(e.target).length === 0) {
                $menu.removeClass('show');
            }
        });
    }

    // Función para recargar mascotas (útil para actualizaciones)
    function recargarMascotas() {
        // Limpiar selección actual
        $('#mascota-dropdown .clear-selection').click();

        // Recargar datos
        cargarMascotas();
    }
     

    // Función de inicialización de mascotas
    function inicializarMascotas() {
        // Cargar mascotas al inicializar
        cargarMascotas();

        // Configurar el dropdown searchable
        configurarDropdownSearchable();
    }

    // Exponer funciones públicas
    obj.load = function () {
        sexo();
    };

    obj.inicializarMascotas = inicializarMascotas;
    obj.recargarMascotas = recargarMascotas; 

    return obj;
}());

// Inicialización cuando el DOM esté listo
$(function () {
    dropdown.load();

    //// Inicializar mascotas si el elemento existe en la página
    //if ($('#mascota-dropdown').length > 0) {
    //    dropdown.inicializarMascotas();
    //}
});