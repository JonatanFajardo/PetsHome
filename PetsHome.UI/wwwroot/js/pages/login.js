/**
 * Funcionalidad JavaScript para el módulo de Login
 */

$(document).ready(function() {
    initializeLoginForm();
    initializeFormValidation();
});

function initializeLoginForm() {
    // Enfoque automático en el campo de usuario
    $('#Usu_Nombre').focus();
    
    // Mostrar/ocultar contraseña
    if ($('.password-toggle').length > 0) {
        $('.password-toggle').on('click', function(e) {
            e.preventDefault();
            togglePasswordVisibility($(this));
        });
    }
    
    // Validación en tiempo real
    $('#Usu_Nombre').on('blur', function() {
        validateUsuario($(this));
    });
    
    $('#Contrasena').on('blur', function() {
        validateContrasena($(this));
    });
    
    // Prevenir submit múltiple
    $('form').on('submit', function() {
        var submitBtn = $(this).find('button[type="submit"]');
        if (submitBtn.hasClass('disabled')) {
            return false;
        }
        
        submitBtn.addClass('disabled').html('<i class="fas fa-spinner fa-spin mr-2"></i>Iniciando...');
        
        // Habilitar botón después de 5 segundos para evitar bloqueo permanente
        setTimeout(function() {
            submitBtn.removeClass('disabled').html('<i class="fas fa-sign-in-alt mr-2"></i>Iniciar Sesión');
        }, 5000);
    });
}

function initializeFormValidation() {
    // Validación personalizada del formulario
    $('form').on('submit', function(e) {
        var isValid = true;
        
        // Validar usuario
        var usuario = $('#Usu_Nombre');
        if (!validateUsuario(usuario)) {
            isValid = false;
        }
        
        // Validar contraseña
        var contrasena = $('#Contrasena');
        if (!validateContrasena(contrasena)) {
            isValid = false;
        }
        
        if (!isValid) {
            e.preventDefault();
            return false;
        }
    });
}

function validateUsuario(input) {
    var value = input.val().trim();
    var errorSpan = input.siblings('.text-danger');
    
    if (value === '') {
        showFieldError(input, errorSpan, 'El usuario es requerido');
        return false;
    }
    
    if (value.length < 3) {
        showFieldError(input, errorSpan, 'El usuario debe tener al menos 3 caracteres');
        return false;
    }
    
    hideFieldError(input, errorSpan);
    return true;
}

function validateContrasena(input) {
    var value = input.val();
    var errorSpan = input.siblings('.text-danger');
    
    if (value === '') {
        showFieldError(input, errorSpan, 'La contraseña es requerida');
        return false;
    }
    
    if (value.length < 6) {
        showFieldError(input, errorSpan, 'La contraseña debe tener al menos 6 caracteres');
        return false;
    }
    
    hideFieldError(input, errorSpan);
    return true;
}

function showFieldError(input, errorSpan, message) {
    input.addClass('is-invalid');
    errorSpan.text(message).show();
}

function hideFieldError(input, errorSpan) {
    input.removeClass('is-invalid');
    errorSpan.hide();
}

function togglePasswordVisibility(toggleBtn) {
    var passwordInput = toggleBtn.siblings('input[type="password"], input[type="text"]');
    var icon = toggleBtn.find('i');
    
    if (passwordInput.attr('type') === 'password') {
        passwordInput.attr('type', 'text');
        icon.removeClass('fa-eye').addClass('fa-eye-slash');
    } else {
        passwordInput.attr('type', 'password');
        icon.removeClass('fa-eye-slash').addClass('fa-eye');
    }
}

// Funciones para recordar usuario (opcional)
function saveRememberMe() {
    var usuario = $('#Usu_Nombre').val();
    var rememberMe = $('#RememberMe').is(':checked');
    
    if (rememberMe) {
        localStorage.setItem('rememberedUser', usuario);
    } else {
        localStorage.removeItem('rememberedUser');
    }
}

function loadRememberMe() {
    var rememberedUser = localStorage.getItem('rememberedUser');
    if (rememberedUser) {
        $('#Usu_Nombre').val(rememberedUser);
        $('#RememberMe').prop('checked', true);
    }
}

// Cargar usuario recordado al inicializar
$(document).ready(function() {
    loadRememberMe();
    
    // Guardar usuario cuando se envía el formulario
    $('form').on('submit', function() {
        saveRememberMe();
    });
});

// Funciones de utilidad para mensajes
function showAlert(message, type = 'info') {
    var alertClass = 'alert-' + type;
    var iconClass = getAlertIcon(type);
    
    var alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="${iconClass} mr-2"></i>
            ${message}
            <button type="button" class="close" data-dismiss="alert">
                <span>&times;</span>
            </button>
        </div>
    `;
    
    $('.login-body').prepend(alertHtml);
    
    // Auto-ocultar después de 5 segundos
    setTimeout(function() {
        $('.alert').alert('close');
    }, 5000);
}

function getAlertIcon(type) {
    switch(type) {
        case 'success': return 'fas fa-check-circle';
        case 'danger': return 'fas fa-exclamation-circle';
        case 'warning': return 'fas fa-exclamation-triangle';
        case 'info': 
        default: return 'fas fa-info-circle';
    }
}

// Manejo de errores de conectividad
$(document).ajaxError(function(event, xhr, settings) {
    if (xhr.status === 0) {
        showAlert('Error de conectividad. Verifique su conexión a internet.', 'danger');
    } else if (xhr.status >= 500) {
        showAlert('Error interno del servidor. Intente nuevamente.', 'danger');
    }
});

// Limpieza al salir de la página
$(window).on('beforeunload', function() {
    // Limpiar datos sensibles si es necesario
    if (!$('#RememberMe').is(':checked')) {
        $('#Contrasena').val('');
    }
});