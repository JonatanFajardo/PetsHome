/*
 * procedimientos-consulta.js — bloque de PROCEDIMIENTOS ESTRUCTURADOS en la consulta.
 *
 * Adaptado de OpenVPM "procedures": nombre, descripcion, anestesia, duracion, notas
 * y, opcional, el servicio facturable. Es ADITIVO e INDEPENDIENTE del Finalizar de la
 * consulta: cada procedimiento se guarda al instante por AJAX (feed, no gate) y NO
 * bloquea ni altera el guardado de la cita. Reutiliza window.postJson/getJson/postForm.
 */
(function () {
  var urls = window._urls || {};
  var mascId = window._mascId || 0;
  var citaId = window._citaId || 0;

  var elForm = document.getElementById('ecProcAdd');
  if (!elForm || mascId <= 0) return; // sin paciente o sin bloque: nada que hacer

  var inNombre = document.getElementById('ecProcNombre');
  var inDesc = document.getElementById('ecProcDescripcion');
  var inDur = document.getElementById('ecProcDuracion');
  var inAnes = document.getElementById('ecProcAnestesia');
  var inNotas = document.getElementById('ecProcNotas');
  var selServ = document.getElementById('ecProcServicio');
  var lines = document.getElementById('ecProcLines');

  function esc(s) {
    return (s == null ? '' : String(s))
      .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;');
  }

  // ── Catalogo de servicios facturables de categoria Procedimiento (opcional) ──
  if (urls.servicios) {
    window.getJson(urls.servicios + '?categoria=Procedimiento')
      .then(function (items) {
        if (!Array.isArray(items) || !items.length) return;
        items.forEach(function (s) {
          var opt = document.createElement('option');
          opt.value = s.id;
          opt.textContent = s.nombre + (s.precio != null ? ' (L ' + s.precio + ')' : '');
          selServ.appendChild(opt);
        });
        selServ.disabled = false;
      })
      .catch(function () { /* el servicio es opcional: si falla, se guarda sin cobro */ });
  }

  function renderEmpty() {
    lines.innerHTML = '<div class="ec-empty-line">Aún no has agregado procedimientos.</div>';
  }

  function renderLine(p) {
    var meta = [];
    if (p.duracion) meta.push(p.duracion + ' min');
    if (p.anestesia) meta.push('Anestesia: ' + esc(p.anestesia));
    if (p.servicioNombre) meta.push('Facturable: ' + esc(p.servicioNombre));
    var sub = meta.length ? '<div class="ec-line-sub" style="color:var(--text-muted,#8E87B3);font-size:12px">' + meta.join(' · ') + '</div>' : '';
    var desc = p.descripcion ? '<div style="font-size:12px;color:var(--text-muted,#8E87B3)">' + esc(p.descripcion) + '</div>' : '';
    var row = document.createElement('div');
    row.className = 'ec-line';
    row.setAttribute('data-proc', p.procId);
    row.style.cssText = 'display:flex;justify-content:space-between;align-items:flex-start;gap:8px;padding:8px 10px;border:1px solid var(--border,#E7E3F5);border-radius:10px;margin-bottom:6px';
    row.innerHTML =
      '<div><div style="font-weight:600">' + esc(p.nombre) + '</div>' + desc + sub + '</div>' +
      '<button type="button" class="ec-line-del" title="Quitar" ' +
      'style="background:none;border:none;color:#DC2626;cursor:pointer;font-size:16px"><i class="bi bi-trash"></i></button>';
    row.querySelector('.ec-line-del').addEventListener('click', function () { eliminar(p.procId, row); });
    return row;
  }

  function appendLine(p) {
    var empty = lines.querySelector('.ec-empty-line');
    if (empty) lines.innerHTML = '';
    lines.appendChild(renderLine(p));
  }

  function cargarExistentes() {
    if (!citaId || citaId <= 0 || !urls.procPorCita) return;
    window.getJson(urls.procPorCita + '?citaId=' + citaId)
      .then(function (items) {
        if (Array.isArray(items) && items.length) {
          lines.innerHTML = '';
          items.forEach(appendLine);
        }
      })
      .catch(function () { /* sin bloqueo */ });
  }

  function limpiarForm() {
    inNombre.value = '';
    inDesc.value = '';
    inDur.value = '';
    inAnes.value = '';
    inNotas.value = '';
    if (selServ) selServ.value = '';
  }

  function toast(icon, title, text) {
    if (window.Swal) Swal.fire({ icon: icon, title: title, text: text || '', timer: 1600, showConfirmButton: false });
    else if (text) alert(text);
  }

  function agregar() {
    var nombre = (inNombre.value || '').trim();
    if (!nombre) { toast('warning', 'Falta el nombre', 'Escribe el nombre del procedimiento.'); inNombre.focus(); return; }

    var durRaw = (inDur.value || '').trim();
    var dur = durRaw === '' ? null : parseInt(durRaw, 10);

    var payload = {
      procId: 0,
      mascId: mascId,
      citaId: (citaId && citaId > 0) ? citaId : null,
      nombre: nombre,
      descripcion: (inDesc.value || '').trim() || null,
      anestesia: (inAnes.value || '').trim() || null,
      duracionMinutos: (dur != null && !isNaN(dur)) ? dur : null,
      notas: (inNotas.value || '').trim() || null,
      servId: (selServ && selServ.value) ? parseInt(selServ.value, 10) : null
    };

    elForm.disabled = true;
    var original = elForm.innerHTML;
    elForm.innerHTML = '<i class="bi bi-hourglass-split"></i> Guardando...';

    window.postJson(urls.procGuardar, payload)
      .then(function (res) {
        elForm.disabled = false; elForm.innerHTML = original;
        if (res && res.ok) {
          appendLine({
            procId: res.procId,
            nombre: payload.nombre,
            descripcion: payload.descripcion,
            duracion: payload.duracionMinutos,
            anestesia: payload.anestesia,
            servicioNombre: (payload.servId && selServ) ? selServ.options[selServ.selectedIndex].text : null
          });
          limpiarForm();
          var extra = res.cargoEncolado ? ' Se agregó al cobro de la visita.' : '';
          toast('success', 'Procedimiento agregado', (res.mensaje || '') + extra);
        } else {
          toast('error', 'No se pudo guardar', (res && res.mensaje) ? res.mensaje : 'Error al guardar el procedimiento.');
        }
      })
      .catch(function () {
        elForm.disabled = false; elForm.innerHTML = original;
        toast('error', 'Error', 'No se pudo guardar el procedimiento.');
      });
  }

  function eliminar(procId, row) {
    if (!urls.procEliminar || !procId) return;
    window.postForm(urls.procEliminar, { procId: procId })
      .then(function (res) {
        if (res && res.ok) {
          row.parentNode.removeChild(row);
          if (!lines.querySelector('.ec-line')) renderEmpty();
        } else {
          toast('error', 'No se pudo eliminar', (res && res.mensaje) ? res.mensaje : '');
        }
      })
      .catch(function () { toast('error', 'Error', 'No se pudo eliminar el procedimiento.'); });
  }

  elForm.addEventListener('click', agregar);
  cargarExistentes();
})();
