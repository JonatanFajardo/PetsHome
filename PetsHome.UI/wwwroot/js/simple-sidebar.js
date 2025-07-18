$(document).ready(function () {
    console.log('🎨 Sidebar mejorado iniciado');

    // Crear overlay para móvil
    if (!$('.sidebar-overlay').length) {
        $('body').append('<div class="sidebar-overlay"></div>');
    }

    // Toggle del sidebar
    $('.sidebarCollapse').on('click', function (e) {
        e.preventDefault();
        console.log('🖱️ Toggle sidebar');

        $('.sidebar-wrapper').toggleClass('sidebar-hidden');

        // Manejo responsive
        if ($(window).width() <= 768) {
            // Móvil: mostrar/ocultar overlay
            if ($('.sidebar-wrapper').hasClass('sidebar-hidden')) {
                $('.sidebar-overlay').removeClass('show');
            } else {
                $('.sidebar-overlay').addClass('show');
            }
        } else {
            // Desktop: ajustar margin del contenido
            if ($('.sidebar-wrapper').hasClass('sidebar-hidden')) {
                $('#content').css('margin-left', '0');
            } else {
                $('#content').css('margin-left', '260px');
            }
        }
    });

    // Cerrar sidebar al hacer click en overlay
    $('.sidebar-overlay').on('click', function() {
        console.log('🖱️ Cerrar sidebar desde overlay');
        $('.sidebar-wrapper').addClass('sidebar-hidden');
        $('.sidebar-overlay').removeClass('show');
    });

    // Manejo responsive en resize
    $(window).on('resize', function() {
        if ($(window).width() <= 768) {
            // Móvil: resetear margin y manejar overlay
            $('#content').css('margin-left', '0');
            if (!$('.sidebar-wrapper').hasClass('sidebar-hidden')) {
                $('.sidebar-overlay').addClass('show');
            }
        } else {
            // Desktop: resetear overlay y manejar margin
            $('.sidebar-overlay').removeClass('show');
            if ($('.sidebar-wrapper').hasClass('sidebar-hidden')) {
                $('#content').css('margin-left', '0');
            } else {
                $('#content').css('margin-left', '260px');
            }
        }
    });

    // Marcar enlace activo
    $('.sidebar-wrapper .menu a').on('click', function() {
        $('.sidebar-wrapper .menu a').removeClass('active');
        $(this).addClass('active');
    });

    // Animación de entrada progresiva
    $('.sidebar-wrapper .menu').each(function(index) {
        $(this).css('animation-delay', (index * 0.1) + 's');
    });

    console.log('✅ Sidebar mejorado listo');
});