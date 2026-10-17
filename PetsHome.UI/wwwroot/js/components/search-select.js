/*
 * SearchSelect — dropdown reutilizable con buscador (typeahead) + crear al vuelo.
 * Vanilla JS, sin dependencias. Expuesto en window.SearchSelect.
 *
 * Uso:
 *   SearchSelect.create(containerEl, {
 *     tipo: 'tipomedicamento',         // arma /Lookup/Search?tipo=...
 *     searchUrl: '/Lookup/Search',     // override opcional
 *     createUrl: '/Lookup/Create',     // override opcional
 *     permiteCrear: true,              // muestra "＋ Crear ..." si no hay match exacto
 *     localCreate: false,              // si true, la fila crear NO hace POST: dispara onCreate({id:0,text:term,enStock:false})
 *     createText: (termEsc) => 'HTML', // texto HTML de la fila crear (termEsc ya viene escapado)
 *     placeholder: 'Buscar...',
 *     minChars: 1,
 *     debounce: 200,
 *     onSelect: (item) => {},          // item completo {id, text, meta, enStock, stock, ...}
 *     onCreate: (item) => {}           // tras crear -> item completo (incluye enStock)
 *   });
 *
 * Badges opcionales por fila (compatibles hacia atrás):
 *   - item.enStock === true + stock > stockMin        -> verde "🟢 En inventario · {stock}"
 *   - item.enStock === true + 0 < stock <= stockMin   -> ámbar "🟠 Quedan {stock} (bajo)"
 *   - item.enStock === true + stock <= 0              -> gris  "⚪ Agotado"
 *   - item.enStock === false + meta ~ "Recetado antes"-> gris  "⚪ Recetado antes"
 *   - sin flag enStock -> se ve igual que siempre (solo meta).
 *   (stockMin es opcional; si falta no se evalúa el estado "bajo".)
 *
 * Devuelve una instancia con: { setValue(item), clear(), getValue(), destroy() }.
 */
(function () {
  'use strict';

  function h(tag, cls, html) {
    var el = document.createElement(tag);
    if (cls) el.className = cls;
    if (html != null) el.innerHTML = html;
    return el;
  }

  function escapeHtml(s) {
    return String(s == null ? '' : s)
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;');
  }

  function getAntiForgeryToken() {
    var el = document.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
  }

  function create(container, options) {
    if (!container) throw new Error('SearchSelect: container requerido');
    var opts = options || {};

    var cfg = {
      tipo: opts.tipo || '',
      searchUrl: opts.searchUrl || '/Lookup/Search',
      createUrl: opts.createUrl || '/Lookup/Create',
      permiteCrear: !!opts.permiteCrear,
      localCreate: !!opts.localCreate,
      createText: typeof opts.createText === 'function' ? opts.createText : null,
      placeholder: opts.placeholder || 'Buscar...',
      minChars: opts.minChars != null ? opts.minChars : 1,
      debounce: opts.debounce != null ? opts.debounce : 200,
      onSelect: typeof opts.onSelect === 'function' ? opts.onSelect : function () {},
      onCreate: typeof opts.onCreate === 'function' ? opts.onCreate : function () {}
    };

    // ---- DOM ----
    var root = h('div', 'ss-root');
    var input = h('input', 'ss-input');
    input.type = 'text';
    input.setAttribute('autocomplete', 'off');
    input.placeholder = cfg.placeholder;
    var panel = h('div', 'ss-panel');
    panel.style.display = 'none';

    root.appendChild(input);
    root.appendChild(panel);
    container.appendChild(root);

    // ---- Estado ----
    var timer = null;
    var items = [];           // resultados actuales del servidor
    var activeIndex = -1;     // índice resaltado (incluye fila "crear")
    var lastTerm = '';
    var selected = null;      // {id, text, meta}

    function buildRows(term) {
      var rows = items.map(function (it) {
        return { kind: 'item', item: it };
      });
      var exact = items.some(function (it) {
        return (it.text || '').toLowerCase() === term.toLowerCase();
      });
      if (cfg.permiteCrear && term && !exact) {
        rows.push({ kind: 'create', term: term });
      }
      return rows;
    }

    function badgeHtml(item) {
      if (item.enStock === true) {
        var s = (item.stock != null && item.stock !== '') ? parseInt(item.stock, 10) : NaN;
        if (isNaN(s)) s = 0;
        var min = (item.stockMin != null && item.stockMin !== '') ? parseInt(item.stockMin, 10) : -1;
        if (isNaN(min)) min = -1; // sin stockMin -> no se evalúa "bajo"
        if (s <= 0) {
          return '<span class="ss-badge ss-badge-out">⚪ Agotado</span>';
        }
        if (min >= 0 && s <= min) {
          return '<span class="ss-badge ss-badge-low">🟠 Quedan ' + escapeHtml(s) + ' (bajo)</span>';
        }
        return '<span class="ss-badge ss-badge-stock">🟢 En inventario · ' + escapeHtml(s) + '</span>';
      }
      if (item.enStock === false && item.meta && /recetad/i.test(item.meta)) {
        return '<span class="ss-badge ss-badge-prev">⚪ Recetado antes</span>';
      }
      return '';
    }

    function render(term) {
      var rows = buildRows(term);
      panel.innerHTML = '';

      if (rows.length === 0) {
        panel.appendChild(h('div', 'ss-empty', 'Sin coincidencias'));
        panel.style.display = 'block';
        return;
      }

      rows.forEach(function (row, idx) {
        var el;
        if (row.kind === 'item') {
          el = h('div', 'ss-item');
          var meta = row.item.meta ? '<div class="ss-item-meta">' + escapeHtml(row.item.meta) + '</div>' : '';
          var badge = badgeHtml(row.item);
          el.innerHTML =
            '<div class="ss-item-main">' +
              '<div class="ss-item-text">' + escapeHtml(row.item.text) + '</div>' + meta +
            '</div>' + badge;
          el.addEventListener('mousedown', function (e) {
            e.preventDefault();
            choose(row.item);
          });
        } else {
          el = h('div', 'ss-create');
          var termEsc = escapeHtml(row.term);
          el.innerHTML = cfg.createText
            ? cfg.createText(termEsc)
            : '<span class="ss-create-plus">＋</span> Crear «' + termEsc + '»';
          el.addEventListener('mousedown', function (e) {
            e.preventDefault();
            doCreate(row.term);
          });
        }
        el.setAttribute('data-idx', idx);
        el.addEventListener('mouseover', function () { setActive(idx); });
        panel.appendChild(el);
      });

      activeIndex = -1;
      panel.style.display = 'block';
    }

    function rowEls() {
      return panel.querySelectorAll('.ss-item, .ss-create');
    }

    function setActive(idx) {
      var els = rowEls();
      activeIndex = idx;
      for (var i = 0; i < els.length; i++) {
        if (i === idx) els[i].classList.add('ss-active');
        else els[i].classList.remove('ss-active');
      }
    }

    function hide() {
      panel.style.display = 'none';
      activeIndex = -1;
    }

    function applyValue(item) {
      selected = item;
      input.value = item.text || '';
      hide();
    }

    function choose(item) {
      applyValue(item);
      cfg.onSelect(item);
    }

    function doCreate(term) {
      // Modo local: no toca el servidor, entrega un item "fuera de catálogo".
      if (cfg.localCreate) {
        var local = { id: 0, text: term, enStock: false, stock: null, meta: null };
        applyValue(local);
        cfg.onCreate(local);
        return;
      }

      var body = 'tipo=' + encodeURIComponent(cfg.tipo) +
                 '&text=' + encodeURIComponent(term) +
                 '&__RequestVerificationToken=' + encodeURIComponent(getAntiForgeryToken());

      panel.innerHTML = '';
      panel.appendChild(h('div', 'ss-empty', 'Creando…'));

      fetch(cfg.createUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
        body: body,
        credentials: 'same-origin'
      })
        .then(function (r) { return r.json().catch(function () { return { success: false, error: 'Respuesta inválida (' + r.status + ')' }; }); })
        .then(function (res) {
          if (res && res.success) {
            var item = {
              id: res.id, text: res.text, meta: res.meta != null ? res.meta : null,
              enStock: res.enStock, stock: res.stock != null ? res.stock : null
            };
            applyValue(item);
            cfg.onCreate(item);
          } else {
            panel.innerHTML = '';
            panel.appendChild(h('div', 'ss-error', (res && res.error) ? res.error : 'No se pudo crear.'));
            panel.style.display = 'block';
          }
        })
        .catch(function () {
          panel.innerHTML = '';
          panel.appendChild(h('div', 'ss-error', 'Error de red al crear.'));
          panel.style.display = 'block';
        });
    }

    function doSearch(term) {
      var url = cfg.searchUrl + '?tipo=' + encodeURIComponent(cfg.tipo) +
                '&term=' + encodeURIComponent(term);
      fetch(url, { credentials: 'same-origin' })
        .then(function (r) { return r.json(); })
        .then(function (data) {
          items = Array.isArray(data) ? data : [];
          if (lastTerm === term && document.activeElement === input) render(term);
        })
        .catch(function () {
          items = [];
          render(term);
        });
    }

    // ---- Eventos ----
    input.addEventListener('input', function () {
      selected = null;
      var term = this.value.trim();
      lastTerm = term;
      clearTimeout(timer);
      if (term.length < cfg.minChars) { hide(); return; }
      timer = setTimeout(function () { doSearch(term); }, cfg.debounce);
    });

    input.addEventListener('focus', function () {
      var term = this.value.trim();
      lastTerm = term;
      if (term.length < cfg.minChars) return;
      // Si ya hay resultados cargados los muestra; si no, los trae para ojear
      // (permite "abrir la lista" sin escribir cuando minChars es 0).
      if (items.length > 0) render(term);
      else doSearch(term);
    });

    input.addEventListener('keydown', function (e) {
      var els = rowEls();
      if (panel.style.display === 'none' || els.length === 0) return;
      if (e.key === 'ArrowDown') {
        e.preventDefault();
        setActive(Math.min(activeIndex + 1, els.length - 1));
      } else if (e.key === 'ArrowUp') {
        e.preventDefault();
        setActive(Math.max(activeIndex - 1, 0));
      } else if (e.key === 'Enter') {
        if (activeIndex >= 0 && els[activeIndex]) {
          e.preventDefault();
          els[activeIndex].dispatchEvent(new MouseEvent('mousedown'));
        }
      } else if (e.key === 'Escape') {
        hide();
      }
    });

    function onDocClick(e) {
      if (!root.contains(e.target)) hide();
    }
    document.addEventListener('click', onDocClick);

    // ---- API pública ----
    return {
      setValue: function (item) {
        selected = item || null;
        input.value = item && item.text ? item.text : '';
      },
      getValue: function () { return selected; },
      clear: function () { selected = null; input.value = ''; items = []; hide(); },
      focus: function () { input.focus(); },
      destroy: function () {
        document.removeEventListener('click', onDocClick);
        if (root.parentNode) root.parentNode.removeChild(root);
      }
    };
  }

  window.SearchSelect = { create: create };
})();
