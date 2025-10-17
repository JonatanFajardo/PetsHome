// Empleado Form Validation
// Modern form validation for ASP.NET MVC

(function () {
    'use strict';

    // Wait for DOM to be ready
    document.addEventListener('DOMContentLoaded', function () {
        const estadoCheckbox = document.getElementById('emp_EsActivo');
        const estadoStatus = document.getElementById('estadoStatus');

        // Estado Switch Handler
        if (estadoCheckbox && estadoStatus) {
            // Set initial state based on checkbox value
            estadoStatus.textContent = estadoCheckbox.checked ? 'Activo' : 'Inactivo';

            // Update status text when checkbox changes
            estadoCheckbox.addEventListener('change', function (e) {
                estadoStatus.textContent = e.target.checked ? 'Activo' : 'Inactivo';
            });
        }

        // Form Validation Rules
        const validationRules = {
            'per_per_PrimerNombre': {
                required: true,
                minLength: 2,
                message: 'El primer nombre es requerido y debe tener al menos 2 caracteres'
            },
            'per_per_ApellidoPaterno': {
                required: true,
                minLength: 2,
                message: 'El apellido paterno es requerido y debe tener al menos 2 caracteres'
            },
            'per_per_ApellidoMaterno': {
                required: true,
                minLength: 2,
                message: 'El apellido materno es requerido y debe tener al menos 2 caracteres'
            },
            'per_per_Identidad': {
                required: true,
                minLength: 5,
                message: 'La identidad es requerida y debe tener al menos 5 caracteres'
            },
            'per_per_FechaNacimiento': {
                required: true,
                message: 'La fecha de nacimiento es requerida'
            },
            'per_per_Correo': {
                required: true,
                email: true,
                message: 'Debe ingresar un correo electrónico válido'
            },
            'per_per_Telefono': {
                required: true,
                minLength: 8,
                message: 'El teléfono debe tener al menos 8 dígitos'
            },
            'per_per_Domicilio': {
                required: true,
                minLength: 10,
                message: 'El domicilio debe tener al menos 10 caracteres'
            },
            'emp_Codigo': {
                required: true,
                message: 'El código es requerido'
            },
            'refg_Id': {
                required: true,
                message: 'Debe seleccionar un refugio'
            },
            'cag_Id': {
                required: true,
                message: 'Debe seleccionar un cargo'
            }
        };

        // Validation function
        function validateField(fieldName, value) {
            const rules = validationRules[fieldName];
            if (!rules) return { valid: true };

            // Required validation
            if (rules.required && (!value || value.trim() === '')) {
                return { valid: false, message: rules.message };
            }

            // Skip other validations if field is not required and empty
            if (!rules.required && (!value || value.trim() === '')) {
                return { valid: true };
            }

            // Min length validation
            if (rules.minLength && value.length < rules.minLength) {
                return { valid: false, message: rules.message };
            }

            // Email validation
            if (rules.email) {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(value)) {
                    return { valid: false, message: rules.message };
                }
            }

            return { valid: true };
        }

        // Show error function
        function showError(fieldName, message) {
            const field = document.getElementById(fieldName);
            if (!field) return;

            const errorElement = field.parentElement.querySelector('.text-danger');

            field.classList.add('error');
            if (errorElement) {
                errorElement.textContent = message;
                errorElement.style.display = 'block';
            }
        }

        // Clear error function
        function clearError(fieldName) {
            const field = document.getElementById(fieldName);
            if (!field) return;

            const errorElement = field.parentElement.querySelector('.text-danger');

            field.classList.remove('error');
            if (errorElement) {
                errorElement.textContent = '';
                errorElement.style.display = 'none';
            }
        }

        // Attach real-time validation to fields
        Object.keys(validationRules).forEach(function (fieldName) {
            const field = document.getElementById(fieldName);
            if (field) {
                // Validation on blur
                field.addEventListener('blur', function (e) {
                    const validation = validateField(fieldName, e.target.value);
                    if (!validation.valid) {
                        showError(fieldName, validation.message);
                    } else {
                        clearError(fieldName);
                    }
                });

                // Clear error on input if field was previously invalid
                field.addEventListener('input', function (e) {
                    if (field.classList.contains('error')) {
                        const validation = validateField(fieldName, e.target.value);
                        if (validation.valid) {
                            clearError(fieldName);
                        }
                    }
                });
            }
        });

        // Form submit validation
        const employeeForm = document.getElementById('employeeForm');
        if (employeeForm) {
            employeeForm.addEventListener('submit', function (e) {
                let isValid = true;

                // Validate all fields
                Object.keys(validationRules).forEach(function (fieldName) {
                    const field = document.getElementById(fieldName);
                    if (field) {
                        const value = field.type === 'checkbox' ? field.checked : field.value;
                        const validation = validateField(fieldName, value);

                        if (!validation.valid) {
                            showError(fieldName, validation.message);
                            isValid = false;
                        } else {
                            clearError(fieldName);
                        }
                    }
                });

                if (!isValid) {
                    e.preventDefault();

                    // Scroll to first error
                    const firstError = document.querySelector('.error');
                    if (firstError) {
                        firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
                    }
                }
            });
        }

        // Prevent form submission on Enter key in input fields (except textarea)
        if (employeeForm) {
            employeeForm.addEventListener('keydown', function (e) {
                if (e.key === 'Enter' && e.target.tagName !== 'TEXTAREA' && e.target.tagName !== 'BUTTON') {
                    e.preventDefault();
                }
            });
        }

        // Handle cancel button confirmation
        const cancelButtons = document.querySelectorAll('.btn-outline[href*="Index"], a[asp-action="Index"]');
        cancelButtons.forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                // Check if form has any filled fields
                const formFields = employeeForm.querySelectorAll('input:not([type="hidden"]), select, textarea');
                let hasData = false;

                formFields.forEach(function (field) {
                    if (field.type === 'checkbox' || field.type === 'radio') {
                        // Don't check checkboxes for cancel confirmation
                        return;
                    }
                    if (field.value && field.value.trim() !== '') {
                        hasData = true;
                    }
                });

                if (hasData) {
                    if (!confirm('¿Está seguro que desea cancelar? Se perderán los datos ingresados.')) {
                        e.preventDefault();
                    }
                }
            });
        });
    });
})();
