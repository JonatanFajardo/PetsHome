$(document).ready(function () { 

    $('.sidebarCollapse').on('click', function (e) {
        e.preventDefault(); 

        $('.sidebar-wrapper').toggleClass('sidebar-hidden');

        // Ajustar margin-left del content
        if ($('.sidebar-wrapper').hasClass('sidebar-hidden')) {
            $('#content').css('margin-left', '0');
        } else {
            $('#content').css('margin-left', '255px');
        }
         
    });
});