/*
 * consulta-firma.js — Firmar la nota clinica + addenda (correcciones).
 *
 * Firmar = bloquear el texto de la nota (SOAP + diagnostico) de una visita.
 * Es distinto de "Finalizar consulta" (que cierra el estado de la cita).
 * Las correcciones posteriores se agregan como addenda (append-only): nunca
 * se edita ni borra el texto original.
 *
 * Usa window._urls (firmar, agregarAddenda, addenda) y los helpers globales
 * window.postForm / window.getJson / window.Swal.
 */
(function () {
  var urls = window._urls || {};

  // Id de la visita en curso (viene de la sala de espera). Sin visita, nada que firmar.
  var citaId = window._citaId || 0;
  if (!citaId) {
    try {
      var q = new URLSearchParams(window.location.search).get('citaId');
      if (q) citaId = parseInt(q, 10) || 0;
    } catch (e) {}
  }
  if (!citaId) return;

  var card = document.getElementById('ecFirmaCard');
  if (!card) return;

  // ── Campos de la nota clinica que se bloquean cuando esta firmada ──
  function notaFields() {
    return [
      'ecTipoConsulta', 'ecMotivo', 'ecPeso', 'ecTemperatura', 'ecFC', 'ecFR',
      'ecMucosas', 'ecLlenadoCapilar', 'ecEscalaDolor', 'ecCondicionCorporal',
      'ecDiagnostico'
    ].map(function (id) { return document.getElementById(id); })
     .filter(function (el) { return !!el; });
  }

  // Deja la nota de solo-lectura (visual) cuando esta firmada.
  function lockNota() {
    notaFields().forEach(function (el) {
      el.setAttribute('disabled', 'disabled');
      el.classList.add('ec-locked');
    });
    // Finalizar reescribe la nota: se desactiva sobre una nota firmada.
    var fin = document.getElementById('btnFinalizar');
    if (fin) {
      fin.setAttribute('disabled', 'disabled');
      fin.classList.add('ec-locked');
      fin.title = 'La nota esta firmada (bloqueada).';
    }
  }

  if (window._notaFirmada === true) lockNota();

  function notify(ok, msg) {
    if (window.Swal) {
      Swal.fire({
        icon: ok ? 'success' : 'error',
        title: ok ? 'Listo' : 'Atencion',
        text: msg || '',
        timer: ok ? 1800 : undefined,
        showConfirmButton: !ok
      });
    } else {
      alert(msg || (ok ? 'Listo' : 'Error'));
    }
  }

  // ───────────────────────── Firmar nota ─────────────────────────
  var btnFirmar = document.getElementById('btnFirmarNota');
  if (btnFirmar) {
    btnFirmar.addEventListener('click', function () {
      var doFirmar = function () {
        btnFirmar.setAttribute('disabled', 'disabled');
        window.postForm(urls.firmar, { citaId: citaId })
          .then(function (res) {
            if (res && res.ok) {
              // Recargar para que el servidor pinte el estado bloqueado + badge.
              if (window.Swal) {
                Swal.fire({ icon: 'success', title: 'Nota firmada', text: res.mensaje || '', timer: 1500, showConfirmButton: false })
                  .then(function () { window.location.reload(); });
              } else {
                window.location.reload();
              }
            } else {
              btnFirmar.removeAttribute('disabled');
              notify(false, (res && res.mensaje) || 'No se pudo firmar la nota.');
            }
          })
          .catch(function () {
            btnFirmar.removeAttribute('disabled');
            notify(false, 'Error de conexion.');
          });
      };

      if (window.Swal) {
        Swal.fire({
          icon: 'warning',
          title: '¿Firmar la nota?',
          html: 'Despues de firmar, el texto de la nota clinica queda <b>bloqueado</b>.<br>Las correcciones solo podran agregarse como addenda.',
          showCancelButton: true,
          confirmButtonText: 'Si, firmar',
          cancelButtonText: 'Cancelar'
        }).then(function (r) { if (r.isConfirmed) doFirmar(); });
      } else if (confirm('¿Firmar la nota? Quedara bloqueada; solo se podra corregir con addenda.')) {
        doFirmar();
      }
    });
  }

  // ───────────────────────── Agregar addenda ─────────────────────────
  var btnAddenda = document.getElementById('btnAgregarAddenda');
  var txtAddenda = document.getElementById('ecAddendaTexto');

  function pintarAddendas(items) {
    var cont = document.getElementById('ecAddendaList');
    if (!cont) return;
    if (!items || !items.length) {
      cont.innerHTML = '<div class="ec-empty-line" id="ecAddendaEmpty">Sin correcciones registradas.</div>';
      return;
    }
    var html = items.map(function (a) {
      return '<div class="ec-addenda-item">' +
               '<div class="ec-addenda-meta"><i class="bi bi-arrow-return-right"></i> ' +
                 escapeHtml(a.fecha) + ' · ' + escapeHtml(a.autor) +
               '</div>' +
               '<div class="ec-addenda-texto">' + escapeHtml(a.texto) + '</div>' +
             '</div>';
    }).join('');
    cont.innerHTML = html;
  }

  function escapeHtml(s) {
    return String(s == null ? '' : s)
      .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
  }

  function refrescarAddendas() {
    if (!urls.addenda) return;
    window.getJson(urls.addenda + '?citaId=' + citaId)
      .then(function (items) { pintarAddendas(items); })
      .catch(function () {});
  }

  if (btnAddenda && txtAddenda) {
    btnAddenda.addEventListener('click', function () {
      var texto = (txtAddenda.value || '').trim();
      if (!texto) { notify(false, 'Escribe la correccion antes de agregarla.'); return; }

      btnAddenda.setAttribute('disabled', 'disabled');
      window.postForm(urls.agregarAddenda, { citaId: citaId, texto: texto })
        .then(function (res) {
          btnAddenda.removeAttribute('disabled');
          if (res && res.ok) {
            txtAddenda.value = '';
            refrescarAddendas();
            notify(true, res.mensaje || 'Correccion agregada.');
          } else {
            notify(false, (res && res.mensaje) || 'No se pudo agregar la correccion.');
          }
        })
        .catch(function () {
          btnAddenda.removeAttribute('disabled');
          notify(false, 'Error de conexion.');
        });
    });
  }
})();
