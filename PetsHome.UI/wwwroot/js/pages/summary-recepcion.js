/* ============================================================
   SUMMARY BAR & FILTERS — RecepcionMercancia/Index
   Requiere: recepcionmercancia.js cargado, DataTable inicializado
   ============================================================ */

const RM_Summary = (function () {

    const fmt = new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL', minimumFractionDigits: 2 });

    // Retorna true si fecha (Date) cae dentro del período seleccionado
    function enPeriodo(fecha, periodo, desde, hasta) {
        if (!fecha || isNaN(fecha.getTime())) return true;
        const hoy  = new Date(); hoy.setHours(0,0,0,0);
        const f    = new Date(fecha); f.setHours(0,0,0,0);

        if (desde && hasta) {
            const d = new Date(desde + 'T00:00:00');
            const h = new Date(hasta  + 'T00:00:00');
            return f >= d && f <= h;
        }
        if (periodo === 'hoy')    return f.getTime() === hoy.getTime();
        if (periodo === 'semana') { const ini = new Date(hoy); ini.setDate(hoy.getDate() - hoy.getDay()); return f >= ini; }
        if (periodo === 'mes')    return f.getMonth() === hoy.getMonth() && f.getFullYear() === hoy.getFullYear();
        if (periodo === 'anio')   return f.getFullYear() === hoy.getFullYear();
        return true; // 'todo'
    }

    // Aplicar filtros al DataTable (custom search)
    function aplicarFiltros() {
        $.fn.dataTable.ext.search.length = 0;

        const periodo = document.getElementById('rm-filter-periodo').value;
        const desde   = document.getElementById('rm-filter-desde').value;
        const hasta   = document.getElementById('rm-filter-hasta').value;
        const tipo    = document.getElementById('rm-filter-tipo').value;

        if (periodo !== 'todo' || desde || hasta || tipo) {
            $.fn.dataTable.ext.search.push(function (settings, data, dataIndex, rowData) {
                if (!rowData) return true;
                const fecha = rowData.recep_Fecha ? new Date(rowData.recep_Fecha) : null;
                const tipo_ = (rowData.recep_TipoRecepcion || '').trim().toLowerCase();

                if (!enPeriodo(fecha, periodo, desde, hasta)) return false;
                if (tipo && tipo_ !== tipo.toLowerCase()) return false;
                return true;
            });
        }

        $('#datatable').DataTable().draw();
    }

    // Calcular y actualizar los stats desde filas visibles
    function calcularStats() {
        let totalRecepciones = 0, totalItems = 0, totalValor = 0, porVencer = 0;

        $('#datatable').DataTable().rows({ search: 'applied' }).data().each(function (row) {
            totalRecepciones++;
            totalItems  += (row.recep_TotalItems     || 0);
            totalValor  += (row.recep_ValorTotal      || 0);
            porVencer   += (row.recep_ItemsPorVencer  || 0);
        });

        document.getElementById('rm-stat-total').textContent  = totalRecepciones;
        document.getElementById('rm-stat-items').textContent  = totalItems.toLocaleString('es-HN');
        document.getElementById('rm-stat-valor').textContent  = fmt.format(totalValor);
        document.getElementById('rm-stat-vencer').textContent = porVencer > 0 ? porVencer : '—';

        // quitar skeleton si estaba
        document.querySelectorAll('.rm-stat.is-loading').forEach(el => el.classList.remove('is-loading'));
    }

    // Limpiar filtros
    function limpiarFiltros() {
        document.getElementById('rm-filter-periodo').value = 'todo';
        document.getElementById('rm-filter-desde').value   = '';
        document.getElementById('rm-filter-hasta').value   = '';
        document.getElementById('rm-filter-tipo').value    = '';
        $.fn.dataTable.ext.search.length = 0;
        $('#datatable').DataTable().draw();
    }

    function init() {
        // Skeleton mientras carga
        document.querySelectorAll('.rm-stat').forEach(el => el.classList.add('is-loading'));

        // Actualizar stats cuando DataTable redibuja
        $('#datatable').on('draw.dt', function () { calcularStats(); });

        // Eventos de filtros
        ['rm-filter-periodo', 'rm-filter-tipo'].forEach(function (id) {
            var el = document.getElementById(id);
            if (el) el.addEventListener('change', aplicarFiltros);
        });
        ['rm-filter-desde', 'rm-filter-hasta'].forEach(function (id) {
            var el = document.getElementById(id);
            if (el) el.addEventListener('change', aplicarFiltros);
        });

        var btnClear = document.getElementById('rm-btn-clear');
        if (btnClear) btnClear.addEventListener('click', limpiarFiltros);
    }

    return { init, calcularStats, limpiarFiltros };
})();
