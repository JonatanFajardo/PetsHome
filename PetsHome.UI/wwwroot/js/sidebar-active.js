// Sidebar Active State Handler
(function() {
    'use strict';

    // Función para marcar el menú activo basado en la URL actual
    function setActiveMenu() {
        const currentPath = window.location.pathname.toLowerCase();

        // Obtener todos los links del sidebar
        const menuItems = document.querySelectorAll('.sidebar-wrapper .menu');
        const submenuLinks = document.querySelectorAll('.sidebar-wrapper .submenu a');

        // Limpiar estados activos previos
        menuItems.forEach(item => item.classList.remove('active'));
        submenuLinks.forEach(link => link.classList.remove('active'));

        // Marcar submenu items activos
        submenuLinks.forEach(link => {
            const href = link.getAttribute('href');
            if (href && currentPath.indexOf(href.toLowerCase()) !== -1) {
                link.classList.add('active');

                // Expandir el menú padre
                const parentCollapse = link.closest('.collapse');
                if (parentCollapse) {
                    parentCollapse.classList.add('show');

                    // Marcar el menú padre como activo
                    const parentMenu = parentCollapse.closest('.menu');
                    if (parentMenu) {
                        parentMenu.classList.add('active');

                        // Actualizar el aria-expanded del toggle
                        const toggle = parentMenu.querySelector('[data-toggle="collapse"]');
                        if (toggle) {
                            toggle.setAttribute('aria-expanded', 'true');
                        }
                    }
                }
            }
        });

        // Si no hay submenu activo, buscar en los menús principales
        const hasActiveSubmenu = document.querySelector('.sidebar-wrapper .submenu a.active');
        if (!hasActiveSubmenu) {
            menuItems.forEach(item => {
                const link = item.querySelector('a');
                if (link) {
                    const href = link.getAttribute('href');
                    if (href && href !== '#' && currentPath.indexOf(href.toLowerCase()) !== -1) {
                        item.classList.add('active');
                    }
                }
            });
        }
    }

    // Ejecutar cuando el DOM esté listo
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', setActiveMenu);
    } else {
        setActiveMenu();
    }

    // Agregar event listeners para los toggles de colapso
    document.addEventListener('DOMContentLoaded', function() {
        const collapseToggles = document.querySelectorAll('.sidebar-wrapper [data-toggle="collapse"]');

        collapseToggles.forEach(toggle => {
            toggle.addEventListener('click', function(e) {
                const target = this.getAttribute('href') || this.getAttribute('data-target');
                if (target && target !== '#') {
                    const isExpanded = this.getAttribute('aria-expanded') === 'true';
                    this.setAttribute('aria-expanded', !isExpanded);
                }
            });
        });
    });

})();
