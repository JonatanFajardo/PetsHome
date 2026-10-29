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
  // mascId robusto: window._mascId y, si falta, el ?mascId= de la URL.
  var mascId = window._mascId || 0;
  if (!mascId) {
    try { var _mq = new URLSearchParams(window.location.search).get('mascId'); if (_mq) mascId = parseInt(_mq, 10) || 0; } catch (e) {}
  }
  var citaId = window._citaId || 0;   // viene de sala de espera (opcional)

  // Estado en memoria de las lineas agregadas
  // {id, nombre(limpio), nombreOriginal, dosis, frecuencia, duracion,
  //  cantidad, entregar, enStock, stock, stockMin}
  var medsSeleccionados = [];
  var vacunaAplicada = null;    // {id, nombre}

  // Servicios facturables de la visita (vacunas / laboratorios / procedimientos)
  // {servId, nombre, precio, categoria, cantidad}
  var serviciosSeleccionados = [];
  var servCatalogCache = {};    // categoria -> [{id, nombre, precio, categoria}]

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

  // Formatea un importe en moneda (tolera null/string).
  function money(v) {
    var n = parseFloat(v);
    if (isNaN(n)) n = 0;
    return '$' + n.toFixed(2);
  }

  // Ícono por categoría de servicio.
  function servIcon(cat) {
    var c = String(cat || '').toLowerCase();
    if (c.indexOf('vacuna') > -1) return '<i class="bi bi-shield-plus"></i>';
    if (c.indexOf('laborat') > -1) return '<i class="bi bi-droplet-half"></i>';
    if (c.indexOf('proced') > -1) return '<i class="bi bi-scissors"></i>';
    return '<i class="bi bi-receipt"></i>';
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

        if (!urls.atender) {
          // Sin endpoint de atender → fallback directo.
          window.location.href = consultaUrl + '?mascId=' + masc + '&citaId=' + cita;
          return;
        }
        window.postForm(urls.atender, { citaId: cita })
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
    if (del.dataset.serv != null) { serviciosSeleccionados.splice(parseInt(del.dataset.serv, 10), 1); renderServicios(); }
    else if (del.dataset.vac) { vacunaAplicada = null; renderVacuna(); }
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

  // ───────────────────────── Servicios de la visita ─────────────────────────
  // Catálogo por categoría (contrato fijo): GET /Consulta/Servicios?categoria=...
  //   -> [{ id, nombre, precio, categoria }]. Se pobla con window.getJson (sin fetch manual).
  function addServicio(it) {
    if (!it) return;
    var servId = toInt(it.id); if (isNaN(servId)) servId = 0;
    var nombre = it.nombre || it.text || '';
    if (!nombre) return;
    var precio = parseFloat(it.precio); if (isNaN(precio)) precio = 0;
    var categoria = it.categoria || '';

    // Mismo servicio del catálogo (servId > 0) -> suma cantidad en vez de duplicar.
    if (servId > 0) {
      var exist = serviciosSeleccionados.find(function (s) { return s.servId === servId; });
      if (exist) { exist.cantidad = (exist.cantidad || 1) + 1; renderServicios(); return; }
    }
    serviciosSeleccionados.push({
      servId: servId,
      nombre: nombre,
      precio: precio,
      categoria: categoria,
      cantidad: 1
    });
    renderServicios();
  }

  function servSubtotal() {
    return serviciosSeleccionados.reduce(function (acc, s) {
      var c = (s.cantidad && s.cantidad > 0) ? s.cantidad : 1;
      return acc + (parseFloat(s.precio) || 0) * c;
    }, 0);
  }

  function updateServSubtotal() {
    var sub = document.getElementById('ecServSubtotal');
    if (!sub) return;
    if (!serviciosSeleccionados.length) { sub.style.display = 'none'; sub.innerHTML = ''; return; }
    sub.style.display = 'flex';
    sub.innerHTML = '<span>Subtotal servicios</span><b>' + money(servSubtotal()) + '</b>';
  }

  function renderServicios() {
    var box = document.getElementById('ecServLines');
    if (!box) return;
    if (!serviciosSeleccionados.length) {
      box.innerHTML = '<div class="ec-empty-line">Aún no has agregado servicios.</div>';
      updateServSubtotal();
      return;
    }
    box.innerHTML = serviciosSeleccionados.map(function (s, i) {
      var cant = (s.cantidad && s.cantidad > 0) ? s.cantidad : 1;
      var importe = (parseFloat(s.precio) || 0) * cant;
      var catBadge = s.categoria ? '<span class="ec-serv-cat">' + escapeHtml(s.categoria) + '</span>' : '';
      return '<div class="ec-line ec-line-serv">' +
        '<div class="ec-serv-head">' +
          '<span class="ec-line-ico">' + servIcon(s.categoria) + '</span>' +
          '<span class="ec-line-name">' + escapeHtml(s.nombre) + '</span>' +
          catBadge +
          '<span class="ec-serv-precio">' + money(s.precio) + '</span>' +
          '<button type="button" class="ec-line-del" data-serv="' + i + '">&times;</button>' +
        '</div>' +
        '<div class="ec-serv-foot">' +
          '<span class="ec-med-qty">Cant.' +
            '<input type="number" class="ec-serv-field" data-i="' + i + '" min="1" value="' + escapeHtml(cant) + '">' +
          '</span>' +
          '<span class="ec-serv-importe">' + money(importe) + '</span>' +
        '</div>' +
      '</div>';
    }).join('');
    updateServSubtotal();
  }

  (function initServicios() {
    var catSel = document.getElementById('ecServCategoria');
    var catalogSel = document.getElementById('ecServCatalogo');
    var addBtn = document.getElementById('ecServAdd');
    var box = document.getElementById('ecServLines');
    if (!catSel || !catalogSel || !addBtn) return;

    function fillCatalog(items) {
      if (!items || !items.length) {
        catalogSel.innerHTML = '<option value="">Sin servicios en esta categoría</option>';
        catalogSel.disabled = true; addBtn.disabled = true; return;
      }
      var opts = ['<option value="">Elige un servicio...</option>'];
      items.forEach(function (it, idx) {
        var precioTxt = (it.precio != null && it.precio !== '') ? ' · ' + money(it.precio) : '';
        opts.push('<option value="' + idx + '">' + escapeHtml(it.nombre) + precioTxt + '</option>');
      });
      catalogSel.innerHTML = opts.join('');
      catalogSel.disabled = false; addBtn.disabled = true;
    }

    catSel.addEventListener('change', function () {
      var cat = catSel.value;
      addBtn.disabled = true;
      if (!cat) {
        catalogSel.innerHTML = '<option value="">Elige una categoría primero...</option>';
        catalogSel.disabled = true; return;
      }
      if (servCatalogCache[cat]) { fillCatalog(servCatalogCache[cat]); return; }
      catalogSel.disabled = true;
      catalogSel.innerHTML = '<option value="">Cargando...</option>';
      window.getJson(urls.servicios + '?categoria=' + encodeURIComponent(cat))
        .then(function (data) {
          var items = Array.isArray(data) ? data : [];
          servCatalogCache[cat] = items;
          if (catSel.value === cat) fillCatalog(items);
        })
        .catch(function () {
          if (catSel.value === cat) catalogSel.innerHTML = '<option value="">No se pudo cargar el catálogo</option>';
        });
    });

    catalogSel.addEventListener('change', function () {
      addBtn.disabled = !catalogSel.value;
    });

    addBtn.addEventListener('click', function () {
      var cat = catSel.value;
      var idx = catalogSel.value;
      if (!cat || idx === '') return;
      var items = servCatalogCache[cat] || [];
      var it = items[parseInt(idx, 10)];
      if (!it) return;
      addServicio(it);
      catalogSel.value = '';   // deja el selector listo para agregar otro
      addBtn.disabled = true;
    });

    // Editar cantidad in-place (sin re-render completo para no perder foco).
    if (box) {
      box.addEventListener('input', function (e) {
        var f = e.target.closest('.ec-serv-field');
        if (!f) return;
        var s = serviciosSeleccionados[parseInt(f.dataset.i, 10)];
        if (!s) return;
        var v = parseInt(f.value, 10);
        s.cantidad = (isNaN(v) || v < 1) ? 1 : v;
        var line = f.closest('.ec-line-serv');
        var imp = line ? line.querySelector('.ec-serv-importe') : null;
        if (imp) imp.textContent = money((parseFloat(s.precio) || 0) * s.cantidad);
        updateServSubtotal();
      });
    }
  })();

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

  // ───────────────────────── Validación suave de signos vitales ─────────────────────────
  // Rangos de referencia (guía general perro/gato; NO bloquea, solo avisa).
  // Fuera de rango => borde ámbar + nota; dentro => limpio.
  var VITAL_RANGES = {
    ecTemperatura:       { min: 37.5, max: 39.2, msg: 'Fuera del rango normal (37.5–39.2 °C).' },
    ecFC:                { min: 60,   max: 180,  msg: 'Fuera del rango normal (60–180 lpm).' },
    ecFR:                { min: 10,   max: 40,   msg: 'Fuera del rango normal (10–40 rpm).' },
    ecLlenadoCapilar:    { min: 1,    max: 2,    msg: 'Fuera del rango normal (1–2 seg).' },
    ecEscalaDolor:       { min: 0,    max: 3,    msg: 'Dolor moderado/severo (0–3 esperado).' },
    ecCondicionCorporal: { min: 4,    max: 5,    msg: 'Condición no ideal (4–5 es ideal, escala 1–9).' }
  };

  function noteFor(id) { return document.querySelector('.ec-vital-note[data-for="' + id + '"]'); }

  function checkVital(id) {
    var el = document.getElementById(id);
    var note = noteFor(id);
    if (!el || !note) return;
    var raw = (el.value || '').trim();

    // Mucosas: cualquier valor distinto de "Rosadas" es un hallazgo a valorar.
    if (id === 'ecMucosas') {
      if (raw && raw !== 'Rosadas') {
        el.classList.add('out-range');
        note.textContent = 'Hallazgo anormal, valorar.';
        note.classList.add('show');
      } else {
        el.classList.remove('out-range');
        note.classList.remove('show'); note.textContent = '';
      }
      return;
    }

    var r = VITAL_RANGES[id];
    if (!r || raw === '') {
      el.classList.remove('out-range');
      note.classList.remove('show'); note.textContent = '';
      return;
    }
    var n = parseFloat(raw);
    if (isNaN(n)) {
      el.classList.remove('out-range');
      note.classList.remove('show'); note.textContent = '';
      return;
    }
    if (n < r.min || n > r.max) {
      el.classList.add('out-range');
      note.textContent = r.msg;
      note.classList.add('show');
    } else {
      el.classList.remove('out-range');
      note.classList.remove('show'); note.textContent = '';
    }
  }

  (function initVitalValidation() {
    var ids = ['ecTemperatura', 'ecFC', 'ecFR', 'ecMucosas', 'ecLlenadoCapilar', 'ecEscalaDolor', 'ecCondicionCorporal'];
    ids.forEach(function (id) {
      var el = document.getElementById(id);
      if (!el) return;
      var evt = (el.tagName === 'SELECT') ? 'change' : 'input';
      el.addEventListener(evt, function () { checkVital(id); });
    });
  })();

  // ───────────────────────── Finalizar / Guardar ─────────────────────────
  function val(id) { var el = document.getElementById(id); return el ? el.value.trim() : ''; }
  function numOrNull(id) { var v = val(id); return v === '' ? null : parseFloat(v); }
  function intOrNull(id) { var v = val(id); if (v === '') return null; var n = parseInt(v, 10); return isNaN(n) ? null : n; }

  var btn = document.getElementById('btnFinalizar');
  if (btn) {
    btn.addEventListener('click', function () {
      var payload = {
        mascId: mascId,
        citaId: citaId || 0,   // 0 = crear cita nueva; >0 = finaliza esa visita de sala de espera
                               // (CitaId en el DTO es int NO nuleable: nunca mandar null)
        tipoConsultaId: val('ecTipoConsulta') ? parseInt(val('ecTipoConsulta'), 10) : null,
        motivo: val('ecMotivo'),
        diagnostico: val('ecDiagnostico'),
        peso: numOrNull('ecPeso'),
        temperatura: numOrNull('ecTemperatura'),
        frecuenciaCardiaca: intOrNull('ecFC'),
        frecuenciaRespiratoria: intOrNull('ecFR'),
        mucosas: val('ecMucosas') || null,
        llenadoCapilar: numOrNull('ecLlenadoCapilar'),
        escalaDolor: intOrNull('ecEscalaDolor'),
        condicionCorporal: numOrNull('ecCondicionCorporal'),
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
        servicios: serviciosSeleccionados.map(function (s) {
          return {
            servId: (s.servId && s.servId > 0) ? s.servId : 0,   // entero, nunca null
            nombre: s.nombre,
            precio: (s.precio != null && !isNaN(parseFloat(s.precio))) ? parseFloat(s.precio) : 0,
            cantidad: (s.cantidad && s.cantidad > 0) ? s.cantidad : 1
          };
        }),
        proximaCita: val('ecProximaCita') || null,
        motivoProximaCita: val('ecMotivoProxima')
      };

      btn.disabled = true;
      var original = btn.innerHTML;
      btn.innerHTML = '<i class="bi bi-hourglass-split"></i> Guardando...';

      window.postJson(urls.guardar, payload)
        .then(function (res) {
          if (res && res.ok) {
            // Checkout contextual: si se dispensó a un cliente, pasar al cobro de la visita.
            var destino = res.requiereCobro ? '/Cobros' : (urls.consultaIndex + '?mascId=' + mascId);
            var titulo = res.requiereCobro ? 'Consulta finalizada — pasa a cobrar la visita' : 'Consulta finalizada';
            if (window.Swal) {
              Swal.fire({ icon: 'success', title: titulo, text: res.mensaje || '', timer: 1800, showConfirmButton: false })
                .then(function () { window.location.href = destino; });
            } else {
              alert(res.mensaje || 'Consulta finalizada');
              window.location.href = destino;
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
