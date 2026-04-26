
const state = { vacunas: true, tratamientos: true, recetas: true, sinconsulta: true };

function toggleSection(id) {
  state[id] = !state[id];
  document.getElementById('body-' + id).classList.toggle('collapsed', !state[id]);
  document.getElementById('tog-' + id).classList.toggle('open', state[id]);
}

function filterSection(btn, filter) {
  document.querySelectorAll('.filter-pill').forEach(b => b.classList.remove('active'));
  if (btn) btn.classList.add('active');
  else {
    document.querySelectorAll('.filter-pill').forEach(b => {
      if (b.dataset.filter === filter) b.classList.add('active');
    });
  }
  document.querySelectorAll('.acc-section').forEach(sec => {
    if (filter === 'all' || sec.dataset.section === filter) {
      sec.removeAttribute('data-hidden');
    } else {
      sec.setAttribute('data-hidden', 'true');
    }
  });
}

window.addEventListener('load', () => {
  setTimeout(() => {
    document.querySelectorAll('.mini-fill[data-w]').forEach(bar => {
      bar.style.width = bar.dataset.w;
    });
  }, 500);
});

document.querySelectorAll('.filter-pill').forEach(btn => {
  btn.addEventListener('click', () => filterSection(btn, btn.dataset.filter));
});
