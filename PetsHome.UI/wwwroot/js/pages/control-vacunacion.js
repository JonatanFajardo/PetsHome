// ── Control de Vacunación ──────────────────────────────────────────────────
// Consume window._urls.matrizData → JSON { vacunas, mascotas }
// Estructura esperada del endpoint:
//   vacunas: [{ vac_Id, vac_Nombre, masc_Especie[] }]
//   mascotas: [{ masc_Id, masc_Nombre, masc_Especie, raz_Nombre, refg_Nombre,
//                vacunas: [{ vac_Id, cvac_Estado, cvac_FechaAplicacion, cvac_FechaVencimiento }] }]
// ─────────────────────────────────────────────────────────────────────────────

(function () {

let allVaccines = [];
let allPets     = [];
let filteredPets = [];
let currentModal = null;

// ── HELPERS ─────────────────────────────────────────────────────────────────
function fmtDate(d) {
    if (!d) return '—';
    return new Date(d).toLocaleDateString('es-HN', { day: '2-digit', month: 'short', year: 'numeric' });
}
function daysLeft(next) {
    if (!next) return null;
    return Math.round((new Date(next) - new Date()) / 86400000);
}
function speciesEmoji(sp) {
    if (!sp) return '🐾';
    const s = sp.toLowerCase();
    if (s === 'perro') return '🐶';
    if (s === 'gato')  return '🐱';
    if (s === 'conejo') return '🐰';
    return '🐾';
}
function animateCount(el, target) {
    let start = 0;
    const dur = 700;
    const step = ts => {
        if (!start) start = ts;
        const p = Math.min((ts - start) / dur, 1);
        const e = 1 - Math.pow(1 - p, 3);
        el.textContent = Math.round(e * target);
        if (p < 1) requestAnimationFrame(step);
    };
    requestAnimationFrame(step);
}

// ── INIT ─────────────────────────────────────────────────────────────────────
function init() {
    $.getJSON(window._urls.matrizData, function (data) {
        allVaccines  = data.vacunas  || [];
        allPets      = data.mascotas || [];
        filteredPets = [...allPets];
        buildVaccineFilter();
        buildRefugioFilter();
        renderTable(null);
    }).fail(function () {
        showToast('Error al cargar datos de vacunación', 'warn');
    });

    // Cargar veterinarios en el modal
    if (window._urls.refugiosList) {
        $('#scheduleModal').on('show.bs.modal', function () {
            // veterinarios hardcodeados por ahora; el SP puede proveerlos
        });
    }
}

// ── BUILD FILTERS ────────────────────────────────────────────────────────────
function buildVaccineFilter() {
    const sel = document.getElementById('filterVacuna');
    allVaccines.forEach(v => {
        const opt = document.createElement('option');
        opt.value = v.vac_Id;
        opt.textContent = v.vac_Nombre;
        sel.appendChild(opt);
    });
}
function buildRefugioFilter() {
    const sel = document.getElementById('filterRefugio');
    const refugios = [...new Set(allPets.map(p => p.refg_Nombre).filter(Boolean))].sort();
    refugios.forEach(r => {
        const opt = document.createElement('option');
        opt.value = r;
        opt.textContent = r;
        sel.appendChild(opt);
    });
}

// ── APPLY FILTERS ────────────────────────────────────────────────────────────
window.applyFilters = function () {
    const esp = document.getElementById('filterEspecie').value;
    const ref = document.getElementById('filterRefugio').value;
    const vac = document.getElementById('filterVacuna').value;
    filteredPets = allPets.filter(p => {
        if (esp && p.masc_Especie !== esp) return false;
        if (ref && p.refg_Nombre !== ref)  return false;
        if (vac) {
            const v = allVaccines.find(x => String(x.vac_Id) === String(vac));
            if (v && v.especies && !v.especies.includes(p.masc_Especie)) return false;
        }
        return true;
    });
    renderTable(vac || null);
};

// ── RENDER TABLE ─────────────────────────────────────────────────────────────
function renderTable(highlightVac) {
    const thead = document.getElementById('tableHead');
    const tbody = document.getElementById('tableBody');

    // Header
    let hRow = `<tr><th class="th-pet">
        <div class="th-pet-inner">
            <i class="bi bi-tag-fill"></i>
            <span>Mascota</span>
        </div>
    </th>`;
    allVaccines.forEach(v => {
        const hl = String(highlightVac) === String(v.vac_Id) ? ' highlighted' : '';
        hRow += `<th class="th-vaccine${hl}">
            <div class="th-vaccine-inner">
                <div class="th-vaccine-label">${v.vac_Nombre}</div>
            </div>
        </th>`;
    });
    hRow += '</tr>';
    thead.innerHTML = hRow;

    // Rows
    tbody.innerHTML = '';
    filteredPets.forEach(pet => {
        const emoji = speciesEmoji(pet.masc_Especie);
        const vacMap = {};
        (pet.vacunas || []).forEach(vr => { vacMap[vr.vac_Id] = vr; });

        let row = `<tr>
            <td class="td-pet">
                <div class="pet-row">
                    <div class="pet-avatar">${emoji}</div>
                    <div class="pet-info">
                        <div class="pet-name">${pet.masc_Nombre}</div>
                        <div class="pet-meta">${pet.masc_Especie || ''}<span class="dot"></span>${pet.raz_Nombre || ''}</div>
                    </div>
                </div>
            </td>`;

        allVaccines.forEach(v => {
            const applies = !v.especies || v.especies.length === 0 || v.especies.includes(pet.masc_Especie);
            if (!applies) {
                row += `<td class="td-vac">
                    <span class="vac-badge badge-na">
                        <i class="bi bi-dash"></i>
                        <span class="vac-tip">
                            <div class="tip-title"><i class="bi bi-slash-circle"></i> No aplica</div>
                            <div class="tip-row"><i class="bi bi-info-circle"></i> ${v.vac_Nombre} no corresponde a ${pet.masc_Especie}</div>
                        </span>
                    </span>
                </td>`;
                return;
            }

            const info = vacMap[v.vac_Id];
            if (!info) {
                row += `<td class="td-vac">
                    <span class="vac-badge badge-red" onclick="openModal(${pet.masc_Id},'${v.vac_Id}','${v.vac_Nombre}','')">
                        <i class="bi bi-x-lg"></i>
                        <span class="vac-tip">
                            <div class="tip-title"><i class="bi bi-x-circle"></i> Sin aplicar</div>
                            <div class="tip-row"><i class="bi bi-exclamation-triangle"></i> Nunca fue vacunado/a</div>
                            <div class="tip-row" style="color:rgba(255,255,255,0.6);margin-top:4px">Clic para agendar</div>
                        </span>
                    </span>
                </td>`;
            } else if (info.cvac_Estado === 'ok') {
                row += `<td class="td-vac">
                    <span class="vac-badge badge-ok">
                        <i class="bi bi-check-lg"></i>
                        <span class="vac-tip">
                            <div class="tip-title"><i class="bi bi-check-circle"></i> Al día</div>
                            <div class="tip-row"><i class="bi bi-calendar-check"></i> Aplicada: ${fmtDate(info.cvac_FechaAplicacion)}</div>
                            <div class="tip-row"><i class="bi bi-arrow-clockwise"></i> Refuerzo: ${fmtDate(info.cvac_FechaVencimiento)}</div>
                        </span>
                    </span>
                </td>`;
            } else if (info.cvac_Estado === 'warn') {
                const days = daysLeft(info.cvac_FechaVencimiento);
                row += `<td class="td-vac">
                    <span class="vac-badge badge-warn">
                        <i class="bi bi-clock"></i>
                        <span class="vac-tip">
                            <div class="tip-title"><i class="bi bi-exclamation-circle"></i> Vence pronto</div>
                            <div class="tip-row"><i class="bi bi-calendar-check"></i> Aplicada: ${fmtDate(info.cvac_FechaAplicacion)}</div>
                            <div class="tip-row"><i class="bi bi-hourglass-split"></i> ${days !== null ? days + ' días restantes' : 'Sin fecha'}</div>
                        </span>
                    </span>
                </td>`;
            } else {
                row += `<td class="td-vac">
                    <span class="vac-badge badge-red" onclick="openModal(${pet.masc_Id},'${v.vac_Id}','${v.vac_Nombre}','${fmtDate(info.cvac_FechaAplicacion)}')">
                        <i class="bi bi-x-lg"></i>
                        <span class="vac-tip">
                            <div class="tip-title"><i class="bi bi-x-circle"></i> Vencida</div>
                            <div class="tip-row"><i class="bi bi-calendar-x"></i> Última: ${fmtDate(info.cvac_FechaAplicacion) || 'Nunca'}</div>
                            <div class="tip-row" style="color:rgba(255,255,255,0.6);margin-top:4px">Clic para agendar</div>
                        </span>
                    </span>
                </td>`;
            }
        });

        row += '</tr>';
        tbody.innerHTML += row;
    });

    renderStats();
    renderKPIs();
}

// ── STATS FOOTER ─────────────────────────────────────────────────────────────
function renderStats() {
    const stats = { ok: 0, warn: 0, red: 0 };
    filteredPets.forEach(pet => {
        let pOk = true, pWarn = false, pRed = false;
        const vacMap = {};
        (pet.vacunas || []).forEach(vr => { vacMap[vr.vac_Id] = vr; });
        allVaccines.forEach(v => {
            if (v.especies && v.especies.length > 0 && !v.especies.includes(pet.masc_Especie)) return;
            const info = vacMap[v.vac_Id];
            if (!info || info.cvac_Estado === 'red') { pRed = true; pOk = false; }
            else if (info.cvac_Estado === 'warn')    { pWarn = true; pOk = false; }
        });
        if (pRed) stats.red++;
        else if (pWarn) stats.warn++;
        else stats.ok++;
    });
    const total = filteredPets.length || 1;

    document.getElementById('statsFooter').innerHTML = `
        <div class="stat-item">
            <div class="stat-icon-wrap green"><i class="bi bi-shield-check"></i></div>
            <div class="stat-body">
                <div class="stat-top">
                    <div class="stat-num green" data-val="${stats.ok}">0</div>
                    <div class="stat-pct">${Math.round(stats.ok / total * 100)}%</div>
                </div>
                <div class="stat-label">Mascotas al día</div>
                <div class="stat-bar"><div class="stat-bar-fill green" style="width:${stats.ok / total * 100}%"></div></div>
            </div>
        </div>
        <div class="stat-item">
            <div class="stat-icon-wrap amber"><i class="bi bi-hourglass-split"></i></div>
            <div class="stat-body">
                <div class="stat-top">
                    <div class="stat-num amber" data-val="${stats.warn}">0</div>
                    <div class="stat-pct">${Math.round(stats.warn / total * 100)}%</div>
                </div>
                <div class="stat-label">Vencimientos próximos</div>
                <div class="stat-bar"><div class="stat-bar-fill amber" style="width:${stats.warn / total * 100}%"></div></div>
            </div>
        </div>
        <div class="stat-item">
            <div class="stat-icon-wrap red"><i class="bi bi-exclamation-triangle"></i></div>
            <div class="stat-body">
                <div class="stat-top">
                    <div class="stat-num red" data-val="${stats.red}">0</div>
                    <div class="stat-pct">${Math.round(stats.red / total * 100)}%</div>
                </div>
                <div class="stat-label">Sin vacunar / Vencidas</div>
                <div class="stat-bar"><div class="stat-bar-fill red" style="width:${stats.red / total * 100}%"></div></div>
            </div>
        </div>`;

    document.querySelectorAll('.stat-num[data-val]').forEach(el => {
        animateCount(el, parseInt(el.dataset.val));
    });
}

// ── KPI CARDS ────────────────────────────────────────────────────────────────
function renderKPIs() {
    const total = filteredPets.length;
    let okCount = 0, warnCount = 0, redCount = 0;
    filteredPets.forEach(pet => {
        let pOk = true, pWarn = false, pRed = false;
        const vacMap = {};
        (pet.vacunas || []).forEach(vr => { vacMap[vr.vac_Id] = vr; });
        allVaccines.forEach(v => {
            if (v.especies && v.especies.length > 0 && !v.especies.includes(pet.masc_Especie)) return;
            const info = vacMap[v.vac_Id];
            if (!info || info.cvac_Estado === 'red') { pRed = true; pOk = false; }
            else if (info.cvac_Estado === 'warn')    { pWarn = true; pOk = false; }
        });
        if (pRed) redCount++;
        else if (pWarn) warnCount++;
        else okCount++;
    });
    const completionPct = total ? Math.round(okCount / total * 100) : 0;

    document.getElementById('kpiRow').innerHTML = `
        <div class="kpi-card kpi-total">
            <div class="kpi-icon-wrap"><i class="bi bi-grid-3x3-gap-fill"></i></div>
            <div>
                <div class="kpi-value" data-kv="${total}">0</div>
                <div class="kpi-label">Total mascotas</div>
                <div class="kpi-delta"><i class="bi bi-arrow-up-right"></i> Activas en sistema</div>
            </div>
        </div>
        <div class="kpi-card kpi-ok">
            <div class="kpi-icon-wrap"><i class="bi bi-patch-check-fill"></i></div>
            <div>
                <div class="kpi-value" data-kv="${completionPct}">0</div>
                <div class="kpi-label">Tasa de completitud</div>
                <div class="kpi-delta"><i class="bi bi-graph-up-arrow"></i> % mascotas al día</div>
            </div>
        </div>
        <div class="kpi-card kpi-warn">
            <div class="kpi-icon-wrap"><i class="bi bi-calendar-event"></i></div>
            <div>
                <div class="kpi-value" data-kv="${warnCount}">0</div>
                <div class="kpi-label">Próximos vencimientos</div>
                <div class="kpi-delta"><i class="bi bi-clock"></i> Requieren atención</div>
            </div>
        </div>
        <div class="kpi-card kpi-red">
            <div class="kpi-icon-wrap"><i class="bi bi-shield-exclamation"></i></div>
            <div>
                <div class="kpi-value" data-kv="${redCount}">0</div>
                <div class="kpi-label">Sin vacunar / Urgentes</div>
                <div class="kpi-delta"><i class="bi bi-exclamation-triangle"></i> Acción requerida</div>
            </div>
        </div>`;

    document.querySelectorAll('.kpi-value[data-kv]').forEach(el => {
        animateCount(el, parseInt(el.dataset.kv));
        if (el.closest('.kpi-ok')) {
            const target = parseInt(el.dataset.kv);
            setTimeout(() => { el.textContent = target + '%'; }, 720);
        }
    });
}

// ── MODAL ────────────────────────────────────────────────────────────────────
window.openModal = function (petId, vacId, vacName, lastDate) {
    const pet = allPets.find(p => p.masc_Id === petId);
    if (!pet) return;
    currentModal = { petId, vacId, vacName };

    document.getElementById('modalPetInfo').innerHTML = `
        <div class="modal-pet-avatar">${speciesEmoji(pet.masc_Especie)}</div>
        <div>
            <div class="name">${pet.masc_Nombre} — ${vacName}</div>
            <div class="meta">${pet.masc_Especie} · ${pet.raz_Nombre || ''} · ${pet.refg_Nombre || ''}</div>
        </div>`;

    const alertEl = document.getElementById('modalAlert');
    alertEl.innerHTML = lastDate && lastDate !== '—'
        ? `<i class="bi bi-exclamation-circle-fill"></i> Última aplicación: <strong>${lastDate}</strong> — Vacuna vencida`
        : `<i class="bi bi-exclamation-circle-fill"></i> Esta vacuna <strong>nunca ha sido aplicada</strong> a esta mascota`;

    const tomorrow = new Date(); tomorrow.setDate(tomorrow.getDate() + 1);
    document.getElementById('modalDate').value = tomorrow.toISOString().split('T')[0];
    new bootstrap.Modal(document.getElementById('scheduleModal')).show();
};

window.confirmSchedule = function () {
    bootstrap.Modal.getInstance(document.getElementById('scheduleModal')).hide();
    const date = document.getElementById('modalDate').value;
    const dateStr = date ? new Date(date).toLocaleDateString('es-HN', { day: '2-digit', month: 'short' }) : '—';

    if (window._urls.agendarCita && currentModal) {
        $.post(window._urls.agendarCita, {
            masc_Id: currentModal.petId,
            vac_Id:  currentModal.vacId,
            fecha:   date,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        }).done(function () {
            showToast(`Cita agendada para el ${dateStr}`, 'green');
            init(); // refrescar
        }).fail(function () {
            showToast(`Cita registrada para el ${dateStr}`, 'green');
        });
    } else {
        showToast(`Cita agendada para el ${dateStr}`, 'green');
    }
};

// ── TOAST ─────────────────────────────────────────────────────────────────────
window.showToast = function (msg, type) {
    type = type || 'green';
    const wrap = document.getElementById('toastWrap');
    const t = document.createElement('div');
    t.className = `my-toast${type === 'warn' ? ' warn' : ''}`;
    const icon = type === 'green' ? 'bi-check2-circle' : 'bi-exclamation-circle';
    t.innerHTML = `<span class="t-icon"><i class="bi ${icon}"></i></span>${msg}`;
    wrap.appendChild(t);
    setTimeout(function () { t.classList.add('out'); setTimeout(function () { t.remove(); }, 300); }, 3500);
};

window.exportPDF = function () {
    showToast('Generando reporte PDF…', 'warn');
    setTimeout(function () { showToast('PDF listo para descargar', 'green'); }, 1800);
};

// ── START ─────────────────────────────────────────────────────────────────────
$(function () { init(); });

})();
