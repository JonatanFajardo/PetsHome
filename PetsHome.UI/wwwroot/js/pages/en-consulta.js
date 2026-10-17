/* ============================================================
   En Consulta — cabina de trabajo del veterinario
   - Buscador para cambiar de paciente (reutiliza /Mascota/Search)
   - SearchSelect de medicamentos y vacunas (componente global)
   - Finalizar -> POST /Consulta/Guardar (JSON)
   Rutas via window._urls (no hardcodear).
   ============================================================ */
(function () {
  'use strict';

  var urls = window._urls || {};
  var mascId = window._mascId || 0;
  var citaId = window._citaId || 0;   // viene de sala de espera (opcional)

  // Estado en memoria de las lineas agregadas
  // {id, nombre(limpio), nombreOriginal, dosis, frecuencia, duracion,
  //  cantidad, entregar, enStock, stock, stockMin}
  var medsSeleccionados = [];
  var vacunaAplicada = null;    // {id, nombre}

  // Frecuencias comunes para el select de la receta
  var FRECUENCIAS = ['c/8h', 'c/12h', 'c/24h', 'c/48h'];

  function escapeHtml(s) {
    return String(s == null ? '' : s)
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;');
  }

  // Quita la presentación/envase al final del nombre para dejar solo el fármaco.
  //  "Antibiotico Amoxicilina 100ml" -> "Antibiotico Amoxicilina"
  //  "Ibuprofeno 5mg x12"            -> "Ibuprofeno"
  function cleanMedName(name) {
    var s = String(name == null ? '' : name).trim();
    var prev;
    do {
      prev = s;
      // unidad de medida: 100ml, 5mg, 1L, 10cc, 250mcg, 500ui...
      s = s.replace(/\s*\d+(?:[.,]\d+)?\s*(ml|mg|mcg|kg|cc|ui|g|l)\b\.?$/i, '');
      // multiplicador de envase: x12, x 50
      s = s.replace(/\s*x\s*\d+\b\.?$/i, '');
      s = s.trim();
    } while (s !== prev && s.length);
    return s.length ? s : String(name == null ? '' : name).trim();
  }

  // Convierte un valor a entero o NaN (tolera null/''/string).
  function toInt(v) {
    if (v == null || v === '') return NaN;
    var n = parseInt(v, 10);
    return isNaN(n) ? NaN : n;
  }

  // Badge de estado de inventario para la LÍNEA de receta.
  function medBadge(m) {
    var enInventario = m.enStock === true && m.id > 0;
    if (!enInventario) {
      return '<span class="ec-med-badge off-stock">⚪ No está en inventario</span>';
    }
    var s = toInt(m.stock); if (isNaN(s)) s = 0;
    var min = toInt(m.stockMin); if (isNaN(min)) min = -1; // sin stockMin -> no se evalúa "bajo"
    if (s <= 0) {
      return '<span class="ec-med-badge agotado">⚪ Agotado</span>';
    }
    if (min >= 0 && s <= min) {
      return '<span class="ec-med-badge low-stock">🟠 Quedan ' + escapeHtml(s) + ' (bajo)</span>';
    }
    return '<span class="ec-med-badge in-stock">🟢 En inventario · ' + escapeHtml(s) + '</span>';
  }

  // ───────────────────────── Buscador de paciente ─────────────────────────
  (function initBuscador() {
    var input = document.getElementById('ecMascSearch');
    var results = document.getElementById('ecMascResults');
    if (!input || !results) return;
    var timer = null;

    function position() {
      var r = input.getBoundingClientRect();
      results.style.top = (r.bottom + 6) + 'px';
      results.style.left = r.left + 'px';
      results.style.width = Math.max(r.width, 300) + 'px';
    }

    function nuevoFooter() {
      if (!urls.crearMascota) return '';
      return '<a class="ec-search-nuevo" href="' + urls.crearMascota + '">' +
        '<i class="bi bi-plus-circle"></i> Registrar nuevo paciente</a>';
    }

    function render(items) {
      if (!items || !items.length) {
        results.innerHTML = '<div style="padding:14px 16px;color:#9ca3af;font-size:13px;">Sin coincidencias</div>' + nuevoFooter();
      } else {
        results.innerHTML = items.map(function (m) {
          return '<div class="ec-search-item" data-id="' + m.id + '">' +
            '<div class="n">' + (m.nombre || '') + '</div>' +
            '<div class="s">' + (m.raza || '') + (m.refugio ? ' · ' + m.refugio : '') + '</div>' +
            '</div>';
        }).join('') + nuevoFooter();
      }
      results.style.display = 'block';
    }

    input.addEventListener('input', function () {
      var term = this.value.trim();
      clearTimeout(timer);
      if (term.length < 1) { results.style.display = 'none'; return; }
      position();
      timer = setTimeout(function () {
        fetch(urls.buscarMascota + '?term=' + encodeURIComponent(term), { credentials: 'same-origin' })
          .then(function (r) { return r.json(); })
          .then(render)
          .catch(function () { results.style.display = 'none'; });
      }, 200);
    });

    results.addEventListener('click', function (e) {
      var item = e.target.closest('.ec-search-item');
      if (!item) return;
      window.location.href = urls.consultaIndex + '?mascId=' + item.dataset.id;
    });

    document.addEventListener('click', function (e) {
      if (!input.contains(e.target) && !results.contains(e.target))
        results.style.display = 'none';
    });
  })();

  // ───────────────────────── Estado vacío: sala de espera + buscador ─────────────────────────
  // Estos elementos solo existen cuando NO hay paciente cargado (Model.SinPaciente).
  // El bloque entero degrada con gracia si no están presentes.
  (function initEstadoVacio() {
    // --- Botón "Atender" de la cola de hoy ---
    var queue = document.querySelector('.ec-queue');
    if (queue) {
      queue.addEventListener('click', function (e) {
        var btn = e.target.closest('.ec-queue-atender');
        if (!btn) return;
        var cita = parseInt(btn.dataset.cita, 10) || 0;
        var masc = parseInt(btn.dataset.masc, 10) || 0;
        var consultaUrl = urls.consultaIndex || window.location.pathname;

        // Sin cita válida → navegar directo con el mascId del item.
        if (!cita) {
          if (masc) window.location.href = consultaUrl + '?mascId=' + masc;
          return;
        }

        btn.disabled = true;
        var original = btn.innerHTML;
        btn.innerHTML = '<i class="bi bi-hourglass-split"></i> Atendiendo...';

        var tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
        if (!urls.atender) {
          // Sin endpoint de atender → fallback directo.
          window.location.href = consultaUrl + '?mascId=' + masc + '&citaId=' + cita;
          return;
        }
        fetch(urls.atender, {
          method: 'POST',
          credentials: 'same-origin',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': tokenEl ? tokenEl.value : ''
          },
          body: JSON.stringify({ citaId: cita })
        })
          .then(function (r) { return r.json(); })
          .then(function (res) {
            if (res && res.ok) {
              window.location.href = consultaUrl + '?mascId=' + (res.mascId || masc) + '&citaId=' + cita;
            } else {
              // Fallback: navegar con el mascId del item.
              window.location.href = consultaUrl + '?mascId=' + masc;
            }
          })
          .catch(function () {
            window.location.href = consultaUrl + '?mascId=' + masc;
          });
      });
    }

    // --- Buscador de expediente del estado vacío ---
    var input = document.getElementById('ecEmptySearch');
    var results = document.getElementById('ecEmptyResults');
    if (input && results) {
      var timer = null;

      function position() {
        var r = input.getBoundingClientRect();
        results.style.top = (r.bottom + 6) + 'px';
        results.style.left = r.left + 'px';
        results.style.width = Math.max(r.width, 300) + 'px';
      }

      function nuevoFooter() {
        if (!urls.crearMascota) return '';
        return '<a class="ec-search-nuevo" href="' + urls.crearMascota + '">' +
          '<i class="bi bi-plus-circle"></i> Registrar nuevo paciente</a>';
      }

      function render(items) {
        if (!items || !items.length) {
          results.innerHTML = '<div style="padding:14px 16px;color:#9ca3af;font-size:13px;">Sin coincidencias</div>' + nuevoFooter();
        } else {
          results.innerHTML = items.map(function (m) {
            return '<div class="ec-search-item" data-id="' + m.id + '">' +
              '<div class="n">' + escapeHtml(m.nombre || '') + '</div>' +
              '<div class="s">' + escapeHtml((m.raza || '') + (m.refugio ? ' · ' + m.refugio : '')) + '</div>' +
              '</div>';
          }).join('') + nuevoFooter();
        }
        results.style.display = 'block';
      }

      input.addEventListener('input', function () {
        var term = this.value.trim();
        clearTimeout(timer);
        if (term.length < 1) { results.style.display = 'none'; return; }
        position();
        timer = setTimeout(function () {
          fetch(urls.buscarMascota + '?term=' + encodeURIComponent(term), { credentials: 'same-origin' })
            .then(function (r) { return r.json(); })
            .then(render)
            .catch(function () { results.style.display = 'none'; });
        }, 200);
      });

      results.addEventListener('click', function (e) {
        var item = e.target.closest('.ec-search-item');
        if (!item) return;
        window.location.href = (urls.consultaIndex || window.location.pathname) + '?mascId=' + item.dataset.id;
      });

      document.addEventListener('click', function (e) {
        if (!input.contains(e.target) && !results.contains(e.target))
          results.style.display = 'none';
      });
    }
  })();

  // ───────────────────────── Render de lineas ─────────────────────────
  function renderMeds() {
    var box = document.getElementById('ecMedLines');
    if (!box) return;
    if (!medsSeleccionados.length) {
      box.innerHTML = '<div class="ec-empty-line">Aún no has agregado medicamentos.</div>';
      return;
    }
    box.innerHTML = medsSeleccionados.map(function (m, i) {
      var enInventario = m.enStock === true && m.id > 0;
      var s = toInt(m.stock); if (isNaN(s)) s = 0;
      var agotado = enInventario && s <= 0;
      var badge = medBadge(m);

      var nameEsc = escapeHtml(m.nombre);
      var titleAttr = (m.nombreOriginal && m.nombreOriginal !== m.nombre)
        ? ' title="' + escapeHtml(m.nombreOriginal) + '"' : '';

      // Select de frecuencia (conserva valores libres fuera de la lista común)
      var freqOpts = '<option value="">Frecuencia…</option>' +
        FRECUENCIAS.map(function (f) {
          return '<option value="' + f + '"' + (m.frecuencia === f ? ' selected' : '') + '>' + f + '</option>';
        }).join('');
      if (m.frecuencia && FRECUENCIAS.indexOf(m.frecuencia) === -1) {
        freqOpts += '<option value="' + escapeHtml(m.frecuencia) + '" selected>' + escapeHtml(m.frecuencia) + '</option>';
      }

      // Fila de entrega (solo para items de inventario)
      var entregaRow = '';
      if (enInventario) {
        var chkDisabled = agotado ? ' disabled' : '';
        var checked = (m.entregar && !agotado) ? ' checked' : '';
        var qtyDisabled = (agotado || !m.entregar) ? ' disabled' : '';
        var maxAttr = (s > 0) ? ' max="' + s + '"' : '';
        entregaRow =
          '<div class="ec-med-entrega">' +
            '<label class="ec-check' + (agotado ? ' is-disabled' : '') + '">' +
              '<input type="checkbox" class="ec-med-chk" data-i="' + i + '"' + checked + chkDisabled + '>' +
              '<span>Entregar del inventario</span>' +
            '</label>' +
            '<span class="ec-med-qty">Cant.' +
              '<input type="number" class="ec-med-field" data-i="' + i + '" data-field="cantidad" ' +
                'min="1"' + maxAttr + ' value="' + escapeHtml(m.cantidad || 1) + '"' + qtyDisabled + '>' +
            '</span>' +
          '</div>';
      }

      return '<div class="ec-line ec-line-med">' +
        '<div class="ec-med-head">' +
          '<span class="ec-line-ico"><i class="bi bi-capsule"></i></span>' +
          '<span class="ec-line-name"' + titleAttr + '>' + nameEsc + '</span>' +
          badge +
          '<button type="button" class="ec-line-del" data-i="' + i + '">&times;</button>' +
        '</div>' +
        '<div class="ec-med-inputs">' +
          '<input type="text" class="ec-med-field" data-i="' + i + '" data-field="dosis" ' +
            'placeholder="Dosis (ej. 0.5 ml)" value="' + escapeHtml(m.dosis || '') + '">' +
          '<select class="ec-med-field ec-med-sel" data-i="' + i + '" data-field="frecuencia">' + freqOpts + '</select>' +
          '<input type="text" class="ec-med-field" data-i="' + i + '" data-field="duracion" ' +
            'placeholder="Duración (ej. 7 días)" value="' + escapeHtml(m.duracion || '') + '">' +
        '</div>' +
        entregaRow +
      '</div>';
    }).join('');
  }

  // Edición in-place de los campos de cada línea (sin re-render, para no perder foco).
  (function initMedLineEvents() {
    var box = document.getElementById('ecMedLines');
    if (!box) return;

    box.addEventListener('input', function (e) {
      var f = e.target.closest('.ec-med-field');
      if (!f) return;
      var m = medsSeleccionados[parseInt(f.dataset.i, 10)];
      if (!m) return;
      if (f.dataset.field === 'cantidad') {
        var v = parseInt(f.value, 10);
        m.cantidad = (isNaN(v) || v < 1) ? 1 : v;
      } else {
        m[f.dataset.field] = f.value;
      }
    });

    box.addEventListener('change', function (e) {
      var chk = e.target.closest('.ec-med-chk');
      if (chk) {
        var m = medsSeleccionados[parseInt(chk.dataset.i, 10)];
        if (!m) return;
        m.entregar = chk.checked;
        var line = chk.closest('.ec-line-med');
        var qty = line ? line.querySelector('[data-field="cantidad"]') : null;
        if (qty) qty.disabled = !chk.checked;
        return;
      }
      var sel = e.target.closest('.ec-med-sel');
      if (sel) {
        var ms = medsSeleccionados[parseInt(sel.dataset.i, 10)];
        if (ms) ms.frecuencia = sel.value;
      }
    });
  })();

  // Agrega un medicamento (desde SearchSelect o desde un chip de sugerencia),
  // conservando enStock/stock y evitando duplicados.
  function addMed(item) {
    var id = item.id || 0;
    var rawNombre = item.text || item.nombre || '';
    if (!rawNombre) return;
    var nombre = cleanMedName(rawNombre);
    var dup = medsSeleccionados.some(function (m) {
      return (id > 0 && m.id === id) ||
             (m.nombre || '').toLowerCase() === nombre.toLowerCase();
    });
    if (dup) return;
    var enStock = item.enStock === true;
    var stockNum = toInt(item.stock);
    medsSeleccionados.push({
      id: id,
      nombre: nombre,
      nombreOriginal: rawNombre,
      dosis: item.dosis || '',
      frecuencia: '',
      duracion: '',
      cantidad: 1,
      enStock: enStock,
      stock: (item.stock != null ? item.stock : null),
      stockMin: (item.stockMin != null ? item.stockMin : null),
      entregar: (enStock && id > 0 && !isNaN(stockNum) && stockNum > 0)
    });
    renderMeds();
  }

  function setVacuna(item) {
    vacunaAplicada = { id: item.id || 0, nombre: item.text || item.nombre || '' };
    renderVacuna();
  }

  function renderVacuna() {
    var box = document.getElementById('ecVacLines');
    if (!box) return;
    if (!vacunaAplicada) {
      box.innerHTML = '<div class="ec-empty-line">Ninguna vacuna aplicada en esta consulta.</div>';
      return;
    }
    box.innerHTML = '<div class="ec-line">' +
      '<span class="ec-line-ico"><i class="bi bi-shield-plus"></i></span>' +
      '<span class="ec-line-txt">' + vacunaAplicada.nombre + '</span>' +
      '<button type="button" class="ec-line-del" data-vac="1">&times;</button>' +
      '</div>';
  }

  document.addEventListener('click', function (e) {
    var del = e.target.closest('.ec-line-del');
    if (!del) return;
    if (del.dataset.vac) { vacunaAplicada = null; renderVacuna(); }
    else { medsSeleccionados.splice(parseInt(del.dataset.i, 10), 1); renderMeds(); }
  });

  // ───────────────────────── SearchSelect: meds y vacunas ─────────────────────────
  if (window.SearchSelect) {
    var medEl = document.getElementById('ecMedSelect');
    if (medEl) {
      SearchSelect.create(medEl, {
        tipo: 'medicamento',
        searchUrl: '/Lookup/Search',
        permiteCrear: true,
        localCreate: true,           // recetar texto libre sin tocar el catálogo
        minChars: 0,
        placeholder: 'Buscar o recetar medicamento...',
        createText: function (termEsc) {
          return '<span class="ss-create-plus">＋</span> Recetar «' + termEsc +
            '» <span class="ss-create-note">(fuera de inventario)</span>';
        },
        onSelect: addMed,
        onCreate: addMed
      });
    }

    var vacEl = document.getElementById('ecVacSelect');
    if (vacEl) {
      SearchSelect.create(vacEl, {
        tipo: 'vacuna',
        searchUrl: '/Lookup/Search',
        createUrl: '/Lookup/Create',
        permiteCrear: false,
        minChars: 0,
        placeholder: 'Buscar vacuna...',
        onSelect: setVacuna
      });
    }
  }

  // ───────────────────────── Sugerencias por diagnóstico ─────────────────────────
  (function initSugerencias() {
    var ta = document.getElementById('ecDiagnostico');
    var box = document.getElementById('ecSugerencias');
    if (!ta || !box || !urls.sugerencias) return;
    var timer = null;

    function hide() { box.style.display = 'none'; box.innerHTML = ''; }

    function renderSug(data) {
      data = data || {};
      var meds = Array.isArray(data.medicamentos) ? data.medicamentos : [];
      var vacs = Array.isArray(data.vacunas) ? data.vacunas : [];
      if (!meds.length && !vacs.length) { hide(); return; }

      var html = '<div class="ec-sug-title">💡 Sugerencias</div><div class="ec-sug-chips">';
      html += meds.map(function (m) {
        var cls = m.enStock ? 'med-in' : 'med-off';
        var stock = (m.enStock && m.stock != null) ? ' · ' + escapeHtml(m.stock) : '';
        return '<button type="button" class="ec-chip ' + cls + '" data-kind="med"' +
          ' data-id="' + (m.id || 0) + '"' +
          ' data-text="' + escapeHtml(m.text) + '"' +
          ' data-enstock="' + (m.enStock ? '1' : '0') + '"' +
          ' data-stock="' + (m.stock != null ? escapeHtml(m.stock) : '') + '">' +
          '<i class="bi bi-capsule"></i> ' + escapeHtml(m.text) + stock + '</button>';
      }).join('');
      html += vacs.map(function (v) {
        return '<button type="button" class="ec-chip vac" data-kind="vac"' +
          ' data-id="' + (v.id || 0) + '"' +
          ' data-text="' + escapeHtml(v.text) + '">' +
          '💉 ' + escapeHtml(v.text) + '</button>';
      }).join('');
      html += '</div>';
      box.innerHTML = html;
      box.style.display = 'block';
    }

    ta.addEventListener('input', function () {
      var texto = this.value.trim();
      clearTimeout(timer);
      if (texto.length < 4) { hide(); return; }
      timer = setTimeout(function () {
        fetch(urls.sugerencias + '?texto=' + encodeURIComponent(texto), { credentials: 'same-origin' })
          .then(function (r) { return r.json(); })
          .then(renderSug)
          .catch(function () { hide(); });
      }, 400);
    });

    box.addEventListener('click', function (e) {
      var chip = e.target.closest('.ec-chip');
      if (!chip) return;
      if (chip.dataset.kind === 'med') {
        addMed({
          id: parseInt(chip.dataset.id, 10) || 0,
          text: chip.dataset.text,
          enStock: chip.dataset.enstock === '1',
          stock: chip.dataset.stock !== '' ? chip.dataset.stock : null
        });
      } else {
        setVacuna({ id: parseInt(chip.dataset.id, 10) || 0, text: chip.dataset.text });
      }
    });
  })();

  // ───────────────────────── Acciones rápidas ─────────────────────────
  var btnSeguimiento = document.getElementById('btnAgendarSeguimiento');
  if (btnSeguimiento) {
    btnSeguimiento.addEventListener('click', function () {
      var fecha = (document.getElementById('ecProximaCita') || {}).value || '';
      var q = urls.crearCita + '?masc_Id=' + mascId + (fecha ? '&fecha=' + encodeURIComponent(fecha) : '');
      window.location.href = q;
    });
  }

  var btnReceta = document.getElementById('btnGenerarReceta');
  if (btnReceta) {
    btnReceta.addEventListener('click', function () {
      // Enfoca el flujo de recetar; la emision formal vive en el modulo de Recetas.
      var el = document.getElementById('ecMedSelect');
      if (el) { el.scrollIntoView({ behavior: 'smooth', block: 'center' }); }
    });
  }

  // ───────────────────────── Finalizar / Guardar ─────────────────────────
  function val(id) { var el = document.getElementById(id); return el ? el.value.trim() : ''; }
  function numOrNull(id) { var v = val(id); return v === '' ? null : parseFloat(v); }

  var btn = document.getElementById('btnFinalizar');
  if (btn) {
    btn.addEventListener('click', function () {
      var payload = {
        mascId: mascId,
        citaId: citaId || null,   // finaliza esta visita (sala de espera) en vez de crear otra
        tipoConsultaId: val('ecTipoConsulta') ? parseInt(val('ecTipoConsulta'), 10) : null,
        motivo: val('ecMotivo'),
        diagnostico: val('ecDiagnostico'),
        peso: numOrNull('ecPeso'),
        temperatura: numOrNull('ecTemperatura'),
        vacunaId: vacunaAplicada ? vacunaAplicada.id : null,
        medicamentos: medsSeleccionados.map(function (m) {
          return {
            id: m.id,
            nombre: m.nombre,            // nombre LIMPIO (sin envase)
            dosis: m.dosis,
            frecuencia: m.frecuencia,
            duracion: m.duracion,
            enStock: m.enStock === true,
            entregar: !!m.entregar,
            cantidad: (m.cantidad && m.cantidad > 0) ? m.cantidad : 1
          };
        }),
        proximaCita: val('ecProximaCita') || null,
        motivoProximaCita: val('ecMotivoProxima')
      };

      btn.disabled = true;
      var original = btn.innerHTML;
      btn.innerHTML = '<i class="bi bi-hourglass-split"></i> Guardando...';

      var tokenEl = document.querySelector('input[name="__RequestVerificationToken"]');
      fetch(urls.guardar, {
        method: 'POST',
        credentials: 'same-origin',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': tokenEl ? tokenEl.value : ''
        },
        body: JSON.stringify(payload)
      })
        .then(function (r) { return r.json(); })
        .then(function (res) {
          if (res && res.ok) {
            if (window.Swal) {
              Swal.fire({ icon: 'success', title: 'Consulta finalizada', text: res.mensaje || '', timer: 1800, showConfirmButton: false })
                .then(function () { window.location.href = urls.consultaIndex + '?mascId=' + mascId; });
            } else {
              alert(res.mensaje || 'Consulta finalizada');
              window.location.href = urls.consultaIndex + '?mascId=' + mascId;
            }
          } else {
            btn.disabled = false; btn.innerHTML = original;
            if (res && Array.isArray(res.faltantes) && res.faltantes.length) {
              // Bloqueo por falta de stock: no se creó nada, listar faltantes.
              var faltas = res.faltantes.map(function (f) {
                var disp = (f.disponible != null) ? f.disponible : 0;
                var ped = (f.pedido != null) ? f.pedido : 0;
                return 'Sin stock suficiente de ' + (f.nombre || 'medicamento') +
                  ' (quedan ' + disp + ', pedías ' + ped + ')';
              });
              if (window.Swal) {
                Swal.fire({
                  icon: 'warning',
                  title: 'No se puede finalizar',
                  html: faltas.map(escapeHtml).join('<br>')
                });
              } else {
                alert(faltas.join('\n'));
              }
            } else {
              var msg = (res && res.mensaje) ? res.mensaje : 'No se pudo guardar la consulta.';
              if (window.Swal) { Swal.fire({ icon: 'error', title: 'Error', text: msg }); }
              else { alert(msg); }
            }
          }
        })
        .catch(function () {
          btn.disabled = false; btn.innerHTML = original;
          if (window.Swal) { Swal.fire({ icon: 'error', title: 'Error', text: 'Error de conexión.' }); }
          else { alert('Error de conexión.'); }
        });
    });
  }
})();
