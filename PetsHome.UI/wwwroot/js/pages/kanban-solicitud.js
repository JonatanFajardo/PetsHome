/* ============================================================
   KANBAN — Solicitud/Index
   Requiere: solicitud.js cargado antes, DataTable inicializado
   ============================================================ */

const KB = (function () {

    const ESTADOS = {
        'pendiente':    { key: 'pendiente', label: 'Pendiente',    lista: 'kb-list-pendiente', count: 'kb-count-pendiente' },
        'en revision':  { key: 'revision',  label: 'En Revisión',  lista: 'kb-list-revision',  count: 'kb-count-revision'  },
        'en revisión':  { key: 'revision',  label: 'En Revisión',  lista: 'kb-list-revision',  count: 'kb-count-revision'  },
        'aprobada':     { key: 'aprobada',  label: 'Aprobada',     lista: 'kb-list-aprobada',  count: 'kb-count-aprobada'  },
        'rechazada':    { key: 'rechazada', label: 'Rechazada',    lista: 'kb-list-rechazada', count: 'kb-count-rechazada' }
    };

    const EMOJIS = {
        'canino': '🐕', 'felino': '🐈', 'ave': '🦜',
        'roedor': '🐹', 'reptil': '🦎', 'pez': '🐟'
    };

    function mapRow(row) {
        return {
            id:          row.sol_Id            || '',
            solicitante: row.sol_Nombres        || '—',
            mascota:     row.masc_Nombre        || '—',
            raza:        row.raza_Descripcion   || '',
            especie:     row.raza_TipoAnimal    || '',
            fecha:       row.sol_Fecha          || '',
            estado:      row.sol_Estado         || 'Pendiente',
            emoji:       EMOJIS[(row.raza_TipoAnimal || '').toLowerCase()] || '🐾'
        };
    }

    function diasDesde(fechaStr) {
        if (!fechaStr) return 0;
        const p = fechaStr.split('/');
        const d = p.length === 3
            ? new Date(p[2], p[1] - 1, p[0])
            : new Date(fechaStr);
        return isNaN(d) ? 0 : Math.floor((Date.now() - d.getTime()) / 86400000);
    }

    function buildCard(item) {
        const dias = diasDesde(item.fecha);
        const badgeClass = dias > 7 ? 'kb-days-badge--danger'
                         : dias > 3 ? 'kb-days-badge--warn'
                         :            'kb-days-badge--ok';
        const badgeText = dias === 0 ? 'Hoy'
                        : dias === 1 ? 'Hace 1 día'
                        : `Hace ${dias} días`;

        const estadoActualKey = (ESTADOS[(item.estado || '').toLowerCase()] || {}).key || '';
        const opcionesDropdown = Object.values(ESTADOS)
            .reduce((acc, e) => {
                if (e.key !== estadoActualKey && !acc.find(x => x.key === e.key))
                    acc.push(e);
                return acc;
            }, [])
            .map(e => `
                <div class="kb-dropdown-item" onclick="KB.cambiarEstado('${item.id}','${e.key}',this)">
                    <span class="kb-dd-dot kb-dd-dot--${e.key}"></span>${e.label}
                </div>`).join('');

        const meta = [item.raza, item.especie].filter(Boolean).join(' · ');

        return `
        <div class="kb-card" data-id="${item.id}">
            <div class="kb-card-top">
                <div class="kb-pet-avatar">${item.emoji}</div>
                <div>
                    <div class="kb-pet-name">${item.mascota}</div>
                    ${meta ? `<div class="kb-pet-meta">${meta}</div>` : ''}
                </div>
            </div>
            <hr class="kb-card-divider">
            <div class="kb-info-row"><i class="fas fa-user"></i>${item.solicitante}</div>
            <div class="kb-info-row"><i class="fas fa-calendar-alt"></i>${item.fecha}</div>
            <span class="kb-days-badge ${badgeClass}">${badgeText}</span>
            <div class="kb-card-footer">
                <button class="kb-btn kb-btn--detail" onclick="KB.verDetalle('${item.id}')">
                    <i class="fas fa-eye me-1"></i>Ver detalle
                </button>
                <div class="kb-state-wrapper">
                    <button class="kb-btn kb-btn--state" onclick="KB.toggleDropdown(this)">
                        <i class="fas fa-exchange-alt me-1"></i>Cambiar ▾
                    </button>
                    <div class="kb-dropdown">${opcionesDropdown}</div>
                </div>
            </div>
        </div>`;
    }

    function render() {
        Object.values(ESTADOS).forEach(e => {
            const el = document.getElementById(e.lista);
            if (el) el.innerHTML = '';
        });

        const counts = { pendiente: 0, revision: 0, aprobada: 0, rechazada: 0 };
        const seenKeys = new Set();

        $('#datatable').DataTable().rows({ search: 'applied' }).data().each(function (row) {
            const item = mapRow(row);
            const cfg  = ESTADOS[(item.estado || '').toLowerCase().trim()];
            if (!cfg) return;

            const lista = document.getElementById(cfg.lista);
            if (lista) lista.insertAdjacentHTML('beforeend', buildCard(item));
            counts[cfg.key] = (counts[cfg.key] || 0) + 1;
            seenKeys.add(cfg.key);
        });

        Object.entries(counts).forEach(([key, n]) => {
            const countEl = document.getElementById(`kb-count-${key}`);
            if (countEl) countEl.textContent = n;

            const listaEl = document.getElementById(`kb-list-${key}`);
            if (listaEl && n === 0)
                listaEl.innerHTML = '<div class="kb-empty"><i class="fas fa-inbox"></i>Sin solicitudes</div>';
        });
    }

    function verDetalle(id) {
        const url = window._urlSolicitudDetail;
        if (url) window.location.href = url + '?id=' + id;
    }

    function toggleDropdown(btn) {
        const dd = btn.nextElementSibling;
        const open = dd.classList.contains('is-open');
        document.querySelectorAll('.kb-dropdown.is-open').forEach(el => el.classList.remove('is-open'));
        if (!open) dd.classList.add('is-open');
    }

    function cambiarEstado(id, nuevoEstado, itemClicked) {
        itemClicked.closest('.kb-dropdown').classList.remove('is-open');

        const token = document.querySelector('input[name="__RequestVerificationToken"]');

        $.ajax({
            url: window._urlCambiarEstado || '/Solicitud/CambiarEstado',
            method: 'POST',
            data: {
                id: id,
                estado: nuevoEstado,
                __RequestVerificationToken: token ? token.value : ''
            },
            success: function (res) {
                if (res.success) {
                    $('#datatable').DataTable().ajax.reload(function () { render(); }, false);
                } else {
                    alert('No se pudo actualizar el estado.');
                }
            },
            error: function () {
                alert('Error de conexión. Intente de nuevo.');
            }
        });
    }

    document.addEventListener('click', function (e) {
        if (!e.target.closest('.kb-state-wrapper'))
            document.querySelectorAll('.kb-dropdown.is-open').forEach(el => el.classList.remove('is-open'));
    });

    return { render, verDetalle, toggleDropdown, cambiarEstado };
})();
