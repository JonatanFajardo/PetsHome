/**
 * JavaScript para la funcionalidad de reportes
 * PetsHome - Sistema de Gestión de Refugios
 */

// Variables globales para gráficos
let chartMascotasPorRaza, chartAdopcionesPorMes, chartCitasMedicasPorTipo;

/**
 * Inicializa el dashboard de reportes
 */
function initReportesDashboard() {
    
    // Inicializar gráficos
    if (typeof mascotasPorRazaData !== 'undefined') {
        initMascotasPorRazaChart();
    }
    
    if (typeof adopcionesPorMesData !== 'undefined') {
        initAdopcionesPorMesChart();
    }
    
    if (typeof citasMedicasPorTipoData !== 'undefined') {
        initCitasMedicasPorTipoChart();
    }
    
    // Inicializar efectos de animación
    animateMetrics();
    
    // Auto-refresh cada 5 minutos
    setInterval(refreshDashboard, 300000);
    
}

/**
 * Inicializa el gráfico de mascotas por raza
 */
function initMascotasPorRazaChart() {
    const ctx = document.getElementById('chartMascotasPorRaza');
    if (!ctx) return;
    
    const data = {
        labels: mascotasPorRazaData.map(item => item.raza),
        datasets: [{
            label: 'Mascotas',
            data: mascotasPorRazaData.map(item => item.cantidad),
            backgroundColor: [
                '#FF6B6B', '#4ECDC4', '#45B7D1', '#96CEB4', 
                '#FFEAA7', '#DDA0DD', '#98D8C8', '#F7DC6F',
                '#BB8FCE', '#85C1E9'
            ],
            borderWidth: 2,
            borderColor: '#fff'
        }]
    };

    chartMascotasPorRaza = new Chart(ctx, {
        type: 'doughnut',
        data: data,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 15,
                        usePointStyle: true,
                        font: {
                            size: 12
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const item = mascotasPorRazaData[context.dataIndex];
                            const percentage = ((context.parsed / context.dataset.data.reduce((a, b) => a + b, 0)) * 100).toFixed(1);
                            return `${context.label}: ${context.parsed} mascotas (${percentage}%)`;
                        },
                        afterLabel: function(context) {
                            const item = mascotasPorRazaData[context.dataIndex];
                            return [
                                `Adoptadas: ${item.adoptadas}`,
                                `Disponibles: ${item.disponibles}`
                            ];
                        }
                    }
                }
            },
            animation: {
                animateScale: true,
                animateRotate: true
            }
        }
    });
}

/**
 * Inicializa el gráfico de adopciones por mes
 */
function initAdopcionesPorMesChart() {
    const ctx = document.getElementById('chartAdopcionesPorMes');
    if (!ctx) return;
    
    const data = {
        labels: adopcionesPorMesData.map(item => `${item.mes} ${item.año}`),
        datasets: [{
            label: 'Adopciones',
            data: adopcionesPorMesData.map(item => item.cantidad),
            backgroundColor: 'rgba(54, 162, 235, 0.2)',
            borderColor: 'rgba(54, 162, 235, 1)',
            borderWidth: 2,
            fill: true,
            tension: 0.4
        }]
    };

    chartAdopcionesPorMes = new Chart(ctx, {
        type: 'line',
        data: data,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        title: function(context) {
                            return context[0].label;
                        },
                        label: function(context) {
                            return `Adopciones: ${context.parsed.y}`;
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    },
                    grid: {
                        color: 'rgba(0,0,0,0.1)'
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            },
            interaction: {
                mode: 'nearest',
                axis: 'x',
                intersect: false
            }
        }
    });
}

/**
 * Inicializa el gráfico de citas médicas por tipo
 */
function initCitasMedicasPorTipoChart() {
    const ctx = document.getElementById('chartCitasMedicasPorTipo');
    if (!ctx) return;
    
    const data = {
        labels: citasMedicasPorTipoData.map(item => item.tipoConsulta),
        datasets: [{
            label: 'Citas',
            data: citasMedicasPorTipoData.map(item => item.cantidad),
            backgroundColor: [
                '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
                '#9966FF', '#FF9F40', '#FF6384', '#C9CBCF'
            ],
            borderWidth: 0
        }]
    };

    chartCitasMedicasPorTipo = new Chart(ctx, {
        type: 'bar',
        data: data,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            const item = citasMedicasPorTipoData[context.dataIndex];
                            return [
                                `${context.label}: ${context.parsed.y} citas`,
                                `Porcentaje: ${item.porcentajeCitas}%`
                            ];
                        }
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1
                    },
                    grid: {
                        color: 'rgba(0,0,0,0.1)'
                    }
                },
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        maxRotation: 45,
                        minRotation: 0
                    }
                }
            }
        }
    });
}

/**
 * Anima las métricas principales
 */
function animateMetrics() {
    $('.widget-card-four .value').each(function() {
        const $this = $(this);
        const finalValue = parseInt($this.text()) || parseFloat($this.text()) || 0;
        
        $({ value: 0 }).animate({ value: finalValue }, {
            duration: 2000,
            easing: 'easeOutQuart',
            step: function() {
                if ($this.text().includes('%')) {
                    $this.text(Math.ceil(this.value) + '%');
                } else if ($this.text().includes('.')) {
                    $this.text(this.value.toFixed(1));
                } else {
                    $this.text(Math.ceil(this.value));
                }
            },
            complete: function() {
                if ($this.text().includes('%')) {
                    $this.text(finalValue + '%');
                } else {
                    $this.text(finalValue);
                }
            }
        });
    });
}

/**
 * Refresca los datos del dashboard
 */
function refreshDashboard() {
    
    // Mostrar indicador de carga
    showLoadingIndicator();
    
    // Realizar petición AJAX para obtener datos actualizados
    $.ajax({
        url: '/Reportes/GetDashboardData',
        type: 'GET',
        success: function(data) {
            if (data.error) {
                console.error('Error al refrescar dashboard:', data.error);
                showErrorMessage('Error al actualizar los datos');
                return;
            }
            
            // Actualizar métricas
            updateMetrics(data);
            
            // Actualizar gráficos
            updateCharts(data);
            
            hideLoadingIndicator();
            showSuccessMessage('Dashboard actualizado correctamente');
        },
        error: function(xhr, status, error) {
            console.error('Error AJAX:', error);
            hideLoadingIndicator();
            showErrorMessage('Error de conexión al actualizar los datos');
        }
    });
}

/**
 * Actualiza las métricas en pantalla
 */
function updateMetrics(data) {
    $('.widget-card-four').each(function() {
        const $widget = $(this);
        const $value = $widget.find('.value');
        const metricName = $widget.find('.name').text().toLowerCase();
        
        let newValue = 0;
        
        if (metricName.includes('total mascotas')) {
            newValue = data.totalMascotas;
        } else if (metricName.includes('tasa de adopción')) {
            newValue = data.porcentajeAdopciones + '%';
        } else if (metricName.includes('citas pendientes')) {
            newValue = data.citasMedicasPendientes;
        } else if (metricName.includes('voluntarios activos')) {
            newValue = data.voluntariosActivos;
        }
        
        // Animación de cambio
        $value.fadeOut(200, function() {
            $(this).text(newValue).fadeIn(200);
        });
    });
}

/**
 * Actualiza los gráficos con nuevos datos
 */
function updateCharts(data) {
    // Actualizar gráfico de mascotas por raza
    if (chartMascotasPorRaza && data.mascotasPorRaza) {
        chartMascotasPorRaza.data.labels = data.mascotasPorRaza.map(item => item.raza);
        chartMascotasPorRaza.data.datasets[0].data = data.mascotasPorRaza.map(item => item.cantidad);
        chartMascotasPorRaza.update('active');
    }
    
    // Actualizar gráfico de adopciones por mes
    if (chartAdopcionesPorMes && data.adopcionesPorMes) {
        chartAdopcionesPorMes.data.labels = data.adopcionesPorMes.map(item => `${item.mes} ${item.año}`);
        chartAdopcionesPorMes.data.datasets[0].data = data.adopcionesPorMes.map(item => item.cantidad);
        chartAdopcionesPorMes.update('active');
    }
    
    // Actualizar gráfico de citas médicas por tipo
    if (chartCitasMedicasPorTipo && data.citasMedicasPorTipo) {
        chartCitasMedicasPorTipo.data.labels = data.citasMedicasPorTipo.map(item => item.tipoConsulta);
        chartCitasMedicasPorTipo.data.datasets[0].data = data.citasMedicasPorTipo.map(item => item.cantidad);
        chartCitasMedicasPorTipo.update('active');
    }
}

/**
 * Muestra indicador de carga
 */
function showLoadingIndicator() {
    if (!$('#loadingIndicator').length) {
        $('body').append(`
            <div id="loadingIndicator" class="loading-overlay">
                <div class="loading-spinner">
                    <i class="fas fa-spinner fa-spin fa-2x"></i>
                    <p>Actualizando datos...</p>
                </div>
            </div>
        `);
    }
    $('#loadingIndicator').fadeIn(200);
}

/**
 * Oculta indicador de carga
 */
function hideLoadingIndicator() {
    $('#loadingIndicator').fadeOut(200);
}

/**
 * Muestra mensaje de éxito
 */
function showSuccessMessage(message) {
    if (typeof toastr !== 'undefined') {
        toastr.success(message);
    } else {
    }
}

/**
 * Muestra mensaje de error
 */
function showErrorMessage(message) {
    if (typeof toastr !== 'undefined') {
        toastr.error(message);
    } else {
        console.error('❌ ' + message);
    }
}

/**
 * Exporta un gráfico como imagen
 */
function exportChart(chartId, filename) {
    const canvas = document.getElementById(chartId);
    if (!canvas) return;
    
    const url = canvas.toDataURL('image/png');
    const link = document.createElement('a');
    link.download = filename || 'grafico.png';
    link.href = url;
    link.click();
}

/**
 * Utilidades para filtros de reportes
 */
const ReportFilters = {
    /**
     * Aplica filtros a una tabla DataTable
     */
    applyTableFilter: function(tableId, columnIndex, filterValue) {
        const table = $(tableId).DataTable();
        table.column(columnIndex).search(filterValue).draw();
    },
    
    /**
     * Limpia todos los filtros de una tabla
     */
    clearTableFilters: function(tableId) {
        const table = $(tableId).DataTable();
        table.search('').columns().search('').draw();
    },
    
    /**
     * Aplica filtro por rango de fechas
     */
    applyDateRangeFilter: function(tableId, columnIndex, startDate, endDate) {
        $.fn.dataTable.ext.search.push(function(settings, data, dataIndex) {
            const dateStr = data[columnIndex];
            const date = new Date(dateStr);
            const start = startDate ? new Date(startDate) : null;
            const end = endDate ? new Date(endDate) : null;
            
            if (!start && !end) return true;
            if (!start && date <= end) return true;
            if (!end && date >= start) return true;
            if (date >= start && date <= end) return true;
            
            return false;
        });
        
        $(tableId).DataTable().draw();
    }
};

// Inicialización cuando el documento esté listo
$(document).ready(function() {
    // Configurar jQuery easing para animaciones suaves
    if (typeof $.easing !== 'undefined') {
        $.easing.easeOutQuart = function (x, t, b, c, d) {
            return -c * ((t=t/d-1)*t*t*t - 1) + b;
        };
    }
    
});