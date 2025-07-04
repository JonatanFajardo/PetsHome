// CitaMedica mejorado con mejor control de errores y campos visualmente mejorados
var CitaMedica = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        // Validación de parámetros
        if (!Direction) {
            console.error('CitaMedica.datatable: Parámetro Direction es requerido');
            return false;
        }

        if (!Direction.urlList) {
            console.error('CitaMedica.datatable: Direction.urlList es requerido');
            return false;
        }

        $(function () {
            try {
                console.log("Inicializando CitaMedica datatable");

                // Configuración de headers con mejoras visuales
                var header = [
                    {
                        FieldName: "masc_Nombre",
                        DisplayName: "👤 Nombre de la Mascota",
                        Width: "200px",
                        Align: "left",
                        Sortable: true,
                        Searchable: true,
                        Render: function (data) {
                            return `<div class="pet-name-cell">
                                        <i class="fas fa-paw text-primary me-2"></i>
                                        <strong>${data || 'N/A'}</strong>
                                    </div>`;
                        }
                    },
                    {
                        FieldName: "medic_FechaConsulta",
                        DisplayName: "📅 Fecha de Consulta",
                        Width: "150px",
                        Align: "center",
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">Sin fecha</span>';
                            const fecha = new Date(data);
                            const fechaFormateada = fecha.toLocaleDateString('es-ES', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric'
                            });
                            return `<div class="date-cell">
                                        <i class="far fa-calendar-alt text-info me-2"></i>
                                        <span class="fw-medium">${fechaFormateada}</span>
                                    </div>`;
                        }
                    },
                    {
                        FieldName: "medic_TipoConsulta",
                        DisplayName: "🏥 Tipo de Consulta",
                        Width: "180px",
                        Align: "center",
                        Sortable: true,
                        Render: function (data) {
                            const tipos = {
                                'Consulta General': { color: 'primary', icon: 'fas fa-stethoscope' },
                                'Emergencia': { color: 'danger', icon: 'fas fa-exclamation-triangle' },
                                'Vacunación': { color: 'success', icon: 'fas fa-syringe' },
                                'Cirugía': { color: 'warning', icon: 'fas fa-cut' },
                                'Control': { color: 'info', icon: 'fas fa-clipboard-check' }
                            };
                            const tipo = tipos[data] || { color: 'secondary', icon: 'fas fa-question' };
                            return `<span class="badge bg-${tipo.color} p-2">
                                        <i class="${tipo.icon} me-1"></i>
                                        ${data || 'No especificado'}
                                    </span>`;
                        }
                    },
                    {
                        FieldName: "medic_Diagnostico",
                        DisplayName: "📋 Diagnóstico",
                        Width: "250px",
                        Align: "left",
                        Sortable: false,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted fst-italic">Sin diagnóstico</span>';
                            const diagnosticoCorto = data.length > 50 ? data.substring(0, 50) + '...' : data;
                            return `<div class="diagnosis-cell" title="${data}">
                                        <i class="fas fa-notes-medical text-success me-2"></i>
                                        <span class="text-dark">${diagnosticoCorto}</span>
                                    </div>`;
                        }
                    },
                    {
                        FieldName: "medic_Peso",
                        DisplayName: "⚖️ Peso",
                        Width: "120px",
                        Align: "center",
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">N/A</span>';
                            const peso = parseFloat(data);
                            let colorClass = 'text-primary';
                            let icon = 'fas fa-weight';

                            if (peso < 5) colorClass = 'text-info';
                            else if (peso > 30) colorClass = 'text-warning';

                            return `<div class="weight-cell">
                                        <i class="${icon} ${colorClass} me-2"></i>
                                        <span class="fw-bold ${colorClass}">${peso} kg</span>
                                    </div>`;
                        }
                    },
                    {
                        FieldName: "medic_Temperatura",
                        DisplayName: "🌡️ Temperatura",
                        Width: "130px",
                        Align: "center",
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">N/A</span>';
                            const temp = parseFloat(data);
                            let colorClass = 'text-success';
                            let icon = 'fas fa-thermometer-half';

                            if (temp < 38) colorClass = 'text-info';
                            else if (temp > 39.5) colorClass = 'text-danger';

                            return `<div class="temperature-cell">
                                        <i class="${icon} ${colorClass} me-2"></i>
                                        <span class="fw-bold ${colorClass}">${temp}°C</span>
                                    </div>`;
                        }
                    },
                    {
                        FieldName: "medic_ProximaCita",
                        DisplayName: "🔄 Próxima Cita",
                        Width: "160px",
                        Align: "center",
                        Sortable: true,
                        Render: function (data) {
                            if (!data) return '<span class="text-muted">Sin programar</span>';

                            const fechaCita = new Date(data);
                            const hoy = new Date();
                            const diasRestantes = Math.ceil((fechaCita - hoy) / (1000 * 60 * 60 * 24));

                            let colorClass = 'text-primary';
                            let icon = 'far fa-calendar-plus';

                            if (diasRestantes < 0) {
                                colorClass = 'text-danger';
                                icon = 'fas fa-calendar-times';
                            } else if (diasRestantes <= 7) {
                                colorClass = 'text-warning';
                                icon = 'fas fa-calendar-exclamation';
                            }

                            const fechaFormateada = fechaCita.toLocaleDateString('es-ES', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric'
                            });

                            return `<div class="next-appointment-cell">
                                        <i class="${icon} ${colorClass} me-2"></i>
                                        <span class="fw-medium ${colorClass}">${fechaFormateada}</span>
                                        <br>
                                        <small class="text-muted">
                                            ${diasRestantes >= 0 ? `En ${diasRestantes} días` : `Vencida hace ${Math.abs(diasRestantes)} días`}
                                        </small>
                                    </div>`;
                        }
                    }
                ];

                // Validar que header tenga elementos
                if (!header || header.length === 0) {
                    throw new Error('La configuración de header está vacía');
                }

                console.log("URL de lista:", Direction.urlList);
                console.log("Configuración de header:", header);

                // Verificar que datatable existe y tiene el método init
                if (typeof datatable === 'undefined') {
                    throw new Error('El objeto datatable no está definido');
                }

                if (typeof datatable.init !== 'function') {
                    throw new Error('datatable.init no es una función');
                }

                // Agregar estilos CSS personalizados
                obj.addCustomStyles();

                // Inicializar datatable con timeout para manejo de carga
                setTimeout(function () {
                    try {
                        datatable.init(Direction, header);
                        console.log("DataTable inicializado correctamente");

                        // Aplicar mejoras visuales adicionales después de la inicialización
                        setTimeout(function () {
                            obj.applyVisualEnhancements();
                        }, 500);

                    } catch (initError) {
                        console.error("Error en datatable.init (timeout):", initError);

                        // Intentar mostrar mensaje de error al usuario
                        if (typeof Swal !== 'undefined') {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: 'No se pudo cargar la tabla de datos',
                                confirmButtonColor: '#3085d6'
                            });
                        } else {
                            alert('Error: No se pudo cargar la tabla de datos');
                        }
                    }
                }, 100);

            } catch (error) {
                console.error("ERROR en CitaMedica.datatable:", error);

                // Log detallado del error
                console.error("Stack trace:", error.stack);
                console.error("Direction object:", Direction);

                // Mostrar error al usuario
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error de Configuración',
                        text: 'Hubo un problema al configurar la tabla: ' + error.message,
                        confirmButtonColor: '#3085d6'
                    });
                } else if (typeof toastr !== 'undefined') {
                    toastr.error('Error al configurar la tabla: ' + error.message);
                } else {
                    alert('Error: ' + error.message);
                }

                return false;
            }
        });

        return true;
    };

    // Método para agregar estilos CSS personalizados
    obj.addCustomStyles = function () {
        const styles = `
            <style id="citamedica-custom-styles">
            /* Cuerpo de la tabla - HOVER EFECTOS AVANZADOS */
.citamedica-table tbody tr {
    border-bottom: 1px solid #f1f5f9;
    transition: all 0.4s cubic-bezier(0.23, 1, 0.32, 1);
    position: relative;
    overflow: hidden;
}

/* Efecto principal con deslizamiento lateral */
.citamedica-table tbody tr:hover {
    background: linear-gradient(135deg, #fafbff 0%, #f0f4ff 100%);
    transform: translateX(8px) translateY(-1px);
    box-shadow:
        -4px 0 0 #3b82f6,
        0 4px 20px rgba(59, 130, 246, 0.15),
        0 2px 8px rgba(0, 0, 0, 0.08);
    border-radius: 0 12px 12px 0;
    z-index: 10;
}

/* Efecto de ondas en hover */
.citamedica-table tbody tr.wave-hover {
    position: relative;
    overflow: hidden;
}

.citamedica-table tbody tr.wave-hover::before {
    content: '';
    position: absolute;
    top: 0;
    left: -100%;
    width: 100%;
    height: 100%;
    background: linear-gradient(90deg,
        transparent 0%,
        rgba(59, 130, 246, 0.1) 50%,
        transparent 100%);
    transition: left 0.6s ease;
}

.citamedica-table tbody tr.wave-hover:hover::before {
    left: 100%;
}

.citamedica-table tbody tr.wave-hover:hover {
    background-color: #f8fafc;
    transform: translateY(-2px);
    box-shadow: 0 6px 20px rgba(0, 0, 0, 0.12);
}

/* Efecto de expansión desde el centro */
.citamedica-table tbody tr.expand-hover {
    position: relative;
    overflow: hidden;
}

.citamedica-table tbody tr.expand-hover::after {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 0;
    height: 0;
    background: radial-gradient(circle, rgba(59, 130, 246, 0.1) 0%, transparent 70%);
    transform: translate(-50%, -50%);
    transition: all 0.5s ease;
    border-radius: 50%;
}

.citamedica-table tbody tr.expand-hover:hover::after {
    width: 200%;
    height: 200%;
}

.citamedica-table tbody tr.expand-hover:hover {
    transform: scale(1.02);
    box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

/* Efecto de deslizamiento múltiple */
.citamedica-table tbody tr.multi-slide:hover {
    background: linear-gradient(45deg, #f8fafc, #e2e8f0, #f8fafc);
    background-size: 300% 300%;
    animation: gradientShift 3s ease infinite;
    transform: translateX(12px) rotateX(2deg);
    box-shadow:
        -6px 0 0 #10b981,
        0 6px 30px rgba(16, 185, 129, 0.2),
        0 3px 10px rgba(0, 0, 0, 0.1);
    border-radius: 0 16px 16px 0;
}

@keyframes gradientShift {
    0% { background-position: 0% 50%; }
    50% { background-position: 100% 50%; }
    100% { background-position: 0% 50%; }
}

/* Efecto de zoom con bordes brillantes */
.citamedica-table tbody tr.zoom-glow:hover {
    background: #ffffff;
    transform: scale(1.03) translateY(-3px);
    box-shadow:
        0 0 0 2px #3b82f6,
        0 0 20px rgba(59, 130, 246, 0.4),
        0 10px 30px rgba(0, 0, 0, 0.2);
    border-radius: 12px;
    position: relative;
    z-index: 20;
}

.citamedica-table tbody tr.zoom-glow:hover::before {
    content: '';
    position: absolute;
    top: -2px;
    left: -2px;
    right: -2px;
    bottom: -2px;
    background: linear-gradient(45deg, #3b82f6, #8b5cf6, #06b6d4, #3b82f6);
    background-size: 300% 300%;
    animation: rainbowBorder 2s ease infinite;
    border-radius: 14px;
    z-index: -1;
}

@keyframes rainbowBorder {
    0% { background-position: 0% 50%; }
    50% { background-position: 100% 50%; }
    100% { background-position: 0% 50%; }
}

/* Efecto de levitación con partículas */
.citamedica-table tbody tr.levitate-hover {
    position: relative;
}

.citamedica-table tbody tr.levitate-hover:hover {
    background: radial-gradient(ellipse at center, #f8fafc 0%, #e2e8f0 100%);
    transform: translateY(-8px) rotateX(5deg);
    box-shadow:
        0 15px 35px rgba(0, 0, 0, 0.1),
        0 5px 15px rgba(0, 0, 0, 0.08),
        0 0 0 1px rgba(59, 130, 246, 0.1);
    border-radius: 12px;
    animation: float 3s ease-in-out infinite;
}

@keyframes float {
    0%, 100% { transform: translateY(-8px) rotateX(5deg); }
    50% { transform: translateY(-12px) rotateX(5deg); }
}

/* Efecto de cristal con blur */
.citamedica-table tbody tr.glass-hover:hover {
    background: rgba(255, 255, 255, 0.9);
    backdrop-filter: blur(10px) saturate(180%);
    border: 1px solid rgba(59, 130, 246, 0.2);
    transform: translateY(-2px);
    box-shadow:
        0 8px 32px rgba(59, 130, 246, 0.12),
        0 2px 8px rgba(0, 0, 0, 0.08),
        inset 0 1px 0 rgba(255, 255, 255, 0.5);
    border-radius: 16px;
}

/* Efecto de neón futurista */
.citamedica-table tbody tr.neon-hover:hover {
    background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
    color: #00ff88;
    transform: translateY(-3px) scale(1.01);
    box-shadow:
        0 0 20px rgba(0, 255, 136, 0.5),
        0 0 40px rgba(0, 255, 136, 0.3),
        0 0 60px rgba(0, 255, 136, 0.1),
        0 5px 20px rgba(0, 0, 0, 0.3);
    border-radius: 8px;
    text-shadow: 0 0 10px rgba(0, 255, 136, 0.8);
}

.citamedica-table tbody tr.neon-hover:hover td {
    color: #00ff88;
}

/* Efectos en celdas específicas durante hover */
.citamedica-table tbody tr:hover .pet-name-cell {
    color: #1e40af;
    font-weight: 700;
    transform: translateX(4px);
    transition: all 0.3s ease;
}

.citamedica-table tbody tr:hover .pet-name-cell i {
    color: #3b82f6;
    transform: scale(1.2) rotate(5deg);
    filter: drop-shadow(0 2px 4px rgba(59, 130, 246, 0.3));
}

.citamedica-table tbody tr:hover .status-badge {
    transform: scale(1.1) rotate(-1deg);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
    filter: brightness(1.1) saturate(1.2);
}

/* Efecto de escritura en máquina para el diagnóstico */
.citamedica-table tbody tr:hover .diagnosis-cell {
    position: relative;
    overflow: hidden;
}

.citamedica-table tbody tr:hover .diagnosis-cell::after {
    content: '';
    position: absolute;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    background: linear-gradient(90deg, transparent 0%, #f8fafc 2px, transparent 4px);
    animation: typewriter 0.8s steps(40, end) 1;
}

@keyframes typewriter {
    from { left: 0; }
    to { left: 100%; }
}

/* Efecto de pulsación en los valores numéricos */
.citamedica-table tbody tr:hover .weight-cell,
.citamedica-table tbody tr:hover .temperature-cell {
    animation: pulse-number 1.5s ease-in-out infinite;
    font-weight: 700;
}

@keyframes pulse-number {
    0%, 100% { transform: scale(1); }
    50% { transform: scale(1.1); }
}

/* Efecto de resplandor en próxima cita */
.citamedica-table tbody tr:hover .next-appointment-cell {
    position: relative;
    overflow: hidden;
}

.citamedica-table tbody tr:hover .next-appointment-cell::before {
    content: '';
    position: absolute;
    top: -2px;
    left: -2px;
    right: -2px;
    bottom: -2px;
    background: linear-gradient(45deg, transparent, rgba(59, 130, 246, 0.1), transparent);
    animation: glow-sweep 2s ease-in-out infinite;
    border-radius: 6px;
}

@keyframes glow-sweep {
    0% { transform: translateX(-100%) rotate(45deg); }
    100% { transform: translateX(100%) rotate(45deg); }
}
            </style>
        `;

        // Remover estilos existentes si los hay
        $('#citamedica-custom-styles').remove();

        // Agregar nuevos estilos
        $('head').append(styles);
    };

    // Método para aplicar mejoras visuales adicionales
    obj.applyVisualEnhancements = function () {
        // Agregar clase CSS a la tabla
        $('table').addClass('citamedica-table table table-striped');

        // Agregar tooltips a las celdas de diagnóstico
        $('[title]').tooltip({
            placement: 'top',
            trigger: 'hover'
        });

        // Animar la aparición de las filas
        $('tbody tr').each(function (index) {
            $(this).css({
                'opacity': '0',
                'animation': `fadeInUp 0.5s ease ${index * 0.1}s forwards`
            });
        });

        // Agregar animación CSS
        $('head').append(`
            <style>
                @keyframes fadeInUp {
                    from {
                        opacity: 0;
                        transform: translateY(20px);
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0);
                    }
                }
            </style>
        `);
    };

    // Método adicional para validar configuración
    obj.validateConfig = function (Direction, header) {
        var errors = [];

        if (!Direction) {
            errors.push('Direction es requerido');
        } else {
            if (!Direction.urlList) errors.push('Direction.urlList es requerido');
        }

        if (!header || !Array.isArray(header)) {
            errors.push('Header debe ser un array');
        } else if (header.length === 0) {
            errors.push('Header no puede estar vacío');
        }

        return {
            isValid: errors.length === 0,
            errors: errors
        };
    };

    // Método para actualizar tema de colores
    obj.updateTheme = function (theme) {
        const themes = {
            medical: {
                primary: '#4f46e5',
                success: '#10b981',
                warning: '#f59e0b',
                danger: '#ef4444',
                info: '#06b6d4'
            },
            veterinary: {
                primary: '#059669',
                success: '#84cc16',
                warning: '#f97316',
                danger: '#dc2626',
                info: '#0891b2'
            }
        };

        if (themes[theme]) {
            const selectedTheme = themes[theme];
            // Actualizar variables CSS
            document.documentElement.style.setProperty('--primary-color', selectedTheme.primary);
            document.documentElement.style.setProperty('--success-color', selectedTheme.success);
            document.documentElement.style.setProperty('--warning-color', selectedTheme.warning);
            document.documentElement.style.setProperty('--danger-color', selectedTheme.danger);
            document.documentElement.style.setProperty('--info-color', selectedTheme.info);
        }
    };

    return obj;
}());