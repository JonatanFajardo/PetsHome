// Empleado Form - UI Behaviors (non-validation)
// Validation is handled by Data Annotations + jQuery Unobtrusive Validation

(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', function () {
        // Estado Switch Handler
        var estadoCheckbox = document.getElementById('emp_EsActivo');
        var estadoStatus = document.getElementById('estadoStatus');

        if (estadoCheckbox && estadoStatus) {
            estadoStatus.textContent = estadoCheckbox.checked ? 'Activo' : 'Inactivo';
            estadoCheckbox.addEventListener('change', function (e) {
                estadoStatus.textContent = e.target.checked ? 'Activo' : 'Inactivo';
            });
        }

        // Prevent form submission on Enter key (except textarea and buttons)
        var employeeForm = document.getElementById('employeeForm');
        if (employeeForm) {
            employeeForm.addEventListener('keydown', function (e) {
                if (e.key === 'Enter' && e.target.tagName !== 'TEXTAREA' && e.target.tagName !== 'BUTTON') {
                    e.preventDefault();
                }
            });
        }

        // Cancel button confirmation
        var cancelButtons = document.querySelectorAll('.btn-outline[href*="Index"], a[asp-action="Index"]');
        cancelButtons.forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                var formFields = employeeForm.querySelectorAll('input:not([type="hidden"]), select, textarea');
                var hasData = false;

                formFields.forEach(function (field) {
                    if (field.type === 'checkbox' || field.type === 'radio') return;
                    if (field.value && field.value.trim() !== '') hasData = true;
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
