/* ============================================================
   Recepción — tablero de sala de espera + check-in de visitas
   - Sala de espera: GET /Recepcion/SalaEspera (auto-refresh ~20s)
   - Nueva visita:   POST /Recepcion/CheckIn
   - Cancelar:       POST /Recepcion/CambiarEstado  {estado:'Cancelada'}
   Rutas via window._urls. Degradación: si un endpoint falla, la
   pantalla no se rompe (muestra estado vacío / mensaje).
   ============================================================ */
(function () {
  'use strict';

  var urls = window._urls || {};
  var REFRESH_MS = 20000;
  var refreshTimer = null;

  // Estado del formulario de check-in
  var checkin = { tipo: 'interno', cliId: null, cliNombre: '', mascId: null, mascNombre: '' };

  function escapeHtml(s) {
    return String(s == null ? '' : s)
      .replace(/&/g, '&amp;').replace(/</g, '&lt;')
      .replace(/>/g, '&gt;').replace(/"/g, '&quot;');
  }

  function token() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
  }

  function postJson(url, body) {
    return fetch(url, {
      method: 'POST',
      credentials: 'same-origin',
      headers: { 'Content-Type': 'application/json', 'RequestVerificationToken': token() },
      body: JSON.stringify(body || {})
    }).then(function (r) { return r.json(); });
  }

  function notify(icon, title, text) {
    if (window.Swal) { Swal.fire({ icon: icon, title: title, text: text || '', timer: icon === 'success' ? 1600 : undefined, showConfirmButton: icon !== 'success' }); }
    else { alert((title || '') + (text ? '\n' + text : '')); }
  }

  // ───────────────────────── Sala de espera ─────────────────────────
  function prioridadBadge(p) {
    var v = (p || 'Normal').toLowerCase();
    if (v.indexOf('urg') >= 0) return '<span class="rc-prio rc-prio-urgente">Urgente</span>';
    return '<span class="rc-prio rc-prio-normal">Normal</span>';
  }

  function estadoBadge(e) {
    var v = (e || '').toLowerCase();
    var cls = 'rc-estado-espera';
    if (v.indexOf('consulta') >= 0 || v.indexOf('atend') >= 0) cls = 'rc-estado-atendiendo';
    else if (v.indexOf('complet') >= 0) cls = 'rc-estado-completada';
    else if (v.indexOf('cancel') >= 0) cls = 'rc-estado-cancelada';
    return '<span class="rc-estado ' + cls + '">' + escapeHtml(e || 'En espera') + '</span>';
  }

  function renderQueue(list) {
    var body = document.getElementById('rcQueueBody');
    var countEl = document.getElementById('rcQueueCount');
    if (!body) return;
    list = Array.isArray(list) ? list : [];
    if (countEl) countEl.textContent = list.length;

    if (!list.length) {
      body.innerHTML = '<tr><td colspan="10" class="rc-empty"><i class="bi bi-cup-hot"></i> Nadie en espera por ahora.</td></tr>';
      return;
    }

    body.innerHTML = list.map(function (c, i) {
      var dueno = c.esInterno
        ? '<span class="rc-interno"><i class="bi bi-house-heart"></i> Interno</span>'
        : escapeHtml(c.duenoNombre || '—');
      var vet = c.vetAsignado
        ? escapeHtml(c.vetAsignado)
        : '<span class="rc-sin-asignar">— sin asignar</span>';
      var min = (c.minutosEspera != null) ? c.minutosEspera : '';
      var esperaCls = (min !== '' && min >= 30) ? ' rc-espera-alta' : '';
      var cancelable = !/complet|cancel/i.test(c.estado || '');
      var accion = cancelable
        ? '<button type="button" class="rc-row-cancel" data-cita="' + (c.citaId || 0) + '" title="Cancelar visita"><i class="bi bi-x-circle"></i></button>'
        : '';
      return '<tr>' +
        '<td class="rc-turno">#' + (i + 1) + '</td>' +
        '<td><b>' + escapeHtml(c.mascotaNombre || '—') + '</b></td>' +
        '<td>' + dueno + '</td>' +
        '<td class="rc-motivo">' + escapeHtml(c.motivo || '—') + '</td>' +
        '<td>' + escapeHtml(c.tipoConsulta || '—') + '</td>' +
        '<td>' + prioridadBadge(c.prioridad) + '</td>' +
        '<td class="rc-vet">' + vet + '</td>' +
        '<td class="rc-espera' + esperaCls + '">' + (min !== '' ? min + ' min' : '—') + '</td>' +
        '<td>' + estadoBadge(c.estado) + '</td>' +
        '<td class="rc-acciones">' + accion + '</td>' +
        '</tr>';
    }).join('');
  }

  function loadQueue() {
    if (!urls.salaEspera) return;
    fetch(urls.salaEspera, { credentials: 'same-origin' })
      .then(function (r) { return r.json(); })
      .then(renderQueue)
      .catch(function () {
        var body = document.getElementById('rcQueueBody');
        if (body && !body.querySelector('tr td:not(.rc-empty)')) {
          body.innerHTML = '<tr><td colspan="10" class="rc-empty">No se pudo cargar la sala de espera. Reintentando…</td></tr>';
        }
      });
  }

  // Cancelar una visita
  document.addEventListener('click', function (e) {
    var btn = e.target.closest('.rc-row-cancel');
    if (!btn) return;
    var citaId = parseInt(btn.dataset.cita, 10) || 0;
    if (!citaId) return;
    function doCancel() {
      postJson(urls.cambiarEstado, { citaId: citaId, estado: 'Cancelada' })
        .then(function (res) {
          if (res && res.ok) { loadQueue(); }
          else { notify('error', 'No se pudo cancelar', (res && res.mensaje) || ''); }
        })
        .catch(function () { notify('error', 'Error de conexión'); });
    }
    if (window.Swal) {
      Swal.fire({ icon: 'warning', title: '¿Cancelar visita?', showCancelButton: true, confirmButtonText: 'Sí, cancelar', cancelButtonText: 'No' })
        .then(function (r) { if (r.isConfirmed) doCancel(); });
    } else if (confirm('¿Cancelar esta visita?')) { doCancel(); }
  });

  // ───────────────────────── Tipos de consulta ─────────────────────────
  function loadTiposConsulta() {
    var sel = document.getElementById('rcTipoConsulta');
    if (!sel || !urls.lookupSearch) return;
    fetch(urls.lookupSearch + '?tipo=tipoconsulta&term=', { credentials: 'same-origin' })
      .then(function (r) { return r.json(); })
      .then(function (items) {
        items = Array.isArray(items) ? items : [];
        items.forEach(function (it) {
          var o = document.createElement('option');
          o.value = it.id; o.textContent = it.text;
          sel.appendChild(o);
        });
      })
      .catch(function () { /* degradar: queda solo "Seleccione…" */ });
  }

  // ───────────────────────── Veterinarios (opcional) ─────────────────────────
  function loadVeterinarios() {
    var sel = document.getElementById('rcVeterinario');
    if (!sel || !urls.veterinarios) return;
    fetch(urls.veterinarios, { credentials: 'same-origin' })
      .then(function (r) { return r.json(); })
      .then(function (items) {
        items = Array.isArray(items) ? items : [];
        items.forEach(function (v) {
          var o = document.createElement('option');
          o.value = v.id; o.textContent = v.nombre;
          sel.appendChild(o);
        });
      })
      .catch(function () { /* degradar: queda solo "Sin asignar" */ });
  }

  // ───────────────────────── Modal de check-in ─────────────────────────
  var modal = document.getElementById('rcModal');

  function openModal() {
    if (!modal) return;
    resetForm();
    modal.classList.add('is-open');
    modal.setAttribute('aria-hidden', 'false');
  }
  function closeModal() {
    if (!modal) return;
    modal.classList.remove('is-open');
    modal.setAttribute('aria-hidden', 'true');
  }

  function resetForm() {
    checkin = { tipo: 'interno', cliId: null, cliNombre: '', mascId: null, mascNombre: '' };
    setTipo('interno');
    var ids = ['rcMascSearch', 'rcPeso', 'rcMotivo'];
    ids.forEach(function (id) { var el = document.getElementById(id); if (el) el.value = ''; });
    var prio = document.getElementById('rcPrioridad'); if (prio) prio.value = 'Normal';
    var tc = document.getElementById('rcTipoConsulta'); if (tc) tc.value = '';
    var vet = document.getElementById('rcVeterinario'); if (vet) vet.value = '';
    hideChip('rcMascSel'); hideChip('rcClienteSel');
    var err = document.getElementById('rcModalError'); if (err) err.style.display = 'none';
  }

  function setTipo(tipo) {
    checkin.tipo = tipo;
    document.querySelectorAll('.rc-toggle-btn').forEach(function (b) {
      b.classList.toggle('is-active', b.dataset.tipo === tipo);
    });
    var duenoField = document.getElementById('rcDuenoField');
    if (duenoField) duenoField.style.display = (tipo === 'cliente') ? 'block' : 'none';
    if (tipo === 'interno') { checkin.cliId = null; checkin.cliNombre = ''; hideChip('rcClienteSel'); }
  }

  function showChip(boxId, text, onClear) {
    var box = document.getElementById(boxId);
    if (!box) return;
    box.innerHTML = '<span class="rc-chip"><i class="bi bi-check-circle-fill"></i> ' + escapeHtml(text) +
      '<button type="button" class="rc-chip-x">&times;</button></span>';
    box.style.display = 'block';
    var x = box.querySelector('.rc-chip-x');
    if (x) x.addEventListener('click', function () { box.style.display = 'none'; box.innerHTML = ''; if (onClear) onClear(); });
  }
  function hideChip(boxId) { var b = document.getElementById(boxId); if (b) { b.style.display = 'none'; b.innerHTML = ''; } }

  // Toggle interno/cliente
  document.querySelectorAll('.rc-toggle-btn').forEach(function (b) {
    b.addEventListener('click', function () { setTipo(this.dataset.tipo); });
  });

  // Abrir / cerrar modal
  var btnNueva = document.getElementById('rcBtnNueva');
  if (btnNueva) btnNueva.addEventListener('click', openModal);
  if (modal) {
    modal.addEventListener('click', function (e) {
      if (e.target.closest('[data-close]')) closeModal();
    });
  }
  document.addEventListener('keydown', function (e) { if (e.key === 'Escape') closeModal(); });

  // SearchSelect de cliente (buscar / crear dueño)
  if (window.SearchSelect) {
    var cliEl = document.getElementById('rcClienteSelect');
    if (cliEl) {
      SearchSelect.create(cliEl, {
        tipo: 'cliente',
        searchUrl: urls.lookupSearch || '/Lookup/Search',
        createUrl: urls.lookupCreate || '/Lookup/Create',
        permiteCrear: true,
        minChars: 0,
        placeholder: 'Buscar o crear dueño…',
        onSelect: function (item) {
          checkin.cliId = item.id || 0; checkin.cliNombre = item.text || '';
          showChip('rcClienteSel', checkin.cliNombre, function () { checkin.cliId = null; checkin.cliNombre = ''; });
        },
        onCreate: function (item) {
          checkin.cliId = item.id || 0; checkin.cliNombre = item.text || '';
          showChip('rcClienteSel', checkin.cliNombre, function () { checkin.cliId = null; checkin.cliNombre = ''; });
        }
      });
    }
  }

  // Buscador de mascota (reutiliza /Mascota/Search)
  (function initMascSearch() {
    var input = document.getElementById('rcMascSearch');
    var results = document.getElementById('rcMascResults');
    if (!input || !results) return;
    var timer = null;

    function render(items) {
      items = Array.isArray(items) ? items : [];
      if (!items.length) {
        results.innerHTML = '<div class="rc-search-empty">Sin coincidencias</div>';
      } else {
        results.innerHTML = items.map(function (m) {
          return '<div class="rc-search-item" data-id="' + m.id + '" data-nombre="' + escapeHtml(m.nombre || '') + '">' +
            '<div class="n">' + escapeHtml(m.nombre || '') + '</div>' +
            '<div class="s">' + escapeHtml(m.raza || '') + (m.refugio ? ' · ' + escapeHtml(m.refugio) : '') + '</div>' +
            '</div>';
        }).join('');
      }
      results.style.display = 'block';
    }

    input.addEventListener('input', function () {
      var term = this.value.trim();
      clearTimeout(timer);
      if (term.length < 1) { results.style.display = 'none'; return; }
      timer = setTimeout(function () {
        fetch(urls.buscarMascota + '?term=' + encodeURIComponent(term), { credentials: 'same-origin' })
          .then(function (r) { return r.json(); })
          .then(render)
          .catch(function () { results.style.display = 'none'; });
      }, 200);
    });

    results.addEventListener('click', function (e) {
      var item = e.target.closest('.rc-search-item');
      if (!item) return;
      checkin.mascId = parseInt(item.dataset.id, 10) || 0;
      checkin.mascNombre = item.dataset.nombre || '';
      input.value = '';
      results.style.display = 'none';
      showChip('rcMascSel', checkin.mascNombre, function () { checkin.mascId = null; checkin.mascNombre = ''; });
    });

    document.addEventListener('click', function (e) {
      if (!input.contains(e.target) && !results.contains(e.target)) results.style.display = 'none';
    });
  })();

  // Link "Registrar mascota nueva"
  var crearMasc = document.getElementById('rcCrearMasc');
  if (crearMasc && urls.crearMascota) crearMasc.setAttribute('href', urls.crearMascota);

  // Submit check-in
  var btnCheckIn = document.getElementById('rcBtnCheckIn');
  if (btnCheckIn) {
    btnCheckIn.addEventListener('click', function () {
      var err = document.getElementById('rcModalError');
      function showErr(msg) { if (err) { err.textContent = msg; err.style.display = 'block'; } else { notify('warning', msg); } }

      if (!checkin.mascId) { showErr('Selecciona una mascota de la lista.'); return; }
      if (checkin.tipo === 'cliente' && !checkin.cliId) { showErr('Selecciona o crea el dueño del paciente.'); return; }

      var tc = document.getElementById('rcTipoConsulta');
      var peso = document.getElementById('rcPeso');
      var vet = document.getElementById('rcVeterinario');
      var payload = {
        mascId: checkin.mascId,
        tipoConsultaId: (tc && tc.value) ? parseInt(tc.value, 10) : null,
        motivo: (document.getElementById('rcMotivo') || {}).value || '',
        prioridad: (document.getElementById('rcPrioridad') || {}).value || 'Normal',
        vetId: (vet && vet.value) ? parseInt(vet.value, 10) : null,
        peso: (peso && peso.value) ? parseFloat(peso.value) : null
      };

      btnCheckIn.disabled = true;
      var orig = btnCheckIn.innerHTML;
      btnCheckIn.innerHTML = '<i class="bi bi-hourglass-split"></i> Registrando…';

      postJson(urls.checkIn, payload)
        .then(function (res) {
          btnCheckIn.disabled = false; btnCheckIn.innerHTML = orig;
          if (res && res.ok) {
            closeModal();
            notify('success', 'Paciente en sala de espera');
            loadQueue();
          } else {
            showErr((res && res.mensaje) || 'No se pudo registrar la visita.');
          }
        })
        .catch(function () {
          btnCheckIn.disabled = false; btnCheckIn.innerHTML = orig;
          showErr('Error de conexión.');
        });
    });
  }

  // ───────────────────────── Init ─────────────────────────
  loadTiposConsulta();
  loadVeterinarios();
  loadQueue();
  refreshTimer = setInterval(loadQueue, REFRESH_MS);
})();
