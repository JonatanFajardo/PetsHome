var Evento = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            console.log("ws");
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                { FieldName: 'eve_Id', Visibility: false },
                {
                    FieldName: 'eve_Descripcion',
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return `
                                <div style="display:flex;align-items:center;gap:10px;">
                                    <div style="background:#ede9fe;border-radius:50%;width:32px;height:32px;
                                                display:flex;align-items:center;justify-content:center;flex-shrink:0;">
                                        <i class="fas fa-paw" style="color:#7c3aed;font-size:14px;"></i>
                                    </div>
                                    <span>${data ?? ''}</span>
                                </div>`;
                        }
                        return data;
                    }
                },
                { FieldName: 'refg_Id', Size: 60, Visibility: true },
                {
                    FieldName: 'refg_Nombre',
                    Size: 150,
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return `
                                <div style="display:flex;align-items:center;gap:10px;">
                                    <div style="background:#ede9fe;border-radius:50%;width:32px;height:32px;
                                                display:flex;align-items:center;justify-content:center;flex-shrink:0;">
                                        <i class="fas fa-house-user" style="color:#7c3aed;font-size:14px;"></i>
                                    </div>
                                    <span>${data ?? ''}</span>
                                </div>`;
                        }
                        return data;
                    }
                },
                {
                    FieldName: 'eve_Fecha',
                    Size: 110,
                    Visibility: true,
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (!data) return '<span style="color:#9ca3af">—</span>';
                            var fecha = new Date(data);
                            return isNaN(fecha.getTime())
                                ? '<span style="color:#9ca3af">—</span>'
                                : fecha.toLocaleDateString('es-HN', {
                                    day: '2-digit',
                                    month: '2-digit',
                                    year: 'numeric'
                                });
                        }
                        return data;
                    }
                }
            ];
            datatable.init(Direction, header);
        })
    };

    /* ---- Calendario ---- */
    var _calState = { year: new Date().getFullYear(), month: new Date().getMonth() };

    obj.renderCalendar = function () {
        var y = _calState.year, m = _calState.month;
        var hoy = new Date(); hoy.setHours(0,0,0,0);
        var primerDia = new Date(y, m, 1).getDay(); // 0=Dom
        // Normalizar a lunes=0
        var offset = (primerDia === 0) ? 6 : primerDia - 1;
        var diasMes = new Date(y, m + 1, 0).getDate();
        var meses = ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre'];

        // Recoger eventos del DataTable
        var eventos = {};
        $('#datatable').DataTable().rows().data().each(function (row) {
            if (!row.eve_Fecha) return;
            var d = new Date(row.eve_Fecha);
            if (isNaN(d.getTime())) return;
            if (d.getFullYear() !== y || d.getMonth() !== m) return;
            var key = d.getDate();
            if (!eventos[key]) eventos[key] = [];
            eventos[key].push({ id: row.eve_Id, nombre: row.eve_Descripcion, refugio: row.refg_Nombre, fecha: d });
        });

        // Header
        document.getElementById('cal-mes-anio').textContent = meses[m] + ' ' + y;

        // Grid
        var grid = document.getElementById('cal-grid');
        grid.innerHTML = '';
        var total = offset + diasMes;
        var celdas = Math.ceil(total / 7) * 7;
        for (var i = 0; i < celdas; i++) {
            var dia = i - offset + 1;
            var cel = document.createElement('div');
            cel.className = 'cal-cell';
            if (dia < 1 || dia > diasMes) {
                cel.classList.add('cal-cell-off');
            } else {
                var fecha = new Date(y, m, dia);
                if (fecha.getTime() === hoy.getTime()) cel.classList.add('cal-cell-hoy');
                var html = '<div class="cal-dia-num">' + dia + '</div>';
                var evs = eventos[dia] || [];
                var max = 2;
                for (var j = 0; j < Math.min(evs.length, max); j++) {
                    html += '<div class="cal-pill" data-id="' + evs[j].id + '" '
                          + 'data-nombre="' + evs[j].nombre + '" '
                          + 'data-refugio="' + (evs[j].refugio || '') + '" '
                          + 'data-fecha="' + evs[j].fecha.toLocaleDateString("es-HN") + '">'
                          + evs[j].nombre + '</div>';
                }
                if (evs.length > max)
                    html += '<div class="cal-pill cal-pill-more">+' + (evs.length - max) + ' más</div>';
                cel.innerHTML = html;
                // Click en pill
                cel.querySelectorAll('.cal-pill[data-id]').forEach(function (el) {
                    el.addEventListener('click', function (e) {
                        e.stopPropagation();
                        obj.abrirModal(this.dataset);
                    });
                });
            }
            grid.appendChild(cel);
        }
    };

    obj.abrirModal = function (data) {
        document.getElementById('cal-modal-nombre').textContent  = data.nombre || '—';
        document.getElementById('cal-modal-refugio').textContent = data.refugio || '—';
        document.getElementById('cal-modal-fecha').textContent   = data.fecha   || '—';
        document.getElementById('cal-modal-detalle').href = window._urlEventoDetail + '?id=' + data.id;
        document.getElementById('cal-modal-editar').href  = window._urlEventoFind   + '?id=' + data.id;
        $('#calEventoModal').modal('show');
    };

    obj.prevMes = function () {
        _calState.month--;
        if (_calState.month < 0) { _calState.month = 11; _calState.year--; }
        obj.renderCalendar();
    };

    obj.nextMes = function () {
        _calState.month++;
        if (_calState.month > 11) { _calState.month = 0; _calState.year++; }
        obj.renderCalendar();
    };

    return obj;

}());